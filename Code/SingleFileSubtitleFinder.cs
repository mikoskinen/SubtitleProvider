using System.IO;
using MediaBrowser.Library.Entities;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class SingleFileSubtitleFinder : ILocalSubtitleFinder
    {
        private readonly Video video;

        public SingleFileSubtitleFinder(Video video)
        {
            this.video = video;
        }

        public bool DoesSubtitleExist()
        {

            var dirInfo = new DirectoryInfo(video.GetMediaFolder());

            var subtitleFiles = dirInfo.GetFiles(dirInfo, SubtitleProvider.SubtitleExtensions, ',');

            if (subtitleFiles.Length == 0)
                return false;

            var videoFileName = Path.GetFileNameWithoutExtension(video.GetVideoFileName()).ToLower();

            foreach (var file in subtitleFiles)
            {
                var subtitleFileName = Path.GetFileNameWithoutExtension(file.Name).ToLower();

                if (videoFileName == subtitleFileName)
                    return true;
            }

            return false;
        }
    }
}