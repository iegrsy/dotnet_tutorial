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
using DevControl;
using Grpc.Core;

namespace DeviceControlWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DevControlService.DevControlServiceClient client;
        private Channel channel;

        public MainWindow()
        {
            InitializeComponent();

            txtBusId.Text = "3";
            txtDevAddr.Text = "80";
            txtRegAddr.Text = "80";
            txtReadLength.Text = "2";

            txtEP.Text = "192.168.2.137:9999";
        }

        private void btnLed_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReadI2C_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(txtBusId.Text, out _) &&
                    int.TryParse(txtDevAddr.Text, out _) &&
                    int.TryParse(txtRegAddr.Text, out _) &&
                    int.TryParse(txtReadLength.Text, out _))
                {
                    var m = new RequestGetI2C()
                    {
                        BusId = int.Parse(txtBusId.Text),
                        DevAddress = int.Parse(txtDevAddr.Text),
                        RegisterAddress = int.Parse(txtRegAddr.Text),
                        ReadLength = int.Parse(txtReadLength.Text)
                    };

                    new System.Threading.Thread(() =>
                    {
                        try
                        {
                            var rsp = client.GetI2C(m, deadline: DateTime.UtcNow.AddMilliseconds(5000));

                            Console.WriteLine("Read I2C: " + ByteArrayToString(rsp.Response.ToByteArray()));
                            this.lblI2C.Dispatcher.Invoke((Action)(() =>
                            {
                                this.lblI2C.Text = ByteArrayToString(rsp.Response.ToByteArray());
                            }));
                        }
                        catch (Exception err) { Console.WriteLine(err); }
                    }).Start();
                }
                else
                {
                    MessageBox.Show("Input Error", $"Check inputs", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception err) { MessageBox.Show("Input Error", $"{err.Message}", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            var ep = txtEP.Text;
            if (ep?.Length <= 0)
                return;
            try
            {
                channel = new Channel(ep, ChannelCredentials.Insecure);
                client = new DevControlService.DevControlServiceClient(channel);
            }
            catch (Exception err) { MessageBox.Show("Input Error", $"{err.Message}", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void btnReadGPIO_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(txtGPIO.Text, out _))
                {
                    var rsp = client.GetGPIO(new MGPIO()
                    {
                        IoPin = int.Parse(txtGPIO.Text)
                    });

                    this.lblGPIO.Dispatcher.Invoke((Action)(() =>
                    {
                        this.lblGPIO.Text = $"{rsp.State}";
                    }));
                }
                else
                {
                    MessageBox.Show("Input Error", $"Check inputs", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }
    }
}
