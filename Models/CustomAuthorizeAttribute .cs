using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanDoAnVaThucUong.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _role;

        public CustomAuthorizeAttribute(string role)
        {
            _role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session[_role + "User"] != null)
            {
                return true;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary(
                    new { controller = "Account", action = "Login", area = "" }));
        }
    }
}