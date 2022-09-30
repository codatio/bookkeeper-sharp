using Codat.Bookkeeper.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Codat.Bookkeeper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly BookkeeperDbContext _dbContext;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            BookkeeperDbContext dbContext
            ) : base(options, logger, encoder, clock)
        {
            _dbContext = dbContext;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader["Basic ".Length..].Trim();
                
                var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialstring.Split(':');

                var username = credentials[0];
                var hashedPwd = GetHashedPassword(credentials[1]);
                var user = _dbContext.Users.SingleOrDefault(x => x.Name == username && x.HashedPassword == hashedPwd.ToLower());

                if (user != null)
                {
                    var claims = new[] {
                        new Claim(ClaimTypes.NameIdentifier, user.Name),
                        new Claim("TenantId", user.TenantId.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
                }

                Response.StatusCode = 401;
                Response.Headers.Add("WWW-Authenticate", "Basic realm=\"bookkeeper.codat.io\"");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
            else
            {
                Response.StatusCode = 401;
                Response.Headers.Add("WWW-Authenticate", "Basic realm=\"bookkeeper.codat.io\"");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }

        private static string GetHashedPassword(string password)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hash);
        }
    }
}
