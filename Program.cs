using DotNetty.Common.Utilities;
using System.Collections.Generic;
using System.Drawing.Printing;
using Newtonsoft.Json;
using System.ComponentModel;
using ModbusDecoderTemp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System;
using Quartz;



static byte[] HexStringToByteArray(string hexInput)
{
    int numberOfBytes = hexInput.Length / 2;  // Floor Div
    byte[] byteArray = new byte[numberOfBytes];

    for (int i = 0; i < numberOfBytes; i++)
    {
        string hexByte = hexInput.Substring(i * 2, 2);
        byteArray[i] = Convert.ToByte(hexByte, 16);
    }

    return byteArray;
}


static Dictionary<string, dynamic> ParseBasicPDUResponseHexString(string pduHexString, string modbusType)
{
    Dictionary<string, dynamic> pdu = new Dictionary<string, dynamic>();

    if (Enum.IsDefined(typeof(ModbusType), modbusType))
    {
        byte[] byteArray = HexStringToByteArray(pduHexString);
        int functionCode = byteArray[0];
        pdu["functionCode"] = functionCode;
        int currentIndex = 1;

        if ((new int[] { 5, 6, 15, 16 }).Contains(functionCode))
        {
            //pdu.Add("startAddress", (byteArray[currentIndex] * 256) + byteArray[currentIndex + 1]);
            pdu["startAddress"] = (byteArray[currentIndex] * 256) + byteArray[currentIndex + 1];
            currentIndex += 2;
        }
        if ((new int[] { 15, 16 }).Contains(functionCode))
        {
            pdu["quantity"] = (byteArray[currentIndex] * 256) + byteArray[currentIndex + 1];
            currentIndex += 2;
        }
        if ((new int[] { 1, 2, 3, 4, 15, 16 }).Contains(functionCode))
        {
            pdu["dataByteCount"] = byteArray[currentIndex];
            currentIndex += 1;
        }
        if ((new int[] { 1, 2, 3, 4, 5, 6 }).Contains(functionCode))
        {
            pdu["data"] = pduHexString.Substring(currentIndex * 2);
            //Console.WriteLine(pdu["data"]);
        }
    }

    return pdu;
}

static Dictionary<string, dynamic> ParseBasicPDURequestHexString(string pduHexString, string modbusType)
{
    Dictionary<string, dynamic> pdu = new Dictionary<string, dynamic>();

    if (Enum.IsDefined(typeof(ModbusType), modbusType))
    {
        byte[] byteArray = HexStringToByteArray(pduHexString);
        int functionCode = byteArray[0];
        pdu["functionCode"] = functionCode;
        pdu["startAddress"] = (byteArray[1] * 256) + byteArray[2];
        int currentIndex = 3;

        if ((new int[] { 1, 2, 3, 4, 15, 16 }).Contains(functionCode))
        {
            pdu["quantity"] = (byteArray[currentIndex] * 256) + byteArray[currentIndex + 1];
            currentIndex += 2;
        }
        if ((new int[] { 15, 16 }).Contains(functionCode))
        {
            pdu["dataByteCount"] = byteArray[currentIndex];
            currentIndex += 1;
        }
        if ((new int[] { 5, 6, 15, 16 }).Contains(functionCode))
        {
            pdu["data"] = pduHexString.Substring(currentIndex * 2);
        }
    }

    return pdu;
}

static int DecodeUInt_AB(int offset, byte[] byteArray)
{
    int byteIndex = offset / 8;
    return (byteArray[byteIndex] * 256) + byteArray[byteIndex + 1];
}


static Dictionary<string, dynamic> ParseBasicADUResponseHexString(string aduHexString,  string modbusType)
{
    Dictionary<string, dynamic> modbusResponse = new Dictionary<string, dynamic>();
    byte[] byteArray = HexStringToByteArray(aduHexString);

    if (modbusType == "RTU")
    {
        modbusResponse["slaveAddress"] = byteArray[0];
        string pduHexString = aduHexString.Substring(2, aduHexString.Length - 6);
        modbusResponse["pduComponents"] = ParseBasicPDUResponseHexString(pduHexString, modbusType);
        modbusResponse["crc"] = (byteArray.Last() * 256) + byteArray[byteArray.Length - 2];
    }
    else if (modbusType == "TCP")
    {
        modbusResponse["TransactionId"] = DecodeUInt_AB(0, byteArray);
        modbusResponse["ProtocolId"] = DecodeUInt_AB(16, byteArray);
        modbusResponse["Length"] = DecodeUInt_AB(32, byteArray);
        modbusResponse["slaveAddress"] = byteArray[6];
        //string pduHexString = aduHexString.Substring(14);
        string pduHexString = aduHexString[14..];
        modbusResponse["pduComponents"] = ParseBasicPDUResponseHexString(pduHexString, modbusType);
    }
    else if (modbusType == "ASCII")
    {
        if (byteArray[0] == 0x3A && 
            byteArray[byteArray.Length - 2] == 0x0D && 
            byteArray[byteArray.Length - 1] == 0x0A)
        {
            // implement logic
        }
    }

    return modbusResponse;
}

