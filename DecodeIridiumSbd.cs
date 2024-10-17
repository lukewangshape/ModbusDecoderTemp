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
        string input = "g\u00105\u0005ü<\aKºD\u0002\b\0\0";


        public void DoTheThing()
        {
            Console.WriteLine("from decode iridium sbd.cs");

            byte[] byteArray = Encoding.ASCII.GetBytes(input);

            for (int i = 0; i < byteArray.Length; i++)
            {
                Console.WriteLine(byteArray[i]);
            }

            byte[] epochBytes = byteArray[..4];
            byte[] longitudeBytes = byteArray[4..6];
            byte[] latitudeBytes = byteArray[7..9];
            byte sdbAvlId = byteArray[10];
            byte digitalIoInfo = byteArray[11];
            byte reserved = byteArray[12];
            byte speed = byteArray[13];

            int epoch = BytesToNumbers.GetInt32(epochBytes);  // little endian

        }



    }
}
