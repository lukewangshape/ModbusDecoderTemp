using DotNetty.Common.Utilities;
using System.Collections.Generic;
using System.Drawing.Printing;
using Newtonsoft.Json;
using System.ComponentModel;
using ModbusDecoderTemp;



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

static void AddSlaveToNetworkWithParseMap(dynamic networkMap, string bus, string address, string name, string description, string registerMap, string parseMap)
{
    Dictionary<string, dynamic> slave = new Dictionary<string, dynamic>();
    var networkMapTable = networkMap;
    slave["Address"] = address;
    slave["Name"] = name;
    slave["Description"] = description;
    slave["RegisterMap"] = registerMap;
    slave["ParseMap"] = parseMap;
    if (networkMapTable["Bus"] == null)
    {
        //c
    }
}


// Testing

//byte[] byteArray = HexStringToByteArray("02030000001185F5");
//foreach (byte b in byteArray)
//{
//    Console.WriteLine(b.ToString());
//}


//var parsedRTUResponse = ParseBasicADUResponseHexString("01030A2FF80000006A48B800013864", "RTU");
var parsedRTUResponse = ParseBasicADURequestHexString("004000000006010300000023", "TCP");
Console.WriteLine(JsonConvert.SerializeObject(parsedRTUResponse, Formatting.Indented));

