using System;
using System.Collections.Generic;
using System.IO;
using MediaBrowser;
using MediaBrowser.Library.Configuration;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library.Persistance;
using MediaBrowser.Library.Providers;
using MediaBrowser.Library.Providers.Attributes;
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

        #region Concurrency objects

        public static Object synchronizeVariable = "locking variable";
        private static Dictionary<string, Video> fetchingCollection;
    
        #endregion

        #region Implemented Methods

        /// <summary>
        /// Finds and downloads the correct subtitle
        /// </summary>
        public override void Fetch()
        {

            try
            {

                if (this.StartFetching(CurrentVideo) == false)
                    return;

                if (this.DoesSubtitleExist())
                {
                    ClearFetching(CurrentVideo);
                    return;
                }

                var finder = new RemoteSubtitleFinder(this.CurrentVideo);

                var subtitle = finder.FindSubtitle(languages);

                if (subtitle == null)
                {
                    Logger.ReportInfo("Downloading subtitle failed. No subtitle found: " + this.CurrentVideo.GetVideoFileName());
                    ClearFetching(CurrentVideo);
                    return;
                }


                var filePath = Path.Combine(ApplicationPaths.AppCachePath, Path.GetRandomFileName() + ".zip");

                var subtitleDownloader = new SubtitleDownloader();
                subtitleDownloader.GetSubtitleToPath(subtitle, filePath);

                var subtitleExtractorFactory = new SubtitleExtractorFactory();
                var subtitleExtractor = subtitleExtractorFactory.CreateSubtitleExtractorByVideo(CurrentVideo);

                subtitleExtractor.ExtractSubtitleFile(filePath);

                ClearFetching(CurrentVideo);

                //var message = "Subtitle downloaded for " + CurrentVideo.Name;
                //Application.CurrentInstance.Information.AddInformationString(message);

            }

            catch (Exception ex)
            {
                var reportedError =
                    string.Format("Error when getting subtitle for video: {0}.", this.CurrentVideo.GetVideoFileName());

                Logger.ReportException(reportedError, ex);

                ClearFetching(CurrentVideo);

            }

        }



        /// <summary>
        /// Returns true if subtitle is missing for the current video.
        /// </summary>
        /// <returns></returns>
        public override bool NeedsRefresh()
        {


            try
            {

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
            var localSubtitleFinder = localSubtitleFinderFactory.CreateLocalSubtitleFinderByVideo(CurrentVideo, Logger.LoggerInstance);

            return localSubtitleFinder.DoesSubtitleExist();

        }

        /// <summary>
        /// Gets the configured languages in a list-format
        /// </summary>
        /// <returns></returns>
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

        #region Concurrency Methods

        /// <summary>
        /// Gets if fetching should proceed. Tries to lock the current video so that only one thread at the time
        /// tries to find the subtitle for one video.
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private bool StartFetching(Video video)
        {
            lock (synchronizeVariable)
            {

                if (fetchingCollection == null)
                    fetchingCollection = new Dictionary<string, Video>();

                var isFetching = IsAlreadyFetching(CurrentVideo);

                if (isFetching)
                    return false;

                var key = video.Name;

                fetchingCollection.Add(key, video);
            }

            return true;
        }

        /// <summary>
        /// Removes the video from the locked-list.
        /// </summary>
        /// <param name="video"></param>
        private void ClearFetching(Video video)
        {
            lock (synchronizeVariable)
            {
                var key = video.Name;

                if (fetchingCollection != null && fetchingCollection.ContainsKey(key))
                    fetchingCollection.Remove(key);
            }
        }

        /// <summary>
        /// Gets if there's already a thread which is trying to find a subtitle
        /// for current video.
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private bool IsAlreadyFetching(Video video)
        {

            if (fetchingCollection == null)
                return false;

            var key = video.Name;

            if (fetchingCollection.ContainsKey(key))
                return true;

            return false;
        }

        #endregion

    }
}
