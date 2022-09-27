using System.Collections.Generic;
using System.Linq;
using Codat.Bookkeeper.Models;

namespace Codat.Bookkeeper.Validator
{
    public class PaymentValidator : IValidator<(Invoice invoice, Payment payment)>
    {
        public bool Validate((Invoice, Payment) obj, out List<string> validationErrors)
        {
            var (invoice, payment) = obj;

            validationErrors = new List<string>();

            if (invoice.AmountOutstanding < payment.Amount)
            {
                validationErrors.Add("Payment amount is greater than invoice outstanding amount");
            }

            if (payment.Amount <= 0)
            {
                validationErrors.Add("Payment must have amount greater than 0");
            }

            return !validationErrors.Any();
        }
    }
}