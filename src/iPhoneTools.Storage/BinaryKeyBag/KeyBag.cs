using System;
using System.Diagnostics.CodeAnalysis;

namespace iPhoneTools
{
    public class KeyBag
    {
        public int Version { get; set; }
        public KeyBagType KeyBagType { get; set; }
        public Guid Uuid { get; set; }
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Expediency")]
        public byte[] HMCK { get; set; }
        public int Wrap { get; set; }
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Expediency")]
        public byte[] Salt { get; set; }
        public int Iterations { get; set; }
        public DataProtectionKeyData DataProtection { get; set; }
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Expediency")]
        public KeyBagEntry[] WrappedKeys { get; set; }
    }
}
