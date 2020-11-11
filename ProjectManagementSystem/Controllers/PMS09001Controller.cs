#region License
/// <copyright file="PMS09001Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS09001;
    using ProjectManagementSystem.ViewModels;
    using ProjectManagementSystem.ViewModels.PMS09001;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Work with sale list by group
    /// </summary>
    public class PMS09001Controller : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// Common service
        /// </summary>
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Main service
        /// </summary>
        private readonly IPMS09001Service mainService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS09001Controller(IPMS09001Service service, IPMSCommonService commonservice)
        {
            this.mainService = service;
            this.commonService = commonservice;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09001Controller()
            : this(new PMS09001Service(), new PMSCommonService())
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
            if (!this.IsInFunctionList(Constant.FunctionID.SalesGroup)
                && !this.IsInFunctionList(Constant.FunctionID.SalesGroup_Admin))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var currentUser = GetLoginUser();
            var model = new PMS09001ListViewModel
            {
                GROUP_LIST = this.commonService.GetUserGroupSelectList(currentUser.CompanyCode),
                CONTRACT_TYPE_LIST = this.commonService.GetContractTypeSelectList(currentUser.CompanyCode),
                BRANCH_LIST = this.commonService.GetBranchSelectList(currentUser.CompanyCode)
            };

            model.Condition.GROUP_ID = currentUser.GroupId.HasValue ? currentUser.GroupId.Value.ToString() : string.Empty;

            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS09001/SalesGroupDetail") && Session[Constant.SESSION_IS_BACK] != null)
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
        /// Sales Group Detail By Project
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SalesGroupDetail()
        {
            if (!this.IsInFunctionList(Constant.FunctionID.SalesGroup)
                && !this.IsInFunctionList(Constant.FunctionID.SalesGroup_Admin))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS09001/SalesProjectDetail"))
            {
                var condition = Session["PMS09001_SalesGroupDetail_Condition"] as SalesGroupDetailCondition;
                var groupName = Session["PMS09001_SalesGroupDetail_GroupName"] as string;
                var contractTypeName = Session["PMS09001_SalesGroupDetail_ContractTypeName"] as string;
                var locationName = Session["PMS09001_SalesGroupDetail_LocationName"] as string;
                var contractTypeID = Session["PMS09001_SalesGroupDetail_ContractTypeID"] as string;
                var model = new PMS09001SalesGroupDetailViewModel
                {
                    GroupName = groupName,
                    ContractTypeName = contractTypeName,
                    LocationName = locationName,
                    ContractTypeID = contractTypeID,
                    Condition = condition
                };
                return this.View("SalesGroupDetail", model);
            }
            else
            {
                return new EmptyResult();
            }
        }
        /// <summary>
        /// Sales Group Detail By Project
        /// Build condition to filter details
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="selected_year"></param>
        /// <param name="selected_month"></param>
        /// <param name="locationID"></param>
        /// <param name="locationName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SalesGroupDetail(int group_id = -1, int selected_year = -1, int selected_month = -1, string contractTypeID = "", string locationID = "", string contractTypeName = "", string locationName = "")
        {
            if (!this.IsInFunctionList(Constant.FunctionID.SalesGroup)
                && !this.IsInFunctionList(Constant.FunctionID.SalesGroup_Admin))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var condition = new SalesGroupDetailCondition()
            {
                GroupId = group_id,
                SelectedMonth = selected_month,
                SelectedYear = selected_year,
                CompanyCode = GetLoginUser().CompanyCode,
                LocationID = locationID
            };

            var model = new PMS09001SalesGroupDetailViewModel
            {
                GroupName = group_id == 0 ? "該当なし" : this.mainService.GetGroupName(group_id),
                ContractTypeName = contractTypeName,
                LocationName = locationName,
                ContractTypeID = contractTypeID,
                Condition = condition
            };

            Session["PMS09001_SalesGroupDetail_Condition"] = condition;
            Session["PMS09001_SalesGroupDetail_GroupName"] = model.GroupName;
            Session["PMS09001_SalesGroupDetail_ContractTypeName"] = contractTypeName;
            Session["PMS09001_SalesGroupDetail_LocationName"] = locationName;
            Session["PMS09001_SalesGroupDetail_ContractTypeID"] = contractTypeID;

            return this.View("SalesGroupDetail", model);
        }

        /// <summary>
        /// Sales Group Detail By Project
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="selected_year"></param>
        /// <param name="selected_month"></param>
        /// <returns></returns>
        public ActionResult SalesProjectDetail(int group_id = -1, int project_id = -1, int selected_year = -1, int selected_month = -1)
        {
            if (!this.IsInFunctionList(Constant.FunctionID.SalesGroup)
                && !this.IsInFunctionList(Constant.FunctionID.SalesGroup_Admin))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var condition = new SalesProjectDetailCondition()
            {
                ProjectId = project_id,
                GroupId = group_id,
                SelectedMonth = selected_month,
                SelectedYear = selected_year,
                CompanyCode = GetLoginUser().CompanyCode
            };
            var model = new PMS09001SalesProjectDetailViewModel
            {
                BasicInfo = this.mainService.GetProjectDetailBasicInfo(group_id, project_id),
                Condition = condition
            };

            return this.View("SalesProjectDetail", model);
        }


        /// <summary>
        /// Export Group Detail CSV
        /// </summary>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="project_name"></param>
        /// <param name="customer_id"></param>
        /// <param name="tag_id"></param>
        /// <param name="eff_type"></param>
        /// <param name="sort_colum"></param>
        /// <param name="sort_type"></param>
        /// <param name="DelFlag"></param>
        /// <param name="status"></param>
        /// <param name="TAB_ID"></param>
        /// <returns></returns>
        public ActionResult ExportGroupDetailCsv(int group_id, int selected_year, int selected_month, int sortCol = 0, string sortDir = "asc", string ContractTypeID = "", string TAB_ID = "")
        {
            var condition = new SalesGroupDetailCondition()
            {
                CompanyCode = GetLoginUser().CompanyCode,
                GroupId = group_id,
                SelectedYear = selected_year,
                SelectedMonth = selected_month,
                ContractTypeID = ContractTypeID
            };

            DataTablesModel model = new DataTablesModel
            {
                sEcho = "1",
                iColumns = 5,
                sColumns = "project_sys_id,project_name,sales_amount,cost,profit",
                iDisplayStart = 0,
                iDisplayLength = int.MaxValue,
                iSortCol_0 = sortCol,
                sSortDir_0 = sortDir,
                iSortingCols = 1
            };

            var listGroupDetail = new List<ProjectSalesInfo>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                listGroupDetail = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as List<ProjectSalesInfo>;
            }
            else
            {
                listGroupDetail = this.mainService.GetListSalesGroupDetail(model, GetLoginUser().CompanyCode, condition).Items;
            }
            var listExport = new List<ProjectSalesInfoExport>();

            foreach (var t in listGroupDetail)
            {
                listExport.Add(new ProjectSalesInfoExport()
                {
                    project_name = t.project_name,
                    sales_amount = Utility.RoundNumber(t.sales_amount, GetLoginUser().DecimalCalculationType, false).ToString("#,##0"),
                    cost = Utility.RoundNumber(t.cost, GetLoginUser().DecimalCalculationType, false).ToString("#,##0"),
                    profit = Utility.RoundNumber(t.profit, GetLoginUser().DecimalCalculationType, false).ToString("#,##0"),
                });
            }
            List<string> columns = new List<string>() {
                "プロジェクト名",
                "売上",
                "原価",
                "利益"
            };

            DataTable dataTable = Utility.ToDataTableT(listExport, columns.ToArray());
            Utility.ExportToCsvData(this, dataTable, "SalesGroupDetail_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv");
            return new EmptyResult();
        }

        /// <summary>
        /// Export Project Detail CSV
        /// </summary>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="project_name"></param>
        /// <param name="customer_id"></param>
        /// <param name="tag_id"></param>
        /// <param name="eff_type"></param>
        /// <param name="sort_colum"></param>
        /// <param name="sort_type"></param>
        /// <param name="DelFlag"></param>
        /// <param name="status"></param>
        /// <param name="TAB_ID"></param>
        /// <returns></returns>
        public ActionResult ExportProjectDetailCsv(int project_id, int group_id, int selected_year, int selected_month, int sortCol = 0, string sortDir = "asc", string TAB_ID = "")
        {
            var condition = new SalesProjectDetailCondition()
            {
                CompanyCode = GetLoginUser().CompanyCode,
                GroupId = group_id,
                ProjectId = project_id,
                SelectedYear = selected_year,
                SelectedMonth = selected_month
            };

            DataTablesModel model = new DataTablesModel
            {
                sEcho = "1",
                iColumns = 6,
                sColumns = "user_sys_id,user_sys_id,user_name,sales_amount,cost,profit",
                iDisplayStart = 0,
                iDisplayLength = int.MaxValue,
                iSortCol_0 = sortCol,
                sSortDir_0 = sortDir,
                iSortingCols = 1
            };

            var listGroupDetail = new List<PersonalSalesInfo>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                listGroupDetail = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as List<PersonalSalesInfo>;
            }
            else
            {
                listGroupDetail = this.mainService.GetListSalesProjectDetail(model, condition).Items;
            }
            var listExport = new List<PersonalSalesInfoExport>();

            foreach (var t in listGroupDetail)
            {
                listExport.Add(new PersonalSalesInfoExport()
                {
                    no = t.peta_rn,
                    user_name = t.user_name,
                    sales_amount = Utility.RoundNumber(t.sales_amount, GetLoginUser().DecimalCalculationType, false).ToString("#,##0"),
                    cost = Utility.RoundNumber(t.cost, GetLoginUser().DecimalCalculationType, false).ToString("#,##0"),
                    profit = Utility.RoundNumber(t.profit, GetLoginUser().DecimalCalculationType, false).ToString("#,##0"),
                });
            }
            List<string> columns = new List<string>() {
                "No.",
                "ユーザー名",
                "売上",
                "原価",
                "利益"
            };

            DataTable dataTable = Utility.ToDataTableT(listExport, columns.ToArray());
            Utility.ExportToCsvData(this, dataTable, "SalesProjectDetail_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv");
            return new EmptyResult();
        }
        #endregion

        #region Ajax Action
        /// <summary>
        /// Search sales by group -- update searh branch condition
        /// </summary>
        /// <param name="model">dataTable info</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Json sales by group</returns>
        public ActionResult Search(DataTablesModel model, Condition condition)
        {
            if (Request.IsAjaxRequest())
            {
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

                    List<object[]> dataList = new List<object[]>();
                    List<IDictionary<string, object>> individualSalesTemp = new List<IDictionary<string, object>>();
                    List<IDictionary<string, object>> actualSalesTemp = new List<IDictionary<string, object>>();
                    List<IDictionary<string, object>> nonActualSalesData = new List<IDictionary<string, object>>();
                    List<int> nonActualSalesPos = new List<int>();
                    List<IDictionary<string, object>> totalSalesTemp = new List<IDictionary<string, object>>();
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
                    #region Merge data to one list of dictionary
                    //Find non-actual sale data 
                    if (individualSalesTemp.Count != actualSalesTemp.Count)
                    {
                        foreach (var individualSalesObj in individualSalesTemp)
                        {
                            if (actualSalesTemp.Find(x => Convert.ToInt32(x.Values.ElementAt(0)) == Convert.ToInt32(individualSalesObj.Values.ElementAt(0))) == null)
                            {
                                nonActualSalesData.Add(individualSalesObj); // save non-actual sale item
                                nonActualSalesPos.Add(individualSalesTemp.IndexOf(individualSalesObj)); // save index of non-actual sale item
                            }
                        }
                    }

                    //Remove non-actual sale data in individualSalesTemp
                    if(nonActualSalesData.Count > 0)
                    {
                        foreach(var nonActualSalesItem in nonActualSalesData)
                        {
                            individualSalesTemp.Remove(nonActualSalesItem);
                        }
                    }

                    for (int i = 0; i <= individualSalesTemp.Count-1; i++)
                    {
                        var individualSalesObj = individualSalesTemp.ElementAt(i);
                        var actualSalesObj = actualSalesTemp.ElementAt(i);
                        //combine two dictionary to one and add to totalSalesTemp
                        IDictionary<string, object> combineDic = new Dictionary<string, object>();

                        for(int j = 0; j< 3; j++)//add group_id, location_id, display_name,display_order to dictionary
                        {
                            combineDic.Add(individualSalesObj.Keys.ElementAt(j), individualSalesObj.Values.ElementAt(j));
                        }

                        for(int k = 3; k < individualSalesTemp[0].Keys.Count; k++) //add data of all months to dictionary
                        {
                            double individualSales = Convert.ToDouble(individualSalesObj.Values.ElementAt(k));
                            double actualSales = Convert.ToDouble(actualSalesObj.Values.ElementAt(k));
                            string totalSales = individualSales +"/"+ actualSales;
                            combineDic.Add(individualSalesObj.Keys.ElementAt(k), totalSales);
                        }
                        totalSalesTemp.Add(combineDic);
                    }

                    //Add non-actual sale data in totalSalesTemp at correct position
                    if (nonActualSalesData.Count > 0)
                    {
                        for (int i = 0; i < nonActualSalesData.Count; i++)
                        {
                            IDictionary<string, object> combineDic = new Dictionary<string, object>();
                            for (int j = 0; j < 3; j++) //add group_id, location_id, display_name,display_order to dictionary
                            {
                                combineDic.Add(nonActualSalesData.ElementAt(i).Keys.ElementAt(j), nonActualSalesData.ElementAt(i).Values.ElementAt(j));
                            }
                            for (int k = 3; k < individualSalesTemp[0].Keys.Count; k++) //add data of all months to dictionary
                            {
                                double individualSales = Convert.ToDouble(nonActualSalesData.ElementAt(i).Values.ElementAt(k));
                                double actualSales = 0;
                                string totalSales = individualSales + "/" + actualSales;
                                combineDic.Add(nonActualSalesData.ElementAt(i).Keys.ElementAt(k), totalSales);
                            }
                            totalSalesTemp.Insert(nonActualSalesPos.ElementAt(i),combineDic);
                        }
                    }
                    #endregion
                    #region Convert to object array
                    int keyCount = totalSalesTemp[0].Keys.Count;
                    foreach (Dictionary<string,object> dic in totalSalesTemp)
                    {
                        object[] obj = new object[keyCount + 1];

                        obj[1] = dic.Values.ElementAt(0); // id
                        string groupName = this.EncodeValue(dic.Values.ElementAt(1));
                        obj[2] = "<div class='short-text text-overflow' title='" + groupName + "'>" + groupName + "</div>"; //groupName Html
                        for (int i = 3; i < keyCount; i++)// data of all months
                        {
                            if(!string.IsNullOrEmpty(dic.Values.ElementAt(i).ToString())){
                                obj[i] = dic.Values.ElementAt(i);
                            }
                            else
                            {
                                obj[i] = "0/0";
                            }
                        }
                        dataList.Add(obj);
                    }

                    #endregion
                    //Count to Sort
                    foreach (var item in dataList)
                    {
                        switch (sort_type)
                        {
                            case SortTypeSummarySale.Earnings:
                                // select sort = sales
                                int totalSales = 0;
                                item[item.Length - 1] = 0;
                                for (int j = 3; j < item.Count() - 1; j++)
                                {
                                    totalSales += item[j] == null ? 0 : Int32.Parse(item[j].ToString().Split('/')[0]);
                                }
                                item[item.Length - 1] = totalSales;
                                
                                break;
                            case SortTypeSummarySale.PROFIT:
                                // select sort = profit
                                int totalProfit = 0;
                                item[item.Length - 1] = 0;
                                for (int j = 3; j < item.Count() - 1; j++)
                                {
                                    int individualSales = item[j] != null ? Int32.Parse(item[j].ToString().Split('/')[0]) : 0;
                                    int actualSales = item[j] != null ? Int32.Parse(item[j].ToString().Split('/')[1]) : 0;
                                    totalProfit += (individualSales - actualSales);
                                }
                                item[item.Length - 1] = totalProfit;
                                
                                break;
                            case SortTypeSummarySale.PROFIT_RATE:
                                // seect sort = profit rate
                                int totalSales_sortRate = 0;
                                int totalProfit_sortRate = 0;
                                double totalProfitRate = 0;
                                item[item.Length - 1] = 0;
                                for (int j = 3; j < item.Count() - 1; j++)
                                {
                                    totalSales_sortRate += item[j] != null ? Int32.Parse(item[j].ToString().Split('/')[0]) : 0;
                                    int individualSales = item[j] != null ? Int32.Parse(item[j].ToString().Split('/')[0]) : 0;
                                    int actualSales = item[j] != null ? Int32.Parse(item[j].ToString().Split('/')[1]) : 0;
                                    totalProfit_sortRate += (individualSales - actualSales);
                                }
                                if (totalSales_sortRate != 0)
                                {
                                    var rate = (Convert.ToDouble(totalProfit_sortRate) / Convert.ToDouble(totalSales_sortRate));
                                    totalProfitRate = rate * 100;
                                }
                                item[item.Length - 1] = totalProfitRate;

                                break;
                            default:
                                
                                break;
                        }
                    }



                    int totalItem = dataList.Count();

                    //reindex datalist
                    int reIndex = 1;
                    foreach(var k in dataList)
                    {
                        k[0] = reIndex;
                        reIndex++;
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
                            end = totalItem;

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
        /// check locationCondition contained locationId or not.
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="locationCondition"></param>
        /// <returns></returns>
        private bool CheckContainsLocationId(string locationId, string[] locationCondition)
        {
            if(string.IsNullOrEmpty(locationId))
            {
                return false;
            }
            var listLocation = locationCondition.ToList();
            if (listLocation.Contains(locationId))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Using Ajax to search data table Sales Group Detail by Project
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <param name="TAB_ID"></param>
        /// <returns></returns>
        public ActionResult SearchSalesGroupDetail(DataTablesModel model, SalesGroupDetailCondition condition, string TAB_ID)
        {
            if (Request.IsAjaxRequest() && ModelState.IsValid)
            {
                var pageInfo = this.mainService.GetListSalesGroupDetail(model, GetLoginUser().CompanyCode, condition);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = pageInfo.Items;
                decimal totalSales = 0;
                decimal totalCost = 0;
                decimal totalProfit = 0;

                foreach (var data in pageInfo.Items)
                {
                    totalSales += data.sales_amount;
                    totalCost += data.cost;
                    totalProfit += data.profit;
                }

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = (from t in pageInfo.Items
                                  select new object[]
   {
                                t.project_sys_id,
                                HttpUtility.HtmlEncode(t.project_name),
                                t.sales_amount.ToString("#,##0") + "円",
                                t.cost.ToString("#,##0") + "円",
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

        /// <summary>
        /// Using Ajax to search data table Sales Project Detail by User
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <param name="TAB_ID"></param>
        /// <returns></returns>
        public ActionResult SearchSalesProjectDetail(DataTablesModel model, SalesProjectDetailCondition condition, string TAB_ID)
        {
            if (Request.IsAjaxRequest() && ModelState.IsValid)
            {
                condition.CompanyCode = GetLoginUser().CompanyCode;
                var pageInfo = this.mainService.GetListSalesProjectDetail(model, condition);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = pageInfo.Items;
                decimal totalSales = 0;
                decimal totalCost = 0;
                decimal totalProfit = 0;


                foreach (var data in pageInfo.Items)
                {
                    totalSales += data.sales_amount;
                    totalCost += data.cost;
                    totalProfit += data.profit;
                }

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData = (from t in pageInfo.Items
                                  select new object[]
   {
                                t.user_sys_id,
                                t.peta_rn,
                                HttpUtility.HtmlEncode(t.user_name),
                                t.sales_amount.ToString("#,##0") + "円",
                                t.cost.ToString("#,##0") + "円",
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
