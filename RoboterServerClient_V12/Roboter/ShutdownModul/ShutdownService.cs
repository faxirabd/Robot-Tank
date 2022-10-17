using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace Roboter.ShutdownModul
{
    class ShutdownService
    {
        Socket clientSock = null;
        EndPoint Remote = null;
        Thread th = null;

        public ShutdownService(int port)
        {
            th = new Thread(new ThreadStart(Run));

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            Remote = (EndPoint)(ip);

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            clientSock.Bind(ip);
        }

        public void Start()
        {
            th.Start();
        }

        byte[] buffReceiver = new byte[1];
        private void Run()
        {
            while (true)
            {
                clientSock.ReceiveFrom(buffReceiver, ref Remote);
                Console.WriteLine("Shut Down={0}", buffReceiver[0]);
                //turn robot off
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo("shutdown", "/p /f");
                p.Start();
            }
        }

        public void Close()
        {
            try
            {
                th.Abort();
                clientSock.Shutdown(SocketShutdown.Both);
                clientSock.Close();
            }
            catch (Exception)
            {
                //thread abort throws an exception
            }
        }
    }
}
