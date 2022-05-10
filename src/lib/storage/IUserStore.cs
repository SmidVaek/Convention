using Conventions.Models;

namespace Conventions.Storage
{
    public interface IUserStore
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task DeleteAsync(string id);
    }
}