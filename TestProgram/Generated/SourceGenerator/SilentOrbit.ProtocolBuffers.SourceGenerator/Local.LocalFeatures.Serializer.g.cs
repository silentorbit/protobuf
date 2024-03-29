﻿﻿
// Generated by ProtocolBuffer
// - a pure c# code generation implementation of protocol buffers
// Report bugs to: https://silentorbit.com/protobuf/

// DO NOT EDIT
// This file will be overwritten when CodeGenerator is run.
// To make custom modifications, edit the .proto file and add //:external before the message line
// then write the code and the changes in a separate file.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using SilentOrbit.ProtocolBuffers;
using static SilentOrbit.ProtocolBuffers.ProtocolParser;

// This is the backend code for reading and writing
namespace Local;

internal partial class LocalFeatures
{
    /// <summary>Helper: create a new instance to deserializing into</summary>
    internal static LocalFeatures Deserialize(Stream stream)
    {
        var instance = new LocalFeatures();
        Deserialize(stream, instance);
        return instance;
    }

    /// <summary>Helper: create a new instance to deserializing into</summary>
    internal static LocalFeatures DeserializeLengthDelimited(Stream stream)
    {
        var instance = new LocalFeatures();
        DeserializeLengthDelimited(stream, instance);
        return instance;
    }

    /// <summary>Helper: create a new instance to deserializing into</summary>
    internal static LocalFeatures DeserializeLength(Stream stream, int length)
    {
        var instance = new LocalFeatures();
        DeserializeLength(stream, length, instance);
        return instance;
    }

    /// <summary>Helper: put the buffer into a MemoryStream and create a new instance to deserializing into</summary>
    internal static LocalFeatures Deserialize(byte[] buffer)
    {
        var instance = new LocalFeatures();
        using (var ms = new MemoryStream(buffer))
            Deserialize(ms, instance);
        return instance;
    }

    /// <summary>Helper: put the buffer into a MemoryStream before deserializing</summary>
    internal static global::Local.LocalFeatures Deserialize(byte[] buffer, global::Local.LocalFeatures instance)
    {
        using (var ms = new MemoryStream(buffer))
            Deserialize(ms, instance);
        return instance;
    }

    /// <summary>Takes the remaining content of the stream and deserialze it into the instance.</summary>
    internal static global::Local.LocalFeatures Deserialize(Stream stream, global::Local.LocalFeatures instance)
    {
        var br = new BinaryReader(stream);
        instance.MyEnum = global::LocalFeatureTest.TopEnum.First;
        while (true)
        {
            int keyByte = stream.ReadByte();
            if (keyByte == -1)
                break;
            // Optimized reading of known fields with field ID < 16
            switch (keyByte)
            {
                // Field 1 Varint
                case 8:
                    instance.Uptime = new TimeSpan((long)(long)ReadUInt64(stream));
                    continue;
                // Field 2 Varint
                case 16:
                    instance.DueDate = new DateTime((long)(long)ReadUInt64(stream), DateTimeKind.Utc);
                    continue;
                // Field 3 Varint
                case 24:
                    instance.DueDateLocal = new DateTime((long)(long)ReadUInt64(stream));
                    continue;
                // Field 4 Fixed64
                case 33:
                    instance.Amount = br.ReadDouble();
                    continue;
                // Field 5 LengthDelimited
                case 42:
                    instance.Denial = ReadString(stream);
                    continue;
                // Field 6 LengthDelimited
                case 50:
                    instance.Secret = ReadString(stream);
                    continue;
                // Field 7 LengthDelimited
                case 58:
                    instance.Internal = ReadString(stream);
                    continue;
                // Field 8 LengthDelimited
                case 66:
                    instance.PR = ReadString(stream);
                    continue;
                // Field 9 LengthDelimited
                case 74:
                    global::Mine.MyMessageV1.DeserializeLengthDelimited(stream, instance.TestingReadOnly);
                    continue;
                // Field 10 LengthDelimited
                case 82:
                    if (instance.MyInterface == null)
                        throw new InvalidOperationException("Can't deserialize into a interfaces null pointer");
                    else
                        global::LocalFeatureTest.InterfaceTestSerializer.DeserializeLengthDelimited(stream, instance.MyInterface);
                    continue;
                // Field 11 LengthDelimited
                case 90:
                    global::LocalFeatureTest.StructTest.DeserializeLengthDelimited(stream, ref instance.MyStruct);
                    continue;
                // Field 12 LengthDelimited
                case 98:
                    global::TestB.ExternalStructSerializer.DeserializeLengthDelimited(stream, ref instance.MyExtStruct);
                    continue;
                // Field 13 LengthDelimited
                case 106:
                    if (instance.MyExtClass == null)
                        instance.MyExtClass = global::TestB.ExternalClassSerializer.DeserializeLengthDelimited(stream);
                    else
                        global::TestB.ExternalClassSerializer.DeserializeLengthDelimited(stream, instance.MyExtClass);
                    continue;
                // Field 14 Varint
                case 112:
                    instance.MyEnum = (global::LocalFeatureTest.TopEnum)ReadUInt64(stream);
                    continue;
            }

            var key = ReadKey((byte)keyByte, stream);

            // Reading field ID > 16 and unknown field ID/wire type combinations
            switch (key.Field)
            {
                case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                default:
                    SkipKey(stream, key);
                    break;
            }
        }

        instance.AfterDeserialize();
        return instance;
    }

