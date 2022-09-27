using System.Collections.Generic;
using System.Linq;
using Codat.Bookkeeper.DataAccess;
using Codat.Bookkeeper.Models;
using Codat.Bookkeeper.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codat.Bookkeeper.Controllers
{
    [ApiController]
    [Route("/invoice/{invoiceId}/payments")]
    [Produces("application/json")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly BookkeeperDbContext _dbContext;
        private readonly IValidator<(Invoice, Payment)> _paymentValidator;

        public PaymentsController(
            BookkeeperDbContext dbContext,
            IValidator<(Invoice, Payment)> paymentValidator)
        {
            _dbContext = dbContext;
            _paymentValidator = paymentValidator;
        }

        [HttpPost("")]
        public IActionResult CreateInvoicePayment(int invoiceId, [FromBody] Payment payment)
        {
            var invoice = _dbContext.Invoices.SingleOrDefault(x => x.InvoiceId == invoiceId);

            if (invoice == null)
            {
                return NotFound();
            }

            if (!_paymentValidator.Validate((invoice, payment), out var validationErrors))
            {
                return BadRequest(validationErrors);
            }

            if(invoice.Payments == null)
            {
                invoice.Payments = new List<Payment>();
            }

            if (_dbContext.Payments.Any())
            {
                payment.PaymentId = _dbContext.Payments.Max(x => x.PaymentId) + 1;
            }

            invoice.Payments.Add(payment);

            _dbContext.Invoices.Update(invoice);
            _dbContext.Payments.Add(payment);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpGet()]
        public ActionResult<IEnumerable<Payment>> ViewInvoicePayments(int invoiceId)
        {
            var invoice = _dbContext.Invoices.SingleOrDefault(x => x.InvoiceId == invoiceId && x.TenantId == User.GetTenantId());

            if(invoice == null)
            {
                return NotFound();
            }

            var invoicePayments = _dbContext.Payments.Where(x => x.InvoiceId == invoiceId);
            if (!invoicePayments.Any())
            {
                return NotFound();
            }

            return invoicePayments.ToList();
        }
    }
}