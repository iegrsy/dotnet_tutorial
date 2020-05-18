using System;
using System.Device.Gpio;

namespace led_on_off
{
    class Program
    {
        static void Main(string[] args)
        {
            int buttonIO = 957;
            int ledIO = 913;
            GpioController controller = new GpioController();
            controller.OpenPin(buttonIO, PinMode.Input);
            controller.OpenPin(ledIO, PinMode.Output);

            PinValue val = PinValue.Low;
            while (true)
            {
                if (val != controller.Read(buttonIO))
                {
                    val = controller.Read(buttonIO);

                    System.Console.WriteLine($"Changed button state: {val}");
                    controller.Write(ledIO, val);
                }

                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
