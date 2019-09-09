using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TopWords.Models;
using TopWords.Services.Interfaces;

namespace TopWords.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class TopWordsController : ControllerBase
    {
        private readonly ILyricsService _lyricsService;

        public TopWordsController(ILyricsService lyricsService)
        {
            _lyricsService = lyricsService;
        }

        /// <summary>
        /// Returns the most used words of an artist in its songs.
        /// </summary>
        /// <param name="topWordsParams">Represents the artist subjected to the analysis.</param>
        /// <response code="200">A dictionary with the words and its frequency.</response>
        /// <response code="400">Usually if artistId is not a int64 number.</response>
        /// <response code="404">If the artist is not found.</response>
        /// <response code="500">Something went wrong. My bad.</response>
        /// <returns></returns>
        [HttpPost("api/topwords")]
        [Consumes("application/json")]
        public async Task<TopWordsResult> TopWordsAsync([FromBody]TopWordsParams topWordsParams)
        {
            if (topWordsParams == null)
            {
                throw new ArgumentNullException(nameof(topWordsParams));
            }

            return await _lyricsService.GetTopWordsFromArtistLyrics(topWordsParams);
        }
    }
}