static Dictionary<string, dynamic> ParseBasicADURequestHexString(string aduHexString, string modbusType)
{
    Dictionary<string, dynamic> modbusRequest = new Dictionary<string, dynamic>();
    byte[] byteArray = HexStringToByteArray(aduHexString);
    if (modbusType == "RTU")
    {
        modbusRequest["slaveAddress"] = byteArray[0];
        string pduHexString = aduHexString.Substring(2, aduHexString.Length - 6);
        modbusRequest["pduComponents"] = ParseBasicPDURequestHexString(pduHexString, modbusType);
        modbusRequest["crc"] = (byteArray.Last() * 256) + byteArray[byteArray.Length - 2];
    }
    else if (modbusType == "TCP")
    {
        modbusRequest["TransactionId"] = DecodeUInt_AB(0, byteArray);
        modbusRequest["ProtocolId"] = DecodeUInt_AB(16, byteArray);
        modbusRequest["Length"] = DecodeUInt_AB(32, byteArray);
        modbusRequest["slaveAddress"] = byteArray[6];
        string pduHexString = aduHexString[14..];
        modbusRequest["pduComponents"] = ParseBasicPDURequestHexString(pduHexString, modbusType);
    }
    else if (modbusType == "ASCII")
    {
        if (byteArray[0] == 0x3A &&
            byteArray[byteArray.Length - 2] == 0x0D &&
            byteArray[byteArray.Length - 1] == 0x0A)
        {
            // implement logic
        }
    }

    return modbusRequest;
}

static Dictionary<dynamic, dynamic> AddSlaveToNetworkWithParseMap(Dictionary<dynamic, dynamic> networkMap, string bus, string address, string name, string description, dynamic registerMap, dynamic parseMap)
{
    Dictionary<string, dynamic> slave = new Dictionary<string, dynamic>();
    Dictionary<dynamic, dynamic> networkMapTable = networkMap;
    slave["Address"] = address;
    slave["Name"] = name;
    slave["Description"] = description;
    slave["RegisterMap"] = registerMap;
    slave["ParseMap"] = parseMap;
    if (!networkMapTable.ContainsKey(bus))
    {
        networkMapTable[bus] = new Dictionary<dynamic, dynamic>();
    }
    networkMapTable[bus][address] = slave;

    return networkMapTable;
}


// Testing

//byte[] byteArray = HexStringToByteArray("02030000001185F5");
//foreach (byte b in byteArray)
//{
//    Console.WriteLine(b.ToString());
//}

//var parsedRTUResponse = ParseBasicADUResponseHexString("01030A2FF80000006A48B800013864", "RTU");
var parsedRTUResponse = ParseBasicADURequestHexString("004000000006010300000023", "TCP");
//Console.WriteLine(JsonConvert.SerializeObject(parsedRTUResponse, Formatting.Indented));

