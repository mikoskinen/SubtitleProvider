using System.Collections.Generic;
using MediaBrowser.Library.Entities;

namespace SubtitleProvider
{
    public class BlackListingProvider
    {
        #region Private Members

        private readonly Video video;
        private readonly IDataSource dataSource;

        #endregion

        #region Constructors

        public BlackListingProvider(Video video)
            : this(video, DataSourceFactory.CreateDataSource())
        {
        }

        public BlackListingProvider(Video video, IDataSource dataSource)
        {
            this.video = video;
            this.dataSource = dataSource;

        }

        #endregion

        #region Private Properties

        private List<Subtitle> blacklistedSubtitleCollection
        {
            get { return this.dataSource.GetBlackListedSubtitles(video); }
        }

        #endregion

        #region Public Properties

        public int BlacklistedSubtitlesCount
        {
            get
            {
                return blacklistedSubtitleCollection == null ? 0 : blacklistedSubtitleCollection.Count;
            }
        }

        #endregion

        #region Public Methods

        public bool IsBlacklisted(Subtitle subtitle)
        {
            return blacklistedSubtitleCollection.Contains(subtitle);
        }

        public void BlackList(Subtitle subtitle)
        {
            if (IsBlacklisted(subtitle))
                return;

            dataSource.BlackListSubtitle(video, subtitle);
        }

        #endregion
    }

}