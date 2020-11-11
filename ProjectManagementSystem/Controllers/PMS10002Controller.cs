using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS10002;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS10002;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManagementSystem.Controllers
{
    public class PMS10002Controller : ControllerBase
    {
        #region Constructors

        private readonly IPMS10002Service _service;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10002Controller()
            : this(new PMS10002Service())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS10002Controller(IPMS10002Service service)
        {
            this._service = service;
        }
        #endregion

        #region InformationList

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if ((!IsInFunctionList(Constant.FunctionID.InfoList_Admin)) && (!IsInFunctionList(Constant.FunctionID.InfoList)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS10002ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS10002/Edit") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// <param name="orderType">zOrder type</param>
        /// <param name="TAB_ID">Tab id</param> 
        /// <returns>Json list information</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    var pageInfo = this._service.Search(model, condition, GetLoginUser().CompanyCode);
                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this._service.GetListInfomation(condition, GetLoginUser().CompanyCode, orderBy, orderType);
                    var result = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = pageInfo.TotalItems,
                            iTotalDisplayRecords = pageInfo.TotalItems,
                            aaData = (from t in pageInfo.Items select new object[]
                                {
                                    t.info_id,
                                    t.peta_rn,
                                    HttpUtility.HtmlEncode(t.content), 
                                    t.publish_start_date.ToString("yyyy/MM/dd"),  
                                    t.publish_end_date.ToString("yyyy/MM/dd"),  
                                    (t.upd_date != null) ? t.upd_date.ToString("yyyy/MM/dd HH:mm") : "",
                                    (t.display_name != null)? HttpUtility.HtmlEncode(t.display_name) : "",
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
        /// Export to CSV
        /// </summary>
        /// <param name="infor_content"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="deleteFlag"></param>
        /// <param name="hdnOrderBy"></param>
        /// <param name="hdnOrderType"></param>
        /// <param name="TAB_ID"></param>
        /// <returns>CSV file export</returns>
        public ActionResult ExportCsv(string infor_content, string start_date, string end_date, bool deleteFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            Condition condition = new Condition();
            condition.INFORMATION_CONTENT = infor_content;
            condition.START_DATE = start_date;
            condition.END_DATE = end_date;
            condition.DELETED_INCLUDE = deleteFlag;

            if (string.IsNullOrEmpty(hdnOrderBy))
                hdnOrderBy = "upd_date";

            IList<InformationPlus> results = new List<InformationPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<InformationPlus>;
            }
            else
            {
                results = this._service.GetListInfomation(condition, GetLoginUser().CompanyCode, hdnOrderBy, hdnOrderType);
            }
            List<InformationExport> dataExport = new List<InformationExport>();
            string[] columns = new[] {
                "No.",
                "掲載内容",
                "掲載開始日",
                "掲載終了日",
                "更新日時",
                "更新者"
            };

            int index = 1;

            foreach (var r in results)
            {
                InformationExport tmpData = new InformationExport();
                tmpData.info_no = index;
                tmpData.content = (string.IsNullOrEmpty(r.content) ? "" : r.content.Replace("\r\n", " "));
                tmpData.publish_start_date = r.publish_start_date.ToString("yyyy/MM/dd");
                tmpData.publish_end_date = r.publish_end_date.ToString("yyyy/MM/dd");
                tmpData.upd_date = (r.upd_date != null) ? r.upd_date.ToString("yyyy/MM/dd HH:mm") : "";
                tmpData.user_update = (r.display_name != null) ? r.display_name : "";
                dataExport.Add(tmpData);
                index++;
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "InformationList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }
        #endregion

        #region InformationRegist
        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!IsInFunctionList(Constant.FunctionID.InfoRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = MakeEditViewModel(id);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit()
        {
            if (!IsInFunctionList(Constant.FunctionID.InfoRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = MakeEditViewModel(0);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Make edit view model
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        private PMS10002EditViewModel MakeEditViewModel(int info_id)
        {
            var model = new PMS10002EditViewModel();
            if (info_id > 0)
            {
                model.INFORMATION = this._service.GetInformation(GetLoginUser().CompanyCode, info_id);
                model.INFORMATION.user_regist = model.INFORMATION.user_regist;
                model.INFORMATION.user_update = model.INFORMATION.user_update;
            }

            return model;
        }

        /// <summary>
        /// Submit information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInformation(PMS10002EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();
                    model.INFORMATION.upd_date = Utility.GetCurrentDateTime();
                    model.INFORMATION.upd_id = loginUser.UserId;
                    model.INFORMATION.company_code = loginUser.CompanyCode;
                    model.INFORMATION.display_order = 1;

                    int infoID = _service.EditInformationData(model.INFORMATION);
                    if (infoID > 0)
                    {
                        string action = model.INFORMATION.info_id > 0 ? "更新" : "登録";
                        string message = string.Format(Resources.Messages.I007, "掲載情報", action);
                        var data = this._service.GetInformation(loginUser.CompanyCode, infoID);

                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = message,
                                infoID = infoID,
                                insDate = data.ins_date.ToString("yyyy/MM/dd HH:mm"),
                                updDate = data.upd_date.ToString("yyyy/MM/dd HH:mm"),
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
                                message = string.Format(Resources.Messages.E045, "掲載情報")
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }
                }

                return new EmptyResult();
            }
            catch
            {
                JsonResult result = Json(
                    new
                    {
                        statusCode = 500,
                        message = string.Format(Resources.Messages.E045, "掲載情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }
        #endregion
    }
}
