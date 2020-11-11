using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10001;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS10001;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;
using System.Data;

namespace ProjectManagementSystem.Controllers
{
    public class PMS10001Controller : ControllerBase
    {
        private readonly IPMS10001Service _service;

        /// <summary>
        /// TempData storage
        /// </summary>
        [System.Serializable]
        private class TmpValues
        {
            public int TagID { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10001Controller()
            : this(new PMS10001Service())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS10001Controller(IPMS10001Service service)
        {
            this._service = service;
        }

        //
        // GET: /PMS02001/

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if ((!IsInFunctionList(Constant.FunctionID.TagList_Admin)) && (!IsInFunctionList(Constant.FunctionID.TagList)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS10001ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS10001/Edit") && Session[Constant.SESSION_IS_BACK] != null)
            {
                var tmpCondition = GetRestoreData() as Condition;

                if (tmpCondition != null)
                    model.Condition = tmpCondition;
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("List", model);
        }

        /// <summary>
        /// Search tag by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by column</param>
        /// <param name="orderType">Order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json list information</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    var pageInfo = this._service.Search(model, condition, GetLoginUser().CompanyCode);
                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this._service.GetListTag(condition, GetLoginUser().CompanyCode, orderBy, orderType);
                    var result = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = pageInfo.TotalItems,
                            iTotalDisplayRecords = pageInfo.TotalItems,
                            aaData = (from t in pageInfo.Items
                                      select
                                          new object[]
                                                  {
                                                      t.tag_id,
                                                      t.peta_rn,
                                                      HttpUtility.HtmlEncode(t.display_name),
                                                      HttpUtility.HtmlEncode(t.tag_name),
                                                      t.display_order,
                                                      (t.upd_date != null) ? t.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                                      (t.user_update != null)? HttpUtility.HtmlEncode(t.user_update) : "",
                                                      t.tag_id,
                                                      t.del_flg
                                                  }).ToList()
                        });
                    SaveRestoreData(condition);
                    return result;
                }
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export to CSV tag list
        /// </summary>
        /// <param name="search_customerId"></param>
        /// <param name="search_tagName"></param>
        /// <param name="search_deleteFlag"></param>
        /// <param name="hdnOrderBy"></param>
        /// <param name="hdnOrderType"></param>
        /// <param name="TAB_ID"></param>
        /// <returns>CSV file export</returns>
        public ActionResult ExportCsvListTag(string search_customerId, string search_tagName, bool search_deleteFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            Condition condition = new Condition();
            if (!string.IsNullOrEmpty(search_customerId))
            {
                condition.CUSTOMER_ID = Convert.ToInt32(search_customerId);
            }

            condition.TAG_NAME = search_tagName;
            condition.DELETED_INCLUDE = search_deleteFlag;

            if (string.IsNullOrEmpty(hdnOrderBy))
                hdnOrderBy = "upd_date";

            IList<CustomerTagPlus> results = new List<CustomerTagPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<CustomerTagPlus>;
            }
            else
            {
                results = this._service.GetListTag(condition, GetLoginUser().CompanyCode, hdnOrderBy, hdnOrderType);
            }

            List<CustomerTagExport> dataExport = new List<CustomerTagExport>();
            string[] columns = new[] {
                    "No.",
                    "取引先名",
                    "タグ名",
                    "表示順",
                    "更新日時",
                    "更新者"
            };
            for (int i = 0; i < results.Count; i++)
            {
                results[i].peta_rn = i + 1;
            }

            foreach (var r in results)
            {
                CustomerTagExport tmpData = new CustomerTagExport();
                tmpData.tag_id = r.peta_rn;
                tmpData.display_name = r.display_name;
                tmpData.tag_name = r.tag_name;
                tmpData.display_order = r.display_order.HasValue ? r.display_order.Value.ToString() : "";
                tmpData.upd_date = (r.upd_date != null) ? r.upd_date.ToString("yyyy/MM/dd HH:mm") : "";
                tmpData.user_update = (r.user_update != null) ? r.user_update : "";
                dataExport.Add(tmpData);
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "TagList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!IsInFunctionList(Constant.FunctionID.TagRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = MakeEditViewModel(id);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <returns>Edit view</returns>
        [HttpGet]
        public ActionResult Edit()
        {
            if (!IsInFunctionList(Constant.FunctionID.TagRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = MakeEditViewModel(0);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit View
        /// </summary>
        /// <param name="tagId">tagId</param>
        /// <returns>Edit View Model</returns>
        private PMS10001EditViewModel MakeEditViewModel(int tagId)
        {
            var model = new PMS10001EditViewModel();
            if (tagId > 0)
            {
                model.CUSTOMERTAG_INFO = this._service.GetTagInfo(GetLoginUser().CompanyCode, tagId);
                model.CUSTOMERTAG_INFO.user_regist = model.CUSTOMERTAG_INFO.user_regist;
                model.CUSTOMERTAG_INFO.user_update = model.CUSTOMERTAG_INFO.user_update;
            }

            return model;
        }

        /// <summary>
        /// Check valid delete member assignment
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId">User Id</param>
        /// <returns>JSON member actual work time</returns>
        [HttpGet]
        public ActionResult CheckUsedTag(string customer_id, string tag_id)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(customer_id) || string.IsNullOrEmpty(tag_id))
            {
                return new EmptyResult();
            }

            int existedData = this._service.GetDataSalesPayment(GetLoginUser().CompanyCode, Convert.ToInt32(customer_id), Convert.ToInt32(tag_id));
            var returnValue = existedData > 0 ? "Existed" : "NotExisted";
            JsonResult result = Json(
                returnValue,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Edit Tag
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTag(PMS10001EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CUSTOMERTAG_INFO.upd_date = Utility.GetCurrentDateTime();
                    model.CUSTOMERTAG_INFO.upd_id = GetLoginUser().UserId;
                    model.CUSTOMERTAG_INFO.company_code = GetLoginUser().CompanyCode;

                    if (!model.CUSTOMERTAG_INFO.display_order.HasValue)
                    {
                        model.CUSTOMERTAG_INFO.display_order = 0;
                    }

                    int tagID = _service.EditTagData(model.CUSTOMERTAG_INFO);
                    if (tagID > 0)
                    {
                        string action = Convert.ToInt32(model.CUSTOMERTAG_INFO.tag_id) > 0 ? "更新" : "登録";
                        string message = string.Format(Resources.Messages.I007, "タグ情報", action);
                        var data = this._service.GetTagInfo(GetLoginUser().CompanyCode, tagID);

                        JsonResult result = Json(
                                new
                                {
                                    statusCode = 201,
                                    message = message,
                                    id = tagID,
                                    insDate = (data.ins_date != null) ? data.ins_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    updDate = (data.upd_date != null) ? data.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    insUser = data.user_regist,
                                    updUser = data.user_update,
                                    deleted = data.del_flg.Equals(Constant.DeleteFlag.DELETE) ? true : false
                                },
                                JsonRequestBehavior.AllowGet);

                        return result;
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.Messages.E001);
                        JsonResult result = Json(
                                    new
                                    {
                                        statusCode = 500,
                                        message = string.Format(Resources.Messages.E045, "タグ情報")
                                    },
                                    JsonRequestBehavior.AllowGet);

                        return result;

                    }
                }
                ModelState.AddModelError("", Resources.Messages.E001);
                return new EmptyResult();
            }
            catch
            {
                JsonResult result = Json(
                     new
                     {
                         statusCode = 500,
                         message = string.Format(Resources.Messages.E045, "タグ情報")
                     },
                     JsonRequestBehavior.AllowGet);

                return result;
            }
        }
    }
}
