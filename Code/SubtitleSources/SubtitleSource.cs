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

        public override IEnumerable<Subtitle> FindSubtitlesForVideo(Video video, List<string> languages)
        {

            var possibleReleaseNames = video.GetPossibleReleaseNames();

            var subtitleCollection = new List<Subtitle>();
            foreach (var releaseName in possibleReleaseNames)
            {
                var foundSubtitles = this.FindSubtitles(video, releaseName);
                if (foundSubtitles.IsNullOrEmpty())
                    continue;

                subtitleCollection.AddRange(foundSubtitles);
            }

            return subtitleCollection;
        }

        #endregion

        #region Private Methods

        private IEnumerable<Subtitle> FindSubtitles(Video video, string releaseName)
        {
            var subtitles = GetSubtitles(video, releaseName);

            return subtitles;
        }

        private IEnumerable<Subtitle> GetSubtitles(Video video, string releaseName)
        {
            var sourceUrl = string.Format(SubSource, releaseName);

            try
            {
                Logger.ReportInfo("Finding subtitles from url: " + sourceUrl);

                var xDoc = XDocument.Parse(Helper.Fetch(sourceUrl).OuterXml);

                return (from subtitle in xDoc.Descendants("sub")
                       select new Subtitle(video.Name, subtitle.Element("language").Value, string.Format(FileSource, subtitle.Element("id").Value))).ToList();
            }
            
            catch (Exception ex)
            {
                Logger.ReportException("Failed loading subtitle from url: " + sourceUrl, ex);

                return null;
            }
        }

        #endregion
    }
}