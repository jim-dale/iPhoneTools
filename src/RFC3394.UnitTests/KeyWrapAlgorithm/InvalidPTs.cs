using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RFC3394.Tests
{
    [TestClass]
    public class KeyWrapAlgorithm_InvalidPTs
    {
        private static readonly byte[] ValidKEK = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

        [TestMethod]
        public void WrapKey_NullPT()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = KeyWrapAlgorithm.WrapKey(ValidKEK, null));
        }

        [TestMethod]
        public void WrapKey_EmptyPT()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _ = KeyWrapAlgorithm.WrapKey(ValidKEK, new byte[0]));
        }

        [TestMethod]
        public void WrapKey_ShortPT()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _ = KeyWrapAlgorithm.WrapKey(ValidKEK, new byte[8]));
        }

        [TestMethod]
        public void WrapKey_BadMultiplePT()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _ = KeyWrapAlgorithm.WrapKey(ValidKEK, new byte[23]));
        }
    }
}
