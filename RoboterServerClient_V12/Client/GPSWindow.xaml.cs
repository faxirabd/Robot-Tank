using Client.GPSModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for GPSWindow.xaml
    /// </summary>
    public partial class GPSWindow : Window
    {
        public GPSWindow()
        {
            InitializeComponent();
        }

        GPSClient gpsclient = null;
        public GPSWindow(string ip, int port):this()
        {
            gpsclient = new GPSClient(ip, port, this);
        }

        byte[] buff = new byte[] { 1, 2, 3 };
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            gpsclient.Send(buff);
            gpsclient.Start();
        }

        private void btn_location_Click(object sender, RoutedEventArgs e)
        {
            string url = null;
            if ((gpsclient.satellitesNr == null) || (gpsclient.satellitesNr == " 00") || (gpsclient.satellitesNr == "----")) //if there is no GPS satalite available
            {
                //in case if there is no GPS signal we will show previuos saved location
                //url = "https://www.google.co.uk/maps/place/36.155050,44.063095";//home
                url = "https://www.google.co.uk/maps/place/36.683562,44.536734";
            }
            else
            {
                int i = gpsclient.Ncoordinate.IndexOf('.');
                string n2 = gpsclient.Ncoordinate.Substring(i - 2);
                i = gpsclient.Ecoordinate.IndexOf('.');
                string e2 = gpsclient.Ecoordinate.Substring(i - 2);

                string n1 = gpsclient.Ncoordinate.Substring(0, i - 3);
                string e1 = gpsclient.Ecoordinate.Substring(0, i - 2);

                double N = double.Parse(n1) + (double.Parse(n2) / 60.0);
                double E = double.Parse(e1) + (double.Parse(e2) / 60.0);

                //MessageBox.Show(N + ",  " + E);

                url = "https://www.google.co.uk/maps/place/"+ N + "," + E;
            }
           

            using (Process process = new Process())
            {
                process.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
                process.StartInfo.Arguments = url;

                process.Start();
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //gpsclient.Close();
        }

        /*
        Informally, specifying a geographic location usually means giving the location's latitude and longitude. The numerical values for latitude and longitude can occur in a number of different formats:[2]

        degrees minutes seconds: 40° 26′ 46″ N 79° 58′ 56″ W
        degrees decimal minutes: 40° 26.767′ N 79° 58.933′ W
        decimal degrees: 40.446° N 79.982° W
        There are 60 minutes in a degree and 60 seconds in a minute. Then to convert from a degrees minutes seconds format to a decimal degrees format, one may use the formula

         \rm{decimal\  degrees} = \rm{degrees} + \rm{minutes}/60 + \rm{seconds}/3600.
        To convert back from decimal degree format to degrees minutes seconds format,

         \begin{align}
          \rm{degrees} & = \lfloor\rm{decimal\  degrees}\rfloor \\
          \rm{minutes} & = \lfloor 60*(\rm{decimal\  degrees} - \rm{degrees})\rfloor  \\
          \rm{seconds} & = \lfloor 3600*(\rm{decimal\  degrees} - \rm{degrees} - \rm{minutes}/60)\rfloor \\
          \end{align} 
        where the notation \lfloor x \rfloor means take the integer part of x.
        */
    }
}
