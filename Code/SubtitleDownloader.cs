using System;
using System.Net;
using MediaBrowser.Library.Logging;

namespace SubtitleProvider
{
    public class SubtitleDownloader
    {
        public void GetSubtitleToPath(Subtitle subtitle, string filePath)
        {

            try
            {
                Logger.ReportInfo("Downloading subtitle: " + subtitle.UrlToFile);

                var webClient = new WebClient();
                webClient.DownloadFile(subtitle.UrlToFile, filePath);
            }

            catch (Exception)
            {
                Logger.ReportError("Error when downloading subtitle: " + subtitle.UrlToFile);
                throw;
            }

        }
    }
}