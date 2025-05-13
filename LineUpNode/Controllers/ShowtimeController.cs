using LineUpNode.Models;
using LineUpNode.Services;
using Microsoft.AspNetCore.Mvc;

namespace LineUpNode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimeController : ControllerBase
    {
        private readonly ScraperService _scraperService;

        public ShowtimeController(ScraperService scraperService)
        {
            _scraperService = scraperService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
        {
            var allMovies = await _scraperService.GetAllMoviesAsync();
            return Ok(allMovies);
        }
    }
}