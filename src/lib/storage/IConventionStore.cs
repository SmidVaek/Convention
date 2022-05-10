using Conventions.Models;

namespace Conventions.Storage
{
    public interface IConventionStore
    {
        Task<Convention> AddAsync(Convention convention);
        Task DeleteAsync(Convention convention);
        Task UpdateAsync(Convention convention);
        Task<IEnumerable<Convention>> GetAllAsync();
        Task<Convention?> GetByNameAsync(string name);
        Task<Convention?> GetByIdAsync(string id);
    }
}