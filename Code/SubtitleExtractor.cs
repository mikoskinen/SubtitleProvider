using System;
using System.IO;
using System.IO.Compression;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using SubtitleProvider.ExtensionMethods;

namespace SubtitleProvider
{
    public class SubtitleExtractor : ISubtitleExtractor
    {
        protected readonly Video video;

        public SubtitleExtractor(Video video)
        {
            this.video = video;
        }

        public void ExtractSubtitleFile(string filePath)
        {

            try
            {
                Logger.ReportInfo("Extracting subtitle file: " + filePath);

                var zip = ZipStorer.Open(filePath, FileAccess.Read);
                var dir = zip.ReadCentralDir();

                foreach (var fileEntry in dir)
                {
                    var fileExtension = Path.GetExtension(fileEntry.FilenameInZip);

                    var isSubtitleFile = SubtitleProvider.SubtitleExtensions.IndexOf(fileExtension) >= 0;
                    
                    if (isSubtitleFile)
                    {
                        var destinationFilePath = GetDestinationFilePath(fileExtension);

                        zip.ExtractStoredFile(fileEntry, destinationFilePath);
                    }
                }

                zip.Close();

                File.Delete(filePath);

            }
            catch (Exception)
            {
                Logger.ReportError("Extracting subtitle file failed: " + filePath);

                throw;
            }
        }

        protected virtual string GetDestinationFilePath(string fileExtension)
        {
            var videoFileNameWithoutExtension = Path.GetFileNameWithoutExtension(video.GetVideoFileName());

            return Path.Combine(video.GetMediaFolder(),
                                videoFileNameWithoutExtension + fileExtension);
        }
    }
}
