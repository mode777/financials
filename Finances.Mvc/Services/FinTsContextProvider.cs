using Finances.Mvc.Data;
using libfintx.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finances.Mvc.Services
{
    public class FinTsContextProvider
    {
        private readonly IMemoryCache cache;
        private readonly HttpClient http;
        private readonly IServiceProvider serviceProvider;

        public FinTsContextProvider(IMemoryCache cache, HttpClient http, IServiceProvider serviceProvider)
        {
            this.cache = cache;
            this.http = http;
            this.serviceProvider = serviceProvider;
        }

        public async Task<ConnectionContext> GetContextAsync(int connectionId)
        {
            return await cache.GetOrCreateAsync(connectionId, CreateContextForConnectionAsync);
        }

        private async Task<ConnectionContext> CreateContextForConnectionAsync(ICacheEntry entry)
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);

            using (var scope = serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
            {
                var details = await context.ConnectionData.FindAsync(entry.Key);
                
                return new ConnectionContext
                {
                    Client = http,
                    Blz = int.Parse(details.Blz),
                    Account = details.AccountId,
                    IBAN = details.Iban,
                    Url = details.FinTsServer,
                    UserId = details.UserId,
                    Pin = details.Pin,
                    HBCIVersion = 300,
                    // TODO: Change me!
                    BIC = "BYLADEM1ANS"
                };
            }
        }
    }
}
