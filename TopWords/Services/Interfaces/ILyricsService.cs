using System.Collections.Generic;
using System.Threading.Tasks;
using TopWords.Models;

namespace TopWords.Services.Interfaces
{
    public interface ILyricsService
    {
        Task<TopWordsResult> GetTopWordsFromArtistLyrics(TopWordsParams topWordsParams);
        Task<List<Artist>> GetAvailableArtistsAsync();
    }
}
