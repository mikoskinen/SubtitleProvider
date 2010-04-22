using System;
using System.Collections.Generic;
using MediaBrowser.Library.Entities;

namespace SubtitleProvider
{
    public abstract class SubtitleSourceBase
    {
        public IEnumerable<Subtitle> FindSubtitlesForVideo(Video video, List<string> languages)
        {
            try
            {
                return this.Find(video, languages);
            }
            catch (Exception)
            {
                return new List<Subtitle>();
            }
        }

        protected abstract IEnumerable<Subtitle> Find(Video video, List<string> languages);
    }
}

