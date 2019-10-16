
namespace iPhoneTools
{
    public class DataProtectionKeyData
    {
        public int Dpwt { get; set; }       // Data Protection Wrap Type?
        public int Dpic { get; set; }       // Data Protection Interation Count?
        public byte[] Dpsl { get; set; }    // Data Protection Salt?
    }
}
