using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using SNetwork.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SNetwork
{
	public class Network
	{

        public static AttributeMethod[] attributes;
        
        public static NetworkObject Instantiate(GameObject newObject, Vector3 position, Quaternion rotation, bool send = true, int targetId = -1)
        {
            if (!ClientManager.instance.GetClientSettings().networkedObjects.Contains(newObject))
            {
                throw new Exception("[SNetwork] Cannot find the GameObject to Instantiate in the Client Settings.");
            }
            
            GameObject newObj = GameObject.Instantiate(newObject, position, rotation) as GameObject;
            NetworkObject view = newObj.GetComponent<NetworkObject>();
            view.isMine = true;

            if (ClientManager.instance.isConnected())
            {
                int index = ClientManager.instance.GetClientSettings().networkedObjects.IndexOf(newObject);
                if(send)
                    Messaging.instance.CreateGameObject(new NetworkObjectData(index, position, rotation, ClientManager.instance.getId()), ClientManager.instance.getSocket());

                view.ownerId = (targetId == -1) ? ClientManager.instance.getId() : targetId;
            }

            return view;
		}

	    public static void Initialize()
	    {
	        ClientAttributes.customId = 1;
            ResponseManager.instance.ClearCustomResponses();
	        attributes = GetAttributeMethods();
	    }

	    public static void SendMessage(Delegate info, params object[] values)
	    {
	        if (!IsConnected(ClientManager.instance.getSocket()))
	            return;

	        if (attributes == null || attributes.Length <= 0)
                Initialize();

	        for (int i = 0; i < attributes.Length; i++)
	        {   
	            if (attributes[i].method == info.Method)
	            {
                    // found the attribute, send it over the network :)
                    MemoryStream stream = new MemoryStream();
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, values);
	                byte[] data = stream.ToArray();
	                stream.Close();
                    Messaging.instance.SendFinal(data, 0, attributes[i].attribute.SendId(), ClientManager.instance.getId(), attributes[i].attribute.Id(), ClientManager.instance.getSocket());
	                break;
	            }
	        }
	    }

        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            var member = expression.Body as MethodCallExpression;

            if (member != null)
                return member.Method;

            throw new ArgumentException("Expression is not a method", "expression");
        }

        public static AttributeMethod FindAttribute(int id)
	    {
	        if (attributes == null || attributes.Length <= 0)
                Initialize();


            for (int i = 0; i < attributes.Length; i++)
            {
	            if (attributes[i].attribute.Id() == id)
	            {
	                return attributes[i];
	            }
	        }

	        return null;
	    }

        private static AttributeMethod[] GetAttributeMethods()
        {
            List<AttributeMethod> response = new List<AttributeMethod>();
            MonoBehaviour[] sceneActive = GameObject.FindObjectsOfType<MonoBehaviour>();

            foreach (MonoBehaviour mono in sceneActive)
            {
                Type monoType = mono.GetType();

                // Retreive the fields from the mono instance
                MethodInfo[] methodFields = monoType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

                // search all fields and find the attribute [Position]
                for (int i = 0; i < methodFields.Length; i++)
                {
                    Networked attribute = Attribute.GetCustomAttribute(methodFields[i], typeof(Networked)) as Networked;
                    
                    // if we detect any attribute 
                    if (attribute != null)
                    {
                        AttributeMethod newAttributeMethod = new AttributeMethod(methodFields[i], attribute, mono);
                        response.Add(newAttributeMethod);
                        attribute.SetAttributeMethod(newAttributeMethod);
                    }
                }
            }

            return response.ToArray();
        }

	    public class AttributeMethod
	    {
	        public MethodInfo method;
	        public Networked attribute;
	        public object classInstance;

	        public AttributeMethod(MethodInfo method, Networked attribute, object classInstance)
	        {
	            this.method = method;
	            this.attribute = attribute;
	            this.classInstance = classInstance;
	        }
	    }

        public static bool IsConnected(Socket socket)
        {
            try
            {
                bool connected = !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);

                return connected;
            }
            catch (SocketException) { return false; }
        }
    }
}
