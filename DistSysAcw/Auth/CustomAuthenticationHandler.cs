using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DistSysAcw.Models;
using System.Threading;

namespace DistSysAcw.Auth
{
    /// <summary>
    /// Authenticates clients by API Key
    /// </summary>
    public class CustomAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationHandler
    {
        private Models.UserContext DbContext { get; set; }

        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            Models.UserContext dbContext)
            : base(options, logger, encoder, clock) 
        {
            DbContext = dbContext;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then create the correct Claims, add these to a ClaimsIdentity, create a ClaimsPrincipal from the identity 
            //        Then use the Principal to generate a new AuthenticationTicket to return a Success AuthenticateResult
            if(!Request.Headers.ContainsKey("ApiKey"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized. Check ApiKey in Header is correct."));
            }

            var token = Request.Headers["ApiKey"].ToString();

            if(!UserDatabaseAccess.CheckKey(token))
            {
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized. Check ApiKey in Header is correct."));
            }

            User user = UserDatabaseAccess.GetUser(token);
            Claim name = new Claim(ClaimTypes.Name, user.UserName);
            Claim role = new Claim(ClaimTypes.Role, user.Role);
            ClaimsIdentity key = new ClaimsIdentity(token);
            key.AddClaim(name);
            key.AddClaim(role);

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(key), this.Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));

            #endregion
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            byte[] messagebytes = Encoding.ASCII.GetBytes("Task 5 Incomplete");
            Context.Response.StatusCode = 401;
            Context.Response.ContentType = "application/json";
            await Context.Response.Body.WriteAsync(messagebytes, 0, messagebytes.Length);
        }
    }
}