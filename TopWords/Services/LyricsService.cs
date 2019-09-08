using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TopWords.Hubs;
using TopWords.Models;
using TopWords.Services.Interfaces;

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

        public LyricsService(ICrawlerService crawlerService, IWordFrequencyService wordFrequencyService, IHostingEnvironment environment, ILogger<LyricsService> logger, IHubContext<TopWordsHub> hub)
        {
            _crawlerService = crawlerService;
            _wordFrequencyService = wordFrequencyService;
            _environment = environment;
            _logger = logger;
            _hub = hub;
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

            // TODO: what happens in case of ajax error? crawl and show artist name. show message 'politely crawling url..'
            // TODO: change or add another lyrics site?

            var songsUrls = await _crawlerService.GetSongsUrls(artistId);

            if (songsUrls.Count == 0)
            {
                _logger.LogInformation("No songs found");
                return new TopWordsResult(message: "No songs were found..");
            }

            _logger.LogInformation($"{songsUrls.Count} songs urls found");
            await NotifyClient($"{songsUrls.Count} songs urls found");

            for (int i = 0; i < songsUrls.Take(5).Count(); i++) // TODO: remove the 10
            {
                _logger.LogInformation($"Crawling song #{i + 1}..");
                await NotifyClient($"Crawling song #{i + 1}..");

                string songLyrics = await _crawlerService.GetSongLyrics(songsUrls[i]);
                _artistLyrics.Add(songLyrics);
            }

            _logger.LogInformation("Analyzing..");
            await NotifyClient("Analyzing..");

            var topWords = _wordFrequencyService.GetWordsFrequencies(_artistLyrics);
            var result = new TopWordsResult(message: "OK", words: topWords);

            SaveTopWordsToFile(result, artistId);

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

        private void SaveTopWordsToFile(TopWordsResult topWords, long artistId)
        {
            _logger.LogInformation("Saving file..");
            File.WriteAllText(GetArtistFileName(artistId), JsonConvert.SerializeObject(topWords));
        }
    }
}
