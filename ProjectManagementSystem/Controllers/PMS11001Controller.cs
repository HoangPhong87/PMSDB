#region License
/// <copyright file="PMS11001Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ - Clone</author>
/// <createdDate>2017/06/28</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS11001;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS11001;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with Branch
    /// </summary>
    public class PMS11001Controller : ControllerBase
    {
        #region Constructor
        /// Main service
        private readonly IPMS11001Service mainService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS11001Controller(IPMS11001Service service)
        {
            this.mainService = service;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11001Controller()
            : this(new PMS11001Service())
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
            if (!this.IsInFunctionList(Constant.FunctionID.BranchList_Admin)
                && !this.IsInFunctionList(Constant.FunctionID.BranchList))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS11001ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS11001/Edit") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// Export Branch list to csv file
        /// </summary>
        /// <param name="hdnBranchName">Branch name</param>
        /// <param name="hdnDelFlag">Branch delete flag</param>
        /// <param name="hdnOrderBy">Branch list order by</param>
        /// <param name="hdnOrderType">Branch list order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>csv file</returns>
        public ActionResult ExportCsv(string hdnBranchName, bool hdnDelFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            try
            {
                Condition condition = new Condition()
                {
                    BranchName = hdnBranchName,
                    DeleteFlag = hdnDelFlag
                };
                List<string> titles = new List<string>()
                {
                    "No.",
                    "拠点名",
                    "拠点（表示名）",
                    "備考",
                    "更新日時",
                    "更新者"
                };

                if (string.IsNullOrEmpty(hdnOrderBy))
                {
                    hdnOrderBy = "upd_date";
                }

                string fileName = "BranchList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";

                IList<BranchPlus> branchList = new List<BranchPlus>();
                if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
                {
                    branchList = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<BranchPlus>;
                }
                else
                {
                    branchList = this.mainService.ExportBranchListToCSV(GetLoginUser().CompanyCode, condition, hdnOrderBy, hdnOrderType);
                }

                List<BranchListExportCSV> dataExport = new List<BranchListExportCSV>();

                foreach (var branchInfo in branchList)
                {
                    dataExport.Add(new BranchListExportCSV()
                    {
                        peta_rn = branchInfo.peta_rn.ToString(),
                        location_name = branchInfo.location_name,
                        display_name = branchInfo.display_name,
                        remarks = (string.IsNullOrEmpty(branchInfo.remarks) ? "" : branchInfo.remarks.Replace("\r\n", " ")),
                        upd_date = branchInfo.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                        upd_user = branchInfo.upd_user
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
        /// <param name="id">Branch ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!this.IsInFunctionList(Constant.FunctionID.BranchRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS11001EditViewModel();

            if (id > 0)
            {
                model.BranchInfo = this.mainService.GetBranchInfo(GetLoginUser().CompanyCode, id);
            }

            return this.View("Edit", model);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.BranchRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS11001EditViewModel();

            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit Branch info
        /// </summary>
        /// <param name="model">Branch info to edit</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBranch(PMS11001EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    model.BranchInfo.upd_date = Utility.GetCurrentDateTime();
                    model.BranchInfo.upd_id = loginUser.UserId;
                    model.BranchInfo.company_code = loginUser.CompanyCode;

                    int branchID = 0;

                    if (this.mainService.EditBranchInfo(model.BranchInfo, out branchID))
                    {
                        string action = model.BranchInfo.location_id > 0 ? "更新" : "登録";
                        string message = String.Format(Resources.Messages.I007, "拠点情報", action);
                        var data = this.mainService.GetBranchInfo(loginUser.CompanyCode, branchID);

                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = message,
                                branchID = branchID,
                                insDate = data.ins_date.Value.ToString("yyyy/MM/dd HH:mm"),
                                updDate = data.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                                insUser = data.ins_user,
                                updUser = data.upd_user,
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
                                message = string.Format(Resources.Messages.E045, "拠点情報")
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
                        message = string.Format(Resources.Messages.E045, "拠点情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        #endregion

        #region Ajax Action
        /// <summary>
        /// Search Branch by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by column</param>
        /// <param name="orderType">Order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json list of Branch</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                var pageInfo = this.mainService.Search(model, GetLoginUser().CompanyCode, condition);

                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this.mainService.ExportBranchListToCSV(GetLoginUser().CompanyCode, condition, orderBy, orderType);

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = (from t in pageInfo.Items select new object[]
                        {
                            t.location_id,
                            t.peta_rn,
                            HttpUtility.HtmlEncode(t.location_name),
                            HttpUtility.HtmlEncode(t.display_name),
                            HttpUtility.HtmlEncode(t.display_order),
                            HttpUtility.HtmlEncode(t.remarks),
                            t.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                            HttpUtility.HtmlEncode(t.upd_user),
                            t.del_flg
                        }).ToList()
                    },
                    JsonRequestBehavior.AllowGet);
                SaveRestoreData(condition);
                return result;
            }
            return new EmptyResult();
        }
        #endregion
    }
}
