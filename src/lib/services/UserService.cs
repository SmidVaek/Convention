using Conventions.Models;
using Conventions.Storage;
using Microsoft.AspNetCore.Identity;

namespace Conventions.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
    }

    public class UserService: IUserService
    {
        private readonly IUserStore _userStore;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserService(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _userStore.GetAllAsync();
            return users;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _userStore.GetByEmailAsync(email);
            return user;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var user = await _userStore.GetByIdAsync(id);
            return user;
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            var users = await _userStore.GetByRoleAsync(role);
            return users;
        }

        public async Task<User> CreateAsync(User user)
        {
            var emailExists = await GetByEmailAsync(user.Email);
            if (emailExists is not null)
            {
                throw new ArgumentException("User with email already exists", nameof(user.Email));
            }

            if (user.Roles == null || !user.Roles.Any())
            {
                user.Roles = new[] { "user" };
            }

            // todo password rules?
            var hashedPassword = _passwordHasher.HashPassword(user, user.Password);
            user.Password = hashedPassword;

            var createdUser = await _userStore.AddAsync(user);
            return createdUser;
        }

        public async Task UpdateAsync(User user)
        {
            var emailExists = await GetByEmailAsync(user.Email);
            if (emailExists is null)
            {
                throw new ArgumentException("User with email does not exist", nameof(user.Email));
            }

            var newHashedPassword = _passwordHasher.HashPassword(user, user.Password);
            if (newHashedPassword != user.Password)
            {
                // for this exercise lets assume if they dont match its a new password.
                user.Password = newHashedPassword;
            }

            await _userStore.UpdateAsync(user);
        }

        public async Task DeleteAsync(string id)
        {
            // We could do checks and such to see if the user
            // exists or not, or just trust that it does.

            await _userStore.DeleteAsync(id);
        }
    }
}
