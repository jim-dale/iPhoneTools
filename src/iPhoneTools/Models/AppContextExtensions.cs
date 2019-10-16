using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public static partial class AppContextExtensions
    {
        public static AppContext SetBackupMetadataInputPaths(this AppContext result, string inputPath)
        {
            result.InfoPropertiesPath = Path.Combine(inputPath, CommonConstants.InfoFileName);
            result.Logger.LogInformation("{InfoFileName} input path set to {InfoPropertiesPath}", CommonConstants.InfoFileName, result.InfoPropertiesPath);

            result.StatusPropertiesPath = Path.Combine(inputPath, CommonConstants.StatusFileName);
            result.Logger.LogInformation("{StatusFileName} input path set to {StatusPropertiesPath}", CommonConstants.StatusFileName, result.StatusPropertiesPath);

            result.ManifestPropertiesPath = Path.Combine(inputPath, CommonConstants.ManifestPropertiesFileName);
            result.Logger.LogInformation("{ManifestPropertiesFileName} input path set to {ManifestPropertiesPath}", CommonConstants.ManifestPropertiesFileName, result.ManifestPropertiesPath);

            return result;
        }

        public static AppContext CopyInfoPropertyListFileToOutput(this AppContext result, string outputPath, bool overwrite)
        {
            var output = Path.Combine(outputPath, CommonConstants.InfoFileName);

            result.Logger.LogInformation("Copying {InputPath} to {OutputPath}", result.InfoPropertiesPath, output);
            File.Copy(result.InfoPropertiesPath, output, overwrite);

            result.InfoPropertiesPath = output;

            return result;
        }

        public static AppContext CopyStatusPropertyListFileToOutput(this AppContext result, string outputPath, bool overwrite)
        {
            var output = Path.Combine(outputPath, CommonConstants.StatusFileName);

            result.Logger.LogInformation("Copying {InputPath} to {OutputPath}", result.StatusPropertiesPath, output);
            File.Copy(result.StatusPropertiesPath, output, overwrite);

            result.StatusPropertiesPath = output;

            return result;
        }

        public static AppContext CopyManifestPropertyListFileToOutput(this AppContext result, string outputPath, bool overwrite)
        {
            var output = Path.Combine(outputPath, CommonConstants.ManifestPropertiesFileName);

            result.Logger.LogInformation("Copying {InputPath} to {OutputPath}", result.ManifestPropertiesPath, output);
            File.Copy(result.ManifestPropertiesPath, output, overwrite);

            result.ManifestPropertiesPath = output;

            return result;
        }

        public static AppContext LoadBackupMetadata(this AppContext result)
        {
            result.Logger.LogInformation("Loading metadata from {InfoPropertiesPath}", result.InfoPropertiesPath);
            result = result
                .AddInfoPropertyListFile(result.InfoPropertiesPath)
                .AddInfoProperties();

            result.Logger.LogInformation("Loading metadata from {StatusPropertiesPath}", result.StatusPropertiesPath);
            result = result
                .AddStatusPropertyListFile(result.StatusPropertiesPath)
                .AddStatusProperties();

            result.Logger.LogInformation("Loading metadata from {ManifestPropertiesPath}", result.ManifestPropertiesPath);
            result = result
                .AddManifestPropertyListFile(result.ManifestPropertiesPath)
                .AddManifestProperties();

            return result;
        }

        public static AppContext SetManifestEntriesFileInputPath(this AppContext result, string inputPath)
        {
            if (result.ManifestVersion.Major == 9)
            {
                result.ManifestEntriesPath = Path.Combine(inputPath, CommonConstants.ManifestFileName_v9);
                result.Logger.LogInformation("{ManifestFileName} input path set to {ManifestEntriesPath}", CommonConstants.ManifestFileName_v9, result.ManifestEntriesPath);
            }
            else if (result.ManifestVersion.Major == 10)
            {
                result.ManifestEntriesPath = Path.Combine(inputPath, CommonConstants.ManifestFileName_v10);
                result.Logger.LogInformation("{ManifestFileName} input path set to {ManifestEntriesPath}", CommonConstants.ManifestFileName_v10, result.ManifestEntriesPath);
            }
            else
            {
                result.Logger.LogError("Manifest input path not set because the manifest version ({ManifestVersion}) is not supported", result.ManifestVersion);
            }

            return result;
        }

        public static AppContext CopyManifestEntriesFileToOutputAsPlainText(this AppContext result, string outputPath, bool overwrite)
        {
            if (result.ManifestVersion.Major == 9)
            {
                var input = result.ManifestEntriesPath;
                var output = Path.Combine(outputPath, CommonConstants.ManifestFileName_v9);

                result.Logger.LogInformation("Copying {InputPath} to {OutputPath}", input, output);
                File.Copy(result.ManifestEntriesPath, output, overwrite);

                result.ManifestEntriesPath = output;
            }
            else if (result.ManifestVersion.Major == 10)
            {
                var input = result.ManifestEntriesPath;
                var output = Path.Combine(outputPath, CommonConstants.ManifestFileName_v10);

                if (result.ManifestProperties.IsEncrypted)
                {
                    var manifestKey = result.UnwrapManifestKey();

                    result.Logger.LogInformation("Decrypting {InputPath} to {OutputPath}", input, output);
                    Encryption.DecryptFile(input, output, manifestKey, overwrite);
                }
                else
                {
                    result.Logger.LogInformation("Copying {InputPath} to {OutputPath}", input, output);
                    File.Copy(input, output, overwrite);
                }
                result.ManifestEntriesPath = output;
            }
            else
            {
                result.Logger.LogError("Manifest file not copied because the manifest version ({ManifestVersion}) is not supported", result.ManifestVersion);
            }

            return result;
        }

        public static AppContext AddManifestEntries(this AppContext result)
        {
            var manifestProvider = result.CreateManifestEntryProvider();

            result.Logger.LogInformation("Retrieving manifest entries");
            result.ManifestEntries = manifestProvider.GetAllFiles().ToList();

            return result;
        }

        public static AppContext CopyManifestEntryFilesToOutputAsPlainText(this AppContext result, string input, string output, bool overwrite)
        {
            if (result.ManifestVersion.Major == 9 || result.ManifestVersion.Major == 10)
            {
                string inputPath = default;
                string outputFolder = default;
                string outputPath = default;

                foreach (var entry in result.ManifestEntries)
                {
                    if (result.ManifestVersion.Major == 9)
                    {
                        inputPath = Path.Combine(input, entry.FileId);
                        outputFolder = output;
                        outputPath = Path.Combine(outputFolder, entry.FileId);
                    }
                    else if (result.ManifestVersion.Major == 10)
                    {
                        inputPath = Path.Combine(input, entry.FileId.Substring(0, 2), entry.FileId);
                        outputFolder = Path.Combine(output, entry.FileId.Substring(0, 2));
                        outputPath = Path.Combine(outputFolder, entry.FileId);
                    }

                    Directory.CreateDirectory(outputFolder);

                    if (result.ManifestProperties.IsEncrypted)
                    {
                        var key = entry.UnwrapEncryptionKey(result.ClassKeys);

                        result.Logger.LogInformation("Decrypting {InputPath} to {OutputPath}", inputPath, outputPath);
                        Encryption.DecryptFile(inputPath, outputPath, key, overwrite);
                    }
                    else
                    {
                        result.Logger.LogInformation("Copying {InputPath} to {OutputPath}", inputPath, outputPath);
                        File.Copy(inputPath, outputPath, overwrite);
                    }
                }
            }

            return result;
        }

        public static AppContext SetVersionsFromMetadata(this AppContext result)
        {
            result.ITunesVersion = Version.Parse(result.InfoProperties.ITunesVersion);
            result.ProductVersion = Version.Parse(result.InfoProperties.ProductVersion);
            result.StatusVersion = Version.Parse(result.StatusProperties.Version);
            result.ManifestVersion = Version.Parse(result.ManifestProperties.Version);

            result.Logger.LogInformation("iTunes Version={ITunesVersion}", result.ITunesVersion);
            result.Logger.LogInformation("Product Version={ProductVersion}", result.ProductVersion);
            result.Logger.LogInformation("Status Version={StatusVersion}", result.StatusVersion);
            result.Logger.LogInformation("Manifest Version={ManifestVersion}", result.ManifestVersion);

            return result;
        }

        public static AppContext UnwrapClassKeysFromManifestKeyBag(this AppContext result, string password)
        {
            if (result.ManifestPropertyList.TryGetValue(CommonConstants.BackupKeyBagTag, out object data))
            {
                if (data != null)
                {
                    var keyBag = BinaryKeyBagReader.Read((byte[])data);

                    if (result.ManifestProperties.IsEncrypted && keyBag != null)
                    {
                        result.Logger.LogInformation("Unwrapping class keys");

                        if (keyBag.DataProtection is null)
                        {
                            result.ClassKeys = keyBag.UnwrapClassKeys_v1(password);
                        }
                        else
                        {
                            result.ClassKeys = keyBag.UnwrapClassKeys_v2(password);
                        }
                    }
                }
            }

            return result;
        }

        public static byte[] UnwrapManifestKey(this AppContext item)
        {
            byte[] result = default;

            if (item.ManifestProperties.IsEncrypted)
            {
                if (item.ManifestPropertyList.TryGetValue("ManifestKey", out object data))
                {
                    if (data != null && data is byte[] keyData)
                    {
                        var wrappedManifestKey = WrappedKeyReader.Read(keyData);
                        if (wrappedManifestKey != null)
                        {
                            result = wrappedManifestKey.UnwrapKey(item.ClassKeys);
                        }
                    }
                }
            }

            return result;
        }

        private static AppContext AddFromFile(AppContext result, string path, bool optional, Action<AppContext> action)
        {
            if (File.Exists(path))
            {
                action(result);
            }
            else
            {
                if (optional == false)
                {
                    throw new FileNotFoundException();
                }
            }

            return result;
        }

        private static ManifestEntryProvider CreateManifestEntryProvider(this AppContext item)
        {
            ManifestEntryProvider result = default;

            if (item.ManifestVersion.Major == 9)
            {
                result = new ManifestEntryProvider()
                    .FromMbdbFile(item.ManifestEntriesPath, item.ManifestProperties.IsEncrypted);
            }
            else if (item.ManifestVersion.Major == 10)
            {
                result = new ManifestEntryProvider()
                    .FromManifestDbFile(item.ManifestEntriesPath, item.ManifestProperties.IsEncrypted);
            }

            return result;
        }
    }
}
