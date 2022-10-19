using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace DistSysAcw.Auth
{
    /// <summary>
    /// Authorises clients by role
    /// </summary>
    public class CustomAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationHandler
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        public CustomAuthorizationHandler(IHttpContextAccessor httpContext)
        {
            HttpContextAccessor = httpContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            #region Task6
            // TODO:  Modify the server's behaviour so that, when the action requires a user to be in Admin role ONLY 
            // (e.g. [Authorize(Roles = "Admin")]) and the user does not have the Admin role, you return a Forbidden status (403) 
            // with the message: "Forbidden. Admin access only."


            if (context.User != null && context.User.Identity.IsAuthenticated)
            {
                if ((requirement.AllowedRoles.Count() == 1) && (requirement.AllowedRoles.First() == "Admin"))
                {
                    if(!context.User.IsInRole("Admin"))
                    {
                        var response = HttpContextAccessor.HttpContext.Response;
                        response.StatusCode = 403;
                        response.WriteAsync("Forbidden. Admin access only.");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            #endregion          


            if (context.User != null && context.User.Identity.IsAuthenticated)
            {
                foreach (string role in requirement.AllowedRoles)
                {
                    if (context.User.IsInRole(role))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
            }
            
            context.Fail();

            return Task.CompletedTask;
        }
    }
}