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
    using ProjectManagementSystem.Models.PMS11003;
    using ProjectManagementSystem.ViewModels.PMS11003;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with Budget setting
    /// </summary>
    public class PMS11003Controller : ControllerBase
    {
        #region Constructor
        /// Main service
        private readonly IPMS11003Service mainService;

        /// Common service
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS11003Controller(IPMS11003Service service, IPMSCommonService cmService)
        {
            this.mainService = service;
            this.commonService = cmService;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11003Controller()
            : this(new PMS11003Service(), new PMSCommonService())
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
            if ((!IsInFunctionList(Constant.FunctionID.BudgetActual_List)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            var currentUser = GetLoginUser();
            int closingMonth = mainService.GetAccountClosingMonth(currentUser.CompanyCode);
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            var timeStart = "";
            var timeEnd = "";
            if (closingMonth == 12)
            {
                timeStart = currentYear + "/01";
                timeEnd = currentYear + "/12";
            }
            else
            {
                if (currentMonth <= closingMonth)
                {
                    timeStart = (currentYear - 1) + "/" + ((closingMonth + 1) >= 10 ? (closingMonth + 1).ToString() : "0" + (closingMonth + 1).ToString());
                    timeEnd = currentYear + "/" + (closingMonth >= 10 ? closingMonth.ToString() : "0" + closingMonth.ToString());
                }
                else
                {
                    timeStart = currentYear + "/" + ((closingMonth + 1) >= 10 ? (closingMonth + 1).ToString() : "0" + (closingMonth + 1).ToString());
                    timeEnd = (currentYear + 1) + "/" + (closingMonth >= 10 ? closingMonth.ToString() : "0" + closingMonth.ToString());
                }
            }

            var model = new PMS11003ListViewModel();

            model.Condition.Year = DateTime.Now.Year.ToString();
            model.TimeStart = timeStart;
            model.TimeEnd = timeEnd;
            var salesData = mainService.GetListSaleData(timeStart, timeEnd, currentUser.CompanyCode, "", "");

            List<IDictionary<string, object>> salesDataTemp = new List<IDictionary<string, object>>();
            if (salesData != null)
            {
                foreach (var item in salesData)
                {
                    salesDataTemp.Add(item);
                }
            }

            model.GroupListToSearch = this.commonService.GetUserGroupSelectList(currentUser.CompanyCode,true);
            model.ContractTypeListToSearch = this.commonService.GetContractTypeSelectList(currentUser.CompanyCode, true);

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("SalesBudget", model);
        }

        #endregion

        #region Ajax Action

        #region Sales
        /// <summary>
        /// Search data sales budget/actual 
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="GroupId"></param>
        /// <param name="ContractTypeId"></param>
        /// <param name="checkSalesType"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public ActionResult SearchSalesBudget(string timeStart, string timeEnd, string GroupId, string ContractTypeId, string checkSalesType, string tabId)
        {
            if (Request.IsAjaxRequest())
            {
                ViewBag.ModeSalesBudget = "true";
                ViewBag.ModeProfitBudget = "false";
                var model = CreateSalesBudgetModel(timeStart, timeEnd, GroupId, ContractTypeId, checkSalesType);
                Session["SalesBudgetData" + tabId] = model;
                var profitModel = CreateProfitBudgetModel(timeStart, timeEnd, GroupId, ContractTypeId, checkSalesType);
                Session["ProfitBudgetData" + tabId] = profitModel;

                model.CheckSalesType = checkSalesType;
                var result = Json(
                   model,
                   JsonRequestBehavior.AllowGet);
                return result;
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Create SalesBudget Model
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="GroupId"></param>
        /// <param name="ContractTypeId"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        public PMS11003ListViewModel CreateSalesBudgetModel(string timeStart, string timeEnd, string GroupId, string ContractTypeId, string checkSalesType)
        {
            var model = new PMS11003ListViewModel();
            var currentUser = GetLoginUser();
            model.Condition.Year = DateTime.Now.Year.ToString();
            model.TimeStart = timeStart;
            model.TimeEnd = timeEnd;
            if (!string.IsNullOrEmpty(GroupId))
            {
                model.Condition.GroupId = GroupId;
            }

            if (!string.IsNullOrEmpty(ContractTypeId))
            {
                model.Condition.ContractTypeId = ContractTypeId;
            }
            model.Condition.List_Contract = mainService.GetListContractTypeBySearch(ContractTypeId, currentUser.CompanyCode).ToList();
            model.Condition.List_Group = mainService.GetListGroupBySearch(GroupId, currentUser.CompanyCode).ToList();
            //total group counting
            var totalGroup = mainService.GetListTotalGroup(timeStart, timeEnd, currentUser.CompanyCode, ContractTypeId, GroupId, checkSalesType);
            List<IDictionary<string, object>> totalGrTemp = new List<IDictionary<string, object>>();
            if (totalGroup != null)
            {
                foreach (var item in totalGroup)
                {
                    totalGrTemp.Add(item);
                }
            }
            var tGrList = new List<TotalGroup>();
            for (int i = 0; i < totalGrTemp.Count; i++)
            {
                var tGr = new TotalGroup();
                tGr.target_year = Convert.ToInt32(totalGrTemp[i].Values.ElementAt(0).ToString());
                tGr.target_month = Convert.ToInt32(totalGrTemp[i].Values.ElementAt(1).ToString());
                tGr.group_id = totalGrTemp[i].Values.ElementAt(2).ToString();
                tGr.tgrBudget = totalGrTemp[i].Values.ElementAt(3).ToString();
                tGr.tgrSales = totalGrTemp[i].Values.ElementAt(4).ToString();
                tGr.tgrProfit = totalGrTemp[i].Values.ElementAt(5).ToString();
                tGrList.Add(tGr);
            }

            //total contract type counting
            var totalCt = mainService.GetListTotalCT(timeStart, timeEnd, currentUser.CompanyCode, ContractTypeId, GroupId, checkSalesType);
            List<IDictionary<string, object>> totalCTTemp = new List<IDictionary<string, object>>();
            if (totalCt != null)
            {
                foreach (var item in totalCt)
                {
                    totalCTTemp.Add(item);
                }
            }
            var tCTList = new List<TotalContractType>();
            for (int i = 0; i < totalCTTemp.Count; i++)
            {
                var tCT = new TotalContractType();
                tCT.target_year = Convert.ToInt32(totalCTTemp[i].Values.ElementAt(0).ToString());
                tCT.target_month = Convert.ToInt32(totalCTTemp[i].Values.ElementAt(1).ToString());
                tCT.contract_type_id = totalCTTemp[i].Values.ElementAt(2).ToString();
                tCT.tgrBudget = totalCTTemp[i].Values.ElementAt(3).ToString();
                tCT.tgrSales = totalCTTemp[i].Values.ElementAt(4).ToString();
                tCT.tgrProfit = totalCTTemp[i].Values.ElementAt(5).ToString();
                tCTList.Add(tCT);
            }

            //total all year of group counting
            var totalGrAll = mainService.GetListTotalAllYearGroup(timeStart, timeEnd, currentUser.CompanyCode, ContractTypeId, GroupId, checkSalesType);
            List<IDictionary<string, object>> totalGrAllTemp = new List<IDictionary<string, object>>();
            if (totalGrAll != null)
            {
                foreach (var item in totalGrAll)
                {
                    totalGrAllTemp.Add(item);
                }
            }
            var totalGrAllList = new List<TotalGrAll>();
            for (int i = 0; i < totalGrAllTemp.Count; i++)
            {
                var tGrAll = new TotalGrAll();
                tGrAll.group_id = totalGrAllTemp[i].Values.ElementAt(0).ToString();
                tGrAll.tgrBudget = totalGrAllTemp[i].Values.ElementAt(1).ToString();
                tGrAll.tgrSales = totalGrAllTemp[i].Values.ElementAt(2).ToString();
                tGrAll.tgrProfit = totalGrAllTemp[i].Values.ElementAt(3).ToString();
                totalGrAllList.Add(tGrAll);
            }

            //sales data counting
            var salesData = mainService.GetListSaleData(timeStart, timeEnd, currentUser.CompanyCode, ContractTypeId, GroupId, checkSalesType);
            List<IDictionary<string, object>> salesDataTemp = new List<IDictionary<string, object>>();
            if (salesData != null)
            {
                foreach (var item in salesData)
                {
                    salesDataTemp.Add(item);
                }
            }
            string grIDExCondition = "";// save all grId after filter second time
            var GroupList = new List<Group>();
            string ctIDExCondition = "";// save all ctId after filter second time
            var ContractTypeList = new List<ContractType>();
            var SaleDataList = new List<SalesData>();
            var timeList = new List<TimeList>();
            for (int i = 0; i < salesDataTemp.Count; i++)
            {
                var targetYear = salesDataTemp[i].Values.ElementAt(0).ToString();
                var targetMonth = salesDataTemp[i].Values.ElementAt(1).ToString();
                var grName = salesDataTemp[i].Values.ElementAt(2).ToString();
                var ctName = salesDataTemp[i].Values.ElementAt(3).ToString();
                var budgetAmount = salesDataTemp[i].Values.ElementAt(4).ToString();
                var ctId = salesDataTemp[i].Values.ElementAt(5).ToString();
                var grId = salesDataTemp[i].Values.ElementAt(6).ToString();
                var salesActual = salesDataTemp[i].Values.ElementAt(7).ToString();
                var profit = salesDataTemp[i].Values.ElementAt(8).ToString();
                var gr_order = salesDataTemp[i].Values.ElementAt(9).ToString();
                var ct_order = salesDataTemp[i].Values.ElementAt(10).ToString();

                var grItem = new Group();
                grItem.display_name = grName;
                grItem.group_id = Convert.ToInt32(grId);
                grItem.display_order = Convert.ToInt32(gr_order);
                var chk = true;
                for (int j = 0; j < GroupList.Count; j++)
                {
                    if (GroupList[j].display_name == grItem.display_name && GroupList[j].group_id == grItem.group_id)
                    {
                        chk = false;
                        break;
                    }
                }
                if (chk)
                {
                    grIDExCondition += grItem.group_id + ",";
                    GroupList.Add(grItem);
                }

                var ctItem = new ContractType();
                ctItem.contract_type = ctName;
                ctItem.contract_type_id = Convert.ToInt32(ctId);
                ctItem.display_order = Convert.ToInt32(ct_order);
                chk = true;
                for (int j = 0; j < ContractTypeList.Count; j++)
                {
                    if (ContractTypeList[j].contract_type == ctItem.contract_type && ContractTypeList[j].contract_type_id == ctItem.contract_type_id)
                    {
                        chk = false;
                        break;
                    }
                }
                if (chk)
                {
                    ctIDExCondition += ctItem.contract_type_id + ",";
                    ContractTypeList.Add(ctItem);
                }
                var tlItem = new TimeList();
                tlItem.year = targetYear;
                tlItem.month = targetMonth;
                chk = true;
                for (int j = 0; j < timeList.Count; j++)
                {
                    if (timeList[j].month == tlItem.month && timeList[j].year == tlItem.year)
                    {
                        chk = false;
                        break;
                    }
                }
                if (chk)
                {
                    timeList.Add(tlItem);
                }

                var saleItem = new SalesData();
                saleItem.target_year = Convert.ToInt32(targetYear);
                saleItem.target_month = Convert.ToInt32(targetMonth);
                saleItem.group_name = grName;
                saleItem.contract_type_name = ctName;
                saleItem.sales_budget = Convert.ToDecimal(budgetAmount);
                saleItem.contract_type_id = ctId;
                saleItem.group_id = grId;
                saleItem.sales_actuals = salesActual;
                saleItem.profit = profit;
                SaleDataList.Add(saleItem);
            }
            if (!string.IsNullOrEmpty(ctIDExCondition) && !string.IsNullOrEmpty(ctIDExCondition))
            {
                ctIDExCondition = ctIDExCondition.Substring(0, ctIDExCondition.Length - 1);
                grIDExCondition = grIDExCondition.Substring(0, grIDExCondition.Length - 1);
                //totalGrAllList; tCTList; tGrList
                CountChargePersonNull(ref tGrList, ref tCTList, ref totalGrAllList, timeStart, timeEnd, currentUser.CompanyCode, ctIDExCondition, grIDExCondition, ref SaleDataList, checkSalesType);
            }
            model.TotalGroup = tGrList;
            model.TotalCT = tCTList;
            model.TotalGrYearList = totalGrAllList;
            model.GroupList = GroupList.OrderBy(m => m.display_order).ThenBy(m => m.group_id).ToList();
            model.ContractTypeList = ContractTypeList.OrderBy(m => m.display_order).ThenBy(m => m.contract_type_id).ToList();
            model.DataSalesBudget = SaleDataList;
            model.TimeList = timeList;
            model.CheckSalesType = checkSalesType;
            return model;
        }
        /// <summary>
        /// Count Charge Person Null
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="ContractTypeId"></param>
        /// <param name="GroupId"></param>
        /// <param name="listSales"></param>
        /// <param name="checkSalesType"></param>
        public void CountChargePersonNull(ref List<TotalGroup> TotalGrList, ref List<TotalContractType> TotalCtList, ref List<TotalGrAll> TotalYear, string timeStart, string timeEnd, string companyCode, string ContractTypeId, string GroupId, ref List<SalesData> listSales, string checkSalesType)
        {
            var listChargePersonNull = mainService.GetListChargePersonNull(timeStart, timeEnd, companyCode, ContractTypeId, GroupId, checkSalesType);
            if (listChargePersonNull != null)
            {
                List<IDictionary<string, object>> cpnListTemp = new List<IDictionary<string, object>>();
                foreach (var item in listChargePersonNull)
                {
                    cpnListTemp.Add(item);
                }

                for (int j = 0; j < cpnListTemp.Count; j++)
                {
                    var targetMonth = cpnListTemp[j].Values.ElementAt(0).ToString();
                    var targetYear = cpnListTemp[j].Values.ElementAt(1).ToString();
                    var salesActual = cpnListTemp[j].Values.ElementAt(2).ToString();
                    var ctId = cpnListTemp[j].Values.ElementAt(3).ToString();
                    var grId = cpnListTemp[j].Values.ElementAt(4).ToString();
                    for (int i = 0; i < listSales.Count; i++)
                    {
                        if (listSales[i].contract_type_id == ctId && listSales[i].group_id == grId && listSales[i].target_month.ToString() == targetMonth && listSales[i].target_year.ToString() == targetYear)
                        {
                            listSales[i].sales_actuals = (Convert.ToInt32(listSales[i].sales_actuals) + Convert.ToInt32(salesActual)).ToString();
                            listSales[i].profit = ((Convert.ToDecimal(listSales[i].sales_actuals) / listSales[i].sales_budget) * 100).ToString();

                            //recount ting total year
                            for (int k = 0; k < TotalYear.Count; k++)
                            {
                                if (TotalYear[k].group_id == listSales[i].group_id)
                                {
                                    TotalYear[k].tgrSales = (Convert.ToInt32(TotalYear[k].tgrSales) + Convert.ToInt32(salesActual)).ToString();
                                    TotalYear[k].tgrProfit = ((Convert.ToDecimal(TotalYear[k].tgrSales) / Convert.ToDecimal(TotalYear[k].tgrBudget)) * 100).ToString();
                                    break;
                                }
                            }
                            //recount ting total contract type
                            for (int k = 0; k < TotalCtList.Count; k++)
                            {
                                if (TotalCtList[k].contract_type_id == listSales[i].contract_type_id && TotalCtList[k].target_month == listSales[i].target_month && TotalCtList[k].target_year == listSales[i].target_year)
                                {
                                    TotalCtList[k].tgrSales = (Convert.ToInt32(TotalCtList[k].tgrSales) + Convert.ToInt32(salesActual)).ToString();
                                    TotalCtList[k].tgrProfit = ((Convert.ToDecimal(TotalCtList[k].tgrSales) / Convert.ToDecimal(TotalCtList[k].tgrBudget)) * 100).ToString();
                                    break;
                                }
                            }

                            //recount ting total group
                            for (int k = 0; k < TotalGrList.Count; k++)
                            {
                                if (TotalGrList[k].group_id == listSales[i].group_id && TotalGrList[k].target_month == listSales[i].target_month && TotalGrList[k].target_year == listSales[i].target_year)
                                {
                                    TotalGrList[k].tgrSales = (Convert.ToInt32(TotalGrList[k].tgrSales) + Convert.ToInt32(salesActual)).ToString();
                                    TotalGrList[k].tgrProfit = ((Convert.ToDecimal(TotalGrList[k].tgrSales) / Convert.ToDecimal(TotalGrList[k].tgrBudget)) * 100).ToString();
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Profit
        /// <summary>
        /// Search profit budget
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="GroupId"></param>
        /// <param name="ContractTypeId"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public ActionResult SearchProfitBudget(string timeStart, string timeEnd, string GroupId, string ContractTypeId, string checkSalesType, string tabId)
        {
            if (Request.IsAjaxRequest())
            {
                ViewBag.ModeProfitBudget = "true";
                ViewBag.ModeSalesBudget = "false";
                var model = CreateProfitBudgetModel(timeStart, timeEnd, GroupId, ContractTypeId, checkSalesType);
                Session["ProfitBudgetData" + tabId] = model;
                var salesModel = CreateSalesBudgetModel(timeStart, timeEnd, GroupId, ContractTypeId, checkSalesType);
                Session["SalesBudgetData" + tabId] = salesModel;
                var result = Json(
                   model,
                   JsonRequestBehavior.AllowGet);

                return result;
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Create Profit Budget Model
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="GroupId"></param>
        /// <param name="ContractTypeId"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        public PMS11003ListViewModelPlus CreateProfitBudgetModel(string timeStart, string timeEnd, string GroupId, string ContractTypeId, string checkSalesType)
        {
            var model = new PMS11003ListViewModelPlus();
            var currentUser = GetLoginUser();
            model.Condition.Year = DateTime.Now.Year.ToString();
            model.TimeStart = timeStart;
            model.TimeEnd = timeEnd;
            if (!string.IsNullOrEmpty(GroupId))
            {
                model.Condition.GroupId = GroupId;
            }

            if (!string.IsNullOrEmpty(ContractTypeId))
            {
                model.Condition.ContractTypeId = ContractTypeId;
            }
            model.Condition.List_Contract = mainService.GetListContractTypeBySearch(ContractTypeId, GetLoginUser().CompanyCode).OrderBy(m => m.display_order).ThenBy(m => m.contract_type_id).ToList();
            model.Condition.List_Group = mainService.GetListGroupBySearch(GroupId, GetLoginUser().CompanyCode).OrderBy(m => m.display_order).ThenBy(m => m.group_id).ToList();
            #region Get profit data
            var listProfitBudget = mainService.GetListProfitBudget(timeStart, timeEnd, currentUser.CompanyCode, ContractTypeId, GroupId);
            var listCost = mainService.GetListCost(timeStart, timeEnd, currentUser.CompanyCode, ContractTypeId, GroupId, checkSalesType);
            var listSalesActual = mainService.GetListSaleActual(timeStart, timeEnd, currentUser.CompanyCode, ContractTypeId, GroupId, checkSalesType);
            var profitDataList = new List<ProfitData>();

            CountChargePersonNullProfit(timeStart, timeEnd, currentUser.CompanyCode, ContractTypeId, GroupId, ref listSalesActual, checkSalesType);

            var profitData = (from pb in listProfitBudget
                              join c in listCost
                                  on new { target_year = pb.target_year, target_month = pb.target_month, group_id = pb.group_id, contract_type_id = pb.contract_type_id } equals
                                  new { target_year = c.target_year, target_month = c.target_month, group_id = c.group_id, contract_type_id = c.contract_type_id }
                                  into joinC
                              from c in joinC.DefaultIfEmpty()
                              join sa in listSalesActual
                                  on new { target_year = pb.target_year, target_month = pb.target_month, group_id = pb.group_id, contract_type_id = pb.contract_type_id } equals
                                  new { target_year = sa.target_year, target_month = sa.target_month, group_id = sa.group_id, contract_type_id = sa.contract_type_id }
                                  into joinSa
                              from sa in joinSa.DefaultIfEmpty()
                              select new ProfitData()
                              {
                                  target_year = pb.target_year,
                                  target_month = pb.target_month,
                                  group_id = pb.group_id,
                                  contract_type_id = pb.contract_type_id,
                                  group_name = pb.group_name,
                                  contract_type = pb.contract_type,
                                  profit_budget = (pb == null) ? 0 : pb.profit_budget,
                                  cost_price = (c == null) ? 0 : c.cost_price,
                                  sales_price = (sa == null) ? 0 : sa.total_sales,
                                  contract_type_display_order = (sa == null) ? 0: sa.contract_type_display_order,
                                  group_display_order = (sa == null) ? 0 : sa.group_display_order
                              }
                ).ToList();

            foreach (var profit in profitData)
            {
                profit.profit_actual = profit.sales_price - profit.cost_price;
                profit.tgrProfitRate = (profit.sales_price != 0) ? ((profit.profit_actual / profit.sales_price) * 100).ToString() : "-";
                profit.tgrSuccessRate = (profit.profit_budget != 0) ? ((profit.profit_actual / profit.profit_budget) * 100).ToString() : "-";
            }

            var profitTotalGroup = profitData.GroupBy(l => new { l.target_year, l.target_month, l.group_id })
                .Select(cl => new TotalGroupProfit
                {
                    target_year = cl.First().target_year,
                    target_month = cl.First().target_month,
                    group_id = cl.First().group_id.ToString(),
                    tgrProfitBudget = cl.Sum(c => c.profit_budget),
                    tgrCost = cl.Sum(c => c.cost_price),
                    tgrSaleActual = cl.Sum(c => c.sales_price),
                    tgrProfitActual = cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price),
                    tgrProfitRate = (cl.Sum(c => c.sales_price) != 0) ? ((cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price)) / cl.Sum(c => c.sales_price) * 100).ToString() : "-",
                    tgrSuccessRate = (cl.Sum(c => c.profit_budget) != 0) ? ((cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price)) / cl.Sum(c => c.profit_budget) * 100).ToString() : "-",
                }).ToList();

            var profitCT = profitData.GroupBy(l => new { l.target_year, l.target_month, l.contract_type_id })
                .Select(cl => new TotalContractTypeProfit
                {
                    target_year = cl.First().target_year,
                    target_month = cl.First().target_month,
                    contract_type_id = cl.First().contract_type_id.ToString(),
                    tgrProfitBudget = cl.Sum(c => c.profit_budget),
                    tgrCost = cl.Sum(c => c.cost_price),
                    tgrSaleActual = cl.Sum(c => c.sales_price),
                    tgrProfitActual = cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price),
                    tgrProfitRate = (cl.Sum(c => c.sales_price) != 0) ? ((cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price)) / cl.Sum(c => c.sales_price) * 100).ToString() : "-",
                    tgrSuccessRate = (cl.Sum(c => c.profit_budget) != 0) ? ((cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price)) / cl.Sum(c => c.profit_budget) * 100).ToString() : "-",
                }).ToList();

            var profitTotalMonth = profitData.GroupBy(l => new { l.target_year, l.target_month })
                .Select(cl => new TotalGrAllProfit
                {
                    target_year = cl.First().target_year,
                    target_month = cl.First().target_month,
                    tgrProfitBudget = cl.Sum(c => c.profit_budget),
                    tgrCost = cl.Sum(c => c.cost_price),
                    tgrSaleActual = cl.Sum(c => c.sales_price),
                    tgrProfitActual = cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price),
                    tgrProfitRate = (cl.Sum(c => c.sales_price) != 0) ? ((cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price)) / cl.Sum(c => c.sales_price) * 100).ToString() : "-",
                    tgrSuccessRate = (cl.Sum(c => c.profit_budget) != 0) ? ((cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price)) / cl.Sum(c => c.profit_budget) * 100).ToString() : "-",
                }).ToList();

            var profitTotalGroupAll = profitData.GroupBy(l => new { l.group_id })
                .Select(cl => new TotalGrAllProfit
                {
                    group_id = cl.First().group_id.ToString(),
                    tgrProfitBudget = cl.Sum(c => c.profit_budget),
                    tgrCost = cl.Sum(c => c.cost_price),
                    tgrSaleActual = cl.Sum(c => c.sales_price),
                    tgrProfitActual = cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price),
                    tgrProfitRate = (cl.Sum(c => c.sales_price) != 0) ? ((cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price)) / cl.Sum(c => c.sales_price) * 100).ToString() : "-",
                    tgrSuccessRate = (cl.Sum(c => c.profit_budget) != 0) ? ((cl.Sum(c => c.sales_price) - cl.Sum(c => c.cost_price)) / cl.Sum(c => c.profit_budget) * 100).ToString() : "-",
                }).ToList();

            var profitDataTemp = new List<ProfitData>();
            if (profitData != null && profitData.Count > 0)
            {
                foreach (var item in profitData)
                {
                    profitDataTemp.Add(item);
                }
            }

            var groupListProfit = new List<Group>();
            var contractTypeListProfit = new List<ContractType>();
            var timeListProfit = new List<TimeList>();

            for (int i = 0; i < profitDataTemp.Count; i++)
            {
                var grItem = new Group();
                grItem.display_name = profitDataTemp[i].group_name ?? mainService.GetGroupName(currentUser.CompanyCode, profitDataTemp[i].group_id);
                grItem.group_id = Convert.ToInt32(profitDataTemp[i].group_id);
                grItem.display_order = Convert.ToInt32(profitDataTemp[i].group_display_order);
                var chk = true;
                for (int j = 0; j < groupListProfit.Count; j++)
                {
                    if (groupListProfit[j].display_name == grItem.display_name && groupListProfit[j].group_id == grItem.group_id)
                    {
                        chk = false;
                        break;
                    }
                }

                if (chk)
                {
                    groupListProfit.Add(grItem);
                }

                var ctItem = new ContractType();
                ctItem.contract_type = profitDataTemp[i].contract_type ?? mainService.GetContractTypeName(currentUser.CompanyCode, profitDataTemp[i].contract_type_id);
                ctItem.contract_type_id = profitDataTemp[i].contract_type_id;
                ctItem.display_order = Convert.ToInt32(profitDataTemp[i].contract_type_display_order);
                chk = true;

                for (int j = 0; j < contractTypeListProfit.Count; j++)
                {
                    if (contractTypeListProfit[j].contract_type == ctItem.contract_type && contractTypeListProfit[j].contract_type_id == ctItem.contract_type_id)
                    {
                        chk = false;
                        break;
                    }
                }

                if (chk)
                {
                    contractTypeListProfit.Add(ctItem);
                }

                var tlItem = new TimeList();
                tlItem.year = profitDataTemp[i].target_year.ToString();
                tlItem.month = profitDataTemp[i].target_month.ToString();
                chk = true;
                for (int j = 0; j < timeListProfit.Count; j++)
                {
                    if (timeListProfit[j].month == tlItem.month && timeListProfit[j].year == tlItem.year)
                    {
                        chk = false;
                        break;
                    }
                }

                if (chk)
                {
                    timeListProfit.Add(tlItem);
                }
            }

            model.GroupListProfit = groupListProfit.OrderBy(m => m.display_order).ThenBy(m => m.group_id).ToList();
            model.ContractTypeListProfit = contractTypeListProfit.OrderBy(m => m.display_order).ThenBy(m => m.contract_type_id).ToList();
            model.DataProfitBudget = profitData;
            model.TimeListProfit = timeListProfit;
            model.TotalGroupProfit = profitTotalGroup;
            model.TotalCTProfit = profitCT;
            model.TotalMonthProfit = profitTotalMonth;
            model.TotalGrYearListProfit = profitTotalGroupAll;
            model.CheckSalesType = checkSalesType;
            return model;
        }

        /// <summary>
        /// Count Charge Person Null for Profit Mode
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="ContractTypeId"></param>
        /// <param name="GroupId"></param>
        /// <param name="listSales"></param>
        /// <param name="checkSalesType"></param>
        public void CountChargePersonNullProfit(string timeStart, string timeEnd, string companyCode, string ContractTypeId, string GroupId, ref IList<SalesPrice> listSales, string checkSalesType)
        {
            var listChargePersonNull = mainService.GetListChargePersonNull(timeStart, timeEnd, companyCode, ContractTypeId, GroupId, checkSalesType);
            if (listChargePersonNull != null)
            {
                List<IDictionary<string, object>> cpnListTemp = new List<IDictionary<string, object>>();
                foreach (var item in listChargePersonNull)
                {
                    cpnListTemp.Add(item);
                }

                for (int j = 0; j < cpnListTemp.Count; j++)
                {
                    var targetMonth = cpnListTemp[j].Values.ElementAt(0).ToString();
                    var targetYear = cpnListTemp[j].Values.ElementAt(1).ToString();
                    var salesActual = cpnListTemp[j].Values.ElementAt(2).ToString();
                    var ctId = cpnListTemp[j].Values.ElementAt(3).ToString();
                    var grId = cpnListTemp[j].Values.ElementAt(4).ToString();
                    for (int i = 0; i < listSales.Count; i++)
                    {
                        if (listSales[i].contract_type_id.ToString() == ctId && listSales[i].group_id.ToString() == grId && listSales[i].target_month.ToString() == targetMonth && listSales[i].target_year.ToString() == targetYear)
                        {
                            listSales[i].total_sales = listSales[i].total_sales + Convert.ToInt32(salesActual);
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Excel download
        /// <summary>
        /// Download Xlsx file
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="GroupId"></param>
        /// <param name="ContractTypeId"></param>
        /// <param name="checkSalesType"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public ActionResult DownloadXlsxFileSales(string timeStart, string timeEnd, string GroupId, string ContractTypeId, string checkSalesType, string tabId)
        {
            PMS11003ListViewModel dataSales = new PMS11003ListViewModel();
            PMS11003ListViewModelPlus dataProfit = new PMS11003ListViewModelPlus();
            if (Session["SalesBudgetData" + tabId] != null)
            {
                dataSales = Session["SalesBudgetData" + tabId] as PMS11003ListViewModel;
            }
            else
            {
                dataSales = CreateSalesBudgetModel(timeStart, timeEnd, GroupId, ContractTypeId, checkSalesType);
            }

            if (Session["SalesBudgetData" + tabId] != null)
            {
                dataProfit = Session["ProfitBudgetData" + tabId] as PMS11003ListViewModelPlus;
            }
            else
            {
                dataProfit = CreateProfitBudgetModel(timeStart, timeEnd, GroupId, ContractTypeId, checkSalesType);
            }
            
            DataSalesProfitExport data = new DataSalesProfitExport();
            data.data_sale = dataSales;
            data.data_profit = dataProfit;
            Utility.DownloadXlsxFile(this, data, "所属別予実一覧.xlsx", "所属別予実一覧.xlsx");
            return new EmptyResult();
        }

        /// <summary>
        /// Clear search result data when close page
        /// </summary>
        /// <param name="TAB_ID"></param>
        /// <returns></returns>
        public ActionResult ClearSearchResult(string TAB_ID)
        {
            Session.Remove("SalesBudgetData" + TAB_ID);
            Session.Remove("ProfitBudgetData" + TAB_ID);
            return new EmptyResult();
        }

        #endregion
        #endregion
    }
}
