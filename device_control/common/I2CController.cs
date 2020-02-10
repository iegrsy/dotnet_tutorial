using System;
using System.Device.I2c;

namespace common
{
    public class I2CController
    {
        public static byte[] Read(int busId, int devAddress, int register, int readLength)
        {
            using (var dev = I2cDevice.Create(new I2cConnectionSettings(busId, devAddress)))
            {
                var readByte = new Span<byte>(new byte[readLength]);
                dev.WriteRead(new ReadOnlySpan<byte>(new byte[] { (byte)register }), readByte);
                return readByte.ToArray();
            }
        }
    }
}
