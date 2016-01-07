using PacketDotNet;
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
            try { var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    lbxCapturedPacketList.Items.Add(packet);
                }));

                Console.WriteLine(packet);
                var tcp = TcpPacket.GetEncapsulated(packet);
                var ip = IpPacket.GetEncapsulated(packet);
                var ethernet = EthernetPacket.GetEncapsulated(packet);
                var udp = UdpPacket.GetEncapsulated(packet);
                var icmpv4 = ICMPv4Packet.GetEncapsulated(packet);
                var icmpv6 = ICMPv6Packet.GetEncapsulated(packet);
                var igmp = IGMPv2Packet.GetEncapsulated(packet);
                var PPPoE = PPPoEPacket.GetEncapsulated(packet);
                var pppp = PPPPacket.GetEncapsulated(packet);
                var LLDP = LLDPPacket.GetEncapsulated(packet);
                var WOL = WakeOnLanPacket.GetEncapsulated(packet);
                /*if (tcp != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                                 {
                                     lbxCapturedPacketList.Items.Add(tcp);
                                 }));
                     Console.WriteLine(tcp.ToString());
                 }
                 if (ip!=null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(tcp);
                     }));
                     Console.WriteLine(ip.ToString());
                 }
                 if (ethernet != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(tcp);
                     }));
                     Console.WriteLine(ethernet.ToString());
                 }
                 if (udp != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(udp);
                     }));
                     Console.WriteLine(udp.ToString());
                 }
                 if (icmpv4 != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(icmpv4);
                     }));
                     Console.WriteLine(icmpv4.ToString());
                 }
                 if (icmpv6 != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(icmpv6);
                     }));
                     Console.WriteLine(icmpv6.ToString());
                 }
                 if (igmp != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(igmp);
                     }));
                     Console.WriteLine(igmp.ToString());
                 }
                 if (PPPoE != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(PPPoE);
                     }));
                     Console.WriteLine(PPPoE.ToString());
                 }
                 if (pppp != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(pppp);
                     }));
                     Console.WriteLine(pppp.ToString());
                 }
                 if (LLDP != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(LLDP);
                     }));
                     Console.WriteLine(LLDP.ToString());
                 }
                 if (WOL != null)
                 {
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         lbxCapturedPacketList.Items.Add(WOL);
                     }));
                     Console.WriteLine(WOL.ToString());
                 }*/
            }catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }
        }

        private void btnStopCapture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Stop the capturing process
                device.StopCapture();

                // Close the pcap device
                device.Close();
            }           
             catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }
}
    }
}
