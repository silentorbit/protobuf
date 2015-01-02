using System;
using Proto.Test;
using System.IO;
using NUnit.Framework;

namespace Test
{
    [TestFixture()]
    public class TestNonOptimizedCases
    {
        /// <summary>
        /// Test serializing a message without any low id fields.
        /// </summary>
        [Test()]
        public void Run()
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
            Assert.AreEqual(l1.FieldX1, l2.FieldX1, "LongMessage FieldX1");
            Assert.AreEqual(l1.FieldX2, l2.FieldX2, "LongMessage FieldX2");
            Assert.AreEqual(l1.FieldX3, l2.FieldX3, "LongMessage FieldX3");
            Assert.AreEqual(l1.FieldX4, l2.FieldX4, "LongMessage FieldX4");
        }
    }
}

