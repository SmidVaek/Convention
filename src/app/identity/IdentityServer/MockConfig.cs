using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Conventions.Identity.IdentityServer
{
    public class MockConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResources.Address(),
                new IdentityResource()
                {
                    Name = "roles",
                    Description = "Allows us to request your role",
                    Required = true,
                    UserClaims = new[]
                    {
                        JwtClaimTypes.Role
                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource()
                {
                    DisplayName = "Backoffice s2s scope",
                    Name = "backoffice.s2s",
                    UserClaims =
                    {
                        JwtClaimTypes.Role
                    },
                    Scopes =
                    {
                        "backoffice.s2s"
                    }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope()
                {
                    DisplayName = "Backoffice s2s scope",
                    Name = "backoffice.s2s",
                    UserClaims =
                    {
                        JwtClaimTypes.Role
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    RequirePkce = true,
                    ClientId = "backoffice.client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes= GrantTypes.Code,
                    AllowOfflineAccess = true,
                    RedirectUris = { "https://localhost:7100/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:7100" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles"
                    },
                },
                new Client
                {
                    ClientId = "backoffice.s2s",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientClaimsPrefix = "",
                    Claims =
                    {
                        new ClientClaim(JwtClaimTypes.Role, "backoffice.s2s")
                    },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes =
                    {
                        "backoffice.s2s"
                    },

                }
            };
        }
    }
}
