using MediaBrowser.Library;
using MediaBrowser.Library.Plugins;

namespace SubtitleProvider
{
    public class Plugin : BasePlugin
    {

        public static PluginConfiguration<PluginOptions> PluginOptions { get; set; }

        public override void Init(Kernel kernel)
        {

            kernel.MetadataProviderFactories.Add(MetadataProviderFactory.Get<SubtitleProvider>());

            PluginOptions = new PluginConfiguration<PluginOptions>(kernel, this.GetType().Assembly);
            PluginOptions.Load();

        }

        public override string Name
        {
            get { return "Subtitle Provider"; }
        }

        public override string Description
        {
            get { return "This plugin finds and downloads subtitles automatically. In configuration, separate the languages with comma. For example: Finnish,English,Swedish"; }
        }

        public override bool IsConfigurable
        {
            get
            {
                return true;
            }
        }

        public override IPluginConfiguration PluginConfiguration
        {
            get
            {
                return PluginOptions;
            }
        }
    }
}
