using System.IO;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class MultiFileSubtitleFinder : ILocalSubtitleFinder
    {
        private readonly Video video;
        private readonly ILogger logger;

        public MultiFileSubtitleFinder(Video video, ILogger logger)
        {
            this.video = video;
            this.logger = logger;
        }

        public bool DoesSubtitleExist()
        {
            var videoFileNames = video.GetVideoFileNames();

            foreach (var fileName in videoFileNames)
            {
                var dirInfo = new DirectoryInfo(Path.GetDirectoryName(fileName));

                var subtitleFiles = dirInfo.GetFiles(dirInfo, SubtitleProvider.SubtitleExtensions, ',');
                if (subtitleFiles.Length == 0)
                    return false;

                foreach (var subtitleFile in subtitleFiles)
                {
                    var subtitleFileName = Path.GetFileNameWithoutExtension(subtitleFile.Name).ToLower();
                    var videoFileName = Path.GetFileNameWithoutExtension(fileName).ToLower();

                    if (subtitleFileName != videoFileName)
                        return false;
                }
            }

            return true;
        }
    }
}