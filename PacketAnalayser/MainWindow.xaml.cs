using MahApps.Metro.Controls;
using PacketDotNet;
using SharpPcap;
using SharpPcap.AirPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

    public partial class MainWindow : MetroWindow
    {
        ICaptureDevice device;
        String packet = "1", prevPacket = "1";
        
        WinPcapDevice devo;

        bool UDPfilter = false;
        bool ICMPfilter = false;
        bool TCPfilter = false;
        bool Nofilter = true;
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
            gridCapture.Visibility = Visibility.Visible;
        }

        private void lbxWinPcapDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfo.Visibility = Visibility.Visible;
            WinPcapDevice dev = lbxWinPcapDeviceList.SelectedItem as WinPcapDevice;
            gbxDevInfo.DataContext = dev;
            lbxAdresses.ItemsSource = dev.Addresses;
            gridCapture.Visibility = Visibility.Visible;
        }

        private void lbxLibPcapLiveDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfo.Visibility = Visibility.Visible;
            LibPcapLiveDevice dev = lbxLibPcapLiveDeviceList.SelectedItem as LibPcapLiveDevice;
            gbxDevInfo.DataContext = dev;
            lbxAdresses.ItemsSource = dev.Addresses;
            gridCapture.Visibility = Visibility.Visible;
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
            try
            {
                Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

                if (packet.PayloadPacket != null)
                {
                    if (UDPfilter)
                    {
                        if ((UdpPacket)packet.Extract(typeof(UdpPacket)) != null)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                lbxCapturedPacketList.Items.Add(packet);
                            }));
                        }
                    }
                    else if (TCPfilter)
                    {
                        if ((TcpPacket)packet.Extract(typeof(TcpPacket)) != null)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                lbxCapturedPacketList.Items.Add(packet);
                            }));
                        }
                    }
                    else if (ICMPfilter)
                    {
                        if ((ICMPv4Packet)packet.Extract(typeof(ICMPv4Packet)) != null || (ICMPv6Packet)packet.Extract(typeof(ICMPv6Packet)) != null)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                lbxCapturedPacketList.Items.Add(packet);
                            }));
                        }

                    }
                    else if (Nofilter)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            lbxCapturedPacketList.Items.Add(packet);
                        }));
                    }
                    else
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            lbxCapturedPacketList.Items.Add(packet);
                        }));
                    }

                }


                Console.WriteLine(packet);

            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }
        }

        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            lbxCapturedPacketList.Items.Clear();
        }

        private void lbxCapturedPacketList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                gbxPacketInfo.Visibility = Visibility.Visible;
                Packet packet = lbxCapturedPacketList.SelectedItem as Packet;
                Console.WriteLine(packet.GetType());
                TcpPacket tcp = (TcpPacket)packet.Extract(typeof(TcpPacket));
                IpPacket ip = (IpPacket)packet.Extract(typeof(IpPacket));
                EthernetPacket ethernet = (EthernetPacket)packet.Extract(typeof(EthernetPacket));
                UdpPacket udp = (UdpPacket)packet.Extract(typeof(UdpPacket));
                ICMPv4Packet icmpv4 = (ICMPv4Packet)packet.Extract(typeof(ICMPv4Packet));
                ICMPv6Packet icmpv6 = (ICMPv6Packet)packet.Extract(typeof(ICMPv6Packet));
                ICMPv6Packet igmp = (ICMPv6Packet)packet.Extract(typeof(ICMPv6Packet));
                PPPoEPacket PPPoE = (PPPoEPacket)packet.Extract(typeof(PPPoEPacket));
                PPPPacket pppp = (PPPPacket)packet.Extract(typeof(PPPPacket));
                LLDPPacket LLDP = (LLDPPacket)packet.Extract(typeof(LLDPPacket));
                WakeOnLanPacket WOL = (WakeOnLanPacket)packet.Extract(typeof(WakeOnLanPacket));
                if (tcp != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                                {
                                    tbxInfo.Text = tcp.ToString(StringOutputType.Verbose);

                                }));
                }
                if (ip != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = ip.ToString(StringOutputType.Verbose);
                    }));
                }
                if (ethernet != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = ethernet.ToString(StringOutputType.Verbose);
                    }));
                }
                if (udp != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = udp.ToString(StringOutputType.Verbose);
                    }));
                }
                if (icmpv4 != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = icmpv4.ToString(StringOutputType.Verbose);
                    }));
                }
                if (icmpv6 != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = icmpv6.ToString(StringOutputType.Verbose);
                    }));
                }
                if (igmp != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = igmp.ToString(StringOutputType.Verbose);
                    }));
                }
                if (PPPoE != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = PPPoE.ToString(StringOutputType.Verbose);
                    }));
                }
                if (pppp != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = pppp.ToString(StringOutputType.Verbose);
                    }));
                }
                if (LLDP != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = LLDP.ToString(StringOutputType.Verbose);
                    }));
                }
                if (WOL != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbxInfo.Text = WOL.ToString(StringOutputType.Verbose);
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }


        }

        private void btnCFilterProtocol_Click(object sender, RoutedEventArgs e)
        {
            if (cbxUDP.IsSelected == true)
            {
                UDPfilter = true;
                ICMPfilter = false;
                TCPfilter = false;
                Nofilter = false;
            }
            if (cbxTCP.IsSelected == true)
            {
                UDPfilter = false;
                ICMPfilter = false;
                TCPfilter = true;
                Nofilter = false;
            }
            if (cbxICMP.IsSelected == true)
            {
                UDPfilter = false;
                ICMPfilter = true;
                TCPfilter = false;
                Nofilter = false;
            }
            if (cbxNo.IsSelected == true)
            {
                UDPfilter = false;
                ICMPfilter = false;
                TCPfilter = false;
                Nofilter = true;
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
        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                CaptureDeviceList cdevices = CaptureDeviceList.Instance;
                if (cdevices.Count >= 1)
                {
                    gbxDeviceListI.Header = cdevices.Count + " Devices";
                    foreach (ICaptureDevice dev in cdevices)
                    {
                        if (dev is AirPcapDevice)
                        {
                            lbxAirPcapDeviceListI.Items.Add(dev as AirPcapDevice);
                        }
                        else if (dev is WinPcapDevice)
                        {
                            lbxWinPcapDeviceListI.Items.Add(dev as WinPcapDevice);
                        }
                        else if (dev is LibPcapLiveDevice)
                        {
                            lbxLibPcapLiveDeviceListI.Items.Add(dev as LibPcapLiveDevice);
                        }
                    }
                }
                else
                {
                    gbxDeviceListI.Header = "No Devices";
                }

                /* // check device isn't null
                 if (cdevices.Count < 1 || cdevices == null)
                     throw new NullReferenceException();*/

                gbxDeviceListI.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

        }

        private void lbxAirPcapDeviceListI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfoI.Visibility = Visibility.Visible;
            AirPcapDevice dev = lbxAirPcapDeviceListI.SelectedItem as AirPcapDevice;
            gbxDevInfo.DataContext = dev;
            lbxAdressesI.ItemsSource = dev.Addresses;
        }

        private void lbxWinPcapDeviceListI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfoI.Visibility = Visibility.Visible;
            devo = lbxWinPcapDeviceListI.SelectedItem as WinPcapDevice;
            gbxDevInfo.DataContext = devo;
            lbxAdressesI.ItemsSource = devo.Addresses;
        }

        private void lbxLibPcapLiveDeviceListI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbxDevInfoI.Visibility = Visibility.Visible;
            LibPcapLiveDevice dev = lbxLibPcapLiveDeviceListI.SelectedItem as LibPcapLiveDevice;
            gbxDevInfo.DataContext = dev;
            lbxAdressesI.ItemsSource = dev.Addresses;
        }

        private void btnSendPacket_Click(object sender, RoutedEventArgs e)
        {
            device = gbxDevInfo.DataContext as ICaptureDevice;
            // Open the device
            device.Open();


            try
            {
                IPAddress ip = IPAddress.Parse(tbxSourceIp.Text);
                IPAddress ipaddress = System.Net.IPAddress.Parse(tbxDestinationIp.Text);
                TcpPacket tcpPakje = new TcpPacket(80, 80);
                IPv4Packet pakje = new IPv4Packet(ip, ipaddress);
                pakje.PayloadData = System.Text.Encoding.ASCII.GetBytes(tbxPayloadIp.Text);
                pakje.TimeToLive = int.Parse(tbxTTLIp.Text);
               // pakje.Protocol = tbxProtocolIp.Text;
                device.SendPacket(pakje);
                Console.WriteLine("-- Packet sent successfuly.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("-- " + ex.Message);
            }

            // Close the pcap device
            device.Close();
            Console.WriteLine("-- Device closed.");
        }

    }
}
