using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MediaBrowser.Library.Entities;

namespace SubtitleProvider
{

    public class XmlDataSource : IDataSource
    {

        #region Private Members

        private readonly string xmlFilePath;

        #endregion

        #region Constructors

        public XmlDataSource(string xmlPath)
        {
            xmlFilePath = xmlPath;

            this.Initialize();
        }

        #endregion

        #region Implemented Methods

        public Subtitle GetCurrentSubtitle(Video video)
        {
            lock (this)
            {
                var xml = XDocument.Load(xmlFilePath);

                var videoElement = this.GetVideoElement(video, xml);
                var subtitleElement = videoElement.Element("CurrentSubtitle");

                if (subtitleElement == null)
                {
                    return new Subtitle("", "", "");
                }

                var language = (string)subtitleElement.Element("Language");
                var url = (string)subtitleElement.Element("Url");

                var subtitle = new Subtitle(video.Name, language, url);

                return subtitle;
            }
        }

        public void SetCurrentSubtitle(Video video, Subtitle subtitle)
        {
            lock (this)
            {
                var xml = XDocument.Load(xmlFilePath);

                var videoElement = this.GetVideoElement(video, xml);

                var subtitleElement = videoElement.Element("CurrentSubtitle");
                if (subtitleElement != null) subtitleElement.Remove();

                var newSubtitleElement = new XElement("CurrentSubtitle",
                                                   new XElement("Language", subtitle.Langugage),
                                                   new XElement("Url", subtitle.UrlToFile));

                videoElement.Add(newSubtitleElement);

                xml.Save(xmlFilePath);
            }
        }

        public List<Subtitle> GetBlackListedSubtitles(Video video)
        {

            lock (this)
            {
                var xml = XDocument.Load(xmlFilePath);
                var videoElement = this.GetVideoElement(video, xml);

                var blackListedElement = videoElement.Element("BlackListedSubtitles");

                var blackListedSubtitles = (from s in blackListedElement.Elements()
                                            select new Subtitle(video.Name, s.Element("Language").Value, s.Element("Url").Value)).ToList();

                return blackListedSubtitles;
            }
        }

        public void BlackListSubtitle(Video video, Subtitle subtitle)
        {
            lock (this)
            {

                if (subtitle.UrlToFile == "")
                    return;

                var xml = XDocument.Load(xmlFilePath);
                var videoElement = this.GetVideoElement(video, xml);

                var blacklistedElement = videoElement.Element("BlackListedSubtitles");
                var newBlacklistedElement = new XElement("BlackListedSubtitle",
                                                         new XElement("Language", subtitle.Langugage),
                                                         new XElement("Url", subtitle.UrlToFile));

                if (blacklistedElement != null) blacklistedElement.Add(newBlacklistedElement);

                xml.Save(xmlFilePath);
            }
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            lock (this)
            {
                if (File.Exists(xmlFilePath))
                    return;

                var directoryPath = Path.GetDirectoryName(xmlFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var xDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("SubtitleProvider"));

                var writer = new StreamWriter(xmlFilePath);
                writer.WriteLine(xDocument.ToString());
                writer.Flush();
                writer.Close();
            }
        }



        private XElement CreateVideoElement(Video video, XDocument document)
        {
            var newVideoElement = new XElement("Video",
                new XAttribute("name", video.Name),
                 new XElement("CurrentSubtitle",
                                   new XElement("Language", ""),
                                   new XElement("Url", "")),
                 new XElement("BlackListedSubtitles"));

            var element = document.Element("SubtitleProvider");

            if (element != null) element.Add(newVideoElement);

            return newVideoElement;
        }

        private XElement GetVideoElement(Video video, XDocument xml)
        {
            var providerElement = xml.Element("SubtitleProvider");
            if (providerElement != null)
            {
                var videoElement = (from s in providerElement.Elements("Video")
                                    where (string)s.Attribute("name") == video.Name
                                    select s).SingleOrDefault();

                return videoElement ?? this.CreateVideoElement(video, xml);
            }
            return null;
        }

        #endregion

    }
}