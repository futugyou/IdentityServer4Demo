using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class InitializeDatabase
    {
        public static async Task InitAsync(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                await context.Database.MigrateAsync();
                if (!context.Clients.Any())
                {
                    await context.Clients.AddRangeAsync(Config.GetClients().Select(c => c.ToEntity()));
                }
                if (!context.IdentityResources.Any())
                {
                    await context.IdentityResources.AddRangeAsync(Config.GetIdentityResources().Select(c => c.ToEntity()));
                }
                if (!context.ApiResources.Any())
                {
                    await context.ApiResources.AddRangeAsync(Config.GetApiResources().Select(c => c.ToEntity()));
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
