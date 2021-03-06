﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace SNetwork.Server
{
    public class CommandHandler : MonoBehaviour 
    {
	    private static List<Command> commands = new List<Command>();

        public static void RunCommand(string command, string commandText, Socket fromSocket, int id)
        {
            Command foundCommand = commands.FirstOrDefault(x => x.commandName == command) as Command;
            if (foundCommand == null)
            {
                Logging.CreateLog("[SNetworking] Invalid Request");
                Messaging.instance.SendInvalid(fromSocket, id);
                return;
            }
            foundCommand.callbackAction(commandText, fromSocket, id);
        }

        public static void RunCommand(string command, Socket fromSocket, int id)
        {
            if (command.StartsWith("!") || command.StartsWith("/"))
            {
                command = command.Substring(1);
            }
            command += " command is confirmed";
            string[] commandStrings = command.Split(' ');
            RunCommand(commandStrings[0], commandStrings[1], fromSocket, id);
        }

        public static void AddCommand(Command command)
        {
            commands.Add(command);
        }
    }

    public class Command
    {
        public Action<string, Socket, int> callbackAction;
        public string commandName;

        public Command(string commandName, Action<string, Socket, int> callbackAction)
        {
            this.callbackAction = callbackAction;
            this.commandName = commandName;
        }
    }
}
