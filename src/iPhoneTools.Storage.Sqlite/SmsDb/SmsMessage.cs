using System;

namespace iPhoneTools
{
    public class SmsMessage
    {
        public string ChatIdentifier { get; set; }
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Text { get; set; }
        public string Service { get; set; }
        public DateTimeOffset Date { get; set; }
        public bool IsFromMe { get; set; }
        public bool CacheHasAttachments { get; set; }
        public SmsAttachment[] Attachments { get; set; }
    }
}
