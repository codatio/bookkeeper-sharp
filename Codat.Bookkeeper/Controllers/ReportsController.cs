using System.Collections.Generic;
using System.Linq;
using Codat.Bookkeeper.DataAccess;
using Codat.Bookkeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codat.Bookkeeper.Controllers
{
    [Route("reports")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly BookkeeperDbContext _dbContext;

        public ReportsController(BookkeeperDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("AmountsOutstandingPerCustomer")]
        public ActionResult<List<OutstandingReportLine>> AmountsOutstandingPerCustomer()
        {
            var invoices = _dbContext.Invoices.Where(x => x.TenantId == User.GetTenantId()).ToList();

            return invoices.GroupBy(i => i.CustomerName)
                .Select(x => new OutstandingReportLine
                {
                    CustomerName = x.Key,
                    AmountOutstanding = x.Sum(i => i.AmountOutstanding)
                }).ToList();
        }
    }
}