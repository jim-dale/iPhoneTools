using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RFC3394.Tests
{
    [TestClass]
    public class KeyWrapAlgorithm_InvalidKEKs
    {
        [TestMethod]
        public void WrapKey_NullKEK()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = KeyWrapAlgorithm.WrapKey(null, new byte[16]));
        }

        [TestMethod]
        public void KeyUnwrap_NullKEK()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = KeyWrapAlgorithm.UnwrapKey(null, new byte[16]));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(8)]
        [DataRow(15)]
        [DataRow(20)]
        [DataRow(33)]
        [DataRow(40)]
        public void WrapKey_InvalidKekLength_ThrowsArgumentOutOfRangeException(int kekLength)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _ = KeyWrapAlgorithm.WrapKey(new byte[kekLength], new byte[16]));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(8)]
        [DataRow(15)]
        [DataRow(20)]
        [DataRow(33)]
        [DataRow(40)]
        public void KeyUnwrap_InvalidKekLength_ThrowsArgumentOutOfRangeException(int kekLength)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _ = KeyWrapAlgorithm.UnwrapKey(new byte[kekLength], new byte[16]));
        }
    }
}
