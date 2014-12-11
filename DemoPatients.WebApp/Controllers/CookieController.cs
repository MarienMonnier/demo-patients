using System.Web.Mvc;

namespace DemoPatients.WebApp.Controllers
{
    public class CookieController : Controller
    {
        [HttpPost]
        public ContentResult HideMessage()
        {
            Session.Add("HasHiddenMessage", true);
            return Content("OK");
        }
    }
}