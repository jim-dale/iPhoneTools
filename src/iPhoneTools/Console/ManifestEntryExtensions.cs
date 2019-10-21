using System;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public static class ManifestEntryExtensions
    {
        public static void Write(this ILogger logger, ManifestEntry item)
        {
            logger.LogInformation("{Id},{Domain},{RelativePath},{Type}", item.Id, item.Domain, item.RelativePath, item.EntryType);
        }


        public static void ConsoleWrite(this ManifestEntry item)
        {
            Console.WriteLine($"ID={item.Id},Domain='{item.Domain}',RelativePath='{item.RelativePath}',Type={item.EntryType}");
        }
    }
}
