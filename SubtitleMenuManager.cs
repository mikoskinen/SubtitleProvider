using System;
using System.Collections.Generic;
using MediaBrowser;
using MediaBrowser.Library;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using Microsoft.MediaCenter;
using SubtitleProvider;

public class SubtitleMenuManager
{

    #region Menu Creation

    public static void CreateMenu(Kernel kernel)
    {

        var subtitleMenuItem = new MenuItem("Subtitle?", "resx://MediaBrowser/MediaBrowser.Resources/IconFloral", SubtitleMenuManager.DoesSubTitleExist, new List<Type>() { typeof(Movie), typeof(Episode) });

        kernel.AddMenuItem(subtitleMenuItem);

    }

    #endregion

    #region Subtitle Menu Item

    public static void DoesSubTitleExist(Item item)
    {

        var video = item.BaseItem as Video;
        if (video == null) return;

        var localSubtitleFinderFactory = new LocalSubtitleFinderFactory();

        var finder = localSubtitleFinderFactory.CreateLocalSubtitleFinderByVideo(video, Logger.LoggerInstance);

        var subtitleExist = finder.DoesSubtitleExist();
        if (subtitleExist)
        {
            HandleSubtitleAvailable();
            return;
        }

        HandleNoSubtitle(video);
    }

    private static void HandleSubtitleAvailable()
    {
        //var blacklistDialogResult = Application.DisplayDialog("Blacklist?", "Subtitle available", DialogButtons.No | DialogButtons.Yes, 4);
        Application.DisplayDialog("", "Subtitle available", DialogButtons.Ok, 2);
    }

    private static void HandleNoSubtitle(Video video)
    {
        var downloadDialogResult = Application.DisplayDialog("Download?", "No subtitle available", DialogButtons.No | DialogButtons.Yes, 4);

        if (downloadDialogResult != DialogResult.Yes) 
            return;
        
        HandleDownloadSubtitle(video);
    }

    private static void HandleDownloadSubtitle(Video video)
    {
        var subtitleProcess = new BackgroundProcessor<Action>(2, action => action(), "Subtitle provider");

        var subtitleController = new SubtitleProviderProcessor(Logger.LoggerInstance, SubtitleProviderProcessor.ProvideRequestSourceEnum.Ui);
        subtitleProcess.Inject(() => subtitleController.ProvideSubtitleForVideo(video));
    }

    #endregion

}