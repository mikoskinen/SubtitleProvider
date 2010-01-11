using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using MediaBrowser.LibraryManagement;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class SubtitleSource : SubtitleSourceBase
    {

        #region Constants

        private const string SubSource = @"http://www.subtitlesource.org/api/xmlsearch/{0}/all/0";
        private const string FileSource = @"http://www.subtitlesource.org/download/zip/{0}";

        #endregion

        #region Implementations

        public override Subtitle FindSubtitleForVideo(Video video, List<string> languages)
        {

            var possibleReleaseNames = video.GetPossibleReleaseNames();

            foreach (var releaseName in possibleReleaseNames)
            {
                var subtitle = this.FindSubtitle(video, releaseName, languages);

                if (subtitle != null)
                    return subtitle;
            }

            return null;
        }

        #endregion

        #region Private Methods

        private Subtitle FindSubtitle(Video video, string releaseName, List<string> languages)
        {
            var subtitles = GetSubtitles(releaseName);

            if (subtitles == null)
                return null;

            foreach (var language in languages)
            {
                foreach (var subtitle in subtitles)
                {
                    if (subtitle.Language == language)
                        return new Subtitle(video.Name, subtitle.Language, subtitle.Url);
                }
            }

            return null;
        }

        private IEnumerable<SubtitleItem> GetSubtitles(string releaseName)
        {
            var sourceUrl = string.Format(SubSource, releaseName);

            try
            {
                Logger.ReportInfo("Finding subtitles from url: " + sourceUrl);

                var xDoc = XDocument.Parse(Helper.Fetch(sourceUrl).OuterXml);

                return from subtitle in xDoc.Descendants("sub")
                       select new SubtitleItem()
                       {
                           Language = subtitle.Element("language").Value,
                           Url = string.Format(FileSource, subtitle.Element("id").Value)
                       };
            }
            catch (Exception ex)
            {
                Logger.ReportException("Failed loading subtitle from url: " + sourceUrl, ex);

                return null;
            }
        }

        #endregion

        private class SubtitleItem
        {
            public string Language;
            public string Url;
        }
    }
}