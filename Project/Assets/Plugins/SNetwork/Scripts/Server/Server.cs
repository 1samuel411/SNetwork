using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using SNetwork;

namespace SNetwork.Server
{
    public class Server
    {

        private byte[] _buffer = new byte[50000];
	    private int _bufferSize;
        public Dictionary<Socket, NetworkPlayer> clientSockets = new Dictionary<Socket, NetworkPlayer>();
        public List<KeyValuePairs> serverData = new List<KeyValuePairs>();
        public List<NetworkObject> networkObjects = new List<NetworkObject>();
        public Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public int currentScene;
        public int maxUsers;
	    private float _userSyncTime;

	    public bool _opened;

        private void BeginAccepting()
        {
            serverSocket.BeginAccept(new AsyncCallback(AcceptedConnection), null);
        }

        private void BeginReceiving(Socket socket)
        {
            try
            {
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (ObjectDisposedException e)
            {
				Console.WriteLine(e);
            }
        }

        private Thread _userSyncThread;
        public void SetupServer(string ip = "127.0.0.1", int port = 100, int bufferSize = 50000, float userSyncTime = 0.5f, int defaultScene = 0, int maxUsers = 2, string serverName = "")
        {
			Logging.Clear();
	        this._userSyncTime = userSyncTime;
	        this._bufferSize = bufferSize;
            this.currentScene = defaultScene;
            this.maxUsers = maxUsers;
	        _buffer = new byte[_bufferSize];
            Logging.CreateLog("[SNetworking] Creating and seting up the server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(10);
            BeginAccepting();
            Logging.CreateLog("[SNetworking] Success!");
	        
		    _opened = true;

            _userSyncThread = new Thread(Sync);
            _userSyncThread.Start();
        }

        private void Sync()
        {
            int iteration = 0;
            while (_opened)
            {
                iteration++;

                // TODO: Make this update when a user leaves or something
                Thread.Sleep((int)(_userSyncTime * 1000) / 3);

                SetMasterClient();

                Messaging.instance.SendNetworkPlayers(ByteParser.ConvertNetworkPlayersToBytes(clientSockets.Values.ToArray()), clientSockets, 2);

                Thread.Sleep((int)(_userSyncTime * 1000) / 3);

                Messaging.instance.SendServerData(ByteParser.ConvertKeyValuePairsToData(serverData.ToArray()), clientSockets, 2);

                Thread.Sleep((int)(_userSyncTime * 1000) / 3);

                SyncUser(2);
            }
        }

        public void SyncUser(int id)
        {
            Messaging.instance.SendRecieveSceneChange(currentScene, clientSockets, id);
        }

        public void CloseServer()
	    {
		    for (int i = 0; i < clientSockets.Count; i++)
		    {
			    Socket clientSocket = clientSockets.Keys.ElementAt(i);
				clientSocket.Shutdown(SocketShutdown.Both);
				clientSocket.Close();
		    }
			clientSockets.Clear();

		    serverSocket.Close();

            _opened = false;
		    
			Logging.Clear();
			Logging.CreateLog("[SNetworking] Server successfully closed!");
	    }

        private void AcceptedConnection(IAsyncResult AR)
        {
            Socket socket = serverSocket.EndAccept(AR);
            if (clientSockets.Count >= maxUsers)
            {
                Logging.CreateLog("[SNetworking] Max clients reached");
                Messaging.instance.SendInfoMessage(socket, "Full",0);
                return;
            }
            Logging.CreateLog("[SNetworking] Connection received from: " + socket.LocalEndPoint);

            int uniqueId = new Random().Next(3, 256);

            Logging.CreateLog("Checking unique id: " + uniqueId);

            if (clientSockets.Count > 0)
            {
                bool unique = false;
                bool changed = false;
                while (!unique)
                {
                    changed = false;
					// TODO: Make this more optimized
                    foreach (NetworkPlayer x in clientSockets.Values)
                    {
                        if (x.id == uniqueId)
                        {
                            changed = true;
                            uniqueId = new Random().Next(3, 256);
                           Logging.CreateLog("Checking unique id: " + uniqueId);
                        }
                    }
                    if (!changed)
                        unique = true;
                }
            }

           Logging.CreateLog("Assigning unique id: " + uniqueId);

            clientSockets.Add(socket, new NetworkPlayer(uniqueId));

            SetMasterClient();

            Messaging.instance.SendId(uniqueId, uniqueId, 0, 0, clientSockets);

            SyncUser(2);

            BeginReceiving(socket);
            BeginAccepting();
        }

        private void SetMasterClient()
        {
            // set master client
            bool hasMasterClient = false;
            for (int i = 0; i < clientSockets.Count; i++)
            {
                if (clientSockets.Values.ElementAt(i).masterUser)
                {
                    hasMasterClient = true;
                    break;
                }
            }
            if (!hasMasterClient && clientSockets.Count > 0)
            {
                clientSockets.Values.ElementAt(0).SetMasterUser(true);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            if (!IsConnectedServer(socket))
                return;

            int received = socket.EndReceive(AR);

            byte[] dataBuffer = new byte[received];
            Array.Copy(_buffer, dataBuffer, received);

            int headerCode = Convert.ToInt32(dataBuffer[0]);
            byte[] customCode = new byte[2];
            customCode[0] = dataBuffer[3];
            customCode[1] = dataBuffer[4];
            int sendCode = Convert.ToInt32(dataBuffer[1]);
            if (headerCode == 0)
            {
                // custom message, reroute it
                Messaging.instance.Send(dataBuffer.Skip(5).ToArray(), headerCode, sendCode, clientSockets[socket].id, BitConverter.ToInt16(customCode, 0), clientSockets);
            }
            else
            {
                // message with a header, keep it
                ResponseManager.instance.HandleResponse(dataBuffer.Skip(5).ToArray(), headerCode, sendCode, BitConverter.ToInt16(customCode, 0), socket, clientSockets[socket].id);
            }

            BeginReceiving(socket);
        }

        public void SetServerData(KeyValuePairs data)
        {
            serverData = SetData(serverData, data);
        }

        public void SetUserData(int target, KeyValuePairs data)
        {
            for (int i = 0; i < clientSockets.Count; i++)
            {
                if (clientSockets.Values.ElementAt(i).id == target)
                {
                    clientSockets.Values.ElementAt(i).data = SetData(clientSockets.Values.ElementAt(i).data, data);
                    return;
                }
            }
        }

        private List<KeyValuePairs> SetData(List<KeyValuePairs> source, KeyValuePairs data)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Key == data.Key)
                {
                    source[i] = data;
                    return source;
                }
            }
            source.Add(data);
            return source;
        }

        public bool IsConnectedServer(Socket socket)
        {
            bool isConnected = Network.IsConnected(socket);
            if (!isConnected)
            {
                KeyValuePair<Socket, NetworkPlayer> socketRetrieved = clientSockets.FirstOrDefault(t => t.Key == socket);
                Logging.CreateLog("[SNetworking] NetworkPlayer: " + socketRetrieved.Value.id + ", has been lost.");
                RemoveSocket(socket);
            }
            return isConnected;
        }

        public void RemoveSocket(Socket socket)
        {
            clientSockets.Remove(socket);
            SetMasterClient();
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
