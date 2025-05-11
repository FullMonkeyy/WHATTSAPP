using Microsoft.Win32;
using System;

using System.IO;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace WHATSAPP_GUI
{
    /// <summary>
    /// Logica di interazione per Registrazione.xaml
    /// </summary>
    public partial class Registrazione : UserControl
{

        NetworkStreamMutexed _stream;
        byte[] img;
        string ext;

        public delegate void MyDelegate(string token);
        public event MyDelegate Registrato;

        public Registrazione(NetworkStreamMutexed st)
        {
            _stream = st;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string user = TXTUsername.Text.Replace('-', '_').Replace('#', '_'); ;
            string password = TXTPassword.Password;

            if (img != null) {

                string command = "REG#" + user + "#" + password + "#" + img.Length + "#" + ext;

                string response = _stream.SendMessage(command,img);

                if (response.Equals("hai fatto la registrazione"))
                {
                    TextError.Text = response;
                }
                else
                {

                    TextError.Text = response;

                }

            }
        }
        // Gestisce il click sul pulsante "Scegli immagine"
        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Configura il dialogo per la selezione dei file immagine
            OpenFileDialog openFileDlg = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All Files (*.*)|*.*"
            };

            // Se l'utente seleziona un file
            if (openFileDlg.ShowDialog() == true)
            {
                try
                {
                    // Carica l'immagine selezionata
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDlg.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    // Imposta l'immagine nel controllo anteprima
                    ProfileImagePreview.Source = bitmap;
             

                    FileStream fs = new FileStream(openFileDlg.FileName, FileMode.Open);
                    ext=System.IO.Path.GetExtension(openFileDlg.FileName);
                    img = new byte[fs.Length];
                    fs.Read(img, 0, img.Length);
                    fs.Close();

                }
                catch (Exception)
                {
                    // Mostra un messaggio di errore in caso di problemi nel caricamento
                    TextError.Text = "Errore nel caricamento dell'immagine.";
                }




            }
        }
        
    }
}
