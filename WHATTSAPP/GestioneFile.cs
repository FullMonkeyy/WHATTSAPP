using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WHATTSAPP_server
{
    static class GestioneFile
    {

        static string pathUtenti = "utenti.xml";
        static string pathChats = "chats.xml";
        static string pathMessages = "messages.xml";

        static Mutex mtx = new Mutex();


        public static List<Utente> LeggiUtenti() {

            List<Utente> lista = new List<Utente>();

            try
            {
                mtx.WaitOne();
                XmlSerializer xmls = new XmlSerializer(typeof(List<Utente>));
                StreamReader sr;
                if (!File.Exists(pathUtenti))
                {

                    ScriviCredenziali(lista);

                }
                sr = new StreamReader(pathUtenti);

                lista = (List<Utente>)xmls.Deserialize(sr);
                sr.Close();
                mtx.ReleaseMutex();



            }
            catch (Exception e)
            {

                Console.WriteLine("errore lettura credenziali: " + e.Message);
                mtx.ReleaseMutex();
            }

            return lista;
        

        
        }

        public static void ScriviCredenziali(List<Utente> lista)
        {

            try
            {
                mtx.WaitOne();
                XmlSerializer xmls = new XmlSerializer(typeof(List<Utente>));
                StreamWriter sw = new StreamWriter(pathUtenti);
                xmls.Serialize(sw, lista);
                sw.Close();
                mtx.ReleaseMutex();


            }
            catch
            {

                Console.WriteLine("errore scrittura credenziali");

            }

        }
        public static void ModificaUtente(Utente u) {

            List<Utente> utenti = LeggiUtenti();



        
        }
        public static List<Chat> ReadChats() {


            List<Chat> lista = new List<Chat>();

            try
            {
                mtx.WaitOne();
                XmlSerializer xmls = new XmlSerializer(typeof(List<Chat>));
                StreamReader sr;
                if (!File.Exists(pathChats))
                {

                    ScriviChat(lista);

                }
                sr = new StreamReader(pathChats);

                lista = (List<Chat>)xmls.Deserialize(sr);
                sr.Close();
                mtx.ReleaseMutex();


            }
            catch (Exception e)
            {

                Console.WriteLine("errore lettura credenziali: " + e.Message);
                mtx.ReleaseMutex();
            }

            return lista;

        
        }
    
        public static Chat ReadChat(int idChat) {

            Chat chat=null;

            try
            {
                mtx.WaitOne();
                XmlSerializer xmls = new XmlSerializer(typeof(List<Chat>));
                StreamReader sr;
                if (!File.Exists(pathChats))
                {

                    ///

                }
                sr = new StreamReader(pathChats);

                List<Chat> lista = (List<Chat>)xmls.Deserialize(sr);
                chat = lista.Find(x => x.Id.Equals(idChat));
                sr.Close();
                mtx.ReleaseMutex();



            }
            catch (Exception e)
            {

                Console.WriteLine("errore lettura credenziali: " + e.Message);
                mtx.ReleaseMutex();
            }

            return chat;

        }
        public static void ScriviChat(List<Chat> chats) {

            try
            {
                mtx.WaitOne();
                XmlSerializer xmls = new XmlSerializer(typeof(List<Chat>));
                StreamWriter sw = new StreamWriter(pathChats);
                xmls.Serialize(sw, chats);
                sw.Close();
                mtx.ReleaseMutex();


            }
            catch
            {

                Console.WriteLine("errore scrittura credenziali");

            }
        
        }


        public static List<Messaggio> LeggiMessaggi()
        {

            List<Messaggio> lista = new List<Messaggio>();

            try
            {
                mtx.WaitOne();
                XmlSerializer xmls = new XmlSerializer(typeof(List<Messaggio>));
                StreamReader sr;
                if (!File.Exists(pathMessages))
                {

                    ScriviCredenziali(lista);

                }
                sr = new StreamReader(pathMessages);

                lista = (List<Messaggio>)xmls.Deserialize(sr);
                sr.Close();
                mtx.ReleaseMutex();


            }
            catch (Exception e)
            {

                Console.WriteLine("errore lettura credenziali: " + e.Message);

            }

            return lista;



        }

        public static void ScriviCredenziali(List<Messaggio> lista)
        {

            try
            {
                mtx.WaitOne();
                XmlSerializer xmls = new XmlSerializer(typeof(List<Messaggio>));
                StreamWriter sw = new StreamWriter(pathMessages);
                xmls.Serialize(sw, lista);
                sw.Close();
                mtx.ReleaseMutex();


            }
            catch
            {

                Console.WriteLine("errore scrittura messaggi");

            }

        }
    
    }
}
