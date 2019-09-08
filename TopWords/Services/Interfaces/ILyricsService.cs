using System.Threading.Tasks;
using TopWords.Models;

namespace TopWords.Services.Interfaces
{
    public interface ILyricsService
    {
        Task<TopWordsResult> GetTopWordsFromArtistLyrics(long artistId);
    }
}
