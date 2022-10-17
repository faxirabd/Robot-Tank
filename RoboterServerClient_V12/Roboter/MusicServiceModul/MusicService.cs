using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using AudioModul;
using System.Windows.Media;
using System.IO;
using System.Speech.Synthesis;

namespace Roboter.MusicServiceModul
{
    class MusicService
    {
        Socket clientSock = null;
        EndPoint Remote = null;
        Thread th = null;

        public MusicService(int port)
        {
            th = new Thread(new ThreadStart(Run));

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);

            Remote = (EndPoint)(ip);

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            clientSock.Bind(ip);
        }

        public void Start()
        {
            th.Start();
        }

        byte[] buffReceiver = new byte[5];
        MediaPlayer mediaplayer = null;
        private void Run()
        {
            mediaplayer = new MediaPlayer();
            while (true)
            {
                clientSock.ReceiveFrom(buffReceiver, ref Remote);
                //buff[0] = 0 -> music change stop and start
                if (buffReceiver[0] == 0)
                {
                    byte musicNumber = buffReceiver[1];
                    byte musicOnOff = buffReceiver[2];
                    Console.WriteLine("musicNumber={0}\tmusicOnOff={1}"
                        , musicNumber, musicOnOff);


                    //turn music off


                    if (musicOnOff == 255)
                    {
                        switch (musicNumber)
                        {
                            case 0:
                                mediaplayer.Open(new Uri(Directory.GetCurrentDirectory() + @"\Music\MrSaxoBeat.mp3"));
                                break;
                            case 1:
                                mediaplayer.Open(new Uri(Directory.GetCurrentDirectory() + @"\Music\Shakira.mp3"));
                                break;
                        }
                        //turn music on
                        mediaplayer.Play();
                    }
                    else if (musicOnOff == 0)
                    {
                        //turn music off
                        mediaplayer.Stop();
                    }
                    else if (musicOnOff == 100)
                    {
                        SpeechSynthesizer sp =
                            new System.Speech.Synthesis.SpeechSynthesizer();

                        //sp.Volume = 100;
                        sp.Rate = -4;
                        sp.SpeakAsync("man kurdy zor bash nazanm. balam hawldadm tozak qsa ba kurdy bkem. har bardawam biit Nevendi kwltwri kurdiy. Hello I'm robot tank made by Mr. Mohammad Fakhir. I can drive forward turn backwards turning left and turning right. you can remote control me via W-Lan. so now i'll demonstrate some movements.");
                    }
                }
                //buff[0] = 255 -> Volume change
                else if (buffReceiver[0] == 255)
                {
                    int volume = (buffReceiver[4] << 24) | (buffReceiver[3] << 16)
                   | (buffReceiver[2] << 8) | (buffReceiver[1]);

                    mediaplayer.Volume = ((double)volume)/100;
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
