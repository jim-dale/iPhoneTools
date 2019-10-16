using System;

namespace iPhoneTools
{
    #region Source table definition
    //CREATE TABLE ABPerson
    //  ROWID INTEGER PRIMARY KEY AUTOINCREMENT,
    //  First TEXT,
    //  Last TEXT,
    //  Middle TEXT,
    //  FirstPhonetic TEXT,
    //  MiddlePhonetic TEXT,
    //  LastPhonetic TEXT,
    //  Organization TEXT,
    //  Department TEXT,
    //  Note TEXT,
    //  Kind INTEGER,
    //  Birthday TEXT,
    //  JobTitle TEXT,
    //  Nickname TEXT,
    //  Prefix TEXT,
    //  Suffix TEXT,
    //  FirstSort TEXT,
    //  LastSort TEXT,
    //  CreationDate INTEGER,
    //  ModificationDate INTEGER,
    //  CompositeNameFallback TEXT,
    //  ExternalIdentifier TEXT,
    //  ExternalModificationTag TEXT,
    //  ExternalUUID TEXT,
    //  StoreID INTEGER,
    //  DisplayName TEXT,
    //  ExternalRepresentation BLOB,
    //  FirstSortSection TEXT,
    //  LastSortSection TEXT,
    //  FirstSortLanguageIndex INTEGER DEFAULT 2147483647,
    //  LastSortLanguageIndex INTEGER DEFAULT 2147483647,
    //  PersonLink INTEGER DEFAULT -1,
    //  ImageURI TEXT,
    //  IsPreferredName INTEGER DEFAULT 1,
    //  guid TEXT NOT NULL DEFAULT(ab_generate_guid()),
    //  PhonemeData TEXT,
    //  AlternateBirthday TEXT,
    //  MapsData TEXT,
    //  FirstPronunciation TEXT,
    //  MiddlePronunciation TEXT,
    //  LastPronunciation TEXT,
    //  OrganizationPhonetic TEXT,
    //  OrganizationPronunciation TEXT,
    //  PreviousFamilyName TEXT,
    //  PreferredLikenessSource TEXT,
    //  PreferredPersonaIdentifier TEXT,
    //  PreferredChannel TEXT
    //)
    #endregion
    public class ContactDbEntry
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Middle { get; set; }
        public string Organization { get; set; }
        public string Note { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string DisplayName { get; set; }
    }
}
