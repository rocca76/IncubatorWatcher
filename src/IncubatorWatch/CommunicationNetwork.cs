using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace HatchWatch
{
    public delegate void MessageEventHandler(String data);

    public sealed class CommunicationNetwork
    {
        #region Private Variables
        private static readonly CommunicationNetwork _instance = new CommunicationNetwork();
        private readonly IPAddress _controllerIPAddress = IPAddress.Parse("192.168.250.200");
        private TcpListener _tcpListener = null;
        private TcpClient _tcpClient = null;
        private Thread _listenerThread = null;
        #endregion


        #region Constructors
        private CommunicationNetwork(){}
        #endregion


        #region Events
        public static event MessageEventHandler EventHandlerMessageReceived;
        #endregion


        #region Public Properties
        public static CommunicationNetwork Instance
        {
            get { return _instance; }
        }
        #endregion

        #region Public Methods
        public void Init()
        {
            try
            {
                if (_listenerThread == null)
                {
                    _listenerThread = new Thread(RunListener);
                    _listenerThread.Start();

                    String dateTime = string.Format("INIT {0} {1} {2} {3} {4} {5} {6}",
                    DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

                    Send(dateTime);
                }
            }
            catch (SocketException sex)
            {
                Debug.Print(sex.ToString());
            }
            catch (ApplicationException aex)
            {
                Debug.Print(aex.ToString());
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        public void Send(String message)
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    IAsyncResult result = socket.BeginConnect(_controllerIPAddress, 11000, null, null);

                    bool success = result.AsyncWaitHandle.WaitOne(5000, true);

                    if (!success)
                    {
                        socket.Close();
                        throw new ApplicationException("Failed to connect to controller.");
                    }

                    byte[] data = Encoding.ASCII.GetBytes(message);
                    socket.Send(data);

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                
            }
            catch (SocketException sex)
            {
                Debug.Print(sex.ToString());
            }
            catch (ApplicationException aex)
            {
                Debug.Print(aex.ToString());
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_tcpClient != null)
                {
                    _tcpClient.GetStream().Close();
                    _tcpClient.Close();
                    _tcpClient = null;
                }

                if (_tcpListener != null)
                {
                    _tcpListener.Stop();
                    _tcpListener = null;
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }
        #endregion


        #region Private Methods
        private void RunListener()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 11000);
                _tcpListener.Start();

                _tcpClient = _tcpListener.AcceptTcpClient();

                var data = new byte[_tcpClient.ReceiveBufferSize];
                StringBuilder dataString = new StringBuilder();

                using (NetworkStream networkStream = _tcpClient.GetStream())
                {
                    int readCount;
                    String dataReceived = String.Empty;

                    while ((readCount = networkStream.Read(data, 0, _tcpClient.ReceiveBufferSize)) != 0)
                    {
                        dataReceived += Encoding.UTF8.GetString(data, 0, readCount);

                        if (dataReceived.IndexOf("</hatcher>") > -1)
                        {
                            RaiseMessageReceivedEvent(dataReceived);
                            dataReceived = String.Empty;
                        }
                    }
                }
            }
            catch (SocketException sex)
            {
                Debug.Print(sex.ToString());
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
            finally
            {
                if (_tcpClient != null)
                {
                    _tcpClient.Close();
                    _tcpClient = null;
                }

                if (_tcpListener != null)
                {
                    _tcpListener.Stop();
                    _tcpListener = null;
                }
            }
        }

        private void RaiseMessageReceivedEvent(String message)
        {
            if (EventHandlerMessageReceived != null)
            {
                EventHandlerMessageReceived(message);
            }
        }
        #endregion
    }
}