    /// <summary>Read the VarInt length prefix and the given number of bytes from the stream and deserialze it into the instance.</summary>
    internal static global::Local.LocalFeatures DeserializeLengthDelimited(Stream stream, global::Local.LocalFeatures instance)
    {
        var br = new BinaryReader(stream);
        instance.MyEnum = global::LocalFeatureTest.TopEnum.First;
        long limit = ReadUInt32(stream);
        limit += stream.Position;
        while (true)
        {
            if (stream.Position >= limit)
            {
                if (stream.Position == limit)
                    break;
                else
                    throw new ProtocolBufferException("Read past max limit");
            }
            int keyByte = stream.ReadByte();
            if (keyByte == -1)
                throw new System.IO.EndOfStreamException();
            // Optimized reading of known fields with field ID < 16
            switch (keyByte)
            {
                // Field 1 Varint
                case 8:
                    instance.Uptime = new TimeSpan((long)(long)ReadUInt64(stream));
                    continue;
                // Field 2 Varint
                case 16:
                    instance.DueDate = new DateTime((long)(long)ReadUInt64(stream), DateTimeKind.Utc);
                    continue;
                // Field 3 Varint
                case 24:
                    instance.DueDateLocal = new DateTime((long)(long)ReadUInt64(stream));
                    continue;
                // Field 4 Fixed64
                case 33:
                    instance.Amount = br.ReadDouble();
                    continue;
                // Field 5 LengthDelimited
                case 42:
                    instance.Denial = ReadString(stream);
                    continue;
                // Field 6 LengthDelimited
                case 50:
                    instance.Secret = ReadString(stream);
                    continue;
                // Field 7 LengthDelimited
                case 58:
                    instance.Internal = ReadString(stream);
                    continue;
                // Field 8 LengthDelimited
                case 66:
                    instance.PR = ReadString(stream);
                    continue;
                // Field 9 LengthDelimited
                case 74:
                    global::Mine.MyMessageV1.DeserializeLengthDelimited(stream, instance.TestingReadOnly);
                    continue;
                // Field 10 LengthDelimited
                case 82:
                    if (instance.MyInterface == null)
                        throw new InvalidOperationException("Can't deserialize into a interfaces null pointer");
                    else
                        global::LocalFeatureTest.InterfaceTestSerializer.DeserializeLengthDelimited(stream, instance.MyInterface);
                    continue;
                // Field 11 LengthDelimited
                case 90:
                    global::LocalFeatureTest.StructTest.DeserializeLengthDelimited(stream, ref instance.MyStruct);
                    continue;
                // Field 12 LengthDelimited
                case 98:
                    global::TestB.ExternalStructSerializer.DeserializeLengthDelimited(stream, ref instance.MyExtStruct);
                    continue;
                // Field 13 LengthDelimited
                case 106:
                    if (instance.MyExtClass == null)
                        instance.MyExtClass = global::TestB.ExternalClassSerializer.DeserializeLengthDelimited(stream);
                    else
                        global::TestB.ExternalClassSerializer.DeserializeLengthDelimited(stream, instance.MyExtClass);
                    continue;
                // Field 14 Varint
                case 112:
                    instance.MyEnum = (global::LocalFeatureTest.TopEnum)ReadUInt64(stream);
                    continue;
            }

            var key = ReadKey((byte)keyByte, stream);

            // Reading field ID > 16 and unknown field ID/wire type combinations
            switch (key.Field)
            {
                case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                default:
                    SkipKey(stream, key);
                    break;
            }
        }

        instance.AfterDeserialize();
        return instance;
    }

