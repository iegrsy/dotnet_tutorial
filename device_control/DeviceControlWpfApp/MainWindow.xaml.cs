using Grpc.Net.Client;
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

namespace DeviceControlWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DevControlService.DevControlServiceClient client;
        private GrpcChannel channel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLed_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReadI2C_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtBusId.Text, out _) &&
                int.TryParse(txtDevAddr.Text, out _) &&
                int.TryParse(txtRegAddr.Text, out _) &&
                int.TryParse(txtReadLength.Text, out _))
            {
                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    var rsp = client.GetI2C(new RequestGetI2C()
                    {
                        BusId = int.Parse(txtBusId.Text),
                        DevAddress = int.Parse(txtDevAddr.Text),
                        RegisterAddress = int.Parse(txtRegAddr.Text),
                        ReadLength = int.Parse(txtReadLength.Text)
                    });

                    Console.WriteLine("Read I2C: " + rsp.ToString());
                    this.lblI2C.Dispatcher.Invoke((Action)(() =>
                    {
                        this.lblI2C.Text = ByteArrayToString(rsp.Response.ToByteArray());
                    }));
                });
            }
            else
            {
                MessageBox.Show("Input Error", $"Check inputs", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            channel = GrpcChannel.ForAddress(ep);
            client = new DevControlService.DevControlServiceClient(channel);
        }

        private void btnReadGPIO_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtGPIO.Text, out _))
            {
                var rsp = client.GetGPIO(new MGPIO()
                {
                    IoPin = int.Parse(txtGPIO.Text)
                });
            }
            else
            {
                MessageBox.Show("Input Error", $"Check inputs", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
