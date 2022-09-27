using System.Collections.Generic;
using System.Linq;
using Codat.Bookkeeper.Models;

namespace Codat.Bookkeeper.Validator
{
    public interface IValidator<in T>
    {
        bool Validate(T obj, out List<string> validationErrors);
    }

    public class InvoiceValidator : IValidator<Invoice>
    {
        public bool Validate(Invoice obj, out List<string> validationErrors)
        {
            validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(obj.CustomerName))
            {
                validationErrors.Add("Customer name must not be empty");
            }

            if (obj.Amount <= 0)
            {
                validationErrors.Add("Invoice must have amount greater than 0");
            }

            if (obj.AmountOutstanding < 0)
            {
                validationErrors.Add("Sum of invoice payments must be less than invoice amount");
            }

            return !validationErrors.Any();
        }
    }
}