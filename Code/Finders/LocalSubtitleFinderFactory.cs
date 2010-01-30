using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class LocalSubtitleFinderFactory
    {
        public ILocalSubtitleFinder CreateLocalSubtitleFinderByVideo(Video video, ILogger logger)
        {
            if (video.VideoFileCount() > 1)
                return new MultiFileSubtitleFinder(video, logger);

            return new SingleFileSubtitleFinder(video, logger);

        }
    }
}