using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Client.ServoMotorModul
{
    class MotorClient
    {
         UdpSender udpsender = null;
        MotorWnd motorwnd = null;
        GamePadController gamepadcontroller =null;
        public MotorClient(UdpSender udpsender, MotorWnd motorwnd)
        {
            this.udpsender = udpsender;
            this.motorwnd = motorwnd;
            gamepadcontroller=new GamePadController(udpsender, motorwnd, buff);

            //start servomotorservice register the Control button click events
            motorwnd.btn_left.Click += new RoutedEventHandler(btn_left_Click);
            motorwnd.btn_right.Click += new RoutedEventHandler(btn_right_Click);
            motorwnd.btn_up.Click += new RoutedEventHandler(btn_up_Click);
            motorwnd.btn_down.Click += new RoutedEventHandler(btn_down_Click);
            motorwnd.btn_neutral.Click += new RoutedEventHandler(btn_neutral_Click);
            motorwnd.btn_FwdBwd.Click += new RoutedEventHandler(btn_FwdBwd_Click);
            /////////////////////////////////////////////////////////////////////////////
        }

        public void Start()
        {
            gamepadcontroller.Start();
        }

        void btn_FwdBwd_Click(object sender, RoutedEventArgs e)
        {
            if (motorwnd.btn_FwdBwd.IsChecked == true)
            {
                motorwnd.btn_FwdBwd.Content = "BWD";
                buff[0] = 2;
                buff[2] = 2;
            }
            else
            {
                motorwnd.btn_FwdBwd.Content = "FWD";
                buff[0] = 1;
                buff[2] = 1;
            }
        }

        byte[] buff = new byte[] {1, 0, 1, 0  };

        void ActualizeKoordinate()
        {
            motorwnd.Title = "(" + buff[1] + ", " + buff[3] + ")";
        }

        public void btn_left_Click(object sender, RoutedEventArgs e)
        {
            buff[0] = 1;
            buff[1]++;
            buff[2] = 2;
            buff[3]++;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }

        public void btn_down_Click(object sender, RoutedEventArgs e)
        {
            buff[1]--;
            buff[3]--;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }

        public void btn_up_Click(object sender, RoutedEventArgs e)
        {
            buff[1]++;
            buff[3]++;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }

        public void btn_right_Click(object sender, RoutedEventArgs e)
        {
            buff[0] = 2;
            buff[1]++;
            buff[2] = 1;
            buff[3]++;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }

        public void btn_neutral_Click(object sender, RoutedEventArgs e)
        {
            buff[1] = 0;
            buff[3] = 0;
            udpsender.Send(buff);
            ActualizeKoordinate();
        }
    }
}
