using System.Collections.Generic;
using System.Linq;
using TopWords.APIResources;
using TopWords.Services.Interfaces;

namespace TopWords.Services
{
    public class WordFrequencyService : IWordFrequencyService
    {
        public List<WordResource> GetWordsFrequencies(List<string> enumerable)
        {
            var dictionary = enumerable
                   .SelectMany(x => x.ToLower().Split())
                   .Where(x => IsValidWord(x))
                   .GroupBy(x => x)
                   .ToDictionary(x => x.Key, x => x.Count())
                   .OrderByDescending(x => x.Value)
                   .Take(10);

            var result = new List<WordResource>();

            foreach (var item in dictionary)
            {
                result.Add(new WordResource { Word = item.Key, Frequency = item.Value });
            }

            return result;
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
