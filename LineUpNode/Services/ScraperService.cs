using LineUpNode.Models;

namespace LineUpNode.Services
{
    public class ScraperService
    {
        private readonly IEnumerable<IScraperService> _scrapers;

        public ScraperService(IEnumerable<IScraperService> scrapers)
        {
            _scrapers = scrapers;
        }

        public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
        {
            var allMovies = new List<MovieDto>();

            foreach (var scraper in _scrapers)
            {
                try
                {
                    var movies = await scraper.GetMoviesAsync();
                    if (movies != null)
                    {
                        allMovies.AddRange(movies);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Scraper error ({scraper.CinemaName}): {ex.Message}");
                }
            }

            return allMovies;
        }
    }
}