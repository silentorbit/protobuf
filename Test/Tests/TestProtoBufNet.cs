using System;
using Personal;
using System.IO;
using System.Collections.Generic;

namespace Test
{
    public class TestProtoBufNet : TestBase
    {
        
        /// <summary>
        /// Test wire format of the person example against protobuf-net - another c# protocol buffers library
        /// </summary>
        public static void Run()
        {
            Person p1 = new Person();
            p1.Name = "Alice";
            p1.Id = -240;
            p1.Email = "alice@silentorbit.com";
            p1.Phone = new List<Person.PhoneNumber>();
            p1.Phone.Add(new Person.PhoneNumber(){ Type = Person.PhoneType.MOBILE, Number = "+46 11111111111"});
            p1.Phone.Add(new Person.PhoneNumber(){ Type = Person.PhoneType.HOME, Number = "+46 777777777"});

            //Serialize using this(Protobuf code generator)
            MemoryStream ms1 = new MemoryStream();
            Person.Serialize(ms1, p1);

            //Deserialize using ProtoBuf.Net
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

    }
}

