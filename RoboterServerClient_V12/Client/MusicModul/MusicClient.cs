using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Controls;
using System.Windows;

namespace Client.MusicModul
{
    class MusicClient
    {
        Socket clientSock = null;
        IPEndPoint ip = null;
        int port = 0;
        public MusicClient(string actip, int port)
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

        //buff[0] = 0 -> music change stop and start
        //buff[0] = 255 -> Volume change
        byte[] buff = new byte[5];

        public void Close()
        {
            clientSock.Disconnect(false);
            clientSock.Close();
        }

        public void cmbx_music_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //select new music
            buff[1] = (byte)(((ComboBox)sender).SelectedIndex);
        }

        public void btn_start_Click(object sender, RoutedEventArgs e)
        {
            //turn music on
            buff[0] = 0;// -> music change stop and start
            buff[2] = 255;
            Send(buff);
        }

        public void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            //turn music off
            buff[0] = 0;// -> music change stop and start
            buff[2] = 0;
            Send(buff);
        }

        public void btn_startdemonstration_Click(object sender, RoutedEventArgs e)
        {
            //start demonstartion
            buff[0] = 0;// -> music change stop and start
            buff[2] = 100;
            Send(buff);
        }

        public void slider_volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            buff[0] = 255;// -> Volume change
            Slider slider = (Slider)sender;
            int value = Convert.ToInt32(slider.Value);
            buff[1] = (byte)(0x000000FF & value);
            buff[2] = (byte)(0x000000FF & (value >> 8));
            buff[3] = (byte)(0x000000FF & (value >> 16));
            buff[4] = (byte)(0x000000FF & (value >> 24));

            Send(buff);
        }
    }
}
