using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;

namespace WHATSAPP_GUI
{
    public class NetworkStreamMutexed
    {
        NetworkStream _net;
        byte[] _data;
        Mutex mtx;
        string _ext;
        public string contextExtension {  get { return _ext; } }

        public NetworkStreamMutexed(NetworkStream net) {

            _net = net;
            mtx=new Mutex();

        }

        public string Write(string comand)
        {
            mtx.WaitOne();
            _data = Encoding.ASCII.GetBytes(comand);
            _net.Write(_data, 0, _data.Length);
            _data = new byte[64000];
            _net.Read(_data, 0, _data.Length);
            mtx.ReleaseMutex();
            return Encoding.ASCII.GetString(_data).Trim('\0');

        }

       

        public string SendMessage(string command, byte[] img)
        {
            mtx.WaitOne();
            _data = Encoding.ASCII.GetBytes(command);
            _net.Write(_data, 0, _data.Length);
            _data = new byte[2500];
            _net.Read(_data, 0, _data.Length);
            _net.Write(img, 0, img.Length);
            _net.Read(_data, 0, _data.Length);
            mtx.ReleaseMutex();
            return Encoding.ASCII.GetString(_data).Trim('\0');

        }



        public byte[] Receive(string command) { 
        
            mtx.WaitOne();
            _data = Encoding.ASCII.GetBytes(command);
            _net.Write(_data, 0, _data.Length);
            byte[] response = new byte[20];
            _net.Read(response, 0, response.Length);
            string tmp = Encoding.ASCII.GetString(response);
            tmp = tmp.Trim('\0');
            if (tmp.Split('#').Count() == 2)
            {
                _ext = tmp.Split('#')[1];
                tmp= tmp.Split('#')[0];


            }
      
            if (tmp.Contains("Pfp non trovata"))
            {
                mtx.ReleaseMutex();
                return null;
            }
            int dim = int.Parse(tmp);
            byte[] file = new byte[dim];
            _data = Encoding.ASCII.GetBytes("OK MANDA");
            _net.Write(_data, 0, _data.Length);
            _net.Read(file, 0, file.Length);
            mtx.ReleaseMutex();

       
            return file;


        }

    }
}
