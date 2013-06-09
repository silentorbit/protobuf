using System;
using Yours;
using System.IO;
using Mine;

namespace Test
{
    public class TestStandardFeatures : TestBase
    {
        /// <summary>
        /// This is a simple test to trigger most functionality of the generated code.
        /// </summary>
        public static void Run()
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
    }
}

