using MMOServer.ConsoleStuff;
using MMOServer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MMOServer.Packets
{
    /// <summary>
    /// The Packet Handling Manager, responsible for handling packets received
    /// </summary>
    class PacketHandlerManager
    {
        private Dictionary<PacketOP, IPacketHandler> _handlers;

        private GameServer _gameServer;

        public PacketHandlerManager(GameServer gameServer)
        {
            _gameServer = gameServer;
            SetupHandlers();
        }

        private void SetupHandlers()
        {
             _handlers = new Dictionary<PacketOP, IPacketHandler>();
            foreach(PacketHandlerBase handler in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(PacketHandlerBase)).Select(t => (PacketHandlerBase)Activator.CreateInstance(t, _gameServer)))
            {
                _handlers.Add(handler.Op, handler);
            }
            ConsoleUtils.Info("Set up {0} handled packets", _handlers.Count);
        }

        public void HandleData(byte[] data, ClientConnectionInfo connection)
        {
            if (!_handlers.ContainsKey((PacketOP)data[0]))
            {
                ConsoleUtils.Warning("Received invalid packet from client on {0} with Packet OP {1}", connection.Peer.GetRemoteAddress().ToString(), data[0]);
                return;
            }

            _handlers[(PacketOP)data[0]].Handle(data, connection);
        }
    }
}