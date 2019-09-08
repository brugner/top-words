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
        private readonly IHubContext<TopWordsHub> _hub;
        private readonly ApiSettings _apiSettings;

        public LyricsService(ICrawlerService crawlerService, IWordFrequencyService wordFrequencyService, IHostingEnvironment environment, ILogger<LyricsService> logger, IHubContext<TopWordsHub> hub, IOptionsSnapshot<ApiSettings> apiSettings)
        {
            _crawlerService = crawlerService;
            _wordFrequencyService = wordFrequencyService;
            _environment = environment;
            _logger = logger;
            _hub = hub;
            _apiSettings = apiSettings.Value;
        }

        public async Task<TopWordsResult> GetTopWordsFromArtistLyrics(long artistId)
        {
            _logger.LogInformation($"Looking for artist with Id {artistId}");
            await NotifyClient("Starting..");

            if (TopWordsFileExists(artistId))
            {
                _logger.LogInformation("File found, done!");
                return GetTopWordsFromFile(artistId);
            }

            var songsPageInfo = await _crawlerService.GetSongsPageInfo(artistId);

            if (songsPageInfo.SongsUrls.Count == 0)
            {
                _logger.LogInformation("No songs found");
                return new TopWordsResult(message: "No songs were found..");
            }

            _logger.LogInformation($"{songsPageInfo.SongsUrls.Count} songs urls found");
            await NotifyClient($"{songsPageInfo.SongsUrls.Count} songs urls found");

            for (int i = 0; i < songsPageInfo.SongsUrls.Take(_apiSettings.CrawlSongsCount).Count(); i++)
            {
                _logger.LogInformation($"Crawling song #{i + 1}..");
                await NotifyClient($"Crawling song #{i + 1}..");

                string songLyrics = await _crawlerService.GetSongLyrics(songsPageInfo.SongsUrls[i]);
                _artistLyrics.Add(songLyrics);
            }

            _logger.LogInformation("Analyzing..");
            await NotifyClient("Analyzing..");

            var topWords = _wordFrequencyService.GetWordsFrequencies(_artistLyrics);
            var result = new TopWordsResult(artist: new Artist(artistId, songsPageInfo.ArtistName), message: "OK", words: topWords);

            await SaveTopWordsToFile(result);

            _logger.LogInformation("Done!");

            return result;
        }

        private async Task NotifyClient(string message)
        {
            await _hub.Clients.All.SendAsync("ReceiveMessage", message);
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
            return Path.Combine(_environment.ContentRootPath, "Data", artistId + ".json");
        }

        private async Task SaveTopWordsToFile(TopWordsResult topWordsResult)
        {
            _logger.LogInformation("Saving file..");
            await File.WriteAllTextAsync(GetArtistFileName(topWordsResult.Artist.Id), JsonConvert.SerializeObject(topWordsResult));
        }
    }
}
