using SharpPcap;
using SharpPcap.AirPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;
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
        ICaptureDevice device;
        String packet="1", prevPacket="1";
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
                    gbxDeviceList.Header = cdevices.Count + " Devices";
                    foreach (ICaptureDevice dev in cdevices)
                    {
                        if (dev is AirPcapDevice)
                        {
                            lbxAirPcapDeviceList.Items.Add(dev as AirPcapDevice);
                        }
                        else if (dev is WinPcapDevice)
                        {
                            lbxWinPcapDeviceList.Items.Add(dev as WinPcapDevice);
                        }
                        else if (dev is LibPcapLiveDevice)
                        {
                            lbxLibPcapLiveDeviceList.Items.Add(dev as LibPcapLiveDevice);
                        }
                    }
                }
                else
                {
                    gbxDeviceList.Header = "No Devices";
                }

                /* // check device isn't null
                 if (cdevices.Count < 1 || cdevices == null)
                     throw new NullReferenceException();*/

                gbxDeviceList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

        }

        private void lbxAirPcapDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfo.Visibility = Visibility.Visible;
            AirPcapDevice dev = lbxAirPcapDeviceList.SelectedItem as AirPcapDevice;
            gbxDevInfo.DataContext = dev;
            lbxAdresses.ItemsSource = dev.Addresses;
        }

        private void lbxWinPcapDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfo.Visibility = Visibility.Visible;
            WinPcapDevice dev = lbxWinPcapDeviceList.SelectedItem as WinPcapDevice;
            gbxDevInfo.DataContext = dev;
            lbxAdresses.ItemsSource = dev.Addresses;
        }

        private void lbxLibPcapLiveDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfo.Visibility = Visibility.Visible;
            LibPcapLiveDevice dev = lbxLibPcapLiveDeviceList.SelectedItem as LibPcapLiveDevice;
            gbxDevInfo.DataContext = dev;
            lbxAdresses.ItemsSource = dev.Addresses;
        }

        private void btnStartCapture_Click(object sender, RoutedEventArgs e)
        {
            // Extract a device from the list
            device = gbxDevInfo.DataContext as ICaptureDevice;

            // Register our handler function to the
            // 'packet arrival' event
            device.OnPacketArrival +=
                new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            // Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            Console.WriteLine("-- Listening on {0}, hit 'Enter' to stop...",
                device.Description);

            // Start the capturing process
            device.StartCapture();

            // Wait for 'Enter' from the user.
            Console.ReadLine();
           

           
        }
        private void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            DateTime time = e.Packet.Timeval.Date;
            int len = e.Packet.Data.Length;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
            time.Hour, time.Minute, time.Second, time.Millisecond, len);
           packet = time.Hour+": "+ time.Minute + ": " + time.Second + ": " + time.Millisecond + ": " + len;
            if (packet != prevPacket)
            {
                tbxPacket.Text = packet;
                prevPacket = packet;
            }
        }

        private void btnStopCapture_Click(object sender, RoutedEventArgs e)
        {
            // Stop the capturing process
            device.StopCapture();

            // Close the pcap device
            device.Close();
        }
    }
}
