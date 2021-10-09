using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;

namespace DiepPlugin {
    public static class RandomExtensionMethods {
        public static Color RandomColor(this Random r, int range) {
            return new Color(r.Next(0, range), r.Next(0, range), r.Next(0, range));
        }

        public static BColor RandomBColor(this Random r, int range) {
            return new BColor((byte)r.Next(0, range), (byte)r.Next(0, range), (byte)r.Next(0, range));
        }
    }

    public static class NetworkExtensionMethods {
        public static void ReadMessage(MessageReceivedEventArgs e, Action<Message> messageCallback) {
            using (Message message = e.GetMessage()) {
                messageCallback(message);
            }
        }

        public static void ReadMessageWithTag(MessageReceivedEventArgs e, ushort tag, Action<DarkRiftReader> readerCallback) {
            ReadMessage(e, (message) => {
                if (message.Tag == tag) {
                    using (DarkRiftReader reader = message.GetReader()) {
                        readerCallback(reader);
                    }
                }
            });
        }

        public static void WriteAndSendMessage(ushort messageTag, SendMode sendMode, IEnumerable<IClient> targetClients, Action<DarkRiftWriter> writerCallback) {
            using (DarkRiftWriter writer = DarkRiftWriter.Create()) {
                writerCallback(writer);

                using (Message message = Message.Create(messageTag, writer)) {
                    foreach (IClient client in targetClients) {
                        client.SendMessage(message, sendMode);
                    }
                }
            }
        }
    }
}
