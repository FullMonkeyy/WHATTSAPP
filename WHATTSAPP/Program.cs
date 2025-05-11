using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WHATTSAPP_server
{
    class Program
    {
        static void Main(string[] args)
        {
            //List< Chat> chats =GestioneFile.ReadChats();
            //chats.ForEach((x) =>
            //{
            //    x.isAcceted1 = true;
            //    x.isAcceted2 = true;

            //});
            //GestioneFile.ScriviChat(chats);


            TcpListener listener = new TcpListener(IPAddress.Any, 7225);           
            listener.Start();
            while (true)
            {

                TcpClient tcp = listener.AcceptTcpClient();
                BuisnessLogic bl = new BuisnessLogic(tcp);




            }



        }
        
    
    }
}
