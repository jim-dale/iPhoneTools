using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RFC3394.UnitTests
{
    [TestClass]
    public class KeyWrapAlgorithm_WrapTests
    {
        #region RFC3394 Section 4.1
        [TestMethod]
        public void KeyWrapAlgorithm_Wrap_128key_128kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F";
            string pt = "00112233445566778899AABBCCDDEEFF";
            string ct = "1FA68B0A8112B447 AEF34BD8FB5A7B82 9D3E862371D2CFE5";

            WrapKey(kek, pt, ct);
        }

        [TestMethod]
        public void KeyWrapAlgorithm_Unwrap_128key_128kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F";
            string ct = "1FA68B0A8112B447 AEF34BD8FB5A7B82 9D3E862371D2CFE5";
            string pt = "00112233445566778899AABBCCDDEEFF";

            UnwrapKey(kek, ct, pt);
        }
        #endregion

        #region RFC3394 Section 4.2
        [TestMethod]
        public void KeyWrapAlgorithm_Wrap_128key_192kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F1011121314151617";
            string pt = "00112233445566778899AABBCCDDEEFF";
            string ct = "96778B25AE6CA435 F92B5B97C050AED2 468AB8A17AD84E5D";

            WrapKey(kek, pt, ct);
        }

        [TestMethod]
        public void KeyWrapAlgorithm_Unwrap_128key_192kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F1011121314151617";
            string ct = "96778B25AE6CA435 F92B5B97C050AED2 468AB8A17AD84E5D";
            string pt = "00112233445566778899AABBCCDDEEFF";

            UnwrapKey(kek, ct, pt);
        }
        #endregion

        #region RFC3394 Section 4.3
        [TestMethod]
        public void KeyWrapAlgorithm_Wrap_128key_256kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F";
            string pt = "00112233445566778899AABBCCDDEEFF";
            string ct = "64E8C3F9CE0F5BA2 63E9777905818A2A 93C8191E7D6E8AE7";

            WrapKey(kek, pt, ct);
        }

        [TestMethod]
        public void KeyWrapAlgorithm_Unwrap_128key_256kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F";
            string ct = "64E8C3F9CE0F5BA2 63E9777905818A2A 93C8191E7D6E8AE7";
            string pt = "00112233445566778899AABBCCDDEEFF";

            UnwrapKey(kek, ct, pt);
        }
        #endregion

        #region RFC3394 Section 4.4
        [TestMethod]
        public void KeyWrapAlgorithm_Wrap_192key_192kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F1011121314151617";
            string pt = "00112233445566778899AABBCCDDEEFF0001020304050607";
            string ct = "031D33264E15D332 68F24EC260743EDC E1C6C7DDEE725A93 6BA814915C6762D2";

            WrapKey(kek, pt, ct);
        }

        [TestMethod]
        public void KeyWrapAlgorithm_Unwrap_192key_192kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F1011121314151617";
            string ct = "031D33264E15D332 68F24EC260743EDC E1C6C7DDEE725A93 6BA814915C6762D2";
            string pt = "00112233445566778899AABBCCDDEEFF0001020304050607";

            UnwrapKey(kek, ct, pt);
        }
        #endregion

        #region RFC3394 Section 4.5
        [TestMethod]
        public void KeyWrapAlgorithm_Wrap_192key_256kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F";
            string pt = "00112233445566778899AABBCCDDEEFF0001020304050607";
            string ct = "A8F9BC1612C68B3F F6E6F4FBE30E71E4 769C8B80A32CB895 8CD5D17D6B254DA1";

            WrapKey(kek, pt, ct);
        }

        [TestMethod]
        public void KeyWrapAlgorithm_Unwrap_192key_256kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F";
            string ct = "A8F9BC1612C68B3F F6E6F4FBE30E71E4 769C8B80A32CB895 8CD5D17D6B254DA1";
            string pt = "00112233445566778899AABBCCDDEEFF0001020304050607";

            UnwrapKey(kek, ct, pt);
        }
        #endregion

        #region RFC3394 Section 4.6
        [TestMethod]
        public void KeyWrapAlgorithm_Wrap_256key_256kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F";
            string pt = "00112233445566778899AABBCCDDEEFF000102030405060708090A0B0C0D0E0F";
            string ct = "28C9F404C4B810F4 CBCCB35CFB87F826 3F5786E2D80ED326 CBC7F0E71A99F43B FB988B9B7A02DD21";

            WrapKey(kek, pt, ct);
        }

        [TestMethod]
        public void KeyWrapAlgorithm_Unwrap_256key_256kek()
        {
            string kek = "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F";
            string ct = "28C9F404C4B810F4 CBCCB35CFB87F826 3F5786E2D80ED326 CBC7F0E71A99F43B FB988B9B7A02DD21";
            string pt = "00112233445566778899AABBCCDDEEFF000102030405060708090A0B0C0D0E0F";

            UnwrapKey(kek, ct, pt);
        }
        #endregion

        #region Helper methods
        private void WrapKey(string kek, string pt, string ct)
        {
            pt = pt.Replace(" ", string.Empty);
            ct = ct.Replace(" ", string.Empty);

            var key = Helpers.HexStringToByteArray(kek);
            var input = Helpers.HexStringToByteArray(pt);

            var actual = KeyWrapAlgorithm.WrapKey(key, input);

            Assert.AreEqual(ct, Helpers.ByteArrayToHexString(actual));
        }

        private void UnwrapKey(string kek, string ct, string pt)
        {
            ct = ct.Replace(" ", string.Empty);
            pt = pt.Replace(" ", string.Empty);

            var key = Helpers.HexStringToByteArray(kek);
            var input = Helpers.HexStringToByteArray(ct);

            var actual = KeyWrapAlgorithm.UnwrapKey(key, input);

            Assert.AreEqual(pt, Helpers.ByteArrayToHexString(actual));
        }
        #endregion
    }
}
