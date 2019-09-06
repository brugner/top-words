using System.Collections.Generic;
using System.Threading.Tasks;

namespace TopWords.Services.Interfaces
{
    public interface ICrawlerService
    {
        Task<List<string>> GetSongsUrls(long artistId);
        Task<string> GetSongLyrics(string songUrl);
    }
}
