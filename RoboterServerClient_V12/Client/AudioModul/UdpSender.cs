using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
//using System.Windows;

namespace AudioModul
{
    class UdpSender
    {
        Socket clientSock = null;
        string actip = "127.0.0.1";
        int port = 2001;
        public UdpSender(string actip, int port)
        {
            this.actip = actip;
            this.port = port;

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Send(byte[] buff)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse(actip), port);
                clientSock.SendTo(buff, buff.Length, SocketFlags.None, ip);
            }
            catch (SocketException ex)
            {
                //client connection closed...
                Console.WriteLine("End...:" + ex.Message);
            }

        }

        public void Close()
        {
            clientSock.Disconnect(false);
            clientSock.Close();
        }
    }
}
