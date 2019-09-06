using System.Collections.Generic;
using TopWords.APIResources;

namespace TopWords.Services.Interfaces
{
    public interface IWordFrequencyService
    {
        List<WordResource> GetWordsFrequencies(List<string> artistLyrics);
    }
}
