using System.Web.Mvc;
using System.Web.Security;

namespace ProjectManagementSystem.Controllers
{
    public class ErrorAuthentController : ControllerBase
    {
        /// <summary>
        /// Error authen
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return View("ErrorAuthent");
        }
    }
}
