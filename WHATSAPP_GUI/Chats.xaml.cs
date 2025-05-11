using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
using static System.Net.WebRequestMethods;
using System.Runtime.InteropServices;


namespace WHATSAPP_GUI
{
    /// <summary>
    /// Logica di interazione per Chats.xaml
    /// </summary>
    public partial class Chats : UserControl
    {
        NetworkStreamMutexed _stream;
        List<Chat> chats;
        Chat contextChat;
        string _token;
        int idUser;
        List<dynamic> utenti;
        Thread CheckUpdates;
        Thread _tmpthread;
        Mutex mtx;
        CancellationTokenSource cts;
        CancellationTokenSource cts2;
       
       
        ChatVisualizer cv;

        public Chats(NetworkStreamMutexed st,string t,int IdUser)
        {
            _stream = st;
            chats = new List<Chat>();
            InitializeComponent();
   
          
            utenti = new List<dynamic>();
            _token = t;
            idUser = IdUser;
            mtx=new Mutex();
            cts = new CancellationTokenSource();
            cts2= new CancellationTokenSource();

        }



        public void GetChats() 
        {


            mtx.WaitOne();
            Chat chtmp;
            chats.Clear();
            List<string> tt = _stream.Write("GETCHATS#" + _token + "#").Split('#').ToList();
            string ss;
            byte[] pfp;
            foreach (string str in tt)
            {
                             


                if (str.Equals("Aggiungi una nuova chat dai"))
                {
                    break;
                }
                else
                {

                    chtmp = new Chat();
                    if (str.Split('-').ToList().Count == 3)
                    {
                        //chtmp.Id = int.Parse(str.Split('-')[0]);
                       
                        
                        ss = str.Split('-')[0];

                        if (!Directory.Exists("download\\" + ss))
                            Directory.CreateDirectory("download\\"+ss);

                        chtmp = GetChat(_token,ss, str.Split('-')[1], str.Split('-')[2]);

                        //GETPFP#TOKEN#IDUTENTE

                        pfp = _stream.Receive("GETPFP#" + _token + "#" + str.Split('-')[2]);

                        if (pfp != null)
                        {

                            string pathdir = "contactsImages\\" + str.Split('-')[2];

                            if (!Directory.Exists(pathdir))
                                Directory.CreateDirectory(pathdir);

                            pathdir = pathdir + "\\Photo" + _stream.contextExtension;

                            FileStream fileStream = new FileStream(pathdir, FileMode.Create);

                            fileStream.Write(pfp, 0, pfp.Length);
                            fileStream.Close();
                            chtmp.pathImg = pathdir;
                        }                                            

                        if (chtmp!=null)
                        chats.Add(chtmp);

                    }
                }
            }

            Dispatcher.Invoke(() =>
            {

                Listachatss.ItemsSource = null;
                if (chats.Count > 0)
                    Listachatss.ItemsSource = chats;

            });
   
         
            mtx.ReleaseMutex();

        }
        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            GetChats();
            CheckUpdates = new Thread(() =>{ RequestUpdate(cts.Token); });
            CheckUpdates.Start();
            CheckUpdates.IsBackground= true;
            _tmpthread = new Thread(() => { checking(cts2.Token); });
            _tmpthread.Start();
            _tmpthread.IsBackground = true;


        }

