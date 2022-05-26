using System;
using Personal;
using System.Collections.Generic;
using System.IO;
using SilentOrbit.ProtocolBuffers;
using System.Threading;
using System.Diagnostics;

namespace Test
{
    public class TestPerformance
    {
        static void Generate(ref AddressBook ab, ref NetAddressBook nab)
        {
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

        }

        public static void Run()
        {
            AddressBook ab = new AddressBook();
            NetAddressBook nab = new NetAddressBook();
            Console.Write("Generating data structures...");
            Generate(ref ab, ref nab);
            Console.WriteLine("done");
            Console.WriteLine("");

            Console.WriteLine("Starting speed tests of "+ ab.List.Count + " posts...");

            Stopwatch sw;

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize
                Console.Write("SilentOrbit.ProtocolBuffers:     Serialize   in ");
                GCWait();
                sw = Stopwatch.StartNew();
                
                AddressBook.Serialize(ms, ab);
                
                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalSeconds + " s");

                Console.Write("SilentOrbit.ProtocolBufferstest: Deserialize in ");
                //Deserialize
                ms.Seek(0, SeekOrigin.Begin);
                GCWait();
                sw = Stopwatch.StartNew();
                
                AddressBook.Deserialize(new PositionStream(ms));

                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalSeconds + " s");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                //Serialize 
                Console.Write("Protobuf-net:                    Serialize   in ");
                GCWait();
                sw = Stopwatch.StartNew();
                
                ProtoBuf.Serializer.Serialize(ms, nab);
                
                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalSeconds + " s");

                //Deserialize
                Console.Write("Protobuf-net:                    Deserialize in ");
                ms.Seek(0, SeekOrigin.Begin);
                GCWait();
                sw = Stopwatch.StartNew();

                ProtoBuf.Serializer.Deserialize<NetAddressBook>(ms);

                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalSeconds + " s");
            }
        }

        static void GCWait()
        {
            GC.Collect();
            Thread.Sleep(500);
        }

    }
}

