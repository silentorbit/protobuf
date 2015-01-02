using System;
using Local;
using System.IO;
using Yours;
using Mine;

namespace Test
{
    public class TestLocalFeatures : TestBase
    {

        /// <summary>
        /// Test wire format of the person example against protobuf-net - another c# protocol buffers library
        /// </summary>
        public static void Run()
        {
            LocalFeatures local = new LocalFeatures("139pt2m7");
            local.Uptime = TimeSpan.FromHours(37.8);
            local.DueDate = DateTime.Now.AddMinutes(1);
            local.Internal = "assembly";
            local.PR = "Hi";
            local.Amount = Math.E;
            local.Deny("they exist");
            local.MyInterface = new MyImplementeInterface();
            local.MyEnum = LocalFeatureTest.TopEnum.Last;
            MemoryStream ms1 = new MemoryStream();
            LocalFeatures.Serialize(ms1, local);

            MemoryStream ms2 = new MemoryStream(ms1.ToArray());
            LocalFeatures l2 = new LocalFeatures("Secret");
            //Since this property is an interface AND required we must set it before calling Deserialize
            l2.MyInterface = new MyImplementeInterface();
            LocalFeatures.Deserialize(ms2, l2);

            //Test in Equals to have access to all fields
            Test("Local Features", local.Equals(l2));


            //Test preservation of unknown fields
            byte[] streamBuffer;
            byte[] streamBufferV2Orig;
            byte[] streamBufferV1Mod;
            MyMessageV2 v2original = MyMessageV2.TestInstance();

            //Write
            using (MemoryStream ms = new MemoryStream())
            {
                MyMessageV2.Serialize(ms, v2original);
                streamBuffer = ms.ToArray();
                streamBufferV2Orig = streamBuffer;
            }

            //Read V1, modify and write back
            MyMessageV1 v1 = MyMessageV1.Deserialize(new MemoryStream(streamBuffer));
            v1.FieldA = 42;
            using (MemoryStream ms = new MemoryStream())
            {
                MyMessageV1.Serialize(ms, v1);
                streamBuffer = ms.ToArray();
                streamBufferV1Mod = streamBuffer;
            }

            //Compare stream buffers
            //Test (
            //  "Stream buffer length",
            //  streamBufferV2Orig.Length == streamBufferV1Mod.Length
            //);
            for (int n = 0; n < streamBufferV2Orig.Length; n++)
            {
                if (n == 1)
                    continue; //expected difference for FieldA
                if (streamBufferV2Orig[n] != streamBufferV1Mod[n])
                    throw new InvalidDataException("Stream buffers do not match at byte " + n);
            }

            //Read V2 and test
            MyMessageV2 v2test = MyMessageV2.Deserialize(new MemoryStream(streamBuffer));
            //Test FieldA
            Test("Modified in v1", v2test.FieldA == v1.FieldA);
            //Restore and test entire v2
            v2test.FieldA = v2original.FieldA;
            Test("MyMessageV2 WriteRead", v2original.Equals(v2test));
        }

    }
}

