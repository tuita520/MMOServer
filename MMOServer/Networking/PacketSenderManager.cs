using ENet;
using MMOServer.ConsoleStuff;
using MMOServer.Game.Entities;
using MMOServer.Packets.PacketDefinitions.CB;
using System.Collections.Generic;
using System.Linq;

namespace MMOServer.Networking
{
    /// <summary>
    /// The Packet Sender Manager, responsible for sending and broadcasting packets
    /// </summary>
    class PacketSenderManager
    {
        private GameServer _gameServer;

        public PacketSenderManager(GameServer gameServer)
        {
            _gameServer = gameServer;
        }

        public void SendHandshakeResponse(ClientConnectionInfo connection, HandshakeResponseCode responseCode)
        {
            var handShakeResponse = new HandshakeResponse
            {
                ResponseCode = responseCode
            };

            DefaultSend(connection, handShakeResponse.Create());

            ConsoleUtils.Info("Sent Handshake Response to client on {0}", connection.Peer.GetRemoteAddress());
        }

        private void DefaultSend(ClientConnectionInfo connection, byte[] bytes)
        {
            if (connection != null)
                connection.Peer.Send(1, bytes.ToArray(), PacketFlags.Reliable);
            else
            {
                foreach (ClientConnectionInfo con in _gameServer.Connections)
                    con.Peer.Send(1, bytes.ToArray(), PacketFlags.Reliable);
            }
        }

        public void SendEntitySpawn(List<ClientConnectionInfo> connections, Entity entity)
        {
            var entitySpawn = new EntitySpawn
            {
                EntityID = entity.EntityID,
                Position = entity.Position,
                Rotation = entity.Rotation
            };

            if (connections != null)
            {
                foreach (ClientConnectionInfo con in connections)
                    con.Peer.Send(1, entitySpawn.Create(), PacketFlags.Reliable);
            }
            else
            {
                foreach (ClientConnectionInfo con in _gameServer.Connections)
                    con.Peer.Send(1, entitySpawn.Create(), PacketFlags.Reliable);
            }

            ConsoleUtils.Info("Broadcasted entity spawn");
        }
    }
}