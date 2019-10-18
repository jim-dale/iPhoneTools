using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public class MessageCommand : ICommand<MessageOptions>
    {
        private readonly AppContext _appContext;
        private readonly ILogger<MessageCommand> _logger;

        public MessageCommand(AppContext appContext, ILogger<MessageCommand> logger)
        {
            _appContext = appContext;
            _logger = logger;
        }

        public int Run(MessageOptions opts)
        {
            _logger.LogInformation("Starting {Command}", nameof(MessageCommand));

            var appContext = _appContext
                .SetBackupMetadataInputPaths(opts.Input)
                .LoadBackupMetadata()
                .SetVersionsFromMetadata()
                .SetManifestEntriesFileInputPath(opts.Input)
                .AddManifestEntries();

            var backupFileProvider = new BackupFileProvider(opts.Input, appContext.ManifestVersion.Major);

            var input = backupFileProvider.GetPath("3d0d7e5fb2ce288813306e4d4636395e047a3d28");

            _logger.LogInformation("Opening SMS database '{Source}'", input);
            using (var repository = new SmsRepository(SqliteRepository.GetConnectionString(input)))
            {
                var items = repository.GetAllItems().ToList();

                if (items.Any())
                {
                    HtmlGenerator.SaveCss(opts.Output, "bootstrap.min.css");
                    HtmlGenerator.SaveCss(opts.Output, "bootstrap-theme.min.css");
                    HtmlGenerator.SaveCss(opts.Output, "site.css");

                    foreach (var item in items)
                    {
                        if (item.CacheHasAttachments)
                        {
                            item.Attachments = repository.GetMessageAttachments(item.Id).ToArray();

                            foreach (var attachment in item.Attachments)
                            {
                                string fileName = attachment.FileName;

                                if (string.IsNullOrEmpty(fileName) == false && fileName.StartsWith("~/"))
                                {
                                    string toHash = KnownDomains.MediaDomain + KnownDomains.DomainSeparator + fileName.Remove(0, 2);

                                    var hash = CommonHelpers.Sha1HashAsHexString(toHash);

                                    var inputPath = backupFileProvider.GetPath(hash);

                                    HtmlGenerator.SaveAttachmentAs(inputPath, opts.Output, attachment.TransferName);
                                }
                            }
                        }
                    }

                    var generator = new HtmlGenerator("Message Index", () =>
                    {
                        return ProcessMessagesByChatIdentifier(items, opts.Output);
                    });

                    generator.SaveAs(opts.Output, "index.html");
                }
            }

            _logger.LogInformation("Completed {Command}", nameof(MessageCommand));
            return 0;
        }

        private XElement ProcessMessagesByChatIdentifier(IEnumerable<SmsMessage> items, string outputFolder)
        {
            int i = 1;
            var chatIdentifiers = items.Select(i => i.ChatIdentifier).Distinct().OrderBy(id => id);

            var parent = new XElement("ul");

            foreach (var chatIdentifier in chatIdentifiers)
            {
                var fileName = "Conversation-" + i + ".html";

                parent.Add(ForEachChatIdentifier(chatIdentifier, fileName));

                var messages = from item in items
                               where item.ChatIdentifier.Equals(chatIdentifier)
                               orderby item.Date
                               select item;

                var generator = new HtmlGenerator(chatIdentifier, () =>
                {
                    return ProcessMessagesForChatIdentifier(messages);
                });

                generator.SaveAs(outputFolder, fileName);

                ++i;
            }
            return new XElement("div", new XAttribute("id", "menu"), parent);
        }

        private XElement ForEachChatIdentifier(string chatIdentifier, string fileName)
        {
            return new XElement("li", new XElement("a", new XAttribute("href", fileName), chatIdentifier));
        }

        private XElement ProcessMessagesForChatIdentifier(IEnumerable<SmsMessage> items)
        {
            var chatIdentifiers = items.Select(i => i.ChatIdentifier).Distinct();

            DateTimeOffset date = default;
            var parent = new XElement("div", new XAttribute("class", "commentArea"));

            foreach (var item in items)
            {
                var diff = (date - item.Date).Duration();
                date = item.Date;

                if (diff.TotalMinutes >= 30.0)
                {
                    parent.Add(new XElement("div", new XAttribute("class", "messageInfo"), item.Date.ToLocalTime().ToString("f")));
                }

                parent.Add(ForEachMessage(item));
            }
            return new XElement("div", new XAttribute("id", "menu"), parent);
        }

        private XElement ForEachMessage(SmsMessage item)
        {
            string side = (item.IsFromMe) ? "bubbleRight" : "bubbleLeft";
            int attachmentIndex = 0;

            XElement contents = new XElement("span");

            var lastPlaceholder = 0;
            while (lastPlaceholder <= item.Text.Length)
            {
                var placeholder = item.Text.IndexOf(HtmlGenerator.UnicodeObjectReplacementCharacter, lastPlaceholder);
                if (placeholder == -1)
                {
                    contents.Add(item.Text.Substring(lastPlaceholder));
                    break;
                }
                var length = placeholder - lastPlaceholder;
                if (length > 0)
                {
                    contents.Add(item.Text.Substring(lastPlaceholder, placeholder - lastPlaceholder));
                }
                if (item.Attachments != null && attachmentIndex < item.Attachments.Length)
                {
                    contents.Add(ProcessAttachment(item.Attachments[attachmentIndex]));
                }
                attachmentIndex++;
                lastPlaceholder = placeholder + 1;
            }
            if (item.Attachments != null)
            {
                for (; attachmentIndex < item.Attachments.Length; attachmentIndex++)
                {
                    contents.Add(ProcessAttachment(item.Attachments[attachmentIndex]));
                }
            }
            return new XElement("div", new XAttribute("class", side), contents);
        }

        private XElement ProcessAttachment(SmsAttachment item)
        {
            _logger.LogInformation("Processing attachment {FileName}, MIME type={MimeType}", item.TransferName, item.MimeType);

            XElement result;
            if (item.MimeType.StartsWith("image/"))
            {
                result = CreateImageElement(item.TransferName);
            }
            else if (item.MimeType.StartsWith("video/"))
            {
                result = CreateVideoElement(item.TransferName, item.MimeType);
            }
            else if (item.MimeType.StartsWith("audio/"))
            {
                result = CreateAudioElement(item.TransferName, item.MimeType);
            }
            else
            {
                result = CreateAnchorElement(item.TransferName);
            }

            return result;
        }

        private XElement CreateAnchorElement(string fileName)
        {
            var result = new XElement("a", new XAttribute("href", "img/" + fileName), fileName);
            return result;
        }

        private XElement CreateVideoElement(string fileName, string mimeType)
        {
            var result = new XElement("span",
                new XElement("video", new XAttribute("controls", true),
                    new XElement("source", new XAttribute("src", "img/" + fileName), new XAttribute("type", mimeType))),
                new XElement("br"));

            return result;
        }

        private XElement CreateAudioElement(string fileName, string mimeType)
        {
            var result = new XElement("span",
                new XElement("audio", new XAttribute("controls", true), new XAttribute("src", "img/" + fileName)),
                new XElement("br"));

            return result;
        }

        private XElement CreateImageElement(string fileName)
        {
            var result = new XElement("span",
                new XElement("img", new XAttribute("src", "img/" + fileName), new XAttribute("width", "200")),
                new XElement("br"));
            return result;
        }
    }
}
