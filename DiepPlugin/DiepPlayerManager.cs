using DarkRift.Server;
using DarkRift;

using System.Collections.Generic;
using System.Linq;
using System;

namespace DiepPlugin {

    using static GameScope;

    public class DiepPlayerManager : Plugin {
        public override bool ThreadSafe => false;

        public PlayerData[] PlayerArray => playerDataByClient.Values.ToArray();
        public override Version Version => new Version("1.0.0");

        Dictionary<IClient, PlayerData> playerDataByClient = new Dictionary<IClient, PlayerData>();

        public DiepPlayerManager(PluginLoadData pluginLoadData) : base(pluginLoadData) {
            ClientManager.ClientConnected += OnClientConnected;
            ClientManager.ClientDisconnected += OnClientDisconnected;
            PlayerManager = this;
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e) {
            playerDataByClient.Remove(e.Client);

            using (DarkRiftWriter writer = DarkRiftWriter.Create()) {
                writer.Write(e.Client.ID);

                using (Message message = Message.Create(GameNetworkTag.DESPAWN_PLAYER, writer)) {
                    foreach (IClient client in ClientManager.GetAllClients()) {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e) {
            Random r = new Random();
            BColor color = r.RandomBColor(200);

            PlayerData newPlayerData = new PlayerData(
                e.Client.ID,
                (float)r.NextDouble() * 0 /*MAP_WIDTH - MAP_WIDTH / 2*/,
                (float)r.NextDouble() * 0 /*MAP_WIDTH - MAP_WIDTH / 2*/,
                1f,
                color);

            using (DarkRiftWriter newPlayerWriter = DarkRiftWriter.Create()) {
                newPlayerWriter.Write(newPlayerData.ID);
                newPlayerWriter.Write(newPlayerData.X);
                newPlayerWriter.Write(newPlayerData.Y);
                newPlayerWriter.Write(newPlayerData.Radius);
                newPlayerWriter.Write(newPlayerData.ColorR);
                newPlayerWriter.Write(newPlayerData.ColorG);
                newPlayerWriter.Write(newPlayerData.ColorB);

                using (Message newPlayerMessage = Message.Create(GameNetworkTag.SPAWN_PLAYER, newPlayerWriter)) {
                    foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client)) {
                        client.SendMessage(newPlayerMessage, SendMode.Reliable);
                    }
                }
            }

            playerDataByClient.Add(e.Client, newPlayerData);
            using ( DarkRiftWriter playerWriter = DarkRiftWriter.Create()) {
                foreach (PlayerData otherPlayer in playerDataByClient.Values) {
                    playerWriter.Write(otherPlayer.ID);
                    playerWriter.Write(otherPlayer.X);
                    playerWriter.Write(otherPlayer.Y);
                    playerWriter.Write(otherPlayer.Radius);
                    playerWriter.Write(otherPlayer.ColorR);
                    playerWriter.Write(otherPlayer.ColorG);
                    playerWriter.Write(otherPlayer.ColorB);
                }

                using (Message playerMessage = Message.Create(GameNetworkTag.SPAWN_PLAYER, playerWriter)) {
                    e.Client.SendMessage(playerMessage, SendMode.Reliable);
                }
            }

            e.Client.MessageReceived += OnMovementMessageReceived;
        }

        private void OnMovementMessageReceived(object sender, MessageReceivedEventArgs e) {
            using(Message message = e.GetMessage()) {
                // if message was from movement
                if(message.Tag == GameNetworkTag.MOVEMENT) {

                    // unpacks the message
                    using(DarkRiftReader reader = message.GetReader()) {
                        float newX = reader.ReadSingle();
                        float newY = reader.ReadSingle();
                        float rotationZ = reader.ReadSingle();

                        // store the updated values
                        PlayerData playerData = playerDataByClient[e.Client];
                        playerData.X = newX;
                        playerData.Y = newY;
                        playerData.RotationZ = rotationZ;

                        // create re-route message
                        using (DarkRiftWriter writer = DarkRiftWriter.Create()) {
                            writer.Write(playerData.ID);
                            writer.Write(playerData.X);
                            writer.Write(playerData.Y);
                            writer.Write(playerData.RotationZ);
                            message.Serialize(writer);
                        }

                        // dispatch the re-route message to OTHER players
                        foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client)) {
                            client.SendMessage(message, e.SendMode);
                        }
                    }
                }
            }
        }
    }
}
