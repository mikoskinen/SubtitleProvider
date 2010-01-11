using System;
using System.IO;
using System.IO.Compression;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using SubtitleProvider.ExtensionMethods;
using SubtitleProvider;

namespace SubtitleProvider
{
    public class SubtitleExtractor
    {
        private readonly Video video;

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
                    var videoFileNameWithoutExtension = Path.GetFileNameWithoutExtension(video.GetVideoFileName());

                    var destinationFilePath = Path.Combine(video.GetMediaFolder(), videoFileNameWithoutExtension + fileExtension);
                    zip.ExtractStoredFile(fileEntry, destinationFilePath);
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
    }
}
