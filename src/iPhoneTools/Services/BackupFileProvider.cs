using System;
using System.IO;

namespace iPhoneTools
{
    public class BackupFileProvider
    {
        private string _input;
        private Func<string, string> _func;

        public BackupFileProvider(string inputPath, int version)
        {
            _input = inputPath;

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

        public string GetPath(string fileName)
        {
            return _func.Invoke(fileName);
        }

        private string PreVersion10Provider(string fileName)
        {
            return Path.Combine(_input, fileName);
        }

        private string Version10Provider(string fileName)
        {
            return Path.Combine(_input, fileName.Substring(0, 2), fileName);
        }
    }
}
