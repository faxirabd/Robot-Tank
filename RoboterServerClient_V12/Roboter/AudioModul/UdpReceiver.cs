using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace AudioModul
{
    class UdpReceiver
    {
        Socket clientSock = null;
        EndPoint Remote = null;

        public UdpReceiver(int port)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            Remote = (EndPoint)(ip);

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            clientSock.Bind(ip);
        }

        public byte[] Receive()
        {
            int receivedDataLength;
            byte[] buffReceiver = new byte[16384];

            receivedDataLength = clientSock.ReceiveFrom(buffReceiver, ref Remote);

            return buffReceiver;
        }

        public void Close()
        {
            clientSock.Shutdown(SocketShutdown.Both);
            clientSock.Close();
        }
    }
}
