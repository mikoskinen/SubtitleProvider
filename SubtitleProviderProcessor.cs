using System;
using System.IO;
using MediaBrowser;
using MediaBrowser.Library.Configuration;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using SubtitleProvider;

public class SubtitleProviderProcessor
{
    #region Private Members

    private readonly ILogger logger;
    private readonly ProvideRequestSourceEnum requestSource;

    #endregion

    #region Constructors

    public SubtitleProviderProcessor(ILogger logger)
        : this(logger, ProvideRequestSourceEnum.Undefined)
    {
    }

    public SubtitleProviderProcessor(ILogger logger, ProvideRequestSourceEnum requestSource)
    {
        this.logger = logger;
        this.requestSource = requestSource;
    }

    #endregion

    #region Public Methods

    public void ProvideSubtitleForVideo(Video video)
    {
        try
        {

            var message = string.Format("Finding subtitle for {0}", video.Name);
            InformUser(message);

            var dataSource = DataSourceFactory.CreateDataSource();
            var blackListingProvider = new BlackListingProvider(video, dataSource);

            var languageProvider = new LanguageProvider();
            var languages = languageProvider.CreateLanguageCollectionFromString(Plugin.PluginOptions.Instance.Languages);
            var finder = new RemoteSubtitleFinder(video);
            var subtitle = finder.FindSubtitle(languages, blackListingProvider);

            if (subtitle == null)
            {
                var failureMessage = string.Format("Downloading subtitle failed. No subtitle found for {0}", video.Name);
                InformUser(failureMessage);
                return;
            }

            var filePath = Path.Combine(ApplicationPaths.AppCachePath, Path.GetRandomFileName() + ".zip");

            var subtitleDownloader = new SubtitleDownloader();
            subtitleDownloader.GetSubtitleToPath(subtitle, filePath);

            var subtitleExtractorFactory = new SubtitleExtractorFactory();
            var subtitleExtractor = subtitleExtractorFactory.CreateSubtitleExtractorByVideo(video);

            try
            {
                subtitleExtractor.ExtractSubtitleFile(filePath);
            }
            catch (InvalidSubtitleFileException)
            {
                blackListingProvider.BlackList(subtitle);
                throw new Exception("Invalid subtitle file, blacklisting: " + subtitle.UrlToFile);
            }

            dataSource.SetCurrentSubtitle(video, subtitle);
            var successMessage = string.Format("Subtitle downloaded for {0} - {1}", video.Name, subtitle.Langugage);
            InformUser(successMessage);

        }
        catch (Exception ex)
        {
            var reportedError =
                string.Format("Error when getting subtitle for video: {0}.", video.Name);

            InformUser(reportedError);

            Logger.ReportException(reportedError, ex);
        }

    }

    #endregion

    #region Private Methods

    private void InformUser(string message)
    {
        if (requestSource == ProvideRequestSourceEnum.Ui)
            Application.CurrentInstance.Information.AddInformationString(message);

        logger.ReportInfo(message);
    }

    #endregion

    #region ProvideRequestSourceEnum

    public enum ProvideRequestSourceEnum
    {
        Undefined,
        Ui,
        Automatic
    }

    #endregion
}