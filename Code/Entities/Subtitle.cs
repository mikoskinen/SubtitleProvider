using System;

namespace SubtitleProvider
{
    public class Subtitle : IEquatable<Subtitle>
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
                int result = videoName.GetHashCode();
                result = (result*397) ^ langugage.GetHashCode();
                result = (result*397) ^ urlToFile.GetHashCode();
                return result;
            }
        }
    }
}
