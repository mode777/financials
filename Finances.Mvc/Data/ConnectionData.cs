using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Finances.Mvc.Data
{
    public class ConnectionData
    {
        public int Id { get; set; }
        [Required]
        public string Blz { get; set; }
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string Iban { get; set; }
        [Required]
        public string FinTsServer { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Pin { get; set; }
        [Required]
        public IdentityUser Owner { get; set; }
    }
}
