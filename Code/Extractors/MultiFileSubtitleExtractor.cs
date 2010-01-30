using System.IO;
using MediaBrowser.Library.Entities;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class MultiFileSubtitleExtractor : SubtitleExtractor
    {
        private int extractedFileCount = 0;
        
        public MultiFileSubtitleExtractor(Video video)
            : base(video)
        {}

        protected override string GetDestinationFilePath(string fileExtension)
        {

            var mediaFolder = this.video.GetMediaFolders()[extractedFileCount];
            var videoFileNameWithoutExtension = Path.GetFileNameWithoutExtension(video.GetVideoFileNames()[extractedFileCount]);

            extractedFileCount += 1;
            return Path.Combine(mediaFolder, videoFileNameWithoutExtension + fileExtension);

        }

    }
}