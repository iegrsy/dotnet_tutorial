using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace control_server
{
    public class Program
    {
        public class Options
        {
            [Option('i', "i2c", Required = false, HelpText = "i2c mode")]
            public bool FlagI2C { get; set; }

            [Option('b', "bus", Required = false, Default = 1, HelpText = "i2c bus id")]
            public int BusId { get; set; }

            [Option('d', "devId", Required = false, Default = (int)0x40, HelpText = "i2c dev id")]
            public int DevId { get; set; }

            [Option('a', "addr", Required = false, Default = (int)0xfe, HelpText = "i2c register addr")]
            public int Addr { get; set; }

            [Option('c', "readLength", Required = false, Default = 2, HelpText = "i2c read legth")]
            public int ReadLength { get; set; }

            [Option('g', "gpio", Required = false, HelpText = "gpio mode")]
            public bool FlagGPIO { get; set; }

            [Option('p', "pin", Required = false, Default = 0, HelpText = "gpio pin")]
            public int GPIOPin { get; set; }

            [Option('s', "state", Required = false, Default = -1, HelpText = "gpio pin state")]
            public int State { get; set; }
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o => RunWithOpts(o, args))
            .WithNotParsed<Options>(e => ErrorHandle(e));
        }

        static void ErrorHandle(IEnumerable<Error> e)
        {
            foreach (var item in e)
                System.Console.WriteLine(item.ToString(), null);
        }

        static void RunWithOpts(Options o, string[] args)
        {
            if (o.FlagGPIO || o.FlagI2C)
            {
                CreateHostBuilder(args).Build().Run();
                return;
            }

            if (o.FlagI2C)
            {
                var resp = common.I2CController.Read(o.BusId, o.DevId, o.Addr, o.ReadLength);
                System.Console.WriteLine($"read: {resp}");
            }

            if (o.FlagGPIO)
            {
                if (o.State == -1)
                {
                    var resp = common.GPIOController.ReadGPIO(o.GPIOPin);
                    System.Console.WriteLine($"GPIO: {resp}");
                }
                else
                {
                    common.GPIOController.SetGPIO(o.GPIOPin, o.State > 0);
                    System.Console.WriteLine($"GPIO: {o.State > 0}");
                }
            }
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
