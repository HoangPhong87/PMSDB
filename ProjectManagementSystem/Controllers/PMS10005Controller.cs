using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10005;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS10005;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;

namespace ProjectManagementSystem.Controllers
{
    using System.Data;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Text;

    public class PMS10005Controller : ControllerBase
    {
        private readonly IPMS10005Service _service;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10005Controller()
            : this(new PMS10005Service())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS10005Controller(IPMS10005Service service)
        {
            this._service = service;
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if ((!IsInFunctionList(Constant.FunctionID.OverHeadCostList_Admin)) && (!IsInFunctionList(Constant.FunctionID.OverHeadCostList)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = new PMS10005ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS10005/Edit") && Session[Constant.SESSION_IS_BACK] != null)
            {
                var tmpCondition = GetRestoreData() as Condition;

                if (tmpCondition != null)
                    model.Condition = tmpCondition;
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("Index", model);
        }

        /// <summary>
        /// Search overhead_cost by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by column</param>
        /// <param name="orderType">Order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json list overhead_cost</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    condition.COMPANY_CODE = GetLoginUser().CompanyCode;
                    var pageInfo = this._service.Search(model, condition);
                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this._service.GetOverheadCostList(condition, orderBy, orderType);
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
                                        t.overhead_cost_id,
                                        t.peta_rn,
                                        HttpUtility.HtmlEncode(t.overhead_cost_type), 
                                        !string.IsNullOrEmpty(t.remarks) ? HttpUtility.HtmlEncode(t.remarks) : "", 
                                        (t.ins_date != null) ? t.ins_date.Value.ToString("yyyy/MM/dd HH:mm") : "",
                                        (t.user_regist != null)? HttpUtility.HtmlEncode(t.user_regist) : "",
                                        null
                                    }).ToList()
                        });
                    SaveRestoreData(condition);
                    return result;
                }
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export csv overhead_cost list
        /// </summary>
        /// <param name="search_overhead_cost_type">search_overhead_cost_type</param>
        /// <param name="hdnOrderBy">hdnOrderBy</param>
        /// <param name="hdnOrderType">hdnOrderType</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>CSV File</returns>
        public ActionResult ExportCsvList(string search_overhead_cost_type, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            var condition = new Condition()
            {
                COMPANY_CODE = GetLoginUser().CompanyCode,
                OVERHEAD_COST_TYPE = search_overhead_cost_type
            };

            if (string.IsNullOrEmpty(hdnOrderBy))
                hdnOrderBy = "ins_date";

            IList<OverHeadCostPlus> results = new List<OverHeadCostPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<OverHeadCostPlus>;
            }
            else
            {
                results = this._service.GetOverheadCostList(condition, hdnOrderBy, hdnOrderType);
            }

            List<OverHeadCostPlusExport> dataExport = new List<OverHeadCostPlusExport>();
            string[] columns = new[] {
                "No.",
                "諸経費種別",
                "備考",
                "登録日時",
                "登録新者"
            };

            int i = 1;
            foreach (var r in results)
            {
                OverHeadCostPlusExport tmpData = new OverHeadCostPlusExport();
                tmpData.no = i++;
                tmpData.overhead_cost_type = r.overhead_cost_type;
                tmpData.remarks = !string.IsNullOrEmpty(r.remarks) ? r.remarks : "";
                tmpData.ins_date = r.ins_date.Value.ToString("yyyy/MM/dd HH:mm");
                tmpData.user_regist = (r.user_regist != null) ? r.user_regist : "";
                dataExport.Add(tmpData);
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "OverHeadCostList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);
            return new EmptyResult();
        }

        /// <summary>
        /// Edit overhead_cost
        /// </summary>
        /// <param name="id">overhead_cost ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!IsInFunctionList(Constant.FunctionID.OverHeadCostRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakeEditViewModel(GetLoginUser().CompanyCode, id);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <returns>Edit View</returns>
        [HttpGet]
        public ActionResult Edit()
        {
            if (!IsInFunctionList(Constant.FunctionID.OverHeadCostRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var model = MakeEditViewModel(GetLoginUser().CompanyCode, 0);

            return this.View("Edit", model);
        }

        /// <summary>
        /// Make Edit view model
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="OVERHEAD_COSTId">OVERHEAD_COSTId</param>
        /// <returns>PMS10005Edit View Model</returns>
        private PMS10005EditViewModel MakeEditViewModel(string company_code, int OVERHEAD_COSTId)
        {
            var model = new PMS10005EditViewModel();
            if (OVERHEAD_COSTId > 0)
            {
                model.OVERHEAD_COST = this._service.GetOverheadCostInfo(company_code, OVERHEAD_COSTId);
            }

            return model;
        }

        /// <summary>
        /// Delete Overhead Cost By overhead_cost_id
        /// </summary>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>Number of Overhead deleted</returns>
        [HttpPost]
        public ActionResult Delete(int overhead_cost_id)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            string companyCode = GetLoginUser().CompanyCode;

            if (this._service.CheckExistOfOverHeadCode(companyCode, overhead_cost_id) == 0)
            {
                var row = this._service.DeleteOverHeadCode(companyCode, overhead_cost_id);

                JsonResult result = Json(
                      new
                      {
                          statusCode = 201,
                          row = row
                      },
                JsonRequestBehavior.AllowGet);

                return result;
            }
            else
            {
                JsonResult result = Json(
                    new
                    {
                        statusCode = 500,
                        message = string.Format(Resources.Messages.E068)
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        /// <summary>
        /// Edit Over Head Cost
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>Edit overhead cost view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOverHeadCost(PMS10005EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();
                    model.OVERHEAD_COST.ins_date = Utility.GetCurrentDateTime();
                    model.OVERHEAD_COST.ins_id = loginUser.UserId;
                    model.OVERHEAD_COST.company_code = loginUser.CompanyCode;

                    int overhead_cost_id = _service.EditOverheadCostData(model.OVERHEAD_COST);
                    if (overhead_cost_id > 0)
                    {
                        string action = model.OVERHEAD_COST.overhead_cost_id > 0 ? "更新" : "登録";
                        string message = String.Format(Resources.Messages.I007, "諸経費種別情報", action);

                        var data = this._service.GetOverheadCostInfo(loginUser.CompanyCode, overhead_cost_id);

                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = message,
                                overhead_cost_id = overhead_cost_id,
                                insDate = data.ins_date.Value.ToString("yyyy/MM/dd HH:mm"),
                                insUser = data.user_regist,
                                overhead_cost_type = data.overhead_cost_type,
                                remarks = data.remarks
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }
                    else
                    {
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 500,
                                message = string.Format(Resources.Messages.E045, "諸経費種別情報")
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
                        message = string.Format(Resources.Messages.E045, "諸経費種別情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }
    }
}
