using System;
using System.IO;
using ProtocolBuffers;

namespace Test
{
    public class TestZigZag : TestBase
    {
        /// <summary>
        /// There was once an issue with the zigzag encoding
        /// </summary>
        public static void Run()
        {
            int[] test32 = new int[] {
                int.MinValue,
                int.MinValue + 1,
                - 4,
                -3,
                -2,
                -1,
                0,
                1,
                2,
                3,
                4,
                int.MaxValue - 1,
                int.MaxValue
            };
            
            for (int n = 0; n < test32.Length; n++)
            {
                MemoryStream ms1 = new MemoryStream();
                ProtocolParser.WriteZInt32(ms1, test32 [n]);
                
                MemoryStream ms2 = new MemoryStream(ms1.ToArray());
                if (ProtocolParser.ReadZInt32(ms2) != test32 [n])
                    throw new InvalidDataException("Test failed");
                
                MemoryStream ms3 = new MemoryStream(ms1.ToArray());
                uint wire = ProtocolParser.ReadUInt32(ms3);
                int testWire = (int)(wire >> 1);
                if (wire % 2 == 1)
                    testWire = (testWire * -1) - 1;
                
                if (testWire != test32 [n])
                    throw new InvalidDataException("Test failed");
            }
            
            long[] test64 = new long[] {
                long.MinValue,
                long.MinValue + 1,
                - 4,
                -3,
                -2,
                -1,
                0,
                1,
                2,
                3,
                4,
                long.MaxValue - 1,
                long.MaxValue
            };
            
            for (int n = 0; n < test32.Length; n++)
            {
                MemoryStream ms1 = new MemoryStream();
                ProtocolParser.WriteZInt64(ms1, test64 [n]);
                
                MemoryStream ms2 = new MemoryStream(ms1.ToArray());
                if (ProtocolParser.ReadZInt64(ms2) != test64 [n])
                    throw new InvalidDataException("Test failed");
                
                MemoryStream ms3 = new MemoryStream(ms1.ToArray());
                ulong wire = ProtocolParser.ReadUInt64(ms3);
                long testWire64 = (long)(wire >> 1);
                if (wire % 2 == 1)
                    testWire64 = (testWire64 * -1) - 1;
                
                if (testWire64 != test64 [n])
                    throw new InvalidDataException("Test failed");
            }
        }
    }
}

