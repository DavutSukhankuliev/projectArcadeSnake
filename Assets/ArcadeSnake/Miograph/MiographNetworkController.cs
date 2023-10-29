using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ArcadeSnake
{
    public class MiographNetworkController : IMiographDataSource
    {
        public bool IsConnected => _isConnected;
        
        private readonly MiographSettings _miographSettings;

        private static bool _isConnected;
        
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _thread;
        
        private bool _isGameActive;

        public MiographNetworkController(MiographSettings miographSettings)
        {
            _miographSettings = miographSettings;
            _isGameActive = Application.isPlaying;
            Connect();
        }

        private void Connect()
        {
            _thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                try
                {
                    _client = new TcpClient(AddressFamily.InterNetwork);
                    _client.Connect(_miographSettings.IP, _miographSettings.port);
                    _isConnected = true;
                }
                catch (SocketException e)
                {
                    Debug.Log($"Application {_isGameActive}");
                    Debug.LogError(e.Message);
                    if (_isGameActive)
                    {
                        Connect();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            });
            _thread.Start();            
            Application.quitting += Quit;            
        }

        public string GetVersion()
        {
            var data = Encoding.ASCII.GetBytes("*IDN?");

            return SendRequest(data);
        }

        public void SetColor(int channel, int red, int green, int blue)
        {
            var data = Encoding.ASCII.GetBytes($"CHAN:LED:COL {channel} {red} {green} {blue}");

            SendRequest(data);
        }

        public string GetColor(int channel)
        {
            var data = Encoding.ASCII.GetBytes($"CHAN:LED:COL? {channel}");

            return SendRequest(data);
        }

        public void GetData(int channel, Action<MiographData> onResponse)
        {
           var data = Encoding.ASCII.GetBytes($"CHAN:DATA:GET {channel}");

            MiographData miographData = new MiographData();
            miographData.Data = SendRequest(data);
            
            onResponse?.Invoke(miographData);
        }
        
        private string SendRequest(byte[] data)
        {
            try
            {
                if (_stream == null)
                {
                    _stream = _client.GetStream();
                }

                _stream.Write(data, 0, data.Length);

                data = new Byte[256];

                String responseData = String.Empty;
                Int32 bytes = _stream.Read(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);

                return responseData;
            }
            catch (SocketException se)
            {
                Debug.LogError(se.Message);
                throw new Exception();
            }
            catch (Exception e) 
            {
                Debug.LogError(e.Message);
               
                throw new Exception();
            }
        }
        
        private void Quit()
        {
            _isGameActive = false;
            _thread.Abort();            
            if (!IsConnected)
            {
                return;
            }

            _client.Close();
        }
    }
}