 
using AudioModul;
using Roboter.AudioVolumeService;
using Roboter.GPSModule;
using Roboter.MusicServiceModul;
using Roboter.RobotArmModul;
using Roboter.ServoModul;
using Roboter.ShutdownModul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoModul;

namespace Roboter
{
 class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Thread.CurrentThread.Name = "Main thread";
                Socket listner = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                listner.Bind(new IPEndPoint(IPAddress.Any, 2000));
                listner.Listen(10);

                byte[] buff = new byte[21];


                while (true)
                {
                    Console.WriteLine("Wait for Connection...");
                    Socket actsocket = listner.Accept();

                    while (true)
                    {
                        try
                        {
                            int count = actsocket.Receive(buff);
                            if ((count == 0) || (!actsocket.Connected))
                            {
                                Console.WriteLine("Client Disconnected...");
                                StopServices();
                                break;
                            }
                            Console.WriteLine("{0}:\t Connected...", actsocket.RemoteEndPoint.ToString());

                            //Check Client Password
                            //read only 21 characters because the password must be 21 characters
                            Console.WriteLine("{0},", Encoding.UTF8.GetString(buff, 0, 21));
                            if (Encoding.UTF8.GetString(buff, 0, 21) != "Roboter safinshaqlawa")
                            {
                                Console.WriteLine("Wrong Password Client disconnected...");
                                actsocket.Shutdown(SocketShutdown.Both);
                                actsocket.Disconnect(false);
                                StopServices();
                                break;
                            }

                            //every thing is OK we will communicate with the client...
                            //send acknoulage back to the client...       
                            actsocket.Send(Encoding.UTF8.GetBytes("Roboter f3gat7sj7gs78"));
                            //now start all Services....
                            string remotIp = actsocket.RemoteEndPoint.ToString().Split(':')[0];
                            string localIp = actsocket.LocalEndPoint.ToString().Split(':')[0];
                            StartServices(localIp, remotIp);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine("Client Disconnected...");
                            Console.WriteLine("{0}", e.Message);
                            StopServices();
                            break;
                        }
                    }
                    Console.WriteLine();
                }
            }
            catch(Exception)
            {
                //when the actual Console is crashing
                //turn robot off
                /*
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo("shutdown", "/p /f");
                p.Start();
                */
                //string path = Directory.GetCurrentDirectory()+@"\Roboter.exe";
                //Process.Start(path);
                Console.WriteLine("Connection lost....");
            }
        }

        static WebCamService webcamservice = null;
        static AudioService audioservice = null;
        static AudioClient audioclient = null;
        static ServoMotorService servomotorservice = null;
        static VolumeService volumeservice = null;
        static MusicService musicservice = null;
        static ShutdownService shutdownservice = null;
        static RoboterArmService roboterarmservice = null;

        static void StartServices(string roboterIp, string clientIp)
        {
            Console.WriteLine("Start all Services....");

            //WebCam Service....
            webcamservice = new WebCamService(roboterIp, 2001);
            ///////////////////////////////////////////
            audioservice = new AudioService(clientIp, 2002);
            ///////////////////////////////////////////
            audioclient = new AudioClient(2003);
            // create VolumeService object to control the audio volume
            volumeservice = new VolumeService(2005, audioclient);
            ///////////////////////////////////////////
            servomotorservice = new ServoMotorService(2004, "COM5");
            ///////////////////////////////////////////
            //create music service object port number=2006
            musicservice = new MusicService(2006);
            // create shutdownservice on port 2007 //
            shutdownservice = new ShutdownService(2007);
            /////////////////////////////////////////
            // create roboterarmservice on port 2008 //
            roboterarmservice = new RoboterArmService(2008);
            /////////////////////////////////////////
            // create GPSService on port 2009 //
            GPSService gpsservice = new GPSService(2009);
            /////////////////////////////////////////
           

            webcamservice.Start();
             Console.WriteLine("WebCamService Started...");
            audioservice.Start_AudioServer();//Error it Blocks the Programm!!!!!!!!!!!
            Console.WriteLine("AudioService Started...");
            audioclient.Start_AudioClient();
            Console.WriteLine("AudioClient Started...");
            //start AudioVolume Service
            volumeservice.Start();
            Console.WriteLine("Audio Volume Service Started...");
            servomotorservice.Start();
            Console.WriteLine("ServoMotorService Started...");
            musicservice.Start();
            Console.WriteLine("Music Service Started...");
            shutdownservice.Start();
            Console.WriteLine("Shutdown Service Started...");
            roboterarmservice.Start();
            Console.WriteLine("RoboterArm Service Started...");
            gpsservice.Start();
            Console.WriteLine("GPS Service Started...");
        }

        static void StopServices()
        {
            webcamservice.Stop();
            audioservice.Stop_AudioServer();
            audioclient.Stop_AudioClient();
            servomotorservice.Close();
            webcamservice = null;
            audioservice = null;
            audioclient = null;
            servomotorservice = null;
        }
    }
}
