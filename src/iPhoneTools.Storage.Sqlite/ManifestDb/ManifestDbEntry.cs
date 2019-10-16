
namespace iPhoneTools
{
    public class ManifestDbEntry
    {
        public string FileID { get; set; }
        public string Domain { get; set; }
        public string RelativePath { get; set; }
        public int Flags { get; set; }
        public byte[] Properties { get; set; }
    }
}