    /// <summary>Read the given number of bytes from the stream and deserialze it into the instance.</summary>
    internal static global::Local.LocalFeatures DeserializeLength(Stream stream, int length, global::Local.LocalFeatures instance)
    {
        var br = new BinaryReader(stream);
        instance.MyEnum = global::LocalFeatureTest.TopEnum.First;
        long limit = stream.Position + length;
        while (true)
        {
            if (stream.Position >= limit)
            {
                if (stream.Position == limit)
                    break;
                else
                    throw new ProtocolBufferException("Read past max limit");
            }
            int keyByte = stream.ReadByte();
            if (keyByte == -1)
                throw new System.IO.EndOfStreamException();
            // Optimized reading of known fields with field ID < 16
            switch (keyByte)
            {
                // Field 1 Varint
                case 8:
                    instance.Uptime = new TimeSpan((long)(long)ReadUInt64(stream));
                    continue;
                // Field 2 Varint
                case 16:
                    instance.DueDate = new DateTime((long)(long)ReadUInt64(stream), DateTimeKind.Utc);
                    continue;
                // Field 3 Varint
                case 24:
                    instance.DueDateLocal = new DateTime((long)(long)ReadUInt64(stream));
                    continue;
                // Field 4 Fixed64
                case 33:
                    instance.Amount = br.ReadDouble();
                    continue;
                // Field 5 LengthDelimited
                case 42:
                    instance.Denial = ReadString(stream);
                    continue;
                // Field 6 LengthDelimited
                case 50:
                    instance.Secret = ReadString(stream);
                    continue;
                // Field 7 LengthDelimited
                case 58:
                    instance.Internal = ReadString(stream);
                    continue;
                // Field 8 LengthDelimited
                case 66:
                    instance.PR = ReadString(stream);
                    continue;
                // Field 9 LengthDelimited
                case 74:
                    global::Mine.MyMessageV1.DeserializeLengthDelimited(stream, instance.TestingReadOnly);
                    continue;
                // Field 10 LengthDelimited
                case 82:
                    if (instance.MyInterface == null)
                        throw new InvalidOperationException("Can't deserialize into a interfaces null pointer");
                    else
                        global::LocalFeatureTest.InterfaceTestSerializer.DeserializeLengthDelimited(stream, instance.MyInterface);
                    continue;
                // Field 11 LengthDelimited
                case 90:
                    global::LocalFeatureTest.StructTest.DeserializeLengthDelimited(stream, ref instance.MyStruct);
                    continue;
                // Field 12 LengthDelimited
                case 98:
                    global::TestB.ExternalStructSerializer.DeserializeLengthDelimited(stream, ref instance.MyExtStruct);
                    continue;
                // Field 13 LengthDelimited
                case 106:
                    if (instance.MyExtClass == null)
                        instance.MyExtClass = global::TestB.ExternalClassSerializer.DeserializeLengthDelimited(stream);
                    else
                        global::TestB.ExternalClassSerializer.DeserializeLengthDelimited(stream, instance.MyExtClass);
                    continue;
                // Field 14 Varint
                case 112:
                    instance.MyEnum = (global::LocalFeatureTest.TopEnum)ReadUInt64(stream);
                    continue;
            }

            var key = ReadKey((byte)keyByte, stream);

            // Reading field ID > 16 and unknown field ID/wire type combinations
            switch (key.Field)
            {
                case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                default:
                    SkipKey(stream, key);
                    break;
            }
        }

        instance.AfterDeserialize();
        return instance;
    }

