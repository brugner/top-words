using System.Collections.Generic;
using System.Threading.Tasks;
using TopWords.APIResources;

namespace TopWords.Services.Interfaces
{
    public interface ILyricsService
    {
        Task<List<WordResource>> GetTopWordsFromArtistLyrics(long artistId);
    }
}
