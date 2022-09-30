using Codat.Bookkeeper.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Codat.Bookkeeper.Controllers
{
    [Route("admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Authorize()]
    public class AdminController : ControllerBase
    {
        private readonly BookkeeperDbContext _dbContext;

        public AdminController(BookkeeperDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpDelete("invoices/clear")]
        public async Task ClearInvoices()
        {
            _dbContext.Invoices.RemoveRange(_dbContext.Invoices);
            await _dbContext.SaveChangesAsync();
        }
    }
}