using System;

namespace iPhoneTools
{
    public static class ManifestDbEntryExtensions
    {
        public static void ConsoleWrite(this ManifestDbEntry item)
        {
            Console.WriteLine($"FileID={item.FileID},Domain='{item.Domain}',RelativePath='{item.RelativePath}',Flags=0x{item.Flags:x4}");
        }
    }
}
