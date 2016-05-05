using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yours;
using System.IO;
using Mine;
using Test.Helpers;
using SilentOrbit.ProtocolBuffers;

namespace Test
{
    [TestClass]
    public class SkipKeyTest
    {
        /// <summary>
        /// Skip unknown keys and values
        /// </summary>
        [TestMethod]
        public void SkipKey()
        {
            var version2before = MyMessageV2.TestInstance();

            //Write
            var buffer = MyMessageV2.SerializeToBytes(version2before);

            //Read by older version, lots of values will be skipped
            var noseek = new NonSeekableMemoryStream(buffer);
            var ps = new PositionStream(noseek);
            var version1 = MyMessageV1NoPreserve.Deserialize(ps);

            Assert.AreEqual(version2before.FieldA, version1.FieldA);
        }

        /// <summary>
        /// Will fail because the stream does not support seek
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SkipKeyFail()
        {
            MyMessageV2 version2before = MyMessageV2.TestInstance();

            //Write
            var buffer = MyMessageV2.SerializeToBytes(version2before);

            //Read by older version, lots of values will be skipped
            var noseek = new NonSeekableMemoryStream(buffer);
            
            //This should trigger a NotSupportedException
            var version1 = MyMessageV1NoPreserve.Deserialize(noseek);

            Assert.AreEqual(version2before.FieldA, version1.FieldA);
        }

    }
}
