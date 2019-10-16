using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RFC3394.UnitTests
{
    [TestClass]
    public class Block_InvalidStuff
    {
        [TestMethod]
        public void Constructor_NullBytes()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = new Block((byte[])null));
        }

        [TestMethod]
        public void Constructor_InvalidSize1()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _ = new Block(new byte[0]));
        }

        [TestMethod]
        public void Constructor_InvalidSize2()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _ = new Block(new byte[7]));
        }

        [TestMethod]
        public void Constructor_NegativeIndex()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _ = new Block(new byte[8], -1));
        }

        [TestMethod]
        public void Constructor_InsufficientBytes()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _ = new Block(new byte[16], 9));
        }

        [TestMethod]
        public void Concat_NullRight()
        {
            var sut = new Block(new byte[8]);

            Assert.ThrowsException<ArgumentNullException>(
                () => _ = sut.Concat(null));
        }

        [TestMethod]
        public void BytesToBlocks_NullBytes()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = Block.BytesToBlocks(null));
        }

        [TestMethod]
        public void BytesToBlocks_InvalidSize1()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _ = Block.BytesToBlocks(new byte[7]));
        }

        [TestMethod]
        public void BytesToBlocks_InvalidSize2()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _ = Block.BytesToBlocks(new byte[9]));
        }

        [TestMethod]
        public void BlocksToBytes_NullBlocks()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = Block.BlocksToBytes(null));
        }

        [TestMethod]
        public void Xor_NullBlock()
        {
            Assert.ThrowsException<ArgumentNullException>(
            () => _ = Block.Xor(null, 0));
        }
    }
}
