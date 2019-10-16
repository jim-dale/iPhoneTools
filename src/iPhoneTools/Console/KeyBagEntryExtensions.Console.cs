using System;

namespace iPhoneTools
{
    public static partial class KeyBagEntryExtensions
    {
        public static void ConsoleWrite(this KeyBagEntry item)
        {
            Console.WriteLine($"UUID={item.Uuid}");
            Console.WriteLine($"CLAS={item.ProtectionClass}");
            Console.WriteLine($"WRAP={item.Wrap}");
            Console.WriteLine($"KTYP={item.KeyType}");
            Console.Write($"WPKY={CommonHelpers.ByteArrayToDebugString(item.Wpky)}");
        }
    }
}
