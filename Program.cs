using Newtonsoft.Json;
using ModbusDecoderTemp;
using Codec8;


string input = "00000000000003918E07000001925006B1DD0100000000000000000000000000000000430019000E00010000020000030000040000B30000B40000320000330000160400470200F00000150500C80000EF00000A0009000A000A0002000B000C00F5000200431D7E0044000000B5000000B6000000420000001800000000000100DA000333B7AA5890DD0000000001925006A9C10100000000000000000000000000000000430019000E00010000020000030000040000B30000B40000320000330000160400470200F00000150500C80000EF00000A00090001000A0003000B000D00F5000200431D800044000000B5000000B6000000420000001800000000000100DA000333B7AA5890DD0000000001925006A5B30100000000000000000000000000000000430019000E00010000020000030000040000B30000B40000320000330000160400470200F00000150500C80000EF00000A00090002000A0004000B001400F5000300431D7F0044000000B5000000B6000000420000001800000000000100DA000333B7AA5890DD0000000001925006A19C0100000000000000000000000000000000430019000E00010000020000030000040000B30000B40000320000330000160400470200F00000150500C80000EF00000A00090002000A0005000B001600F5000400431D840044000000B5000000B6000000420000001800000000000100DA000333B7AA5890DD00000000019250069D970100000000000000000000000000000000430019000E00010000020000030000040000B30000B40000320000330000160400470200F00000150500C80000EF00000A00090003000A0002000B000E00F5000500431D860044000000B5000000B6000000420000001800000000000100DA000333B7AA5890DD000000000192500699910100000000000000000000000000000000430019000E00010000020000030000040000B30000B40000320000330000160400470200F00000150500C80000EF00000A00090001000A0002000B001100F5000100431D880044000000B5000000B6000000420000001800000000000100DA000333B7AA5890DD000000000192500691680100000000000000000000000000000000430019000E00010000020000030000040000B30000B40000320000330000160400470200F00000150500C80000EF00000A00090001000A0003000B001500F5000200431D8B0044000000B5000000B6000000420000001800000000000100DA000333B7AA5890DD0000070000819E";

(GenericDecodeResult result, object valueOrError) = Codec8ExtendedDecoder.ParseHexadecimalString(input);
Codec8ExtendedFrame frame = (Codec8ExtendedFrame)valueOrError;
//if (result == GenericDecodeResult.SuccessCodec8Extended)
//{
//    frame = (Codec8ExtendedFrame)valueOrError;
//}


foreach (byte[] avlByteData in frame.avlDataBytesList)
{
    //Console.WriteLine(string.Join(", ", avlData));
    //byte[] timestamp = avlData[..8];  // UNIX
    //int priority = avlData[8];
    //byte[] gpsElementData = avlData[9..24];  // 15 bytes
    //byte[] ioElement = avlData[24..];

    AvlDataCodec8Extended avlData = new AvlDataCodec8Extended(avlByteData);
    DateTimeOffset avlRecordTimestamp = avlData.GetTimestamp();
    Console.WriteLine(avlRecordTimestamp);
    int priority = avlData.priority;

    GPSElement gpsElement = avlData.GetGPSElement();
    int longitude = gpsElement.GetLongitude();
    int latitude = gpsElement.GetLatitude();
    int altitude = gpsElement.GetAltitude();
    int angle = gpsElement.GetAngle();
    int satellites = gpsElement.visibleSatellites;
    int speed = gpsElement.GetSpeed();
    Console.WriteLine($"Coordinates: {longitude}, {latitude}");

    IOElementCodec8Extended ioElement = avlData.GetIOElement();
    int eventIoId = BytesToNumbers.GetInt16(ioElement.eventIoId);
    int totalCount = BytesToNumbers.GetInt16(ioElement.totalCount);

    foreach ((byte[] id, byte value) in ioElement.oneByteIdValuePairs)
    {
        int parameterId = BytesToNumbers.GetInt16(id);
        bool idExists = Enum.IsDefined(typeof(Fmc650InputOutputElement), parameterId);
        string parameterName = "(Unknown Parameter)";
        int parameterValue = (int)value;
        if (idExists)
        {
            parameterName = ((Fmc650InputOutputElement)parameterId).ToString();
        }

        Console.WriteLine($"IO Parameter: {parameterName} ({parameterId}) | Value: {parameterValue}");
    }

    foreach ((byte[] id, byte[] value) in ioElement.twoByteIdValuePairs)
    {
        int parameterId = BytesToNumbers.GetInt16(id);
        bool idExists = Enum.IsDefined(typeof(Fmc650InputOutputElement), parameterId);
        string ioParameterName = "(Unknown Parameter)";
        int parameterValue = BytesToNumbers.GetInt16(value);
        if (idExists)
        {
            ioParameterName = ((Fmc650InputOutputElement)parameterId).ToString();
        }

        Console.WriteLine($"IO Parameter: {ioParameterName} ({parameterId}) | Value: {parameterValue}");
    }

    foreach ((byte[] id, byte[] value) in ioElement.fourByteIdValuePairs)
    {
        int parameterId = BytesToNumbers.GetInt16(id);
        bool idExists = Enum.IsDefined(typeof(Fmc650InputOutputElement), parameterId);
        string ioParameterName = "(Unknown Parameter)";
        int parameterValue = BytesToNumbers.GetInt32(value);
        if (idExists)
        {
            ioParameterName = ((Fmc650InputOutputElement)parameterId).ToString();
        }

        Console.WriteLine($"IO Parameter: {ioParameterName} ({parameterId}) | Value: {parameterValue}");
    }

    foreach ((byte[] id, byte[] value) in ioElement.eightByteIdValuePairs)
    {
        int parameterId = BytesToNumbers.GetInt16(id);
        bool idExists = Enum.IsDefined(typeof(Fmc650InputOutputElement), parameterId);
        string ioParameterName = "(Unknown Parameter)";
        long parameterValue = BytesToNumbers.GetInt64(value);
        if (idExists)
        {
            ioParameterName = ((Fmc650InputOutputElement)parameterId).ToString();
        }

        Console.WriteLine($"IO Parameter: {ioParameterName} ({parameterId}) | Value: {parameterValue}");
    }

    foreach ((byte[] id, byte[] value) in ioElement.xByteIdValuePairs)
    {
        int parameterId = BytesToNumbers.GetInt16(id);
        bool idExists = Enum.IsDefined(typeof(Fmc650InputOutputElement), parameterId);
        string ioParameterName = "(Unknown Parameter)";
        long parameterValue = BytesToNumbers.GetInt64(value);
        if (idExists)
        {
            ioParameterName = ((Fmc650InputOutputElement)parameterId).ToString();
        }

        Console.WriteLine($"IO Parameter: {ioParameterName} ({parameterId}) | Value: {parameterValue}");
    }

    Console.WriteLine();
}

Console.ReadLine();


