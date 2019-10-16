using System;

namespace iPhoneTools
{
    public static partial class ConsoleHelpers
    {
        public static void WriteFileHeader(string path)
        {
            Console.WriteLine();
            Console.WriteLine("=======================================================================");
            Console.WriteLine("\"" + path + "\"");
        }

        public static void Write(object item)
        {
            Console.Write(CommonHelpers.ObjectToString(item));
        }

        public static void WriteLine(object item)
        {
            Console.WriteLine(CommonHelpers.ObjectToString(item));
        }
    }
}
