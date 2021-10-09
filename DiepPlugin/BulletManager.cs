using System.Collections.Generic;
using System.Timers;
using System.Text;
using System;

using DarkRift.Server;
using System.Linq;

namespace DiepPlugin {
    using static NetworkExtensionMethods;
    using static GameNetworkTag;

    public class BulletManager : Plugin {
        Dictionary<uint, BulletData> bulletDataDictionary = new Dictionary<uint, BulletData>();
        List<BulletData> bulletList = new List<BulletData>();
        List<int> indexHit = new List<int>();
        Timer bulletTimer;

        public BulletManager(PluginLoadData pluginLoadData) : base(pluginLoadData) {
            GameScope.BulletManager = this;
            ClientManager.ClientConnected += OnClientConnected; ;
            ClientManager.ClientDisconnected += OnClientDisconnected;

            bulletTimer = new Timer(1000 / 120);
            bulletTimer.Elapsed += BulletTimer_Elapsed;
            bulletTimer.Start();
        }

        private void BulletTimer_Elapsed(object sender, ElapsedEventArgs e) {
            indexHit.Clear();
            foreach (var player in GameScope.PlayerManager.PlayerArray) {
                for (int i = 0; i < bulletList.Count; i++) {
                    if (Math.Pow(player.X - bulletList[i].X, 2) + Math.Pow(player.Y - bulletList[i].Y, 2) < Math.Pow(player.Radius, 2)) {
                        indexHit.Add(i);
                    }   
                }
            }

            foreach (var index in indexHit) {
                bulletList.RemoveAt(index);
            }
        } 

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e) {

        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e) {
            e.Client.MessageReceived += Client_MessageReceived;
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e) {
            ReadMessageWithTag(e, SPAWN_BULLET,
                (reader) => {
                    BulletData bData = new BulletData {
                        ownerID = reader.ReadUInt16(),
                        X = reader.ReadSingle(),
                        Y = reader.ReadSingle(),
                        vX = reader.ReadSingle(),
                        vY = reader.ReadSingle(),
                        ColorR = reader.ReadByte(),
                        ColorG = reader.ReadByte(),
                        ColorB = reader.ReadByte(),
                        radius = reader.ReadSingle(),
                    };

                    bulletList.Add(bData);

                    WriteAndSendMessage(SPAWN_BULLET, DarkRift.SendMode.Reliable, ClientManager.GetAllClients().Where(x => x != e.Client),
                        (writer) => {
                            writer.Write(bData.ownerID);
                            writer.Write(bData.X);
                            writer.Write(bData.Y);
                            writer.Write(bData.vX);
                            writer.Write(bData.vY);
                            writer.Write(bData.ColorR);
                            writer.Write(bData.ColorG);
                            writer.Write(bData.ColorB);
                            writer.Write(bData.radius);
                        }
                    );
                }
            );
        }

            

        public override bool ThreadSafe => false;

        public override Version Version => new Version("1.0.0");
    }
}
