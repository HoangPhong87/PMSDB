using ProjectManagementSystem.Common;
using ProjectManagementSystem.Resources;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjectManagementSystem.Controllers
{
    /// <summary>
    /// BaseController for all the controllers
    /// </summary>
    [Authorize]
    [ValidateInput(false)]
    public abstract class ControllerBase : Controller
    {
        protected static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string SESSION_SITEMAP = "SESSION_SITEMAP";

        private readonly IPMS01001Service _service = new PMS01001Service();

        /// <summary>
        /// Return the login user object
        /// </summary>
        /// <returns></returns>
        protected LoginUser GetLoginUser()
        {
            return Session[Constant.SESSION_LOGIN_USER] as LoginUser;
        }

        /// <summary>
        /// Set the login user to session
        /// </summary>
        /// <param name="user">Current user info</param>
        protected void SetLoginUser(LoginUser user)
        {
            Session[Constant.SESSION_LOGIN_USER] = user;
        }

        /// <summary>
        /// Check user authen by function
        /// </summary>
        /// <param name="func_id">Function ID</param>
        /// <returns>Access/Deny</returns>
        public bool IsInFunctionList(int func_id)
        {
            var currentUser = Session[Constant.SESSION_LOGIN_USER] as LoginUser;
            if (currentUser == null)
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                return false;
            }

            return (currentUser.PlanFunctionList.Contains(func_id) && currentUser.FunctionList.Contains(func_id));
        }

        /// <summary>
        /// This method is called before the acion method
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;
            var controller = routeData.Values["controller"].ToString();
            var loginUser = GetLoginUser();

            // Check login by license of user's company
            if (loginUser != null && controller != "PMS01001")
            {
                if (_service.CheckLicense(loginUser.CompanyCode) == 0)
                {
                    if (Request.IsAjaxRequest())
                    {
                        this.Response.StatusCode = 420;
                        return;
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(Url.Action("", "ErrorOutOfDate"));
                        return;
                    }
                }
            }

            if (!Request.IsAjaxRequest())
            {
                var action = routeData.Values["action"].ToString();

                // Check user password is expired
                if (loginUser != null
                    && action != "PersonalSetting"
                    && action != "Logout"
                    && action != "AuthentTimeout"
                    && controller != "ErrorAuthent"
                    && controller != "Error")
                {
                    if (loginUser.Is_expired_password)
                    {
                        Session[Constant.PASSWORD_OUT_OF_DATE] = 1;
                        filterContext.Result = new RedirectResult(Url.Action("PersonalSetting", "PMS01002"));
                        return;
                    }
                }

                int pos = Sitemap.FindIndex(item => item.ControllerName == controller);
                string[] controllerArr = { "PMS06001", "PMS09001", "PMS09002", "PMS09003", "PMS09004", "PMS09005" };

                // Clear Jquery datatable state data
                if (controllerArr.Contains(controller)
                    && action == "ClearSaveCondition"
                    && 0 <= pos)
                {
                    Sitemap[pos].RestoreData = null;
                }

                if (0 <= pos)
                {
                    Sitemap.RemoveRange(pos, 0);
                }
                else
                {
                    var item = new SitemapItem
                    {
                        ControllerName = controller,
                        RestoreData = null
                    };

                    Sitemap.Insert(0, item);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// This method is called after the acion method
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (Request.IsAjaxRequest())
            {
                if (!ModelState.IsValid)
                {
                    filterContext.Result = Json(
                        new
                        {
                            ErrorMessages = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
                        }
                    );
                }
            }
        }

        /// <summary>
        /// This method is called while the acion method
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;

            if (viewResult != null)
            {
                viewResult.ViewBag.LoginUser = GetLoginUser();
                viewResult.ViewBag.WindowName = GetWindowName();
                viewResult.ViewBag.CheckServer = CheckIPAddress();
            }

            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        ///  This method is called when has exception occur
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null)
            {
                var loginUser = GetLoginUser();

                if (loginUser != null)
                {
                    logger.FatalFormat("Company code: [{0}]; Logged in user： [{1}]", loginUser.CompanyCode, loginUser.UserAccount);
                }

                logger.Fatal(Messages.E999, filterContext.Exception);
            }

            base.OnException(filterContext);

            if (!filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
            {
                this.Response.Redirect("/Error");
            }
        }

        /// <summary>
        /// Go back to the previous screen
        /// </summary>
        /// <param name="n">steps</param>
        /// <returns>ActionResult</returns>
        protected ActionResult Backward(int n)
        {
            if (0 < n && n < Sitemap.Count)
            {
                var sitemap = Sitemap[n];

                return Backward(sitemap);
            }
            else
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        protected ActionResult Backward(string controllerName)
        {
            int pos = Sitemap.FindIndex(item => item.ControllerName == controllerName);
            if (0 <= pos)
            {
                var sitemap = Sitemap[pos];

                return Backward(sitemap);
            }
            else
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sitemap"></param>
        /// <returns></returns>
        private ActionResult Backward(SitemapItem sitemap)
        {
            if (sitemap != null)
            {
                var factory = ControllerBuilder.Current.GetControllerFactory();
                var controller = factory.CreateController(Request.RequestContext, sitemap.ControllerName);

                string actionName = "Index";
                if (controller is IRestoreStateController)
                {
                    actionName = "Restore";
                }

                factory.ReleaseController(controller);

                return RedirectToAction(actionName, sitemap.ControllerName);
            }
            else
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Get Jquery datatable
        /// </summary>
        /// <returns></returns>
        protected object GetRestoreData()
        {
            object data = null;

            int pos = Sitemap.FindIndex(item => item.ControllerName == RouteData.Values["controller"].ToString());
            if (0 <= pos)
            {
                data = Sitemap[pos].RestoreData;
                //Sitemap[pos].RestoreData = null;
            }

            return data;
        }

        /// <summary>
        /// Save Jquery datatable
        /// </summary>
        /// <param name="data"></param>
        protected void SaveRestoreData(object data)
        {
            if (data != null)
            {
                int pos = Sitemap.FindIndex(item => item.ControllerName == RouteData.Values["controller"].ToString());
                if (0 <= pos)
                {
                    Sitemap[pos].RestoreData = data;
                }
            }
        }

        /// <summary>
        /// Clear Jquery datatable
        /// </summary>
        public ActionResult ClearRestoreData()
        {
            int pos = Sitemap.FindIndex(item => item.ControllerName == RouteData.Values["controller"].ToString());
            if (0 <= pos)
            {
                Sitemap[pos].RestoreData = null;
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Get window name to set title page
        /// </summary>
        /// <returns></returns>
        private string GetWindowName()
        {
            var controller = RouteData.Values["controller"].ToString();

            string windowName = Constant.WindowName.MAIN;

            HttpCookie cookie = Request.Cookies[Constant.WindowName.COOKIE_NAME];
            if (cookie != null)
            {
                if (Constant.WindowName.Items.Contains(controller))
                {
                    cookie.Value = Constant.WindowName.Items[controller] as string;
                }

                windowName = cookie.Value;
            }

            return windowName;
        }


        /// <summary>
        /// checking the ip address of the server pc
        /// </summary>
        /// <returns></returns>
        public int CheckIPAddress()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(strHostName);
            string strIP = string.Join<IPAddress>(",", ipHostInfo.AddressList.ToArray());
            if (strIP.Contains(Constant.TEST_ENVIRONMENT_IP))
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Build site map
        /// </summary>
        private List<SitemapItem> Sitemap
        {
            get
            {
                var windowName = GetWindowName();
                var sitemaps = Session[SESSION_SITEMAP] as IDictionary<string, List<SitemapItem>>;

                if (sitemaps == null)
                {
                    sitemaps = new Dictionary<string, List<SitemapItem>>();
                    foreach (string name in Constant.WindowName.Items.Values)
                    {
                        sitemaps.Add(name, new List<SitemapItem>());
                    }

                    Session[SESSION_SITEMAP] = sitemaps;
                }

                var sitemap = sitemaps[windowName];

                return sitemap;
            }
        }

        /// <summary>
        /// Site map item
        /// </summary>
        [Serializable]
        private class SitemapItem
        {
            public string ControllerName { get; set; }

            public object RestoreData { get; set; }
        }

        /// <summary>
        /// Print sitemap to log file
        /// </summary>
        private void PrintSitemap()
        {
            var windowName = GetWindowName();
            var sitemaps = Session[SESSION_SITEMAP] as IDictionary<string, List<SitemapItem>>;

            var sitemap = sitemaps[windowName];

            var sb = new System.Text.StringBuilder();
            sb.Append(string.Format("\nSitemap stack [{0}]\n", windowName));
            foreach (var item in sitemap.ToArray().Reverse())
            {
                sb.AppendFormat("\t{0}\n", item.ControllerName);
            }
            sb.Append("---------");

            logger.Debug(sb.ToString());
        }
    }
}
