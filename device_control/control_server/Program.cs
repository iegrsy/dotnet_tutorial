using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace control_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine($"{args.Length}: {string.Join(" ", args)}");

            if (args.Length == 0)
            {
                CreateHostBuilder(args).Build().Run();
                return;
            }
            else if (args.Length == 2 && args[0].Equals("gpio") && int.TryParse(args[1], out _))
            {
                var resp = common.GPIOController.ReadGPIO(int.Parse(args[1]));
                System.Console.WriteLine($"GPIO {int.Parse(args[1])}: {resp}");
            }
            else if (args.Length == 3 && args[0].Equals("gpio") && int.TryParse(args[1], out _) && bool.TryParse(args[2], out _))
            {
                common.GPIOController.SetGPIO(int.Parse(args[1]), bool.Parse(args[2]));
                System.Console.WriteLine($"Set GPIO: {int.Parse(args[1])} => {bool.Parse(args[2])}");
            }
            else if (args.Length == 5 &&
                    args[0].Equals("i2c") &&
                    int.TryParse(args[1], out _) &&
                    int.TryParse(args[2], out _) &&
                    int.TryParse(args[3], out _) &&
                    int.TryParse(args[4], out _))
            {
                var resp = common.I2CController.Read(int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]));
                System.Console.WriteLine($"read: {ByteArrayToString(resp)}");
            }
            else
            {
                System.Console.WriteLine("Wrong input parameters. Please check");
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        options.ListenAnyIP(9999, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                        options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
