using Client.ServoMotorModul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.RoboterArmModul
{
    class RobotArmClient
    {
        Socket clientSock = null;
        IPEndPoint ip = null;
        int port = 0;
        RobotArmWnd armwindow = null;

        public RobotArmClient(string actip, int port, RobotArmWnd armwindow)
        {
            ip = new IPEndPoint(IPAddress.Parse(actip), port);
            this.port = port;

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

             this.armwindow = armwindow;

            /** fill the servo speed comboBox with speed values **/
            armwindow.cmbx_speed.Items.Add("Max");
            for (int i = 1; i < 100; i++)
                armwindow.cmbx_speed.Items.Add(i);
            armwindow.cmbx_speed.SelectedIndex = 0;
            /*****************************************************/

            /** register events to set servo speed **/
            armwindow.btn_setspeed.Click += new RoutedEventHandler(btn_setspeed_Click);
            /***********************************/

            //register events needed to move servos ...
            armwindow.btn_left0.Click += new RoutedEventHandler(btn_left_Click0);
            armwindow.btn_right0.Click += new RoutedEventHandler(btn_right_Click0);

            armwindow.btn_left1.Click += new RoutedEventHandler(btn_left_Click1);
            armwindow.btn_right1.Click += new RoutedEventHandler(btn_right_Click1);

            armwindow.btn_left2.Click += new RoutedEventHandler(btn_left_Click2);
            armwindow.btn_right2.Click += new RoutedEventHandler(btn_right_Click2);

            armwindow.btn_left3.Click += new RoutedEventHandler(btn_left_Click3);
            armwindow.btn_right3.Click += new RoutedEventHandler(btn_right_Click3);

            armwindow.btn_left4.Click += new RoutedEventHandler(btn_left_Click4);
            armwindow.btn_right4.Click += new RoutedEventHandler(btn_right_Click4);

            armwindow.btn_left5.Click += new RoutedEventHandler(btn_left_Click5);
            armwindow.btn_right5.Click += new RoutedEventHandler(btn_right_Click5);
            /***********************************************************************/

            /** register event for robot park Button  **/
            armwindow.btn_parkrobotarm.Click += new RoutedEventHandler(btn_parkrobotarm_Click);
            /*******************************************/
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

        //index 0 = set position/ set speed
        //index 1 = servo number
        //index 2 = new position/ new speed
        byte[] buff = new byte[3];

        public void Close()
        {
            clientSock.Disconnect(false);
            clientSock.Close();
        }
           

        void btn_parkrobotarm_Click(object sender, RoutedEventArgs e)
        {
            /** move Servo 0 to park position **/
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 0;

            servoposition[0] = 50;
            buff[2] = servoposition[0];

            Send(buff);
            armwindow.lbl_actualPosition0.Content = servoposition[0].ToString();
            /*******************************************************************/

            /** move Servo 1 to park position **/
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 1;

            servoposition[1] = 100;
            buff[2] = servoposition[1];

            Send(buff);
            armwindow.lbl_actualPosition1.Content = servoposition[1].ToString();
            /*******************************************************************/

            /** move Servo 2 to park position **/
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 2;

            servoposition[2] = 0;
            buff[2] = servoposition[2];

            Send(buff);
            armwindow.lbl_actualPosition2.Content = servoposition[2].ToString();
            /*******************************************************************/

            /** move Servo 3 to park position **/
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 3;//servo number 3

            servoposition[3] = 100;
            buff[2] = servoposition[3];

            Send(buff);
            armwindow.lbl_actualPosition3.Content = servoposition[3].ToString();
            /*******************************************************************/
        }

        void btn_setspeed_Click(object sender, RoutedEventArgs e)
        {
            buff[0] = 4;//to tell the robot that we want to change servo speed

            buff[1] = (byte)armwindow.cmbx_servos.SelectedIndex;//set the servo number we want to change speed
            buff[2] = (byte)armwindow.cmbx_speed.SelectedIndex;
            Send(buff);//and now send the command to robot to change servo speed
        }

        //actual positions of each servo
        byte[] servoposition = new byte[] { 50, 50, 50, 50, 50, 50};

        //Servo number 0
        //moving servo to right site
        private void btn_right_Click0(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 0;
            if (servoposition[0] < 100)
            {
                servoposition[0]++;
                buff[2] = servoposition[0];
            }
            Send(buff);
            armwindow.lbl_actualPosition0.Content = servoposition[0].ToString();
        }
        //moving servo to left site
        private void btn_left_Click0(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 0;
            if (servoposition[0] > 0)
            {
                servoposition[0]--;
                buff[2] = servoposition[0];
            }
            Send(buff);
            armwindow.lbl_actualPosition0.Content = servoposition[0].ToString();
        }

        //Servo number 1
        //moving servo to right site
        private void btn_right_Click1(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 1;
            if (servoposition[1] < 100)
            {
                servoposition[1]++;
                buff[2] = servoposition[1];
            }
            Send(buff);
            armwindow.lbl_actualPosition1.Content = servoposition[1].ToString();
        }
        //moving servo to left site
        private void btn_left_Click1(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 1;
            if (servoposition[1] > 0)
            {
                servoposition[1]--;
                buff[2] = servoposition[1];
            }
            Send(buff);
            armwindow.lbl_actualPosition1.Content = servoposition[1].ToString();
        }

        //Servo number 2
        //moving servo to right site
        private void btn_right_Click2(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 2;
            if (servoposition[2] < 100)
            {
                servoposition[2]++;
                buff[2] = servoposition[2];
            }
            Send(buff);
            armwindow.lbl_actualPosition2.Content = servoposition[2].ToString();
        }
        //moving servo to left site
        private void btn_left_Click2(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 2;
            if (servoposition[2] > 0)
            {
                servoposition[2]--;
                buff[2] = servoposition[2];
            }
            Send(buff);
            armwindow.lbl_actualPosition2.Content = servoposition[2].ToString();
        }

        //Servo number 3
        //moving servo to right site
        private void btn_right_Click3(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 3;
            if (servoposition[3] < 100)
            {
                servoposition[3]++;
                buff[2] = servoposition[3];
            }
            Send(buff);
            armwindow.lbl_actualPosition3.Content = servoposition[3].ToString();
        }
        //moving servo to left site
        private void btn_left_Click3(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 3;
            if (servoposition[3] > 0)
            {
                servoposition[3]--;
                buff[2] = servoposition[3];
            }
            Send(buff);
            armwindow.lbl_actualPosition3.Content = servoposition[3].ToString();
        }

        //Servo number 4
        //moving servo to right site
        private void btn_right_Click4(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 4;
            if (servoposition[4] < 100)
            {
                servoposition[4]++;
                buff[2] = servoposition[4];
            }
            Send(buff);
            armwindow.lbl_actualPosition4.Content = servoposition[4].ToString();
        }
        //moving servo to left site
        private void btn_left_Click4(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 4;
            if (servoposition[4] > 0)
            {
                servoposition[4]--;
                buff[2] = servoposition[4];
            }
            Send(buff);
            armwindow.lbl_actualPosition4.Content = servoposition[4].ToString();
        }

        //Servo number 5
        //moving servo to right site
        private void btn_right_Click5(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 5;
            if (servoposition[5] < 100)
            {
                servoposition[5]++;
                buff[2] = servoposition[5];
            }
            Send(buff);
            armwindow.lbl_actualPosition5.Content = servoposition[5].ToString();
        }
        //moving servo to left site
        private void btn_left_Click5(object sender, RoutedEventArgs e)
        {
            buff[0] = 0;//to tell the robot that we want to move servo
            buff[1] = 5;
            if (servoposition[5] > 0)
            {
                servoposition[5]--;
                buff[2] = servoposition[5];
            }
            Send(buff);
            armwindow.lbl_actualPosition5.Content = servoposition[5].ToString();
        }
    }
}
