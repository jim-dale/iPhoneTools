#region Copyright Notice
/*
 * RFC3394 Key Wrapping Algorithm
 * Written by Jay Miller
 * 
 * This code is hereby released into the public domain, This applies
 * worldwide.
 */
#endregion

using System;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace RFC3394
{
    /// <summary>
    /// An implementation of the RFC3394 key-wrapping algorithm
    /// </summary>
    public static class KeyWrapAlgorithm
    {
        private static readonly byte[] DefaultIV = { 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6 };

        /// <summary>
        /// Wrap key data
        /// </summary>
        /// <param name="kek">The key encryption key. This must be a valid AES key</param>
        /// <param name="plaintext">The key to be wrapped, two or more 8-byte blocks</param>
        /// <returns>The encrypted, wrapped key</returns>
        /// <exception cref="ArgumentNullException"><c>kek</c> or <c>plaintext</c> is <b>null</b></exception>
        /// <exception cref="ArgumentOutOfRangeException">Either <c>kek</c> is an invalid AES key, or <c>plaintext</c> contains fewer than 16 bytes</exception>
        /// <exception cref="ArgumentException"><c>plaintext</c> is not made up of 64-bit blocks</exception>
        public static byte[] WrapKey(byte[] kek, byte[] plaintext)
        {
            ValidateKEK(kek);
            ValidateInput(plaintext, nameof(plaintext));

            // 1) Initialize variables

            Block A = new Block(DefaultIV);
            Block[] R = Block.BytesToBlocks(plaintext);
            long n = R.Length;

            // 2) Calculate intermediate values

            for (long j = 0; j < 6; j++)
            {
                for (long i = 0; i < n; i++)
                {
                    long t = n * j + i + 1;  // add 1 because i is zero-based

#if DEBUG_KEYWRAPALGORITHM
                    Debug.WriteLine(t);
                    Debug.WriteLine("In   {0} {1} {2}", A, R[0], R[1]);
#endif

                    Block[] B = Encrypt(kek, A.Concat(R[i]));

                    A = MSB(B);
                    R[i] = LSB(B);

#if DEBUG_KEYWRAPALGORITHM
                    Debug.WriteLine("Enc  {0} {1} {2}", A, R[0], R[1]);
#endif

                    A ^= t;

#if DEBUG_KEYWRAPALGORITHM
                    Debug.WriteLine("XorT {0} {1} {2}", A, R[0], R[1]);
#endif
                }
            }

            // 3) Output the results

            Block[] C = new Block[n + 1];
            C[0] = A;
            for (long i = 1; i <= n; i++)
            {
                C[i] = R[i - 1];
            }

            return Block.BlocksToBytes(C);
        }

        /// <summary>
        /// Unwrap encrypted key data
        /// </summary>
        /// <param name="kek">The key encryption key. This must be a valid AES key</param>
        /// <param name="ciphertext">The wrapped key, two or more 8-byte blocks</param>
        /// <returns>The original key data</returns>
        /// <exception cref="ArgumentNullException"><c>kek</c> or <c>ciphertext</c> is <b>null</b></exception>
        /// <exception cref="ArgumentOutOfRangeException">Either <c>kek</c> is an invalid AES key, or <c>ciphertext</c> contains fewer than 16 bytes</exception>
        /// <exception cref="ArgumentException"><c>ciphertext</c> is not made up of 64-bit blocks</exception>
        /// <exception cref="CryptographicException">The decryption process failed an integrity check</exception>
        public static byte[] UnwrapKey(byte[] kek, byte[] ciphertext)
        {
            ValidateKEK(kek);
            ValidateInput(ciphertext, nameof(ciphertext));

            Block[] C = Block.BytesToBlocks(ciphertext);

            // 1) Initialize variables

            Block A = C[0];
            Block[] R = new Block[C.Length - 1];
            for (int i = 1; i < C.Length; i++)
            {
                R[i - 1] = C[i];
            }
            long n = R.Length;

            // 2) Calculate intermediate values

            for (long j = 5; j >= 0; j--)
            {
                for (long i = n - 1; i >= 0; i--)
                {
                    long t = n * j + i + 1;  // add 1 because i is zero-based

#if DEBUG_KEYWRAPALGORITHM
                    Debug.WriteLine(t);
                    Debug.WriteLine("In   {0} {1} {2}", A, R[0], R[1]);
#endif

                    A ^= t;

#if DEBUG_KEYWRAPALGORITHM
                    Debug.WriteLine("XorT {0} {1} {2}", A, R[0], R[1]);
#endif

                    Block[] B = Decrypt(kek, A.Concat(R[i]));

                    A = MSB(B);
                    R[i] = LSB(B);

#if DEBUG_KEYWRAPALGORITHM
                    Debug.WriteLine("Dec  {0} {1} {2}", A, R[0], R[1]);
#endif
                }
            }

            // 3) Output the results

            if (ArraysAreEqual(DefaultIV, A.Bytes) == false)
                throw new CryptographicException(Constants.IntegrityError);

            return Block.BlocksToBytes(R);
        }

#region Helper methods
        /// <summary>
        /// Validates a key encryption key
        /// </summary>
        /// <param name="kek">The key encryption key (KEK) to validate</param>
        private static void ValidateKEK(byte[] kek)
        {
            if (kek == null)
                throw new ArgumentNullException(nameof(kek));
            if (kek.Length != 16 && kek.Length != 24 && kek.Length != 32)
                throw new ArgumentOutOfRangeException(nameof(kek));
        }

        /// <summary>
        /// Validates the input to the (un)wrapping methods
        /// </summary>
        /// <param name="input">Input to validate</param>
        /// <param name="paramName">Name to use for exception messages</param>
        /// <remarks>n must be at least 2, see §2</remarks>
        private static void ValidateInput(byte[] input, string paramName)
        {
            if (input == null)
                throw new ArgumentNullException(paramName);
            if (input.Length < 16)
                throw new ArgumentOutOfRangeException(paramName);
            if (input.Length % 8 != 0)
                throw new ArgumentException(Constants.DivisibleBy8Error, paramName);
        }

        /// <summary>
        /// Encrypts a block of plaintext with AES
        /// </summary>
        /// <param name="plaintext">Plaintext to encrypt</param>
        /// <returns><see cref="Block"/> containing the ciphertext bytes</returns>
        private static Block[] Encrypt(byte[] kek, byte[] plaintext)
        {
            byte[] result;

            using (var alg = new AesManaged())
            {
                alg.Padding = PaddingMode.None;
                alg.Mode = CipherMode.ECB;
                alg.Key = kek;

                if (plaintext.Length != alg.BlockSize / 8)
                    throw new ArgumentOutOfRangeException(nameof(plaintext));

                using (var ms = new MemoryStream())
                {
                    var encryptor = alg.CreateEncryptor();

                    using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plaintext, 0, plaintext.Length);
                    }

                    result = ms.ToArray();
                }
            }

            return Block.BytesToBlocks(result);
        }

        /// <summary>
        /// Decrypts a block of ciphertext with AES
        /// </summary>
        /// <param name="ciphertext">Ciphertext to decrypt</param>
        /// <returns><see cref="Block"/> containing the plaintext bytes</returns>
        private static Block[] Decrypt(byte[] kek, byte[] ciphertext)
        {
            byte[] result;

            using (var alg = new AesManaged())
            {
                alg.Padding = PaddingMode.None;
                alg.Mode = CipherMode.ECB;
                alg.Key = kek;

                if (ciphertext.Length != alg.BlockSize / 8)
                    throw new ArgumentOutOfRangeException(nameof(ciphertext));

                var decryptor = alg.CreateDecryptor();

                using (var ms = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(ciphertext, 0, ciphertext.Length);
                    }

                    result = ms.ToArray();
                }
            }

            return Block.BytesToBlocks(result);
        }

        /// <summary>
        /// Retrieves the 64 most significant bits of a 128-bit <see cref="Block"/>[]
        /// </summary>
        /// <param name="value">An array of two blocks (128 bits)</param>
        /// <returns>The 64 most significant bits of <paramref name="value"/></returns>
        private static Block MSB(Block[] value)
        {
            Debug.Assert(value.Length == 2);
            return value[0];
        }

        /// <summary>
        /// Retrieves the 64 least significant bits of a 128-bit <see cref="Block"/>[]
        /// </summary>
        /// <param name="value">An array of two blocks (128 bits)</param>
        /// <returns>The 64 most significant bits of <paramref name="value"/></returns>
        private static Block LSB(Block[] value)
        {
            Debug.Assert(value.Length == 2);
            return value[1];
        }

        /// <summary>
        /// Tests whether two arrays have the same contents
        /// </summary>
        /// <param name="array1">The first array</param>
        /// <param name="array2">The second array</param>
        /// <returns><b>true</b> if the two arrays have the same contents, otherwise <b>false</b></returns>
        private static bool ArraysAreEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            if (array1.Length == 0 && array2.Length == 0)
                return true;

            for (int i = 0; i < array1.Length; i++)
                if (array1[i] != array2[i])
                    return false;
            return true;
        }
#endregion
    }
}
