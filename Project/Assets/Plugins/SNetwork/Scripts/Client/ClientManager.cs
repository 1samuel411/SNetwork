using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using SNetwork;
using SNetwork.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour 
{

    private Client _client;
    private ClientResponseHandler _clientResponseHandler;

    private static ClientManager _instance;
    public static ClientManager instance
    {
        get
        {
            if (_instance == null)
            {
                // create an instance
                GameObject newObject = new GameObject();
                _instance = newObject.AddComponent<ClientManager>();
                newObject.name = "Client Instance";
            }

            return _instance;
        }
        set { _instance = value; }
    }

    private bool masterClient;

	void Awake()
	{
        ResponseManager.instance.Clear();
		_client = gameObject.AddComponent<Client>();
        _client.clientSettings = Resources.Load<ClientSettingsScriptableObject>(ClientSettingsScriptableObject.location);
	    if (_client.clientSettings == null)
	    {
	        instance = null;
            Destroy(this.gameObject);
	        throw new Exception("ClientSettings Missing... Go to Tools/SNetworking/Client Settings to create and configure ClientSettings");
        }
        _clientResponseHandler = new ClientResponseHandler(_client);
	    _clientResponseHandler.Initialize();
        SNetwork.Network.Initialize();

        ResponseManager.instance.AddServerResponse(MessageResponse, 7);
        ResponseManager.instance.AddServerResponse(InstantiateGameObject, 50);

        DontDestroyOnLoad(this.gameObject);

	    _client.sceneChangeCallback += OnSceneChange;
    }

    void Update()
    {
        masterClient = _client.masterClient;
    }
    
    public ClientSettingsScriptableObject GetClientSettings()
    {
        return _client.clientSettings;
    }

    public SNetwork.NetworkPlayer GetNetworkPlayer()
    {
        return _client.networkPlayers.FirstOrDefault(x => x.id == getId());
    }

    public Socket getSocket()
    {
        return _client.clientSocket;
    }

    public bool isConnected()
	{
		return _client.IsConnectedClient();
	}

	public bool isConnecting()
	{
		return _client.connecting;
	}

	public bool isDisconnecting()
	{
		return _client.disconnecting;
	}

    public bool isMasterClient()
    {
        return masterClient;
    }

    public int getCurrentScene()
    {
        return _client.currentScene;
    }

    public int getId()
    {
        return _client.ourId;
    }

    public object GetServerData(string key)
    {
        return _client.serverData.FirstOrDefault(x => x.Key.Equals(key)).Value;
    }

    public object GetClientData(string key, int client = -1)
    {
        if (client == -1)
            client = _client.ourId;
        return _client.networkPlayers.FirstOrDefault(x => x.id.Equals(client)).data.FirstOrDefault(x=> x.Key.Equals(key)).Value;
    }

    public void Connect(string ip, int port)
	{
		_client.Connect(ip, port, OnConnected, OnClose);
	}

	public void OnConnected(ResponseMessage response)
	{
		if (response.type == ResponseMessage.ResponseType.Success)
		{
            Debug.Log("Success!");
			// Do something
		}
		else if (response.type == ResponseMessage.ResponseType.Failure)
		{
			Debug.Log("Failed!");
			// Do something
		}
        else if (response.type == ResponseMessage.ResponseType.Full)
		{
		    Debug.Log("Server full!");
		    // Do something
		}
	}

	public void OnClose()
	{
		Debug.Log("Connection Closed!");
		// Do something
	}

	public void Disconnect()
	{
		_client.Disconnect(OnDisconnected);
	}

	public void OnDisconnected()
	{
		Debug.Log("Disconnected!");

        // Destroy ourselves
        GameObject.Destroy(this.gameObject);
	    instance = null;
	}

    public void OnSceneChange(int newScene)
    {
        Debug.Log("Scene Changed");
        SNetwork.Network.Initialize();
        //SceneManager.LoadScene(newScene);
    }

    public void SendString(string message)
	{
		_client.SendString(message);
	}

    public void SetServerData(KeyValuePairs data)
    {
        Messaging.instance.SendServerDataSetting(ByteParser.ConvertKeyValuePairToData(data), _client.clientSocket);
    }

    public void SetUserData(KeyValuePairs data)
    {
        Messaging.instance.SendUserDataSetting(ByteParser.ConvertKeyValuePairToData(data), _client.ourId, _client.clientSocket);
    }

    public void InstantiateGameObject(byte[] data, Socket socket, int from)
    {
        NetworkObjectData objectData = ByteParser.ConvertDataToObject(data) as NetworkObjectData;

        if (objectData.index >= ClientManager.instance.GetClientSettings().networkedObjects.Count)
        {
            Logging.CreateLog("Error: Object not found: " + objectData.index + ", " + ClientManager.instance.GetClientSettings().networkedObjects.Count);
            return;
        }

        SNetwork.Network.Instantiate(ClientManager.instance.GetClientSettings().networkedObjects[objectData.index], objectData.position, objectData.rotation, false, objectData.fromId);
    }

    void MessageResponse(byte[] response, Socket socket, int from)
    {
        string message = ByteParser.ConvertToASCII(response);

        if (message.Equals("Full"))
        {
            ResponseMessage messageResposne = new ResponseMessage();
            messageResposne.type = ResponseMessage.ResponseType.Full;
            OnConnected(messageResposne);
            Disconnect();
        }
    }
}
