using MediaBrowser.Library.Entities;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class LocalSubtitleFinderFactory
    {
        public ILocalSubtitleFinder CreateLocalSubtitleFinderByVideo(Video video)
        {
            if (video.VideoFileCount() > 1)
                return new MultiFileSubtitleFinder(video);

            return new SingleFileSubtitleFinder(video);

        }
    }
}