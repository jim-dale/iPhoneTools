
namespace iPhoneTools
{
    public class PropertyListContext
    {
        public string MagicNumber { get; internal set; }
        public string FileFormatVersion { get; internal set; }
        public int SortVersion { get; internal set; }
        public int OffsetTableOffsetSize { get; internal set; }
        public int ObjectRefSize { get; internal set; }
        public long NumObjects { get; internal set; }
        public long TopObjectOffset { get; internal set; }
        public long OffsetTableStart { get; internal set; }
        public int[] Offsets { get; internal set; }
        public PropertyContext Root { get; internal set; }
    }
}
