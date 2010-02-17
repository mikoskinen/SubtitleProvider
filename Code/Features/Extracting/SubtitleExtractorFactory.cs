using MediaBrowser.Library.Entities;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class SubtitleExtractorFactory
    {
        public ISubtitleExtractor CreateSubtitleExtractorByVideo(Video video)
        {
            if (video.VideoFileCount() > 1)
                return new MultiFileSubtitleExtractor(video);

            return new SubtitleExtractor(video);
            
        }
    }
}