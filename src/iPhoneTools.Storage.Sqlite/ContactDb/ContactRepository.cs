using System.Collections.Generic;

namespace iPhoneTools
{
    public class ContactRepository : SqliteRepository
    {
        public ContactRepository(string connectionString)
            : base(connectionString)
        {
        }

        public IEnumerable<ContactDbEntry> GetAllItems()
        {
            var command = Connection.CreateCommand();

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            command.CommandText =
    "SELECT "
        + "ROWID"
        + ",guid"
        + ",ModificationDate"
        + ",First"
        + ",Last"
        + ",Middle"
        + ",Prefix"
        + ",Suffix"
        + ",Birthday"
        + ",JobTitle"
        + ",Department"
        + ",Organization"

        + ",(select value from ABMultiValue where property = 3 and record_id = ABPerson.ROWID and label = (select ROWID from ABMultiValueLabel where value = '_$!<Work>!$_')) as WorkPhone"
        + ",(select value from ABMultiValue where property = 3 and record_id = ABPerson.ROWID and label = (select ROWID from ABMultiValueLabel where value = '_$!<Mobile>!$_')) as MobilePhone"
        + ",(select value from ABMultiValue where property = 3 and record_id = ABPerson.ROWID and label = (select ROWID from ABMultiValueLabel where value = '_$!<Home>!$_')) as HomePhone"

        + ",(select value from ABMultiValue where property = 4 and record_id = ABPerson.ROWID and label is null) as Email"

        + ",(select value from ABMultiValueEntry where parent_id in (select ROWID from ABMultiValue where record_id = ABPerson.ROWID) and key = (select ROWID from ABMultiValueEntryKey where lower(value) = 'street')) as Street"
        + ",(select value from ABMultiValueEntry where parent_id in (select ROWID from ABMultiValue where record_id = ABPerson.ROWID) and key = (select ROWID from ABMultiValueEntryKey where lower(value) = 'city')) as City"
        + ",(select value from ABMultiValueEntry where parent_id in (select ROWID from ABMultiValue where record_id = ABPerson.ROWID) and key = (select ROWID from ABMultiValueEntryKey where lower(value) = 'state')) as State"
        + ",(select value from ABMultiValueEntry where parent_id in (select ROWID from ABMultiValue where record_id = ABPerson.ROWID) and key = (select ROWID from ABMultiValueEntryKey where lower(value) = 'zip')) as ZIP"
        + ",(select value from ABMultiValueEntry where parent_id in (select ROWID from ABMultiValue where record_id = ABPerson.ROWID) and key = (select ROWID from ABMultiValueEntryKey where lower(value) = 'country')) as Country"
    + " FROM ABPerson"
    + " ORDER BY ROWID";
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var result = new ContactDbEntry
                    {
                        Id = reader.GetInt32(0),
                        Guid = reader.GetGuidAtStartOfString(1),
                        ModificationDate = reader.GetDateTimeOffsetFromLongMacTime(2),
                        First = reader.GetValueOrDefault<string>(3),
                        Last = reader.GetValueOrDefault<string>(4),
                        Middle = reader.GetValueOrDefault<string>(5),
                        Prefix = reader.GetValueOrDefault<string>(6),
                        Suffix = reader.GetValueOrDefault<string>(7),
                        Birthday = reader.GetDateTimeOffsetFromStringMacTime(8),
                        JobTitle = reader.GetValueOrDefault<string>(9),
                        Department = reader.GetValueOrDefault<string>(10),
                        Organization = reader.GetValueOrDefault<string>(11),
                        WorkPhone = reader.GetValueOrDefault<string>(12),
                        MobilePhone = reader.GetValueOrDefault<string>(13),
                        HomePhone = reader.GetValueOrDefault<string>(14),
                        EMail = reader.GetValueOrDefault<string>(15),
                        Street = reader.GetValueOrDefault<string>(16),
                        City = reader.GetValueOrDefault<string>(17),
                        State = reader.GetValueOrDefault<string>(18),
                        ZIP = reader.GetValueOrDefault<string>(19),
                        Country = reader.GetValueOrDefault<string>(20),
                    };

                    yield return result;
                }
            }

            Connection.Close();
        }
    }
}
