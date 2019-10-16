using System;

namespace iPhoneTools
{
    public class KeyBagEntry
    {
        public Guid Uuid { get; set; }
        public ProtectionClass ProtectionClass { get; set; }
        public WrapTypes Wrap { get; set; }
        public KeyType KeyType { get; set; }
        public byte[] Wpky { get; set; }
    }
}
