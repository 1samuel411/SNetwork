using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNetwork
{
    [Serializable]
	public class NetworkObject : MonoBehaviour
    {

        public bool isMine;

		public int ownerId;

		void Awake()
		{
			
		}

        void Update()
        {
            if (ClientManager.instance.isConnected() == false)
            {
                isMine = true;
                return;
            }
            isMine = (ClientManager.instance.getId() == ownerId);
        }

	}

    [Serializable]
    public class NetworkObjectData
    {
        public int index;
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public int fromId;

        public NetworkObjectData(int index, Vector3 position, Quaternion rotation, int fromId)
        {
            this.fromId = fromId;
            this.index = index;
            this.position = position;
            this.rotation = rotation;
        }
    }
}