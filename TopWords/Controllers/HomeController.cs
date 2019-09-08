using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TopWords.Models;

namespace TopWords.Controllers
{
    public class HomeController : Controller
    {
        private readonly TopWordsController _topWordsController;

        public HomeController(TopWordsController topWordsController)
        {
            _topWordsController = topWordsController;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("mvc/topwords")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> TopWordsAsync([FromBody]Artist artist)
        {
            var result = await _topWordsController.TopWordsAsync(artist);
            return Ok(result);
        }
    }
}
