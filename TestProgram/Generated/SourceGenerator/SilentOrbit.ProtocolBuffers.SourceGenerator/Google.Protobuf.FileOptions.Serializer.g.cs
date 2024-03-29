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
namespace Google.Protobuf;

public partial class FileOptions
{
    /// <summary>Helper: create a new instance to deserializing into</summary>
    public static FileOptions Deserialize(Stream stream)
    {
        var instance = new FileOptions();
        Deserialize(stream, instance);
        return instance;
    }

    /// <summary>Helper: create a new instance to deserializing into</summary>
    public static FileOptions DeserializeLengthDelimited(Stream stream)
    {
        var instance = new FileOptions();
        DeserializeLengthDelimited(stream, instance);
        return instance;
    }

    /// <summary>Helper: create a new instance to deserializing into</summary>
    public static FileOptions DeserializeLength(Stream stream, int length)
    {
        var instance = new FileOptions();
        DeserializeLength(stream, length, instance);
        return instance;
    }

    /// <summary>Helper: put the buffer into a MemoryStream and create a new instance to deserializing into</summary>
    public static FileOptions Deserialize(byte[] buffer)
    {
        var instance = new FileOptions();
        using (var ms = new MemoryStream(buffer))
            Deserialize(ms, instance);
        return instance;
    }

    /// <summary>Helper: put the buffer into a MemoryStream before deserializing</summary>
    public static global::Google.Protobuf.FileOptions Deserialize(byte[] buffer, global::Google.Protobuf.FileOptions instance)
    {
        using (var ms = new MemoryStream(buffer))
            Deserialize(ms, instance);
        return instance;
    }

    /// <summary>Takes the remaining content of the stream and deserialze it into the instance.</summary>
    public static global::Google.Protobuf.FileOptions Deserialize(Stream stream, global::Google.Protobuf.FileOptions instance)
    {
        instance.JavaMultipleFiles = false;
        instance.JavaGenerateEqualsAndHash = false;
        instance.OptimizeFor = global::Google.Protobuf.FileOptions.OptimizeMode.SPEED;
        instance.CcGenericServices = false;
        instance.JavaGenericServices = false;
        instance.PyGenericServices = false;
        if (instance.UninterpretedOption == null)
            instance.UninterpretedOption = new List<global::Google.Protobuf.UninterpretedOption>();
        while (true)
        {
            int keyByte = stream.ReadByte();
            if (keyByte == -1)
                break;
            // Optimized reading of known fields with field ID < 16
            switch (keyByte)
            {
                // Field 1 LengthDelimited
                case 10:
                    instance.JavaPackage = ReadString(stream);
                    continue;
                // Field 8 LengthDelimited
                case 66:
                    instance.JavaOuterClassname = ReadString(stream);
                    continue;
                // Field 10 Varint
                case 80:
                    instance.JavaMultipleFiles = ReadBool(stream);
                    continue;
                // Field 9 Varint
                case 72:
                    instance.OptimizeFor = (global::Google.Protobuf.FileOptions.OptimizeMode)ReadUInt64(stream);
                    continue;
                // Field 11 LengthDelimited
                case 90:
                    instance.GoPackage = ReadString(stream);
                    continue;
            }

            var key = ReadKey((byte)keyByte, stream);

            // Reading field ID > 16 and unknown field ID/wire type combinations
            switch (key.Field)
            {
                case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 20:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.JavaGenerateEqualsAndHash = ReadBool(stream);
                    continue;
                case 16:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.CcGenericServices = ReadBool(stream);
                    continue;
                case 17:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.JavaGenericServices = ReadBool(stream);
                    continue;
                case 18:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.PyGenericServices = ReadBool(stream);
                    continue;
                case 999:
                    if(key.WireType != Wire.LengthDelimited)
                        break;
                    // repeated
                    instance.UninterpretedOption.Add(global::Google.Protobuf.UninterpretedOption.DeserializeLengthDelimited(stream));
                    continue;
                default:
                    SkipKey(stream, key);
                    break;
            }
        }

        return instance;
    }

