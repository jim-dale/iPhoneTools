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

            command.CommandText = "SELECT "
                            + "ROWID,"
                            + "guid,"
                            + "First,"
                            + "Last,"
                            + "Middle,"
                            + "Organization,"
                            + "Note,"
                            + "Prefix,"
                            + "Suffix,"
                            + "DisplayName"
                            + " FROM ABPerson";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var result = new ContactDbEntry
                    {
                        Id = reader.GetInt32(0),
                        Guid = reader.GetGuidAtStartOfString(1),
                        First = reader.GetValueOrDefault<string>(2),
                        Last = reader.GetValueOrDefault<string>(3),
                        Middle = reader.GetValueOrDefault<string>(4),
                        Organization = reader.GetValueOrDefault<string>(5),
                        Note = reader.GetValueOrDefault<string>(6),
                        Prefix = reader.GetValueOrDefault<string>(7),
                        Suffix = reader.GetValueOrDefault<string>(8),
                        DisplayName = reader.GetValueOrDefault<string>(9),
                    };

                    yield return result;
                }
            }

            Connection.Close();
        }
    }
}
