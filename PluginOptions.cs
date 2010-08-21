using MediaBrowser.Library.Plugins;
using MediaBrowser.Library.Plugins.Configuration;

namespace SubtitleProvider
{
    public class PluginOptions : PluginConfigurationOptions 
    {
        [Label("Languages:")] public string Languages;

        [Label("Disable automatic downloading:")] public bool DisableAutomaticDownloading;

        [Label("Extended logging:")] public bool ExtendedLogging;

    }
}
