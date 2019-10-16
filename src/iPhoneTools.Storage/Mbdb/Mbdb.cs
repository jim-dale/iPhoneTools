
namespace iPhoneTools
{
    public class Mbdb
    {
        public string MagicNumber { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public MbdbEntry[] Items { get; set; }
    }
}
