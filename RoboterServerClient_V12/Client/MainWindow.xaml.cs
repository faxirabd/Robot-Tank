using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;

using VideoModul;
using Client.ServoMotorModul;
using AudioModul;
using Client.AudioVolumeService;
using Client.MusicModul;
using Client.ShutdownModul;
using Client.RoboterArmModul;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        Socket socket = null;
        VideoClient videoclient = null;
        AudioClient audioclient = null;
        AudioService audioservice = null;
        VolumeClient volumeclient = null;
        ServoClient servomotorclient = null;
        MotorClient motorclient = null;
        MotorWnd motorwnd = null;
        MusicWindow musicwnd = null;
        MusicClient musicClient = null;
        ShutdownClient shutdownclient = null;
        RobotArmWnd robotarmwnd = null;
        GPSWindow gpswindow = null;

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);

            try
            {
                socket.Connect(txb_ip.Text, 2000);

                byte[] buff = Encoding.UTF8.GetBytes("Roboter safinshaqlawa");
                socket.Send(buff);
                int count = socket.Receive(buff);
                if (count == 0)
                    MessageBox.Show("Wrong Password, Roboter will close the Connection...");
                //Robot is connected activate some buttons
                btn_disconnect.IsEnabled = true;
                btn_shutdown.IsEnabled = true;
                btn_gpsstart.IsEnabled = true;
                //every thing is OK we can now Communicate with Roboter
                videoclient = new VideoClient(txb_ip.Text, 2001, img_1, this);

                audioclient = new AudioClient(2002);
                audioservice = new AudioService(txb_ip.Text, 2003);
                volumeclient = new VolumeClient(txb_ip.Text, 2005);
                Client.ServoMotorModul.UdpSender udpsender = new ServoMotorModul.UdpSender(txb_ip.Text, 2004);
                servomotorclient = new ServoClient(udpsender, this);
                motorwnd = new MotorWnd();
                motorclient = new MotorClient(udpsender, motorwnd);
                //create Music Control Windows and register all needed events
                musicwnd = new MusicWindow();
                musicClient = new MusicClient(txb_ip.Text, 2006);
                musicwnd.cmbx_music.SelectionChanged += new SelectionChangedEventHandler(musicClient.cmbx_music_SelectionChanged);
                musicwnd.btn_start.Click += new RoutedEventHandler(musicClient.btn_start_Click);
                musicwnd.btn_stop.Click += new RoutedEventHandler(musicClient.btn_stop_Click);
                musicwnd.btn_startdemonstration.Click += new RoutedEventHandler(musicClient.btn_startdemonstration_Click);
                musicwnd.slider_volume.ValueChanged += new RoutedPropertyChangedEventHandler<double>(musicClient.slider_volume_ValueChanged);
                musicwnd.Show();
                /* create window to control Robotarm */
                robotarmwnd = new RobotArmWnd();
                RobotArmClient armclient = new RobotArmClient(txb_ip.Text,2008, robotarmwnd);
                robotarmwnd.Show();
                /////////////////////////////////////
                // create shut down Client and register the needed events //
                shutdownclient = new ShutdownClient(txb_ip.Text, 2007);
                btn_shutdown.Click +=new RoutedEventHandler(shutdownclient.btn_shutdown_Click);
                ////////////////////////////////////////////////////////////
                videoclient.Start();
                audioclient.Start_AudioClient();
                audioservice.Start_AudioServer();
                //set volume to zero initialy....
                volumeclient.SetToZero();
                //register Silder Value changed event
                slider_volume.ValueChanged += new RoutedPropertyChangedEventHandler<double>
                    (volumeclient.slider_volume_ValueChanged);
                //start servomotorservice register the Control button click events
                btn_left.Click += new RoutedEventHandler(servomotorclient.btn_left_Click);
                btn_right.Click += new RoutedEventHandler(servomotorclient.btn_right_Click);
                btn_up.Click += new RoutedEventHandler(servomotorclient.btn_up_Click);
                btn_down.Click += new RoutedEventHandler(servomotorclient.btn_down_Click);
                btn_neutral.Click += new RoutedEventHandler(servomotorclient.btn_neutral_Click);
                /////////////////////////////////////////////////////////////////////////////
                //start Motor Control Window
                motorwnd.Show();
                motorclient.Start();
                //start music window
                musicwnd.Show();                
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_disconnect_Click(object sender, RoutedEventArgs e)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);

            videoclient.Stop();
            videoclient = null;
            audioclient.Stop_AudioClient();
            audioclient = null;
            audioservice.Stop_AudioServer();
            audioservice = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(motorwnd !=null)
                motorwnd.Close();
            if (socket != null)
            {
                socket.Close();
                //socket.Shutdown(SocketShutdown.Both);
                //socket.Disconnect(false);
            }
            if (musicwnd != null)
                musicwnd.Close();
            if (robotarmwnd != null)
                robotarmwnd.Close();
            if (gpswindow != null)
                gpswindow.Close();
            if(audioclient != null)
            audioclient.Stop_AudioClient();
        }

        private void btn_gpsstart_Click(object sender, RoutedEventArgs e)
        {
            gpswindow = new GPSWindow(txb_ip.Text, 2009);
            gpswindow.Show();
        }  
    }
}
