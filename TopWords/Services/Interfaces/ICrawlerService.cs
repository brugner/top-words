using System.Threading.Tasks;
using TopWords.Models;

namespace TopWords.Services.Interfaces
{
    public interface ICrawlerService
    {
        Task<SongsPageInfo> GetSongsPageInfo(long artistId);
        Task<string> GetSongLyrics(string songUrl);
    }
}
