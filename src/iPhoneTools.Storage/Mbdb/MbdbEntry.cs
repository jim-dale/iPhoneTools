using System;
using System.Collections.Generic;

namespace iPhoneTools
{
    public class MbdbEntry
    {
        public string Domain { get; set; }
        public string RelativePath { get; set; }
        public string Target { get; set; }
        public byte[] FileContentsHash { get; set; }
        public WrappedKey WrappedKey { get; set; }
        public int Mode { get; set; }
        public ulong InodeNumber { get; set; }
        public uint UserId { get; set; }
        public uint GroupId { get; set; }
        public DateTimeOffset LastModifiedTime { get; set; }
        public DateTimeOffset LastAccessedTime { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public long FileLength { get; set; }
        public byte Flags { get; set; }
        public IReadOnlyDictionary<string, object> Properties { get; set; }
    }
}
