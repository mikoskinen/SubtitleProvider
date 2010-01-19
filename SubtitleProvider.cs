using System;
using System.Collections.Generic;
using System.IO;
using MediaBrowser.Library.Configuration;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library.Persistance;
using MediaBrowser.Library.Providers;
using MediaBrowser.Library.Providers.Attributes;
using MediaBrowser.LibraryManagement;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    [SupportedType(typeof(Video))]
    [RequiresInternet]
    public class SubtitleProvider : BaseMetadataProvider
    {

        #region Constants

        /// <summary>
        /// Known extensions of subtitle files
        /// </summary>
        public static readonly string SubtitleExtensions = "*.srt,*.sub";

        #endregion

        #region Private Properties

        private Video CurrentVideo { get { return (Video)Item; } }

        [Persist]
        DateTime lastFetched = DateTime.MinValue;

        private List<string> languageCollection;

        private List<string> languages
        {
            get
            {
                if (languageCollection != null)
                    return languageCollection;

                return GetLanguages();
            }
        }


        #endregion

        #region Implemented Methods

        public static Object synchronizeVariable = "locking variable";

        /// <summary>
        /// Finds and downloads the correct subtitle
        /// </summary>
        public override void Fetch()
        {

            try
            {
                lock (synchronizeVariable)
                {

                    if (this.DoesSubtitleExist())
                        return;

                    var finder = new RemoteSubtitleFinder(this.CurrentVideo);

                    var subtitle = finder.FindSubtitle(languages);

                    if (subtitle == null)
                    {
                        Logger.ReportInfo("Downloading subtitle failed. No subtitle found: " + this.CurrentVideo.GetVideoFileName());
                        return;
                    }

                    var filePath = Path.Combine(ApplicationPaths.AppCachePath, Path.GetRandomFileName() + ".zip");

                    var subtitleDownloader = new SubtitleDownloader();
                    subtitleDownloader.GetSubtitleToPath(subtitle, filePath);

                    var subtitleExtractorFactory = new SubtitleExtractorFactory();
                    var subtitleExtractor = subtitleExtractorFactory.CreateSubtitleExtractorByVideo(CurrentVideo);

                    subtitleExtractor.ExtractSubtitleFile(filePath);

                    this.lastFetched = DateTime.Now;

                }

            }

            catch (Exception ex)
            {
                var reportedError =
                    string.Format("Error when getting subtitle for video: {0}.", this.CurrentVideo.GetVideoFileName());

                Logger.ReportException(reportedError, ex);
            }
        }

        public override bool NeedsRefresh()
        {

            try
            {

                if ((DateTime.Now - lastFetched).Days < 3)
                    return false;

                Logger.ReportInfo("Checking if subtitle exists for video: " + this.CurrentVideo.GetVideoFileName());

                var subtitleFound = this.DoesSubtitleExist();

                if (subtitleFound)
                {
                    Logger.ReportInfo("Subtitle found for video: " + this.CurrentVideo.GetVideoFileName());
                }
                else
                {
                    Logger.ReportInfo("No subtitle available for: " + this.CurrentVideo.GetVideoFileName());
                }

                return subtitleFound == false;

            }
            catch (Exception ex)
            {
                Logger.ReportException("NeedsRefresh failed in SubtitleProvider", ex);
            }

            return false;

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if a subtitle already exists for the given video
        /// </summary>
        /// <returns></returns>
        private bool DoesSubtitleExist()
        {

            var localSubtitleFinderFactory = new LocalSubtitleFinderFactory();
            var localSubtitleFinder = localSubtitleFinderFactory.CreateLocalSubtitleFinderByVideo(CurrentVideo);

            return localSubtitleFinder.DoesSubtitleExist();

        }


        private List<string> GetLanguages()
        {
            var languageString = Plugin.PluginOptions.Instance.Languages;

            if (languageString == "")
            {
                languageCollection = new List<string>() { "English" };
                return languageCollection;
            }

            var languageArray = languageString.Split(',');

            languageCollection = new List<string>();

            foreach (var lang in languageArray)
            {
                languageCollection.Add(lang);
            }

            return languageCollection;
        }


        #endregion

    }
}
