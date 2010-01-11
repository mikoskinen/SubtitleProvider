namespace SubtitleProvider
{
    public class Subtitle
    {
        private readonly string videoName;
        private readonly string langugage;
        private readonly string urlToFile;

        public Subtitle(string videoName, string langugage, string urlToFile)
        {
            this.videoName = videoName;
            this.langugage = langugage;
            this.urlToFile = urlToFile;
        }

        public string UrlToFile
        {
            get { return urlToFile; }
        }

        public string Langugage
        {
            get { return langugage; }
        }

        public string VideoName
        {
            get { return videoName; }
        }
    }
}