    /// <summary>Read the VarInt length prefix and the given number of bytes from the stream and deserialze it into the instance.</summary>
    public static global::Google.Protobuf.FileOptions DeserializeLengthDelimited(Stream stream, global::Google.Protobuf.FileOptions instance)
    {
        instance.JavaMultipleFiles = false;
        instance.JavaGenerateEqualsAndHash = false;
        instance.OptimizeFor = global::Google.Protobuf.FileOptions.OptimizeMode.SPEED;
        instance.CcGenericServices = false;
        instance.JavaGenericServices = false;
        instance.PyGenericServices = false;
        if (instance.UninterpretedOption == null)
            instance.UninterpretedOption = new List<global::Google.Protobuf.UninterpretedOption>();
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
                // Field 1 LengthDelimited
                case 10:
                    instance.JavaPackage = ReadString(stream);
                    continue;
                // Field 8 LengthDelimited
                case 66:
                    instance.JavaOuterClassname = ReadString(stream);
                    continue;
                // Field 10 Varint
                case 80:
                    instance.JavaMultipleFiles = ReadBool(stream);
                    continue;
                // Field 9 Varint
                case 72:
                    instance.OptimizeFor = (global::Google.Protobuf.FileOptions.OptimizeMode)ReadUInt64(stream);
                    continue;
                // Field 11 LengthDelimited
                case 90:
                    instance.GoPackage = ReadString(stream);
                    continue;
            }

            var key = ReadKey((byte)keyByte, stream);

