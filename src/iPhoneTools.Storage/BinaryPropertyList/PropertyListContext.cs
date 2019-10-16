
namespace iPhoneTools
{
    public class PropertyListContext
    {
        public string MagicNumber { get; set; }
        public string FileFormatVersion { get; set; }
        public int SortVersion { get; set; }
        public int OffsetTableOffsetSize { get; set; }
        public int ObjectRefSize { get; set; }
        public long NumObjects { get; set; }
        public long TopObjectOffset { get; set; }
        public long OffsetTableStart { get; set; }
        public int[] Offsets { get; set; }
    }
}
