using Finances.Mvc.Data;
using libfintx.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly ApplicationDbContext db;
        private readonly HttpClient http;

        public FinTsContextProvider(IMemoryCache cache, ApplicationDbContext db, HttpClient http)
        {
            this.cache = cache;
            this.db = db;
            this.http = http;
        }

        public async Task<ConnectionContext> GetContextAsync(int connectionId)
        {
            return await cache.GetOrCreateAsync(connectionId, CreateContextForConnectionAsync);
        }

        private async Task<ConnectionContext> CreateContextForConnectionAsync(ICacheEntry entry)
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);

            var details = await db.ConnectionData.FindAsync(entry.Key);

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
