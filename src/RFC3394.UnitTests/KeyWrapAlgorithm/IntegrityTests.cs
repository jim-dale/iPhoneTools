using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RFC3394.UnitTests
{
    [TestClass]
    public class KeyWrapAlgorithm_IntegrityTests
    {
        [TestMethod]
        public void MangledCiphertext()
        {
            string kek = "000102030405060708090A0B0C0D0E0F";
            string pt = "00112233445566778899AABBCCDDEEFF";

            var key = Helpers.HexStringToByteArray(kek);
            var input = Helpers.HexStringToByteArray(pt);

            var wrapped = KeyWrapAlgorithm.WrapKey(key, input);

            wrapped[0] ^= 0x01;  // mangle the ciphertext

            Assert.ThrowsException<CryptographicException>(
                () => _ = KeyWrapAlgorithm.UnwrapKey(key, wrapped));
        }
    }
}
