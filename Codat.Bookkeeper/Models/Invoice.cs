using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Codat.Bookkeeper.Models
{
    [Index(nameof(TenantId))]
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string CustomerName { get; set; }
        public string Reference { get; set; }
        public int TenantId { get; set; }

        public decimal AmountPaid => Payments?.Sum(x => x.Amount) ?? 0;
        public decimal AmountOutstanding => Amount - AmountPaid;

        public List<Payment> Payments { get; set; }
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
    }
}