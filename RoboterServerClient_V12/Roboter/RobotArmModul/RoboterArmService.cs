using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roboter.RobotArmModul
{
    class RoboterArmService
    {
        Socket clientSock = null;
        EndPoint Remote = null;
        Thread th = null;

        ServoCtrlUSBDevice device = null;
        ushort[,] servoMinMax = new ushort[4, 2]//index = servo number
        {
            {768, 2256},//servo 0
            {768, 2390},//servo 1
            {800, 2256},//servo 2
            {1540, 2256},//servo 3
        };

        public RoboterArmService(int port)
        {
            th = new Thread(new ThreadStart(Run));

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            Remote = (EndPoint)(ip);

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            clientSock.Bind(ip);

        }

        public void Start()
        {
            device = ServoCtrlUSBDevice.connectToDevice();
            th.Start();
        }

        byte[] buffReceiver = new byte[3];  //index=0 set position/ set speed
                                            //index=1 servo number
                                            //index=2 new position/ new speed
        private void Run()
        {
            while (true)
            {
                clientSock.ReceiveFrom(buffReceiver, ref Remote);
                Console.WriteLine(buffReceiver[0] + ", " + buffReceiver[1] + ", "
                       + buffReceiver[2]);

                if (buffReceiver[0] == 0)//move servo to new position
                    if(buffReceiver[1] < 4) //because we use for now only 4 servos
                {
                    ushort servoPosition = (ushort)(servoMinMax[buffReceiver[1], 0] + buffReceiver[2] *
                        ((servoMinMax[buffReceiver[1], 1] - servoMinMax[buffReceiver[1], 0]) / 100.0));
                    Console.WriteLine(servoPosition);

                    device.setTarget(buffReceiver[1], (ushort)(servoPosition * (ushort)4));
                }
                else if (buffReceiver[0] == 1)//set speed of servo to new value
                {
                    device.setSpeed(buffReceiver[1], buffReceiver[2]);
                }
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
