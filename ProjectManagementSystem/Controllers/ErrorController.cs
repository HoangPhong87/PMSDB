using System.Web.Mvc;
using System.Web.Security;

namespace ProjectManagementSystem.Controllers
{
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Error
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return View("Error");
        }
    }
}
