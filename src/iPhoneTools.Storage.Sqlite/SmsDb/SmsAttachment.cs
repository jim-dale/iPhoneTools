using System;

namespace iPhoneTools
{
    public class SmsAttachment
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public int TransferState { get; set; }
        public string TransferName { get; set; }
    }
}
