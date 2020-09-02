using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCpChat.Class
{
    class ClientCore
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        private static TcpClient client;
        private static NetworkStream stream;
        public string userName;

        public ClientCore(string userName)
        {
            this.userName = userName;
            client = new TcpClient();
        }

        public void Start()
        {
            try
            {
                client.Connect(host, port);
                stream = client.GetStream();

                ClientPacket packet = new ClientPacket(userName, "", ClientPacket.PacketType.RegisterInform);
                SendMessage(packet);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //Disconnect();
            }
        }

        public void SendMessage(ClientPacket packet)
        {
            stream.Write(packet.Data, 0, packet.Data.Length);
        }

        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    
                    StringBuilder builder = new StringBuilder();
                    byte[] data = new byte[64];
                    int bytes = 0;

                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                    }
                    while (stream.DataAvailable);

                    P package = new P(data);
                    new ClientPacketCore(package);
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
