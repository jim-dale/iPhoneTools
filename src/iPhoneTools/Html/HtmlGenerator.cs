using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace iPhoneTools
{
    public class HtmlGenerator
    {
        public const char UnicodeObjectReplacementCharacter = (char)0xFFFC;

        private readonly string _title;
        private readonly Func<XElement> _getBody;

        public HtmlGenerator(string title, Func<XElement> getBody)
        {
            _title = title;
            _getBody = getBody;
        }

        public void SaveAs(string outputFolder, string fileName)
        {
            var xDocument = new XDocument(
                new XDocumentType("html", null, null, null),
                    new XElement("html",
                        new XElement("head",
                            new XElement("meta", new XAttribute("charset", "utf-8")),
                            new XElement("meta", new XAttribute("http-equiv", "X-UA-Compatible"), new XAttribute("content", "IE=edge")),
                            new XElement("meta", new XAttribute("name", "viewport"), new XAttribute("content", "width=device-width, initial-scale=1")),
                            new XElement("link", new XAttribute("href", "css/bootstrap.min.css"), new XAttribute("rel", "stylesheet")),
                            new XElement("link", new XAttribute("href", "css/bootstrap-theme.min.css"), new XAttribute("rel", "stylesheet")),
                            new XElement("link", new XAttribute("href", "css/site.css"), new XAttribute("rel", "stylesheet")),
                            new XElement("title", _title)
                        ),
                        new XElement("body",
                            new XElement("div", new XAttribute("class", "container"), _getBody?.Invoke())
                        )
                    )
                );

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = true,
                Indent = true,
            };

            var outputFile = Path.Combine(outputFolder, fileName);
            Directory.CreateDirectory(outputFolder);

            using (var writer = XmlWriter.Create(outputFile, settings))
            {
                xDocument.WriteTo(writer);
            }
        }

        #region Helper methods
        public static void SaveAttachmentAs(string inputFile, string outputFolder, string fileName)
        {
            var outputSubFolder = Path.Combine(outputFolder, "img");
            var outputFile = Path.Combine(outputSubFolder, fileName);

            Directory.CreateDirectory(outputSubFolder);
            File.Copy(inputFile, outputFile, overwrite: true);
        }

        public static void SaveCss(string outputFolder, string fileName)
        {
            var inputFile = Path.Combine("Content", fileName);
            var outputSubFolder = Path.Combine(outputFolder, "css");
            var outputFile = Path.Combine(outputFolder, fileName);

            Directory.CreateDirectory(outputSubFolder);
            File.Copy(inputFile, outputFile, overwrite: true);
        }

        public static XElement CreateAnchorElement(string fileName)
        {
            var result = new XElement("a", new XAttribute("href", "img/" + fileName), fileName);

            return result;
        }

        public static XElement CreateVideoElement(string fileName, string mimeType)
        {
            var result = new XElement("span",
                new XElement("video", new XAttribute("controls", true),
                    new XElement("source", new XAttribute("src", "img/" + fileName), new XAttribute("type", mimeType))),
                new XElement("br"));

            return result;
        }

        public static XElement CreateAudioElement(string fileName, string mimeType)
        {
            var result = new XElement("span",
                new XElement("audio", new XAttribute("controls", true), new XAttribute("src", "img/" + fileName)),
                new XElement("br"));

            return result;
        }

        public static XElement CreateImageElement(string fileName)
        {
            var result = new XElement("span",
                new XElement("img", new XAttribute("src", "img/" + fileName), new XAttribute("width", "200")),
                new XElement("br"));

            return result;
        }
        #endregion
    }
}
