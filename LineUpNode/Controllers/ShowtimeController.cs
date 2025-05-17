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
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            [FromQuery] string? date = null,
            [FromQuery] string? cinemaName = null,
            [FromQuery] string? title = null)
        {
            var movies = await _scraperService.GetFilteredSortedPagedMoviesAsync(page, pageSize, date, cinemaName, title);
            return Ok(movies);
        }
    }
}