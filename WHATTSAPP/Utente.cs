using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHATTSAPP_server
{
    public class Utente
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public string Token { get; set; }
        public List<int> IdChats { get; set; }
        public Utente() { }
        public Utente(string u, string p, int id) {

            Username = u;
            Password = p;
            Id = id;
            Token = "";



        }

        public override bool Equals(object obj)
        {
            if (!(obj is Utente) || obj == null) return false;

            Utente other = obj as Utente;

            if (other.Username.Equals(Username)) return true;
            else return false;

        }

    }
}
