using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TopWords.APIResources;
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

        public LyricsService(ICrawlerService crawlerService, IWordFrequencyService wordFrequencyService, IHostingEnvironment environment, ILogger<LyricsService> logger)
        {
            _crawlerService = crawlerService;
            _wordFrequencyService = wordFrequencyService;
            _environment = environment;
            _logger = logger;
        }

        public async Task<List<WordResource>> GetTopWordsFromArtistLyrics(long artistId)
        {
            _logger.LogInformation($"Looking for artist with Id {artistId}");

            if (TopWordsFileExists(artistId))
            {
                _logger.LogInformation("File found, done!");
                return GetTopWordsFromFile(artistId);
            }

            // TODO: web sockets? SignalR? push notifications? If it's worth saving to a logfile, it's worth showing in the user interface. remove logs
            // TODO: what happens in case of ajax error? crawl and show artist name. show message 'politely crawling url..'
            // TODO: add artist name - id table
            // TODO: change or add another lyrics site?

            var songsUrls = await _crawlerService.GetSongsUrls(artistId);

            if (songsUrls.Count == 0)
            {
                _logger.LogInformation("No songs found");
                return null;
            }

            _logger.LogInformation($"{songsUrls.Count} songs urls found");

            for (int i = 0; i < songsUrls.Take(10).Count(); i++) // TODO: remove the 10
            {
                _logger.LogInformation($"Crawling song #{i + 1}..");

                string songLyrics = await _crawlerService.GetSongLyrics(songsUrls[i]);
                _artistLyrics.Add(songLyrics);
            }

            _logger.LogInformation($"Analyzing..");

            var result = _wordFrequencyService.GetWordsFrequencies(_artistLyrics);

            SaveTopWordsToFile(result, artistId);

            _logger.LogInformation("Done!");

            return result;
        }

        private bool TopWordsFileExists(long artistId)
        {
            return File.Exists(GetArtistFileName(artistId));
        }

        private List<WordResource> GetTopWordsFromFile(long artistId)
        {
            return JsonConvert.DeserializeObject<List<WordResource>>(File.ReadAllText(GetArtistFileName(artistId)));
        }

        private string GetArtistFileName(long artistId)
        {
            return Path.Combine(_environment.ContentRootPath, "Data", artistId + ".json");
        }

        private void SaveTopWordsToFile(List<WordResource> topWords, long artistId)
        {
            _logger.LogInformation("Saving file..");
            File.WriteAllText(GetArtistFileName(artistId), JsonConvert.SerializeObject(topWords));
        }
    }
}
