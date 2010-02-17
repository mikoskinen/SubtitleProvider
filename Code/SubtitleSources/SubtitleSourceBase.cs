using System.Collections.Generic;
using MediaBrowser.Library.Entities;

namespace SubtitleProvider
{
    public abstract class SubtitleSourceBase
    {
        public abstract IEnumerable<Subtitle> FindSubtitlesForVideo(Video video, List<string> languages);
    }
}

