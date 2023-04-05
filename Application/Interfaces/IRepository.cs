using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetAll();
        Task<T> AddAsync(T value);
        Task<T> UpdateAsync(T value);
        Task RemoveAsync(T value);
        Task RemoveAllAsync();
    }
}
