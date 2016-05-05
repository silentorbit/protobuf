using System;
using Personal;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class TestProtoBufNet
    {

        /// <summary>
        /// Test wire format of the person example against protobuf-net - another c# protocol buffers library
        /// </summary>
        [TestMethod]
        public void Run()
        {
            Person p1 = new Person();
            p1.Name = "Alice";
            p1.Id = -240;
            p1.Email = "alice@silentorbit.com";
            p1.Phone = new List<Person.PhoneNumber>();

            // Pretty sure protobuf-net serializes default optional fields wrong (https://code.google.com/p/protobuf-net/issues/detail?id=136)
            // Don't set PhoneType.HOME, otherwise tests will fail.
            bool nullable = HasNullable<Person.PhoneNumber>("Type");

            p1.Phone.Add(new Person.PhoneNumber() { Type = Person.PhoneType.MOBILE, Number = "+46 11111111111" });
            if (nullable)
                p1.Phone.Add(new Person.PhoneNumber() { Type = Person.PhoneType.WORK, Number = "+46 777777777" });
            else
                p1.Phone.Add(new Person.PhoneNumber() { Type = Person.PhoneType.HOME, Number = "+46 777777777" });

            //Serialize using this(Protobuf code generator)
            MemoryStream ms1 = new MemoryStream();
            Person.Serialize(ms1, p1);

            //Deserialize using ProtoBuf.Net
            MemoryStream ms2 = new MemoryStream(ms1.ToArray());
            NetPerson p2 = ProtoBuf.Serializer.Deserialize<NetPerson>(ms2);

            //Test
            Assert.AreEqual(p1.Name, p2.Name);
            Assert.AreEqual(p1.Id, p2.Id);
            Assert.AreEqual(p1.Email, p2.Email);
            Assert.AreEqual(p1.Phone.Count, p2.Phone.Count);
            Assert.AreEqual(p1.Phone[0].Number, p2.Phone[0].Number);
            Assert.AreEqual((int)p1.Phone[0].Type, (int)p2.Phone[0].Type);
            Assert.AreEqual(p1.Phone[1].Number, p2.Phone[1].Number);
            //Disabled test since missing data should return the default value(HOME).
            //Test ("12 Phone[1]", (int)p1.Phone [1].Type, (int)p2.Phone [1].Type);

            //Correct invalid data for the next test
            if (nullable == false)
                p2.Phone[1].Type = Person.PhoneType.HOME;

            MemoryStream ms3 = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms3, p2);

            //Test wire data
            byte[] b1 = ms1.ToArray();
            byte[] b3 = ms3.ToArray();
            Assert.AreEqual(b1.Length, b3.Length, "WireLength");
            for(int n = 0; n < b1.Length; n++)
                Assert.AreEqual(b1[n], b3[n]);

            MemoryStream ms4 = new MemoryStream(ms3.ToArray());
            Person p4 = Person.Deserialize(ms4);

            //Test          
            Assert.AreEqual(p1.Name, p4.Name);
            Assert.AreEqual(p1.Id, p4.Id);
            Assert.AreEqual(p1.Email, p4.Email);
            Assert.AreEqual(p1.Phone.Count, p4.Phone.Count);
            Assert.AreEqual(p1.Phone[0].Number, p4.Phone[0].Number);
            Assert.AreEqual(p1.Phone[0].Type, p4.Phone[0].Type);
            Assert.AreEqual(p1.Phone[1].Number, p4.Phone[1].Number);
            Assert.AreEqual(p1.Phone[1].Type, p4.Phone[1].Type);
        }

        /// <summary>
        /// Return true if T has a property that is nullable.
        /// The property is assumed to exist.
        /// </summary>
        static bool HasNullable<T>(string property)
        {
            var t = typeof(T);
            var p = t.GetProperty(property);
            var pt = p.PropertyType;
            if (pt.IsGenericType == false)
                return false;
            var td = pt.GetGenericTypeDefinition();
            return td == typeof(Nullable<>);
        }
    }
}

