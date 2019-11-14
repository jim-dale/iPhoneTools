using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public class MessageCommand : ICommand<MessageOptions>
    {
        private readonly AppContext _appContext;
        private readonly ILogger<MessageCommand> _logger;

        private BackupFileProvider _backupFileProvider;

        public MessageCommand(AppContext appContext, ILogger<MessageCommand> logger)
        {
            _appContext = appContext;
            _logger = logger;
        }

        public int Run(MessageOptions opts)
        {
            _logger.LogInformation("Starting {Command}", nameof(MessageCommand));

            var appContext = _appContext
                .SetBackupMetadataInputPaths(opts.InputFolder)
                .LoadBackupMetadata()
                .SetVersionsFromMetadata();

            _backupFileProvider = new BackupFileProvider(opts.InputFolder, appContext.ManifestVersion.Major);

            var input = _backupFileProvider.GetPath(KnownDomains.HomeDomain, KnownFiles.Messages);

            _logger.LogInformation("Opening message database '{Source}'", input);
            using (var repository = new SmsRepository(SqliteRepository.GetConnectionString(input)))
            {
                var items = repository.GetAllItems().ToList();

                if (items.Any())
                {
                    HtmlGenerator.SaveCss(opts.OutputFolder, "bootstrap.min.css");
                    HtmlGenerator.SaveCss(opts.OutputFolder, "bootstrap-theme.min.css");
                    HtmlGenerator.SaveCss(opts.OutputFolder, "site.css");

                    foreach (var item in items)
                    {
                        if (item.CacheHasAttachments)
                        {
                            item.Attachments = repository.GetMessageAttachments(item.Id).ToArray();

                            SaveAttachments(item.Attachments, opts.OutputFolder);
                        }
                    }

                    var generator = new HtmlGenerator("Message Index", () =>
                    {
                        return ProcessMessagesByChatIdentifier(items, opts.OutputFolder);
                    });

                    generator.SaveAs(opts.OutputFolder, "index.html");
                }
            }

            _logger.LogInformation("Completed {Command}", nameof(MessageCommand));
            return 0;
        }


        private XElement ProcessMessagesByChatIdentifier(IEnumerable<SmsMessage> items, string outputFolder)
        {
            var chatIdentifiers = items.Select(i => i.ChatIdentifier).Distinct().OrderBy(id => id);

            var container = new XElement("ul");
            var result = new XElement("div", new XAttribute("id", "menu"), container);

            foreach (var chatIdentifier in chatIdentifiers)
            {
                var fileName = GetFileNameFromChatIdentifier(chatIdentifier);
                fileName = "chat-" + fileName + ".html";

                container.Add(ForEachChatIdentifier(chatIdentifier, fileName));

                var messages = from item in items
                               where item.ChatIdentifier.Equals(chatIdentifier)
                               orderby item.Date
                               select item;

                var generator = new HtmlGenerator(chatIdentifier, () =>
                {
                    return ProcessMessages(messages);
                });

                generator.SaveAs(outputFolder, fileName);
            }

            return result;
        }

        private XElement ForEachChatIdentifier(string chatIdentifier, string fileName)
        {
            return new XElement("li", new XElement("a", new XAttribute("href", fileName), chatIdentifier));
        }

        private XElement ProcessMessages(IEnumerable<SmsMessage> items)
        {
            var container = new XElement("div", new XAttribute("class", "commentArea"));
            var result = new XElement("div", new XAttribute("id", "menu"), container);

            DateTimeOffset date = default;
            foreach (var item in items)
            {
                var diff = (date - item.Date).Duration();
                date = item.Date;

                if (diff.TotalMinutes >= 30.0)
                {
                    container.Add(new XElement("div", new XAttribute("class", "messageInfo"), item.Date.ToLocalTime().ToString("f")));
                }

                container.Add(ForEachMessage(item));
            }

            return result;
        }

        private XElement ForEachMessage(SmsMessage item)
        {
            string side = (item.IsFromMe) ? "bubbleRight" : "bubbleLeft";

            var container = new XElement("span");
            var result = new XElement("div", new XAttribute("class", side), container);

            int attachmentIndex = 0;
            var lastPlaceholder = 0;
            while (lastPlaceholder <= item.Text.Length)
            {
                var placeholder = item.Text.IndexOf(HtmlGenerator.UnicodeObjectReplacementCharacter, lastPlaceholder);
                if (placeholder == -1)
                {
                    container.Add(item.Text.Substring(lastPlaceholder));
                    break;
                }
                var length = placeholder - lastPlaceholder;
                if (length > 0)
                {
                    container.Add(item.Text.Substring(lastPlaceholder, placeholder - lastPlaceholder));
                }
                if (item.Attachments != null && attachmentIndex < item.Attachments.Length)
                {
                    container.Add(ProcessAttachment(item.Attachments[attachmentIndex]));
                }
                attachmentIndex++;
                lastPlaceholder = placeholder + 1;
            }
            if (item.Attachments != null)
            {
                for (; attachmentIndex < item.Attachments.Length; attachmentIndex++)
                {
                    container.Add(ProcessAttachment(item.Attachments[attachmentIndex]));
                }
            }

            return result;
        }

        private string GetFileNameFromChatIdentifier(string value)
        {
            var result = value;

            foreach (char ch in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(ch.ToString(), string.Empty);
            }

            return result;
        }

        private void SaveAttachments(SmsAttachment[] items, string outputFolder)
        {
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.FileName) == false)
                {
                    _logger.LogInformation("Saving attachment '{InputFileName}','{TransferName}',MIME type='{MimeType}'", item.FileName, item.TransferName, item.MimeType);

                    var fileName = item.FileName.RemovePrefix("~/");
                    var inputFile = _backupFileProvider.GetPath(KnownDomains.MediaDomain, fileName);

                    _logger.LogInformation("Saving attachment '{InputFile}' to folder '{OutputFolder}'", inputFile, outputFolder);
                    HtmlGenerator.SaveAttachmentAs(inputFile, outputFolder, item.TransferName);
                }
                else
                {
                    _logger.LogWarning("Missing file name for attachment with Id={Id}", item.Id);
                }
            }
        }

        private XElement ProcessAttachment(SmsAttachment item)
        {
            _logger.LogInformation("Processing attachment '{FileName}',MIME type='{MimeType}'", item.TransferName, item.MimeType);

            XElement result;
            if (item.MimeType.StartsWith("image/"))
            {
                result = HtmlGenerator.CreateImageElement(item.TransferName);
            }
            else if (item.MimeType.StartsWith("video/"))
            {
                result = HtmlGenerator.CreateVideoElement(item.TransferName, item.MimeType);
            }
            else if (item.MimeType.StartsWith("audio/"))
            {
                result = HtmlGenerator.CreateAudioElement(item.TransferName, item.MimeType);
            }
            else
            {
                result = HtmlGenerator.CreateAnchorElement(item.TransferName);
            }

            return result;
        }
    }
}
