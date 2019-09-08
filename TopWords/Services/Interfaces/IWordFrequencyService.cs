using System.Collections.Generic;

namespace TopWords.Services.Interfaces
{
    public interface IWordFrequencyService
    {
        IDictionary<string, int> GetWordsFrequencies(IEnumerable<string> artistLyrics);
    }
}
