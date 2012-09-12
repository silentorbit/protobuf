using System;
using System.IO;
using Personal;

namespace Test
{
    public class TestReadme : TestBase
    {
        /// <summary>
        /// Example found in the README file
        /// </summary>
        public static void Run()
        {
            MemoryStream stream = new MemoryStream();
            
            Person person = new Person();
            person.Name = "George";
            Person.Serialize(stream, person);
            
            stream.Seek(0, SeekOrigin.Begin);
            
            Person person2 = Person.Deserialize(stream);
            Test("ReadMe Person test", person.Name == person2.Name);
        }
    }
}

