using System.Collections.Generic;
using MediaBrowser.Library.Entities;

namespace SubtitleProvider
{
    public interface IDataSource
    {
        Subtitle GetCurrentSubtitle(Video video);
        void SetCurrentSubtitle(Video video, Subtitle subtitle);
        List<Subtitle> GetBlackListedSubtitles(Video video);
        void BlackListSubtitle(Video video, Subtitle subtitle);
    }

}