using Conventions.Models;
using Conventions.Storage;

namespace Conventions.Services
{
    public interface IConventionService
    {
        Task<IEnumerable<Convention>> GetAllAsync();
        Task<Convention?> GetByIdAsync(string id);
        Task DeleteAsync(string id);
        Task UpdateAsync(Convention convention);
        Task<Convention> CreateAsync(Convention convention);
    }

    public class ConventionService: IConventionService
    {
        private readonly IConventionStore _conventionStore;

        public ConventionService(IConventionStore conventionStore)
        {
            _conventionStore = conventionStore;
        }

        public async Task<IEnumerable<Convention>> GetAllAsync()
        {
            return await _conventionStore.GetAllAsync();
        }

        public async Task<Convention?> GetByIdAsync(string id)
        {
            return await _conventionStore.GetByIdAsync(id);
        }

        public async Task<Convention> CreateAsync(Convention convention)
        {
            return await _conventionStore.AddAsync(convention);
        }

        public async Task DeleteAsync(string id)
        {
            var convention = await _conventionStore.GetByIdAsync(id);
            if (convention == null)
            {
                // throw some reasonable here?
                return;
            }
            await _conventionStore.DeleteAsync(convention);
        }

        public async Task UpdateAsync(Convention convention)
        {
            await _conventionStore.UpdateAsync(convention);
        }
    }
}