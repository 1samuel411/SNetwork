using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Boo.Lang;
using SNetwork;
using UnityEngine;
using UnityEngine.UI;

public class ClientMenuManager : MonoBehaviour
{

	public Text infoText;

    public InputField ipField;
	public InputField portField;

	public string ip;
	public int port;

	public string message;

    public GameObject playerGameObject;

	void Update()
	{
	    infoText.text = "Connected: " + ClientManager.instance.isConnected()
	                    + "\n" + "Connecting: " + ClientManager.instance.isConnecting()
	                    + "\n" + "Disconnecting: " + ClientManager.instance.isDisconnecting()
	                    + "\n" + "ID: " + ClientManager.instance.getId().ToString()
	                    + "\n" + "Master Client: " + ClientManager.instance.isMasterClient();
	}

	void Start()
	{
		port = 100;
		ip = "127.0.0.1";
		ipField.text = ip;
		portField.text = port.ToString();
	}

    public void CreatePlayer()
    {
        SNetwork.Network.Instantiate(playerGameObject, Vector3.left, Quaternion.identity);
    }

	public void Connect()
	{
		ClientManager.instance.Connect(ip, port);
	}

	public void Disconnect()
	{
		ClientManager.instance.Disconnect();
	}

	public void SendMessage()
	{
		ClientManager.instance.SendString(message);
	}

	public void SetMessage(string newMessage)
	{
		message = newMessage;
	}

	public void SetIp(string newIp)
	{
		ip = newIp;
	}

	public void SetPort(string newPort)
	{
		port = Int32.Parse(newPort);
	}

    private int index = 0;
    public void SetServerData()
    {
        index++;
        ClientManager.instance.SetServerData(new KeyValuePairs("helloServer" + index, "ppoo"));
    }

    private int index2 = 0;
    public void SetUserData()
    {
        index2++;
        ClientManager.instance.SetUserData(new KeyValuePairs("hello" + index2, "ppoo"));
    }

    public void Set()
    {
        SNetwork.Network.Initialize();
    }

    public void TestSend()
    {
        SetHealth(20);
    }

    [Networked(SendType.All)]
    public void SetHealth(int newHealth, bool original = true)
    {
        if(original)
            SNetwork.Network.SendMessage((Action<int, bool>) SetHealth, newHealth, false);
        
        Logging.CreateLog("Setting hp to: " + newHealth);
    }

}
