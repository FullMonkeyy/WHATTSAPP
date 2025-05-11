using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

using System.Windows.Media.Imaging;
using System.Drawing;
using WHATSAPP_GUI.Properties;

namespace WHATSAPP_GUI
{
    public class Messaggio
    {

        public string Text { get; set; }
        public string pathImg { get; set; }
        public int Id { get; set; }
        public int IdChat { get; set; }
        public int IdUtente { get; set; }
        public DateTime dataora { get; set; }
        public bool Letto { get; set; }
        public bool Visualizzato { get; set;}
        public string Color { get; set; }
        public string Alignment { get; set; }
        public bool Removed { get; set; } 
        public bool NoPhoto { get; set; }
        public string FileName {

            get
            {

                if(pathImg.Length>0) return "Scarica "+Path.GetFileName(pathImg); 
                else return null;

            }                
                
         }

        public BitmapImage Immagine
        {
            get
            {
                if(pathImg!="NULL"){

                    try
                    {

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(pathImg, UriKind.Relative);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        return bitmap;

                    }
                    catch {


                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("pack://siteoforigin:,,,/Resources/istockphoto.jpg");                  
                        bitmap.EndInit();
                        return bitmap;
              

                    }
                }
                return null;

            }
        }

        public Messaggio() { }

        public Messaggio(string text, int id,int idc, int idu) {

            Text = text;
            Id=id;
            IdChat = idc;
            IdUtente = idu;
            dataora = DateTime.Now;
            pathImg = "NULL";
            Letto = false;
            Visualizzato = false;
            NoPhoto = true;
        }
        public Messaggio(string text, int id, int idc, int idu, string pathIMG)
        {

            Text = text;
            Id = id;
            IdChat = idc;
            IdUtente = idu;
            dataora = DateTime.Now;
            pathImg = pathIMG;
            Letto = false;
            Visualizzato = false;
        }


    }
}
