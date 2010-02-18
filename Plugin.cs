using System;
using MediaBrowser.Library;
using MediaBrowser.Library.Plugins;

namespace SubtitleProvider
{
    public class Plugin : BasePlugin
    {
        #region Public Properties

        /// <summary>
        /// Contains the configuration for this plugin
        /// </summary>
        public static PluginConfiguration<PluginOptions> PluginOptions { get; set; }

        /// <summary>
        /// Gets name of the plugin
        /// </summary>
        public override string Name
        {
            get { return "Subtitle Provider"; }
        }

        /// <summary>
        /// Gets description of the plugin
        /// </summary>
        public override string Description
        {
            get
            {
                var description =
                    string.Format(
                        "SubtitleProvider is a plugin that automatically downloads the missing subtitle files for your movies and tv shows.{0}{1}In configuration, separate the languages with comma. For example: Finnish,English,Swedish",
                        Environment.NewLine,Environment.NewLine);

                return description;
            }
        }

        public override bool IsConfigurable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets plugin's configuration
        /// </summary>
        public override IPluginConfiguration PluginConfiguration
        {
            get
            {
                return PluginOptions;
            }
        }


        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        /// <param name="kernel"></param>
        public override void Init(Kernel kernel)
        {

            kernel.MetadataProviderFactories.Add(MetadataProviderFactory.Get<SubtitleProvider>());

            PluginOptions = new PluginConfiguration<PluginOptions>(kernel, this.GetType().Assembly);
            PluginOptions.Load();

            var isMc = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
            if (isMc) //only do this inside of MediaCenter as menus can only be created inside MediaCenter
                SubtitleMenuManager.CreateMenu(kernel);
        }

        #endregion
    }
}
