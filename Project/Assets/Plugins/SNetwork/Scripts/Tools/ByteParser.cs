using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SNetwork;
using UnityEngine;

namespace SNetwork
{
    public class ByteParser : MonoBehaviour
    {
        public static string ConvertToASCII(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }

        public static NetworkPlayer[] ConvertToNetworkPlayers(byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (formatter.Deserialize(memStream) as NetworkPlayer[]);
            }
        }

        public static byte[] ConvertNetworkPlayersToBytes(NetworkPlayer[] list)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(memStream, list);
            byte[] networkList = memStream.ToArray();
            memStream.Close();

            return networkList;
        }

        public static byte[] ConvertKeyValuePairsToData(KeyValuePairs[] data)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(memStream, data);
            byte[] networkList = memStream.ToArray();
            memStream.Close();

            return networkList;
        }

        public static byte[] ConvertKeyValuePairToData(KeyValuePairs data)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(memStream, data);
            byte[] networkList = memStream.ToArray();
            memStream.Close();

            return networkList;
        }

        public static KeyValuePairs[] ConvertDataToKeyValuePairs(byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (formatter.Deserialize(memStream) as KeyValuePairs[]);
            }
        }

        public static KeyValuePairs ConvertDataToKeyValuePair(byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (formatter.Deserialize(memStream) as KeyValuePairs);
            }
        }

        public static object[] ConvertDataToObjects(byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (formatter.Deserialize(memStream) as object[]);
            }
        }

        public static object ConvertDataToObject(byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (formatter.Deserialize(memStream) as object);
            }
        }

        public static byte[] ConvertObjectToBytes(object Object)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(memStream, Object);
            byte[] data = memStream.ToArray();
            memStream.Close();

            return data;
        }
    }
}
