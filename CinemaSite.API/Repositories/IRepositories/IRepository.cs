using CinemaSite.API.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaSite.API.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        // CRUD

        Task<T> CreateAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task CommitAsync();

        Task<List<T>> GetAsync(Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null, bool tracked = true);

        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null, bool tracked = true);
    }
}
