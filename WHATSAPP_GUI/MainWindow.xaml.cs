using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

namespace WHATSAPP_GUI
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkStreamMutexed canale;
        TcpClient client;
        string token;
        int IdUser;
        Login windowLogin;
        Registrazione windowReg;
        Chats windowChats;
        public MainWindow()
        {

            /*
            IPAddress iP = IPAddress.Parse("Es. 35.23.11.34");
            client = new TcpClient();

            client.Connect(iP, 7225);
            */
            client = new TcpClient(Dns.GetHostName(), 7225);
            canale = new NetworkStreamMutexed(client.GetStream());
            InitializeComponent();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {

            mainGrid.Children.Clear();
            windowLogin = new Login(canale);
            windowLogin.Loggato += Logged;
            mainGrid.Children.Add(windowLogin);

        }
        private void SingupClick(object sender, RoutedEventArgs e)
        {

            mainGrid.Children.Clear();
            windowReg = new Registrazione(canale);

            mainGrid.Children.Add(windowReg);
        }
        private void Logged(string token,int idUser, string username)
        {

            mainGrid.Children.Clear();
            windowChats = new Chats(canale,token,idUser);
            mainGrid.Children.Add(windowChats);
            this.Title = username;


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            windowLogin = new Login(canale);
            windowLogin.Loggato += Logged;
            mainGrid.Children.Add(windowLogin);
        }
    }
}
