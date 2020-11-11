#region License
/// <copyright file="PMS09002Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/05</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS09002;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS09002;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with sale list by personal
    /// </summary>
    public class PMS09002Controller : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// Common service
        /// </summary>
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Main service
        /// </summary>
        private readonly IPMS09002Service mainService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS09002Controller(IPMS09002Service service, IPMSCommonService commonservice)
        {
            this.mainService = service;
            this.commonService = commonservice;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09002Controller()
            : this(new PMS09002Service(), new PMSCommonService())
        {
        }

        #endregion

        #region Action
        /// <summary>
        /// Clear save condition
        /// </summary>
        /// <returns>Index</returns>
        [HttpGet]
        public ActionResult ClearSaveCondition()
        {
            base.ClearRestoreData();
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.SalesPersonal)
                && !this.IsInFunctionList(Constant.FunctionID.SalesPersonal_Admin))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var currentUser = GetLoginUser();
            var model = new PMS09002ListViewModel
            {
                GROUP_LIST = this.commonService.GetUserGroupSelectList(currentUser.CompanyCode),
                CONTRACT_TYPE_LIST = this.commonService.GetContractTypeSelectList(currentUser.CompanyCode),
                BRANCH_LIST = this.commonService.GetBranchSelectList(currentUser.CompanyCode)
            };

            model.Condition.GROUP_ID = currentUser.GroupId;

            ViewBag.iDisplayLength = ConfigurationManager.AppSettings[ConfigurationKeys.LIST_ITEMS_PER_PAGE];

            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS09002/Detail") && Session[Constant.SESSION_IS_BACK] != null)
            {
                var tmpCondition = GetRestoreData() as Condition;

                if (tmpCondition != null)
                    model.Condition = tmpCondition;

                var iDisplayLength = Session["PMS09002_DisplayLength"] as int?;
                if (iDisplayLength != null)
                {
                    ViewBag.iDisplayLength = iDisplayLength;
                }
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("List", model);
        }

        /// <summary>
        /// Detail
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <param name="targetYear">Target year</param>
        /// <param name="targetMonth">Target month</param>
        /// <param name="locationName">locationName</param>
        /// <returns>Detail view</returns>
        [HttpPost]
        public ActionResult Detail(int userID = 0, int targetYear = 0, int targetMonth = 0, string contractTypeID = "", string contractTypeName = "", string locationName ="", string sendFromScreen = "")
        {
            if (userID == 0 || targetYear == 0 || targetMonth == 0)
                return this.RedirectToAction("Index", "Error");

            var model = new PMS09002DetailViewModel
            {
                UserID = userID,
                TargetYear = targetYear,
                TargetMonth = targetMonth,
                ContractTypeName = contractTypeName,
                ContractTypeID = contractTypeID,
                UserName = this.mainService.GetUserName(GetLoginUser().CompanyCode, userID),
                LocationName = locationName
            };
            ViewBag.SendFromScreen = sendFromScreen;
            return this.View("Detail", model);
        }


        /// <summary>
        /// Export data to Csv file
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="TargetYear">Target year</param>
        /// <param name="TargetMonth">Target month</param>
        /// <param name="sortCol">Sort column</param>
        /// <param name="sortDir">Sort direction</param>
        /// <param name="ContractTypeID">Contract type id</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>EmptyResult</returns>
        [HttpPost]
        public ActionResult ExportCsv(int UserID = 0, int TargetYear = 0, int TargetMonth = 0, int sortCol = 0, string sortDir = "asc", string ContractTypeID = "", string TAB_ID = "")
        {
            if (UserID > 0 && TargetYear > 0 && TargetMonth > 0)
            {
                List<string> titles = new List<string>()
                {
                    "プロジェクト名",
                    "売上",
                    "原価",
                    "利益"
                };

                DataTablesModel model = new DataTablesModel
                {
                    sEcho = "1",
                    iColumns = 5,
                    sColumns = "project_name,project_name,project_sales,actual_cost,profit",
                    iDisplayStart = 0,
                    iDisplayLength = int.MaxValue,
                    iSortCol_0 = sortCol,
                    sSortDir_0 = sortDir,
                    iSortingCols = 1
                };

                SalesDetailByPersonalCondition condition = new SalesDetailByPersonalCondition
                {
                    CompanyCode = GetLoginUser().CompanyCode,
                    UserID = UserID,
                    SelectedYear = TargetYear,
                    SelectedMonth = TargetMonth,
                    ContractTypeID = ContractTypeID
                };

                var salesDetails = new List<SalesDetailByPersonal>();
                if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
                {
                    salesDetails = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as List<SalesDetailByPersonal>;
                }
                else
                {
                    salesDetails = this.mainService.GetSalesDetailByPersonal(model, condition).Items;
                }
                IList<SalesDetailByPersonalExportCSV> datalist = new List<SalesDetailByPersonalExportCSV>();

                foreach (var data in salesDetails)
                {
                    datalist.Add(new SalesDetailByPersonalExportCSV()
                    {
                        project_name = data.project_name,
                        sales = data.project_sales.ToString("#,##0"),
                        cost = data.actual_cost.ToString("#,##0"),
                        profit = (data.project_sales - data.actual_cost).ToString("#,##0")
                    });
                }

                string fileName = "PersonalSalesDetail_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
                DataTable dt = Utility.ToDataTableT(datalist, titles.ToArray());
                Utility.ExportToCsvData(this, dt, fileName);
            }

            return new EmptyResult();
        }

        #endregion

        #region Ajax Action
        /// <summary>
        /// Search by condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Json list information</returns>
        public ActionResult Search(DataTablesModel model, Condition condition)
        {
            if (Request.IsAjaxRequest())
            {
                Session["PMS09002_DisplayLength"] = model.iDisplayLength;
                DateTime from;
                DateTime to;
                string sort_type = condition.SELECT_SORT_TYPE;
                if (DateTime.TryParse(condition.FROM_DATE, out from) && DateTime.TryParse(condition.TO_DATE, out to))
                {
                    condition.SORT_COL = model.iSortCol_0;
                    condition.SORT_TYPE = model.sSortDir_0;
                    string companyCode = GetLoginUser().CompanyCode;

                    var individualSalesObjList = this.mainService.GetIndividualSalesList(from, to, companyCode, condition);
                    var actualSalesObjList = this.mainService.GetActualSalesList(from, to, companyCode, condition);

                    List<IDictionary<string, object>> individualSalesTemp = new List<IDictionary<string, object>>();
                    List<IDictionary<string, object>> actualSalesTemp = new List<IDictionary<string, object>>();

                    if (individualSalesObjList != null)
                    {
                        foreach (var item in individualSalesObjList)
                        {
                            individualSalesTemp.Add(item);
                        }
                    }

                    if (actualSalesObjList != null)
                    {
                        foreach (var item in actualSalesObjList)
                        {
                            actualSalesTemp.Add(item);
                        }
                    }

                    int totalItem = individualSalesTemp.Count();
                    List<object[]> dataList = new List<object[]>();

                    if (totalItem > 0)
                    {
                        int keyCount = individualSalesTemp[0].Keys.Count;
                        for (int i = 0; i < totalItem; i++)
                        {
                            object[] obj = new object[keyCount + 1];
                            int inc = 5;
                            string deleted = individualSalesTemp[i].Values.ElementAt(6).ToString() == Constant.DeleteFlag.DELETE ? " delete-row" : string.Empty; // is deleted data
                            string groupName = this.EncodeValue(individualSalesTemp[i].Values.ElementAt(4));
                            string userName = this.EncodeValue(individualSalesTemp[i].Values.ElementAt(5));
                            string locationName = this.EncodeValue(individualSalesTemp[i].Values.ElementAt(7));
                            obj[0] = i + 1; // index
                            obj[1] = individualSalesTemp[i].Values.ElementAt(2); // ID
                            obj[2] = "<div class='short-text text-overflow' title='" + groupName + "'>" + groupName + "</div>";
                            obj[3] = "<div class='short-text text-overflow" + deleted + "' title='" + userName + "'>" + userName + "</div>";
                            obj[4] = "<div class='short-text text-overflow" + "' title='" + locationName + "'>" + locationName + "</div>";
                            int indexMap = -1;
                            for (int j = 0; j < actualSalesTemp.Count; j++)
                            {
                                // coincidence group ID,location ID and user ID
                                if (Convert.ToInt32(actualSalesTemp[j].Values.ElementAt(0)) == Convert.ToInt32(individualSalesTemp[i].Values.ElementAt(0))
                                    && Convert.ToInt32(actualSalesTemp[j].Values.ElementAt(1)) == Convert.ToInt32(individualSalesTemp[i].Values.ElementAt(1))
                                    && Convert.ToInt32(actualSalesTemp[j].Values.ElementAt(2)) == Convert.ToInt32(individualSalesTemp[i].Values.ElementAt(2)))
                                {
                                    indexMap = j;
                                    break;
                                }
                            }

                            for (int k = 8; k < keyCount; k++)
                            {
                                obj[inc] = this.CheckValue(individualSalesTemp[i].Values.ElementAt(k)) + '/' + (0 > indexMap ? "0" : this.CheckValue(actualSalesTemp[indexMap].Values.ElementAt(k)));

                                inc++;
                            }

                            switch (sort_type)
                            {
                                case SortTypeSummarySale.Earnings:
                                    // select sort = sales
                                    int totalSales = 0;
                                    obj[obj.Length - 1] = 0;

                                    for (int j = 5; j < keyCount; j++)
                                    {
                                        if (obj[j]!=null)
                                        {
                                            totalSales += Int32.Parse(obj[j].ToString().Split('/')[0]);
                                        }
                                    }

                                    obj[obj.Length - 1] = totalSales;
                                    dataList.Add(obj);
                                    break;
                                case SortTypeSummarySale.PROFIT:
                                    // select sort = profit
                                    int totalProfit = 0;
                                    obj[obj.Length - 1] = 0;

                                    for (int j = 5; j < keyCount; j++)
                                    {
                                        if (obj[j]!=null)
                                        {
                                            int individualSales = obj[j].ToString().Split('/')[0] != null ? Int32.Parse(obj[j].ToString().Split('/')[0]) : 0;
                                            int actualSales = obj[j].ToString().Split('/')[1] != null ? Int32.Parse(obj[j].ToString().Split('/')[1]) : 0;
                                            totalProfit += (individualSales - actualSales);
                                        }  
                                    }

                                    obj[obj.Length - 1] = totalProfit;
                                    dataList.Add(obj);
                                    break;
                                case SortTypeSummarySale.PROFIT_RATE:
                                    // select sort = profit rate
                                    int totalSales_sortRate = 0;
                                    int totalProfit_sortRate = 0;
                                    double totalProfitRate = 0;
                                    obj[obj.Length - 1] = 0;

                                    for (int j = 5; j < keyCount; j++)
                                    {
                                        if (obj[j]!=null)
                                        {
                                            totalSales_sortRate += Int32.Parse(obj[j].ToString().Split('/')[0]);
                                            int individualSales = obj[j].ToString().Split('/')[0] != null ? Int32.Parse(obj[j].ToString().Split('/')[0]) : 0;
                                            int actualSales = obj[j].ToString().Split('/')[1] != null ? Int32.Parse(obj[j].ToString().Split('/')[1]) : 0;
                                            totalProfit_sortRate += (individualSales - actualSales);
                                        }
                                    }

                                    if (totalSales_sortRate != 0)
                                    {
                                        var rate = (Convert.ToDouble(totalProfit_sortRate) / Convert.ToDouble(totalSales_sortRate));
                                        totalProfitRate = rate * 100;
                                    }

                                    obj[obj.Length - 1] = totalProfitRate;
                                    dataList.Add(obj);
                                    break;
                                default:
                                    dataList.Add(obj);
                                    break;
                            }
                        }
                    }
                    if (sort_type != "")
                    {
                        object[] tmp;
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            for (int j = 0; j < dataList.Count; j++)
                            {
                                if (sort_type == SortTypeSummarySale.PROFIT_RATE)
                                {
                                    double num1 = double.Parse(dataList[i][dataList[i].Count() - 1].ToString());
                                    double num2 = double.Parse(dataList[j][dataList[j].Count() - 1].ToString());
                                    if (num1 > num2)
                                    {
                                        tmp = dataList[i];
                                        dataList[i] = dataList[j];
                                        dataList[j] = tmp;
                                    }
                                }
                                else
                                {
                                    int num1 = int.Parse(dataList[i][dataList[i].Count() - 1].ToString());
                                    int num2 = int.Parse(dataList[j][dataList[j].Count() - 1].ToString());
                                    if (num1 > num2)
                                    {
                                        tmp = dataList[i];
                                        dataList[i] = dataList[j];
                                        dataList[j] = tmp;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < dataList.Count; i++)
                        {
                            dataList[i][0] = i + 1;
                        }
                    }

                    List<object[]> dataListObj = new List<object[]>();
                    if (dataList.Count > 0)
                    {
                        int start = model.iDisplayStart;
                        int end = model.iDisplayStart + model.iDisplayLength;
                        if (end > totalItem)
                        {
                            end = totalItem;
                        }

                        for (int i = start; i < end; i++)
                        {
                            dataListObj.Add(dataList[i]);
                        }
                    }

                    var result = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = totalItem,
                            iTotalDisplayRecords = totalItem,
                            aaData = dataListObj.ToList<object[]>()
                        },
                        JsonRequestBehavior.AllowGet);

                    SaveRestoreData(condition);

                    return result;
                }
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Get sales detail by personal
        /// </summary>
        /// <param name="model">data table model</param>
        /// <param name="condition">search condition</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>JSON sales detail by personal</returns>
        public ActionResult SalesDetail(DataTablesModel model, SalesDetailByPersonalCondition condition, string TAB_ID)
        {
            if (Request.IsAjaxRequest() && ModelState.IsValid)
            {
                condition.CompanyCode = GetLoginUser().CompanyCode;
                var pageInfo = this.mainService.GetSalesDetailByPersonal(model, condition);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = pageInfo.Items;
                decimal totalSales = 0;
                decimal totalCost = 0;
                decimal totalProfit = 0;

                foreach (var data in pageInfo.Items)
                {
                    totalSales += data.project_sales;
                    totalCost += data.actual_cost;
                    totalProfit += data.profit;
                }

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = (from t in pageInfo.Items select new object[]
                            {
                                t.project_no,
                                HttpUtility.HtmlEncode(t.project_name),
                                t.project_sales.ToString("#,##0") + "円",
                                t.actual_cost.ToString("#,##0") + "円",
                                t.profit.ToString("#,##0") + "円",
                            }).ToList(),
                        totalSales = totalSales.ToString("#,##0") + "円",
                        totalCost = totalCost.ToString("#,##0") + "円",
                        totalProfit = totalProfit.ToString("#,##0") + "円"
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
            return new EmptyResult();
        }

        #endregion

        #region Method
        /// <summary>
        /// Check value is null
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>value</returns>
        private string CheckValue(object value)
        {
            if (value != null)
                return Convert.ToString(value);

            return Constant.DEFAULT_VALUE;
        }

        /// <summary>
        /// Encode value
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>Value encoded</returns>
        private string EncodeValue(object value)
        {
            if (value != null)
                return HttpUtility.HtmlEncode(value.ToString());

            return string.Empty;
        }

        #endregion
    }
}
