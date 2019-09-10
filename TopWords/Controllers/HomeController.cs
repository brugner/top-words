using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TopWords.Models;
using TopWords.Services.Interfaces;

namespace TopWords.Controllers
{
    public class HomeController : Controller
    {
        private readonly TopWordsController _topWordsController;
        private readonly ILyricsService _lyricsService;

        public HomeController(TopWordsController topWordsController, ILyricsService lyricsService)
        {
            _topWordsController = topWordsController;
            _lyricsService = lyricsService;
        }

        public async Task<IActionResult> Index()
        {
            var artists = await _lyricsService.GetAvailableArtistsAsync();

            return View(artists);
        }

        [HttpPost("mvc/topwords")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> TopWordsAsync([FromBody]TopWordsParams topWordsParams)
        {
            var result = await _topWordsController.TopWordsAsync(topWordsParams);
            return Ok(result);
        }
    }
}