            // Reading field ID > 16 and unknown field ID/wire type combinations
            switch (key.Field)
            {
                case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 20:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.JavaGenerateEqualsAndHash = ReadBool(stream);
                    continue;
                case 16:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.CcGenericServices = ReadBool(stream);
                    continue;
                case 17:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.JavaGenericServices = ReadBool(stream);
                    continue;
                case 18:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.PyGenericServices = ReadBool(stream);
                    continue;
                case 999:
                    if(key.WireType != Wire.LengthDelimited)
                        break;
                    // repeated
                    instance.UninterpretedOption.Add(global::Google.Protobuf.UninterpretedOption.DeserializeLengthDelimited(stream));
                    continue;
                default:
                    SkipKey(stream, key);
                    break;
            }
        }

        return instance;
    }

    /// <summary>Read the given number of bytes from the stream and deserialze it into the instance.</summary>
    public static global::Google.Protobuf.FileOptions DeserializeLength(Stream stream, int length, global::Google.Protobuf.FileOptions instance)
    {
        instance.JavaMultipleFiles = false;
        instance.JavaGenerateEqualsAndHash = false;
        instance.OptimizeFor = global::Google.Protobuf.FileOptions.OptimizeMode.SPEED;
        instance.CcGenericServices = false;
        instance.JavaGenericServices = false;
        instance.PyGenericServices = false;
        if (instance.UninterpretedOption == null)
            instance.UninterpretedOption = new List<global::Google.Protobuf.UninterpretedOption>();
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
                // Field 1 LengthDelimited
                case 10:
                    instance.JavaPackage = ReadString(stream);
                    continue;
                // Field 8 LengthDelimited
                case 66:
                    instance.JavaOuterClassname = ReadString(stream);
                    continue;
                // Field 10 Varint
                case 80:
                    instance.JavaMultipleFiles = ReadBool(stream);
                    continue;
                // Field 9 Varint
                case 72:
                    instance.OptimizeFor = (global::Google.Protobuf.FileOptions.OptimizeMode)ReadUInt64(stream);
                    continue;
                // Field 11 LengthDelimited
                case 90:
                    instance.GoPackage = ReadString(stream);
                    continue;
            }

            var key = ReadKey((byte)keyByte, stream);

            // Reading field ID > 16 and unknown field ID/wire type combinations
            switch (key.Field)
            {
                case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 20:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.JavaGenerateEqualsAndHash = ReadBool(stream);
                    continue;
                case 16:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.CcGenericServices = ReadBool(stream);
                    continue;
                case 17:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.JavaGenericServices = ReadBool(stream);
                    continue;
                case 18:
                    if(key.WireType != Wire.Varint)
                        break;
                    instance.PyGenericServices = ReadBool(stream);
                    continue;
                case 999:
                    if(key.WireType != Wire.LengthDelimited)
                        break;
                    // repeated
                    instance.UninterpretedOption.Add(global::Google.Protobuf.UninterpretedOption.DeserializeLengthDelimited(stream));
                    continue;
                default:
                    SkipKey(stream, key);
                    break;
            }
        }

        return instance;
    }

    /// <summary>Serialize the instance into the stream</summary>
    public static void Serialize(Stream stream, FileOptions instance)
    {
        using (var msField = new MemoryStream())
        {
            if (instance.JavaPackage != null)
            {
                // Key for field: 1, LengthDelimited
                stream.WriteByte(10);
                WriteBytes(stream, Encoding.UTF8.GetBytes(instance.JavaPackage));
            }
            if (instance.JavaOuterClassname != null)
            {
                // Key for field: 8, LengthDelimited
                stream.WriteByte(66);
                WriteBytes(stream, Encoding.UTF8.GetBytes(instance.JavaOuterClassname));
            }
            if (instance.JavaMultipleFiles != false)
            {
                // Key for field: 10, Varint
                stream.WriteByte(80);
                WriteBool(stream, instance.JavaMultipleFiles);
            }
            if (instance.JavaGenerateEqualsAndHash != false)
            {
                // Key for field: 20, Varint
                stream.WriteByte(160);
                stream.WriteByte(1);
                WriteBool(stream, instance.JavaGenerateEqualsAndHash);
            }
            if (instance.OptimizeFor != global::Google.Protobuf.FileOptions.OptimizeMode.SPEED)
            {
                // Key for field: 9, Varint
                stream.WriteByte(72);
                WriteUInt64(stream,(ulong)instance.OptimizeFor);
            }
            if (instance.GoPackage != null)
            {
                // Key for field: 11, LengthDelimited
                stream.WriteByte(90);
                WriteBytes(stream, Encoding.UTF8.GetBytes(instance.GoPackage));
            }
            if (instance.CcGenericServices != false)
            {
                // Key for field: 16, Varint
                stream.WriteByte(128);
                stream.WriteByte(1);
                WriteBool(stream, instance.CcGenericServices);
            }
            if (instance.JavaGenericServices != false)
            {
                // Key for field: 17, Varint
                stream.WriteByte(136);
                stream.WriteByte(1);
                WriteBool(stream, instance.JavaGenericServices);
            }
            if (instance.PyGenericServices != false)
            {
                // Key for field: 18, Varint
                stream.WriteByte(144);
                stream.WriteByte(1);
                WriteBool(stream, instance.PyGenericServices);
            }
            if (instance.UninterpretedOption != null)
            {
                foreach (var i999 in instance.UninterpretedOption)
                {
                    // Key for field: 999, LengthDelimited
                    stream.WriteByte(186);
                    stream.WriteByte(62);
                    ﻿msField.SetLength(0);
                    global::Google.Protobuf.UninterpretedOption.Serialize(msField, i999);
                    // Length delimited byte array
                    uint length999 = (uint)msField.Length;
                    WriteUInt32(stream, length999);
                    msField.WriteTo(stream);

                }
            }
        }
    }

    /// <summary>Helper: Serialize into a MemoryStream and return its byte array</summary>
    public static byte[] SerializeToBytes(FileOptions instance)
    {
        using (var ms = new MemoryStream())
        {
            Serialize(ms, instance);
            return ms.ToArray();
        }
    }
    /// <summary>Helper: Serialize with a varint length prefix</summary>
    public static void SerializeLengthDelimited(Stream stream, FileOptions instance)
    {
        var data = SerializeToBytes(instance);
        WriteUInt32(stream, (uint)data.Length);
        stream.Write(data, 0, data.Length);
    }
}

