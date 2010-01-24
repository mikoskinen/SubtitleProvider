using System;
using System.Collections.Generic;
using MediaBrowser.Library;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Plugins;
using SubtitleProvider.Views;

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
            get { return "This plugin finds and downloads subtitles automatically. In configuration, separate the languages with comma. For example: Finnish,English,Swedish"; }
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


            var isMC = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
            if (isMC) //only do this inside of MediaCenter as menus can only be created inside MediaCenter
                SubtitleMenuManager.CreateMenu(kernel);
        }

        #endregion

        //public override void Configure()
        //{
        //    var configView = new ConfigureView();

        //    configView.ShowDialog();

        //}
    }
}
