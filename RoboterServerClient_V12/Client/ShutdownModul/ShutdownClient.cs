using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows;

namespace Client.ShutdownModul
{
    class ShutdownClient
    {
        Socket clientSock = null;
        IPEndPoint ip = null;
        int port = 0;
        public ShutdownClient(string actip, int port)
        {
            ip = new IPEndPoint(IPAddress.Parse(actip), port);
            this.port = port;

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
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

        byte[] buff = new byte[1];
        public void btn_shutdown_Click(object sender, RoutedEventArgs e)
        {
            //turn robot off
            buff[0] = 255;
            Send(buff);
        }

        public void Close()
        {
            clientSock.Disconnect(false);
            clientSock.Close();
        }
    }
}