    /// <summary>Serialize the instance into the stream</summary>
    internal static void Serialize(Stream stream, LocalFeatures instance)
    {
        instance.BeforeSerialize();

        var bw = new BinaryWriter(stream);
        using (var msField = new MemoryStream())
        {
            // Key for field: 1, Varint
            stream.WriteByte(8);
            WriteUInt64(stream,(ulong)instance.Uptime.Ticks);
            // Key for field: 2, Varint
            stream.WriteByte(16);
            WriteUInt64(stream,(ulong)(instance.DueDate.Kind == DateTimeKind.Utc ? instance.DueDate : instance.DueDate.ToUniversalTime()).Ticks);
            // Key for field: 3, Varint
            stream.WriteByte(24);
            WriteUInt64(stream,(ulong)instance.DueDateLocal.Ticks);
            // Key for field: 4, Fixed64
            stream.WriteByte(33);
            bw.Write(instance.Amount);
            if (instance.Denial != null)
            {
                // Key for field: 5, LengthDelimited
                stream.WriteByte(42);
                WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Denial));
            }
            if (instance.Secret != null)
            {
                // Key for field: 6, LengthDelimited
                stream.WriteByte(50);
                WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Secret));
            }
            if (instance.Internal != null)
            {
                // Key for field: 7, LengthDelimited
                stream.WriteByte(58);
                WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Internal));
            }
            if (instance.PR != null)
            {
                // Key for field: 8, LengthDelimited
                stream.WriteByte(66);
                WriteBytes(stream, Encoding.UTF8.GetBytes(instance.PR));
            }
            if (instance.TestingReadOnly != null)
            {
                // Key for field: 9, LengthDelimited
                stream.WriteByte(74);
                ﻿msField.SetLength(0);
                global::Mine.MyMessageV1.Serialize(msField, instance.TestingReadOnly);
                // Length delimited byte array
                uint length9 = (uint)msField.Length;
                WriteUInt32(stream, length9);
                msField.WriteTo(stream);

            }
            if (instance.MyInterface == null)
                throw new ProtocolBufferException("MyInterface is required by the proto specification.");
            // Key for field: 10, LengthDelimited
            stream.WriteByte(82);
            ﻿msField.SetLength(0);
            global::LocalFeatureTest.InterfaceTestSerializer.Serialize(msField, instance.MyInterface);
            // Length delimited byte array
            uint length10 = (uint)msField.Length;
            WriteUInt32(stream, length10);
            msField.WriteTo(stream);

            // Key for field: 11, LengthDelimited
            stream.WriteByte(90);
            ﻿msField.SetLength(0);
            global::LocalFeatureTest.StructTest.Serialize(msField, instance.MyStruct);
            // Length delimited byte array
            uint length11 = (uint)msField.Length;
            WriteUInt32(stream, length11);
            msField.WriteTo(stream);

            // Key for field: 12, LengthDelimited
            stream.WriteByte(98);
            ﻿msField.SetLength(0);
            global::TestB.ExternalStructSerializer.Serialize(msField, instance.MyExtStruct);
            // Length delimited byte array
            uint length12 = (uint)msField.Length;
            WriteUInt32(stream, length12);
            msField.WriteTo(stream);

            if (instance.MyExtClass != null)
            {
                // Key for field: 13, LengthDelimited
                stream.WriteByte(106);
                ﻿msField.SetLength(0);
                global::TestB.ExternalClassSerializer.Serialize(msField, instance.MyExtClass);
                // Length delimited byte array
                uint length13 = (uint)msField.Length;
                WriteUInt32(stream, length13);
                msField.WriteTo(stream);

            }
            // Key for field: 14, Varint
            stream.WriteByte(112);
            WriteUInt64(stream,(ulong)instance.MyEnum);
        }
    }

    /// <summary>Helper: Serialize into a MemoryStream and return its byte array</summary>
    internal static byte[] SerializeToBytes(LocalFeatures instance)
    {
        using (var ms = new MemoryStream())
        {
            Serialize(ms, instance);
            return ms.ToArray();
        }
    }
    /// <summary>Helper: Serialize with a varint length prefix</summary>
    internal static void SerializeLengthDelimited(Stream stream, LocalFeatures instance)
    {
        var data = SerializeToBytes(instance);
        WriteUInt32(stream, (uint)data.Length);
        stream.Write(data, 0, data.Length);
    }
}

