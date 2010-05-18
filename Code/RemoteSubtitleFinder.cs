using System.Collections.Generic;
using MediaBrowser.Library.Entities;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class RemoteSubtitleFinder
    {
        #region Private Members

        private readonly Video video;

        private readonly List<SubtitleSourceBase> sources;

        #endregion

        #region Constructors

        public RemoteSubtitleFinder(Video video)
        {
            this.video = video;

            if (sources == null)
            {
                sources = new List<SubtitleSourceBase>();
            }

            sources.Add(new OpenSubtitlesSubtitleSource());
            //sources.Add(new SubtitleSource());
        }

        #endregion

        #region Public Methods

        public Subtitle FindSubtitle(List<string> preferredLanguages, BlackListingProvider blackListingProvider)
        {

            var subtitleCollection = GetSubtitlesFromAllSources(preferredLanguages);

            if (subtitleCollection.IsNullOrEmpty())
                return null;

            var bestSubtitle = SelectBestSubtitle(subtitleCollection, preferredLanguages, blackListingProvider);

            return bestSubtitle;

        }

        #endregion

        #region Private Methods

        private List<Subtitle> GetSubtitlesFromAllSources(List<string> preferredLanguages)
        {
            var subtitleCollection = new List<Subtitle>();
            foreach (var source in sources)
            {
                var foundSubtitleCollection = source.FindSubtitlesForVideo(video, preferredLanguages);

                if (foundSubtitleCollection.IsNullOrEmpty())
                    continue;

                subtitleCollection.AddRange(foundSubtitleCollection);
            }
            return subtitleCollection;
        }

        private Subtitle SelectBestSubtitle(List<Subtitle> subtitleCollection, List<string> preferredLanguages, BlackListingProvider blackListingProvider)
        {
            if (subtitleCollection == null) return null;

            foreach (var language in preferredLanguages)
            {
                foreach (var subtitle in subtitleCollection)
                {
                    if (blackListingProvider.IsBlacklisted(subtitle))
                        continue;

                    if (subtitle.Langugage == language)
                        return subtitle;
                }
            }

            return null;
        }

        #endregion

    }
}
