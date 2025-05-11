using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tester
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient(Dns.GetHostName(), 16543);
            NetworkStream canale = client.GetStream();
            byte[] cmd = Encoding.ASCII.GetBytes("LOG#ciccio#1234#");
            canale.Write(cmd, 0, cmd.Length);
            cmd = new byte[100];
            canale.Read(cmd, 0, cmd.Length);
            string token = Encoding.ASCII.GetString(cmd).TrimEnd('\0');
   
            Console.WriteLine(token);
            //CREATECHAT#TOKEN#USERNAME
            cmd = Encoding.ASCII.GetBytes("CREATECHAT#" + token + "#cicciodue#");
            canale.Write(cmd, 0, cmd.Length);
            cmd = new byte[100];
            canale.Read(cmd, 0, cmd.Length);
            Console.WriteLine(Encoding.ASCII.GetString(cmd));

            //GETCHATS#TOKEN#       
            cmd = Encoding.ASCII.GetBytes("GETCHATS#" + token + "#");
            canale.Write(cmd, 0, cmd.Length);
            cmd = new byte[100];
            canale.Read(cmd, 0, cmd.Length);
            string[]chats=Encoding.ASCII.GetString(cmd).Split('#');
            chats.ToList().ForEach(x => Console.WriteLine(x));

            //GETCHAT#TOKEN#IDCHAT
            cmd = Encoding.ASCII.GetBytes("GETCHAT#" + token + "#" + chats[0].Split('-')[0] + "#");
            canale.Write(cmd, 0, cmd.Length);
            cmd = new byte[100];
            canale.Read(cmd, 0, cmd.Length); 
            
            string[] messaggi = Encoding.ASCII.GetString(cmd).Split('#');
            messaggi.ToList().ForEach(x => Console.WriteLine(x));
            

            //ADDMESSAGE#TOKEN#IDCHAT#TESTO#PATHIMAGINE#NUMBYTE#DATAORA

            FileStream fs = new FileStream("Immagine.png", FileMode.Open);
            byte[] data = new byte[(int)fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();
            byte[] cmdtotale = new byte[20000000];
            cmdtotale = Encoding.ASCII.GetBytes("ADDMESSAGE#" + token + "#" + chats[0].Split('-')[0] + "#Messaggio Cancro#Immagine.png#"+data.Length+"#" + DateTime.Now.ToLongDateString() + "#");
            canale.Write(cmdtotale, 0, cmdtotale.Length);
            cmdtotale = new byte[2500];
            canale.Read(cmdtotale, 0, cmdtotale.Length);
          

            Console.WriteLine(Encoding.ASCII.GetString(cmdtotale));
            canale.Write(data, 0, data.Length);
            canale.Read(cmdtotale, 0, cmdtotale.Length);
            canale.Read(cmdtotale, 0, cmdtotale.Length);


            //GETFILE#TOKEN#IDMESSAGE#IDCHAT
            cmdtotale = Encoding.ASCII.GetBytes("GETFILE#" + token + "#" + 1 + "#" + 0 + "#");

            canale.Write(cmdtotale, 0, cmdtotale.Length);

            byte[] response = new byte[20];
            canale.Read(response, 0, response.Length);
            string tmp = Encoding.ASCII.GetString(response);
            int dim = int.Parse(tmp);

            byte[] file = new byte[dim];

            cmdtotale = Encoding.ASCII.GetBytes("OK MANDA");
            canale.Write(cmdtotale, 0, cmdtotale.Length);

            canale.Read(file, 0, file.Length);


            if (!Directory.Exists("download"))
            {
                Directory.CreateDirectory("download");
            }

            FileStream fileStream = new FileStream("download\\Immagine.png", FileMode.Create);
            fileStream.Write(file, 0, file.Length);

            //DELETEMESSAGE#TOKEN#IDMESSAGE#IDCHAT

            cmd = Encoding.ASCII.GetBytes("DELETEMESSAGE#" + token + "#1#0#");
            canale.Write(cmd, 0, cmd.Length);
            cmd = new byte[100];
            canale.Read(cmd, 0, cmd.Length);
            Console.WriteLine(Encoding.ASCII.GetString(cmd));
    

          

            Console.ReadKey();
        }
    }
}
