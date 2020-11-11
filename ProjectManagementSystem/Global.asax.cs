using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem
{
    using System.Configuration;
    using System.IO;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // log4net setting.
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Config/log.config")));

            Application[ConfigurationKeys.LIST_ITEMS_PER_PAGE] = ConfigurationManager.AppSettings[ConfigurationKeys.LIST_ITEMS_PER_PAGE];
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders[typeof(string)] = new ProjectManagementSystem.Common.StringModelBinder();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            Exception ex = null;
            
            
            if ((ex = Server.GetLastError()) != null)
            {
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
            }

            var loginUser = Session[Constant.SESSION_LOGIN_USER] as LoginUser;
            if (loginUser != null)
            {
                logger.FatalFormat("Company code： [{0}]; Logged in user: [{1}]", loginUser.CompanyCode, loginUser.UserAccount);
            }
            if (ex != null)
            {
                logger.Fatal(Messages.E999, ex);
            }
            else
            {
                logger.Fatal(Messages.E999);
            }

            Response.Redirect("/Error/");
        }

    }
}