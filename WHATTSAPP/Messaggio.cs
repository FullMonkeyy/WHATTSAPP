using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHATTSAPP_server
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
        public bool Removed { get; set; }
        public bool Visualizzato { get; set; }
        public bool ControVisualizzato { get; set; }

        //Chat ti calcola il numero di messaggi da leggere. Visualizzato è un flag utile per indicare se un messaggio è stato visualizzato. 
        // se get chat ottinee il 4 parametro significa che si vogliono marcare come visualizzato tutti i messaggi

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
            ControVisualizzato = false;
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
            ControVisualizzato = false;


        }


    }
}
