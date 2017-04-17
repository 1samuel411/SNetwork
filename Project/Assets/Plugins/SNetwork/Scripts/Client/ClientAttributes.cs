using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SNetwork;
using SNetwork.Client;
using UnityEngine;
using Network = SNetwork.Network;

public class ClientAttributes : MonoBehaviour
{
    public static int customId = 1;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple =  true)]
public class Networked : Attribute
{
    public Networked(SendType sendType)
    {
        if (sendType == SendType.All)
            _sendId = 2;
        else if (sendType == SendType.MasterClient)
            _sendId = 1;

        _id = GetUniqueId();
        Register();
    }

    private int GetUniqueId()
    {
        int id = ClientAttributes.customId;
        ClientAttributes.customId++;
        return id;
    }

    private void Register()
    {
        ResponseManager.instance.AddCustomResponse(Response, Id());
    }
    
    private void SendMethod()
    {
        
    }

    private void Response(byte[] data, Socket fromSocket, int fromId)
    {
        object[] objects = ByteParser.ConvertDataToObjects(data);
        attributeMethod.method.Invoke(attributeMethod.classInstance, objects);
    }

    private Network.AttributeMethod attributeMethod;
    public void SetAttributeMethod(Network.AttributeMethod attributeMethod)
    {
        this.attributeMethod = attributeMethod;
    }

    private int _sendId;
    public int SendId()
    {
        return _sendId;
    }

    private int _id;
    public int Id()
    {
        return _id;
    }
}
