using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Client.ServoMotorModul
{
    class UdpSender
    {
        Socket clientSock = null;
        IPEndPoint ip = null;
        int port = 2004;//default port number for servo motor control
        public UdpSender(string actip, int port)
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

        public void Close()
        {
            clientSock.Disconnect(false);
            clientSock.Close();
        }
    }
}
