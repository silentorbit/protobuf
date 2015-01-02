using System;

namespace Test
{
    public class TestBase
    {
        public static void Test(string message, bool result)
        {
            Console.WriteLine(message + ": " + (result ? "OK" : "Fail!"));
            if (result == false)
                throw new InvalidProgramException("Failed test: " + message);
        }
    }
}

