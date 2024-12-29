using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace web.features.shared;

public class AuthorizeOrRedirectAttribute : ActionFilterAttribute
{
        private readonly string _role;
        private readonly string _redirectRoute;

        public AuthorizeOrRedirectAttribute(string role, string redirectRoute)
        {
                _role = role;
                _redirectRoute = redirectRoute;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
                ClaimsPrincipal user = context.HttpContext.User;

                if (!user.IsInRole(_role))
                {
                        context.Result = new RedirectResult(_redirectRoute);
                }

                base.OnActionExecuting(context);
        }
}
