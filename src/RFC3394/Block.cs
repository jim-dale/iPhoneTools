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

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("RFC3394.UnitTests")]

namespace RFC3394
{
    /// <summary>
    /// A <b>Block</b> contains exactly 64 bits of data.  This class
    /// provides several handy block-level operations.
    /// </summary>
    internal class Block
    {
        private readonly byte[] _data = new byte[8];
        public byte[] Bytes
        {
            get { return _data; }
        }

        public Block(Block value) : this(value.Bytes)
        {
        }

        public Block(byte[] value) : this(value, 0)
        {
        }

        public Block(byte[] value, int index)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (index + 8 > value.Length)
                throw new ArgumentException(Constants.BufferLengthError, nameof(value));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            Array.Copy(value, index, _data, 0, 8);
        }

        public override string ToString()
        {
            var result = BitConverter.ToString(_data);
            return result.Replace("-", string.Empty);
        }

        // Concatenates the current Block with the specified Block
        public byte[] Concat(Block value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var result = new byte[16];

            _data.CopyTo(result, 0);
            value.Bytes.CopyTo(result, 8);

            return result;
        }

        // Converts an array of bytes to an array of Blocks
        public static Block[] BytesToBlocks(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Length % 8 != 0)
                throw new ArgumentException(Constants.DivisibleBy8Error, nameof(value));

            var result = new Block[value.Length / 8];

            for (int i = 0; i < value.Length; i += 8)
            {
                result[i / 8] = new Block(value, i);
            }
            return result;
        }

        // Converts an array of Blocks to an arry of bytes
        public static byte[] BlocksToBytes(Block[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var result = new byte[value.Length * 8];

            for (int i = 0; i < value.Length; i++)
            {
                value[i].Bytes.CopyTo(result, i * 8);
            }
            return result;
        }

        // XOR operator against a 64-bit value
        public static Block operator ^(Block blockA, long blockB)
        {
            return Xor(blockA, blockB);
        }

        // XORs a block with a 64-bit value
        public static Block Xor(Block blockA, long blockB)
        {
            if (blockA == null)
                throw new ArgumentNullException(nameof(blockA));

            var result = new Block(blockA);
            Array.Reverse(result.Bytes);
            long temp = BitConverter.ToInt64(result.Bytes, 0);

            result = new Block(BitConverter.GetBytes(temp ^ blockB));
            Array.Reverse(result.Bytes);

            return result;
        }
    }
}
