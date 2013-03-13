﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNetS
{
    /// <summary>
    /// contains information about the message
    /// </summary>
    public class NetMessageInfo
    {
        /// <summary>
        /// Mode the rpc was sent from by the client
        /// </summary>
        public readonly RPCMode mode;
        /// <summary>
        /// change this to false if the message should not continue forwarding to the rest of the players (you can tell who it'll forward to by the rpcmode)
        /// </summary>
        public bool continueForwarding = true;
        /// <summary>
        /// Player who sent the message
        /// </summary>
        public readonly Player player;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="player"></param>
        internal NetMessageInfo(RPCMode mode, Player player)
        {
            this.mode = mode;
            this.player = player;
        }
    }
}
