using System;
using System.Data.SQLite;

namespace iPhoneTools
{
    public class SqliteRepository : IDisposable
    {
        public SQLiteConnection Connection { get; private set; }

        public SqliteRepository(string connectionString)
        {
            Connection = new SQLiteConnection(connectionString);

            Connection.Open();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Connection.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        public static string GetConnectionString(string path)
        {
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                Version = 3,
                ReadOnly = true,
                FailIfMissing = true,
            };

            return builder.ConnectionString;
        }
    }
}
