using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        /// <param name="artistId">Artist Id as appears in the last segment of the artist's page url.</param>
        /// <response code="200">A list with the words and its frequency.</response>
        /// <response code="400">Usually if artistId is not a int64 number.</response>
        /// <response code="404">If the artist is not found.</response>
        /// <response code="500">Something went wrong. My bad.</response>
        /// <returns></returns>
        [HttpGet("topwords/{artistId}")]
        public async Task<IActionResult> TopWordsAsync(long artistId)
        {
            var result = await _lyricsService.GetTopWordsFromArtistLyrics(artistId);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound($"Nothing found for artist with Id {artistId}");
            }
        }
    }
}