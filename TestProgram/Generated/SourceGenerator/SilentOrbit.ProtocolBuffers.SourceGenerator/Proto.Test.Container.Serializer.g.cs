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
namespace Proto.Test;

public partial class Container
{
    /// <summary>Helper: create a new instance to deserializing into</summary>
    public static Container Deserialize(Stream stream)
    {
        var instance = new Container();
        Deserialize(stream, instance);
        return instance;
    }

    /// <summary>Helper: create a new instance to deserializing into</summary>
    public static Container DeserializeLengthDelimited(Stream stream)
    {
        var instance = new Container();
        DeserializeLengthDelimited(stream, instance);
        return instance;
    }

    /// <summary>Helper: create a new instance to deserializing into</summary>
    public static Container DeserializeLength(Stream stream, int length)
    {
        var instance = new Container();
        DeserializeLength(stream, length, instance);
        return instance;
    }

    /// <summary>Helper: put the buffer into a MemoryStream and create a new instance to deserializing into</summary>
    public static Container Deserialize(byte[] buffer)
    {
        var instance = new Container();
        using (var ms = new MemoryStream(buffer))
            Deserialize(ms, instance);
        return instance;
    }

    /// <summary>Helper: put the buffer into a MemoryStream before deserializing</summary>
    public static global::Proto.Test.Container Deserialize(byte[] buffer, global::Proto.Test.Container instance)
    {
        using (var ms = new MemoryStream(buffer))
            Deserialize(ms, instance);
        return instance;
    }

