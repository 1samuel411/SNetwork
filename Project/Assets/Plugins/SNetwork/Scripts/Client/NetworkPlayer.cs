﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SNetwork
{
    [Serializable]
	public class NetworkPlayer
	{

        public int id = -1;
        public string username = "";
	    public bool masterUser = false;

        public List<KeyValuePairs> data = new List<KeyValuePairs>();

        public NetworkPlayer(int id, string username)
        {
            this.id = id;
            this.username = username;
        }

	    public void SetMasterUser(bool masterUser)
	    {
	        this.masterUser = masterUser;
	    }

        public NetworkPlayer(int id)
        {
            this.id = id;
            this.username = "";
        }

        public NetworkPlayer()
        {
            this.id = -1;
            this.username = "";
        }
	}
}
