using System;
using Yours;

namespace Test
{
    /// <summary>
    /// Test that default values are generated,
    /// both in constructor and in deserialization
    /// </summary>
    public class TestDefaults : TestBase
    {
        public static void Run()
        {
            //Test constuctor values
            var m = new MyMessageV2();
            TestDefault(m);

            //Test deserialization
            m.FieldA = 0;
            m.FieldB = 0;
            m.FieldC = 0;
            m.FieldD = 0;
            m.FieldE = 0;
            m.FieldF = 0;
            m.FieldG = 0;
            m.FieldI = 0;
            m.FieldJ = 0;
            m.FieldK = 0;
            m.FieldL = 0;
            m.FieldM = 0;
            m.FieldN = true;
            m.FieldO = null;
            MyMessageV2.Deserialize(new byte[0], m);
            TestDefault(m);
        }

        static void TestDefault(MyMessageV2 m)
        {
            Test("", m.FieldA == -1);
            Test("", m.FieldB == 4.5);
            Test("", m.FieldC == 5.4f);
            Test("", m.FieldD == -2);
            Test("", m.FieldE == -3);
            Test("", m.FieldF == 4);
            Test("", m.FieldG == 5);
            Test("", m.FieldH == -6);
            Test("", m.FieldI == -7);
            Test("", m.FieldJ == 8);
            Test("", m.FieldK == 9);
            Test("", m.FieldL == -10);
            Test("", m.FieldM == -11);
            Test("", m.FieldN == false);
            Test("", m.FieldO == "test");
        }
    }
}

