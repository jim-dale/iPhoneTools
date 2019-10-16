
using System.Diagnostics.CodeAnalysis;

namespace iPhoneTools
{
    public class WrappedKey
    {
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Expediency")]
        public byte[] Unknown { get; set; }

        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Expediency")]
        public byte[] Key { get; set; }
    }
}
