using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using Client;

namespace VideoModul
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class VideoClient 
    {
        DoImages doImages;
        static int m_Count;
        string actip = "127.0.0.1";
        int port = 2001;
        System.Windows.Controls.Image img = null;
        MainWindow mainwnd = null;
        public VideoClient(string actip, int port, 
            System.Windows.Controls.Image img, MainWindow mainwnd)
        {
            this.mainwnd = mainwnd;
            this.actip = actip;
            this.port = port;
            this.img = img;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected void Dispose()
        {
            if (doImages != null)
            {
                doImages.Done = true;
                doImages = null;
            }
        }

        public void Start()
        {
            // If the button says 'Start'
           
                // Create a new thread to receive the images
                try
                {
                    img.Source = null;
                    m_Count = 0;
                    doImages = new DoImages(actip, port, img, mainwnd);
                    ThreadStart o = new ThreadStart(doImages.ThreadProc);
                    Thread thread = new Thread(o);
                    thread.IsBackground = true;
                    thread.Name = "Imaging";
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
        }

        public void Stop()
        {
            // Inform the thread it should stop
            doImages.Done = true;
            doImages = null;
        }

        // ================================================================================================
        public class DoImages
        {
            // Abort indicator
            public bool Done;

            // Form to write to
            MainWindow mainwnd = null;

            // Client connection to server
            private TcpClient tcpClient;

            // stream to read from
            private NetworkStream networkStream;
            
            System.Windows.Controls.Image img = null;

            public DoImages(string actip, int nPort, System.Windows.Controls.Image img, MainWindow mainwnd)
            {

                Done = false;
                this.img = img;
                this.mainwnd = mainwnd;
                // Connect to the server and get the stream
                tcpClient = new TcpClient(actip, nPort);
                tcpClient.NoDelay = false;
                tcpClient.ReceiveTimeout = 5000;
                tcpClient.ReceiveBufferSize = 20000;
                networkStream = tcpClient.GetStream();
            }

            public void ThreadProc()
            {
                string s;
                int iBytesComing, iBytesRead, iOffset;
                byte[] byLength = new byte[10];
                byte[] byImage = new byte[1000];
                MemoryStream m = new MemoryStream(byImage);
                DLG dl=new DLG(ActualizeImage);
                do
                {
                    try
                    {
                        // Read the fixed length string that
                        // tells the image size
                        iBytesRead = networkStream.Read(byLength, 0, 10);

                        if (iBytesRead != 10)
                        {
                            Console.WriteLine("No response from host");
                           
                            break;
                        }
                        s = Encoding.ASCII.GetString(byLength);
                        iBytesComing = Convert.ToInt32(s);

                        // Make sure our buffer is big enough
                        if (iBytesComing > byImage.Length)
                        {
                            byImage = new byte[iBytesComing];
                            m = new MemoryStream(byImage);
                            tcpClient.ReceiveBufferSize = iBytesComing + 10;
                        }
                        else
                        {
                            m.Position = 0;
                        }

                        // Read the image
                        iOffset = 0;

                        do
                        {
                            iBytesRead = networkStream.Read(byImage, iOffset, iBytesComing - iOffset);
                            if (iBytesRead != 0)
                            {
                                iOffset += iBytesRead;
                            }
                            else
                            {
                                Console.WriteLine("No response from host");
                            }
                        } while ((iOffset != iBytesComing) && (!Done));


                        if (!Done)
                        {
                            // Write back a byte
                            networkStream.Write(byImage, 0, 1);

                            //// Put the image on the screen
                            ////################# m_f.pictureBox1.Image = new System.Drawing.Bitmap(m);
                           
                            img.Dispatcher.Invoke(dl, m);
                            
                            // Increment the frame count
                            m_Count++;
                        }
                    }
                    catch (Exception e)
                    {
                        // If we get out of sync, we're probably toast, since
                        // there is currently no resync mechanism
                        Console.WriteLine(e.Message);
                    }

                } while (!Done);

                networkStream.Close();
                tcpClient.Close();
                networkStream = null;
                tcpClient = null;
            }
            delegate void DLG(MemoryStream m);
            void ActualizeImage(MemoryStream m)
            {
                try
                {
                    // Put the image on the screen
                    //################# m_f.pictureBox1.Image = new System.Drawing.Bitmap(m);
                    // Create the bitmap object
                    // NOTE: This is *not* a GDI+ Bitmap object
                    BitmapImage bitmap = new BitmapImage();

                    // Well-known work-around to make Northwind images work
                    //strm.Write(photoSource, offset, photoSource.Length - offset);

                    // Read the image into the bitmap object
                    bitmap.BeginInit();
                    bitmap.StreamSource = m;
                    bitmap.EndInit();

                    // Set the Image with the Bitmap
                    img.Source = bitmap;
                }
                catch (Exception )
                {
                    //some times wrong Images come they must be thrown.....
                   //MessageBox.Show(e.Message);
                }
            }
        }
    }
}
