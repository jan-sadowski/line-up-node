using LineUpNode.Models;
using LineUpNode.Helpers;
using HtmlAgilityPack;

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

                var dayBlocks = doc.DocumentNode.SelectNodes("//h3");

                if (dayBlocks != null)
                {
                    string currentDate;

                    foreach (var dayBlock in dayBlocks)
                    {
                        var dateText = dayBlock.InnerText.Trim();
                        Console.WriteLine($"Found date: {dateText}");
           
                        currentDate = dateText;

                        var showNodes = dayBlock.SelectNodes(".//following-sibling::table//span[@class='hour']");
                        if (showNodes == null) continue;

                        foreach (var show in showNodes)
                        {
                            var fullText = show.InnerText.Trim();
                            var parts = fullText.Split('-', 2, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length < 2) continue;

                            var time = parts[0].Trim();
                            var title = parts[1].Trim();

                            var formattedDateTime = IluzjonDateParser.FormatDateTime(currentDate, time); // metoda pomocnicza
                            
                            movies.Add(new MovieDto
                            {
                                Title = title,
                                Time = formattedDateTime,
                                Cinema = CinemaName
                            });

                            Console.WriteLine($"Added: {title} at {formattedDateTime}");
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
                Console.WriteLine($"Error: {ex.Message}");
            }

            return movies;
        }
    }
}