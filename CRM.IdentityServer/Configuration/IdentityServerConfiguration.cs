using System.Collections.Generic;
using System.Reflection;
using CRM.DAL.Models.DatabaseModels.Users;
using CRM.IdentityServer.Extensions.Constants;
using CRM.IdentityServer.Models;
using CRM.IdentityServer.Services;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace CRM.IdentityServer.Configuration
{
    public static class IdentityServerConfiguration
    {
        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<IdentityServerDbContext>()
                .AddRoles<Role>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                
            });

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryClients(GetClients(configuration))
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(configuration.GetConnectionString("CRM"),
                            sql => sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

                    // this enables automatic token cleanup. this is optional.
                    options.DefaultSchema = "public";
                    options.EnableTokenCleanup = false;
                    options.TokenCleanupInterval = 3600; // interval in seconds
                    
                })
                .AddAspNetIdentity<User>()
                .AddProfileService<ProfileService>();

            services.AddTransient<IProfileService, ProfileService>();

            return services;
        }


        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = Scopes.Roles,
                    DisplayName = "Your user roles",
                    Required = true,
                    UserClaims = new List<string> { ClaimTypes.Role }
                },
                new IdentityResource
                {
                    Name = Scopes.SecurityStamp,
                    DisplayName = "Your security stamp",
                    Required = true,
                    UserClaims = new List<string> { Extensions.Constants.ClaimTypes.SecurityStamp }
                },
                new IdentityResource
                {
                    Name = Scopes.UserId,
                    DisplayName = "Your user id",
                    Required = true,
                    UserClaims = new List<string> { Extensions.Constants.ClaimTypes.UserId }
                }
            };
        }

        private static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var identityClients = new List<Client>();

            var configSection = configuration.GetSection("IdentityClients");
            foreach (var section in configSection.GetChildren())
            {
                var clientName = section.GetValue<string>("ClientName");
                var clientId = section.GetValue<string>("ClientId");
                var clientSecret = section.GetValue<string>("ClientSecret");
                var url = section.GetValue<string>("Url");

                var type = section.GetValue<string>("ClientType");

                if (type == JwtBearerDefaults.AuthenticationScheme)
                {
                    identityClients.Add(new Client
                    {
                        ClientName = clientName,
                        ClientId = clientId,
                        ClientSecrets =
                        {
                            new Secret(clientSecret.Sha256())
                        },
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        AlwaysIncludeUserClaimsInIdToken = true,
                        // scopes that client has access to
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            Scopes.SecurityStamp,
                            Scopes.Policies,
                            Scopes.Roles,
                            Scopes.UserId
                        },
                        AllowOfflineAccess = false,
                        RefreshTokenExpiration = TokenExpiration.Sliding,
                        
                    });
                }
                
            }

            return identityClients;
        }
    }
}