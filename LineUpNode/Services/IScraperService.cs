using LineUpNode.Models;

namespace LineUpNode.Services
{
    public interface IScraperService
    {
        string CinemaName { get; }
        Task<IEnumerable<MovieDto>> GetMoviesAsync();
    }
}