using System;

namespace iPhoneTools
{
    public class ContactDbEntry
    {
        public int Id { get; internal set; }
        public Guid Guid { get; internal set; }
        public DateTimeOffset ModificationDate { get; internal set; }
        public string First { get; internal set; }
        public string Last { get; internal set; }
        public string Middle { get; internal set; }
        public string Prefix { get; internal set; }
        public string Suffix { get; internal set; }
        public DateTimeOffset Birthday { get; internal set; }
        public string JobTitle { get; internal set; }
        public string Department { get; internal set; }
        public string Organization { get; internal set; }
        public string WorkPhone { get; internal set; }
        public string MobilePhone { get; internal set; }
        public string HomePhone { get; internal set; }
        public string EMail { get; internal set; }
        public string Street { get; internal set; }
        public string City { get; internal set; }
        public string State { get; internal set; }
        public string ZIP { get; internal set; }
        public string Country { get; internal set; }
    }
}
