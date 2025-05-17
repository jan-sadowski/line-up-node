using HtmlAgilityPack;
using LineUpNode.Models;
using System.Text.RegularExpressions;
using System.Globalization;

namespace LineUpNode.Services.Scrapers
{
    public class PrahaScraper : IScraperService
    {
        public string CinemaName => "Praha";

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            var url = "https://www.mteatr.pl/pl/repertuar-kino-praha";
            var movies = new List<MovieDto>();

            try
            {
                using var client = new HttpClient();
                var html = await client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var newsDiv = doc.DocumentNode.SelectSingleNode("//div[@id='tresc_news']");
                if (newsDiv == null)
                    return movies;

                var paragraphs = newsDiv.SelectNodes("./p");
                if (paragraphs == null)
                    return movies;

                var dateRegex = new Regex(@"<b>([A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż\s]+)\s*(\d{1,2}\.\d{2})<\/b>", RegexOptions.IgnoreCase);
                var showRegex = new Regex(@"(\d{1,2}:\d{2})\s*[„“""]?(.+?)[””""]?\s*\((\d+)\s*MIN\)", RegexOptions.IgnoreCase);

                int currentYear = DateTime.Now.Year;

                foreach (var p in paragraphs)
                {
                    var innerHtml = p.InnerHtml;

                    var dateMatch = dateRegex.Match(innerHtml);
                    if (!dateMatch.Success)
                        continue;

                    var datePart = dateMatch.Groups[2].Value.Trim();

                    if (!DateTime.TryParseExact(datePart, "dd.MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        continue;

                    date = new DateTime(currentYear, date.Month, date.Day);

                    var showMatches = showRegex.Matches(innerHtml);
                    foreach (Match show in showMatches)
                    {
                        var timePart = show.Groups[1].Value.Trim();
                        var rawTitle = show.Groups[2].Value.Trim();
                        var title = HtmlEntity.DeEntitize(rawTitle);
                        title = title?.Replace("„", "").Replace("”", "").Replace("\"", "").Trim() ?? string.Empty;

                        if (!TimeSpan.TryParse(timePart, out var timeSpan))
                            continue;

                        var dateTime = date.Date + timeSpan;
                        var formattedTime = dateTime.ToString("yyyy-MM-dd HH:mm");

                        movies.Add(new MovieDto
                        {
                            Title = title,
                            Time = formattedTime,
                            Cinema = CinemaName
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return movies;
        }
    }
}