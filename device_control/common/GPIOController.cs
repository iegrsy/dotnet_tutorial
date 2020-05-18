using System;
using System.Device.Gpio;

namespace common
{
    public class GPIOController
    {
        public static bool ReadGPIO(int gpio)
        {
            using (var c = new GpioController())
            {
                c.OpenPin(gpio, PinMode.Input);
                return (bool)c.Read(gpio);
            }
        }

        public static void SetGPIO(int gpio, bool state)
        {
            using (var c = new GpioController())
            {
                c.OpenPin(gpio, PinMode.Output);
                c.Write(gpio, state);
            }
        }
    }
}
