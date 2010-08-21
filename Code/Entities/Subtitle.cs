using System;

namespace SubtitleProvider
{
    public class Subtitle : IEquatable<Subtitle>
    {
        private readonly string videoName;
        private readonly string langugage;
        private readonly string urlToFile;
        private readonly string fromProvider;

        public Subtitle(string videoName, string langugage, string urlToFile)
        {
            this.videoName = videoName;
            this.langugage = langugage;
            this.urlToFile = urlToFile;
        }

        public Subtitle(string videoName, string langugage, string urlToFile, string fromProvider)
        {
            this.videoName = videoName;
            this.langugage = langugage;
            this.urlToFile = urlToFile;
            this.fromProvider = fromProvider;
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

        public string FromProvider
        {
            get { return fromProvider; }
        }

        public bool Equals(Subtitle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.videoName, videoName) && Equals(other.langugage, langugage) && Equals(other.urlToFile, urlToFile);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Subtitle)) return false;
            return Equals((Subtitle) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (videoName != null ? videoName.GetHashCode() : 0);
                result = (result*397) ^ (langugage != null ? langugage.GetHashCode() : 0);
                result = (result*397) ^ (urlToFile != null ? urlToFile.GetHashCode() : 0);
                return result;
            }
        }
    }
}
