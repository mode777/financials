using libfintx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finances.Mvc.Dtos
{
    public class TransactionResult
    {
        public TransactionResult(int connectionId, HBCIDialogResult result)
        {
            if (result == null)
                return;

            ConnectionId = connectionId;

            if(result.IsSuccess && !result.IsSCARequired)
            {
                Status = "success";
            }
            else if (result.IsSCARequired)
            {
                Status = "tan_required";
            }
            else
            {
                Status = "error";
            }

            Messages = result.Messages?.Select(x => x.ToString()).ToArray();
            
        }

        public int ConnectionId { get; }
        public string Status { get; set; }
        public string[] Messages { get; set; }
    }
}
