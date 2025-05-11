using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WHATSAPP_GUI
{
    public class Chat
    {

        public int Id { get; set; }
        public string nomecontact { get; set; }
        public int Id_2 { get; set; }
        public int Id_1 { get; set; }
        public List<Messaggio> Messages { get; set; }
        public string pathImg {  get; set; }
        public bool Notification {  get; set; }
        public SolidColorBrush ColorNotification
        {

            get
            {

                if (Notification)
                    return Brushes.Green;
                else return Brushes.Transparent;

            }

        }
        public Messaggio  LastMessage { get {

                if (Messages.Count > 0)
                {

                    return Messages[Messages.Count - 1];

                }
                else return null;

            
        } }
        public string PATHCHAT { get; set; }
        public BitmapImage Immagine
        {
            get
            {              

                    try
                    {

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(pathImg, UriKind.Relative);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        return bitmap;

                    }
                    catch
                    {


                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("pack://siteoforigin:,,,/Resources/profile-icon-design-free-vector.jpg");
                        bitmap.EndInit();
                        return bitmap;


                    }               
              
            }
        }

        public Chat() {

            Notification = false;

        }
        public Chat(int Id, int Id2) {

            this.Id = Id;
        
            Id_2 = Id2;
            PATHCHAT = "Chats/"+this.Id + "";
                    
        }



    }
}
