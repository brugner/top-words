using System.Collections.Generic;
using System.Linq;
using TopWords.Services.Interfaces;

namespace TopWords.Services
{
    public class WordFrequencyService : IWordFrequencyService
    {
        public IDictionary<string, int> GetWordsFrequencies(IEnumerable<string> artistLyrics)
        {
            return artistLyrics
                 .SelectMany(x => x.Split())
                 .Where(x => IsValidWord(x))
                 .GroupBy(x => x)
                 .ToDictionary(x => x.Key, x => x.Count())
                 .OrderByDescending(x => x.Value)
                 .Take(10)
                 .ToDictionary(x => x.Key, x => x.Value);
        }

        private static bool IsValidWord(string word)
        {
            // TODO: add more words to the list
            var bannedWords = new List<string>
            {
                "the", "and", "to", "a", "of", "in", "my", "it", "on", "is", "this", "don't", "that", "it's", "with",
                "for", "but", "so", "are", "up", "ain't"
            };

            return !string.IsNullOrEmpty(word) && !bannedWords.Contains(word);
        }
    }
}
