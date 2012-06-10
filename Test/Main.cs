using System;
using System.IO;
using ProtoBuf;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using Personal;
using Yours;
using Mine;
using ProtocolBuffers;
using Local;

namespace Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello Binary World!");
            
            TestReadme();
            
            TestZigZag();
            
            TestStandardFeatures();
            
            TestLocalFeatures();
            
            TestProtoBufNet();

            TestPerformance();
        }
        
        /// <summary>
        /// Example found in the README file
        /// </summary>
        public static void TestReadme()
        {
            MemoryStream stream = new MemoryStream();
            
            Person person = new Person();
            person.Name = "George";
            Person.Serialize(stream, person);
            
            stream.Seek(0, SeekOrigin.Begin);
            
            Person person2 = Person.Deserialize(stream);
            Test("ReadMe Person test", person.Name == person2.Name);
        }
        
        /// <summary>
        /// There was once an issue with the zigzag encoding
        /// </summary>
        public static void TestZigZag()
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
                ProtocolParser.WriteSInt32(ms1, test32 [n]);
                
                MemoryStream ms2 = new MemoryStream(ms1.ToArray());
                if (ProtocolParser.ReadSInt32(ms2) != test32 [n])
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
                ProtocolParser.WriteSInt64(ms1, test64 [n]);
                
                MemoryStream ms2 = new MemoryStream(ms1.ToArray());
                if (ProtocolParser.ReadSInt64(ms2) != test64 [n])
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
        
        /// <summary>
        /// This is a simple test to trigger most functionality of the generated code.
        /// </summary>
        static void TestStandardFeatures()
        {
            MyMessageV2 version2before = MyMessageV2.TestInstance();

            //Write
            MemoryStream ms = new MemoryStream();
            MyMessageV2.Serialize(ms, version2before);
            
            Console.WriteLine("Wire bytes: " + ms.Length);
            
            //Read
            MemoryStream msread = new MemoryStream(ms.ToArray());
            MyMessageV2 version2after = MyMessageV2.Deserialize(msread);
            
            //Verify
            Test("MyMessageV2 WriteRead", version2before.Equals(version2after));

            //Read by older version
            MemoryStream msread1 = new MemoryStream(ms.ToArray());
            MyMessageV1 version1 = MyMessageV1.Deserialize(msread1);
            
            Console.WriteLine("Version 1");
            Test("FieldA", version2before.FieldA == version1.FieldA);
        }
        
        /// <summary>
        /// Test wire format of the person example against protobuf-net - another c# protocol buffers library
        /// </summary>
        static void TestLocalFeatures()
        {
            LocalFeatures local = new LocalFeatures("139pt2m7");
            local.Uptime = TimeSpan.FromHours(37.8);
            local.DueDate = DateTime.Now.AddMinutes(1);
            local.Internal = "assembly";
            local.PR = "Hi";
            local.Amount = Math.E;
            local.Deny("they exist");
            local.MyInterface = new MyImplementeInterface();
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
            using (MemoryStream ms = new MemoryStream ())
            {
                MyMessageV2.Serialize(ms, v2original);
                streamBuffer = ms.ToArray();
                streamBufferV2Orig = streamBuffer;
            }
            
            //Read V1, modify and write back
            MyMessageV1 v1 = MyMessageV1.Deserialize(new MemoryStream(streamBuffer));
            v1.FieldA = 42;
            using (MemoryStream ms = new MemoryStream ())
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
                if (streamBufferV2Orig [n] != streamBufferV1Mod [n])
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

        /// <summary>
        /// Test wire format of the person example against protobuf-net - another c# protocol buffers library
        /// </summary>
        static void TestProtoBufNet()
        {
            Person p1 = new Person();
            p1.Name = "Alice";
            p1.Id = 17532;
            p1.Email = "alice@silentorbit.com";
            p1.Phone = new List<Person.PhoneNumber>();
            p1.Phone.Add(new Person.PhoneNumber(){ Type = Person.PhoneType.MOBILE, Number = "+46 11111111111"});
            p1.Phone.Add(new Person.PhoneNumber(){ Type = Person.PhoneType.HOME, Number = "+46 777777777"});
            MemoryStream ms1 = new MemoryStream();
            Person.Serialize(ms1, p1);

            MemoryStream ms2 = new MemoryStream(ms1.ToArray());
            NetPerson p2 = ProtoBuf.Serializer.Deserialize<NetPerson>(ms2);

            //Test
            Test("12 Name", p1.Name == p2.Name);
            Test("12 Id", p1.Id == p2.Id);
            Test("12 Email", p1.Email == p2.Email);
            Test("12 Phone", p1.Phone.Count == p2.Phone.Count);
            Test("12 Phone[0]", p1.Phone [0].Number == p2.Phone [0].Number);
            Test("12 Phone[0]", (int)p1.Phone [0].Type == (int)p2.Phone [0].Type);
            Test("12 Phone[1]", p1.Phone [1].Number == p2.Phone [1].Number);
            //Disabled test since missing data should return the default value(HOME).
            //Test ("12 Phone[1]", (int)p1.Phone [1].Type == (int)p2.Phone [1].Type);
            
            //Correct invalid data for the next test
            p2.Phone [1].Type = Person.PhoneType.HOME;
            
            MemoryStream ms3 = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms3, p2);

            //Test wire data
            byte[] b1 = ms1.ToArray();
            byte[] b3 = ms3.ToArray();
            Test("WireLength", b1.Length == b3.Length);
            if (b1.Length == b3.Length)
            {
                for (int n = 0; n < b1.Length; n++)
                    if (b1 [n] != b3 [n])
                        Test("Wire" + n, b1 [n] == b3 [n]);
            } else
            {
                Console.WriteLine(BitConverter.ToString(b1));
                Console.WriteLine();
                Console.WriteLine(BitConverter.ToString(b3));
            }
            
            MemoryStream ms4 = new MemoryStream(ms3.ToArray());
            Person p4 = Person.Deserialize(ms4);
            
            //Test          
            Test("14 Name", p1.Name == p4.Name);
            Test("14 Id", p1.Id == p4.Id);
            Test("14 Email", p1.Email == p4.Email);
            Test("14 Phone", p1.Phone.Count == p4.Phone.Count);
            Test("14 Phone[0]", p1.Phone [0].Number == p4.Phone [0].Number);
            Test("14 Phone[0]", p1.Phone [0].Type == p4.Phone [0].Type);
            Test("14 Phone[1]", p1.Phone [1].Number == p4.Phone [1].Number);
            Test("14 Phone[1]", p1.Phone [1].Type == p4.Phone [1].Type);
        }

        private static void TestPerformance()
        {
            AddressBook ab = new AddressBook();
            NetAddressBook nab = new NetAddressBook();
            ab.List = new List<Person>();
            nab.List = new List<NetPerson>();
            //Generating structures
            for (int n = 0; n < 1000; n++)
            {
                Person p = new Person();
                p.Name = "Alice" + n;
                p.Id = 17532;
                p.Email = "Alice@silentobit.com";
                p.Phone = new List<Person.PhoneNumber>();
                ab.List.Add(p);
                
                NetPerson np = new NetPerson();
                np.Name = p.Name;
                np.Id = p.Id;
                np.Email = p.Email;
                np.Phone = new List<NetPerson.NetPhoneNumber>();
                nab.List.Add(np);

                for (int m = 0; m < 1000; m++)
                {
                    Person.PhoneNumber pn = new Person.PhoneNumber();
                    pn.Type = Person.PhoneType.MOBILE;
                    pn.Number = m.ToString();
                    p.Phone.Add(pn);

                    NetPerson.NetPhoneNumber npn = new NetPerson.NetPhoneNumber();
                    npn.Type = Person.PhoneType.MOBILE;
                    npn.Number = pn.Number;
                    np.Phone.Add(npn);
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize
                DateTime start = DateTime.Now;
                AddressBook.Serialize(ms, ab);
                TimeSpan serialize = DateTime.Now - start;
                Console.WriteLine("Speed test: Serialize " + ab.List.Count + " posts in   " + serialize.TotalSeconds + " s");

                //Deserialize
                ms.Seek(0, SeekOrigin.Begin);
                start = DateTime.Now;
                var dab = AddressBook.Deserialize(new StreamRead(ms));
                TimeSpan deserialize = DateTime.Now - start;
                Console.WriteLine("Speed test: Deserialize " + dab.List.Count + " posts in " + deserialize.TotalSeconds + " s");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize 
                DateTime start = DateTime.Now;
                ProtoBuf.Serializer.Serialize(ms, nab);
                TimeSpan serialize = DateTime.Now - start;
                Console.WriteLine("Protobuf-net: Serialize " + nab.List.Count + " posts in   " + serialize.TotalSeconds + " s");

                //Deserialize
                ms.Seek(0, SeekOrigin.Begin);
                start = DateTime.Now;
                var dab = ProtoBuf.Serializer.Deserialize<NetAddressBook>(ms);
                TimeSpan deserialize = DateTime.Now - start;
                Console.WriteLine("Protobuf-net: Deserialize " + dab.List.Count + " posts in " + deserialize.TotalSeconds + " s");
            }
        }

        public static void Test(string message, bool result)
        {
            Console.WriteLine(message + ": " + (result ? "OK" : "Fail!"));
            if (result == false)
                throw new InvalidProgramException("Failed test: " + message);
        }
        
    }
}
