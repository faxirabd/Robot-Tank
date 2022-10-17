using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Client.AudioVolumeService
{
    class VolumeClient
    {
        Socket clientSock = null;
        IPEndPoint ip = null;
        int port = 0;
        public VolumeClient(string actip, int port)
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

        public void SetToZero()
        {
            //set Volume to zero, no sound!!!
            buff[0] = 0;
            buff[1] = 0;
            buff[2] = 0;
            buff[3] = 0;

            Send(buff);
        }

        byte[] buff = new byte[4];
        public void slider_volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            int value = Convert.ToInt32(slider.Value);
            buff[0] = (byte)(0x000000FF & value);
            buff[1] = (byte)(0x000000FF & (value >> 8));
            buff[2] = (byte)(0x000000FF & (value >> 16));
            buff[3] = (byte)(0x000000FF & (value >> 24));

            Send(buff);
        }

        public void Close()
        {
            clientSock.Disconnect(false);
            clientSock.Close();
        }
    }
}
