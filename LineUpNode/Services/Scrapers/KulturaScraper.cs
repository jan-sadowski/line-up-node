using HtmlAgilityPack;
using LineUpNode.Models;

namespace LineUpNode.Services.Scrapers;

public class KulturaScraper : IScraperService
{
    public string CinemaName => "Kultura";

    public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
    {
        var movies = new List<MovieDto>();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        var currentDate = DateTime.Today;

        try
        {
            for (int i = 0; i < 5; i++)
            {
                var date = currentDate.AddDays(i);
                var dateStr = date.ToString("yyyy-MM-dd");
                var timestamp = GetUnixTimestamp(date);
                var url = $"https://www.kinokultura.pl/_core/_include/_rep/rep_posters.php?ajax=1&u_time={timestamp}&rep_date={dateStr}";

                Console.WriteLine($"Scraping: {url}");

                var html = await client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Znajdź wszystkie węzły filmów
                var movieNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'content_rep_posters_1') and contains(@class, 'hand')]");

                if (movieNodes == null)
                {
                    Console.WriteLine($"No movies found for {dateStr}");
                    continue;
                }

                foreach (var node in movieNodes)
                {
                    var timeNode = node.SelectSingleNode(".//div[contains(@class,'content_rep_posters_1_2')]");
                    var imgNode = node.SelectSingleNode(".//img[contains(@class,'poster_photo')]");
                    var cinemaNode = node.SelectSingleNode(".//div[contains(@class,'content_rep_posters_1_1')]");

                    if (timeNode == null || imgNode == null || cinemaNode == null)
                        continue;

                    var hourPart = timeNode.SelectSingleNode(".//font")?.InnerText.Trim();
                    var minutePart = timeNode.InnerText.Replace(hourPart ?? "", "").Trim();

                    var title = imgNode.GetAttributeValue("title", "").Trim();
                    var cinema = cinemaNode.InnerText.Trim();
                    var timeStr = $"{hourPart}{minutePart}";

                    // Zmieniamy logikę, jeśli nazwa sali to 'Rejs', przypisujemy nazwę sali, jeśli nie to przypisujemy 'Kultura'
                    if (cinema != CinemaName && cinema != "Rejs")
                        continue;

                    movies.Add(new MovieDto
                    {
                        Title = title,
                        Time = $"{dateStr} {timeStr}",
                        Cinema = $"{cinema}"
                    });

                    Console.WriteLine($"{title}, {dateStr} {timeStr}, {cinema}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return movies;
    }

    private long GetUnixTimestamp(DateTime date)
    {
        return new DateTimeOffset(date).ToUnixTimeSeconds();
    }
}