using System;

namespace iPhoneTools
{
    public static partial class MbdbExtensions
    {
        public static bool IsSupportedFormat(this Mbdb item)
        {
            bool result = false;

            if (string.Equals(MbdbConstants.MagicNumber, item.MagicNumber, StringComparison.OrdinalIgnoreCase)
                && item.MajorVersion == 5 && item.MinorVersion == 0)
            {
                result = true;
            }

            return result;
        }
    }
}
