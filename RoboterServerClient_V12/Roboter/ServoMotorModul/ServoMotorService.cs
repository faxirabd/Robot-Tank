using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO.Ports;

namespace Roboter.ServoModul
{
    class ServoMotorService
    {
        Socket clientSock = null;
        EndPoint Remote = null;
        Thread th = null;

        SerialPort serialPort1 = null;
        public ServoMotorService(int port, string serialportname)
        {
            serialPort1 = new SerialPort(serialportname, 9600, Parity.None, 8, StopBits.One);
            th = new Thread(new ThreadStart(Run));

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            Remote = (EndPoint)(ip);

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            clientSock.Bind(ip);

        }

        public void Start()
        {
            serialPort1.Open();
            th.Start();
        }

        byte[] buffReceiver = new byte[4];
        private void Run()
        {
            while (true)
            {
                clientSock.ReceiveFrom(buffReceiver, ref Remote);
                serialPort1.Write(buffReceiver, 0, 4);
            }
        }

        public void Close()
        {
            try
            {
                th.Abort();
                clientSock.Shutdown(SocketShutdown.Both);
                clientSock.Close();
                serialPort1.Close();
            }
            catch (Exception)
            {
                //thread abort throws an exception
            }
        }
    }
}
