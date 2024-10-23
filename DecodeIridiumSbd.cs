using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codec8;

namespace ModbusDecoderTemp
{
    internal class DecodeIridiumSbd
    {
        //string input = "g\u00105\u0005ü<\aKºD\u0002\b\0\0";
        byte[] byteArray = new byte[] {
            0x67, 0x10, 0x33, 0xD0, 0xFC, 0x3C, 0x07,
            0x4B, 0xBA, 0x48, 0x03, 0xE8, 0x20, 0x01
        };

        public double CalculateLongitude(byte[] longitudeBytes)
        {
            byte[] longitude4Bytes = new byte[] { 0x00 }.Concat(longitudeBytes).ToArray();
            double longitude = BytesToNumbers.GetUInt32(longitude4Bytes);

            longitude = longitude / 46603.375;
            longitude -= 180;
            return longitude;
        }

        public double CalculateLatitude(byte[] latitudeBytes)
        {
            latitudeBytes = new byte[] { 0x00 }.Concat(latitudeBytes).ToArray();
            double latitude = BytesToNumbers.GetUInt32(latitudeBytes);

            latitude = latitude / 93206.750;
            latitude -= 90;
            return latitude;
        }

        public string ByteToBinaryString(byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }


        public void DoTheThing()
        {
            Console.WriteLine("from decode iridium sbd.cs");

            //byte[] byteArray = Encoding.ASCII.GetBytes(input);

            for (int i = 0; i < byteArray.Length; i++)
            {
                Console.WriteLine(byteArray[i]);
            }

            byte[] epochBytes = byteArray[..4];
            byte[] longitudeBytes = byteArray[4..7];
            byte[] latitudeBytes = byteArray[7..10];
            byte sdbAvlId = byteArray[10];
            byte digitalIoInfo = byteArray[11];
            byte reserved = byteArray[12];
            int speed = byteArray[13];

            int epoch = BytesToNumbers.GetInt32(epochBytes);  // little endian
            double longitude = CalculateLongitude(longitudeBytes);
            double latitude = CalculateLatitude(latitudeBytes);
        }



    }
}
