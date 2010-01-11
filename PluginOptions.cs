using MediaBrowser.Library.Plugins;
using MediaBrowser.Library.Plugins.Configuration;

namespace SubtitleProvider
{
    public class PluginOptions : PluginConfigurationOptions 
    {
        [Label("Languages:")] public string Languages;
    }
}
