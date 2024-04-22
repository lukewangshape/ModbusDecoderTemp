using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusDecoderTemp
{
    public enum ModbusType
    {
        RTU,
        TCP,
        ASCII
    }

    //public Dictionary<string, dynamic> ModbusMap = new Dictionary<string, dynamic> { 
    //    { "MOD_MAP_CONXIONS_WATER_METER_V1", "{\"MODBUS_MAP\":{\"2\":{\"HOLDING_REG\":{\"Units\":\"M3/H\", \"Writeable\":false, \"Description\":\"Instantaneous Flow rate\", \"Name\":\"Flow Rate\", \"Size\":32, \"Readable\":true, \"Encoding\":\"UINT32\",\"_Current\":true,\"_StartTime\":1593732266,\"_EndTime\":0,\"_Id\":1593732266}}, \"4\":{\"HOLDING_REG\":{\"Units\":\"C\", \"Writeable\":false, \"Description\":\"Instantaneous Water Temperature\", \"Name\":\"Water Temperature\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732267,\"_EndTime\":0,\"_Id\":1593732267}}, \"5\":{\"HOLDING_REG\":{\"Units\":\"ss\", \"Writeable\":false, \"Description\":\"Time Second\", \"Name\":\"Second\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732268,\"_EndTime\":0,\"_Id\":1593732268}}, \"6\":{\"HOLDING_REG\":{\"Units\":\"mm\", \"Writeable\":false, \"Description\":\"Time Minute\", \"Name\":\"Minute\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732269,\"_EndTime\":0,\"_Id\":1593732269}}, \"7\":{\"HOLDING_REG\":{\"Units\":\"hh\", \"Writeable\":false, \"Description\":\"Time Hour\", \"Name\":\"Hour\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732270,\"_EndTime\":0,\"_Id\":1593732270}}, \"8\":{\"HOLDING_REG\":{\"Units\":\"DD\", \"Writeable\":false, \"Description\":\"Time Day\", \"Name\":\"Day\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732271,\"_EndTime\":0,\"_Id\":1593732271}}, \"9\":{\"HOLDING_REG\":{\"Units\":\"MM\", \"Writeable\":false, \"Description\":\"Time Month\", \"Name\":\"Month\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732272,\"_EndTime\":0,\"_Id\":1593732272}}, \"10\":{\"HOLDING_REG\":{\"Units\":\"YYYY\", \"Writeable\":false, \"Description\":\"Time Year\", \"Name\":\"Year\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732273,\"_EndTime\":0,\"_Id\":1593732273}}, \"11\":{\"HOLDING_REG\":{\"Units\":\"b\", \"Writeable\":false, \"Description\":\"Status\", \"Name\":\"Status\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732274,\"_EndTime\":0,\"_Id\":1593732274}}, \"16\":{\"HOLDING_REG\":{\"Units\":\"a\", \"Writeable\":true, \"Description\":\"Slave Address\", \"Name\":\"Slave Address\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732275,\"_EndTime\":0,\"_Id\":1593732275}}, \"0\":{\"HOLDING_REG\":{\"Units\":\"M3\", \"Writeable\":false, \"Description\":\"Meter Reading\", \"Name\":\"Total Consumption\", \"Size\":32, \"Readable\":true, \"Encoding\":\"UINT32\",\"_Current\":true,\"_StartTime\":1593732276,\"_EndTime\":0,\"_Id\":1593732276}}}}"}, 
    
    //};
}
