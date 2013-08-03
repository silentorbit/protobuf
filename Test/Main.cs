using System;
using System.IO;
using ProtoBuf;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using Personal;
using Yours;
using Mine;
using Local;
using Proto.test;

namespace Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello Binary World!");

            TestReadme.Run();

            TestZigZag.Run();

            TestStandardFeatures.Run();

            TestLocalFeatures.Run();

            TestNonOptimizedCases.Run();

            TestProtoBufNet.Run();

            TestPerformance.Run();
        }
    }
}
