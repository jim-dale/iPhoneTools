
namespace iPhoneTools
{
    public struct PropertyContext
    {
        public long Position { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public PropertyType PropertyType { get; set; }
        public object Value { get; set; }

        public PropertyContext(long position, int count, int size, PropertyType type, object value)
        {
            Position = position;
            Count = count;
            Size = size;
            PropertyType = type;
            Value = value;
        }
    }
}
