using CinemaSite.API.Models;

namespace CinemaSite.API.Repositories.IRepositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task CreateRangeAsync(List<Movie> movie);
    }
}
