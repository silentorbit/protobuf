using System.IO;
using NUnit.Framework;
using Personal;
using Yours.Nullables;

namespace Test
{
    [TestFixture]
    public class TestNullables
    {

        [Test]
        public void Run()
        {
            MyMessageV2 msgIn1 = MyMessageV2.TestInstance();
            msgIn1.NullableEnum = MyMessageV2.AliasedEnum.Nada;
            msgIn1.NullableInt = 123;
           
            var ms1 = new MemoryStream();
            MyMessageV2.Serialize(ms1, msgIn1);

            var msgOut1 = MyMessageV2.Deserialize(ms1.ToArray());

            Assert.AreEqual(msgIn1.NullableEnum, msgOut1.NullableEnum);
            Assert.AreEqual(msgIn1.NullableInt, msgOut1.NullableInt);
        
            MyMessageV2 msgIn2 = MyMessageV2.TestInstance();

            var ms2 = new MemoryStream();
            MyMessageV2.Serialize(ms2, msgIn2);

            var msgOut2 = MyMessageV2.Deserialize(ms2.ToArray());

            Assert.IsNull(msgOut2.NullableEnum);
            Assert.IsNull(msgOut2.NullableInt);
        }

    }

}