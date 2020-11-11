using System.Web.Mvc;
using System.Web.Security;

namespace ProjectManagementSystem.Controllers
{
    public class ErrorOutOfDateController : ControllerBase
    {
        /// <summary>
        /// Error license out of date
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return View("OutOfDate");
        }
    }
}
