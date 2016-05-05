using System;
using Yours;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    /// <summary>
    /// Test that default values are generated,
    /// both in constructor and in deserialization
    /// </summary>
    [TestClass]
    public class TestDefaults
    {
        [TestMethod]
        public void RunDefaultTest()
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
            Assert.AreEqual(-1, m.FieldA);
            Assert.AreEqual(4.5, m.FieldB);
            Assert.AreEqual(5.4f, m.FieldC);
            Assert.AreEqual(-2, m.FieldD);
            Assert.AreEqual(-3, m.FieldE);
            Assert.AreEqual(4u, m.FieldF);
            Assert.AreEqual(5u, m.FieldG);
            Assert.AreEqual(-6, m.FieldH);
            Assert.AreEqual(-7, m.FieldI);
            Assert.AreEqual(8u, m.FieldJ);
            Assert.AreEqual(9u, m.FieldK);
            Assert.AreEqual(-10, m.FieldL);
            Assert.AreEqual(-11, m.FieldM);
            Assert.AreEqual(false, m.FieldN);
            Assert.AreEqual("test", m.FieldO);
        }
    }
}

