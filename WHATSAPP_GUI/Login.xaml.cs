using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logica di interazione per Login.xaml
    /// </summary>
    public partial class Login : UserControl
{
        NetworkStreamMutexed _stream;


    public delegate void MyDelegate(string token,int idUser,string username);
    public event MyDelegate Loggato;

    public Login(NetworkStreamMutexed st)
    {
        _stream = st;
     
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string user = TXTUsername.Text.Replace('-', '_').Replace('#', '_'); ;
        string password = TXTPassword.Password.Replace('-', '_').Replace('#', '_'); ;

        string command = "LOG#"+user+"#"+password+"#";
            
        string response = _stream.Write(command);

        response.Trim('\0');

            if (response.Split('#').Length==3)
            {
                string token = response.Split('#')[0];
                string idUser = response.Split('#')[1];
                string Username = response.Split('#')[2];
                int num = 0;
                if (int.TryParse(token, out num))
                {
                    Loggato(token, int.Parse(idUser), Username);
                }
            }
            else
            {

                TextError.Text = response.Trim('\0');

            }






        }
}
}
