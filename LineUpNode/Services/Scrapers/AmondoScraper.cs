using HtmlAgilityPack;
using LineUpNode.Models;

namespace LineUpNode.Services.Scrapers
{
    public class AmondoScraper : IScraperService
    {
        public string CinemaName => "Amondo";

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            var url = "https://kinoamondo.pl/repertuar/";
            var movies = new List<MovieDto>();

            try
            {
                using var client = new HttpClient();
                var html = await client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                Console.WriteLine($"Scraping page: {url}");

                var scheduleSections = doc.DocumentNode.SelectNodes("//div[starts-with(@id, 'schedule-')]");

                if (scheduleSections != null)
                {
                    foreach (var daySection in scheduleSections)
                    {
                        var id = daySection.Id;
                        var date = id.Replace("schedule-", ""); 

                        var movieNodes = daySection.SelectNodes(".//div[contains(@class, 'movie-tabs')]");
                        if (movieNodes == null)
                            continue;

                        foreach (var movieNode in movieNodes)
                        {
                            var titleNode = movieNode.SelectSingleNode(".//h3[@class='no-underline']");
                            if (titleNode == null)
                                continue;

                            var title = titleNode.InnerText.Trim();

                            var timeNodes = movieNode.SelectNodes(".//div[contains(@class, 'time-wrap')]/span[contains(@class, 'time')]");
                            if (timeNodes == null)
                                continue;

                            foreach (var timeNode in timeNodes)
                            {
                                var timeText = timeNode.InnerText.Trim();

                                var parts = timeText.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                var time = parts.Length > 1 ? parts[1].Trim() : parts[0].Trim();

                                movies.Add(new MovieDto
                                {
                                    Title = title,
                                    Time = $"{date} {time}",
                                    Cinema = CinemaName
                                });

                                Console.WriteLine($"Added: {title} {date} {time}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Not found");
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