using System;
using System.Data;

namespace iPhoneTools
{
    public static class IDataRecordExtensions
    {
        public static T GetValueOrDefault<T>(this IDataRecord item, int index)
        {
            T result = default;

            var value = item.GetValue(index);
            if ((value is DBNull) == false)
            {
                result = (T)value;
            }

            return result;
        }

        public static Guid GetGuidAtStartOfString(this IDataRecord item, int index)
        {
            var value = item.GetString(index);

            return Guid.Parse(value.Substring(0, 36));
        }

        public static DateTimeOffset GetDateTimeOffsetFromMacTime(this IDataRecord item, int index)
        {
            DateTimeOffset result = default;

            var value = item.GetValue(index);
            if ((value is DBNull) == false)
            {
                result = CommonHelpers.ConvertFromMacTime((long)value);
            }

            return result;
        }
    }
}
