using System.IO;
using System.Web.Hosting;
using System.Web.Optimization;

namespace ProjectManagementSystem
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // js css cache clear
            BundleTable.EnableOptimizations = false;

            bundles.IgnoreList.Clear();
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-1.9.1.min.js",
                "~/Scripts/jquery-ui-1.10.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery/plugins").Include(
                "~/Scripts/plugins/app.min.js"
                , "~/Scripts/plugins/datepicker/bootstrap-datepicker.js"
                , "~/Scripts/plugins/datepicker/locales/bootstrap-datepicker.ja.js"
                , "~/Scripts/plugins/bootstrap-dialog.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                "~/Scripts/kickstart.js"
                , "~/Scripts/thickbox.js"
                , "~/Scripts/bootstrap.min.js"
                , "~/Scripts/jquery.numeric.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/input").Include(
                "~/Scripts/PMS.CmnEventUtil.js"
                , "~/Scripts/PMS.numeric.js"
                , "~/Scripts/PMS.Utility.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                "~/Scripts/jquery.dataTables.js"
                , "~/Scripts/fnStandingRedraw.js"
                , "~/Scripts/dataTables.colReorder.js"
                , "~/Scripts/PMS.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/dragOn").Include("~/Scripts/plugins/drag-on.js"));

            bundles.Add(new ScriptBundle("~/bundles/windowControl").Include("~/Scripts/PMS.windowControl.js"));

            bundles.Add(new ScriptBundle("~/bundles/loadingPlugin").Include("~/Scripts/pace.js"));

            bundles.Add(new ScriptBundle("~/bundles/projectRegist").Include("~/Scripts/PMS.ProjectRegist.js"));

            bundles.Add(new ScriptBundle("~/bundles/projectHistory").Include("~/Scripts/PMS.ProjectHistory.js"));

            bundles.Add(new ScriptBundle("~/bundles/actualWorkRegist").Include("~/Scripts/PMS.ActualWorkRegist.js"));

            bundles.Add(new ScriptBundle("~/bundles/actualWorkRegistNew").Include("~/Scripts/PMS.ActualWorkRegistNew.js"));

            bundles.Add(new ScriptBundle("~/bundles/actualRegistNew").Include("~/Scripts/jquery.vgrid.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-filestyle").Include("~/Scripts/bootstrap-filestyle.min.js"));

            bundles.Add(new StyleBundle("~/Content/css/bootstrap").Include("~/Content/css/bootstrap.css"));

            //HLQ Update
            bundles.Add(new StyleBundle("~/Content/themes/base/minified/jquery-ui.min").Include("~/Content/themes/base/minified/jquery-ui.min.css"));
            //

            bundles.Add(new StyleBundle("~/Content/fontawesome/css/icon").Include("~/Content/fontawesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/common").Include(
                "~/Content/datepicker.css"
                , "~/Content/cm-layout.css"
                , "~/Content/cm-form.css"
                , "~/Content/thickbox.css"
                , "~/Content/cm-table-paging.css"
                , "~/Content/dataTables.colReorder.css"
            ));

            bundles.Add(new StyleBundle("~/Content/css/Error").Include(
                "~/Content/Error.css"
            ));
            bundles.Add(new StyleBundle("~/Content/css/ErrorAuthent").Include(
               "~/Content/Error.css"
           ));
            bundles.Add(new StyleBundle("~/Content/css/ErrorOutOfDate").Include(
              "~/Content/Error.css"
           ));
            // Add css with controllerName 
            const string virtualPath = "~/Content";
            const string extension = "css";
            string path = HostingEnvironment.MapPath(virtualPath); 
            if (Directory.Exists(path))
            {
                string viewsDirectory = HostingEnvironment.MapPath("~/Views");
                foreach (string contentPath in Directory.GetFiles(path, "*" + extension))
                {
                    string controllerName = Path.GetFileNameWithoutExtension(contentPath);
                    string controllerDirectory = Path.Combine(viewsDirectory, controllerName);
                    if (Directory.Exists(controllerDirectory))
                    {
                        string actualVirtualPropPath = virtualPath + "/" + Path.GetFileName(contentPath);
                        bundles.Add(new StyleBundle("~/Content/css/" + controllerName).Include(actualVirtualPropPath));
                    }
                }
            }

        }
    }
}