using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO.Ports;
using AudioModul;

namespace Roboter.AudioVolumeService
{
    class VolumeService
    {
        Socket clientSock = null;
        EndPoint Remote = null;
        Thread th = null;
        AudioClient audioclient = null;

        public VolumeService(int port, AudioClient audioclient)
        {
            this.audioclient = audioclient;

            th = new Thread(new ThreadStart(Run));

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            Remote = (EndPoint)(ip);

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            clientSock.Bind(ip);
        }

        public void Start()
        {
            audioclient.SetVolume(0);
            th.Start();
        }

        byte[] buffReceiver = new byte[4];
        private void Run()
        {
            while (true)
            {
                clientSock.ReceiveFrom(buffReceiver, ref Remote);
                int volume = (buffReceiver[3] << 24) | (buffReceiver[2] << 16)
                    | (buffReceiver[1] << 8) | (buffReceiver[0]);
                audioclient.SetVolume(volume);
                //just to test whether the correct number is received!
                //Console.WriteLine("Volume:{0}", volume);
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
