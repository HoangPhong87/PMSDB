#region License
/// <copyright file="PMS06001Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/08</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06001;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS06001;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with project info
    /// </summary>
    public class PMS06001Controller : ControllerBase
    {
        #region Constructor
        /// Common service
        /// </summary>
        private readonly IPMSCommonService commonService;

        /// Main service
        private readonly IPMS06001Service _service;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS06001Controller(IPMS06001Service service, IPMSCommonService commonservice)
        {
            this._service = service;
            this.commonService = commonservice;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS06001Controller()
            : this(new PMS06001Service(), new PMSCommonService())
        {
        }
        #endregion

        #region Action

        /// <summary>
        /// Download Project plan
        /// </summary>
        /// <param name="project_id">Project ID</param>
        /// <returns>Project plan file</returns>
        public ActionResult DownloadProjectPlan(int project_id)
        {
            ProjectPlanInfoPlus projectPlan = this._service.GetProjectPlanInfo(project_id, GetLoginUser().CompanyCode);

            Utility.DownloadXlsxFile(this, projectPlan, "【○△□案件】プロジェクト計画書.xlsx", "【" + projectPlan.project_name + "案件】プロジェクト計画書.xlsx");
            return new EmptyResult();
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        [HttpGet]
        public ActionResult Index()
        {
            if ((!this.IsInFunctionList(Constant.FunctionID.ProjectList_Admin))
                && (!this.IsInFunctionList(Constant.FunctionID.ProjectList)))
                return this.RedirectToAction("Index", "ErrorAuthent");

            var currentUser = GetLoginUser();

            var model = new PMS06001ListViewModel
            {
                COMPANY_CODE = currentUser.CompanyCode,
                CONTRACT_TYPE_LIST = this.commonService.GetContractTypeSelectList(currentUser.CompanyCode),
                GROUP_LIST = new MultiSelectList(this.commonService.GetUserGroupSelectList(currentUser.CompanyCode), "Value", "Text"),
                TAG_LIST = new List<SelectListItem>(),
                STATUS_SELECT_LIST = this.commonService.GetStatusSelectList(currentUser.CompanyCode)
            };

            if (currentUser.GroupId.HasValue)
            {
                model.Condition.GROUP_ID = new List<string>() { currentUser.GroupId.ToString() };
            }

            // Get Jquery data table state
            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                var tmpCondition = GetRestoreData() as Condition;

                if (tmpCondition != null)
                    model.Condition = tmpCondition;
            }

            if (model.Condition.CUSTOMER_ID != null)
            {
                model.TAG_LIST = this.commonService.GetTagSelectList(currentUser.CompanyCode, model.Condition.CUSTOMER_ID.Value);
            }

            Session["PMS06001_Plan_From"] = "ProjectList";

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("List", model);
        }

        /// <summary>
        /// Export project list to csv file
        /// </summary>
        /// <param name="hdnProjectName">Project name</param>
        /// <param name="hdnFrom">Project start from</param>
        /// <param name="hdnTo">Project start to</param>
        /// <param name="hdnCustomerId">Customer ID</param>
        /// <param name="hdnContractTypeId">Contract type ID</param>
        /// <param name="hdnGroupId">User group ID</param>
        /// <param name="hdnStatusId">Project status ID</param>
        /// <param name="hdnDelFlag">Project delete flag</param>
        /// <param name="hdnOrderBy">Project list order by</param>
        /// <param name="hdnOrderType">Project list order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>csv file</returns>
        public ActionResult ExportProjectListToCSV(
            string hdnProjectName,
            int hdnTimeConditionType,
            string hdnFrom,
            string hdnTo,
            int? hdnCustomerId,
            int? hdnChargePersonId,
            int? hdnTagId,
            int? hdnContractTypeId,
            string hdnGroupId,
            int? hdnStatusId,
            int? hdnCompleteId,
            bool hdnDelFlag,
            string hdnOrderBy,
            string hdnOrderType,
            string TAB_ID
            )
        {
            LoginUser currentUser = GetLoginUser();
            string[] groupIdArr = hdnGroupId.Split(',');
            List<string> groupId = groupIdArr.ToList();

            Condition condition = new Condition()
            {
                PROJECT_NAME = hdnProjectName,
                TIME_CONDITION_TYPE = hdnTimeConditionType,
                FROM_DATE = hdnFrom,
                TO_DATE = hdnTo,
                CUSTOMER_ID = hdnCustomerId,
                CHARGE_PERSON_ID = hdnChargePersonId,
                TAG_ID = hdnTagId,
                CONTRACT_TYPE_ID = hdnContractTypeId,
                GROUP_ID = groupId,
                STATUS_ID = hdnStatusId,
                COMPLETE_ID = hdnCompleteId,
                DELETE_FLG = hdnDelFlag
            };
            List<string> titles = new List<string>()
            {
                "プロジェクト名",
                "プロジェクトNo",
                "タグ名",
                "契約種別",
                "ランク",
                "開始日",
                "納品日",
                "検収日",
                "担当者",
                "営業担当者",
                "ステータス",
                "受注金額",
                "支払金額",
                "工数予定",
                "工数実績",
                "進捗",
                "利益予定",
                "利益実績",
                "発注元",
                "備考",
                "更新日時",
                "更新者"
            };

            if (string.IsNullOrEmpty(hdnOrderBy))
                hdnOrderBy = "status_order";

            string fileName = "ProjectList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            IList<ProjectInfoPlus> projectList = new List<ProjectInfoPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                projectList = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<ProjectInfoPlus>;
            }
            else
            {
                projectList = this._service.ExportProjectListToCSV(currentUser.CompanyCode, condition, hdnOrderBy, hdnOrderType);
            }

            List<ProjectListExportCSV> dataExport = new List<ProjectListExportCSV>();

            foreach (var projectInfo in projectList)
            {
                dataExport.Add(new ProjectListExportCSV()
                {
                    project_name = projectInfo.project_name,
                    project_no = projectInfo.project_no,
                    tag_name = projectInfo.tag_name,
                    contract_type = projectInfo.contract_type,
                    rank = projectInfo.rank,
                    start_date = projectInfo.start_date.Value.ToString("yyyy/MM/dd"),
                    end_date = projectInfo.end_date.Value.ToString("yyyy/MM/dd"),
                    acceptance_date = projectInfo.acceptance_date != null ? projectInfo.acceptance_date.Value.ToString("yyyy/MM/dd") : string.Empty,
                    charge_person = projectInfo.charge_person,
                    charge_of_sales_person = projectInfo.charge_of_sales,
                    status = projectInfo.status,
                    total_sales = projectInfo.total_sales.ToString("#,##0"),
                    total_payment = (projectInfo.total_payment != null) ? projectInfo.total_payment.Value.ToString("#,##0") : "0",
                    estimate_man_days = projectInfo.estimate_man_days + "人日",
                    actual_man_day = projectInfo.actual_man_day.ToString("#,##0.00") + "人日",
                    progress = Convert.ToInt32(projectInfo.progress != null ? (projectInfo.progress * 100) : 0) + "%",
                    plan_profit = (projectInfo.plan_profit * 100).ToString("#,##0.00") + "%",
                    actual_profit = (projectInfo.actual_profit * 100).ToString("#,##0.00") + "%",
                    customer_name = projectInfo.customer_name,
                    remarks = string.IsNullOrEmpty(projectInfo.remarks) ? string.Empty : projectInfo.remarks.Replace("\r", "").Replace("\r\n", "").Replace("\n", ""),
                    upd_date = projectInfo.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                    upd_user = projectInfo.upd_user
                });
            }

            DataTable dt = Utility.ToDataTableT(dataExport, titles.ToArray());
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }

        /// <summary>
        /// Edit action POST - to hide project ID
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="isLinkToCopy">Is create copy info</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(string id, bool? isLinkToCopy, string isNotBack)
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ProjectRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            if (string.IsNullOrEmpty(id))
                id = Constant.DEFAULT_VALUE;

            int projectID = Convert.ToInt32(id);
            bool isCreateCopy = isLinkToCopy != null ? true : false;
            var model = this.MakeEditViewModel(projectID, isCreateCopy, Constant.CopyType.NORMAL);

            if (isNotBack == "EditByGet")
            {
                model.isNotBack = "EditByGet";
            }

            if (isNotBack == "EditFromTop")
            {
                model.isNotBack = "EditFromTop";
            }

            Session["PMS06001_Plan_From"] = "ProjectEdit";
            return this.View("Edit", model);
        }

        /// <summary>
        /// Edit action GET - by project no
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                if (!this.IsInFunctionList(Constant.FunctionID.ProjectRegist))
                    return this.RedirectToAction("Index", "ErrorAuthent");

                int projectID = 0;
                if (string.IsNullOrEmpty(id))
                {
                    return this.RedirectToAction("Index", "ErrorAuthent");
                }
                else
                {
                    var companyCode = GetLoginUser().CompanyCode;
                    var projectSysId = this.commonService.getProjectIdByProjectNo(id, companyCode);

                    if (string.IsNullOrEmpty(projectSysId))
                    {
                        return this.RedirectToAction("Index", "ErrorAuthent");
                    }
                    else
                    {
                        projectID = Convert.ToInt32(projectSysId);
                    }
                }

                var model = this.MakeEditViewModel(projectID, false, Constant.CopyType.NORMAL);
                if (projectID > 0)
                    model.isNotBack = "EditByGet";
                return this.View("Edit", model);
            }
            catch
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

        }

        /// <summary>
        /// POST action - Copy to create
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="isLinkToCopy">Is create copy info</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult New(string id, bool? isLinkToCopy, int? copyType)
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ProjectRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            if (string.IsNullOrEmpty(id))
                id = Constant.DEFAULT_VALUE;

            int projectID = Convert.ToInt32(id);
            bool isCreateCopy = isLinkToCopy != null ? true : false;
            int copyProjectType = copyType != null && copyType == Constant.CopyType.ALL_INFORMATION ? Constant.CopyType.ALL_INFORMATION : Constant.CopyType.NORMAL;

            var model = this.MakeEditViewModel(projectID, isCreateCopy, copyProjectType);
            Session["PMS06001_Plan_From"] = "ProjectEdit";
            return this.View("Edit", model);
        }

        /// <summary>
        /// GET - only create new
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult New()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ProjectRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            var model = this.MakeEditViewModel(0, false, Constant.CopyType.NORMAL);

            return this.View("Edit", model);
        }

        /// <summary>
        ///  Delete project
        /// </summary>
        /// <param name="dataListProjectId">Project ID list</param>
        /// <returns>Result</returns>
        [HttpPost]
        public ActionResult DeleteProject(IList<string> dataListProjectId)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (dataListProjectId == null || dataListProjectId.Count == 0)
            {
                return new EmptyResult();
            }

            var result = this._service.DeleteProject(dataListProjectId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update status project
        /// </summary>
        /// <param name="dataListProjectId">Project ID list</param>
        /// <param name="statusID">Selected status ID</param>
        /// <returns>Result</returns>
        [HttpPost]
        public ActionResult UpdateStatusProject(IList<string> dataListProjectId, int statusID)
        {
            if (!Request.IsAjaxRequest() || dataListProjectId == null || dataListProjectId.Count == 0 || statusID == 0)
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            var result = this._service.UpdateStatusProject(dataListProjectId, statusID, GetLoginUser().UserId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action add/edit
        /// </summary>
        /// <param name="model">Project info</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProject(PMS06001EditViewModel model)
        {
            try
            {
                // Check validation of data
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();
                    bool existPhase = false;

                    foreach (var phase in model.PHASE_LIST)
                    {
                        if (phase.check)
                        {
                            existPhase = true;
                            break;
                        }
                    }

                    // Check not exist target phase
                    if (!existPhase)
                    {
                        ModelState.AddModelError(string.Empty, string.Format(Resources.Messages.E017, "対象フェーズ"));
                        return new EmptyResult();
                    }
                    else
                    {
                        model.PROJECT_INFO.company_code = loginUser.CompanyCode;
                        model.PROJECT_INFO.upd_date = Utility.GetCurrentDateTime();
                        model.PROJECT_INFO.upd_id = loginUser.UserId;
                        model.PROJECT_INFO.tax_rate = model.PROJECT_INFO.tax_rate / 100;
                        model.OUTSOURCER.total_amount = model.PROJECT_INFO.total_sales;
                        model.OUTSOURCER_LIST.Add(model.OUTSOURCER);
                        if ((!model.old_definite_assign_date && model.definite_assign_date)
                            || (model.old_definite_assign_date && model.definite_assign_date && model.IS_UPDATE_ASSIGN_DATE))
                        {
                            model.PROJECT_INFO.assign_fix_date = model.PROJECT_INFO.upd_date;
                        }
                        else if (model.old_definite_assign_date && !model.definite_assign_date)
                        {
                            model.PROJECT_INFO.assign_fix_date = null;
                        }

                        bool allowRegistHistory = (model.PROJECT_INFO.sales_type.Equals(Constant.DEFAULT_VALUE) && model.IS_CHANGE_HISTORY) ? true : false;
                        string errMsg = string.Empty;
                        int projectID = 0;
                        string action = model.PROJECT_INFO.project_sys_id > 0 ? "更新" : "登録";
                        string IsNotBack = model.isNotBack;

                        if (this._service.EditProjectInfo( // Edit
                            model.PROJECT_INFO,
                            model.PHASE_LIST,
                            model.TARGET_CATEGORY_LIST,
                            model.OUTSOURCER_LIST,
                            model.SUBCONTRACTOR_LIST,
                            model.PAYMENT_DETAIL_LIST,
                            model.OVERHEAD_COST_LIST,
                            model.OVERHEAD_COST_DETAIL_LIST,
                            model.MEMBER_ASSIGNMENT_LIST,
                            model.MEMBER_ASSIGNMENT_DETAIL_LIST,
                            model.PROGRESS_LIST,
                            model.FILE_LIST,
                            allowRegistHistory,
                            out projectID,
                            out errMsg))
                        {
                            var data = this._service.GetProjectInfo(loginUser.CompanyCode, projectID, false, Constant.CopyType.NORMAL);

                            string message = string.Format(Resources.Messages.I007, "プロジェクト情報", action);

                            JsonResult result = Json(
                                new
                                {
                                    statusCode = 201,
                                    message = message,
                                    projectID = projectID,
                                    IsNotBack = IsNotBack,
                                    rowVersion = Convert.ToBase64String(data.row_version),
                                    projectNo = data.project_no,
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
                            if (model.PROJECT_INFO.project_sys_id > 0) // Duplicate action update
                            {
                                JsonResult result = Json(
                                    new
                                    {
                                        statusCode = 202,
                                        IsNotBack = IsNotBack,
                                        message = string.Format(Resources.Messages.E031),
                                        projectID = projectID
                                    },
                                    JsonRequestBehavior.AllowGet);

                                return result;
                            }
                            else // Has error when regist project info
                            {
                                JsonResult result = Json(
                                    new
                                    {
                                        statusCode = 500,
                                        message = string.Format(Resources.Messages.E045, "プロジェクト情報")
                                    },
                                    JsonRequestBehavior.AllowGet);

                                return result;
                            }
                        }
                    }
                }

                return new EmptyResult();
            }
            catch (Exception)
            {
                JsonResult result = Json(
                    new
                    {
                        statusCode = 500,
                        message = string.Format(Resources.Messages.E045, "プロジェクト情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        /// <summary>
        /// Detail action
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ProjectDetail))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS06001/ActualWork")
                || Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS06001/Plan"))
            {
                string companyCode = GetLoginUser().CompanyCode;
                var id = Session["PMS06001_Detail_ID"] as string;
                var selected_year = Session["PMS06001_Detail_Year"] as int?;
                var selected_month = Session["PMS06001_Detail_Month"] as int?;
                int projectID = Convert.ToInt32(id);
                var model = new PMS06001DetailViewModel();

                model.PROJECT_INFO = this._service.GetProjectInfo(companyCode, projectID, false, Constant.CopyType.NORMAL);
                model.PROJECT_INFO.progress = model.PROJECT_INFO.progress != null ? Convert.ToInt32(model.PROJECT_INFO.progress.Value * 100) : 0;
                model.SALES = model.PROJECT_INFO.total_sales.ToString("#,##0");
                model.PAYMENT = model.PROJECT_INFO.total_payment != null ? model.PROJECT_INFO.total_payment.Value.ToString("#,##0") : Constant.DEFAULT_VALUE;
                model.PLAN_PROFIT = (model.PROJECT_INFO.total_sales - (model.PROJECT_INFO.total_payment != null ? model.PROJECT_INFO.total_payment.Value : 0) - model.PROJECT_INFO.total_plan_cost).ToString("#,##0") + " 円";
                model.ACTUAL_PROFIT = (model.PROJECT_INFO.total_sales - (model.PROJECT_INFO.total_payment != null ? model.PROJECT_INFO.total_payment.Value : 0) - model.PROJECT_INFO.actual_cost).ToString("#,##0") + " 円";
                model.PLAN_PROFIT_RATE = (model.PROJECT_INFO.plan_profit * 100).ToString("#,##0.00") + "%";
                model.ACTUAL_PROFIT_RATE = (model.PROJECT_INFO.actual_profit * 100).ToString("#,##0.00") + "%";
                model.selected_year = selected_year ?? -1;
                model.selected_month = selected_month ?? -1;

                var timeUnit = Session["PMS06001_Summary_TimeUnit"] as string;
                model.TIME_UNIT = timeUnit;
                model.PHASE_LIST = this._service.GetWorkTimeByPhase(companyCode, projectID, model.TIME_UNIT);
                return this.View("Detail", model);
            }
            else
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Detail action
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="selected_year">Select year</param>
        /// <param name="selected_month">Select month</param>
        /// <returns>Detail view</returns>
        [HttpPost]
        public ActionResult Detail(string id = "", int selected_year = -1, int selected_month = -1)
        {
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS06003/AssignmentByProject"))
            {
                Session["PMS06001_ProjectRecord_From"] = "AssignmentByProject";
            }
            else if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS06001"))
            {
                Session["PMS06001_ProjectRecord_From"] = "ProjectList";
            }
            else if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS06002/Detail"))
            {
                Session["PMS06001_ProjectRecord_From"] = "PersonalRecord";
            }

            if (!this.IsInFunctionList(Constant.FunctionID.ProjectDetail))
                return this.RedirectToAction("Index", "ErrorAuthent");

            if (string.IsNullOrEmpty(id))
                return this.RedirectToAction("Index", "Error");
            Session["PMS06001_Detail_ID"] = id;
            Session["PMS06001_Detail_Year"] = selected_year;
            Session["PMS06001_Detail_Month"] = selected_month;
            string companyCode = GetLoginUser().CompanyCode;
            int projectID = Convert.ToInt32(id);
            var model = new PMS06001DetailViewModel();

            model.PROJECT_INFO = this._service.GetProjectInfo(companyCode, projectID, false, Constant.CopyType.NORMAL);
            model.PROJECT_INFO.progress = model.PROJECT_INFO.progress != null ? Convert.ToInt32(model.PROJECT_INFO.progress.Value * 100) : 0;
            model.SALES = model.PROJECT_INFO.total_sales.ToString("#,##0");
            model.PAYMENT = model.PROJECT_INFO.total_payment != null ? model.PROJECT_INFO.total_payment.Value.ToString("#,##0") : Constant.DEFAULT_VALUE;
            model.PLAN_PROFIT = (model.PROJECT_INFO.total_sales - (model.PROJECT_INFO.total_payment != null ? model.PROJECT_INFO.total_payment.Value : 0) - model.PROJECT_INFO.total_plan_cost).ToString("#,##0") + " 円";
            model.ACTUAL_PROFIT = (model.PROJECT_INFO.total_sales - (model.PROJECT_INFO.total_payment != null ? model.PROJECT_INFO.total_payment.Value : 0) - model.PROJECT_INFO.actual_cost).ToString("#,##0") + " 円";
            model.PLAN_PROFIT_RATE = (model.PROJECT_INFO.plan_profit * 100).ToString("#,##0.00") + "%";
            model.ACTUAL_PROFIT_RATE = (model.PROJECT_INFO.actual_profit * 100).ToString("#,##0.00") + "%";
            //model.TOTAL_ACTUAL_EFFORT = model.PROJECT_INFO.actual_man_day.ToString("#,##0.00") + "人日";
            //model.TOTAL_PLAN_EFFORT = model.PROJECT_INFO.total_plan_man_days.ToString("#,##0.00") + "人日";
            model.selected_year = selected_year;
            model.selected_month = selected_month;
            model.PHASE_LIST = this._service.GetWorkTimeByPhase(companyCode, projectID, Constant.TimeUnit.DAY);
            Session["PMS06001_Plan_From"] = "ProjectRecord";
            return this.View("Detail", model);
        }

        /// <summary>
        /// Actual work of member in project by target phase
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="userId">User ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualWork(int projectId = 0, int userId = 0)
        {
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS06001/Detail"))
            {
                Session["PMS06001_Actual_Work_From"] = "Detail";
            }
            else
            {
                Session["PMS06001_Actual_Work_From"] = "ActualWorkListByIndividualPhase";
            }

            if (!this.IsInFunctionList(Constant.FunctionID.ProjectDetail))
                return this.RedirectToAction("Index", "ErrorAuthent");

            if (projectId == 0 || userId == 0)
                return this.RedirectToAction("Index", "Error");

            string companyCode = GetLoginUser().CompanyCode;
            var info = this._service.GetActualWorkDetailInfo(companyCode, projectId, userId);

            var model = new PMS06001ActualWorkViewModel
            {
                PROJECT_ID = projectId,
                USER_ID = userId,
                PROJECT_NAME = info.project_name,
                DURATION = info.start_date.Value.Year.ToString() + "年" + info.start_date.Value.ToString("MM") + "月~" + info.end_date.Value.Year.ToString() + "年" + info.end_date.Value.ToString("MM") + "月",
                GROUP_NAME = info.group_name,
                USER_NAME = info.charge_person,
                FROM = info.start_date.Value.ToString("yyyy/MM/dd"),
                TO = info.end_date.Value.ToString("yyyy/MM/dd")
            };

            return this.View("ActualWork", model);
        }

        /// <summary>
        /// Export data to Csv file
        /// </summary>
        /// <param name="startDate">Project start date</param>
        /// <param name="endDate">Project end date</param>
        /// <param name="projectId">Project ID</param>
        /// <param name="timeUnit">Time unit</param>
        /// <returns>EmptyResult</returns>
        public ActionResult ExportCsv(DateTime startDate, DateTime endDate, int projectId, string timeUnit, int sortCol, string sortType)
        {
            DetailCondition condition = new DetailCondition
            {
                PROJECT_ID = projectId,
                FROM_DATE = startDate,
                TO_DATE = endDate,
                TIME_UNIT = timeUnit
            };
            List<string> colums = this.GetMonthList(condition.FROM_DATE, condition.TO_DATE);
            List<string> titles = new List<string>()
            {
                "No.",
                "所属",
                "ユーザー名",
                "実績工数／予定工数"
            };

            foreach (var item in colums)
            {
                titles.Add(item);
            }

            titles.Add("原価/売上金額");

            foreach (var item in colums)
            {
                titles.Add(item);
            }

            string companyCode = GetLoginUser().CompanyCode;
            DataTablesModel model = new DataTablesModel
            {
                sEcho = "1",
                iColumns = 5,
                sColumns = "member_name,member_name,group_name,member_name,member_name",
                iDisplayStart = 0,
                iDisplayLength = int.MaxValue,
                iSortCol_0 = sortCol,
                sSortDir_0 = sortType,
                iSortingCols = 1
            };

            var pageInfoEffort = this._service.GetProjectMemberDetail(model, companyCode, condition);
            var pageInfoProfit = this._service.GetProjectMemberProfitDetail(model, companyCode, condition);
            IList<object[]> resultList = new List<object[]>();

            int countMonth = ((condition.TO_DATE.Year - condition.FROM_DATE.Year) * 12) + condition.TO_DATE.Month - condition.FROM_DATE.Month;

            for (int i = 0; i < pageInfoEffort.Items.Count; i++)
            {
                IList<string> dataItem = new List<string>();
                var dataEffort = pageInfoEffort.Items.ElementAt(i);
                dataItem.Add(((IDictionary<string, object>)dataEffort).ToList()[0].Value.ToString()); // index
                dataItem.Add((string)((IDictionary<string, object>)dataEffort).ToList()[2].Value); // group
                dataItem.Add((string)((IDictionary<string, object>)dataEffort).ToList()[3].Value); // user

                decimal totalActual = 0;
                decimal totalPlan = 0;

                for (int j = 4; j <= (countMonth + 4); j++)
                {
                    var res = this.FormatData(((IDictionary<string, object>)dataEffort).ToList()[j].Value);
                    var resArr = res.Split('/');

                    dataItem.Add(res); // actual/plan by month
                    totalActual += Convert.ToDecimal(resArr[0]);
                    totalPlan += Convert.ToDecimal(resArr[1]);
                }

                var dataProfit = pageInfoProfit.Items.ElementAt(i);
                Int64 totalCost = 0;
                Int64 totalIndvSales = 0;

                for (int k = 4; k <= (countMonth + 4); k++)
                {
                    var res = ((IDictionary<string, object>)dataProfit).ToList()[k].Value.ToString();
                    var resArr = res.Split('/');

                    dataItem.Add(resArr[0] + "円/" + resArr[1] + "円"); // cost/individual sales by month
                    totalCost += Convert.ToInt64(resArr[0]);
                    totalIndvSales += Convert.ToInt64(resArr[1]);
                }
                string totalActualPlan = totalActual.ToString("#,##0.00") + "/" + totalPlan.ToString("#,##0.00");
                dataItem.Insert(3, totalActualPlan); // total actual/total plan
                string totalCostIndvSales = totalCost + "円/" + totalIndvSales + "円";
                dataItem.Insert(5 + countMonth, totalCostIndvSales);
                resultList.Add(dataItem.ToArray()); // add data row
            }

            string fileName = "ProjectDetail_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            DataTable dt = Utility.ToDateTable(resultList, titles.ToArray());
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }

        /// <summary>
        /// Show history of project info
        /// </summary>
        /// <returns>History view</returns>
        [HttpGet]
        public ActionResult History()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ProjectRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            return this.View("History");
        }

        /// <summary>
        /// Clear save condition
        /// </summary>
        /// <returns>Index</returns>
        [HttpGet]
        public ActionResult ClearSaveCondition()
        {
            return this.RedirectToAction("Index");
        }

        #endregion

        #region Ajax Action
        /// <summary>
        /// Search project by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <param name="orderBy">order by column</param>
        /// <param name="orderType">order type</param>
        /// <returns>Json list information</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string TAB_ID, string orderBy, string orderType)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    decimal totalOrder = 0;
                    decimal totalPayment = 0;
                    decimal totalCost = 0;
                    decimal totalActualCost = 0;
                    decimal totalProfit = 0;
                    decimal profitRate = 0;
                    decimal totalActualProfit = 0;
                    decimal actualProfitRate = 0;

                    condition.sortCol = model.iSortCol_0;
                    condition.sortDir = model.sSortDir_0;

                    LoginUser currentUser = GetLoginUser();
                    var projectList = this._service.Search(model, currentUser.CompanyCode, condition);
                    var allProject = this._service.ExportProjectListToCSV(currentUser.CompanyCode, condition, orderBy, orderType);
                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = allProject;

                    if (allProject.Count > 0)
                    {
                        foreach (var item in allProject)
                        {
                            totalOrder += item.total_sales;
                            totalPayment += item.total_payment != null ? item.total_payment.Value : 0;
                            totalCost += item.total_plan_cost;
                            totalActualCost += item.actual_cost;
                        }

                        totalProfit = totalOrder - totalPayment - totalCost;
                        totalActualProfit = totalOrder - totalPayment - totalActualCost;
                        if (totalOrder != 0)
                        {
                            profitRate = totalProfit / totalOrder * 100;
                            actualProfitRate = totalActualProfit / totalOrder * 100;
                        }
                    }

                    IList<object> dataList = new List<object>();

                    foreach (var data in projectList)
                    {
                        var totalPlanCostEach = Utility.RoundNumber(data.total_plan_cost, currentUser.DecimalCalculationType, false);
                        var totalActualCostEach = Utility.RoundNumber(data.actual_cost, currentUser.DecimalCalculationType, false);

                        var totalPaymentEach = data.total_payment != null ? data.total_payment.Value : 0;

                        var actual_profit_amount = data.total_sales - totalPaymentEach - totalActualCostEach;
                        var plan_profit_amount = data.total_sales - totalPaymentEach - totalPlanCostEach;

                        dataList.Add(new object[] {
                            data.project_sys_id,
                            data.upd_date,
                            HttpUtility.HtmlEncode(data.project_name),
                            HttpUtility.HtmlEncode(data.contract_type),
                            HttpUtility.HtmlEncode(data.rank),
                            data.start_date.Value.ToString("yyyy/MM/dd"),
                            data.end_date.Value.ToString("yyyy/MM/dd"),
                            data.acceptance_date != null ? data.acceptance_date.Value.ToString("yyyy/MM/dd") : string.Empty,
                            HttpUtility.HtmlEncode(data.charge_person),
                            HttpUtility.HtmlEncode(data.status),
                            data.total_sales.ToString("#,##0") + " 円",
                            (data.assign_fix_date != null ? "*" : "") + data.estimate_man_days + "人日",
                            data.actual_man_day.ToString("#,##0.00")  + "人日",
                            Convert.ToInt32(data.progress != null ? (data.progress * 100) : 0) + "%",

                            Utility.RoundNumber(plan_profit_amount, currentUser.DecimalCalculationType, false).ToString("#,##0") + " 円<br/>"+ (data.plan_profit * 100).ToString("#,##0.00") + "%",
                            Utility.RoundNumber(actual_profit_amount, currentUser.DecimalCalculationType, false).ToString("#,##0") + " 円<br/>"+ (data.actual_profit * 100).ToString("#,##0.00") + "%",

                            HttpUtility.HtmlEncode(data.customer_name),
                            data.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                            HttpUtility.HtmlEncode(data.upd_user),
                            data.del_flg,
                            (Convert.ToInt32(data.sales_type) > 0 ?
                                "" : ((data.estimate_man_days > data.history_estimate_man_days) ?
                                    "rise" : (data.history_estimate_man_days > data.estimate_man_days) ?
                                        "reduce" : ""
                                )
                            ),
                            (Convert.ToInt32(data.sales_type) > 0 ?
                                "" : ((data.total_sales > data.history_total_sales) ?
                                    "rise" : (data.history_total_sales > data.total_sales) ?
                                        "reduce" : ""
                                )
                            ),
                            (Convert.ToInt32(data.sales_type) > 0 ?
                                "" : ((Convert.ToDecimal((data.plan_profit * 100).ToString("#,##0.00")) > Convert.ToDecimal(Convert.ToDecimal(data.history_total_sales == 0 ? 0 : data.history_gross_profit/data.history_total_sales * 100).ToString("#,##0.00"))) ?
                                    "rise" : (Convert.ToDecimal(Convert.ToDecimal(data.history_total_sales == 0 ? 0 : data.history_gross_profit/data.history_total_sales * 100).ToString("#,##0.00")) > Convert.ToDecimal((data.plan_profit * 100).ToString("#,##0.00"))) ?
                                        "reduce" : ""
                                )
                            ),
                            data.progress_regist_date != null ? data.progress_regist_date.Value.ToString("yyyy/MM/dd") : string.Empty,
                            data.count_plan,
                            data.project_no
                        });
                    }

                    var result = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = allProject.Count,
                            iTotalDisplayRecords = allProject.Count,
                            aaData = dataList,
                            condition = condition,
                            totalOrder = totalOrder.ToString("#,##0") + " 円",
                            totalPayment = totalPayment.ToString("#,##0") + " 円",
                            totalCost = Utility.RoundNumber(totalCost, currentUser.DecimalCalculationType, false).ToString("#,##0") + " 円",
                            totalProfit = Utility.RoundNumber(totalProfit, currentUser.DecimalCalculationType, false).ToString("#,##0") + " 円",
                            profitRate = profitRate.ToString("#,##0.00") + "%",
                            totalActualProfit = Utility.RoundNumber(totalActualProfit, currentUser.DecimalCalculationType, false).ToString("#,##0") + " 円",
                            actualProfitRate = actualProfitRate.ToString("#,##0.00") + "%"
                        },
                        JsonRequestBehavior.AllowGet);

                    // save search condition
                    SaveRestoreData(condition);

                    return result;
                }
            }

            return new EmptyResult();
        }


        /// <summary>
        /// Project Detail summary data
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="projectId">Project ID</param>
        /// <param name="timeUnit">Time unit</param>
        /// <param name="projectStartDate">Project start date</param>
        /// <param name="projectEndDate">Project end date</param>
        /// <returns>Member work data </returns>
        public ActionResult Summary(
            DataTablesModel model,
            DetailCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                string companyCode = GetLoginUser().CompanyCode;
                var isFromActualWork = Session["PMS06001_IsFromActualWork"] as string;
                Session["PMS06001_Summary_TimeUnit"] = condition.TIME_UNIT;
                IList<TargetPhasePlus> phaseList = new List<TargetPhasePlus>();

                phaseList = this._service.GetWorkTimeByPhase(companyCode, condition.PROJECT_ID, condition.TIME_UNIT);

                var pageInfoEffort = this._service.GetProjectMemberDetail(model, companyCode, condition);

                if (condition.MODE == "Effort")
                {
                    #region get effort actual/plan data
                    IList<object> leftList = new List<object>();
                    IList<object> rightList = new List<object>();
                    decimal projectActualEffort = 0;
                    decimal projectPlanEffort = 0;

                    int countMonth = ((condition.TO_DATE.Year - condition.FROM_DATE.Year) * 12) + condition.TO_DATE.Month - condition.FROM_DATE.Month;

                    foreach (var data in pageInfoEffort.Items)
                    {
                        IList<string> leftItem = new List<string>();
                        IList<string> rightItem = new List<string>();
                        string userID = ((IDictionary<string, object>)data).ToList()[1].Value.ToString();

                        leftItem.Add(userID); // user ID
                        leftItem.Add(((IDictionary<string, object>)data).ToList()[0].Value.ToString()); // index
                        leftItem.Add(this.EncodeData(((IDictionary<string, object>)data).ToList()[2].Value)); // group name
                        leftItem.Add(this.EncodeData(((IDictionary<string, object>)data).ToList()[3].Value)); // user name

                        decimal totalActual = 0;
                        decimal totalPlan = 0;

                        for (int i = 4; i <= (countMonth + 4); i++)
                        {
                            var res = this.FormatData(((IDictionary<string, object>)data).ToList()[i].Value);
                            var resArr = res.Split('/');
                            rightItem.Add(res); // actual/plan by month
                            totalActual += Convert.ToDecimal(resArr[0]);
                            totalPlan += Convert.ToDecimal(resArr[1]);
                        }
                        string totalActualPlan = string.Empty;
                        totalActualPlan = "<a href='#' class='view-detail' alt='" + userID + "'>" + totalActual.ToString("#,##0.00") + "/" + totalPlan.ToString("#,##0.00") + "</a>";
                        leftItem.Add(totalActualPlan); // total actual/total plan
                        leftList.Add(leftItem); // add data row left side

                        rightList.Add(rightItem); // add data row right side

                        projectActualEffort += Convert.ToDecimal(totalActual.ToString("#,##0.00"));
                        projectPlanEffort += Convert.ToDecimal(totalPlan.ToString("#,##0.00"));
                    }
                    var result =
                        Json(data: new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = pageInfoEffort.TotalItems,
                            iTotalDisplayRecords = pageInfoEffort.TotalItems,
                            aaData = leftList,
                            rightList = rightList,
                            phaseList = phaseList,
                            projectActualEffort = projectActualEffort,
                            projectPlanEffort = projectPlanEffort
                        });
                    return result;
                    #endregion
                }
                else
                {
                    #region get profit actual/plan data
                    var pageInfoProfit = this._service.GetProjectMemberProfitDetail(model, companyCode, condition);
                    IList<object> leftList = new List<object>();
                    IList<object> rightList = new List<object>();
                    decimal projectActualEffort = 0;
                    decimal projectPlanEffort = 0;

                    int countMonth = ((condition.TO_DATE.Year - condition.FROM_DATE.Year) * 12) + condition.TO_DATE.Month - condition.FROM_DATE.Month;

                    foreach (var data in pageInfoEffort.Items)
                    {
                        decimal totalActual = 0;
                        decimal totalPlan = 0;

                        for (int i = 4; i <= (countMonth + 4); i++)
                        {
                            var res = this.FormatData(((IDictionary<string, object>)data).ToList()[i].Value);
                            var resArr = res.Split('/');
                            totalActual += Convert.ToDecimal(resArr[0]);
                            totalPlan += Convert.ToDecimal(resArr[1]);
                        }

                        projectActualEffort += Convert.ToDecimal(totalActual.ToString("#,##0.00"));
                        projectPlanEffort += Convert.ToDecimal(totalPlan.ToString("#,##0.00"));
                    }

                    foreach (var data in pageInfoProfit.Items)
                    {
                        IList<string> leftItem = new List<string>();
                        IList<string> rightItem = new List<string>();
                        string userID = ((IDictionary<string, object>)data).ToList()[1].Value.ToString();

                        leftItem.Add(userID); // user ID
                        leftItem.Add(((IDictionary<string, object>)data).ToList()[0].Value.ToString()); // index
                        leftItem.Add(this.EncodeData(((IDictionary<string, object>)data).ToList()[2].Value)); // group name
                        leftItem.Add(this.EncodeData(((IDictionary<string, object>)data).ToList()[3].Value)); // user name

                        decimal totalCost = 0;
                        decimal totalIndvSales = 0;

                        for (int i = 4; i <= (countMonth + 4); i++)
                        {
                            var res = this.FormatData(((IDictionary<string, object>)data).ToList()[i].Value);
                            var resArr = res.Split('/');

                            var cost = Convert.ToDecimal(resArr[0]);
                            var indvSales = Convert.ToDecimal(resArr[1]);
                            res = string.Empty;
                            res += (cost > 0) ? String.Format("{0:#,0}", cost) : "0";
                            res += "円/";
                            res += (indvSales > 0) ? String.Format("{0:#,0}", indvSales) : "0";
                            res += "円";
                            rightItem.Add(res);
                            totalCost += Convert.ToDecimal(resArr[0]);
                            totalIndvSales += Convert.ToDecimal(resArr[1]);
                        }

                        string totalProfitData = "<span class='total-profit'>";
                        totalProfitData += (totalCost > 0) ? String.Format("{0:#,0}", totalCost) : "0";
                        totalProfitData += "円/<br/>";
                        totalProfitData += (totalIndvSales > 0) ? String.Format("{0:#,0}", totalIndvSales) : "0";
                        totalProfitData += "円</span>";

                        leftItem.Add(totalProfitData);
                        leftList.Add(leftItem); // add data row left side

                        rightList.Add(rightItem); // add data row right side
                    }
                    var result =
                    Json(data: new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfoProfit.TotalItems,
                        iTotalDisplayRecords = pageInfoProfit.TotalItems,
                        aaData = leftList,
                        rightList = rightList,
                        phaseList = phaseList,
                        projectActualEffort = projectActualEffort,
                        projectPlanEffort = projectPlanEffort
                    });
                    return result;
                    #endregion
                }
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Show actual work of member in project
        /// </summary>
        /// <param name="model">Jquery data table model</param>
        /// <param name="condition">search condition</param>
        /// <returns></returns>
        public ActionResult SearchActualWork(
            DataTablesModel model,
            ActualWorkCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                string companyCode = GetLoginUser().CompanyCode;
                var pageInfo = this._service.GetUserWorkTimeByPhase(model, companyCode, condition);

                IList<object> leftList = new List<object>();
                IList<object> rightList = new List<object>();

                int countMonth = ((condition.TO_DATE.Year - condition.FROM_DATE.Year) * 12) + condition.TO_DATE.Month - condition.FROM_DATE.Month;

                foreach (var data in pageInfo.Items)
                {
                    decimal totalActualWork = 0;
                    IList<string> leftItem = new List<string>();
                    IList<string> rightItem = new List<string>();

                    leftItem.Add(((IDictionary<string, object>)data).ToList()[1].Value.ToString()); // phase ID
                    leftItem.Add(((IDictionary<string, object>)data).ToList()[0].Value.ToString()); // index
                    leftItem.Add(this.EncodeData(((IDictionary<string, object>)data).ToList()[3].Value)); // phase name

                    for (int i = 4; i <= (countMonth + 4); i++)
                    {
                        var actualWork = this.FormatDataActualWork(((IDictionary<string, object>)data).ToList()[i].Value).ToString("#,##0.00");
                        totalActualWork += Convert.ToDecimal(actualWork);

                        rightItem.Add(actualWork); // actual effort by month
                    }

                    leftItem.Add(totalActualWork.ToString("#,##0.00")); // total actual effort

                    leftList.Add(leftItem);
                    rightList.Add(rightItem);
                }

                var result =
                    Json(data: new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = leftList,
                        rightList = rightList
                    });
                return result;
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Encode html
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string EncodeData(object data)
        {
            var s = (string)data;
            return HttpUtility.HtmlEncode(s);
        }

        /// <summary>
        /// Format effort get from data base
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string FormatData(object data)
        {
            if (data == null)
            {
                return "0.00/0.00";
            }
            else
            {
                string[] arr = data.ToString().Split('/');
                var dec_actual = Convert.ToDecimal(arr[0]);
                var dec_plan = Convert.ToDecimal(arr[1]);

                return dec_actual.ToString("#,##0.00") + "/" + dec_plan.ToString("#,##0.00");
            }
        }

        /// <summary>
        /// Format actual effort
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private decimal FormatDataActualWork(object data)
        {
            if (data == null)
            {
                return 0;
            }
            else
            {
                var actual = Convert.ToDecimal(data);

                return actual;
            }
        }

        /// <summary>
        /// Get all phase by contract type id
        /// </summary>
        /// <param name="contractTypeId">Contract type Id</param>
        /// <returns>JSON list of phase</returns>
        [HttpGet]
        public ActionResult PhaseListJson(string contractTypeId, int projectID, bool needCheck)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(contractTypeId))
            {
                return new EmptyResult();
            }

            string companyCode = GetLoginUser().CompanyCode;
            IList<PhasePlus> list = this._service.GetPhaseList(companyCode, Convert.ToInt32(contractTypeId));

            if (projectID > 0 && needCheck)
            {
                // get phase data
                var targetPhaseList = this._service.GetTargetPhaseList(companyCode, projectID);

                if (targetPhaseList.Count > 0)
                {
                    foreach (var targetPhase in targetPhaseList)
                    {
                        bool exist = false;

                        foreach (var phase in list)
                        {
                            if (phase.phase_id == targetPhase.phase_id)
                            {
                                phase.check = true;
                                exist = true;
                            }
                        }


                        // add phase existed in project to phase list
                        if (!exist)
                        {
                            list.Add(new PhasePlus
                            {
                                phase_id = targetPhase.phase_id,
                                display_name = targetPhase.phase_name,
                                check = false
                            });
                        }
                    }
                }
            }

            JsonResult result = Json(
                list,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get default category by contract type id
        /// </summary>
        /// <param name="contractTypeId">Contract type Id</param>
        /// <returns>JSON list of category</returns>
        [HttpGet]
        public ActionResult CategoryListJson(string contractTypeId)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(contractTypeId))
            {
                return new EmptyResult();
            }

            string companyCode = GetLoginUser().CompanyCode;
            IList<int> list = this._service.GetDefaultCategoryListByContract(companyCode, Convert.ToInt32(contractTypeId));


            JsonResult result = Json(
                list,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get all sub category by category id
        /// </summary>
        /// <param name="categoryId">Category Id</param>
        /// <returns>JSON list of sub category</returns>
        [HttpGet]
        public ActionResult SubCategoryListJson(string categoryId, int projectID)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(categoryId))
            {
                return new EmptyResult();
            }

            string companyCode = GetLoginUser().CompanyCode;
            IList<SubCategory> list = this._service.GetSubCategoryList(companyCode, projectID, Convert.ToInt32(categoryId));

            JsonResult result = Json(
                list,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get tax rate
        /// </summary>
        /// <param name="fromDate">Project from date</param>
        /// <returns>JSON tax rate</returns>
        [HttpGet]
        public ActionResult TaxRateJson(string fromDate)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            DateTime targetDate;

            if (DateTime.TryParse(fromDate, out targetDate))
            {
                ConsumptionTax taxRate = this._service.GetDefaultTaxRate(GetLoginUser().CompanyCode, targetDate);
                JsonResult result = Json(
                    taxRate != null ? Convert.ToInt32(taxRate.tax_rate * 100) : 0,
                    JsonRequestBehavior.AllowGet);
                return result;
            }
            else
            {
                this.Response.StatusCode = 500;
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Get work day of month
        /// </summary>
        /// <param name="fromYear"></param>
        /// <param name="fromMonth"></param>
        /// <param name="toYear"></param>
        /// <param name="toMonth"></param>
        /// <returns>JSON work day of month</returns>
        [HttpGet]
        public ActionResult WorkDayOfMonthJson(int fromYear, int fromMonth, int toYear, int toMonth)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            var workDayOfMonth = this._service.GetWorkDayOfMonth(GetLoginUser().CompanyCode, fromYear, fromMonth, toYear, toMonth);

            JsonResult result = Json(
                workDayOfMonth,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get actual work day of member
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <param name="fromYear"></param>
        /// <param name="fromMonth"></param>
        /// <param name="toYear"></param>
        /// <param name="toMonth"></param>
        /// <returns></returns>
        public ActionResult GetMemberActualWorkDayJson(int projectID, int userID, int fromYear, int fromMonth, int toYear, int toMonth)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            var dataList = this._service.GetMemberActualWorkDay(GetLoginUser().CompanyCode, projectID, userID, fromYear, fromMonth, toYear, toMonth);

            JsonResult result = Json(
                dataList,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Check valid delete member assignment
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId">User Id</param>
        /// <returns>JSON member actual work time</returns>
        [HttpGet]
        public ActionResult CheckDeleteMember(string projectId, string userId)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(userId))
            {
                return new EmptyResult();
            }

            var memberActualWorkTime = this._service.GetActualWorkTimeByUser(GetLoginUser().CompanyCode, Convert.ToInt32(projectId), Convert.ToInt32(userId));

            JsonResult result = Json(
                memberActualWorkTime,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Check valid delete target phase
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="phaseId">Phase Id</param>
        /// <returns>JSON member actual work time</returns>
        [HttpGet]
        public ActionResult CheckDeletePhase(string projectId, string phaseId)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(phaseId))
            {
                return new EmptyResult();
            }

            var memberActualWorkTime = this._service.GetActualWorkTimeByPhase(GetLoginUser().CompanyCode, Convert.ToInt32(projectId), Convert.ToInt32(phaseId));

            JsonResult result = Json(
                memberActualWorkTime,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Check valid change duration of project
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="timeArr">Time array to check</param>
        /// <returns>True/False</returns>
        [HttpGet]
        public ActionResult CheckChangeDuration(string projectId, List<string> timeArr)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(projectId) || timeArr.Count == 0)
            {
                return new EmptyResult();
            }

            var memberActualWorkTime = this._service.GetActualWorkTimeByMonthYear(GetLoginUser().CompanyCode, Convert.ToInt32(projectId), timeArr);

            JsonResult result = Json(
                memberActualWorkTime,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get data on table list of Project Regist screen
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <param name="startDate">Project start date</param>
        /// <param name="endDate">Project end date</param>
        /// <returns>JSON data on table list</returns>
        [HttpGet]
        public ActionResult GetDataOnTableList(int projectID, string startDate, string endDate)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            string companyCode = GetLoginUser().CompanyCode;
            DateTime start = Convert.ToDateTime(startDate);
            DateTime end = Convert.ToDateTime(endDate);

            // get member assignment data
            var memberAssignmentsTemp = this._service.GetMemberAssignmentList(start, end, companyCode, projectID);
            List<IDictionary<string, object>> memberAssignments = new List<IDictionary<string, object>>();

            if (memberAssignmentsTemp != null)
            {
                foreach (var item in memberAssignmentsTemp)
                {
                    memberAssignments.Add(item);
                }
            }

            // get payment data
            var paymentDetailsTemp = this._service.GetSalePaymentList(start, end, companyCode, projectID, Constant.OrderingFlag.PAYMENT);
            List<IDictionary<string, object>> paymentDetails = new List<IDictionary<string, object>>();

            if (paymentDetailsTemp != null)
            {
                foreach (var item in paymentDetailsTemp)
                {
                    paymentDetails.Add(item);
                }
            }

            // get overhead cost data
            var overheadCosts = this._service.GetOverheadCostDetailList(companyCode, projectID);

            JsonResult result = Json(
                new
                {
                    memberAssignments = memberAssignments,
                    paymentDetails = paymentDetails,
                    overheadCosts = overheadCosts
                },
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get overheadcost data to reload after using ajax submit
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <returns>JSON overheadcost list</returns>
        [HttpGet]
        public ActionResult GetOverheadCostToReload(int projectID)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            string companyCode = GetLoginUser().CompanyCode;

            // get overhead cost data
            var overheadCostList = this._service.GetOverheadCostList(companyCode, projectID);

            JsonResult result = Json(
                new
                {
                    overheadCostList = overheadCostList
                },
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get all history by project id
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <returns>JSON list of project info history</returns>
        [HttpGet]
        public ActionResult HistoryListJson(string projectID)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(projectID))
            {
                return new EmptyResult();
            }

            IList<ProjectInfoHistory> historyList = this._service.GetHistoryOfProject(GetLoginUser().CompanyCode, Convert.ToInt32(projectID));

            foreach (ProjectInfoHistory history in historyList)
            {
                history.insert_date = history.ins_date.ToString("yyyy/MM/dd HH:mm");
            }

            JsonResult result = Json(
                historyList,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get history of project
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ProjectInfoHistoryJson(string projectID, int historyID)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(projectID))
            {
                return new EmptyResult();
            }
            LoginUser currentUser = GetLoginUser();
            string companyCode = currentUser.CompanyCode;

            int projectId = Convert.ToInt32(projectID);
            ProjectInfoHistoryPlus projectInfo = this._service.GetProjectInfoHistory(companyCode, projectId, historyID);

            IList<string> targetTimes = this._service.GetTargetTimeHistory(companyCode, projectId, historyID);

            var memberAssignments = this._service.GetMemberAssignmentHistory(companyCode, projectId, historyID);
            var memberAssignmentDetails = this._service.GetMemberAssignmentDetailHistory(companyCode, projectId, historyID);

            foreach (var item in memberAssignmentDetails)
            {
                item.plan_cost = Utility.RoundNumber(item.plan_cost ?? 0, currentUser.DecimalCalculationType, false);
            }

            var payments = this._service.GetPaymentHistory(companyCode, projectId, historyID);
            var paymentDetails = this._service.GetPaymentDetailHistory(companyCode, projectId, historyID);
            var overheadCosts = this._service.GetOverheadCostHistory(companyCode, projectId, historyID);
            var overheadCostDetails = this._service.GetOverheadCostDetailHistory(companyCode, projectId, historyID);

            JsonResult result = Json(
                new
                {
                    projectInfo = projectInfo,
                    targetTimes = targetTimes,
                    memberAssignments = memberAssignments,
                    memberAssignmentDetails = memberAssignmentDetails,
                    payments = payments,
                    paymentDetails = paymentDetails,
                    overheadCosts = overheadCosts,
                    overheadCostDetails = overheadCostDetails
                },
                JsonRequestBehavior.AllowGet);

            return result;
        }

        [HttpGet]
        public ActionResult CheckChangeStatus(string projectId, string statusId)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(statusId))
            {
                return new EmptyResult();
            }

            string isAccepted = "Accepted";
            decimal memberActualWorkTime = this._service.GetActualWorkTimeByProjectID(GetLoginUser().CompanyCode, Convert.ToInt32(projectId));
            string operationTargetFlag = this._service.GetOperationTargetFlag(GetLoginUser().CompanyCode, Convert.ToInt32(statusId));

            if (memberActualWorkTime > 0 && operationTargetFlag == "0")
            {
                isAccepted = "NotAccepted";
            }

            JsonResult result = Json(
                isAccepted,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Check change status of multi project
        /// </summary>
        /// <param name="projectIds"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckChangeStatusMultiProject(string projectIds, string statusId)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(projectIds) || string.IsNullOrEmpty(statusId))
            {
                return new EmptyResult();
            }

            string isAccepted = "Accepted";
            string operationTargetFlag = this._service.GetOperationTargetFlag(GetLoginUser().CompanyCode, Convert.ToInt32(statusId));
            List<string> projectIdList = projectIds.Split(',').ToList();

            foreach (string projectId in projectIdList)
            {
                decimal memberActualWorkTime = this._service.GetActualWorkTimeByProjectID(GetLoginUser().CompanyCode, Convert.ToInt32(projectId));
                if (memberActualWorkTime > 0 && operationTargetFlag == "0")
                {
                    isAccepted = "NotAccepted";
                    break;
                }
            }

            JsonResult result = Json(
                isAccepted,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get unit cost from history
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPlanCostHistory(string projectId)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (string.IsNullOrEmpty(projectId))
            {
                return new EmptyResult();
            }

            var listPlanCostHistory = this._service.GetPlanCostHistory(GetLoginUser().CompanyCode, Convert.ToInt32(projectId));

            JsonResult result = Json(listPlanCostHistory, JsonRequestBehavior.AllowGet);

            return result;
        }
        #endregion

        #region Method
        /// <summary>
        /// Create Edit view model
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="isCreateCopy">Is create copy project info</param>
        /// <returns>Edit view model</returns>
        private PMS06001EditViewModel MakeEditViewModel(int projectId, bool isCreateCopy, int copyType)
        {
            string companyCode = GetLoginUser().CompanyCode;
            var model = new PMS06001EditViewModel
            {
                STATUS_LIST = this.commonService.GetStatusList(companyCode).ToList(),
                CONTRACT_TYPE_LIST = this._service.GetContractTypeList(companyCode, projectId, isCreateCopy, copyType),
                RANK_LIST = this.commonService.GetRankSelectList(companyCode),
                RANK_TABLE = this.commonService.GetRankList(companyCode).ToList(),
                OVERHEAD_COST_TYPE_LIST = this.GetOverheadCostTypeList(companyCode),
                CATEGORY_LIST = this.GetCategoryList(companyCode, projectId, isCreateCopy, copyType),
                TARGET_CATEGORY_LIST = new List<TargetCategoryPlus>(),
                OUTSOURCER = new SalesPaymentPlus(),
                TAG_LIST = new List<SelectListItem>(),
                MEMBER_ASSIGNMENT_LIST = new List<MemberAssignmentPlus>(),
                MEMBER_ASSIGNMENT_DETAIL_LIST = new List<MemberAssignmentDetailPlus>(),
                SUBCONTRACTOR_LIST = new List<SalesPaymentPlus>(),
                PAYMENT_DETAIL_LIST = new List<SalesPaymentDetailPlus>(),
                OVERHEAD_COST_LIST = new List<OverheadCostPlus>(),
                OVERHEAD_COST_DETAIL_LIST = new List<OverheadCostDetailPlus>(),
                PROGRESS_LIST = new List<ProgressHistoryPlus>(),
                FILE_LIST = new List<ProjectAttachFilePlus>(),
                IS_CREATE_COPY = isCreateCopy,
                COPY_TYPE = copyType,
                PRJ_SYS_ID_TO_COPY = projectId
            };

            if (projectId > 0)
            {
                model.PROJECT_INFO = this._service.GetProjectInfo(companyCode, projectId, isCreateCopy, copyType);
                model.PROJECT_INFO.tax_rate = Convert.ToInt32(model.PROJECT_INFO.tax_rate * 100);
                model.definite_assign_date = model.PROJECT_INFO.assign_fix_date.HasValue;
                model.old_definite_assign_date = model.PROJECT_INFO.assign_fix_date.HasValue;
                model.PHASE_LIST = this._service.GetPhaseList(companyCode, model.PROJECT_INFO.contract_type_id);
                model.TARGET_CATEGORY_LIST = this._service.GetTargetCategoryList(companyCode, model.PROJECT_INFO.project_sys_id).ToList();
                model.PROJECT_INFO.temp_start_date = model.PROJECT_INFO.start_date.HasValue ? model.PROJECT_INFO.start_date.Value.ToString("yyyy/MM/dd") : "";
                model.PROJECT_INFO.temp_end_date = model.PROJECT_INFO.end_date.HasValue ? model.PROJECT_INFO.end_date.Value.ToString("yyyy/MM/dd") : "";
                var defaultCateByContract = this._service.GetDefaultCategoryListByContract(companyCode, model.PROJECT_INFO.contract_type_id);

                var listCateDefault = new List<TargetCategoryPlus>();
                foreach (var item in defaultCateByContract)
                {
                    var existsItem = model.TARGET_CATEGORY_LIST.Where(x => x.category_id == item).FirstOrDefault();
                    if (existsItem != null)
                    {
                        existsItem.is_default = true;
                        listCateDefault.Add(existsItem);
                        model.TARGET_CATEGORY_LIST.Remove(existsItem);
                    }
                    else
                    {
                        listCateDefault.Add(new TargetCategoryPlus()
                        {
                            category_id = item,
                            category_name = model.CATEGORY_LIST.Where(x=>x.Value == item.ToString()).Select(e=>e.Text).FirstOrDefault(),
                            sub_category_id = 0,
                            is_default = true
                        });
                    }
                }

                model.TARGET_CATEGORY_LIST.InsertRange(0, listCateDefault);

                foreach (var item in model.TARGET_CATEGORY_LIST)
                {
                    if (defaultCateByContract.Any(e => e == item.category_id))
                    {
                        item.is_default = true;
                        defaultCateByContract.Remove(item.category_id.Value);
                    }
                }

                // get outsourcer data
                var outsourcers = this._service.GetCustomerList(companyCode, projectId, Constant.OrderingFlag.SALES);

                if (outsourcers.Count > 0)
                {
                    model.OUTSOURCER = outsourcers.FirstOrDefault();

                    var tagList = this.commonService.GetTagSelectList(companyCode, model.OUTSOURCER.customer_id.Value);

                    model.TAG_LIST = tagList;
                }

                // get target phase data
                var targetPhaseList = this._service.GetTargetPhaseList(companyCode, model.PROJECT_INFO.project_sys_id);

                // Check to display logic delete phase
                if (targetPhaseList.Count > 0)
                {
                    foreach (var targetPhase in targetPhaseList)
                    {
                        bool exist = false;

                        foreach (var phase in model.PHASE_LIST)
                        {
                            if (phase.phase_id == targetPhase.phase_id)
                            {
                                phase.check = true;
                                exist = true;
                                //get estimate manday
                                phase.estimate_man_days = targetPhase.estimate_man_days;
                            }
                        }

                        if (!exist)
                        {
                            model.PHASE_LIST.Add(new PhasePlus
                            {
                                phase_id = targetPhase.phase_id,
                                display_name = targetPhase.phase_name,
                                estimate_man_days = targetPhase.estimate_man_days,
                                check = true
                            });
                        }
                    }
                }

                model.SELECT_ALL_PHASES = (model.PHASE_LIST.Count == targetPhaseList.Count);

                // to copy project info
                if (isCreateCopy)
                {
                    model.PROJECT_INFO.project_sys_id = 0;
                    model.PROJECT_INFO.del_flg = Constant.DeleteFlag.NON_DELETE;
                    model.PROJECT_INFO.project_name = "【複写】" + model.PROJECT_INFO.project_name;
                    model.PROJECT_INFO.start_date = null;
                    model.PROJECT_INFO.end_date = null;
                    model.PROJECT_INFO.acceptance_date = null;
                }
                else
                {
                    // to edit project info
                    model.PROGRESS_LIST = this._service.GetProgressList(companyCode, Convert.ToInt32(projectId));
                }

                //#70214
                if (!isCreateCopy || (isCreateCopy && copyType == Constant.CopyType.ALL_INFORMATION))
                {
                    model.PROJECT_INFO.estimate_man_days = Math.Round(model.PROJECT_INFO.estimate_man_days, 1);
                    // get subcontractor data
                    var subcontractors = this._service.GetCustomerList(companyCode, projectId, Constant.OrderingFlag.PAYMENT);

                    if (subcontractors.Count > 0)
                        model.SUBCONTRACTOR_LIST = subcontractors;

                    var overheadCostList = this._service.GetOverheadCostList(companyCode, projectId);

                    if (overheadCostList.Count > 0)
                    {
                        if (isCreateCopy && copyType == Constant.CopyType.ALL_INFORMATION)
                        {
                            foreach (var ovc in overheadCostList)
                            {
                                ovc.detail_no = ovc.detail_no != null ? ovc.detail_no.Value * -1 : ovc.detail_no;
                            }
                        }

                        model.OVERHEAD_COST_LIST = overheadCostList;
                    }
                }
            }
            else
            {
                model.PHASE_LIST = this._service.GetPhaseList(companyCode, model.CONTRACT_TYPE_LIST.FirstOrDefault().contract_type_id);
                var defaultCateByContract = this._service.GetDefaultCategoryListByContract(companyCode, model.CONTRACT_TYPE_LIST.FirstOrDefault().contract_type_id);

                model.TARGET_CATEGORY_LIST = new List<TargetCategoryPlus>();
                foreach (var item in defaultCateByContract)
                {
                    model.TARGET_CATEGORY_LIST.Add(new TargetCategoryPlus()
                    {
                        category_id = item,
                        category_name = model.CATEGORY_LIST.Where(x => x.Value == item.ToString()).Select(e => e.Text).FirstOrDefault(),
                        sub_category_id = 0,
                        is_default = true
                    });
                }
                model.PROJECT_INFO.data_editable_time = this._service.GetDataEditTableTime(companyCode);
            }

            if (model.TARGET_CATEGORY_LIST.Count == 0)
            {
                model.TARGET_CATEGORY_LIST.Add(new TargetCategoryPlus()
                {
                    category_id = 0,
                    sub_category_id = 0
                });
            }

            return model;
        }

        /// <summary>
        /// Create list month from startDate and EndDate
        /// </summary>
        /// <param name="startDate">Project start date</param>
        /// <param name="endDate">Project end date</param>
        /// <returns>List of month</returns>
        private List<string> GetMonthList(DateTime startDate, DateTime endDate)
        {
            List<string> columns = new List<string>();
            var y = startDate.Year;
            var m = startDate.Month;

            while (y < endDate.Year || (y == endDate.Year && m <= endDate.Month))
            {
                string col = string.Format("{0}/{1:00}", y, m);
                columns.Add(col);
                m++;
                if (m == 13)
                {
                    m = 1;
                    y++;
                }
            }
            return columns;
        }

        /// <summary>
        /// Plan project screen _ by POST
        /// </summary>
        /// <param name="plan_projectId"></param>
        /// <param name="read_only"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Plan(int plan_projectId = 0, string read_only = "", string isNotBack = "")
        {
            if (read_only == "1")
            {
                if (!IsInFunctionList(Constant.FunctionID.ProjectPlanReadOnly))
                {
                    return this.RedirectToAction("Index", "ErrorAuthent");
                }
            }
            else
            {
                if (!IsInFunctionList(Constant.FunctionID.ProjectPlanRegist))
                {
                    return this.RedirectToAction("Index", "ErrorAuthent");
                }
            }

            if (plan_projectId == 0)
                return this.RedirectToAction("Index", "Error");

            var model = MakeEditPlanViewModel(plan_projectId);
            model.PROJECT_PLAN_INFO.read_only = read_only;

            if (isNotBack == "PlanByGet")
            {
                model.isNotBack = "PlanByGet";
            }
            return this.View("Plan", model);
        }

        /// <summary>
        /// Plan project screen - by GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Plan(string id)
        {
            try
            {
                var read_only = "";
                if (IsInFunctionList(Constant.FunctionID.ProjectPlanRegist))
                {
                    read_only = "";
                }
                else
                {
                    if (IsInFunctionList(Constant.FunctionID.ProjectPlanReadOnly))
                    {
                        read_only = "1";
                    }
                    else
                    {
                        return this.RedirectToAction("Index", "ErrorAuthent");
                    }
                }

                int projectID = 0;
                if (string.IsNullOrEmpty(id))
                {
                    return this.RedirectToAction("Index", "ErrorAuthent");
                }
                else
                {
                    var companyCode = GetLoginUser().CompanyCode;
                    var projectSysId = this.commonService.getProjectIdByProjectNo(id, companyCode);

                    if (string.IsNullOrEmpty(projectSysId))
                    {
                        return this.RedirectToAction("Index", "ErrorAuthent");
                    }
                    else
                    {
                        projectID = Convert.ToInt32(projectSysId);
                    }
                }

                if (projectID == 0)
                    return this.RedirectToAction("Index", "ErrorAuthent");

                var model = MakeEditPlanViewModel(projectID);
                model.PROJECT_PLAN_INFO.read_only = read_only;

                model.isNotBack = "PlanByGet";

                return this.View("Plan", model);
            }
            catch
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
        }

        /// <summary>
        /// Create Edit view model
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="isCreateCopy">Is create copy project info</param>
        /// <returns>Edit view model</returns>
        private PMS06001EditPlanViewModel MakeEditPlanViewModel(int projectId)
        {
            string companyCode = GetLoginUser().CompanyCode;
            var model = new PMS06001EditPlanViewModel()
            {
                PROJECT_PLAN_INFO = this._service.GetProjectPlanInfo(projectId, companyCode),
            };
            model.PROJECT_PLAN_INFO.targetList = new List<string>();
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_01);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_02);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_03);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_04);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_05);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_06);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_07);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_08);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_09);
            model.PROJECT_PLAN_INFO.targetList.Add(model.PROJECT_PLAN_INFO.target_10);

            model.PROJECT_PLAN_INFO.riskList = new List<MeasureAndConcern>();

            var riskListTemp = new List<MeasureAndConcern>() { };
            riskListTemp.Add(new MeasureAndConcern() { Measure = model.PROJECT_PLAN_INFO.measures_01, Concern = model.PROJECT_PLAN_INFO.concerns_01 });
            riskListTemp.Add(new MeasureAndConcern() { Measure = model.PROJECT_PLAN_INFO.measures_02, Concern = model.PROJECT_PLAN_INFO.concerns_02 });
            riskListTemp.Add(new MeasureAndConcern() { Measure = model.PROJECT_PLAN_INFO.measures_03, Concern = model.PROJECT_PLAN_INFO.concerns_03 });
            riskListTemp.Add(new MeasureAndConcern() { Measure = model.PROJECT_PLAN_INFO.measures_04, Concern = model.PROJECT_PLAN_INFO.concerns_04 });
            riskListTemp.Add(new MeasureAndConcern() { Measure = model.PROJECT_PLAN_INFO.measures_05, Concern = model.PROJECT_PLAN_INFO.concerns_05 });

            model.PROJECT_PLAN_INFO.riskList = riskListTemp;

            model.PROJECT_PLAN_INFO.customer_name = model.PROJECT_PLAN_INFO.customer_name;
            model.PROJECT_PLAN_INFO.project_name = model.PROJECT_PLAN_INFO.project_name;
            model.PROJECT_PLAN_INFO.person_in_charge = model.PROJECT_PLAN_INFO.person_in_charge;
            model.PROJECT_PLAN_INFO.sales_person_in_charge = model.PROJECT_PLAN_INFO.sales_person_in_charge;
            model.PROJECT_PLAN_INFO.user_regist = model.PROJECT_PLAN_INFO.user_regist;
            model.PROJECT_PLAN_INFO.user_update = model.PROJECT_PLAN_INFO.user_update;

            return model;
        }

        /// <summary>
        /// Edit plan project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPlan(PMS06001EditPlanViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    for (var i = 0; i < model.PROJECT_PLAN_INFO.targetList.Count; i++)
                    {
                        if (model.PROJECT_PLAN_INFO.targetList.Count > 0)
                        {
                            if (model.PROJECT_PLAN_INFO.targetList[i].Length > 40)
                            {
                                ModelState.AddModelError("", string.Format(Resources.Messages.E020, "目標", "40"));
                                return new EmptyResult();
                            }
                        }

                        if (string.IsNullOrEmpty(model.PROJECT_PLAN_INFO.targetList[i]))
                        {
                            model.PROJECT_PLAN_INFO.targetList.RemoveAt(i);
                            i--;
                        }
                    }

                    for (var i = 0; i < model.PROJECT_PLAN_INFO.riskList.Count; i++)
                    {
                        if (model.PROJECT_PLAN_INFO.riskList.Count > 0)
                        {
                            if (model.PROJECT_PLAN_INFO.riskList[i].Concern.Length > 50 || model.PROJECT_PLAN_INFO.riskList[i].Measure.Length > 50)
                            {
                                ModelState.AddModelError("", string.Format(Resources.Messages.E020, "リスク", "50"));
                                return new EmptyResult();
                            }
                        }

                        if (string.IsNullOrEmpty(model.PROJECT_PLAN_INFO.riskList[i].Concern) && string.IsNullOrEmpty(model.PROJECT_PLAN_INFO.riskList[i].Measure))
                        {
                            model.PROJECT_PLAN_INFO.riskList.RemoveAt(i);
                            i--;
                        }
                    }

                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 1)
                        model.PROJECT_PLAN_INFO.target_01 = model.PROJECT_PLAN_INFO.targetList[0];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 2)
                        model.PROJECT_PLAN_INFO.target_02 = model.PROJECT_PLAN_INFO.targetList[1];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 3)
                        model.PROJECT_PLAN_INFO.target_03 = model.PROJECT_PLAN_INFO.targetList[2];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 4)
                        model.PROJECT_PLAN_INFO.target_04 = model.PROJECT_PLAN_INFO.targetList[3];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 5)
                        model.PROJECT_PLAN_INFO.target_05 = model.PROJECT_PLAN_INFO.targetList[4];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 6)
                        model.PROJECT_PLAN_INFO.target_06 = model.PROJECT_PLAN_INFO.targetList[5];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 7)
                        model.PROJECT_PLAN_INFO.target_07 = model.PROJECT_PLAN_INFO.targetList[6];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 8)
                        model.PROJECT_PLAN_INFO.target_08 = model.PROJECT_PLAN_INFO.targetList[7];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 9)
                        model.PROJECT_PLAN_INFO.target_09 = model.PROJECT_PLAN_INFO.targetList[8];
                    if (model.PROJECT_PLAN_INFO.targetList.Count >= 10)
                        model.PROJECT_PLAN_INFO.target_10 = model.PROJECT_PLAN_INFO.targetList[9];

                    if (model.PROJECT_PLAN_INFO.riskList.Count >= 1)
                    {
                        model.PROJECT_PLAN_INFO.concerns_01 = model.PROJECT_PLAN_INFO.riskList[0].Concern;
                        model.PROJECT_PLAN_INFO.measures_01 = model.PROJECT_PLAN_INFO.riskList[0].Measure;
                    }
                    if (model.PROJECT_PLAN_INFO.riskList.Count >= 2)
                    {
                        model.PROJECT_PLAN_INFO.concerns_02 = model.PROJECT_PLAN_INFO.riskList[1].Concern;
                        model.PROJECT_PLAN_INFO.measures_02 = model.PROJECT_PLAN_INFO.riskList[1].Measure;
                    }
                    if (model.PROJECT_PLAN_INFO.riskList.Count >= 3)
                    {
                        model.PROJECT_PLAN_INFO.concerns_03 = model.PROJECT_PLAN_INFO.riskList[2].Concern;
                        model.PROJECT_PLAN_INFO.measures_03 = model.PROJECT_PLAN_INFO.riskList[2].Measure;
                    }
                    if (model.PROJECT_PLAN_INFO.riskList.Count >= 4)
                    {
                        model.PROJECT_PLAN_INFO.concerns_04 = model.PROJECT_PLAN_INFO.riskList[3].Concern;
                        model.PROJECT_PLAN_INFO.measures_04 = model.PROJECT_PLAN_INFO.riskList[3].Measure;
                    }
                    if (model.PROJECT_PLAN_INFO.riskList.Count >= 5)
                    {
                        model.PROJECT_PLAN_INFO.concerns_05 = model.PROJECT_PLAN_INFO.riskList[4].Concern;
                        model.PROJECT_PLAN_INFO.measures_05 = model.PROJECT_PLAN_INFO.riskList[4].Measure;
                    }

                    var loginUser = GetLoginUser();
                    model.PROJECT_PLAN_INFO.upd_date = Utility.GetCurrentDateTime();
                    model.PROJECT_PLAN_INFO.upd_id = loginUser.UserId;
                    model.PROJECT_PLAN_INFO.company_code = loginUser.CompanyCode;
                    string IsNotBack = model.isNotBack;

                    int projectId = _service.EditProjectPlanData(model.PROJECT_PLAN_INFO);
                    if (projectId > 0)
                    {
                        string action = model.PROJECT_PLAN_INFO.row_version != null ? "更新" : "登録";
                        string message = string.Format(Resources.Messages.I007, "プロジェクト計画書情報", action);
                        var data = this._service.GetProjectPlanInfo(model.PROJECT_PLAN_INFO.project_sys_id, loginUser.CompanyCode);

                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = message,
                                IsNotBack = IsNotBack,
                                rowVersion = Convert.ToBase64String(data.row_version),
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
                        if (model.PROJECT_PLAN_INFO.row_version.Length > 0) // Duplicate action update
                        {
                            JsonResult result = Json(
                                new
                                {
                                    statusCode = 202,
                                    IsNotBack = IsNotBack,
                                    message = string.Format(Resources.Messages.E031)
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
                                    message = string.Format(Resources.Messages.E045, "プロジェクト計画書情報")
                                },
                                JsonRequestBehavior.AllowGet);

                            return result;
                        }
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
                        message = string.Format(Resources.Messages.E045, "プロジェクト計画書情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="isCreateCopy">Is function copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>List of category</returns>
        private IList<SelectListItem> GetCategoryList(string companyCode, int projectID, bool isCreateCopy, int copyType)
        {
            return
                this._service.GetCategoryList(companyCode, projectID, isCreateCopy, copyType)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.category_id.ToString(),
                            Text = f.category
                        })
                    .ToList();
        }

        /// <summary>
        /// Get all overhead cost type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of overhead cost type</returns>
        private IList<SelectListItem> GetOverheadCostTypeList(string companyCode)
        {
            return
                this._service.GetOverheadCostTypeList(companyCode)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.overhead_cost_id.ToString(),
                            Text = f.overhead_cost_type
                        })
                    .ToList();
        }

        #endregion
    }
}
