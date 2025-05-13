using LineUpNode.Models;
using LineUpNode.Services;
using Microsoft.AspNetCore.Mvc;

namespace LineUpNode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimeController : ControllerBase
    {
        private readonly IEnumerable<IScraperService> _scrapers;

        public ShowtimeController(IEnumerable<IScraperService> scrapers)
        {
            _scrapers = scrapers;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
        {
            var allMovies = new List<MovieDto>();

            foreach (var scraper in _scrapers)
            {
                try
                {
                    var movies = await scraper.GetMoviesAsync();
                    
                    if (movies is List<MovieDto> movieList && movieList.Any())
                    {
                        allMovies.AddRange(movieList);
                        Console.WriteLine($"[{scraper.CinemaName}] Found {movieList.Count} movies");
                    }
                    else if (movies is MovieDto[] movieArray && movieArray.Any())
                    {
                        allMovies.AddRange(movieArray);
                        Console.WriteLine($"[{scraper.CinemaName}] Found {movieArray.Length} movies");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with scraper {scraper.GetType().Name}: {ex.Message}");
                }
            }

            return Ok(allMovies);
        }
    }
}