    /// <summary>Takes the remaining content of the stream and deserialze it into the instance.</summary>
    public static global::Proto.Test.Container Deserialize(Stream stream, global::Proto.Test.Container instance)
    {
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
                    if (instance.MyNestedMessage == null)
                        instance.MyNestedMessage = global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream);
                    else
                        global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream, instance.MyNestedMessage);
                    continue;
                // Field 2 LengthDelimited
                case 18:
                    if (instance.NestedField == null)
                        instance.NestedField = global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream);
                    else
                        global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream, instance.NestedField);
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

        return instance;
    }

    /// <summary>Read the VarInt length prefix and the given number of bytes from the stream and deserialze it into the instance.</summary>
    public static global::Proto.Test.Container DeserializeLengthDelimited(Stream stream, global::Proto.Test.Container instance)
    {
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
                    if (instance.MyNestedMessage == null)
                        instance.MyNestedMessage = global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream);
                    else
                        global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream, instance.MyNestedMessage);
                    continue;
                // Field 2 LengthDelimited
                case 18:
                    if (instance.NestedField == null)
                        instance.NestedField = global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream);
                    else
                        global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream, instance.NestedField);
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

        return instance;
    }

    /// <summary>Read the given number of bytes from the stream and deserialze it into the instance.</summary>
    public static global::Proto.Test.Container DeserializeLength(Stream stream, int length, global::Proto.Test.Container instance)
    {
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
                    if (instance.MyNestedMessage == null)
                        instance.MyNestedMessage = global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream);
                    else
                        global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream, instance.MyNestedMessage);
                    continue;
                // Field 2 LengthDelimited
                case 18:
                    if (instance.NestedField == null)
                        instance.NestedField = global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream);
                    else
                        global::Proto.Test.Container.Nested.DeserializeLengthDelimited(stream, instance.NestedField);
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

        return instance;
    }

    /// <summary>Serialize the instance into the stream</summary>
    public static void Serialize(Stream stream, Container instance)
    {
        using (var msField = new MemoryStream())
        {
            if (instance.MyNestedMessage != null)
            {
                // Key for field: 1, LengthDelimited
                stream.WriteByte(10);
                ﻿msField.SetLength(0);
                global::Proto.Test.Container.Nested.Serialize(msField, instance.MyNestedMessage);
                // Length delimited byte array
                uint length1 = (uint)msField.Length;
                WriteUInt32(stream, length1);
                msField.WriteTo(stream);

            }
            if (instance.NestedField != null)
            {
                // Key for field: 2, LengthDelimited
                stream.WriteByte(18);
                ﻿msField.SetLength(0);
                global::Proto.Test.Container.Nested.Serialize(msField, instance.NestedField);
                // Length delimited byte array
                uint length2 = (uint)msField.Length;
                WriteUInt32(stream, length2);
                msField.WriteTo(stream);

            }
        }
    }

    /// <summary>Helper: Serialize into a MemoryStream and return its byte array</summary>
    public static byte[] SerializeToBytes(Container instance)
    {
        using (var ms = new MemoryStream())
        {
            Serialize(ms, instance);
            return ms.ToArray();
        }
    }
    /// <summary>Helper: Serialize with a varint length prefix</summary>
    public static void SerializeLengthDelimited(Stream stream, Container instance)
    {
        var data = SerializeToBytes(instance);
        WriteUInt32(stream, (uint)data.Length);
        stream.Write(data, 0, data.Length);
    }

    public partial class Nested
    {
        /// <summary>Helper: create a new instance to deserializing into</summary>
        public static Nested Deserialize(Stream stream)
        {
            var instance = new Nested();
            Deserialize(stream, instance);
            return instance;
        }

        /// <summary>Helper: create a new instance to deserializing into</summary>
        public static Nested DeserializeLengthDelimited(Stream stream)
        {
            var instance = new Nested();
            DeserializeLengthDelimited(stream, instance);
            return instance;
        }

        /// <summary>Helper: create a new instance to deserializing into</summary>
        public static Nested DeserializeLength(Stream stream, int length)
        {
            var instance = new Nested();
            DeserializeLength(stream, length, instance);
            return instance;
        }

        /// <summary>Helper: put the buffer into a MemoryStream and create a new instance to deserializing into</summary>
        public static Nested Deserialize(byte[] buffer)
        {
            var instance = new Nested();
            using (var ms = new MemoryStream(buffer))
                Deserialize(ms, instance);
            return instance;
        }

        /// <summary>Helper: put the buffer into a MemoryStream before deserializing</summary>
        public static global::Proto.Test.Container.Nested Deserialize(byte[] buffer, global::Proto.Test.Container.Nested instance)
        {
            using (var ms = new MemoryStream(buffer))
                Deserialize(ms, instance);
            return instance;
        }

        /// <summary>Takes the remaining content of the stream and deserialze it into the instance.</summary>
        public static global::Proto.Test.Container.Nested Deserialize(Stream stream, global::Proto.Test.Container.Nested instance)
        {
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
                        if (instance.NestedData == null)
                            instance.NestedData = global::Proto.Test.Data.DeserializeLengthDelimited(stream);
                        else
                            global::Proto.Test.Data.DeserializeLengthDelimited(stream, instance.NestedData);
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

            return instance;
        }

        /// <summary>Read the VarInt length prefix and the given number of bytes from the stream and deserialze it into the instance.</summary>
        public static global::Proto.Test.Container.Nested DeserializeLengthDelimited(Stream stream, global::Proto.Test.Container.Nested instance)
        {
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
                        if (instance.NestedData == null)
                            instance.NestedData = global::Proto.Test.Data.DeserializeLengthDelimited(stream);
                        else
                            global::Proto.Test.Data.DeserializeLengthDelimited(stream, instance.NestedData);
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

            return instance;
        }

        /// <summary>Read the given number of bytes from the stream and deserialze it into the instance.</summary>
        public static global::Proto.Test.Container.Nested DeserializeLength(Stream stream, int length, global::Proto.Test.Container.Nested instance)
        {
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
                        if (instance.NestedData == null)
                            instance.NestedData = global::Proto.Test.Data.DeserializeLengthDelimited(stream);
                        else
                            global::Proto.Test.Data.DeserializeLengthDelimited(stream, instance.NestedData);
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

            return instance;
        }

        /// <summary>Serialize the instance into the stream</summary>
        public static void Serialize(Stream stream, Nested instance)
        {
            using (var msField = new MemoryStream())
            {
                if (instance.NestedData != null)
                {
                    // Key for field: 1, LengthDelimited
                    stream.WriteByte(10);
                    ﻿msField.SetLength(0);
                    global::Proto.Test.Data.Serialize(msField, instance.NestedData);
                    // Length delimited byte array
                    uint length1 = (uint)msField.Length;
                    WriteUInt32(stream, length1);
                    msField.WriteTo(stream);

                }
            }
        }

        /// <summary>Helper: Serialize into a MemoryStream and return its byte array</summary>
        public static byte[] SerializeToBytes(Nested instance)
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, instance);
                return ms.ToArray();
            }
        }
        /// <summary>Helper: Serialize with a varint length prefix</summary>
        public static void SerializeLengthDelimited(Stream stream, Nested instance)
        {
            var data = SerializeToBytes(instance);
            WriteUInt32(stream, (uint)data.Length);
            stream.Write(data, 0, data.Length);
        }
    }

}
