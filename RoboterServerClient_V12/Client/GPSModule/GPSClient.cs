using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.GPSModule
{
    class GPSClient
    {
        Socket clientSock = null;
        IPEndPoint ip = null;
        int port = 0;
        Thread th = null;
        GPSWindow wnd = null;

        public GPSClient(string actip, int port, GPSWindow wnd)
        {
            ip = new IPEndPoint(IPAddress.Parse(actip), port);
            this.port = port;
            this.wnd = wnd;

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            th = new Thread(new ThreadStart(Run));
            th.IsBackground = true; //to terminate this thread as soon the main thread is terminated
        }

        public void Start()
        {
            th.Start();
        }

        public void Send(byte[] buff)
        {
            try
            {
                clientSock.SendTo(buff, buff.Length, SocketFlags.None, ip);
            }
            catch (SocketException)
            {
                //client connection closed...
                //Console.WriteLine("End...:" + ex.Message);
            }
        }

        public byte[] buff = new byte[150];

        public int Receive()
        {
            int i = 0;
            try
            {
                i = clientSock.Receive(buff);
            }
            catch (SocketException)
            {
                //client connection closed...
                //Console.WriteLine("End...:" + ex.Message);
            }
            return i;
        }

        public string Ncoordinate = null;
        public string Ecoordinate = null;
        public string satellitesNr = null;
        private void Run()
        {
            while (true)
            {
                int j = this.Receive();
                string msg =GetString(this.buff, j);
                string[] data = msg.Split(',');
                //because only the main thread can access UI
                wnd.Dispatcher.Invoke((Action)(() =>
                {
                    if (data[0] == "S") //S data
                    {
                        wnd.lbl_sateleteNr.Content = "satellites Nr:" + data[1];
                        wnd.lbl_N.Content = data[2] + "," + data[3];
                        wnd.lbl_E.Content = data[4] + "," + data[5];
                        Ncoordinate = data[2];
                        Ecoordinate = data[4];
                        satellitesNr = data[1];
                    }
                    else if (data[0] == "H") //H data
                    {
                        wnd.lbl_actualHeading.Content = "Act. Heading: " + data[1] + "°";
                    }
                }));
            }
        }

        //convert string to byte array
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        //convert byte array to string
        public static string GetString(byte[] bytes, int i)
        {
            char[] chars = new char[i / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, i);
            return new string(chars);
        }

        public void Close()
        {
            th.Abort();
            clientSock.Disconnect(false);
            clientSock.Close();
        }
    }
}
