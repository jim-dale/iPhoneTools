using System.Collections.Generic;

namespace iPhoneTools
{
    public class ManifestRepository : SqliteRepository
    {
        public ManifestRepository(string connectionString)
            : base(connectionString)
        {
        }

        public IEnumerable<ManifestDbEntry> GetAllItems()
        {
            var command = Connection.CreateCommand();

            command.CommandText = "SELECT "
                + "fileId,"
                + "domain,"
                + "relativePath,"
                + "flags,"
                + "file"
                + " FROM Files";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var result = new ManifestDbEntry
                    {
                        FileID = reader.GetString(0),
                        Domain = reader.GetString(1),
                        RelativePath = reader.GetString(2),
                        Flags = reader.GetInt32(3),
                        Properties = (byte[])reader.GetValue(4)
                    };

                    yield return result;
                }
            }

            Connection.Close();
        }
    }
}
