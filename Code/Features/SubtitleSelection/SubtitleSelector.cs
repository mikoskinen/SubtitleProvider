using System.Collections.Generic;
using System.Linq;

namespace SubtitleProvider
{
    public class SubtitleSelector
    {
        private BlackListingProvider blackListingProvider;

        public SubtitleSelector(BlackListingProvider blackListingProvider)
        {
            this.blackListingProvider = blackListingProvider;
        }

        public Subtitle SelectBestSubtitle(IEnumerable<Subtitle> subtitleCollection, IEnumerable<string> preferredLanguages)
        {
            if (subtitleCollection == null) return null;

            var subtitlesInOrder = this.OrderSubtitles(subtitleCollection, preferredLanguages);

            if (subtitlesInOrder == null || subtitlesInOrder.Count < 1) return null;

            foreach (var subtitle in subtitlesInOrder)
            {
                if (!blackListingProvider.IsBlacklisted(subtitle))
                    return subtitle;

            }

            return null;

        }

        public List<Subtitle> OrderSubtitles(IEnumerable<Subtitle> subtitleCollection, IEnumerable<string> preferredLanguages)
        {
            if (subtitleCollection == null)
                return null;

            var currentIndexCounter = new Dictionary<string, int>();
            var subs = new List<OrderedSubtitle>();

            foreach (var subtitle in subtitleCollection)
            {
                var key = subtitle.FromProvider + subtitle.Langugage;

                var doesContain = currentIndexCounter.ContainsKey(key);
                if (doesContain)
                {
                    var currentIndex = currentIndexCounter[key];
                    var newIndex = currentIndex + 1;
                    subs.Add(new OrderedSubtitle(newIndex, subtitle));

                    currentIndexCounter[key] = newIndex;
                    continue;
                }

                var orderedSubtitle = new OrderedSubtitle(0, subtitle);
                currentIndexCounter.Add(key, 0);

                subs.Add(orderedSubtitle);
            }

            return (from language in preferredLanguages
                    from ordSubtitle in subs
                    where ordSubtitle.Subtitle.Langugage == language
                    orderby ordSubtitle.Index
                    select ordSubtitle.Subtitle).ToList();

        }

        private class OrderedSubtitle
        {
            public readonly int Index;
            public readonly Subtitle Subtitle;

            public OrderedSubtitle(int index, Subtitle subtitle)
            {
                Index = index;
                Subtitle = subtitle;
            }
        }
    }
}
