﻿using EuNet.Core;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EuNet.Client
{
    /// <summary>
    /// P2P 멤버에 사용되는 다른 유저에게 데이터를 전송하기 위한 세션
    /// </summary>
    public class P2pSession : ISession
    {
        private readonly NetClient _netClient;

        private ushort _sessionId;
        public ushort SessionId => _sessionId;

        private TcpChannel _tcpChannel;
        public TcpChannel TcpChannel => _tcpChannel;

        private UdpChannel _udpChannel;
        public UdpChannel UdpChannel => _udpChannel;

        private SessionState _state;
        public SessionState State => _state;

        private CancellationTokenSource _cts;
        private ConcurrentQueue<NetPacket> _receivedPacketQueue;
        private NetDataReader _packetReader;
        private UdpSocketEx _udpSocket;

        private SessionRequest _request;
        public SessionRequest SessionRequest => _request;

        public Func<ISession, NetDataReader, NetDataWriter, Task> OnViewRequestReceived;

        public P2pSession(NetClient netClient, ushort sessionId, IPEndPoint remoteEp, IPEndPoint localEp)
        {
            _netClient = netClient;
            _sessionId = sessionId;

            _cts = new CancellationTokenSource();

            _udpSocket = new UdpSocketEx(
                netClient.LoggerFactory.CreateLogger("P2pSessionUdpSocket"),
                netClient.OnPreProcessUdpRawData,
                netClient.UdpSocketReceiveRawAsyncObject);

            _udpSocket.CreateClient();

            _tcpChannel = null;

            _udpChannel = new UdpChannel(
                _netClient.ClientOption,
                _netClient.LoggerFactory.CreateLogger(nameof(UdpChannel)),
                _netClient.Statistic,
                sessionId);

            _udpChannel.Init(_cts);
            _udpChannel.PacketReceived = OnReceiveFromChannel;
            _udpChannel.SetSocket(netClient.UdpSocket);

            _udpChannel.RemoteEndPoint = remoteEp;
            _udpChannel.LocalEndPoint = localEp;
            _udpChannel.TempEndPoint = remoteEp;
            _udpChannel.RelayEndPoint = NetUtil.GetEndPoint(netClient.ClientOption.UdpServerAddress, netClient.ClientOption.UdpServerPort);

            _udpChannel.IsRunMtu = false;
            _udpChannel.IsRunPing = false;

            _receivedPacketQueue = new ConcurrentQueue<NetPacket>();
            _packetReader = new NetDataReader();
            _request = new SessionRequest(this, netClient.Statistic);

            _state = SessionState.Connected;
        }

        public void Init(SessionInitializeInfo info)
        {

        }

        public void Close()
        {
            _cts.Cancel();

            NetPacket poolingPacket;
            while (_receivedPacketQueue.TryDequeue(out poolingPacket) == true &&
                poolingPacket != null)
            {
                NetPool.PacketPool.Free(poolingPacket);
            }

            try
            {
                _udpChannel?.Close();
                _udpChannel?.OnClosed();

                _udpSocket?.Close(false);
                _udpSocket = null;

                _request.Close();
            }
            catch
            {

            }
        }

        public void OnError(Exception exception)
        {
            _netClient.OnError(exception);
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

        public Task OnReceive(NetDataReader dataReader)
        {
            return _netClient.OnP2pReceived?.Invoke(this, dataReader);
        }

        public void SendAsync(byte[] data, int offset, int length, DeliveryMethod deliveryMethod)
        {
            PacketProperty property = PacketProperty.UserData;
            int headerSize = NetPacket.GetHeaderSize(property);

            NetPacket packet = NetPool.PacketPool.Alloc(headerSize + length);
            packet.Property = property;
            packet.DeliveryMethod = deliveryMethod;

            Buffer.BlockCopy(data, offset, packet.RawData, headerSize, length);

            UdpChannel.SendAsync(packet);
        }

        public void SendAsync(NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
        {
            SendAsync(dataWriter.Data, 0, dataWriter.Length, deliveryMethod);
        }

        public Task<NetDataBufferReader> RequestAsync(byte[] data, int offset, int length, DeliveryMethod deliveryMethod, TimeSpan? timeout)
        {
            return _request.RequestAsync(data, offset, length, deliveryMethod, timeout);
        }

        public Task<NetDataBufferReader> RequestAsync(NetDataWriter dataWriter, DeliveryMethod deliveryMethod, TimeSpan? timeout)
        {
            return RequestAsync(dataWriter.Data, 0, dataWriter.Length, deliveryMethod, timeout);
        }

        public void SendRawAsync(NetPacket poolingPacket, DeliveryMethod deliveryMethod)
        {
            poolingPacket.DeliveryMethod = deliveryMethod;
            UdpChannel.SendAsync(poolingPacket);
        }

        public void Update(int elapsedTime)
        {
            try
            {
                _udpChannel?.Update(elapsedTime);

                NetPacket packet;
                while (_receivedPacketQueue.TryDequeue(out packet) == true &&
                    packet != null)
                {
                    try
                    {
                        int headerSize = NetPacket.GetHeaderSize(packet.Property);
                        _packetReader.SetSource(packet.RawData, headerSize, packet.Size);

                        if(packet.Property == PacketProperty.Request)
                        {
                            _request.OnReceive(packet.Property, packet.DeliveryMethod, _packetReader, _netClient.OnRequestReceive).Wait();
                        }
                        else if (packet.Property == PacketProperty.ViewRequest)
                        {
                            _request.OnReceive(packet.Property, packet.DeliveryMethod, _packetReader, OnViewRequestReceived).Wait();
                        }
                        else
                        {
                            OnReceive(_packetReader).Wait();
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
                OnError(ex);
            }
        }
    }
}
