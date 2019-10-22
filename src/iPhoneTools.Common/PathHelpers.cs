using System.IO;

namespace iPhoneTools
{
    /// <summary>
    /// The file names in an iTunes backup are made by a SHA-1 hash of their name,
    /// together with their path and domain. Between the domain and the path there is a dash.
    /// <code>
    /// SHA1('HomeDomain-Library/SMS/sms.db') = 3d0d7e5fb2ce288813306e4d4636395e047a3d28
    /// </code>
    /// iTunes backups currently come with two folder structures.
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///         Flat i.e. every file in the backup is in a single folder
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Files are stored in subfolders of the main folder. 
    ///         The subfolder name is the first two characters of the SHA-1 hash.
    ///         </description>
    ///     </item>
    /// </list>
    /// This class provides a couple of methods that can be bound at runtime to an Action class instance
    /// so that there is a single decision point when deciding the backup format
    /// </summary>
    public static class PathHelpers
    {
        public static string GetFlatPath(string folder, string fileName)
        {
            return Path.Combine(folder, fileName);
        }

        public static string GetSplitPath(string folder, string fileName)
        {
            string subFolder = fileName.Substring(0, 2);
            return Path.Combine(folder, subFolder, fileName);
        }
    }
}
