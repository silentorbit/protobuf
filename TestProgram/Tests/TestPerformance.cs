using System;
using Personal;
using System.Collections.Generic;
using System.IO;
using SilentOrbit.ProtocolBuffers;
using System.Threading;

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
                p.Email = "Alice" + n + "@silentobit.com";
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

            RunTestSerialize(new AllocationStack(), ab);
            RunTestSerialize(new ThreadSafeStack(), ab);
            RunTestSerialize(new ThreadUnsafeStack(), ab);
            RunTestSerialize(new ConcurrentBagStack(), ab);

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize
                GC.Collect();
                var start = DateTime.Now;
                AddressBook.Serialize(ms, ab);
                var serialize = DateTime.Now - start;
                Console.WriteLine("Speed test ConcurrentBagStack: Serialize " + ab.List.Count + " posts in   " + serialize.TotalSeconds + " s");

                //Deserialize
                ms.Seek(0, SeekOrigin.Begin);
                GC.Collect();
                start = DateTime.Now;
                var dab = AddressBook.Deserialize(new PositionStream(ms));
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

        static void RunTestSerialize(MemoryStreamStack stack, AddressBook ab)
        {
            ProtocolParser.Stack.Dispose();
            ProtocolParser.Stack = stack;

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize
                GC.Collect();
                Thread.Sleep(1000);

                var start = DateTime.Now;
                AddressBook.Serialize(ms, ab);
                TimeSpan serialize = DateTime.Now - start;

                Console.WriteLine("Speed test " + stack.GetType().Name + ": Serialize " + ab.List.Count + " posts in   " + serialize.TotalSeconds + " s");
            }
        }
    }
}

