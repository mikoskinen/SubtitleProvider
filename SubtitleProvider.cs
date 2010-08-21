using System;
using System.Collections.Generic;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
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
                if (Plugin.PluginOptions.Instance.DisableAutomaticDownloading)
                    return;

                if (this.StartFetching(CurrentVideo) == false)
                    return;

                if (this.DoesSubtitleExist())
                {
                    ClearFetching(CurrentVideo);
                    return;
                }

                var processor = new SubtitleProviderProcessor(Logger.LoggerInstance,
                                                              SubtitleProviderProcessor.ProvideRequestSourceEnum.Automatic);

                processor.ProvideSubtitleForVideo(CurrentVideo);

                ClearFetching(CurrentVideo);

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

            var localSubtitleFinderFactory = new LocalSubtitleFinderFactory(Plugin.PluginOptions.Instance.ExtendedLogging);
            var localSubtitleFinder = localSubtitleFinderFactory.CreateLocalSubtitleFinderByVideo(CurrentVideo, Logger.LoggerInstance);

            return localSubtitleFinder.DoesSubtitleExist();

        }

        /// <summary>
        /// Gets the configured languages in a list-format
        /// </summary>
        /// <returns></returns>
        private List<string> GetLanguages()
        {

            var languageProvider = new LanguageProvider();

            var languageString = Plugin.PluginOptions.Instance.Languages;
            return languageProvider.CreateLanguageCollectionFromString(languageString);

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
