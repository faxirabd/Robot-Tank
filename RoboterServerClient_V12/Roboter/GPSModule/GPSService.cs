using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roboter.GPSModule
{
    class GPSService
    {
        Socket clientSock = null;
        EndPoint Remote = null;
        Thread th = null;
        //open serial port
        SerialPort serialPort1 = new SerialPort("COM9", 4800, Parity.None, 8, StopBits.One);

        public GPSService(int port)
        {
            th = new Thread(new ThreadStart(Run));

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            Remote = (EndPoint)(ip);

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            clientSock.Bind(ip);
            Console.WriteLine("Bind....");

        }

        public void Start()
        {
            th.Start();
        }

        byte[] buffReceiver = new byte[6];  //index=0 set position/ set speed
 
        private void Run()
        {
            //Console.WriteLine("wait for client request");
            clientSock.ReceiveFrom(buffReceiver, ref Remote);
            //Console.WriteLine(buffReceiver[0] + ", " + buffReceiver[1] + ", "
            //       + buffReceiver[2]);
            //Console.WriteLine("data received from client");

            //open serial port after the client request it
            serialPort1.Open();
            //Console.WriteLine("GPS Sensor serial port opened...");

            //separate different received data
            string[] data = null;



            while (true)
            {
                //filter only the line with start marker "$GPGGA"
                string msg = serialPort1.ReadLine();


                if (msg.IndexOf("$GPGGA") != -1) //GPRMC
                {
                    try
                    {
                        string msgTosend = null;
                        data = msg.Split(',');

                        //"satellites Nr
                        msgTosend = "S, " + data[7] + ",";
                        //value, N
                        msgTosend += data[2] + "," + data[3] + ",";
                        //value, E
                        msgTosend += data[4] + "," + data[5] + ",";

                        buffReceiver = GetBytes(msgTosend);

                        //  Console.WriteLine("Send GPS 1");
                        //Console.WriteLine(GetString(buffReceiver));
                        clientSock.SendTo(buffReceiver, buffReceiver.Length, SocketFlags.None, Remote);
                    }
                    catch
                    {
                        //Cannot Read GPS values
                        Console.WriteLine("GPS Unavailable");
                    }
                }

                if (msg.IndexOf("$GPRMC") != -1) //GPRMC
                {
                    try
                    {
                        string msgTosend = null;
                        //separate different received data
                        string[] data2 = msg.Split(',');

                        //store actual position
                        //Console.WriteLine("Act. Heading: " + data2[8] + "°");
                        msgTosend = "H, " + data2[8];

                        //   Console.WriteLine("Send GPS 2");
                        buffReceiver = GetBytes(msgTosend);
                        // Console.WriteLine(GetString(buffReceiver));
                        clientSock.SendTo(buffReceiver, buffReceiver.Length, SocketFlags.None, Remote);
                    }
                    catch
                    {
                        //Cannot Read GPS values
                        Console.WriteLine("GPS Unavailable");
                    }

                }
            }

        }

        //convert string to byte array
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        //convert byte array to string
        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
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
