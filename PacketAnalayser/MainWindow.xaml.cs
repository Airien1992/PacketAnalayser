using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacketAnalayser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Retrieve all capture devices
                   
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnCap_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                CaptureDeviceList cdevices = CaptureDeviceList.Instance;
                if (cdevices.Count >= 1)
                {
                    gbxDevList.Header = cdevices.Count + " Devices";
                    foreach (ICaptureDevice dev in cdevices)
                    {
                        lbxNetworkCardsLists.Items.Add(dev);
                    }
                }
                else
                {
                    gbxDevList.Header = "No Devices";
                }

                /* // check device isn't null
                 if (cdevices.Count < 1 || cdevices == null)
                     throw new NullReferenceException();*/

                gbxDevList.Visibility = Visibility.Visible;
            }

            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

        }


        private void NetworkCardsLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfo.Visibility = Visibility.Visible;
            ICaptureDevice dev = lbxNetworkCardsLists.SelectedItem as ICaptureDevice;
            gbxDevInfo.DataContext = dev;
        }
    }
}
