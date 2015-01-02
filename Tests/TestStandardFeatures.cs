using System;
using Yours;
using System.IO;
using Mine;
using NUnit.Framework;

namespace Test
{
    [TestFixture()]
    public class TestStandardFeatures
    {
        /// <summary>
        /// This is a simple test to trigger most functionality of the generated code.
        /// </summary>
        [Test()]
        public void Run()
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
            Assert.IsTrue(version2before.Equals(version2after), "MyMessageV2 WriteRead");

            //Read by older version
            MemoryStream msread1 = new MemoryStream(ms.ToArray());
            MyMessageV1 version1 = MyMessageV1.Deserialize(msread1);

            Console.WriteLine("Version 1");
            Assert.AreEqual(version2before.FieldA, version1.FieldA);
        }
    }
}

