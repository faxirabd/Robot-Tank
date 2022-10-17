/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Net;
using VideoModul;

namespace VideoModul
{
    public class WebCamService
    {


        #region Member Variables
        private const int MAXOUTSTANDINGPACKETS = 3;

        /// <summary>
        /// The thread will run the job.
        /// The job is the Method Run() below
        /// </summary>
        protected Thread thread = null;
        private ManualResetEvent ConnectionReady;
        private volatile bool bShutDown;
        private volatile int iConnectionCount;

        int port = 2000;
        string actip = "127.0.0.1";
        #endregion

        public WebCamService(string actip, int port)
        {
            this.actip = actip;
            this.port = port;
        }
        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        public void Start()
        {
            if (thread == null)
            {
                ThreadStart starter = new ThreadStart(Run);
                thread = new Thread(starter);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        /// <summary>
        /// Stop this service.
        /// The Run() Method tests for this thread state each second
        /// </summary>
        public void Stop()
        {
            // Set exit condition
            bShutDown = true;

            // Need to get out of wait
            if(ConnectionReady != null)
            ConnectionReady.Set();
        }
        public void Run()
        {
            const int VIDEODEVICE = 0; // zero based index of video capture device to use
            const int FRAMERATE = 15;  // Depends on video device caps.  Generally 4-30.
            const int VIDEOWIDTH = 320; // Depends on video device caps
            const int VIDEOHEIGHT = 240; // Depends on video device caps
            const long JPEGQUALITY = 30; // 1-100 or 0 for default


            Capture cam = null;
            TcpServer serv = null;
            ImageCodecInfo myImageCodecInfo;
            EncoderParameters myEncoderParameters;

            try
            {
                // Set up member vars
                ConnectionReady = new ManualResetEvent(false);
                bShutDown = false;

                // Set up tcp server
                iConnectionCount = 0;
                serv = new TcpServer(port, IPAddress.Parse(actip));//TcpServer.GetAddresses()[0]);
                serv.Connected += new TcpConnected(Connected);
                serv.Disconnected += new TcpConnected(Disconnected);
                serv.DataReceived += new TcpReceive(Receive);
                serv.Send += new TcpSend(Send);

                myEncoderParameters = null;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");

                if (JPEGQUALITY != 0)
                {
                    // If not using the default jpeg quality setting
                    EncoderParameter myEncoderParameter;
                    myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, JPEGQUALITY);
                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                }

                cam = new Capture(VIDEODEVICE, FRAMERATE, VIDEOWIDTH, VIDEOHEIGHT);
                // Initialization succeeded.  Now, start serving up frames
                DoIt(cam, serv, myImageCodecInfo, myEncoderParameters); 
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine(String.Format("{0}: Failed on startup {1}", DateTime.Now.ToString(), ex));
                }
                catch { }
            }
            finally
            {
                // Cleanup
                if (serv != null)
                {
                    serv.Dispose();
                }

                if (cam != null)
                {
                    cam.Dispose();
                }
            }
        }

        // Start serving up frames
        private void DoIt(Capture cam, TcpServer serv, ImageCodecInfo myImageCodecInfo, EncoderParameters myEncoderParameters)
        {

            MemoryStream m = new MemoryStream(20000);
            Bitmap image = null;
            IntPtr ip = IntPtr.Zero;
            do
            {
                    // Wait til a client connects before we start the graph
                    ConnectionReady.WaitOne();
                cam.Start();

                // While not shutting down, and still at least one client
                while ((!bShutDown) && (serv.Connections > 0))
                {
                    try
                    {

                        // capture image
                        ip = cam.GetBitMap();
                        image = new Bitmap(cam.Width, cam.Height, cam.Stride, PixelFormat.Format24bppRgb, ip);
                        image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                        // save it to jpeg using quality options
                        m.Position = 10;
                        image.Save(m, myImageCodecInfo, myEncoderParameters);

                        // Send the length as a fixed length string
                        m.Position = 0;
                        m.Write(Encoding.ASCII.GetBytes((m.Length - 10).ToString("d8") + "\r\n"), 0, 10);

                        // send the jpeg image
                        serv.SendToAll(m);

                        // Empty the stream
                        m.SetLength(0);

                        // remove the image from memory
                        image.Dispose();
                        image = null;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Console.WriteLine(DateTime.Now.ToString());
                            Console.WriteLine(ex);
                        }
                        catch { }
                    }
                    finally
                    {
                        if (ip != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(ip);
                            ip = IntPtr.Zero;
                        }
                    }
                }

                // Clients have all disconnected.  Pause, then sleep and wait for more
                cam.Pause();
                Console.WriteLine("Dropped frames: " + cam.m_Dropped.ToString());

            } while (!bShutDown);
        }

        class PacketCount : IDisposable
        {
            private int m_PacketCount;
            private int m_MaxPackets;

            public PacketCount(int i)
            {
                m_MaxPackets = i;
                m_PacketCount = 0;
            }

            public bool AddPacket()
            {
                bool b;

                lock (this)
                {
                    b = m_PacketCount < m_MaxPackets;
                    if (b)
                    {
                        m_PacketCount++;
                    }
                    else
                    {
                        Debug.WriteLine("Max outstanding Packets reached");
                    }
                }

                return b;
            }

            public void RemovePacket()
            {
                lock (this)
                {
                    if (m_PacketCount > 0)
                    {
                        m_PacketCount--;
                    }
                    else
                    {
                        Debug.WriteLine("Packet count is messed up");
                    }
                }
            }

            public int Count()
            {
                return m_PacketCount;
            }

            #region IDisposable Members

            public void Dispose()
            {
#if DEBUG
                if (m_PacketCount != 0)
                {
                    Debug.WriteLine("Packets left over: " + m_PacketCount.ToString());
                }
#endif
            }

            #endregion
        }

        // A client attached to the tcp port
        private void Connected(object sender, ref object t)
        {
            lock (this)
            {
                t = new PacketCount(MAXOUTSTANDINGPACKETS);
                iConnectionCount++;

                if (iConnectionCount == 1)
                {
                    ConnectionReady.Set();
                }
            }
        }

        // A client detached from the tcp port
        private void Disconnected(object sender, ref object t)
        {
            lock (this)
            {
                iConnectionCount--;
                if (iConnectionCount == 0)
                {
                    ConnectionReady.Reset();
                }
            }
        }

        private void Receive(Object sender, ref object o, ref byte[] b, int ByteCount)
        {
            PacketCount pc = (PacketCount)o;
            pc.RemovePacket();
        }

        private void Send(Object sender, ref object o, ref bool b)
        {
            PacketCount pc = (PacketCount)o;

            b = pc.AddPacket();
        }

        // Find the appropriate encoder
        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
