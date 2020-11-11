#region License
/// <copyright file="PMS04001Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/20</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS04001;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS04001;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with phase
    /// </summary>
    public class PMS04001Controller : ControllerBase
    {
        #region Constructor
        /// Main service
        private readonly IPMS04001Service mainService;

        /// Common service
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS04001Controller(IPMS04001Service service, IPMSCommonService cmService)
        {
            this.mainService = service;
            this.commonService = cmService;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS04001Controller()
            : this(new PMS04001Service(), new PMSCommonService())
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
            if (!this.IsInFunctionList(Constant.FunctionID.PhaseList_Admin)
                && !this.IsInFunctionList(Constant.FunctionID.PhaseList))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS04001ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS04001/Edit") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// Export phase list to csv file
        /// </summary>
        /// <param name="hdnPhaseName">Phase name</param>
        /// <param name="hdnDelFlag">Phase delete flag</param>
        /// <param name="hdnOrderBy">Phase list order by</param>
        /// <param name="hdnOrderType">Phase list order type</param>
        /// <returns>csv file</returns>
        public ActionResult ExportCsv(string hdnPhaseName, bool hdnDelFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            try
            {
                Condition condition = new Condition()
                {
                    PhaseName = hdnPhaseName,
                    DeleteFlag = hdnDelFlag
                };
                List<string> titles = new List<string>()
                {
                    "No.",
                    "フェーズ名",
                    "フェーズ",
                    "備考",
                    "見積対象",
                    "更新日時",
                    "更新者"
                };

                if (string.IsNullOrEmpty(hdnOrderBy))
                    hdnOrderBy = "upd_date";

                string fileName = "PhaseList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";

                IList<PhasePlus> phaseList = new List<PhasePlus>();
                if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
                {
                    phaseList = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<PhasePlus>;
                }
                else
                {
                    phaseList = this.mainService.ExportPhaseListToCSV(GetLoginUser().CompanyCode, condition, hdnOrderBy, hdnOrderType);
                }

                List<PhaseListExportCSV> dataExport = new List<PhaseListExportCSV>();

                foreach (var phaseInfo in phaseList)
                {
                    dataExport.Add(new PhaseListExportCSV()
                    {
                        peta_rn = phaseInfo.peta_rn.ToString(),
                        phase_name = phaseInfo.phase_name,
                        display_name = phaseInfo.display_name,
                        remarks = (string.IsNullOrEmpty(phaseInfo.remarks) ? "" : phaseInfo.remarks.Replace("\r\n", " ")),
                        estimate_target = phaseInfo.estimate_target_flg,
                        upd_date = phaseInfo.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                        upd_user = phaseInfo.upd_user
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
        /// <param name="id">Phase ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!this.IsInFunctionList(Constant.FunctionID.PhaseRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            var model = new PMS04001EditViewModel();

            if (id > 0)
            {
                model.PhaseInfo = this.mainService.GetPhaseInfo(GetLoginUser().CompanyCode, id);
            }

            return this.View("Edit", model);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!IsInFunctionList(Constant.FunctionID.PhaseRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS04001EditViewModel();

            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit phase info
        /// </summary>
        /// <param name="model">Phase info to edit</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPhase(PMS04001EditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();
                    model.PhaseInfo.upd_date = Utility.GetCurrentDateTime();
                    model.PhaseInfo.upd_id = loginUser.UserId;
                    model.PhaseInfo.company_code = loginUser.CompanyCode;

                    // Check limit data by license of company
                    if ((model.PhaseInfo.phase_id == 0
                        || (model.OLD_DEL_FLAG
                        && Constant.DeleteFlag.NON_DELETE.Equals(model.PhaseInfo.del_flg)))
                        && !this.commonService.CheckValidUpdateData(loginUser.CompanyCode, Constant.LicenseDataType.PHASE))
                    {
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 500,
                                message = string.Format(Resources.Messages.E067, "フェーズ")
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }

                    int phaseID = 0;
                    if (this.mainService.EditPhaseInfo(model.PhaseInfo, out phaseID))
                    {
                        string action = model.PhaseInfo.phase_id > 0 ? "更新" : "登録";
                        string message = string.Format(Resources.Messages.I007, "フェーズ情報", action);

                        var data = this.mainService.GetPhaseInfo(loginUser.CompanyCode, phaseID);

                        JsonResult result = Json(
                                new
                                {
                                    statusCode = 201,
                                    message = message,
                                    id = phaseID,
                                    insDate = (data.ins_date != null) ? data.ins_date.Value.ToString("yyyy/MM/dd HH:mm") : "",
                                    updDate = (data.upd_date != null) ? data.upd_date.Value.ToString("yyyy/MM/dd HH:mm") : "",
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
                                        message = string.Format(Resources.Messages.E045, "フェーズ情報")
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
                         message = string.Format(Resources.Messages.E045, "フェーズ情報")
                     },
                     JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        #endregion

        #region Ajax Action
        /// <summary>
        /// Search phase by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Json list of phase</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                var pageInfo = this.mainService.Search(model, GetLoginUser().CompanyCode, condition);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this.mainService.ExportPhaseListToCSV(GetLoginUser().CompanyCode, condition, orderBy, orderType);
                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = (from t in pageInfo.Items select new object[]
                        {
                            t.phase_id,
                            t.peta_rn,
                            HttpUtility.HtmlEncode(t.phase_name),
                            HttpUtility.HtmlEncode(t.display_name),
                            HttpUtility.HtmlEncode(t.remarks),
                            t.estimate_target_flg == "1"?"対象":"",
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
