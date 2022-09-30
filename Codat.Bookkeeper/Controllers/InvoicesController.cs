using Codat.Bookkeeper.DataAccess;
using Codat.Bookkeeper.Models;
using Codat.Bookkeeper.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Codat.Bookkeeper.Controllers
{
    [Route("invoices")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly BookkeeperDbContext _dbContext;
        private readonly IValidator<Invoice> _invoiceValidator;

        public InvoicesController(BookkeeperDbContext dbContext, IValidator<Invoice> invoiceValidator)
        {
            _dbContext = dbContext;
            _invoiceValidator = invoiceValidator;
        }

        [HttpGet()]
        public IEnumerable<Invoice> Get()
        {
            return _dbContext.Invoices.Include(x => x.Payments).Where(x => x.TenantId == User.GetTenantId());
        }

        [HttpGet("{invoiceId}")]
        public ActionResult<Invoice> Get(int invoiceId)
        {
            return _dbContext.Invoices.Include(x => x.Payments).Single(x => x.InvoiceId == invoiceId);
        }

        [HttpGet("search")]
        public IEnumerable<Invoice> Search([FromQuery]string reference)
        {
            var query = $"SELECT * FROM Invoices WHERE reference = '{reference}' AND TenantId = {{0}}";
            return _dbContext.Invoices.FromSqlRaw(query, User.GetTenantId());
        }

        [HttpPost()]
        public IActionResult Post([FromBody] Invoice invoice)
        {
            if (!_invoiceValidator.Validate(invoice, out var validationErrors))
            {
                return BadRequest(validationErrors);
            }

            SetInvoiceId();

            if (_dbContext.Invoices.Any(x => x.InvoiceId == invoice.InvoiceId))
            {
                return BadRequest($"Invoice numbered {invoice.InvoiceId} already exists");
            }

            _dbContext.Invoices.Add(invoice);
            _dbContext.SaveChanges();

            return Created($"/invoices/{invoice.InvoiceId}", invoice);

            void SetInvoiceId()
            {
                if (invoice.InvoiceId == 0 && _dbContext.Invoices.Any())
                {
                    invoice.InvoiceId = _dbContext.Invoices.Max(x => x.InvoiceId) + 1;
                }
            }
        }

        [HttpPut("{invoiceId}")]
        public IActionResult Put(int invoiceId, [FromBody] Invoice invoice)
        {
            if (!_dbContext.Invoices.Any(x => x.InvoiceId == invoiceId))
            {
                return NotFound();
            }

            if (invoice.InvoiceId != invoiceId)
            {
                return BadRequest("invoiceId in POST body does not match invoiceId in URL");
            }

            _dbContext.Update(invoice);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("{invoiceId}")]
        public IActionResult Delete(int invoiceId)
        {
            var invoice = _dbContext.Invoices.SingleOrDefault(x => x.InvoiceId == invoiceId);

            if (invoice == null)
            {
                return NotFound();
            }

            _dbContext.Invoices.Remove(invoice);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}