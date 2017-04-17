using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SNetwork.Server
{
    public class ServerResponseHandler
    {

        private Server _server;

        public ServerResponseHandler(Server server)
        {
            _server = server;
        }
        
        public void Initialize()
        {
            ResponseManager.instance.Clear();
            ResponseManager.instance.AddServerResponse(Response50, 50);
            ResponseManager.instance.AddServerResponse(Response40, 40);
            ResponseManager.instance.AddServerResponse(Response21, 21);
            ResponseManager.instance.AddServerResponse(Response20, 20);
            ResponseManager.instance.AddServerResponse(Response12, 12);
            ResponseManager.instance.AddServerResponse(Response3, 3);
            ResponseManager.instance.AddServerResponse(Response2, 2);
        }

        public void Response50(byte[] resposneBytes, Socket fromSocket, int fromId)
        {
            Messaging.instance.Send(resposneBytes, 50, 2, fromId, 0, ServerManager.instance.server.clientSockets);
            Logging.CreateLog("Adding GameObject to game :) from: " + fromId);
            ServerManager.instance.server.networkObjects.Add(ByteParser.ConvertDataToObject(resposneBytes) as NetworkObject);
        }

        public void Response40(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _server.currentScene = BitConverter.ToInt32(responseBytes, 0);
            Logging.CreateLog("Recieved a 40: " + fromId + ": " + _server.currentScene);
        }

        public void Response21(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a 21: " + fromId + ": " + ByteParser.ConvertToASCII(responseBytes));
            CommandHandler.RunCommand(ByteParser.ConvertToASCII(responseBytes), fromSocket, fromId);
        }

        public void Response20(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a 20: " + ByteParser.ConvertToASCII(responseBytes));
        }

        public void Response12(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a 12: " + fromId + ": " + responseBytes.Length);
        }

        public void Response3(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a 3: " + fromId + ": " + responseBytes.Length);
            _server.SetUserData(fromId, ByteParser.ConvertDataToKeyValuePair(responseBytes));
        }

        public void Response2(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a 2: " + fromId + ": " + responseBytes.Length);
            _server.SetServerData(ByteParser.ConvertDataToKeyValuePair(responseBytes));
        }
    }
}
