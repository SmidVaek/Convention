using Conventions.Services;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Security.Claims;

namespace Conventions.Identity.IdentityServer
{
    public class ProfileService : IProfileService
    {
        private readonly IUserService _userService;

        public ProfileService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.Claims.Where(x => x.Type == "sub").FirstOrDefault();

            if (subject == default)
            {
                throw new ArgumentException("sub claim missing in Claims");
            }

            var userId = subject.Value;
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }

            var defaultClaims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.GivenName, user.Name),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.Address, user.Address),
                new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
            };

            var roleClaims = user.Roles?.Select(x => new Claim(JwtClaimTypes.Role, x)) ?? Array.Empty<Claim>();
            defaultClaims.AddRange(roleClaims);

            context.AddRequestedClaims(defaultClaims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            // the concept of deactivation does not exist in this mvp.
            await Task.CompletedTask;
            context.IsActive = true;
        }
    }
}
