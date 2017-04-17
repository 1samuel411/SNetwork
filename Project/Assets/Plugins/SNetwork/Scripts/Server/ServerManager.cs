using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace SNetwork.Server
{
	public class ServerManager : MonoBehaviour
	{
		public Server server;
        private ServerResponseHandler _serverResponseHandler;

        private static ServerManager _instance;
        public static ServerManager instance
        {
            get
            {
                if (_instance == null)
                {
                    // create an instance
                    GameObject newObject = new GameObject();
                    _instance = newObject.AddComponent<ServerManager>();
                    newObject.name = "Server Instance";
                }

                return _instance;
            }
            set { _instance = value; }
        }

        void Awake()
        {
            ResponseManager.instance.Clear();

            DontDestroyOnLoad(gameObject);

			server = new Server();
            _serverResponseHandler = new ServerResponseHandler(server);
		    _serverResponseHandler.Initialize();
            InitializeCommands();
		}
        
		public void Create(string ip = "127.0.0.1", int port = 100, int bufferSize = 50000, int defaultScene = 0, int maxUsers = 2)
		{
			server.SetupServer(ip, port, bufferSize, 0.5f, defaultScene, maxUsers);
		}

		public void Close()
		{
			server.CloseServer();
			Destroy(this.gameObject);
		    instance = null;
		}

	    private void InitializeCommands()
	    {
	        CommandHandler.AddCommand(new Command("time", TimeCommand));
	        CommandHandler.AddCommand(new Command("setname", SetName));
	        CommandHandler.AddCommand(new Command("kick", Kick));
	        CommandHandler.AddCommand(new Command("leave", Leave));
        }

	    private void TimeCommand(string text, Socket fromSocket, int fromId)
	    {
            Messaging.instance.SendCommandResponse(DateTime.UtcNow.ToString(), fromId, 0, 0, server.clientSockets);
        }

        private void SetName(string newName, Socket fromSocket, int fromId)
	    {
            server.clientSockets[fromSocket].username = newName;
            Logging.CreateLog("[SNetworking] Setting NetworkPlayer: " + server.clientSockets[fromSocket].id + ", NetworkPlayername to: " + server.clientSockets[fromSocket].username);
            Messaging.instance.SendCommandResponse("[Success]", server.clientSockets[fromSocket].id, 0, 0, server.clientSockets);
        }

	    private void Kick(string name, Socket fromSocket, int fromId)
	    {
            var NetworkPlayerSelected = from x in server.clientSockets where x.Value.username == name select x;

            if (NetworkPlayerSelected.Any())
            {
                Logging.CreateLog("[SNetworking] Could not find NetworkPlayer with the name: " + name);
            }
            else
            {
                for (int i = 0; i < NetworkPlayerSelected.Count(); i++)
                {
                    Logging.CreateLog("[SNetworking] Kicking NetworkPlayer: " + NetworkPlayerSelected.ElementAt(i).Value.id + ", with the NetworkPlayername: " + name);
                    Messaging.instance.SendCommandResponse("[Kicked]", 2, 0, 0, server.clientSockets);
                    NetworkPlayerSelected.ElementAt(i).Key.Shutdown(SocketShutdown.Both);
                    NetworkPlayerSelected.ElementAt(i).Key.Close();
                }
            }
        }

        private void Leave(string text, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("[SNetworking] NetworkPlayer: " + server.clientSockets[fromSocket].id + " has left.");
            server.RemoveSocket(fromSocket);
            fromSocket.Shutdown(SocketShutdown.Both);
            fromSocket.Close();
        }
    }
}