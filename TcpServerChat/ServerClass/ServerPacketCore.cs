using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpChat.Class;

namespace ServerChat.Class
{
    public class ServerPacketCore
    {
        private P packet { get; set; } = null;
        private ServerObject server { get; set; } = null;

        public ServerPacketCore(P packet, ServerObject server)
        {
            this.packet = packet;
            this.server = server;

            switch (this.packet.Type)
            {
                case ServerPacket.PacketType.Message:
                    MessageCallBack();
                    break;
                case ServerPacket.PacketType.RegisterInform:
                    RegCallBack();
                    break;
            }
        }

        private void RegCallBack()
        {
            string message = $"{packet.Nickname} conected!";
            Console.WriteLine(message);
            server.BroadcastMessage(message);
        }

        private void MessageCallBack()
        {
            string message = $"{packet.Nickname}: {packet.Data}";
            Console.WriteLine(message);
            server.BroadcastMessage(message);
        }
    }

    public class ServerPacket
    {
        public PacketType Type { get; private set; }
        public byte[] Data { get; private set; }
        public string Sender { get; private set; }
        public string Information { get; private set; }

        public enum PacketType
        {
            RegisterInform,
            Message
        }

        public ServerPacket(string sender, string information, PacketType type = PacketType.Message)
        {
            this.Type = type;
            this.Information = information;
            this.Sender = sender;

            Data = Make(sender, information, type);
        }

        private static byte[] Make(string sender, string inform, PacketType type)
        {
            string buffer = $"{sender}|{(int)type}|{inform}";

            return Encoding.Default.GetBytes(buffer);
        }
    }

    public class P
    {
        public ServerPacket.PacketType Type { get; set; }
        public string Data { get; set; }
        public string Nickname { get; set; }

        public void RecPacket(byte[] bf)
        {
            string buffer = Encoding.Default.GetString(bf);
            string[] arr = buffer.Split('|');
            int type = int.Parse(arr[1]);

            Nickname = arr[0];
            Type = (ServerPacket.PacketType)type;
            Data = arr[2];
        }
    }
}
