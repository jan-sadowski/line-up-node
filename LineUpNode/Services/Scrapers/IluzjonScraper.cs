using HtmlAgilityPack;
using LineUpNode.Models;

namespace LineUpNode.Services.Scrapers
{
    public class IluzjonScraper : IScraperService
    {
        public string CinemaName => "Iluzjon";

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            const string url = "https://www.iluzjon.fn.org.pl/repertuar.html";
            var movies = new List<MovieDto>();

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                var html = await client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var dayBlocks = doc.DocumentNode.SelectNodes("//div[contains(@class, 'box wide')]");

                if (dayBlocks != null)
                {
                    foreach (var block in dayBlocks)
                    {
                        var dateNode = block.SelectSingleNode(".//h3");
                        if (dateNode == null) continue;

                        var dateText = dateNode.InnerText.Trim(); 

                        var showNodes = block.SelectNodes(".//span[@class='hour']");
                        if (showNodes == null) continue;

                        foreach (var show in showNodes)
                        {
                            var fullText = show.InnerText.Trim();

                            var parts = fullText.Split('-', 2, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length < 2) continue;

                            var time = parts[0].Trim();
                            var title = parts[1].Trim();

                            movies.Add(new MovieDto
                            {
                                Title = title,
                                Time = $"{dateText} {time}",
                                Cinema = CinemaName
                            });

                            Console.WriteLine("Added: {0} {1} {2}", title, dateText, time);
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