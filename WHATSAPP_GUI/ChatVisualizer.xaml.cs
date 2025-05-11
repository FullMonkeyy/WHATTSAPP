using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
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

namespace WHATSAPP_GUI
{
    /// <summary>
    /// Logica di interazione per ChatVisualizer.xaml
    /// </summary>
    public partial class ChatVisualizer : UserControl
    {
        NetworkStreamMutexed _stream;        
        Chat contextChat;
        string _token;
        string pathFile;
        int _idUser;
        bool _isFileSelected;
        public delegate void MyDelegate(string token,string id);
        public event MyDelegate RequestedUpdate;

        public delegate void DelegateChatDeleted(string token,ChatVisualizer chat);
        public event DelegateChatDeleted OnChatDeleted; 

        public ChatVisualizer(NetworkStreamMutexed st, string t, Chat c, int idUser)
        {
            _stream = st;
            contextChat = c;
            InitializeComponent();
            _token = t;
            contactName.Text = c.nomecontact;
            _idUser = idUser;
            _isFileSelected = false;
            ChatProfilo.Source = contextChat.Immagine;
        }

        private void Messages_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string msg = MessageboxDaInviare.Text;
            byte[] data=null;
            byte[] cmdtotale = new byte[20000000];
            if(contextChat!=null)
            if (pathFile != null) 
            {
                FileStream fs = new FileStream(pathFile, FileMode.Open);
                data = new byte[(int)fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();
                string p = System.IO.Path.GetFileName(pathFile);
                p = p.Replace('-', '_').Replace('#', '_');
              

                _stream.SendMessage("ADDMESSAGE#" + _token + "#" + contextChat.Id + "#" + msg + "#" + p + "#" + data.Length + "#" + DateTime.Now.ToLongDateString() + "#", data);

            }
            else 
            {
                
                _stream.Write("ADDMESSAGE#" + _token + "#" + contextChat.Id + "#" + msg + "###" + DateTime.Now.ToLongDateString() + "#");
 
            }

            if(contextChat!=null)
            RequestedUpdate(_token,contextChat.Id.ToString());
         
            pathFile = null;
        }

        private void Button_Click_FILE(object sender, RoutedEventArgs e)
        {

            if (!_isFileSelected)
            {

                OpenFileDialog ofp = new OpenFileDialog();
                if ((bool)ofp.ShowDialog())
                {

                    pathFile = ofp.FileName;
                    (sender as Button).Background = Brushes.Green;
                    _isFileSelected = true;
                }
                else _isFileSelected = false;
            
            }
            else
            {

                (sender as Button).Background = Brushes.Gray;
                pathFile = null;
                _isFileSelected = false;
            }

        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            Messages.ItemsSource = contextChat.Messages;
            if(Messages.Items.Count>0)
            Messages.ScrollIntoView(Messages.Items[Messages.Items.Count - 1]);
        }

        private void Messages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Dialog per eliminare il messaggio

            Messaggio m = Messages.SelectedItem as Messaggio;

            if (m != null) {

                if (m.IdUtente.Equals(_idUser))
                {

                    MessageBoxResult result = MessageBox.Show(
                       "Vuoi eliminare questo messaggio?",
                       "Conferma eliminazione",
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Warning
                   );

                    // Se l'utente conferma, elimina il messaggio
                    if (result == MessageBoxResult.Yes)
                    {

                        //DELETEMESSAGE#TOKEN#IDMESSAGE#IDCHAT
      
                        _stream.Write("DELETEMESSAGE#" + _token + "#" + m.Id + "#" + contextChat.Id + "#");

                        m.Text = "!!MESSAGGIO ELIMINATO!!";

                    

                    }
                }
                else {

                    MessageBoxResult result = MessageBox.Show(
                       "Non puoi cancellare il messaggio altrui",
                       "Attenzione",
                       MessageBoxButton.OK,
                       MessageBoxImage.Warning
                   );

                }
                // Deseleziona l'elemento per evitare ripetuti popup
                Messages.SelectedItem = null;

            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ELIMINA CHAT

            MessageBoxResult result = MessageBox.Show(
                      "Vuoi eliminare questa chat per sempre?",
                      "Conferma eliminazione",
                      MessageBoxButton.YesNo,
                      MessageBoxImage.Warning
                  );

            // Se l'utente conferma, elimina il messaggio
            if (result == MessageBoxResult.Yes)
            {

                //DELETECHAT#TOKEN#IDCHAT               
                _stream.Write("DELETECHAT#" + _token + "#" + contextChat.Id + "#");
                //  OnChatDeleted(_token,this);


                contextChat = null;

                contactName.Text = "Chat eliminata";
                Messages.ItemsSource = null;
                ChatProfilo.Source = null;



            }
        }

        private void SpunteBlu_Loaded(object sender, RoutedEventArgs e)
        {
            Image spunteBlu = sender as Image;
            if (spunteBlu != null)
            {
                // Trova il messaggio associato all'elemento
                Messaggio messaggio = (spunteBlu.DataContext as Messaggio);

                if (messaggio != null && messaggio.Visualizzato && messaggio.IdUtente.Equals(_idUser))
                {
                    spunteBlu.Visibility = Visibility.Visible;  // Mostra spunta blu
                }
                else
                {
                    spunteBlu.Visibility = Visibility.Collapsed; // Nasconde spunta blu
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                Messaggio message = btn.DataContext as Messaggio;
                if (message != null)
                {
                    string filePath;
                   

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName= message.FileName;
                    if ((bool)saveFileDialog.ShowDialog())
                    {
                        filePath=saveFileDialog.FileName;                

                        byte[] dati = _stream.Receive("GETFILE#" + _token + "#" + message.Id + "#" + contextChat.Id + "#");

                        FileStream fs = new FileStream(filePath, FileMode.Create);

                        fs.Write(dati, 0, dati.Length);
                        fs.Close();

                    }



                }
            }
        }
    }
}
