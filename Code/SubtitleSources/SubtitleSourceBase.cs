using System.Collections.Generic;
using MediaBrowser.Library.Entities;

namespace SubtitleProvider
{
    public abstract class SubtitleSourceBase
    {
        public abstract Subtitle FindSubtitleForVideo(Video video, List<string> languages);
    }
}

