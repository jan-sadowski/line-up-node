using HtmlAgilityPack;
using LineUpNode.Models;

namespace LineUpNode.Services.Scrapers
{
    public class WislaScraper : IScraperService
    {
        public string CinemaName => "Wisła";

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            var movies = new List<MovieDto>();
            var today = DateTime.Today;

            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(i);
                var dateStr = date.ToString("yyyy-MM-dd");
                var url = $"https://www.novekino.pl/kina/wisla/repertuar.php?data={dateStr}#1";

                try
                {
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    var html = await client.GetStringAsync(url);
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    Console.WriteLine($"Scraping page: {url}");

                    var movieRows = doc.DocumentNode.SelectNodes("//table[contains(@class,'repertoire-list')]/tr[contains(@class,'repertoire-movie-tr')]");
                    if (movieRows != null)
                    {
                        foreach (var row in movieRows)
                        {
                            var titleNode = row.SelectSingleNode(".//div[contains(@class,'repertoire-movie-title')]/a");
                            var title = titleNode?.InnerText.Trim() ?? "Brak tytułu";

                            var timeCell = row.SelectSingleNode("./td[contains(@class,'repertoire-movie-info-td')]/following-sibling::td");
                            if (timeCell != null)
                            {
                                var timeNodes = timeCell.SelectNodes(".//a");
                                if (timeNodes != null)
                                {
                                    foreach (var timeNode in timeNodes)
                                    {
                                        var time = timeNode.InnerText.Trim();
                                        movies.Add(new MovieDto
                                        {
                                            Title = title,
                                            Time = $"{dateStr} {time}",
                                            Cinema = CinemaName
                                        });
                                        Console.WriteLine($"Added: {title} {dateStr} {time}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"No showtimes found for: {title}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No movies found for {dateStr}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error for {0}: {1}", dateStr, ex.Message);
                }
            }

            return movies;
        }
    }
}