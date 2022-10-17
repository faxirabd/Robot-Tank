using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows;

namespace Client.ServoMotorModul
{
    class ServoClient
    {
        UdpSender udpsender = null;
        MainWindow mainwindow = null;
        public ServoClient(UdpSender udpsender, MainWindow mainwindow)
        {
            this.udpsender = udpsender;
            this.mainwindow = mainwindow;
        }

        byte[] buff = new byte[] {0, 123, 0, 123 };

        void ActualizeKoordinate()
        {
            mainwindow.Title = "(" + buff[1] + ", " + buff[3] + ")";
        }

        public void btn_left_Click(object sender, RoutedEventArgs e)
        {
            buff[1]--;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }

        public void btn_down_Click(object sender, RoutedEventArgs e)
        {
            buff[3]--;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }

        public void btn_up_Click(object sender, RoutedEventArgs e)
        {
            buff[3]++;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }

        public void btn_right_Click(object sender, RoutedEventArgs e)
        {
            buff[1]++;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }

        public void btn_neutral_Click(object sender, RoutedEventArgs e)
        {
            buff[1] = 123;
            buff[3] = 123;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }
    }
}
