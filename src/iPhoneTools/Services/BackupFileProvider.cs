using System;
using System.IO;

namespace iPhoneTools
{
    public class BackupFileProvider
    {
        private readonly string _folder;
        private readonly Func<string, string> _func;

        public BackupFileProvider(string folder, int version)
        {
            _folder = folder;

            switch (version)
            {
                default:
                case 9:
                    _func = PreVersion10Provider;
                    break;
                case 10:
                    _func = Version10Provider;
                    break;
            }
        }

        public string GetPath(string domain, string relativePath)
        {
            var hash = CommonHelpers.Sha1HashAsHexString(domain + KnownDomains.DomainSeparator + relativePath);

            return _func.Invoke(hash);
        }

        public string GetPath(string fileName)
        {
            return _func.Invoke(fileName);
        }

        private string PreVersion10Provider(string fileName)
        {
            return Path.Combine(_folder, fileName);
        }

        private string Version10Provider(string fileName)
        {
            return Path.Combine(_folder, fileName.Substring(0, 2), fileName);
        }
    }
}
