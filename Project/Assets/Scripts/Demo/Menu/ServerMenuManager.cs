using System;
using System.Collections;
using System.Collections.Generic;
using SNetwork.Server;
using UnityEngine;
using UnityEngine.UI;

public class ServerMenuManager : MonoBehaviour
{

	public InputField bufferSizeInputField;
	public InputField userSyncTimeInputField;
	public InputField portInputField;
	public InputField ipInputField;
	public InputField defaultSceneField;
	public InputField maxUsersField;
	public InputField serverNameField;

    public float userSyncTime;
	public int port, bufferSize, defaultScene, maxUsers;
	public string ip, serverName;

    public void SetDefaultScene(string defaultScene)
    {
        this.defaultScene = Int32.Parse(defaultScene);
    }

    public void SetServerName(string newName)
    {
        serverName = newName;
    }

    public void SetBufferSize(string newBufferSize)
	{
		bufferSize = Int32.Parse(newBufferSize);
	}

	public void SetUserSyncTime(string newSyncTime)
	{
		userSyncTime = float.Parse(newSyncTime);
	}

	public void SetPort(string newPort)
	{
		port = Int32.Parse(newPort);
	}

	public void SetIp(string newIp)
	{
		ip = newIp;
	}

    public void SetMaxUsers(string newUsers)
    {
        maxUsers = Int32.Parse(newUsers);
    }

	public void CreateServer()
	{
		ServerManager.instance.Create(ip, port, bufferSize, defaultScene, maxUsers);
	}

	public void CloseServer()
	{
		ServerManager.instance.Close();
	}

	void Start()
	{
		ip = "127.0.0.1";
		port = 100;
		bufferSize = 50000;
		userSyncTime = 0.5f;
	    defaultScene = 0;
	    maxUsers = 2;
	    serverName = "Server name";

		bufferSizeInputField.text = bufferSize.ToString();
		portInputField.text = port.ToString();
		ipInputField.text = ip;
		userSyncTimeInputField.text = userSyncTime.ToString();
	    defaultSceneField.text = defaultScene.ToString();
	    maxUsersField.text = maxUsers.ToString();
	    serverNameField.text = serverName;
	}

}
