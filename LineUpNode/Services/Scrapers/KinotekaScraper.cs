using HtmlAgilityPack;
using LineUpNode.Models;

namespace LineUpNode.Services.Scrapers
{
    public class KinotekaScraper : IScraperService
    {
        public string CinemaName => "Kinoteka";

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            const string url = "https://kinoteka.pl/repertuar/";
            var movies = new List<MovieDto>();

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                
                var html = await client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                
                Console.WriteLine("Scraping page: {0}", url);
                
                var movieLinks = doc.DocumentNode.SelectNodes("//article[@class='e-movie']");
                if (movieLinks != null)
                {
                    Console.WriteLine("Found {0} movies", movieLinks.Count);

                    foreach (var movieLink in movieLinks)
                    {
                        var titleNode = movieLink.SelectSingleNode(".//a[@aria-label]");
                        var hourNode = movieLink.SelectSingleNode(".//a[@data-hour]");
                        var dayNode = movieLink.SelectSingleNode(".//a[@data-day]");

                        if (titleNode != null && hourNode != null && dayNode != null)
                        {
                            var title = titleNode.GetAttributeValue("aria-label", "").Trim();
                            var time = hourNode.GetAttributeValue("data-hour", "").Trim();
                            var day = dayNode.GetAttributeValue("data-day", "").Trim();
                            
                            var dateTime = $"{day} {time}";

                            movies.Add(new MovieDto
                            {
                                Title = title,
                                Time = dateTime,
                                Cinema = CinemaName
                            });
                            
                            Console.WriteLine("{0}, {1}, {2}", title, dateTime, CinemaName);
                        }
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