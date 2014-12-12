using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DemoPatients.WebApp
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class BasicAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return Authenticate(httpContext);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!AuthorizeCore(filterContext.HttpContext))
            {
                HttpResponseBase res = filterContext.HttpContext.Response;
                res.StatusCode = 401;
                res.AddHeader("WWW-Authenticate", "Basic realm=\"Demo Patients\"");
                res.End();
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        private bool Authenticate(HttpContextBase filterContext)
        {
            HttpRequestBase request = filterContext.Request;
            string authorization = request.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(authorization) && authorization.StartsWith("Basic"))
            {
                string[] cred = Encoding.ASCII.GetString(Convert.FromBase64String(authorization.Substring(6))).Split(':');
                if (cred[0] == "login" && cred[1] == "password")
                    return true;
            }

            return false;
        }
    }
}