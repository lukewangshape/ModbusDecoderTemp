using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusDecoderTemp
{
    public enum Fmc650InputOutputElement
    {
        // One byte count elements
        DigitalInput1 = 1,
        DigitalInput2 = 2,
        DigitalInput3 = 3,
        DigitalInput4 = 4,

        DigitalOutput1 = 179,
        DigitalOutput2 = 180,
        DigitalOutput3 = 50,
        DigitalOutput4 = 51,

        DataMode = 22,
        GnssStatus = 71,
        Movement = 240,
        GsmSignal = 21,
        SleepMode = 200,
        Ignition = 239,

        // Two byte count elements
        AnalogInput1 = 9,
        AnalogInput2 = 10,
        AnalogInput3 = 11,
        AnalogInput4 = 245,
        BatteryVoltage = 67,
        BatteryCurrent = 68,
        GnssPdop = 181,
        GnssHdop = 182,
        ExternalVoltage = 66,
        Speed = 24,

        // Four byte count elements


        // Eight byte count elements
        IMSI = 218
    }


}
