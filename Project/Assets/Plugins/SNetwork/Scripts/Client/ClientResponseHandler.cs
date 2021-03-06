﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SNetwork.Client
{
    public class ClientResponseHandler
    {

        private Client _client;

        public ClientResponseHandler(Client client)
        {
            _client = client;
        }

        public void Initialize()
        {
            ResponseManager.instance.Clear();
            ResponseManager.instance.AddServerResponse(Response1, 1);
            ResponseManager.instance.AddServerResponse(Response4, 4);
            ResponseManager.instance.AddServerResponse(Response5, 5);
            ResponseManager.instance.AddServerResponse(Response6, 6);
            ResponseManager.instance.AddServerResponse(Response7, 7);
            ResponseManager.instance.AddServerResponse(Response9, 9);
            ResponseManager.instance.AddServerResponse(Response14, 14);
            ResponseManager.instance.AddServerResponse(Response41, 41);
        }

        public void Response1(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _client.serverData = ByteParser.ConvertDataToKeyValuePairs(responseBytes);
        }

        public void Response4(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a command response: " + ByteParser.ConvertToASCII(responseBytes));
        }

        public void Response5(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a invalid request");
        }

        public void Response6(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a failure");
        }

        public void Response7(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a message: " + ByteParser.ConvertToASCII(responseBytes));
        }

        public void Response9(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved the id: " + Convert.ToInt32(responseBytes[0]));
            _client.ourId = Convert.ToInt32(responseBytes[0]);
        }

        public void Response14(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _client.networkPlayers = ByteParser.ConvertToNetworkPlayers(responseBytes);
        }

        public void Response41(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _client.currentScene = BitConverter.ToInt32(responseBytes, 0);
        }
    }
}
