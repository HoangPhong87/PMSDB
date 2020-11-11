#region License
/// <copyright file="PMS03001Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS03001;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS03001;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with contractType
    /// </summary>
    public class PMS03001Controller : ControllerBase
    {
        #region Constructor
        /// Main service
        private readonly IPMS03001Service mainService;

        // Common service
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS03001Controller(IPMS03001Service service, IPMSCommonService cmService)
        {
            this.mainService = service;
            this.commonService = cmService;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS03001Controller()
            : this(new PMS03001Service(), new PMSCommonService())
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
            if (!this.IsInFunctionList(Constant.FunctionID.ContractTypeList_Admin)
                && !this.IsInFunctionList(Constant.FunctionID.ContractTypeList))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS03001ListViewModel();

            // Get Jquery data table state
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS03001/Edit") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// Export contractType list to csv file
        /// </summary>
        /// <param name="hdnContractTypeName">ContractType name</param>
        /// <param name="hdnDelFlag">ContractType delete flag</param>
        /// <param name="hdnOrderBy">ContractType list order by</param>
        /// <param name="hdnOrderType">ContractType list order type</param>
        /// <returns>csv file</returns>
        public ActionResult ExportCsv(string hdnContractTypeName, bool hdnDelFlag, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            try
            {
                Condition condition = new Condition()
                {
                    ContractTypeName = hdnContractTypeName,
                    DeleteFlag = hdnDelFlag
                };
                List<string> titles = new List<string>()
                {
                    "No.",
                    "契約種別",
                    "営業担当者",
                    "特殊計算",
                    "予算対象",
                    "表示順",
                    "備考",
                    "更新日時",
                    "更新者"
                };

                if (string.IsNullOrEmpty(hdnOrderBy))
                    hdnOrderBy = "upd_date";

                string fileName = "ContractTypeList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";

                IList<ContractTypePlus> contractTypeList = new List<ContractTypePlus>();
                if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
                {
                    contractTypeList = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<ContractTypePlus>;
                }
                else
                {
                    contractTypeList = this.mainService.ExportContractTypeListToCSV(GetLoginUser().CompanyCode, condition, hdnOrderBy, hdnOrderType);
                }

                List<ContractTypeListExportCSV> dataExport = new List<ContractTypeListExportCSV>();

                foreach (var contractTypeInfo in contractTypeList)
                {
                    dataExport.Add(new ContractTypeListExportCSV()
                    {
                        peta_rn = contractTypeInfo.peta_rn.ToString(),
                        contract_type = contractTypeInfo.contract_type,
                        charge_of_sales_flg = (contractTypeInfo.charge_of_sales_flg.Equals(Constant.DEFAULT_VALUE) ? "無" : "有"),
                        exceptional_calculate_flg = (contractTypeInfo.exceptional_calculate_flg.Equals(Constant.DEFAULT_VALUE) ? "無" : "有"),
                        budget_setting_flg = (contractTypeInfo.budget_setting_flg.Equals(Constant.DEFAULT_VALUE) ? "無" : "有"),
                        display_order = contractTypeInfo.display_order.ToString(),
                        remarks = (string.IsNullOrEmpty(contractTypeInfo.remarks) ? "" : contractTypeInfo.remarks.Replace("\r\n", " ")),
                        upd_date = contractTypeInfo.upd_date.Value.ToString("yyyy/MM/dd HH:mm"),
                        upd_user = contractTypeInfo.upd_user
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
        /// <param name="id">ContractType ID</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        public ActionResult Edit(int id = 0)
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ContractTypeRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            var model = MakeEditViewModel(id);

            return this.View("Edit", model);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.ContractTypeRegist))
                return this.RedirectToAction("Index", "ErrorAuthent");

            var model = MakeEditViewModel(0);

            return this.View("Edit", model);
        }

        private PMS03001EditViewModel MakeEditViewModel(int id)
        {
            string companyCode = GetLoginUser().CompanyCode;
            var model = new PMS03001EditViewModel()
            {
                PHASE_LIST = this.GetPhaseList(companyCode, id),
                CATEGORY_LIST = this.GetCategoryList(companyCode, id),
                CONTRACT_TYPE_PHASE_LIST = new List<ContractTypePhasePlus>(),
                CONTRACT_TYPE_CATEGORY_LIST = new List<ContractTypeCategoryPlus>(),
                CheckPlanFlag = true,
                CheckProgressFlag = true,
                CheckPeriodFlag = true,
                CheckSalesFlag = true
            };

            if (id > 0)
            {
                model.ContractTypeInfo = this.mainService.GetContractTypeInfo(companyCode, id);
                model.CONTRACT_TYPE_PHASE_LIST = this.mainService.GetContractTypePhaseList(companyCode, id);
                model.CONTRACT_TYPE_CATEGORY_LIST = this.mainService.GetContractTypeCategoryList(companyCode, id);
            }

            return model;
        }

        /// <summary>
        /// Edit contractType info
        /// </summary>
        /// <param name="model">ContractType info to edit</param>
        /// <returns>Edit view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditContractType(PMS03001EditViewModel model)
        {
            try
            {
                var loginUser = GetLoginUser();

                if (ModelState.IsValid)
                {
                    bool validData = true;
                    bool existPhase = false;

                    foreach (var obj in model.CONTRACT_TYPE_PHASE_LIST)
                    {
                        if (obj.phase_id > 0)
                        {
                            existPhase = true;
                            break;
                        }
                    }

                    // Check not exist phase by contract type
                    if (!existPhase)
                    {
                        validData = false;
                        ModelState.AddModelError(string.Empty, string.Format(Resources.Messages.E017, "フェーズ"));
                    }

                    if (validData && model.ContractTypeInfo.contract_type_id > 0)
                    {
                        var oldContractTypePhaseList = this.mainService.GetContractTypePhaseList(loginUser.CompanyCode, model.ContractTypeInfo.contract_type_id);

                        foreach (var dataOld in oldContractTypePhaseList)
                        {
                            if (dataOld.project_count > 0)
                            {
                                bool validPhase = false;

                                foreach (var dataNew in model.CONTRACT_TYPE_PHASE_LIST)
                                {
                                    if (dataNew.phase_id == dataOld.phase_id)
                                    {
                                        validPhase = true;
                                        break;
                                    }
                                }

                                // Check phase existed in project cannot delete
                                if (!validPhase)
                                {
                                    validData = false;
                                    ModelState.AddModelError(string.Empty, string.Format(Resources.Messages.E059));
                                    break;
                                }
                            }
                        }
                    }

                    if (validData)
                    {
                        // Check limit data by license of company
                        if ((model.ContractTypeInfo.contract_type_id == 0
                            || (model.OLD_DEL_FLAG
                            && Constant.DeleteFlag.NON_DELETE.Equals(model.ContractTypeInfo.del_flg)))
                            && !this.commonService.CheckValidUpdateData(loginUser.CompanyCode, Constant.LicenseDataType.CONTRACT_TYPE))
                        {
                            JsonResult result = Json(
                                new
                                {
                                    statusCode = 500,
                                    message = string.Format(Resources.Messages.E067, "契約種別")
                                },
                                JsonRequestBehavior.AllowGet);

                            return result;
                        }

                        model.ContractTypeInfo.upd_date = Utility.GetCurrentDateTime();
                        model.ContractTypeInfo.upd_id = loginUser.UserId;
                        model.ContractTypeInfo.company_code = loginUser.CompanyCode;

                        int contractTypeID = 0;

                        // Update info
                        if (this.mainService.EditContractTypeInfo(model.ContractTypeInfo, model.CONTRACT_TYPE_PHASE_LIST, model.CONTRACT_TYPE_CATEGORY_LIST, out contractTypeID))
                        {
                            string action = model.ContractTypeInfo.contract_type_id > 0 ? "更新" : "登録";

                            model.ContractTypeInfo.contract_type_id = contractTypeID;
                            string message = String.Format(Resources.Messages.I007, "契約種別", action);

                            var data = this.mainService.GetContractTypeInfo(GetLoginUser().CompanyCode, contractTypeID);

                            JsonResult result = Json(
                                    new
                                    {
                                        statusCode = 201,
                                        message = message,
                                        id = contractTypeID,
                                        insDate = (data.ins_date != null) ? data.ins_date.Value.ToString("yyyy/MM/dd HH:mm") : "",
                                        updDate = (data.upd_date != null) ? data.upd_date.Value.ToString("yyyy/MM/dd HH:mm") : "",
                                        insUser = data.ins_user,
                                        updUser = data.upd_user,
                                        deleted = data.del_flg.Equals(Constant.DeleteFlag.DELETE) ? true : false,
                                    },
                                    JsonRequestBehavior.AllowGet);

                            return result;
                        }
                        else // has error when update
                        {
                            JsonResult result = Json(
                                new
                                {
                                    statusCode = 500,
                                    message = string.Format(Resources.Messages.E045, "契約種別情報")
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
                        message = string.Format(Resources.Messages.E045, "契約種別情報")
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
        }

        #endregion

        #region Ajax Action
        /// <summary>
        /// Search contractType by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Json list of contractType</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string orderBy, string orderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                var pageInfo = this.mainService.Search(model, GetLoginUser().CompanyCode, condition);

                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this.mainService.ExportContractTypeListToCSV(GetLoginUser().CompanyCode, condition, orderBy, orderType);

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = (from t in pageInfo.Items
                                  select new object[]
{
                            t.contract_type_id,
                            t.peta_rn,
                            HttpUtility.HtmlEncode(t.contract_type),
                            (t.charge_of_sales_flg.Equals(Constant.DEFAULT_VALUE) ? "無" : "有"),
                            (t.exceptional_calculate_flg.Equals(Constant.DEFAULT_VALUE) ? "無" : "有"),
                            (t.budget_setting_flg.Equals(Constant.DEFAULT_VALUE) ? "無" : "有"),
                            t.display_order,
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

        /// <summary>
        /// Get target phase
        /// </summary>
        /// <param name="phaseID">Phase Id</param>
        /// <returns>JSON list of target phase</returns>
        [HttpGet]
        public ActionResult GetTargetPhase(int phaseID = 0, int contractTypeID = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (phaseID == 0)
                return new EmptyResult();

            var targetPhaseList = this.mainService.GetTargetPhaseList(GetLoginUser().CompanyCode, phaseID, contractTypeID);

            JsonResult result = Json(
                targetPhaseList,
                JsonRequestBehavior.AllowGet);

            return result;
        }

        /// <summary>
        /// Get target category
        /// </summary>
        /// <param name="categoryID">Category Id</param>
        /// <returns>JSON list of target category</returns>
        [HttpGet]
        public ActionResult GetTargetCategory(int categoryID = 0, int contractTypeID = 0)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            if (categoryID == 0)
                return new EmptyResult();

            var targetCategoryList = this.mainService.GetTargetCategoryList(GetLoginUser().CompanyCode, categoryID, contractTypeID);

            JsonResult result = Json(
                targetCategoryList,
                JsonRequestBehavior.AllowGet);

            return result;
        }
        #endregion

        #region Private

        /// <summary>
        /// Get all phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of phase</returns>
        private IList<SelectListItem> GetPhaseList(string companyCode, int contractTypeID)
        {
            return
                this.mainService.GetPhaseList(companyCode, contractTypeID)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.phase_id.ToString(),
                            Text = f.display_name
                        })
                    .ToList();
        }

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of category</returns>
        private IList<SelectListItem> GetCategoryList(string companyCode, int contractTypeID)
        {
            return
                this.mainService.GetCategoryList(companyCode, contractTypeID)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.category_id.ToString(),
                            Text = f.category
                        })
                    .ToList();
        }
        #endregion
    }
}
