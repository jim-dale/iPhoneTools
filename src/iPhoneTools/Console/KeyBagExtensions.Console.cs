using System;

namespace iPhoneTools
{
    public static partial class KeyBagExtensions
    {
        public static void ConsoleWrite(this KeyBag item)
        {
            Console.WriteLine($"Version={item.Version}");
            Console.WriteLine($"KeyBagType={item.KeyBagType}");
            Console.WriteLine($"UUID={item.Uuid}");
            Console.WriteLine("HMCK=" + CommonHelpers.ByteArrayToDebugString(item.HMCK));
            Console.WriteLine($"WRAP={item.Wrap}");
            Console.WriteLine("SALT=" + CommonHelpers.ByteArrayToDebugString(item.Salt));
            Console.WriteLine($"ITER={item.Iterations}");
            if (item.DataProtection != null)
            {
                Console.WriteLine($"DPWT={item.DataProtection.Dpwt}");
                Console.WriteLine($"DPIC={item.DataProtection.Dpic}");
                Console.WriteLine($"DPSL={item.DataProtection.Dpsl}");
            }
            foreach (var key in item.WrappedKeys)
            {
                key.ConsoleWrite();
            }
        }
    }
}
