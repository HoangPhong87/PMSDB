#region License
/// <copyright file="PMS11002Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using Models.Entities;
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS11002;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS11002;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with Budget setting
    /// </summary>
    public class PMS11002Controller : ControllerBase
    {
        #region Constructor
        /// Main service
        private readonly IPMS11002Service mainService;

        /// Common service
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS11002Controller(IPMS11002Service service, IPMSCommonService cmService)
        {
            this.mainService = service;
            this.commonService = cmService;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11002Controller()
            : this(new PMS11002Service(), new PMSCommonService())
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
            //SA: TODO this function just assume, must build function to Budget Setting
            if ((!IsInFunctionList(Constant.FunctionID.Budget_Setting)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var currentUser = GetLoginUser();
            int close_month = mainService.GetAccountClosingMonth(currentUser.CompanyCode);
            ViewBag.CloseMonth = close_month;
            var model = new PMS11002ListViewModel
            {
                GroupList = this.commonService.GetUserGroupSelectList(currentUser.CompanyCode,true),
                ContractTypeList = this.commonService.GetContractTypeSelectList(currentUser.CompanyCode,true),
                MonthList = Utility.QuarterList(close_month)
            };
            string quaterMonth = "";
            int month = Utility.GetCurrentDateTime().Month;

            foreach (var quarter in model.MonthList)
            {
                string quarterValue = quarter.Value;
                List<string> result = quarterValue.Split(',').ToList();
                for (int i = 0; i < result.Count; i++)
                {
                    if (month == Convert.ToInt32(result[i]))
                    {
                        quaterMonth = quarterValue;
                        break;
                    }
                }
            }

            model.Condition.Month = quaterMonth;
            model.Condition.GroupId = null;
            model.Condition.ContractTypeId = null;
            model.Condition.Year = Utility.GetCurrentFinancialYear(close_month).ToString();

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("List", model);
        }

        #endregion


        #region Ajax Action
        /// <summary>
        /// Search data budget setting
        /// </summary>
        /// <param name="GroupId">GroupId</param>
        /// <param name="ContractTypeId">ContractTypeId</param>
        /// <param name="Month">Month</param>
        /// <param name="Year">Year</param>
        /// <param name="TimeList">TimeList</param>
        /// <returns></returns>
        public ActionResult SearchBudgetSetting(string GroupId, string ContractTypeId, string Month = "", string Year = "", List<TimeListBudget> TimeList = null)
        {
            if (Request.IsAjaxRequest())
            {
                PMS11002ListViewModel model = new PMS11002ListViewModel();
                model.Condition.CompanyCode = GetLoginUser().CompanyCode;
                model.Condition.Year = Year;

                if (!string.IsNullOrEmpty(GroupId))
                {
                    model.Condition.GroupId = GroupId;
                }
               
                if (!string.IsNullOrEmpty(ContractTypeId))
                {
                    model.Condition.ContractTypeId = ContractTypeId;
                }

                if (!string.IsNullOrEmpty(Month))
                {
                    model.Condition.Month = Month;
                }

                model.Condition.List_Contract = mainService.GetListContractTypeBySearch(ContractTypeId, GetLoginUser().CompanyCode).OrderBy(m=>m.display_order).ThenBy(m=>m.contract_type_id).ToList();
                model.Condition.List_Group = mainService.GetListGroupBySearch(GroupId, GetLoginUser().CompanyCode).OrderBy(m => m.display_order).ThenBy(m => m.group_id).ToList();

                if (model.Condition.Month != null)
                {
                    model.Condition.List_Month = model.Condition.Month.Split(',').ToList();
                }
                else
                {
                    //build condition month list default
                    var monthList = new List<string>();
                    for (int i = 1; i <= 12; i++)
                    {
                        monthList.Add(i.ToString());
                    }
                    model.Condition.List_Month = monthList;
                }

                model.BudgetList = mainService.GetListBudgetBySearch(ContractTypeId, GroupId, Month, Year, model.Condition.CompanyCode, TimeList);
                var totalYear = mainService.GetListTotalYearBySearch(Year, model.Condition.CompanyCode, ContractTypeId);
                var totalYearList = new List<TotalYearList>();
                foreach (var a in totalYear)
                {
                    var eachGroupTotal = new TotalYearList();
                    eachGroupTotal.group_id = a.group_id.ToString();
                    var totalProfit = a.total_profit;
                    var salesBudget = a.total_sales;
                    eachGroupTotal.profit_budget = totalProfit == null ? "0" : totalProfit.ToString();
                    eachGroupTotal.sales_budget = salesBudget == null ? "0" : salesBudget.ToString();
                    totalYearList.Add(eachGroupTotal);
                }

                model.TotalYearList = totalYearList;

                var result = Json(
                   model,
                   JsonRequestBehavior.AllowGet);
                return result;
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Update budget setting info
        /// </summary>
        /// <param name="dataListBudget"></param>
        /// <returns></returns>
        public JsonResult UpdateBudget(IList<Budget> dataListBudget)
        {
            try
            {
                if (ModelState.IsValid && Request.IsAjaxRequest())
                {
                    if (dataListBudget.Count > 0)
                    {
                        var currentUser = this.GetLoginUser();
                        int userId = currentUser.UserId;
                        DateTime now = Utility.GetCurrentDateTime();
                        foreach (var listBudget in dataListBudget)
                        {
                            listBudget.company_code = currentUser.CompanyCode;
                            //listBudget.del_flg = "0";
                            listBudget.ins_date = now;
                            listBudget.ins_id = userId;
                            listBudget.upd_date = now;
                            listBudget.upd_id = userId;
                        }
                        var result = mainService.ProcessUpdateBudget(dataListBudget);
                        if (result)
                        {
                            return Json("OK");
                        }
                        else
                        {
                            return Json("E015");
                        }
                    }
                    else
                    {
                        return Json("E043");
                    }
                }
                return Json(null);
            }
            catch
            {
                return Json("E043");
            }
        }
        #endregion
    }
}
