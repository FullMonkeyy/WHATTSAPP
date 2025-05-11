using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WHATTSAPP_server
{
    public class Chat
    {

        public int Id { get; set; }
        public int Id_1 { get; set; }
        public bool isAcceted1 { get; set; }
        public DateTime LastVisualized1 { get; set; }
        public int Id_2 { get; set; }
        public bool isAcceted2 { get; set; }
        public DateTime LastVisualized2 { get; set; }
        
        public List<Messaggio> Messages { get; set; }
        public string PATHCHAT { get; set; }
        public Chat() {

            isAcceted2 = false;
            isAcceted1 = false;


        }
        public Chat(int Id, int Id1, int Id2) {

            this.Id = Id;
            Id_1 = Id1;
            Id_2 = Id2;
            PATHCHAT = "Chats/"+this.Id + "";
            isAcceted2 = false;
            isAcceted1 = false;
        }

        public int MessaggiDaLeggere(int id)
        {

            DateTime tmp=DateTime.MinValue;
            if (Id_2.Equals(id))
            {
                tmp = LastVisualized2;
     
            }
            else if (Id_1.Equals(id))
            {
                tmp = LastVisualized1;
           
            }


            if (tmp != DateTime.MinValue) {
                int daLeggere = Messages.FindAll(x => x.IdUtente != id && x.dataora > tmp).Count();

                return daLeggere;
            }
            else return 0;

        }
        public int MessaggiCheDeveLeggereLui(int id)
        {

            DateTime tmp = DateTime.MinValue;
      
            if (Id_2.Equals(id))
            {
                tmp = LastVisualized1;
                
            }
            else if (Id_1.Equals(id))
            {
                tmp = LastVisualized2;
              
            }


            if (tmp != DateTime.MinValue)
            {
                int daLeggere = Messages.FindAll(x => x.IdUtente == id && x.dataora > tmp).Count();

              
                return daLeggere;
            }
            else return 0;


        }

    }
}
