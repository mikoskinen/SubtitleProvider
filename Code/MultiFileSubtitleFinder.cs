using System.IO;
using MediaBrowser.Library.Entities;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class MultiFileSubtitleFinder : ILocalSubtitleFinder
    {
        private readonly Video video;

        public MultiFileSubtitleFinder(Video video)
        {
            this.video = video;
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