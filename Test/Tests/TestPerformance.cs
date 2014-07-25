using System;
using Personal;
using System.Collections.Generic;
using System.IO;
using SilentOrbit.ProtocolBuffers;

namespace Test
{
    public class TestPerformance : TestBase
    {
        public static void Run()
        {
            Console.WriteLine("Starting speed test...");

            AddressBook ab = new AddressBook();
            NetAddressBook nab = new NetAddressBook();
            ab.List = new List<Person>();
            nab.List = new List<NetPerson>();
            //Generating structures
            for (int n = 0; n < 5000; n++)
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
                GC.Collect();
                var start = DateTime.Now;
                AddressBook.Serialize(ms, ab);
                var serialize = DateTime.Now - start;
                Console.WriteLine("Speed test no stack: Serialize " + ab.List.Count + " posts in   " + serialize.TotalSeconds + " s");
            }

            ProtocolParser.Stack.Dispose();
            ProtocolParser.Stack = new ThreadSafeStack();

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize
                GC.Collect();
                var start = DateTime.Now;
                AddressBook.Serialize(ms, ab);
                var serialize = DateTime.Now - start;
                Console.WriteLine("Speed test thread safe: Serialize " + ab.List.Count + " posts in   " + serialize.TotalSeconds + " s");
            }

            ProtocolParser.Stack.Dispose();
            ProtocolParser.Stack = new ThreadUnsafeStack();

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize
                GC.Collect();
                var start = DateTime.Now;
                AddressBook.Serialize(ms, ab);
                var serialize = DateTime.Now - start;
                Console.WriteLine("Speed test not thread safe: Serialize " + ab.List.Count + " posts in   " + serialize.TotalSeconds + " s");

                //Deserialize
                ms.Seek(0, SeekOrigin.Begin);
                GC.Collect();
                start = DateTime.Now;
                var dab = AddressBook.Deserialize(new StreamRead(ms));
                TimeSpan deserialize = DateTime.Now - start;
                Console.WriteLine("Speed test: Deserialize " + dab.List.Count + " posts in " + deserialize.TotalSeconds + " s");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize 
                GC.Collect();
                DateTime start = DateTime.Now;
                ProtoBuf.Serializer.Serialize(ms, nab);
                TimeSpan serialize = DateTime.Now - start;
                Console.WriteLine("Protobuf-net: Serialize " + nab.List.Count + " posts in   " + serialize.TotalSeconds + " s");

                //Deserialize
                ms.Seek(0, SeekOrigin.Begin);
                GC.Collect();
                start = DateTime.Now;
                var dab = ProtoBuf.Serializer.Deserialize<NetAddressBook>(ms);
                TimeSpan deserialize = DateTime.Now - start;
                Console.WriteLine("Protobuf-net: Deserialize " + dab.List.Count + " posts in " + deserialize.TotalSeconds + " s");
            }
        }
    }
}

