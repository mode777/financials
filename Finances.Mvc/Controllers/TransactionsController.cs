using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finances.Mvc.Data;
using Finances.Mvc.Dtos;
using Finances.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finances.Mvc.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly BankingServices banking;
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> users;

        public TransactionsController(BankingServices banking, ApplicationDbContext db, UserManager<IdentityUser> users)
        {
            this.banking = banking;
            this.db = db;
            this.users = users;
        }
                
        [HttpPost]
        public async Task<TransactionResult> Sync()
        {
            var id = await GetConnectionId();

            await banking.UpdateAsync(id);

            return new TransactionResult(id, null);
        }

        public class CompleteParams
        {
            public int ConnectionId { get; set; }
            public string Tan { get; set; }
        }

        [HttpPost("complete")]
        public async Task<TransactionResult> Complete([FromBody]CompleteParams param)
        {
            await banking.CompleteTransactionAsync(param.ConnectionId, param.Tan);

            return new TransactionResult(param.ConnectionId, null);
        }

        private async Task<int> GetConnectionId()
        {
            var userId = users.GetUserId(User);
            var connection = await db.ConnectionData.FirstOrDefaultAsync(x => x.Owner.Id == userId);
            return connection.Id;
        }
    }
}