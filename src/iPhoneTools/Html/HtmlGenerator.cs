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

        private string _title;
        private Func<XElement> _getBody;

        public HtmlGenerator(string title, Func<XElement> getBody)
        {
            _title = title;
            _getBody = getBody;
        }

        public void SaveAs(string folder, string fileName)
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

            string path = Path.Combine(folder, fileName);
            Directory.CreateDirectory(folder);

            using (var writer = XmlWriter.Create(path, settings))
            {
                xDocument.WriteTo(writer);
            }
        }

        #region Helper methods
        public static void SaveAttachmentAs(string inputPath, string outputFolder, string fileName)
        {
            outputFolder = Path.Combine(outputFolder, "img");
            var outputPath = Path.Combine(outputFolder, fileName);

            Directory.CreateDirectory(outputFolder);
            File.Copy(inputPath, outputPath, overwrite: true);
        }

        public static void SaveCss(string outputFolder, string fileName)
        {
            var inputPath = Path.Combine("Content", fileName);
            outputFolder = Path.Combine(outputFolder, "css");
            var outputPath = Path.Combine(outputFolder, fileName);

            Directory.CreateDirectory(outputFolder);
            File.Copy(inputPath, outputPath, overwrite: true);
        }
        #endregion
    }
}
