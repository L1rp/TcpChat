using ServerChat.Class;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TCpChat;

namespace TcpChat.Class
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }

        ServerObject server;
        TcpClient client;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                P packet = new P();
                packet.RecPacket(GetMessage());
                new ServerPacketCore(packet, server);
                while (true)
                {
                    packet.RecPacket(GetMessage());

                    new ServerPacketCore(packet, server);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private byte[] GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
            }
            while (Stream.DataAvailable);

            return data;
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
