using HtmlAgilityPack;
using LineUpNode.Models;

namespace LineUpNode.Services.Scrapers
{
    public class IluzjonScraper : IScraperService
    {
        public string CinemaName => "Iluzjon";

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            var url = "https://www.iluzjon.fn.org.pl/repertuar.html";
            var movies = new List<MovieDto>();

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                
                var html = await client.GetStringAsync(url);
                
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                
                var titleNodes = doc.DocumentNode.SelectNodes("//article//h3");
                var timeNodes = doc.DocumentNode.SelectNodes("//article//span[@class='hour']");
                var locationNodes = doc.DocumentNode.SelectNodes("//article//div[@class='location']");

                if (titleNodes != null && timeNodes != null && locationNodes != null)
                {
                    var count = Math.Min(titleNodes.Count, Math.Min(timeNodes.Count, locationNodes.Count));
                    
                    for (int i = 0; i < count; i++)
                    {
                        var title = titleNodes[i].InnerText.Trim();
                        var time = timeNodes[i].InnerText.Trim();
                        var location = locationNodes[i].InnerText.Trim();

                        movies.Add(new MovieDto
                        {
                            Title = title,
                            Time = time,
                            Cinema = CinemaName
                        });

                        Console.WriteLine("{0}, {1}, {2}", title, time, location);
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