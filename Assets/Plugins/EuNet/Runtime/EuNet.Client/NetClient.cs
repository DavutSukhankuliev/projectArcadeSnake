﻿using EuNet.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EuNet.Client
{
    /// <summary>
    /// 클라이언트 클래스. 서버에 접속하여 데이터를 주고 받는 용도
    /// </summary>
    public class NetClient : IClient, ISession
    {
        /// <summary>
        /// 서버로부터 받은 세션의 고유 아이디 (연결된 서버의 세션아이디와 동일)
        /// </summary>
        public ushort SessionId { get; protected set; }

        /// <summary>
        /// 세션 상태
        /// </summary>
        public SessionState State { get; private set; } = SessionState.Closed;

        /// <summary>
        /// 서버와 UDP 연결이 완료되었는지 여부
        /// </summary>
        public bool IsUdpConnected => _isUdpConnected;

        protected readonly ILogger _logger;

        private TcpChannel _tcpChannel;
        public TcpChannel TcpChannel => _tcpChannel;

        private UdpChannel _udpChannel;
        public UdpChannel UdpChannel => _udpChannel;

        /// <summary>
        /// 연결됨 콜백
        /// </summary>
        public Action OnConnected;

        /// <summary>
        /// 연결해제됨 콜백
        /// </summary>
        public Action OnClosed;

        /// <summary>
        /// 데이터를 받음 콜백
        /// </summary>
        public Func<NetDataReader, Task> OnReceived;

        /// <summary>
        /// 요청을 받음 콜백
        /// </summary>
        public Func<ISession, NetDataReader, NetDataWriter, Task> OnRequestReceived;

        /// <summary>
        /// View 요청을 받은 콜백
        /// </summary>
        public Func<ISession, NetDataReader, NetDataWriter, Task> OnViewRequestReceived;

        /// <summary>
        /// P2P 데이터를 받음 콜백
        /// </summary>
        public Func<ISession, NetDataReader, Task> OnP2pReceived;

        /// <summary>
        /// 에러 발생 콜백
        /// </summary>
        public Action<Exception> OnErrored;

        /// <summary>
        /// P2P 그룹에 가입됨 콜백. <c>Action(세션아이디, 본인여부)</c>
        /// </summary>
        public Action<ushort, bool> OnP2pGroupJoined;

        /// <summary>
        /// P2P 그룹에서 떠남 콜백. <c>Action(세션아이디, 본인여부)</c>
        /// </summary>
        public Action<ushort, bool> OnP2pGroupLeaved;

        private ILoggerFactory _loggerFactory;
        public ILoggerFactory LoggerFactory => _loggerFactory;

        private Socket _socket;
        private ClientOption _clientOption;
        public ClientOption ClientOption => _clientOption;
        private long _connectId;

        /// <summary>
        /// 서버로 연결할 TCP 주소
        /// </summary>
        private IPEndPoint _serverEndPoint;

        /// <summary>
        /// 서버의 UDP에 처음 연결 패킷을 보낼 주소 
        /// </summary>
        private IPEndPoint _serverUdpEndPoint;
        private ConcurrentQueue<NetPacket> _receivedPacketQueue;
        private NetDataReader _packetReader;
        private UdpSocketEx _udpSocket;
        public UdpSocketEx UdpSocket => _udpSocket;
        public readonly object UdpSocketReceiveRawAsyncObject = new object();

        private bool _isUdpConnected;
        private Task _connectUdpLoopTask;
        private CancellationTokenSource _cts;
        private volatile bool _isPossibleUpdate;
        private P2pGroup _p2pGroup;
        public P2pGroup P2pGroup => _p2pGroup;

        private SessionRequest _request;
        public SessionRequest SessionRequest => _request;

        private readonly NetStatistic _statistic;
        public NetStatistic Statistic => _statistic;

        protected List<IRpcInvokable> _rpcHandlers;

        public NetClient(ClientOption clientOption, ILoggerFactory loggerFactory = null)
        {
            _clientOption = clientOption;
            //_clientOptions.PacketFilter = _clientOptions.PacketFilter ?? new XorPacketFilter();

            _loggerFactory = loggerFactory ?? DefaultLoggerFactory.Create(builder => { builder.AddConsoleLogger(); });

            _logger = _loggerFactory.CreateLogger(nameof(NetClient));
            _receivedPacketQueue = new ConcurrentQueue<NetPacket>();
            _packetReader = new NetDataReader();

            _statistic = new NetStatistic();

            _tcpChannel = new TcpChannel(
                _clientOption,
                _loggerFactory.CreateLogger(nameof(TcpChannel)),
                _statistic);

            if (_clientOption.IsServiceUdp == true)
            {
                _udpChannel = new UdpChannel(
                    _clientOption,
                    _loggerFactory.CreateLogger(nameof(UdpChannel)),
                    _statistic,
                    0);
            }

            _request = new SessionRequest(this, _statistic);

            _rpcHandlers = new List<IRpcInvokable>();
        }

        /// <summary>
        /// 사용되지 않음
        /// </summary>
        /// <param name="info"></param>
        public void Init(SessionInitializeInfo info)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// RPC Service를 등록합니다.
        /// </summary>
        /// <param name="service"></param>
        public void AddRpcService(IRpcInvokable service)
        {
            if (State != SessionState.Closed)
                throw new Exception("Only possible when the client is stopped");

            if (_rpcHandlers.Contains(service))
                throw new Exception("Already exist IRpcInvokable in _rpcHandlers");

            _rpcHandlers.Add(service);
        }

        /// <summary>
        /// 서버로 연결을 시도합니다
        /// </summary>
        /// <param name="timeout">타임아웃</param>
        /// <returns>성공여부</returns>
        public async Task<bool> ConnectAsync(TimeSpan? timeout = null)
        {
            if (_socket != null || State != SessionState.Closed)
                return false;

            try
            {
                State = SessionState.Initialized;
                TimeSpan currentTimeout = timeout ?? TimeSpan.FromSeconds(10);

                _cts = new CancellationTokenSource();

                _serverEndPoint = NetUtil.GetEndPoint(_clientOption.TcpServerAddress, _clientOption.TcpServerPort);
                _serverUdpEndPoint = NetUtil.GetEndPoint(_clientOption.UdpServerAddress, _clientOption.UdpServerPort);

                if (_clientOption.IsServiceUdp)
                {
                    _udpSocket = new UdpSocketEx(
                        _loggerFactory.CreateLogger("ClientUdpSocket"),
                        OnPreProcessUdpRawData,
                        UdpSocketReceiveRawAsyncObject);
                    _udpSocket.CreateClient();
                }

                _isUdpConnected = false;

                _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                await _socket.ConnectAsync(_serverEndPoint).TimeoutAfter(currentTimeout);

                if (_socket.Connected == false)
                {
                    _logger.LogInformation("Fail to connect");

                    State = SessionState.Closed;
                    _socket.Close();
                    _socket = null;
                    return false;
                }

                if (_tcpChannel != null)
                {
                    _tcpChannel.Init(_cts);
                    _tcpChannel.PacketReceived = OnReceiveFromChannel;
                    _tcpChannel.SetSocket(_socket);
                }

                if (_udpChannel != null)
                {
                    _udpChannel.Init(_cts);
                    _udpChannel.PacketReceived = OnReceiveFromChannel;

                    if (_udpSocket != null)
                    {
                        _udpChannel.SetSocket(_udpSocket);
                        _udpChannel.LocalEndPoint = _udpSocket.GetLocalEndPoint();
                    }
                }

                RunAsync().DoNotAwait();

                if (_clientOption.IsServiceUdp)
                {
                    currentTimeout = TimeSpan.FromSeconds(10);
                    while (IsUdpConnected == false && State == SessionState.Initialized)
                    {
                        await Task.Delay(100);
                        currentTimeout -= TimeSpan.FromMilliseconds(100);

                        if (currentTimeout < TimeSpan.FromMilliseconds(0))
                            throw new Exception("udp connection timeout");
                    }
                }

                OnSessionConnected();

                _logger.LogInformation("Connect succeed");

                return true;
            }
            catch (Exception ex)
            {
                OnError(ex);
                Close();
            }

            State = SessionState.Closed;
            return false;
        }

        /// <summary>
        /// 주기적인 업데이트 호출
        /// 외부에서 주기적으로 (ex.30ms) 호출하여 내부로직을 처리해야 함
        /// </summary>
        /// <param name="elapsedTime">기존 업데이트로부터 지난 시간. 밀리세컨드(ms)</param>
        public void Update(int elapsedTime)
        {
            if (_isPossibleUpdate == false)
                return;

            try
            {
                if (_tcpChannel?.Update(elapsedTime) == false)
                    throw new Exception("Disconnected due to TCP timeout");

                if (_udpChannel?.Update(elapsedTime) == false)
                    throw new Exception("Disconnected due to RUDP timeout");

                _p2pGroup?.Update(elapsedTime);

                NetPacket packet;
                while (_receivedPacketQueue.TryDequeue(out packet) == true &&
                    packet != null)
                {
                    try
                    {
                        int headerSize = NetPacket.GetHeaderSize(packet.Property);
                        _packetReader.SetSource(packet.RawData, headerSize, packet.Size);

                        if (OnPreProcessPacket(packet, _packetReader) == true)
                            continue;

                        switch (packet.Property)
                        {
                            case PacketProperty.ResponseConnection:
                                {
                                    SessionId = packet.SessionIdForConnection;
                                    _connectId = _packetReader.ReadInt64();

                                    if (_clientOption.IsServiceUdp &&
                                        _connectUdpLoopTask == null)
                                    {
                                        _connectUdpLoopTask = ConnectUdpLoopAsync(SessionId);
                                    }
                                }
                                break;
                            default:
                                {
                                    OnReceive(_packetReader).Wait();
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError(ex);
                        break;
                    }
                    finally
                    {
                        NetPool.PacketPool.Free(packet);
                    }
                }
            }
            catch (Exception ex)
            {
                _isPossibleUpdate = false;
                OnError(ex);
                Close();
            }
        }

        private async Task RunAsync()
        {
            _isPossibleUpdate = true;

            try
            {
                await Task.Delay(-1, _cts.Token);
            }
            catch
            {

            }

            _isPossibleUpdate = false;
            Close();

            try
            {
                _tcpChannel?.OnClosed();
                _udpChannel?.OnClosed();
            }
            catch
            {

            }

            _cts?.Dispose();
            _cts = null;

        }

        /// <summary>
        /// 에러가 발생되었음
        /// </summary>
        /// <param name="exception">예외</param>
        public void OnError(Exception exception)
        {
            OnErrored?.Invoke(exception);
        }

        /// <summary>
        /// 서버와 접속을 해제하고 리소스를 해제한다
        /// </summary>
        public void Disconnect()
        {
            Close();
        }

        /// <summary>
        /// 서버와 접속을 해제하고 리소스를 해제한다
        /// </summary>
        public void Close()
        {
            OnSessionClosed();

            try
            {
                _cts.Cancel();
            }
            catch
            {

            }

            NetPacket poolingPacket = null;
            while (_receivedPacketQueue.TryDequeue(out poolingPacket) == true &&
                poolingPacket != null)
            {
                NetPool.PacketPool.Free(poolingPacket);
            }

            try
            {
                _tcpChannel?.Close();
                _udpChannel?.Close();

                _p2pGroup?.Close();
                _p2pGroup = null;

                if (_connectUdpLoopTask != null)
                    _connectUdpLoopTask = null;

                _udpSocket?.Close(false);
                _udpSocket = null;

                _request.Close();
            }
            catch
            {

            }

            _socket = null;
            _isUdpConnected = false;
        }

        internal void OnSessionConnected()
        {
            try
            {
                State = SessionState.Connected;
                OnConnected?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        internal bool OnSessionClosed()
        {
            try
            {
                if (State != SessionState.Connected)
                    return false;

                State = SessionState.Closed;

                OnClosed?.Invoke();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            
            return true;
        }

        private IChannel GetChannel(DeliveryMethod deliveryMethod)
        {
            if (deliveryMethod == DeliveryMethod.Tcp)
                return _tcpChannel;

            return _udpChannel;
        }

        /// <summary>
        /// 데이터를 전송함
        /// </summary>
        /// <param name="data">보낼 데이터 버퍼</param>
        /// <param name="offset">보낼 데이터 버퍼 오프셋</param>
        /// <param name="length">보낼 데이터 길이</param>
        /// <param name="deliveryMethod">전송 방법</param>
        public void SendAsync(byte[] data, int offset, int length, DeliveryMethod deliveryMethod)
        {
            IChannel channel = GetChannel(deliveryMethod);
            if (channel == null)
                throw new Exception($"can not found channel : {deliveryMethod}");

            PacketProperty property = PacketProperty.UserData;
            int headerSize = NetPacket.GetHeaderSize(property);

            NetPacket packet = NetPool.PacketPool.Alloc(headerSize + length);
            packet.Property = property;
            packet.DeliveryMethod = deliveryMethod;

            Buffer.BlockCopy(data, offset, packet.RawData, headerSize, length);

            channel.SendAsync(packet);
        }

        /// <summary>
        /// 데이터를 전송함
        /// </summary>
        /// <param name="dataWriter">보낼 데이터를 가지고 있는 NetDataWriter</param>
        /// <param name="deliveryMethod">전송 방법</param>
        public void SendAsync(NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
        {
            SendAsync(dataWriter.Data, 0, dataWriter.Length, deliveryMethod);
        }

        /// <summary>
        /// 요청을 보내고 답을 기다립니다.
        /// </summary>
        /// <param name="data">보낼 데이터 버퍼</param>
        /// <param name="offset">보낼 데이터 버퍼 오프셋</param>
        /// <param name="length">보낼 데이터 길이</param>
        /// <param name="deliveryMethod">전송 방법</param>
        /// <param name="timeout">답을 기다리는 시간</param>
        /// <returns>요청에 대한 답 (데이터)</returns>
        public Task<NetDataBufferReader> RequestAsync(byte[] data, int offset, int length, DeliveryMethod deliveryMethod, TimeSpan? timeout)
        {
            return _request.RequestAsync(data, offset, length, deliveryMethod, timeout);
        }

        /// <summary>
        /// 요청을 보내고 답을 기다립니다.
        /// </summary>
        /// <param name="dataWriter">보낼 데이터를 가지고 있는 NetDataWriter</param>
        /// <param name="deliveryMethod">전송 방법</param>
        /// <param name="timeout">답을 기다리는 시간</param>
        /// <returns>요청에 대한 답 (데이터)</returns>
        public Task<NetDataBufferReader> RequestAsync(NetDataWriter dataWriter, DeliveryMethod deliveryMethod, TimeSpan? timeout)
        {
            return RequestAsync(dataWriter.Data, 0, dataWriter.Length, deliveryMethod, timeout);
        }

        /// <summary>
        /// 패킷을 저수준에서 그대로 전송. 내부에서만 사용.
        /// </summary>
        /// <param name="poolingPacket">보낼패킷. NetPool.PacketPool.Alloc 으로 할당하여 사용하세요</param>
        /// <param name="deliveryMethod">전송 방법</param>
        public void SendRawAsync(NetPacket poolingPacket, DeliveryMethod deliveryMethod)
        {
            IChannel channel = GetChannel(deliveryMethod);
            if (channel == null)
                throw new Exception($"can not found channel : {deliveryMethod}");

            poolingPacket.DeliveryMethod = deliveryMethod;

            channel.SendAsync(poolingPacket);
        }

        private void OnReceiveFromChannel(IChannel ch, NetPacket poolingPacket)
        {
            if (State == SessionState.Closed)
            {
                NetPool.PacketPool.Free(poolingPacket);
                return;
            }

            _receivedPacketQueue.Enqueue(poolingPacket);
        }

        private bool OnPreProcessPacket(NetPacket poolingPacket, NetDataReader reader)
        {
            try
            {
                switch (poolingPacket.Property)
                {
                    case PacketProperty.AliveCheck:
                        {
                            byte type = poolingPacket.RawData[NetPacket.HeaderSize];
                            //_logger.LogInformation($"Received PacketProperty.AliveCheck Type[{type}]");

                            if (type == 0xFF)
                            {
                                var packet = NetPool.PacketPool.Alloc(PacketProperty.AliveCheck);
                                packet.RawData[NetPacket.HeaderSize] = 0;
                                TcpChannel.SendAsync(packet);
                            }
                        }
                        break;
                    case PacketProperty.JoinP2p:
                        {
                            ushort groupId = reader.ReadUInt16();
                            ushort sessionId = reader.ReadUInt16();
                            ushort masterSessionId = reader.ReadUInt16();
                            IPEndPoint remoteEndPoint = reader.ReadIPEndPoint();
                            IPEndPoint localEndPoint = reader.ReadIPEndPoint();

                            if (SessionId == sessionId)
                            {
                                // 같다면 P2P그룹을 생성한다.
                                if (_p2pGroup != null)
                                    throw new Exception("already joined p2p group");

                                _udpChannel.RemoteEndPoint = remoteEndPoint;
                                _udpChannel.TempEndPoint = remoteEndPoint;

                                _p2pGroup = new P2pGroup(this, groupId, masterSessionId);

                                // 자신을 참가 시킨다
                                var member = _p2pGroup.Join(this, masterSessionId);

                                OnP2pGroupJoined?.Invoke(sessionId, true);

                                //NetDebug.Log(string.Format("Create P2P Group {0} {1} {2} {3}", groupSessionKey, sessionKey, masterSessionKey, remoteEndPoint));
                            }
                            else
                            {
                                if (_p2pGroup == null)
                                    throw new Exception("not exist join group");

                                var p2pSession = new P2pSession(this, sessionId, remoteEndPoint, localEndPoint);
                                p2pSession.OnViewRequestReceived = OnViewRequestReceive;

                                // 다르다면 조인시킨다
                                var member = _p2pGroup.Join(p2pSession, masterSessionId);

                                OnP2pGroupJoined?.Invoke(sessionId, false);

                                //NetDebug.Log(string.Format("Join P2P Group {0} {1} {2} {3}", groupSessionKey, sessionKey, masterSessionKey, remoteEndPoint));
                            }
                        }
                        break;
                    case PacketProperty.LeaveP2p:
                        {
                            ushort groupId = reader.ReadUInt16();
                            ushort sessionId = reader.ReadUInt16();
                            ushort masterSessionId = reader.ReadUInt16();

                            if (SessionId == sessionId)
                            {
                                // 같다면 P2P그룹을 파괴한다.
                                _p2pGroup?.Close();
                                _p2pGroup = null;

                                OnP2pGroupLeaved?.Invoke(sessionId, true);

                                _logger.LogInformation($"Destroy P2p {groupId} {sessionId}");
                            }
                            else
                            {
                                // 다르다면 떠나보낸다
                                var member = _p2pGroup.Leave(sessionId, masterSessionId);
                                if (member != null)
                                {
                                    _udpSocket.RemoveSession(member.Session, true);
                                    member.Close();
                                    OnP2pGroupLeaved?.Invoke(sessionId, false);
                                }

                                _logger.LogInformation($"Leave P2p {groupId} {sessionId} {masterSessionId}");
                            }
                        }
                        break;
                    case PacketProperty.ChangeP2pMaster:
                        {

                        }
                        break;
                    case PacketProperty.Request:
                        {
                            _request.OnReceive(poolingPacket.Property, poolingPacket.DeliveryMethod, reader, OnRequestReceive).Wait();
                        }
                        break;
                    case PacketProperty.ViewRequest:
                        {
                            _request.OnReceive(poolingPacket.Property, poolingPacket.DeliveryMethod, reader, OnViewRequestReceive).Wait();
                        }
                        break;
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                OnErrored?.Invoke(ex);
                return true;
            }

            return true;
        }

        internal async Task OnRequestReceive(ISession session, NetDataReader reader, NetDataWriter writer)
        {
            var preReaderPos = reader.Position;
            var preWriterPos = writer.Length;

            foreach (var handler in _rpcHandlers)
            {
                reader.Position = preReaderPos;
                writer.Length = preWriterPos;

                var result = await handler.Invoke(session, reader, writer);
                if (result == true)
                    return;
            }

            reader.Position = preReaderPos;
            writer.Length = preWriterPos;

            await OnRequestReceived(session, reader, writer);
        }

        internal Task OnViewRequestReceive(ISession session, NetDataReader reader, NetDataWriter writer)
        {
            return OnViewRequestReceived(session, reader, writer);
        }

        /// <summary>
        /// 데이터를 받음. 데이터 처리가 끝날때까지 기다릴 수 있는 비동기 메서드
        /// </summary>
        /// <param name="dataReader">받은 데이터</param>
        public Task OnReceive(NetDataReader dataReader)
        {
            return OnReceived(dataReader);
        }

        private ISession FindSession(ushort sessionId)
        {
            if (sessionId == 0)
                return this;

            if (_p2pGroup == null)
                return null;

            var member = _p2pGroup.Find(sessionId);
            if (member == null)
                return null;

            return member.Session;
        }

        private async Task ConnectUdpLoopAsync(ushort sessionId)
        {
            IPEndPoint localEp = _udpSocket.GetLocalEndPoint();

            var writer = NetPool.DataWriterPool.Alloc();
            try
            {
                writer.Write(_connectId);
                writer.Write(localEp);

                var packet = NetPool.PacketPool.Alloc(PacketProperty.RequestConnection, writer);
                try
                {
                    packet.DeliveryMethod = DeliveryMethod.Unreliable;
                    packet.SessionIdForConnection = sessionId;

                    SocketError error = SocketError.Success;

                    while (State == SessionState.Initialized &&
                        IsUdpConnected == false)
                    {
                        _udpSocket.SendTo(packet.RawData, 0, packet.Size, _serverUdpEndPoint, ref error);
                        await Task.Delay(100);
                    }
                }
                finally
                {
                    NetPool.PacketPool.Free(packet);
                }
            }
            finally
            {
                NetPool.DataWriterPool.Free(writer);
            }
        }

        internal bool OnPreProcessUdpRawData(byte[] data, int size, NetPacket cachedPacket, IPEndPoint endPoint)
        {
            Interlocked.Increment(ref _statistic.UdpReceivedCount);
            Interlocked.Add(ref _statistic.UdpReceivedBytes, size);

            try
            {
                switch (cachedPacket.Property)
                {
                    case PacketProperty.RequestConnection:
                        {

                        }
                        break;
                    case PacketProperty.ResponseConnection:
                        {
                            ushort sessionId = cachedPacket.SessionIdForConnection;
                            var session = FindSession(sessionId);

                            if (session != null && session.UdpChannel != null)
                            {
                                if (session.UdpChannel.SetPunchedEndPoint(endPoint) == true)
                                    _udpSocket.AddSession(session);

                                if (session == this)
                                {
                                    // 서버응답이라면 특수처리를 해주자
                                    // 더이상 연결메시지를 보내지 않아도 됨
                                    // 연결이 완료되었음을 알림
                                    _isUdpConnected = true;
                                }
                            }
                        }
                        break;
                    case PacketProperty.HolePunchingStart:
                        {
                            ushort sessionId = cachedPacket.SessionIdForConnection;
                            var session = FindSession(sessionId);

                            if (session != null && session.UdpChannel != null)
                            {
                                NetDataReader reader = new NetDataReader(cachedPacket);
                                IPEndPoint ep = reader.ReadIPEndPoint();

                                session.UdpChannel.TempEndPoint = endPoint;

                                ///////////////////////////////////////////////////////////////////////////////////////////////

                                var writer = NetPool.DataWriterPool.Alloc();
                                try
                                {
                                    writer.Write(ep);

                                    NetPacket sendPacket = NetPool.PacketPool.Alloc(PacketProperty.HolePunchingEnd, writer);

                                    try
                                    {
                                        sendPacket.SessionIdForConnection = SessionId;
                                        sendPacket.DeliveryMethod = DeliveryMethod.Unreliable;
                                        SocketError error = SocketError.Success;

                                        UdpSocket.SendTo(sendPacket.RawData, 0, sendPacket.Size, endPoint, ref error);

                                        var ep1 = session.UdpChannel.LocalEndPoint;
                                        var ep2 = session.UdpChannel.RemoteEndPoint;

                                        if (endPoint.Equals(ep1) == false)
                                            UdpSocket.SendTo(sendPacket.RawData, 0, sendPacket.Size, ep1, ref error);

                                        if (endPoint.Equals(ep2) == false && ep1.Equals(ep2) == false)
                                            UdpSocket.SendTo(sendPacket.RawData, 0, sendPacket.Size, ep2, ref error);
                                    }
                                    finally
                                    {
                                        NetPool.PacketPool.Free(sendPacket);
                                    }
                                }
                                finally
                                {
                                    NetPool.DataWriterPool.Free(writer);
                                }
                            }
                        }
                        break;
                    case PacketProperty.HolePunchingEnd:
                        {
                            ushort sessionId = cachedPacket.SessionIdForConnection;

                            if (_p2pGroup != null)
                            {
                                var member = _p2pGroup.Find(sessionId);
                                if (member != null)
                                {
                                    NetDataReader reader = new NetDataReader(cachedPacket);
                                    IPEndPoint ep = reader.ReadIPEndPoint();

                                    member.Session.UdpChannel.SetPunchedEndPoint(ep, true);

                                    // 메인소켓에 등록해줌
                                    _udpSocket.AddSession(member.Session, endPoint);
                                    // 피어별 소켓에도 등록해줌
                                    (member.Session.UdpChannel.Socket as UdpSocketEx)?.AddSession(member.Session, endPoint);

                                    member.SetState(P2pConnectState.BothConnected);
                                }
                            }
                        }
                        break;
                    case PacketProperty.UserData:
                    case PacketProperty.Ack:
                    case PacketProperty.ViewRequest:
                        {
                            if (_serverUdpEndPoint.Equals(endPoint) == false)
                                return false;

                            // 릴레이를 통해서 온 패킷
                            ushort sessionId = cachedPacket.P2pSessionId;
                            if (sessionId == 0)
                                return false;

                            if (sessionId == SessionId)
                                return true;

                            if (_p2pGroup != null)
                            {
                                var member = _p2pGroup.Find(sessionId);
                                if (member != null &&
                                    member.Session != null &&
                                    member.Session.UdpChannel != null)
                                {
                                    SocketError error = SocketError.Success;
                                    member.Session.UdpChannel.OnReceivedRawUdpData(data, size, cachedPacket, error, endPoint);
                                }
                            }
                        }
                        break;
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
                return true;
            }

            return true;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
