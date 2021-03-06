﻿using Lidgren.Network;
using PNetC;

namespace PNetU
{
    public partial class NetworkView
    {
        /// <summary>
        /// find a network view based on the given NetworkViewId
        /// </summary>
        /// <param name="viewID"></param>
        /// <returns></returns>
        public static NetworkView Find(NetworkViewId viewID)
        {
            return Net.Peer.NetworkViewManager.Find(viewID).Container as NetworkView;
        }

        /// <summary>
        /// find a networkview based on a networkviewid that was serialized into an rpc
        /// </summary>
        /// <param name="message">uses deserialize, so the read location does advance</param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static bool Find(ref NetIncomingMessage message, out NetworkView view)
        {
            PNetC.NetworkView netview;
            var result = Net.Peer.NetworkViewManager.Find(ref message, out netview);
            view = netview.Container as NetworkView;
            return result;
        }
    }
}
