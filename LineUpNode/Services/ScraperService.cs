using LineUpNode.Models;
using System.Globalization;

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
                        var movieList = movies.ToList();

                        if (movieList.Any())
                        {
                            allMovies.AddRange(movieList);
                            Console.WriteLine($"[{scraper.CinemaName}] Found {movieList.Count} movies");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Scraper error ({scraper.CinemaName}): {ex.Message}");
                }
            }

            return allMovies;
        }

        public async Task<IEnumerable<MovieDto>> GetFilteredSortedPagedMoviesAsync(
            int page = 1,
            int pageSize = 15,
            string? date = null,
            string? cinemaName = null,
            string? title = null)
        {
            var allMovies = (await GetAllMoviesAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(cinemaName))
            {
                allMovies = allMovies
                    .Where(m => m.Cinema != null && m.Cinema.Contains(cinemaName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                allMovies = allMovies
                    .Where(m => m.Title != null && m.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out var filterDate))
            {
                allMovies = allMovies
                    .Where(m =>
                        DateTime.TryParse(m.Time, out var movieTime) &&
                        movieTime.Date == filterDate.Date)
                    .ToList();
            }
            allMovies = allMovies
                .Where(m => DateTime.TryParse(m.Time, out _))
                .OrderBy(m => DateTime.Parse(m.Time!))
                .ToList();
                
            return allMovies
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}