Dictionary<string, dynamic> ModbusMap = new Dictionary<string, dynamic> {
    //{ "MOD_MAP_CONXIONS_WATER_METER_V1", "{\"MODBUS_MAP\":{\"2\":{\"HOLDING_REG\":{\"Units\":\"M3/H\", \"Writeable\":false, \"Description\":\"Instantaneous Flow rate\", \"Name\":\"Flow Rate\", \"Size\":32, \"Readable\":true, \"Encoding\":\"UINT32\",\"_Current\":true,\"_StartTime\":1593732266,\"_EndTime\":0,\"_Id\":1593732266}}, \"4\":{\"HOLDING_REG\":{\"Units\":\"C\", \"Writeable\":false, \"Description\":\"Instantaneous Water Temperature\", \"Name\":\"Water Temperature\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732267,\"_EndTime\":0,\"_Id\":1593732267}}, \"5\":{\"HOLDING_REG\":{\"Units\":\"ss\", \"Writeable\":false, \"Description\":\"Time Second\", \"Name\":\"Second\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732268,\"_EndTime\":0,\"_Id\":1593732268}}, \"6\":{\"HOLDING_REG\":{\"Units\":\"mm\", \"Writeable\":false, \"Description\":\"Time Minute\", \"Name\":\"Minute\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732269,\"_EndTime\":0,\"_Id\":1593732269}}, \"7\":{\"HOLDING_REG\":{\"Units\":\"hh\", \"Writeable\":false, \"Description\":\"Time Hour\", \"Name\":\"Hour\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732270,\"_EndTime\":0,\"_Id\":1593732270}}, \"8\":{\"HOLDING_REG\":{\"Units\":\"DD\", \"Writeable\":false, \"Description\":\"Time Day\", \"Name\":\"Day\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732271,\"_EndTime\":0,\"_Id\":1593732271}}, \"9\":{\"HOLDING_REG\":{\"Units\":\"MM\", \"Writeable\":false, \"Description\":\"Time Month\", \"Name\":\"Month\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732272,\"_EndTime\":0,\"_Id\":1593732272}}, \"10\":{\"HOLDING_REG\":{\"Units\":\"YYYY\", \"Writeable\":false, \"Description\":\"Time Year\", \"Name\":\"Year\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732273,\"_EndTime\":0,\"_Id\":1593732273}}, \"11\":{\"HOLDING_REG\":{\"Units\":\"b\", \"Writeable\":false, \"Description\":\"Status\", \"Name\":\"Status\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732274,\"_EndTime\":0,\"_Id\":1593732274}}, \"16\":{\"HOLDING_REG\":{\"Units\":\"a\", \"Writeable\":true, \"Description\":\"Slave Address\", \"Name\":\"Slave Address\", \"Size\":16, \"Readable\":true, \"Encoding\":\"UINT16\",\"_Current\":true,\"_StartTime\":1593732275,\"_EndTime\":0,\"_Id\":1593732275}}, \"0\":{\"HOLDING_REG\":{\"Units\":\"M3\", \"Writeable\":false, \"Description\":\"Meter Reading\", \"Name\":\"Total Consumption\", \"Size\":32, \"Readable\":true, \"Encoding\":\"UINT32\",\"_Current\":true,\"_StartTime\":1593732276,\"_EndTime\":0,\"_Id\":1593732276}}}}"}
    { "MOD_MAP_CONXIONS_WATER_METER_V1", """{"MODBUS_MAP":{"2":{"HOLDING_REG":{"Units":"M3/H", "Writeable":false, "Description":"Instantaneous Flow rate", "Name":"Flow Rate", "Size":32, "Readable":true, "Encoding":"UINT32","_Current":true,"_StartTime":1593732266,"_EndTime":0,"_Id":1593732266}}, "4":{"HOLDING_REG":{"Units":"C", "Writeable":false, "Description":"Instantaneous Water Temperature", "Name":"Water Temperature", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732267,"_EndTime":0,"_Id":1593732267}}, "5":{"HOLDING_REG":{"Units":"ss", "Writeable":false, "Description":"Time Second", "Name":"Second", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732268,"_EndTime":0,"_Id":1593732268}}, "6":{"HOLDING_REG":{"Units":"mm", "Writeable":false, "Description":"Time Minute", "Name":"Minute", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732269,"_EndTime":0,"_Id":1593732269}}, "7":{"HOLDING_REG":{"Units":"hh", "Writeable":false, "Description":"Time Hour", "Name":"Hour", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732270,"_EndTime":0,"_Id":1593732270}}, "8":{"HOLDING_REG":{"Units":"DD", "Writeable":false, "Description":"Time Day", "Name":"Day", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732271,"_EndTime":0,"_Id":1593732271}}, "9":{"HOLDING_REG":{"Units":"MM", "Writeable":false, "Description":"Time Month", "Name":"Month", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732272,"_EndTime":0,"_Id":1593732272}}, "10":{"HOLDING_REG":{"Units":"YYYY", "Writeable":false, "Description":"Time Year", "Name":"Year", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732273,"_EndTime":0,"_Id":1593732273}}, "11":{"HOLDING_REG":{"Units":"b", "Writeable":false, "Description":"Status", "Name":"Status", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732274,"_EndTime":0,"_Id":1593732274}}, "16":{"HOLDING_REG":{"Units":"a", "Writeable":true, "Description":"Slave Address", "Name":"Slave Address", "Size":16, "Readable":true, "Encoding":"UINT16","_Current":true,"_StartTime":1593732275,"_EndTime":0,"_Id":1593732275}}, "0":{"HOLDING_REG":{"Units":"M3", "Writeable":false, "Description":"Meter Reading", "Name":"Total Consumption", "Size":32, "Readable":true, "Encoding":"UINT32","_Current":true,"_StartTime":1593732276,"_EndTime":0,"_Id":1593732276}}}, "Parse": null}"""}
};

var networkConfig = new Dictionary<dynamic, dynamic>();
//var waterMeterMap = ModbusMap["MOD_MAP_CONXIONS_WATER_METER_V1"];
Dictionary<string, dynamic> waterMeterMap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ModbusMap["MOD_MAP_CONXIONS_WATER_METER_V1"]);
networkConfig = AddSlaveToNetworkWithParseMap(networkConfig, "BUS_A", "2", "Conxions", "Test Modbus Map", waterMeterMap["MODBUS_MAP"], waterMeterMap["Parse"]);
Console.WriteLine(JsonConvert.SerializeObject(networkConfig, Formatting.Indented));
