using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Resources;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;

namespace ProjectManagementSystem.Controllers
{
    public class CommonController : ControllerBase
    {
        private readonly IPMSCommonService commonService;
        private readonly IPMS01002Service servicePMS01002;
        private readonly IPMS02001Service servicePMS02001;

        public CommonController()
            : this(new PMSCommonService(), new PMS01002Service(), new PMS02001Service())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public CommonController(IPMSCommonService commonservice, IPMS01002Service servicePMS01002, IPMS02001Service servicePMS02001)
        {
            this.commonService = commonservice;
            this.servicePMS01002 = servicePMS01002;
            this.servicePMS02001 = servicePMS02001;
        }

        /// <summary>
        /// Get the company logo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetImage(string id, string type)
        {
            try
            {
                if ((Constant.GetImage.USER_IMAGE.Equals(type) || Constant.GetImage.CUSTOMER_IMAGE.Equals(type)) && string.IsNullOrEmpty(id))
                {
                    return new EmptyResult();
                }

                if (!Request.IsAjaxRequest())
                {
                    this.Response.StatusCode = 403;
                    return new EmptyResult();
                }

                string path = string.Empty;

                if (type == Constant.GetImage.USER_IMAGE)
                {
                    var userInfo = this.servicePMS01002.GetUserInfo(GetLoginUser().CompanyCode, Convert.ToInt32(id));
                    path = ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + userInfo.image_file_path;
                }
                else if (type == Constant.GetImage.CUSTOMER_IMAGE)
                {
                    var customerInfo = this.servicePMS02001.GetCustomerInfo(GetLoginUser().CompanyCode, Convert.ToInt32(id));
                    path = ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + customerInfo.logo_image_file_path;
                }
                else
                {
                    path = ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_BASE_FILE_PATH] + GetLoginUser().CompanyLogoImgPath;
                }

                if (System.IO.File.Exists(path))
                {
                    FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    try
                    {
                        byte[] data = new byte[(int)fileStream.Length];

                        fileStream.Read(data, 0, data.Length);

                        return Json(new
                            {
                                base64imgage = Convert.ToBase64String(data)
                            },
                            JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }

                return new EmptyResult();
            }
            catch
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Check session timeout
        /// </summary>
        /// <returns>Session status</returns>
        [HttpGet]
        public ActionResult CheckTimeOut()
        {
            if (Session[Constant.SESSION_LOGIN_USER] == null)
            {
                this.Response.StatusCode = 419;
                logger.Fatal(Messages.E044);
                return new EmptyResult();
            }

            JsonResult result = Json(
                "Success",
                JsonRequestBehavior.AllowGet
            );
            return result;
        }

        /// <summary>
        /// Check current user authent
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult AuthentTimeout()
        {
            logger.Info(Messages.E044);
            Session.Clear();

            if (Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 419;
                return new EmptyResult();
            }
            else
            {
                return RedirectToAction("Login", "PMS01001", new { id = "timeout" });
            }
        }

        /// <summary>
        /// Get JSON tag list by customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>JSON tag list</returns>
        public ActionResult GetTagListJson(string customerId)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(customerId))
            {
                return new EmptyResult();
            }

            IList<CustomerTag> list = commonService.GetTagList(GetLoginUser().CompanyCode, Convert.ToInt32(customerId));

            JsonResult result = Json(
                list,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Retrieve the Jquery DataTable state data
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDataTableState(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.EndsWith("Index"))
                {
                    path = path.Replace("/Index", "");
                }
            }

            if (Session[path] != null)
                return Json(Session[path], JsonRequestBehavior.AllowGet);
            else
                return new EmptyResult();
        }

        /// <summary>
        /// Save Jquery DataTable state data
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDataTableState(string path, object data)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.EndsWith("Index"))
                {
                    path = path.Replace("/Index", "");
                }
            }
            Session[path] = data;

            return new JsonResult();
        }

        /// <summary>
        /// Check limit data when update by liscense of current user's company
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public ActionResult CheckRegistNewData(string dataType)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(dataType))
            {
                return new EmptyResult();
            }

            var loginUser = GetLoginUser();
            var valid = this.commonService.CheckValidUpdateData(loginUser.CompanyCode, dataType);

            JsonResult result = Json(
                valid,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Set session of button Back click
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetButtonBackSession()
        {
            Session[Constant.SESSION_IS_BACK] = "true";
            JsonResult result = Json(
                "",
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Clear search result data when close page
        /// </summary>
        /// <param name="TAB_ID"></param>
        /// <returns></returns>
        public ActionResult ClearSearchResult(string TAB_ID)
        {
            Session.Remove(Constant.SESSION_SEARCH_RESULT + TAB_ID);
            return new EmptyResult();
        }
    }
}
