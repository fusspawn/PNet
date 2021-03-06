﻿using System;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;
using PNetS;

namespace ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ServerConfiguration(Properties.Settings.Default.MaximumPlayers,
                                                 Properties.Settings.Default.ListenPort);
            PNetServer.InitializeServer(config);

            PNetServer.ApproveConnection = ApproveConnection;
            PNetServer.OnPlayerConnected += OnPlayerConnected;
            PNetServer.OnPlayerDisconnected += delegate(Player player) { Debug.Log("player {0} disconnected", player.Id); };
            
            //If you want a global 'update' function, assign it here
            GameState.update += Update;

            Debug.logger = new DefaultConsoleLogger();

            //TODO: make some Room child classes, and load them into the _rooms dictionary
            Room newRoom = Room.CreateRoom("basic room");
            _room = newRoom.AddBehaviour<BasicRoom>();
            //loading of other data as well

            //Finish starting the server. Started in a new thread so that the console can sit open and still accept input
            _serverThread = new Thread(() => PNetServer.Start(Properties.Settings.Default.FrameTime));
            _serverThread.Start();

            Console.WriteLine("Server is running");
            //let the console sit open, waiting for a quit
            //this will throw errors if the program isn't running as a console app, like on unix as a background process
            //recommend including Mono.Unix.Native, and separately handling unix signals if this is running on unix.
            //you could also write a service, and run things that way. (Might also work on Unix better)
            while(true)
            {
                //This will throw errors on linux if not attached to a terminal
                var input = Console.ReadLine();

                if (input == "quit")
                    break;

                //if you wanted, you could also process other commands here to pass to the server/rooms.

                Thread.Sleep(100);
            }
            
            //shut down lidgren
            PNetServer.Disconnect();
            //shut down server. Will actually cause the server thread to finish running.
            PNetServer.Shutdown();
            //and give plenty of time for the server thread to close nicely
            Thread.Sleep(50);
            
            //we're exiting. make sure the server thread is closed
            if (_serverThread.IsAlive)
            {
                Console.WriteLine("Should not have had to abort thread. This could be a bug\nPress any key to continue shutting down.");
                Console.ReadKey();
                _serverThread.Abort();
            }
        }

        //This is called AFTER a connection has been approved
        private static void OnPlayerConnected(Player player)
        {
            //TODO: you could save where the player was last when they dc'd
            //then in here, move them to that room again. (if you have an mmo or something)
            //or move them to a lobby room
            //or a character select
            Debug.Log("player {0} connected", player.Id);
            player.ChangeRoom(_room.Room);
        }

        private static BasicRoom _room;
        private static Thread _serverThread;

        //main loop. run once every game tick.
        private static void Update()
        {
            //Approve connections that are waiting
            //remove them from the list after approving
            _clientsWaitingToBeApproved.RemoveAll(c =>
                {
                    c.Approve();
                    return true;
                    //TODO: maybe deny clients if their login credentials weren't valid?
                });
        }

        //This is called very first when a client connects to the server, before OnPlayerConnected
        //if you don't want the clients to use the HailMessage, just call netIncomingMessage.SenderConnection.Approve here
        //alternatively, don't even assign the ApproveConnection delegate, and it will just auto-approve
        private static void ApproveConnection(NetIncomingMessage netIncomingMessage)
        {
            //TODO: If you have login data, read it from netIncomingMessage.
            //netIncomingMessage is the data that was serialized in the WriteHailMessage on the client
            _clientsWaitingToBeApproved.Add(netIncomingMessage.SenderConnection);
        }

        private static readonly List<NetConnection> _clientsWaitingToBeApproved = new List<NetConnection>();
    }
}
