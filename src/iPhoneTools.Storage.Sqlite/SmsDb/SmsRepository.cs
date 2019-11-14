using System.Collections.Generic;

namespace iPhoneTools
{
    public class SmsRepository : SqliteRepository
    {
        public SmsRepository(string connectionString)
            : base(connectionString)
        {
        }

        public IEnumerable<SmsMessage> GetAllItems()
        {
            var command = Connection.CreateCommand();

            command.CommandText = "SELECT "
                + "chat.chat_identifier,"
                + "message.ROWID,"
                + "message.guid,"
                + "message.text,"
                + "message.service,"
                + "message.date,"
                + "message.is_from_me,"
                + "message.cache_has_attachments"
                + " FROM chat"
                + " INNER JOIN chat_message_join ON chat.ROWID = chat_message_join.chat_id"
                + " INNER JOIN message ON message.ROWID = chat_message_join.message_id"
                + " ORDER BY chat.chat_identifier,message.date";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var result = new SmsMessage
                    {
                        ChatIdentifier = reader.GetString(0),
                        Id = reader.GetInt32(1),
                        Guid = reader.GetGuidAtStartOfString(2),
                        Text = reader.GetValueOrDefault<string>(3),
                        Service = reader.GetValueOrDefault<string>(4),
                        Date = reader.GetDateTimeOffsetFromLongMacTime(5),
                        IsFromMe = reader.GetBoolean(6),
                        CacheHasAttachments = reader.GetBoolean(7),
                    };

                    yield return result;
                }
            }
        }

        public IEnumerable<SmsAttachment> GetMessageAttachments(int messageId)
        {
            var command = Connection.CreateCommand();

            command.CommandText = "SELECT "
                + "ROWID,"
                + "guid,"
                + "filename,"
                + "mime_type,"
                + "transfer_state,"
                + "transfer_name"
                + " FROM attachment"
                + " inner join message_attachment_join"
                + " ON attachment.ROWID=message_attachment_join.attachment_id"
                + " WHERE message_attachment_join.message_id=@param1";

            command.Parameters.AddWithValue("@param1", messageId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var result = new SmsAttachment
                    {
                        Id = reader.GetInt32(0),
                        Guid = reader.GetGuidAtStartOfString(1),
                        FileName = reader.GetValueOrDefault<string>(2),
                        MimeType = reader.GetValueOrDefault<string>(3),
                        TransferState = reader.GetInt32(4),
                        TransferName = reader.GetValueOrDefault<string>(5),
                    };

                    yield return result;
                }
            }
        }
    }
}
