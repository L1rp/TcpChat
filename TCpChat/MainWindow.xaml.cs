using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TcpChat
{
    public partial class MainWindow : Window
    {
        public static Dispatcher d = Dispatcher.CurrentDispatcher;
        ClientChat clientChat;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(nicknameTextBox.Text))
            {
                clientChat = new ClientChat(nicknameTextBox.Text, chatTextBox,usersList);
                clientChat.Start();
                mainGrid.IsEnabled = true;
                usersListGrid.IsEnabled = true;
                looginGrid.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("You Need Better Nickname!");
            }
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            clientChat.SendMessage(messageTextBox.Text);
            messageTextBox.Text = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            clientChat.Disconnect();
            Environment.Exit(0);
        }
    }

    public class ClientChat
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        private static TcpClient client;
        private static NetworkStream stream;
        public string userName;
        TextBox chat;
        ListBox users;

        public ClientChat(string userName, TextBox chat, ListBox users)
        {
            this.userName = userName;
            this.chat = chat;
            client = new TcpClient();
            this.users = users;
        }

        public void Start()
        {
            try
            {
                client.Connect(host, port);
                stream = client.GetStream();

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

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

        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    if (!String.IsNullOrEmpty(message))
                    {
                        MainWindow.d.Invoke(new Action(() =>
                        {
                            if (message.EndsWith(" вошел в чат"))
                            {
                                string[] words = message.Split(new char[] { ' ' });
                                users.Items.Add(words[0]);
                            }
                            else if(message.EndsWith(": покинул чат"))
                            {
                                string[] words = message.Split(new char[] { ':' });
                                users.Items.Remove(words[0]);
                            }
                            chat.Text += message + Environment.NewLine;
                        }));
                    }
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
