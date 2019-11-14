
using System.Text;

namespace iPhoneTools
{
    public static class StringExtensions
    {
        public static string RemovePrefix(this string str, params string[] prefixes)
        {
            var result = str;

            if (string.IsNullOrEmpty(result) == false)
            {
                foreach (var prefix in prefixes)
                {
                    if (string.IsNullOrEmpty(prefix) == false && result.StartsWith(prefix))
                    {
                        result = result.Remove(0, prefix.Length);
                        break;
                    }
                }
            }

            return result;
        }

        public static string JoinIgnoreEmptyValues(string separator, params string[] values)
        {
            if (values.Length == 0)
                return string.Empty;

            if (separator is null)
            {
                separator = string.Empty;
            }
            var result = new StringBuilder();
            
            string lastValue = default;
            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    if (string.IsNullOrWhiteSpace(lastValue) == false)
                    {
                        result.Append(separator);
                    }
                    lastValue = value;
                    result.Append(value);
                }
            }

            return result.ToString();
        }
    }
}
