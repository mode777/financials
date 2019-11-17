using libfintx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Finances.Mvc.Data
{
    public class AccountItem
    {
        private AccountItem()
        {
        }

        public AccountItem(TTransaction trans, int connectionId)
        {
            AccountCode = trans.accountCode;
            Amount = (double)trans.amount;
            BankCode = trans.bankCode;
            Description = trans.description;
            Id = trans.id;
            InputDate = trans.inputDate;
            MandateId = trans.mndtId;
            PartnerName = trans.partnerName;
            Type = trans.text;
            ValueDate = trans.valueDate;
            ConnectionId = connectionId;
        }

        [Key]
        public string Id { get; set; }
        public string AccountCode { get; set; }
        public double Amount { get; set; }
        public string BankCode { get; set; }
        public string Description { get; set; }
        public DateTime? InputDate { get; set; }
        public string MandateId { get; set; }
        public string PartnerName { get; set; }
        public string Type { get; set; }
        public DateTime? ValueDate { get; set; }
        public int ConnectionId { get; set; }

        [ForeignKey(nameof(ConnectionId))]
        public ConnectionData Connection { get; set; }

    }
}
