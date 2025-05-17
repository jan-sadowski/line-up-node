using HtmlAgilityPack;
using LineUpNode.Models;

namespace LineUpNode.Services.Scrapers
{
    public class MuranowScraper : IScraperService
    {
        public string CinemaName => "Muranów";

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            var url = "https://kinomuranow.pl/repertuar";
            var movies = new List<MovieDto>();

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                var html = await client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                Console.WriteLine($"Scraping page: {url}");

                var movieNodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'movie-calendar-info__inner') and @data-toggle='collapse']");
                if (movieNodes != null)
                {
                    Console.WriteLine($"Found {movieNodes.Count} movies");

                    foreach (var movieNode in movieNodes)
                    {
                        var timeNode = movieNode.SelectSingleNode(".//span[contains(@class,'movie-calendar-info__date')]");
                        var titleNode = movieNode.SelectSingleNode(".//h5[contains(@class,'movie-calendar-info__title')]");

                        var time = timeNode?.InnerText.Trim();
                        var title = titleNode?.InnerText.Trim();

                        var dateHeader = movieNode.SelectSingleNode("preceding::div[contains(@class,'cell-date-header')][1]");
                        string dateStr = "";
                        if (dateHeader != null)
                        {
                            var dayNum = dateHeader.SelectSingleNode(".//span[contains(@class,'cell-date-header__day-num')]")?.InnerText.Trim();
                            var month = dateHeader.SelectSingleNode(".//span[contains(@class,'cell-date-header__day-month-short') or contains(@class,'cell-date-header__day-month')]")?.InnerText.Trim();

                            var months = new Dictionary<string, string>
                            {
                                {"stycznia","01"}, {"lutego","02"}, {"marca","03"}, {"kwietnia","04"},
                                {"maja","05"}, {"czerwca","06"}, {"lipca","07"}, {"sierpnia","08"},
                                {"września","09"}, {"października","10"}, {"listopada","11"}, {"grudnia","12"}
                            };

                            string monthNum = "01";
                            if (!string.IsNullOrEmpty(month) && months.TryGetValue(month, out var foundMonth))
                            {
                                monthNum = foundMonth;
                            }

                            var year = DateTime.Now.Year.ToString();
                            dateStr = $"{year}-{monthNum}-{dayNum?.PadLeft(2, '0')}";
                        }
                        else
                        {
                            dateStr = DateTime.Now.ToString("yyyy-MM-dd");
                        }

                        string dateTime = !string.IsNullOrEmpty(time) ? $"{dateStr} {time}" : dateStr;

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
                    Console.WriteLine("No movie sections found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

            return movies;
        }
    }
}