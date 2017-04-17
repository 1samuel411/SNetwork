using System;
using System.Collections.Generic;
using UnityEngine;

namespace SNetwork.Client
{

    [Serializable]
    public class ClientSettingsScriptableObject : ScriptableObject
    {
        public static string location = "ClientSettings";
        public static string resourcesLocation = "Assets/Plugins/SNetwork/Resources/ClientSettings.asset";

        [Header("Settings")]
        public int maxConnAttempts = 5;
        public float retryTime = 0.5f;

        [Header("Master Server Info")]
        public string ipAddress = "0.0.0.0";
        public int port = 1000;

        [Header("Spawnable Objects")]
        public List<GameObject> networkedObjects = new List<GameObject>();
    }
}