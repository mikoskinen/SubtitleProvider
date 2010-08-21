using System;
using System.Collections.Generic;
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

                var extractedFiles = new List<string>();
                foreach (var fileEntry in dir)
                {
                    var fileExtension = Path.GetExtension(fileEntry.FilenameInZip);

                    fileExtension = ChangeFileExtensionIfTextFile(fileExtension);

                    var isSubtitleFile = SubtitleProvider.SubtitleExtensions.IndexOf(fileExtension) >= 0;

                    if (!isSubtitleFile) continue;

                    var destinationFilePath = GetDestinationFilePath(fileExtension);

                    zip.ExtractStoredFile(fileEntry, destinationFilePath);

                    extractedFiles.Add(destinationFilePath);
                }

                zip.Close();
                File.Delete(filePath);

                if (extractedFiles.Count < 1)
                {
                    throw new InvalidSubtitleFileException();
                }

                // In most cases the old subtitle has been replaced. But if the
                // file extension changes, we must delete the older files too.                
                RemoveOldSubtitles(extractedFiles);

            }
            catch (Exception ex)
            {
                Logger.ReportException("Extracting subtitle file failed: " + filePath, ex);

                throw;
            }
        }



        private string ChangeFileExtensionIfTextFile(string fileExtension)
        {
            var isTextFile = fileExtension == ".txt";
            if (isTextFile)
                fileExtension = ".srt";
            return fileExtension;
        }

        protected virtual string GetDestinationFilePath(string fileExtension)
        {
            var videoFileNameWithoutExtension = Path.GetFileNameWithoutExtension(video.GetVideoFileName());

            return Path.Combine(video.GetMediaFolder(),
                                videoFileNameWithoutExtension + fileExtension);
        }

        private void RemoveOldSubtitles(IEnumerable<string> extractedFiles)
        {
            foreach (var filePath in extractedFiles)
            {
                var lookForExtension = ".sub";
                
                var currentFileExtension = Path.GetExtension(filePath);
                if (currentFileExtension == ".sub")
                    lookForExtension = ".srt";

                var lookForFile = Path.ChangeExtension(filePath, lookForExtension);

                if (File.Exists(lookForFile))
                    File.Delete(lookForFile);
            }
        }

    }
}