        private void Listachatss_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            contextChat = Listachatss.SelectedItem as Chat;
            if(contextChat!=null)
            UpdateChat(_token, contextChat.Id.ToString());


        }
        public string GetFile(int idMessage, int idChat, string filename,string directory){
    
              //GETFILE#TOKEN#IDMESSAGE#IDCHAT


            if(!(bool)System.IO.File.Exists(directory + "\\"+filename)){                   

                byte[] file = _stream.Receive("GETFILE#" + _token + "#" + idMessage + "#" + idChat + "#");
                            
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                filename=System.IO.Path.GetFileName(filename);
                FileStream fileStream = new FileStream(directory + "\\" + filename, FileMode.Create);
                fileStream.Write(file, 0, file.Length);
                fileStream.Close();

            }
            return directory + "\\" + filename;
        
    
        }
        public Chat GetChat(string token, string idChat,string nome, string id2,bool davisualizzare=false) {


            Chat chat = new Chat();
            chat.Id = int.Parse(idChat);
            chat.nomecontact = nome;
            chat.Id_2 = int.Parse(id2);           
            string comandino= "GETCHAT#" + _token + "#" + idChat;
            if(davisualizzare)
                comandino= "GETCHAT#" + _token + "#" + idChat+"#VISUALIZZA";


            string strtt = _stream.Write(comandino);
            if (strtt.Equals("CHAT NON ESISTENTE"))
            {
                return null;
            }
            else
            {
                string[] messaggi = strtt.Trim('\0').Split('#');
                string ex;
                chat.Messages = new List<Messaggio>();
                Messaggio m;
                // msg += m.IdUtente + "-" + m.pathImg + "-" + m.Text + "-" + m.dataora + "#";
                List<string> strings = new List<string>() {"png","jpeg","jpg", "bmp" };
                foreach (string str in messaggi)
                {
                    if (str.Split('-').ToList().Count == 7)
                    {
                        m = new Messaggio();
                        m.IdUtente = int.Parse(str.Split('-')[0]);
                        m.pathImg = str.Split('-')[1].Replace('-', '_').Replace('#', '_').Replace('%', '_');
                        m.Id = int.Parse(str.Split('-')[4]);
                        m.Visualizzato=bool.Parse(str.Split('-')[6]);
                        if (m.pathImg.Length > 0 && m.pathImg != "NULL")
                        {

                            ex = (m.pathImg.Split('.').Count() > 0 ? m.pathImg.Split('.')[m.pathImg.Split('.').Count() - 1] : "cazzzeee");
                            if (strings.Contains(ex))
                            {
                                m.NoPhoto = false;

                                string nf = GetFile(m.Id, chat.Id, m.pathImg,"download\\"+chat.Id);
                                //m.Immagine = new BitmapImage(new Uri());
                                m.pathImg = nf;

                            }
                            else { 
                            
                                m.NoPhoto = true;
                            
                            }

                        }
                        m.Text = str.Split('-')[2];
                        m.dataora = DateTime.Parse(str.Split('-')[3]);
                        string tttt = str.Split('-')[5];
                        if (tttt.Contains("True"))
                            m.Letto = true;
                        else m.Letto = false;

                        if (m.IdUtente.Equals(chat.Id_2))
                        {
                            m.Alignment = "Left";
                            m.Color = "#FFFFFF";
                        }
                        else
                        {
                            m.Color = "#DCF8C6";
                            m.Alignment = "Right";
                        }



                        chat.Messages.Add(m);
                    }

                }
            }
            return chat;


        }
        public void UpdateChat(string token,string idChat) {
                                




            if (contextChat != null)
            {
                if(contextChat.Id.Equals(int.Parse(idChat)))
                Dispatcher.Invoke(() =>
                {
                    Chat tmp = chats.Find(x => x.Id.Equals(int.Parse(idChat)));
                    tmp.Notification = false;
                  
                    Listachatss.ItemsSource = null;
                    Listachatss.ItemsSource = chats;
               
                    contextChat = GetChat(token, idChat, tmp.nomecontact, tmp.Id_2 + "",true);
                    string pathdir = "contactsImages\\" + tmp.Id_2;

                    if (!Directory.Exists(pathdir))
                        Directory.CreateDirectory(pathdir);

                    string[] cose = Directory.GetFiles(pathdir);


                    if (cose.Length == 0){

                        byte[] pfp = _stream.Receive("GETPFP#" + _token + "#" + contextChat.Id_2);

                        if (pfp != null)
                        {



                            pathdir = pathdir + "\\Photo" + _stream.contextExtension;

                            FileStream fileStream = new FileStream(pathdir, FileMode.Create);

                            fileStream.Write(pfp, 0, pfp.Length);
                            fileStream.Close();
                            contextChat.pathImg = pathdir;
                        }
                    }
                    else
                    {

                        string filename = Directory.GetFiles(pathdir)[0];
                        contextChat.pathImg = filename;

                    }

                    cv = new ChatVisualizer(_stream, _token, contextChat, idUser);
                    cv.RequestedUpdate += UpdateChat;
                    //cv.OnChatDeleted += OnChatDeleted;
                    Grid.SetColumn(cv, 1);
                    cv.Name = "savds";

                    Maingrid.Children.Add(cv);

                });

            }
          

        }
        private void checking(CancellationToken ct2)
        {

            while (!ct2.IsCancellationRequested)
            {

                if (!CheckUpdates.IsAlive)
                {

                    GetChats();
                    CheckUpdates = new Thread(() => { RequestUpdate(cts.Token); });
                    CheckUpdates.Start();
                    CheckUpdates.IsBackground = true;

                }

                Thread.Sleep(200);
            }

        }
 
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            PopupGrid.Visibility = Visibility.Visible;         
           
            string[] utentiSTR = _stream.Write("GETUSERS#" + _token + "#").TrimEnd('#').Split('#');
            dynamic obj;
            utenti.Clear();
            if (!utentiSTR[0].Equals("NESSUN CONTATTO DISPONIBILE"))
            {
                foreach (string str in utentiSTR)
                {

                    obj = new ExpandoObject();
                    obj.Id = str.Split('-')[0];
                    obj.Username = str.Split('-')[1];
                    utenti.Add(obj);
                }
                UserListBox.ItemsSource = null;
                UserListBox.ItemsSource = utenti;
            }
            else{
                obj = new ExpandoObject();
                obj.Id = "sto cacco";
                obj.Username = "Nessun utente trovato";
                utenti.Add(obj);
                UserListBox.ItemsSource = utenti;
            }

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchBox.Text.ToLower();
            UserListBox.ItemsSource = null;
            if(filter.Length>0){
     
                UserListBox.ItemsSource = utenti.Where(u => u.Username.ToLower().Contains(filter)).ToList();
            }
            else
            {
                UserListBox.ItemsSource = utenti;
            }
    
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //CREATECHAT#TOKEN#USERNAME


            if (UserListBox.SelectedItem is dynamic)
            {
                             
                _stream.Write("CREATECHAT#" + _token + "#" + (UserListBox.SelectedItem as dynamic).Username + "#");
            
                PopupGrid.Visibility = Visibility.Collapsed;
                GetChats();
            }
            else
            {
                MessageBox.Show("Seleziona un utente prima di confermare.");
                PopupGrid.Visibility = Visibility.Collapsed;
            }
        
        }

        private void RequestUpdate(CancellationToken ct)
        {
            bool DaSostituire;
            Chat richiesta=null;
            Chat nuovachat = null;
            string comandino = "";
            string result="";
            bool rimossa = false;
            bool istherenuovachat=false;

            while (!ct.IsCancellationRequested)
            {
                //SHOULDIUPDATE#TOKEN#IDCHAT#!BONUS VISUALIZZA
                DaSostituire = false;
                rimossa = false;
                istherenuovachat = false;
                mtx.WaitOne();
                foreach (Chat chat in chats)
                {
                    Console.WriteLine("Sto controllando la chat: " + chat.Id);
                    comandino = "SHOULDIUPDATE#" + _token + "#" + chat.Id;

                    if(contextChat!=null)
                    if (chat.Id.Equals(contextChat.Id))
                        comandino  = "SHOULDIUPDATE#" + _token + "#" + chat.Id + "#VISUALIZZA";
                    Console.WriteLine("Adesso scrivo lo shouldiupdate ");
                    string res = _stream.Write(comandino);
                    Console.WriteLine("Mi ha risposto con: " + res);
                    if (res.Equals("yes"))
                    {
                        //fai cose
                        DaSostituire = true;
                        richiesta= chat;
                        nuovachat = GetChat(_token, chat.Id + "", chat.nomecontact, chat.Id_2 + "");

                        byte[] pfp = _stream.Receive("GETPFP#" + _token + "#" + chat.Id_2);

                        if (pfp != null)
                        {

                            try
                            {
                                string pathdir = "contactsImages\\" + chat.Id_2;

                                if (!Directory.Exists(pathdir))
                                    Directory.CreateDirectory(pathdir);

                                pathdir = pathdir + "\\Photo" + _stream.contextExtension;

                                FileStream fileStream = new FileStream(pathdir, FileMode.Create);

                                fileStream.Write(pfp, 0, pfp.Length);
                                fileStream.Close();
                                nuovachat.pathImg = pathdir;
                            }catch (Exception ex) { Console.WriteLine(ex.Message); }
                        }
                        break;


                    }
                    else if (res.Contains("CHAT REMOVED")) {

                        rimossa = true;
                        break;

                    }                     

                    if (ct.IsCancellationRequested)
                        break;
                    Console.WriteLine("Ho finito di controllare la chat: " + chat);

                }
   

                if (DaSostituire && !ct.IsCancellationRequested)
                {

                    chats.Remove(richiesta);
                    chats.Add(nuovachat);
                    nuovachat.Notification = true;

                    Dispatcher.Invoke(() =>
                    {
                        Listachatss.ItemsSource = null;
                        Listachatss.ItemsSource = chats;
                    });

                    if(contextChat!=null)
                    if (contextChat.Id.Equals(nuovachat.Id))
                        UpdateChat(_token,nuovachat.Id+"");
               

                }
                mtx.ReleaseMutex();

                
                if (rimossa)
                {

                    Dispatcher.Invoke(() => {

                        Maingrid.Children.Remove(cv);
                        cv = null;
                    });

                    break;
                }
                //ISTHERENEWCHAT#TOKEN
                result = _stream.Write("ISTHERENEWCHAT#" + _token);

                if (result.Equals("nuovachat"))
                {
                    break;
                }
                Thread.Sleep(200);

            }

         
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            cts2.Cancel();
        }
    }
}
