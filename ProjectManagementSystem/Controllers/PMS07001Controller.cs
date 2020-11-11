#region License
/// <copyright file="PMS07001Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/22</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS07001;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS07001;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with consumption tax
    /// </summary>
    public class PMS07001Controller : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// Main service
        /// </summary>
        private readonly IPMS07001Service mainService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS07001Controller(IPMS07001Service service)
        {
            this.mainService = service;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS07001Controller()
            : this(new PMS07001Service())
        {
        }

        #endregion

        #region Action
        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ConsumptionTaxList_Admin)
                && !this.IsInFunctionList(Constant.FunctionID.ConsumptionTaxList))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS07001ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS07001/Edit") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// Export consumption tax list to csv file
        /// </summary>
        /// <param name="hdnApplyStartDate">ApplyStartDate</param>
        /// <param name="hdnOrderBy">ConsumptionTax list order by</param>
        /// <param name="hdnOrderType">ConsumptionTax list order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>csv file</returns>
        public ActionResult ExportCsv(DateTime? hdnApplyStartDate, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            try
            {
                Condition condition = new Condition()
                {
                    ApplyStartDate = hdnApplyStartDate
                };
                List<string> titles = new List<string>()
                {
                    "No.",
                    "適用開始日",
                    "消費税率",
                    "備考",
                    "登録日時",
                    "登録者"
                };

                if (string.IsNullOrEmpty(hdnOrderBy))
                    hdnOrderBy = "apply_start_date";

                string fileName = "ConsumptionTaxList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";

                IList<ConsumptionTaxPlus> consumptionTaxList = new List<ConsumptionTaxPlus>();
                if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
                {
                    consumptionTaxList = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<ConsumptionTaxPlus>;
                }
                else
                {
                    consumptionTaxList = this.mainService.ExportConsumptionTaxListToCSV(GetLoginUser().CompanyCode, condition, hdnOrderBy, hdnOrderType);
                }

                List<ConsumptionTaxListExportCSV> dataExport = new List<ConsumptionTaxListExportCSV>();

                foreach (var consumptionTaxInfo in consumptionTaxList)
                {
                    dataExport.Add(new ConsumptionTaxListExportCSV()
                    {
                        peta_rn = consumptionTaxInfo.peta_rn.ToString(),
                        apply_start_date = consumptionTaxInfo.apply_start_date.Value.ToString("yyyy/MM/dd"),
                        tax_rate = Convert.ToInt32(consumptionTaxInfo.tax_rate * 100).ToString() + "%",
                        remarks = (string.IsNullOrEmpty(consumptionTaxInfo.remarks) ? "" : consumptionTaxInfo.remarks.Replace("\r\n", " ")),
                        ins_date = consumptionTaxInfo.ins_date.Value.ToString("yyyy/MM/dd HH:mm"),
                        ins_user = consumptionTaxInfo.ins_user
                    });
                }

                DataTable dt = Utility.ToDataTableT(dataExport, titles.ToArray());
                Utility.ExportToCsvData(this, dt, fileName);

                return new EmptyResult();
            }
            catch
            {
                return this.RedirectToAction("Index", "Error");
            }
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="applyStartDate">ApplyStartDate</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(DateTime? applyStartDate)
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ConsumptionTaxRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            var model = new PMS07001EditViewModel();

            if (applyStartDate != null)
            {
                model.ConsumptionTaxInfo = this.mainService.GetConsumptionTaxInfo(GetLoginUser().CompanyCode, applyStartDate.Value);
                model.ConsumptionTaxInfo.tax_rate = Convert.ToInt32(model.ConsumptionTaxInfo.tax_rate * 100);
            }

            return this.View("Edit", model);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ConsumptionTaxRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");
            var model = new PMS07001EditViewModel();
            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit consumption tax info
        /// </summary>
        /// <param name="model">ConsumptionTax info to edit</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConsumptionTax(PMS07001EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    if (model.ConsumptionTaxInfo.apply_start_date != model.ConsumptionTaxInfo.old_apply_start_date
                        && this.mainService.CountConsumptionTax(GetLoginUser().CompanyCode, model.ConsumptionTaxInfo.apply_start_date.Value) > 0)
                    {
                        ModelState.AddModelError("", Resources.Messages.E060);
                        return new EmptyResult();
                    }
                    else
                    {
                        model.ConsumptionTaxInfo.ins_date = Utility.GetCurrentDateTime();
                        model.ConsumptionTaxInfo.ins_id = loginUser.UserId;
                        model.ConsumptionTaxInfo.company_code = loginUser.CompanyCode;

                        if (this.mainService.EditConsumptionTaxInfo(model.ConsumptionTaxInfo))
                        {
                            string action = model.ConsumptionTaxInfo.old_apply_start_date != null ? "更新" : "登録";
                            string message = String.Format(Resources.Messages.I007, "消費税率情報", action);
                            var data = this.mainService.GetConsumptionTaxInfo(GetLoginUser().CompanyCode, model.ConsumptionTaxInfo.apply_start_date.Value);
                            JsonResult result = Json(
                                new
                                {
                                    statusCode = 201,
                                    message = message,
                                    apply_start_date = (data.apply_start_date != null) ? data.apply_start_date.Value.ToString("yyyy/MM/dd") : "",
                                    insDate = (data.ins_date != null) ? data.ins_date.Value.ToString("yyyy/MM/dd HH:mm") : "",
                                    insUser = data.ins_user,
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
                                       message = string.Format(Resources.Messages.E045, "消費税率情報")
                                   },
                                   JsonRequestBehavior.AllowGet);

                            return result;
                        }
                            
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
                         message = string.Format(Resources.Messages.E045, "消費税率情報")
                     },
                     JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        #endregion

        #region Ajax Action
        /// <summary>
        /// Search consumption tax by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by column</param>
        /// <param name="orderType">Order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json list of consumption tax</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                var pageInfo = this.mainService.Search(model, GetLoginUser().CompanyCode, condition);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this.mainService.ExportConsumptionTaxListToCSV(GetLoginUser().CompanyCode, condition, orderBy, orderType);
                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = (from t in pageInfo.Items select new object[]
                        {
                            null,
                            t.peta_rn,
                            t.apply_start_date.Value.ToString("yyyy/MM/dd"),
                            Convert.ToInt32(t.tax_rate * 100).ToString() + "%",
                            HttpUtility.HtmlEncode(t.remarks),
                            t.ins_date.Value.ToString("yyyy/MM/dd HH:mm"),
                            HttpUtility.HtmlEncode(t.ins_user),
                            null
                        }).ToList()
                    },
                    JsonRequestBehavior.AllowGet);
                SaveRestoreData(condition);
                return result;
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Count consumption tax by apply start date
        /// </summary>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>Number of consumption tax by apply start date</returns>
        [HttpGet]
        public ActionResult CountConsumptionTax(string applyStartDate)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            DateTime applyDate;

            if (DateTime.TryParse(applyStartDate, out applyDate))
            {
                var count = this.mainService.CountConsumptionTax(GetLoginUser().CompanyCode, applyDate);

                JsonResult result = Json(
                    count,
                JsonRequestBehavior.AllowGet);

                return result;
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Delete consumption tax by apply start date
        /// </summary>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>Number of consumption tax deleted</returns>
        [HttpPost]
        public ActionResult Delete(string applyStartDate)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            DateTime applyDate;

            if (DateTime.TryParse(applyStartDate, out applyDate))
            {
                var row = this.mainService.DeleteConsumptionTax(GetLoginUser().CompanyCode, applyDate);

                JsonResult result = Json(
                    row,
                JsonRequestBehavior.AllowGet);

                return result;
            }

            return new EmptyResult();
        }

        #endregion
    }
}
