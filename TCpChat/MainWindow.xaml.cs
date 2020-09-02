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
using TCpChat;
using TCpChat.Class;

namespace TcpChat
{
    public partial class MainWindow : Window
    {
        public static Dispatcher d = Dispatcher.CurrentDispatcher;
        ClientCore clientCore;
        public string nickname;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(nicknameTextBox.Text))
            {
                nickname = nicknameTextBox.Text;
                clientCore = new ClientCore(nicknameTextBox.Text);
                clientCore.Start();

                // Включение \ Отключение елементов интерфейса
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
            ClientPacket packet = new ClientPacket(nickname, messageTextBox.Text, ClientPacket.PacketType.Message);
            clientCore.SendMessage(packet);
            messageTextBox.Text = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            clientCore.Disconnect();
            Environment.Exit(0);
        }
    }

    
}
