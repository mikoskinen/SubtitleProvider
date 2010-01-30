using System.IO;
using MediaBrowser.Library.Configuration;

namespace SubtitleProvider
{
    public class DataSourceFactory
    {
        private static readonly string subtitleXmlPath = Path.Combine(ApplicationPaths.AppPluginPath, "subtitles\\list.xml");

        public static IDataSource CreateDataSource()
        {
            return new XmlDataSource(subtitleXmlPath);
        }
    }
}
