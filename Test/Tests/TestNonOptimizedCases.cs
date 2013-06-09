using System;
using Proto.test;
using System.IO;

namespace Test
{
    public class TestNonOptimizedCases : TestBase
    {
        /// <summary>
        /// Test serializing a message without any low id fields.
        /// </summary>
        public static void Run()
        {
            Random r = new Random();
            LongMessage l1 = new LongMessage();
            l1.FieldX1 = r.Next();
            l1.FieldX2 = r.Next();
            l1.FieldX3 = r.Next();
            l1.FieldX4 = r.Next();

            MemoryStream ms1 = new MemoryStream();
            LongMessage.Serialize(ms1, l1);

            //Console.WriteLine(BitConverter.ToString(ms1.ToArray()));

            MemoryStream ms2 = new MemoryStream(ms1.ToArray());
            LongMessage l2 = LongMessage.Deserialize(ms2);
            
            //Test
            Test("LongMessage FieldX1", l1.FieldX1 == l2.FieldX1);
            Test("LongMessage FieldX2", l1.FieldX2 == l2.FieldX2);
            Test("LongMessage FieldX3", l1.FieldX3 == l2.FieldX3);
            Test("LongMessage FieldX4", l1.FieldX4 == l2.FieldX4);
        }
    }
}

