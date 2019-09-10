using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TopWords.Hubs;
using TopWords.Models;
using TopWords.Services.Interfaces;
using TopWords.Settings;

namespace TopWords.Services
{
    public class LyricsService : ILyricsService
    {
        private readonly List<string> _artistLyrics = new List<string>();
        private readonly ICrawlerService _crawlerService;
        private readonly IWordFrequencyService _wordFrequencyService;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<LyricsService> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly IHubContext<TopWordsHub> _hub2;
        private readonly string _dataPath;

        public LyricsService(ICrawlerService crawlerService, IWordFrequencyService wordFrequencyService, IHostingEnvironment environment, ILogger<LyricsService> logger, IHubContext<TopWordsHub> hub2, IOptionsSnapshot<ApiSettings> apiSettings)
        {
            _crawlerService = crawlerService;
            _wordFrequencyService = wordFrequencyService;
            _environment = environment;
            _logger = logger;
            _hub2 = hub2;
            _apiSettings = apiSettings.Value;
            _dataPath = Path.Combine(_environment.ContentRootPath, "Data");
        }

        public async Task<TopWordsResult> GetTopWordsFromArtistLyrics(TopWordsParams topWordsParams)
        {
            _logger.LogInformation($"Looking for artist with Id {topWordsParams.ArtistId}");
            await NotifyClient(topWordsParams.ConnectionId, "Starting..");

            if (TopWordsFileExists(topWordsParams.ArtistId))
            {
                _logger.LogInformation("File found, done!");
                return GetTopWordsFromFile(topWordsParams.ArtistId);
            }

            var songsPageInfo = await _crawlerService.GetSongsPageInfo(topWordsParams.ArtistId);

            if (songsPageInfo.SongsUrls.Count == 0)
            {
                _logger.LogInformation("No songs found");
                return new TopWordsResult(message: "No songs were found..");
            }

            _logger.LogInformation($"{songsPageInfo.SongsUrls.Count} songs urls found");
            await NotifyClient(topWordsParams.ConnectionId, $"{songsPageInfo.SongsUrls.Count} songs urls found");

            for (int i = 0; i < songsPageInfo.SongsUrls.Take(_apiSettings.CrawlSongsCount).Count(); i++)
            {
                _logger.LogInformation($"Crawling song #{i + 1}..");
                await NotifyClient(topWordsParams.ConnectionId, $"Crawling song #{i + 1}..");

                string songLyrics = await _crawlerService.GetSongLyrics(songsPageInfo.SongsUrls[i]);
                _artistLyrics.Add(songLyrics);
            }

            _logger.LogInformation("Analyzing..");
            await NotifyClient(topWordsParams.ConnectionId, "Analyzing..");

            var topWords = _wordFrequencyService.GetWordsFrequencies(_artistLyrics);
            var result = new TopWordsResult(artist: new Artist(topWordsParams.ArtistId, songsPageInfo.ArtistName), message: "OK", words: topWords);

            await SaveTopWordsToFile(result);

            _logger.LogInformation("Done!");

            return result;
        }


        public async Task<List<Artist>> GetAvailableArtistsAsync()
        {
            var artists = new List<Artist>();
            var filesNames = Directory.GetFiles(_dataPath, "*.json");

            foreach (var fileName in filesNames)
            {
                if (File.Exists(fileName))
                {
                    var fileContent = await File.ReadAllTextAsync(fileName);
                    var topWordsResult = JsonConvert.DeserializeObject<TopWordsResult>(fileContent);

                    if (topWordsResult?.Artist != null)
                    {
                        artists.Add(topWordsResult.Artist);
                    }
                }
            }

            return artists.OrderBy(x => x.Name).ToList();
        }

        private async Task NotifyClient(string connectionId, string message)
        {
            await _hub2.Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }

        private bool TopWordsFileExists(long artistId)
        {
            return File.Exists(GetArtistFileName(artistId));
        }

        private TopWordsResult GetTopWordsFromFile(long artistId)
        {
            return JsonConvert.DeserializeObject<TopWordsResult>(File.ReadAllText(GetArtistFileName(artistId)));
        }

        private string GetArtistFileName(long artistId)
        {
            return Path.Combine(_dataPath, $"{artistId}.json");
        }

        private async Task SaveTopWordsToFile(TopWordsResult topWordsResult)
        {
            _logger.LogInformation("Saving file..");
            await File.WriteAllTextAsync(GetArtistFileName(topWordsResult.Artist.Id), JsonConvert.SerializeObject(topWordsResult));
        }
    }
}
