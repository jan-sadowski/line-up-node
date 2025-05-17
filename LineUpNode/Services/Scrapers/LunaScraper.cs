using HtmlAgilityPack;
using LineUpNode.Models;

namespace LineUpNode.Services.Scrapers;

public class LunaScraper : IScraperService
{
    public string CinemaName => "Luna";
    public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
    {
        var movies = new List<MovieDto>();
        var today = DateTime.Today;

        for (int i = 0; i < 7; i++)
        {
            var date = today.AddDays(i);
            var dateStr = date.ToString("yyyy-MM-dd");
            var url = $"https://kinoluna.bilety24.pl/?b24_day={dateStr}";

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                var html = await client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                Console.WriteLine($"Scraping page: {url}");

                var movieLinks = doc.DocumentNode.SelectNodes("//div[@class='list-item']");
                if (movieLinks != null)
                {
                    Console.WriteLine($"Found {movieLinks.Count} movies for {dateStr}");

                    foreach (var movieLink in movieLinks)
                    {
                        var titleNode = movieLink.SelectSingleNode(".//h3[contains(@class,'list-item-title')]/a");
                        var hourNode = movieLink.SelectSingleNode(".//div[contains(@class,'list-item-btns')]//a/span[contains(@class,'hour')]");

                        var time = hourNode?.InnerText.Trim();
                        var title = titleNode?.InnerText.Trim();

                        // Łączymy datę z godziną
                        string dateTime = time != null ? $"{dateStr} {time}" : dateStr;

                        movies.Add(new MovieDto
                        {
                            Title = title,
                            Time = dateTime,
                            Cinema = CinemaName
                        });

                        Console.WriteLine("{0}, {1}, {2}", title, dateTime, CinemaName);
                    }
                }
                else
                {
                    Console.WriteLine($"No movie sections found for {dateStr}.");
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