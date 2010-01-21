using MediaBrowser;
using MediaBrowser.Library;
using MediaBrowser.Library.Entities;
using Microsoft.MediaCenter;
using SubtitleProvider;

public class SubtitleMenuManager
{
    public static void DoesSubTitleExist(Item item)
    {
        if (!(item.BaseItem is Video)) return;

        var video = (Video) item.BaseItem;
        var localSubTitleFinderFactory = new LocalSubtitleFinderFactory();

        var finder = localSubTitleFinderFactory.CreateLocalSubtitleFinderByVideo(video);

        var message = finder.DoesSubtitleExist() ? "Subtitle exists" : "No subtitle available";

        Application.DisplayDialog("", message, DialogButtons.Ok, 2);
    }
}
