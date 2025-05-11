using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WHATTSAPP_server
{
    class BuisnessLogic
    {

        TcpClient connes;
        public BuisnessLogic(TcpClient connessione)
        {

            connes = connessione;
            new Thread(Attivita).Start();
        }
        public void Attivita()
        {


            NetworkStream canale = connes.GetStream();

            byte[] dati = new byte[1024];

            string[] cmd = null;
            string username, password,totale,rispo;
            int nletti,a,b,c,d;

            do
            {

                try
                {
                    dati = new byte[1024];
                    nletti = canale.Read(dati, 0, dati.Length);
                    totale = Encoding.ASCII.GetString(dati);
                    totale=totale.TrimEnd('\0');
                    cmd = totale.Split('#');
                    Console.WriteLine("MESSAGGIO: \n" + Encoding.ASCII.GetString(dati));

                    switch (cmd[0])
                    {

                        case "REG":

                            username = cmd[1];
                            password = cmd[2];
                            
                            int id=0;
                            List<Utente> utenti = GestioneFile.LeggiUtenti();

                            if (!utenti.Exists(x => x.Username.Equals(username)))
                             {
                                 if(utenti.Count>0)
                                 id = utenti[utenti.Count - 1].Id+1;

                                 utenti.Add(new Utente(username, password, id));
                                

                                if (cmd.Count() > 2)
                                {


                                    if (!Directory.Exists("Users"))
                                        Directory.CreateDirectory("Users");

                                    string dir = "Users\\" + id;
                                    Directory.CreateDirectory(dir);

                                    int dim = int.Parse(cmd[3]);
                                    string extension = cmd[4].Trim('\0');

                                    dati = Encoding.ASCII.GetBytes("MANDA FOTO");
                                    canale.Write(dati, 0, dati.Length);
                                    byte[] bufferimag = new byte[dim];
                                    canale.Read(bufferimag, 0, bufferimag.Length);

                                    string path = Path.Combine(dir, "PhotoProfile" + extension);

                                    FileStream fs = new FileStream(path, FileMode.Create);
                                    fs.Write(bufferimag, 0, bufferimag.Length);
                                    fs.Close();


                                    GestioneFile.ScriviCredenziali(utenti);


                                }




                                rispo = "hai fatto la registrazione";
                                 Console.WriteLine(rispo);
                             }
                             else
                             {

                                 rispo = "Hai sbagliato, questo username esiste già";
                                 Console.WriteLine(rispo);

                             }

                             dati = Encoding.ASCII.GetBytes(rispo);
                             canale.Write(dati, 0, dati.Length);

                        
                           
                            break;
                        case "LOG":

                            username = cmd[1];
                            password = cmd[2];
                            utenti = GestioneFile.LeggiUtenti();
                            if (utenti.Exists(x => x.Username.Equals(username)))
                            {

                                Utente user = utenti.Find(x => x.Username.Equals(username));
                                if (user.Password.Equals(password))
                                {

                                    user.Token = DateTime.Now.GetHashCode()+user.Username.GetHashCode()+"";

                                    GestioneFile.ScriviCredenziali(utenti);

                                    rispo = user.Token + "#" + user.Id+"#"+user.Username;
                                    Console.WriteLine("hai fatto l'accesso. TOKEN: " + rispo);

                                }
                                else
                                {

                                    rispo = "accesso rifiutato";
                                    Console.WriteLine("accesso rifiutato");

                                }
 
                            }
                            else
                            {
                                rispo = "Utente non trovato";
                                Console.WriteLine(rispo);
                            }



                            dati = Encoding.ASCII.GetBytes(rispo);
                            canale.Write(dati, 0, dati.Length);
                           

                            break;
                        case "GETCHATS":

                            //GETCHATS#TOKEN#
                            string token = cmd[1];
                            token = token.Trim('\0');
                            utenti = GestioneFile.LeggiUtenti();
                                
                        
                            if (utenti.Exists(X => X.Token.Equals(token)))
                            {

                                Utente user = utenti.Find(x=> x.Token.Equals(token));
                                List<Chat> chats = GestioneFile.ReadChats().FindAll(x => x.Id_1.Equals(user.Id) || x.Id_2.Equals(user.Id));
                                string msg = "";
                                foreach (Chat ch in chats) {

                                    msg += ch.Id + "-" + utenti.Find(x => x.IdChats.Contains(ch.Id) && x.Token != token).Username + "-" + utenti.Find(x => x.IdChats.Contains(ch.Id) && x.Token != token).Id+ "#";
                                
                                }
                                if (msg.Length == 0) 
                                {

                                    msg = "Aggiungi una nuova chat dai";

                                }
                                dati = Encoding.ASCII.GetBytes(msg);    
                                
                                

                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando get chats");                             
                            }

                            canale.Write(dati, 0, dati.Length);


                            break;
                        case "CREATECHAT":


                            //CREATECHAT#TOKEN#USERNAME

                            token = cmd[1];
                            token = token.Trim('\0');
                            utenti = GestioneFile.LeggiUtenti();
                                
                        
                            if (utenti.Exists(X => X.Token.Equals(token)))
                            {

                                string msg="";
                                Utente u = utenti.Find(x => x.Username.Equals(Encoding.ASCII.GetString(dati).Split('#')[2]));
                                Utente ut = utenti.Find(x => x.Token.Equals(token));
                                List<Chat> chats=GestioneFile.ReadChats(); 
                                Chat chat = chats.Find(x => x.Id_1.Equals(u.Id) && x.Id_2.Equals(ut.Id) || x.Id_2.Equals(u.Id) && x.Id_1.Equals(ut.Id));
                                if (chat == null) { 
                                
                                    int ID=0;
                                    if(chats.Count>0)
                                        ID=chats[chats.Count-1].Id+1;
                                    chat = new Chat(ID, ut.Id, u.Id);
                                    chat.isAcceted1 = true;
                                    chats.Add(chat);
                                    if (!Directory.Exists("Chats"))
                                        Directory.CreateDirectory("Chats");

                                    Directory.CreateDirectory(chat.PATHCHAT);

                                    u.IdChats.Add(ID);
                                    ut.IdChats.Add(ID);
                                    GestioneFile.ScriviChat(chats);
                                    GestioneFile.ScriviCredenziali(utenti);

                                    msg = "chat creata";
                                
                                }
                                else {

                                    msg = "chat già esistente";
                                
                                }
                               
                                
                                dati = Encoding.ASCII.GetBytes(msg);    
                                                         

                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando get chats");                             
                            }

                            canale.Write(dati, 0, dati.Length);


                            break;
                        case "GETCHAT":
                            //GETCHAT#TOKEN#IDCHAT#!BONUS!

                            token = cmd[1];
                            token = token.Trim('\0');
                            utenti = GestioneFile.LeggiUtenti();
                            int idchat;    
                        
                            if (utenti.Exists(X => X.Token.Equals(token)) && int.TryParse(cmd[2],out idchat))
                            {

                                List<Chat> chats = GestioneFile.ReadChats();
                                Chat chat=chats.Find(x => x.Id == idchat && utenti.Find(y => y.Token.Equals(token)).IdChats.Contains(idchat));                              
                                Utente u = utenti.Find(x => x.Token.Equals(token));
                                bool davisualizzare = false;
                                if (cmd.Length == 4)
                                    if (cmd[3] == "VISUALIZZA")
                                        davisualizzare = true;

                                if(chat.Id_2.Equals(u.Id))
                                chat.isAcceted2 = true;
                                string msg;
                                if (chat == null)
                                    msg = "CHAT NON ESISTENTE";
                                else if (chat.Messages.Count > 0)
                                {
                                    msg = "";

                                    foreach (Messaggio m in chat.Messages)
                                    {

                                        msg += m.IdUtente + "-" + m.pathImg + "-" + m.Text + "-" + m.dataora + "-" + m.Id + "-"+m.Letto+"-"+m.Visualizzato.ToString()+"#";
                                        if (!m.IdUtente.Equals(u.Id))
                                            m.Letto = true;

                                        if (davisualizzare && !m.IdUtente.Equals(u.Id))
                                            m.Visualizzato = true;
                                                                                                                     

                                    }
                                    
                                    GestioneFile.ScriviChat(chats);

                                }
                                else msg = "NO MESSAGGI";

                              
                                dati = Encoding.ASCII.GetBytes(msg);   

                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando get chats");                             
                            }

                            canale.Write(dati, 0, dati.Length);


                            break;
                        case "ADDMESSAGE":
                            //ADDMESSAGE#TOKEN#IDCHAT#IDUTENTE#TESTO#PATHIMAGINE#NUMBYTE#DATAORA

                            token = cmd[1];
                            token = token.Trim('\0');
                            utenti = GestioneFile.LeggiUtenti();
                   


                            if (utenti.Exists(X => X.Token.Equals(token)) && int.TryParse(cmd[2], out idchat))
                            {
                                List<Chat> chats= GestioneFile.ReadChats();
                                Chat chat = chats.Find(x => x.Id == idchat && utenti.Find(y => y.Token.Equals(token)).IdChats.Contains(idchat));
                                Utente u = utenti.Find(x => x.Token.Equals(token));
                                string msg = "";
                                string testo = cmd[3];
                                string nomefile= cmd[4];
                                
                                if ((testo.Length > 0 || nomefile.Length > 0) && chat!=null) {

                                    if (nomefile.Length > 0) { 
                                    
                                        dati = Encoding.ASCII.GetBytes("MANDA FOTO");
                                        canale.Write(dati, 0, dati.Length);
                                        int dim= int.Parse(cmd[5]);
                                        byte[] bufferimag = new byte[dim];
                                        canale.Read(bufferimag, 0, bufferimag.Length);
                                        int ii = 0;
                                        nomefile = chat.PATHCHAT + "/" + nomefile;
                                        string tmp = "";
                                        List<string> list = new List<string>() { "0","1", "2", "3", "4", "5", "6", "7", "8", "9", };
                                        while (File.Exists(nomefile)) {

                                            if (list.TrueForAll(x => !nomefile.Split('.')[0].EndsWith(x)))
                                            {
                                                tmp = nomefile.Split('.')[0] + ii;

                                            }
                                            else
                                            {
                                                tmp = nomefile.Split('.')[0].Remove(nomefile.Split('.')[0].Length - 1) + ii;
                                            }
        
                                            nomefile = tmp+"."+ nomefile.Split('.')[1];
     
                                            ii++;

                                        }

                                        FileStream fs = new FileStream(nomefile, FileMode.Create);
                                        fs.Write(bufferimag, 0, bufferimag.Length);
                                        fs.Close();  
                                        int idtmp=0;
                                        if(chat.Messages.Count>0)
                                        idtmp = chat.Messages[chat.Messages.Count-1].Id+1;
                                        
                                        chat.Messages.Add(new Messaggio(testo, idtmp, idchat, u.Id, nomefile));
                                        msg = "messaggio con foto aggiunto ";

                                       

                                   }
                                   else{
                                    
                                        int idtmp=0;
                                        if(chat.Messages.Count>0)
                                        idtmp = chat.Messages[chat.Messages.Count-1].Id+1;

                                        chat.Messages.Add(new Messaggio(testo, idtmp, idchat, u.Id));
                                        msg = "messaggio senza foto aggiunto ";
                                    
                                  }

                                    GestioneFile.ScriviChat(chats);
                                    
                                
                                
                                }
                                else {

                                    msg = "devi mandare qualcosa dai...";
                                
                                }


                                dati = Encoding.ASCII.GetBytes(msg);   

                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando get chats");                             
                            }

                            canale.Write(dati, 0, dati.Length);
                            
                            
                            break;
                        case "GETFILE":

                            //GETFILE#TOKEN#IDMESSAGE#IDCHAT
                            utenti = GestioneFile.LeggiUtenti();
                            token = cmd[1];
                            token=token.Trim('\0');
                            int idme=0;
                            if (utenti.Exists(X => X.Token.Equals(token)) && int.TryParse(cmd[3],out idchat) && int.TryParse(cmd[2], out idme) )
                            {
                                Utente u = utenti.Find(x => x.Token.Equals(token));
                                Chat chat=GestioneFile.ReadChats().Find(x => x.Id.Equals(idchat));
                                Messaggio m = chat.Messages.Find(x => x.Id.Equals(idme));

                                if(m.Id==8 && chat.Id == 7)
                                {
                                    Console.WriteLine(" ");
                                }

                                string nomefile = m.pathImg;
                               
                                    FileInfo fileInfo = new FileInfo(nomefile);
                                    long fileSize = fileInfo.Length;

                                    FileStream fs = new FileStream(nomefile, FileMode.Open);
                                    byte[] bufferimg = new byte[(int)fileSize];
                                    fs.Read(bufferimg, 0, bufferimg.Length);




                                    byte[] dim = Encoding.ASCII.GetBytes("" + fs.Length);
                                    canale.Write(dim, 0, dim.Length);
                                    dim = new byte[20];
                                    canale.Read(dim, 0, dim.Length);
                                    Console.WriteLine("Messaggio client: " + Encoding.ASCII.GetString(dim));
                                    canale.Write(bufferimg, 0, bufferimg.Length);
                                    fs.Close();
                                
                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando upload");
                                canale.Write(dati, 0, dati.Length);
                            }

           

                            break;
                        case "DELETEMESSAGE":

                            //DELETEMESSAGE#TOKEN#IDMESSAGE#IDCHAT

                            utenti = GestioneFile.LeggiUtenti();
                            token = cmd[1];
                            token=token.Trim('\0');


                            
                            if (utenti.Exists(X => X.Token.Equals(token)) && int.TryParse(cmd[3],out idchat) && int.TryParse(cmd[2], out idme) )
                            {
                                Utente u = utenti.Find(x => x.Token.Equals(token));
                                List<Chat> chats=GestioneFile.ReadChats();
                                Chat chat = chats.Find(x => x.Id.Equals(idchat));
                                Messaggio m = chat.Messages.Find(x => x.Id.Equals(idme));

                                m.Text = "!!MESSAGGIO ELIMINATO!!";
                                m.Letto = false;
                                m.Removed = true;
                                if (m.pathImg!= "NULL") {

                                    File.Delete(m.pathImg);
                                    m.pathImg = "NULL";
                                
                                }

                                GestioneFile.ScriviChat(chats);                            

                                dati = Encoding.ASCII.GetBytes("Messaggio ID: " + idme + " CHAT ID: " + idchat + " eliminato");
                                canale.Write(dati, 0, dati.Length);

                                
                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando upload");
                                canale.Write(dati, 0, dati.Length);
                            }


                            break;
                        case "DELETECHAT":

                            //DELETECHAT#TOKEN#IDCHAT

                            utenti = GestioneFile.LeggiUtenti();
                            token = cmd[1];
                            token = token.Trim('\0');
                            
                            if (utenti.Exists(X => X.Token.Equals(token)) && int.TryParse(cmd[2], out idchat))
                            {
                                Utente u = utenti.Find(x => x.Token.Equals(token));
                                List<Chat> chats = GestioneFile.ReadChats();
                                Chat chat = chats.Find(x => x.Id.Equals(idchat));
                                string mes;

                                if (chat != null)
                                {

                                    u.IdChats.Remove(chat.Id);

                                    Utente altro = utenti.Find(x => x.Id.Equals(chat.Id_2));
                                    altro.IdChats.Remove(chat.Id);

                                    chats.Remove(chat);


                                    GestioneFile.ScriviChat(chats);
                                    GestioneFile.ScriviCredenziali(utenti);
                                    mes = "CHAT ID: " + idchat + " eliminata";
                                }
                                else
                                {
                                    mes= "CHAT ID: " + idchat + " non trovata";
                                }
                                    dati = Encoding.ASCII.GetBytes(mes);
                                    canale.Write(dati, 0, dati.Length);
                                                             
                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando delete chat");
                                canale.Write(dati, 0, dati.Length);
                            }


                            break;
                        case "GETUSERS":
                            //GETUSERS#TOKEN#
                            utenti = GestioneFile.LeggiUtenti();
                            token = cmd[1];
                            token = token.Trim('\0');
                            
                            if (utenti.Exists(X => X.Token.Equals(token)))
                            {
                                Utente u = utenti.Find(x => x.Token.Equals(token));
                                List<Chat> chats = GestioneFile.ReadChats();

                                string tmp="";
                                List<Chat> chatuser = chats.FindAll(x => x.Id_1.Equals(u.Id) || x.Id_2.Equals(u.Id));
                                if (chatuser.Count > 0)
                                    foreach (Utente user in utenti)
                                    {


                                        if (user.Token.Equals(token) || chatuser.Exists(x => x.Id_1.Equals(user.Id) || x.Id_2.Equals(user.Id)))
                                            continue;
                                        tmp += user.Id + "-" + user.Username + "#";
                                    }
                                else {

                                    foreach (Utente user in utenti)
                                    {


                                        if (user.Token.Equals(token) )
                                            continue;
                                        tmp += user.Id + "-" + user.Username + "#";
                                    }

                                }

                                if (tmp.Length == 0)
                                    tmp = "NESSUN CONTATTO DISPONIBILE";

                                dati = Encoding.ASCII.GetBytes(tmp);
                                canale.Write(dati, 0, dati.Length);
                                
                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando delete chat");
                                canale.Write(dati, 0, dati.Length);
                            }

                            break;
                        case "SHOULDIUPDATE":

                            //SHOULDIUPDATE#TOKEN#IDCHAT#BONUS

                            utenti = GestioneFile.LeggiUtenti();
                            token = cmd[1];
                            token = token.Trim('\0');
                            
                            if (utenti.Exists(X => X.Token.Equals(token)) && int.TryParse(cmd[2], out idchat))
                            {
                                Utente u = utenti.Find(x => x.Token.Equals(token));
                                List<Chat> chats = GestioneFile.ReadChats();
                                Chat chat = chats.Find(x => x.Id.Equals(idchat));
                                
                                string tmp = "no";

                                if (chat != null)
                                {
                                    if (chat.Messages.Exists(x => !x.IdUtente.Equals(u.Id) && !x.Letto || x.Removed))
                                    {

                                        tmp = "yes";


                                    }

                                    if (cmd.Length == 4)
                                        if (cmd[3].Equals("VISUALIZZA") && chat.Messages.Exists(x => x.IdUtente.Equals(u.Id) && x.Visualizzato && !x.ControVisualizzato))
                                        {


                                            chat.Messages.FindAll(x => x.IdUtente.Equals(u.Id)).ForEach(x => x.ControVisualizzato = true);
                                            tmp = "yes";


                                        }

                                    GestioneFile.ScriviChat(chats);

                                  
                                  
                                    
                                    
                                    
                                    if (chat.Messages.Exists(x => x.IdUtente.Equals(u.Id) && x.Removed))
                                    {

                                        chat.Messages.ForEach(x =>
                                        {

                                            if (x.Removed)
                                                x.Removed = false;

                                        });
                                        GestioneFile.ScriviChat(chats);

                                    }


                                }
                                else {

                                    tmp = "CHAT REMOVED";
                                
                                }
                          
                                dati = Encoding.ASCII.GetBytes(tmp);
                                canale.Write(dati, 0, dati.Length);

                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando should i update chat");
                                canale.Write(dati, 0, dati.Length);
                            }

                            break;
                        case "GETPFP":

                            //GETPFP#TOKEN#IDUTENTE
                            token = cmd[1];
                            token = token.Trim('\0');
                            utenti = GestioneFile.LeggiUtenti();


                            if (utenti.Exists(X => X.Token.Equals(token)))
                            {
                                try
                                {

                                    string extension = Directory.GetFiles("Users\\" + cmd[2]).Count()>0? Path.GetExtension( Directory.GetFiles("Users\\" + cmd[2])[0]): null;




                                    FileStream fs = new FileStream("Users\\" + cmd[2] + "\\PhotoProfile"+ extension, FileMode.Open);

                                    string dim= fs.Length+"#"+ extension;
                                    dati = Encoding.ASCII.GetBytes(dim);
                                    canale.Write(dati,0,dati.Length);
                                    dati = new byte[20];
                                    canale.Read(dati,0,dati.Length);


                                    byte[] data = new byte[fs.Length];
                                    fs.Read(data, 0, data.Length);
                                    fs.Close();
                                    dati = data;
                                }
                                catch {

                                    dati = Encoding.ASCII.GetBytes("Pfp non trovata");

                                }   


                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("non autorizzato al comando get PFP");
                            }

                            canale.Write(dati, 0, dati.Length);


                            break;
                        case "ISTHERENEWCHAT":

                            //ISTHERENEWCHAT#TOKEN#IDMESSAGE#IDCHAT
                            token = cmd[1];
                            token = token.Trim('\0');
                            utenti = GestioneFile.LeggiUtenti();


                            if (utenti.Exists(X => X.Token.Equals(token)))
                            {

                                Utente u = utenti.Find(x => x.Token.Equals(token));
                                List<Chat> chats = GestioneFile.ReadChats().FindAll(x => x.Id_1.Equals(u.Id) || x.Id_2.Equals(  u.Id));
                                string tmp="no";
                            
                                foreach (Chat chatt in chats.FindAll(x => x.Id_1.Equals(u.Id) || x.Id_2.Equals(u.Id)))
                                {

                                    if (chatt.Id_1.Equals(u.Id))
                                    {
                                        if (!chatt.isAcceted1)
                                        {
                                          
                                            dati = Encoding.ASCII.GetBytes("nuovachat");
                                            break;
                                         }

                                    }
                                    else
                                    {
                                        if (!chatt.isAcceted2)
                                        {
                                            dati = Encoding.ASCII.GetBytes("nuovachat");
                                            break;
                                         }
                                     }
                                }


                            }
                            else
                            {
                                dati = Encoding.ASCII.GetBytes("nigger");
                            }

                            canale.Write(dati, 0, dati.Length);

                            break;

                    }



                }
                catch (Exception e)
                {

                    Console.WriteLine("Richiesta non compresa: " + e.Message);
                    break;
                }




            } while (!cmd[0].Equals("OUT"));




        }

    }
}
