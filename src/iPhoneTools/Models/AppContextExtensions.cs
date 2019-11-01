using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public static partial class AppContextExtensions
    {
        public static AppContext SetBackupMetadataInputPaths(this AppContext result, string inputFolder)
        {
            result.InfoPropertiesFile = Path.Combine(inputFolder, CommonConstants.InfoFileName);
            result.Logger.LogInformation("{InfoFileName} input file path set to '{InfoPropertiesFile}'", CommonConstants.InfoFileName, result.InfoPropertiesFile);

            result.StatusPropertiesFile = Path.Combine(inputFolder, CommonConstants.StatusFileName);
            result.Logger.LogInformation("{StatusFileName} input file path set to '{StatusPropertiesFile}'", CommonConstants.StatusFileName, result.StatusPropertiesFile);

            result.ManifestPropertiesFile = Path.Combine(inputFolder, CommonConstants.ManifestPropertiesFileName);
            result.Logger.LogInformation("{ManifestPropertiesFileName} input file path set to '{ManifestPropertiesFile}'", CommonConstants.ManifestPropertiesFileName, result.ManifestPropertiesFile);

            return result;
        }

        public static AppContext CopyInfoPropertyListFileToOutput(this AppContext result, string outputFolder, bool overwrite)
        {
            var outputFile = Path.Combine(outputFolder, CommonConstants.InfoFileName);

            result.Logger.LogInformation("Copying '{InputFile}' to '{OutputFile}'", result.InfoPropertiesFile, outputFile);
            File.Copy(result.InfoPropertiesFile, outputFile, overwrite);

            result.InfoPropertiesFile = outputFile;

            return result;
        }

        public static AppContext CopyStatusPropertyListFileToOutput(this AppContext result, string outputFolder, bool overwrite)
        {
            var outputFile = Path.Combine(outputFolder, CommonConstants.StatusFileName);

            result.Logger.LogInformation("Copying '{InputFile}' to '{OutputFile}'", result.StatusPropertiesFile, outputFile);
            File.Copy(result.StatusPropertiesFile, outputFile, overwrite);

            result.StatusPropertiesFile = outputFile;

            return result;
        }

        public static AppContext CopyManifestPropertyListFileToOutput(this AppContext result, string outputFolder, bool overwrite)
        {
            var outputFile = Path.Combine(outputFolder, CommonConstants.ManifestPropertiesFileName);

            result.Logger.LogInformation("Copying '{InputFile}' to '{OutputFile}'", result.ManifestPropertiesFile, outputFile);
            File.Copy(result.ManifestPropertiesFile, outputFile, overwrite);

            result.ManifestPropertiesFile = outputFile;

            return result;
        }

        public static AppContext LoadBackupMetadata(this AppContext result)
        {
            result.Logger.LogInformation("Loading metadata from '{InfoPropertiesFile}'", result.InfoPropertiesFile);
            result = result
                .AddInfoPropertyListFile(result.InfoPropertiesFile)
                .AddInfoProperties();

            result.Logger.LogInformation("Loading metadata from '{StatusPropertiesFile}'", result.StatusPropertiesFile);
            result = result
                .AddStatusPropertyListFile(result.StatusPropertiesFile)
                .AddStatusProperties();

            result.Logger.LogInformation("Loading metadata from '{ManifestPropertiesFile}'", result.ManifestPropertiesFile);
            result = result
                .AddManifestPropertyListFile(result.ManifestPropertiesFile)
                .AddManifestProperties();

            return result;
        }

        public static AppContext SetVersionsFromMetadata(this AppContext result)
        {
            result.ITunesVersion = Version.Parse(result.InfoProperties.ITunesVersion);
            result.ProductVersion = Version.Parse(result.InfoProperties.ProductVersion);
            result.Logger.LogInformation("iTunes Version={ITunesVersion}", result.ITunesVersion);
            result.Logger.LogInformation("Product Version={ProductVersion}", result.ProductVersion);

            result.StatusVersion = Version.Parse(result.StatusProperties.Version);
            result.Logger.LogInformation("Status Version={StatusVersion}", result.StatusVersion);

            result.ManifestVersion = Version.Parse(result.ManifestProperties.Version);
            result.Logger.LogInformation("Manifest Version={ManifestVersion}", result.ManifestVersion);

            return result;
        }

        public static AppContext SetClassKeysFromManifestKeyBag(this AppContext result, string password)
        {
            if (result.ManifestProperties.IsEncrypted)
            {
                if (result.ManifestPropertyList.TryGetValue(CommonConstants.BackupKeyBagTag, out object data))
                {
                    if (data != null && data is byte[] keyBagData)
                    {
                        var keyBag = BinaryKeyBagReader.Read(keyBagData);

                        result.KeyStore.UnwrapClassKeysFromKeyBag(keyBag, password);
                    }
                }
            }

            return result;
        }

        public static AppContext SetManifestKey(this AppContext result)
        {
            if (result.ManifestProperties.IsEncrypted)
            {
                if (result.ManifestPropertyList.TryGetValue(CommonConstants.ManifestKeyTag, out object data))
                {
                    if (data != null && data is byte[] wrappedKeyData)
                    {
                        result.KeyStore.SetManifestKey(wrappedKeyData);
                    }
                }
            }

            return result;
        }

        public static AppContext SetManifestEntriesFileInputPath(this AppContext result, string inputFolder)
        {
            if (result.ManifestVersion.Major <= 9)
            {
                result.ManifestEntriesFile = Path.Combine(inputFolder, CommonConstants.ManifestFileName_v9);
                result.Logger.LogInformation("{ManifestFileName} input file path set to '{ManifestEntriesFile}'", CommonConstants.ManifestFileName_v9, result.ManifestEntriesFile);
            }
            else if (result.ManifestVersion.Major == 10)
            {
                result.ManifestEntriesFile = Path.Combine(inputFolder, CommonConstants.ManifestFileName_v10);
                result.Logger.LogInformation("{ManifestFileName} input file path set to '{ManifestEntriesFile}'", CommonConstants.ManifestFileName_v10, result.ManifestEntriesFile);
            }
            else
            {
                result.Logger.LogError("Manifest input file path not set because the manifest version ({ManifestVersion}) is not supported", result.ManifestVersion);
            }

            return result;
        }

        public static AppContext CopyManifestEntriesFileToOutputAsPlainText(this AppContext result, string outputFolder, bool overwrite)
        {
            if (result.ManifestVersion.Major <= 9)
            {
                var inputFile = result.ManifestEntriesFile;
                var outputFile = Path.Combine(outputFolder, CommonConstants.ManifestFileName_v9);

                result.Logger.LogInformation("Copying '{InputFile}' to '{OutputFile}'", inputFile, outputFile);
                File.Copy(result.ManifestEntriesFile, outputFile, overwrite);

                result.ManifestEntriesFile = outputFile;
            }
            else if (result.ManifestVersion.Major == 10)
            {
                var inputFile = result.ManifestEntriesFile;
                var outputFile = Path.Combine(outputFolder, CommonConstants.ManifestFileName_v10);

                if (result.ManifestProperties.IsEncrypted)
                {
                    result.Logger.LogInformation("Decrypting '{InputFile}' to '{OutputFile}'", inputFile, outputFile);
                    result.KeyStore.DecryptManifestFile(inputFile, outputFile, overwrite);
                }
                else
                {
                    result.Logger.LogInformation("Copying '{InputFile}' to '{OutputFile}'", inputFile, outputFile);
                    File.Copy(inputFile, outputFile, overwrite);
                }
                result.ManifestEntriesFile = outputFile;
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

        public static AppContext CopyManifestEntryFilesToOutputAsPlainText(this AppContext result, string inputFolder, string outputFolder, bool overwrite)
        {
            if (result.ManifestVersion.Major == 9 || result.ManifestVersion.Major == 10)
            {
                string inputFile = default;
                string outputSubFolder = outputFolder;
                string outputFile = default;

                foreach (var entry in result.ManifestEntries)
                {
                    if (result.ManifestVersion.Major <= 9)
                    {
                        inputFile = Path.Combine(inputFolder, entry.Id);
                        outputFile = Path.Combine(outputFolder, entry.Id);
                    }
                    else if (result.ManifestVersion.Major == 10)
                    {
                        inputFile = Path.Combine(inputFolder, entry.Id.Substring(0, 2), entry.Id);
                        outputSubFolder = Path.Combine(outputFolder, entry.Id.Substring(0, 2));
                        outputFile = Path.Combine(outputSubFolder, entry.Id);
                    }

                    Directory.CreateDirectory(outputSubFolder);

                    if (result.ManifestProperties.IsEncrypted)
                    {
                        result.Logger.LogInformation("Decrypting '{InputFile}' to '{OutputFile}'", inputFile, outputFile);
                        result.KeyStore.DecryptFile(inputFile, outputFile, entry.WrappedKey, entry.ProtectionClass, overwrite);
                    }
                    else
                    {
                        result.Logger.LogInformation("Copying '{InputFile}' to '{OutputFile}'", inputFile, outputFile);
                        File.Copy(inputFile, outputFile, overwrite);
                    }
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
                    .FromMbdbFile(item.ManifestEntriesFile, item.ManifestProperties.IsEncrypted);
            }
            else if (item.ManifestVersion.Major == 10)
            {
                result = new ManifestEntryProvider()
                    .FromManifestDbFile(item.ManifestEntriesFile, item.ManifestProperties.IsEncrypted);
            }

            return result;
        }
    }
}
