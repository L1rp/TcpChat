using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCpChat.Class
{
    class ClientPacketCore
    {
        private P packet { get; set; } = null;

        public ClientPacketCore(P packet)
        {
            this.packet = packet;

            switch (this.packet.Type)
            {
                case ClientPacket.PacketType.Message:
                    MessageCallBack();
                    break;
            }
        }

        private void MessageCallBack()
        {
            string message = packet.Data;
            MessageBox.Show(message);
        }
    }

    public class ClientPacket
    {
        public PacketType Type { get; private set; }
        public byte[] Data { get; private set; }
        public string Sender { get; private set; }
        public string Information { get; private set; }

        public enum PacketType
        {
            RegisterInform = 0,
            Message
        }

        public ClientPacket(string sender, string inform, PacketType type = PacketType.Message)
        {
            this.Type = type;
            Information = inform;
            Sender = sender;

            Data = Make(inform, sender, type);
        }

        private static byte[] Make(string inform, string sender, PacketType type)
        {
            string buffer = $"{sender}|{(int)type}|{inform}";

            return Encoding.Default.GetBytes(buffer);
        }
    }

    public class P
    {
        public ClientPacket.PacketType Type { get; set; }
        public string Data { get; set; }
        public string Nickname { get; set; }

        public P(byte[] bf)
        {
            string buffer = Encoding.Default.GetString(bf);
            string[] arr = buffer.Split('|');
            int type = int.Parse(arr[1]);

            Nickname = arr[0];
            Type = (ClientPacket.PacketType)type;
            Data = arr[2];
        }

        public void RecPacket(byte[] bf)
        {
            string buffer = Encoding.Default.GetString(bf);
            string[] arr = buffer.Split('|');
            int type = int.Parse(arr[1]);

            Nickname = arr[0];
            Type = (ClientPacket.PacketType)type;
            Data = arr[2];
        }
    }
}
