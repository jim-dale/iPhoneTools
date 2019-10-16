using System;

namespace iPhoneTools
{
    public static class MbdbEntryExtensions
    {
        public static void ConsoleWrite(this MbdbEntry item)
        {
            Console.WriteLine($"Domain=\"{item.Domain}\"");
            Console.WriteLine($"RelativePath=\"{item.RelativePath}\"");
            Console.WriteLine($"Target=\"{item.Target}\"");

            Console.WriteLine($"Flags={item.Flags}");
            if (item.FileContentsHash != null)
            {
                var s = CommonHelpers.ObjectToString(item.FileContentsHash);
                Console.WriteLine($"FileContentsHash={s}");
            }
            if (item.WrappedKey != null)
            {
                var s = CommonHelpers.ObjectToString(item.WrappedKey.Unknown);
                Console.WriteLine($"EncryptionKey.Unknown={s}");

                s = CommonHelpers.ObjectToString(item.WrappedKey.Key);
                Console.WriteLine($"EncryptionKey.Key={s}");
            }
            if (item.Properties != null)
            {
                var s = CommonHelpers.ObjectToString(item.Properties);
                Console.WriteLine($"Properties={s}");
            }
        }
    }
}
