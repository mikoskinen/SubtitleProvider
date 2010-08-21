using System.IO;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class SingleFileSubtitleFinder : ILocalSubtitleFinder
    {
        private readonly Video video;
        private readonly ILogger logger;
        private readonly bool extendedLogging;

        public SingleFileSubtitleFinder(Video video, ILogger logger, bool extendedLogging)
        {
            this.video = video;
            this.logger = logger;
            this.extendedLogging = extendedLogging;
        }

        public bool DoesSubtitleExist()
        {

            var dirInfo = new DirectoryInfo(video.GetMediaFolder());

            var subtitleFiles = dirInfo.GetFiles(dirInfo, SubtitleProvider.SubtitleExtensions, ',');

            if (subtitleFiles.Length == 0)
            {
                if (extendedLogging)
                    logger.ReportInfo("No subtitle files found from directory: " + dirInfo.FullName);

                return false;
            }

            var videoFileName = Path.GetFileNameWithoutExtension(video.GetVideoFileName()).ToLower();

            foreach (var file in subtitleFiles)
            {
                var subtitleFileName = Path.GetFileNameWithoutExtension(file.Name).ToLower();

                if (videoFileName == subtitleFileName)
                {
                    if (extendedLogging)
                    {

                        var foundInfo = string.Format(@"Subtitle file ""{0}"" matches video file ""{1}""", file.Name,
                             video.GetVideoFileName());
                        logger.ReportInfo(foundInfo);
                    }

                    return true;

                }

                if (!extendedLogging) continue;

                var notFoundInfo = string.Format(@"Subtitle file ""{0}"" did not match video file ""{1}""", file.Name,
                                                 video.GetVideoFileName());
                logger.ReportInfo(notFoundInfo);
            }

            return false;
        }
    }
}