using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09003;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS09003;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;

namespace ProjectManagementSystem.Controllers
{
    public class PMS09003Controller : ControllerBase
    {
        //
        // GET: /PMS09003/
        /// <summary>
        /// Service instance reference
        /// </summary>

        private readonly IPMSCommonService commonService;

        private readonly IPMS09003Service service;

        /// <summary>
        /// TempData storage
        /// </summary>
        [Serializable]
        private class TmpValues
        {
            public string TotalSaleProceeds { get; set; }
            public string TotalRecords { get; set; }
            public string TotalGrossProfit { get; set; }
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service">service instance</param>
        public PMS09003Controller(IPMS09003Service service, IPMSCommonService commonservice)
        {
            this.service = service;
            this.commonService = commonservice;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PMS09003Controller()
            : this(new PMS09003Service(), new PMSCommonService())
        {
        }

        /// <summary>
        /// Clear save condition
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult ClearSaveCondition()
        {
            base.ClearRestoreData();
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Function display Sales customer list
        /// </summary>
        /// <returns>Index view</returns>
        public ActionResult Index()
        {
            if ((!IsInFunctionList(Constant.FunctionID.SalesCustomer_Admin)) && (!IsInFunctionList(Constant.FunctionID.SalesCustomer)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS09003SalesViewModel();
            model.GROUP_LIST = this.commonService.GetUserGroupSelectList(GetLoginUser().CompanyCode);
            model.TAG_LIST = new List<SelectListItem>();
            model.CONTRACT_TYPE_LIST = this.commonService.GetContractTypeSelectList(GetLoginUser().CompanyCode);
            model.BRANCH_LIST = this.commonService.GetBranchSelectList(GetLoginUser().CompanyCode);

            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS09003/SaleTagByCustomer") && Session[Constant.SESSION_IS_BACK] != null)
            {
                var tmpCondition = GetRestoreData() as Condition;

                if (tmpCondition != null)
                {
                    model.Condition = tmpCondition;
                }
            }


            if (model.Condition.CUSTOMER_ID != null)
            {
                model.TAG_LIST = this.commonService.GetTagSelectList(GetLoginUser().CompanyCode, model.Condition.CUSTOMER_ID.Value);
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("Index", model);
        }

        /// <summary>
        /// Functon search to search sale list by customer 
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="hdnOrderBy">order by column</param>
        /// <param name="hdnOrderType">order type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Sale list by Customer</returns>
        public ActionResult Search(DataTablesModel model, Condition condition, string hdnOrderBy, string hdnOrderType, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    var pageInfo = this.service.Search(model, condition, GetLoginUser().CompanyCode);
                    var salesCustomer = this.service.SearchAll(GetLoginUser().CompanyCode, condition);

                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = this.service.GetListSalesCustomer(condition, GetLoginUser().CompanyCode, hdnOrderBy, hdnOrderType);

                    decimal total_sale_proceeds = 0;
                    int total_records = 0;
                    decimal total_gross_profit = 0;

                    if (salesCustomer.Count > 0)
                    {
                        foreach (var item in salesCustomer)
                        {
                            total_sale_proceeds += Utility.RoundNumber(item.total_sales, GetLoginUser().DecimalCalculationType, false);
                            total_records += item.records;
                            total_gross_profit += Utility.RoundNumber(item.gross_profit, GetLoginUser().DecimalCalculationType, false);
                        }

                    }

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
                                            t.customer_id,
                                            t.peta_rn,
                                            HttpUtility.HtmlEncode(t.display_name), 
                                            t.records, 
                                            Utility.RoundNumber(t.total_sales, GetLoginUser().DecimalCalculationType, false).ToString("#,##0")+ "円",
                                            total_sale_proceeds == 0 ? "0.00%" : (t.total_sales / total_sale_proceeds * 100).ToString("#,##0.00") + "%",
                                            Utility.RoundNumber(t.gross_profit, GetLoginUser().DecimalCalculationType, false).ToString("#,##0")+ "円",
                                            t.total_sales == 0 ? "0.00%" : (t.gross_profit_rate * 100).ToString("#,##0.00") + "%",
                                            t.del_flg
                                        }).ToList(),
                            total_records = total_records.ToString(),
                            total_sale = total_sale_proceeds.ToString("#,##0"),
                            total_gross_profit = total_gross_profit.ToString("#,##0")
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
        /// Export to CSV
        /// </summary>
        /// <param name="search_customerId"></param>
        /// <param name="search_tagId"></param>
        /// <param name="search_startDate"></param>
        /// <param name="search_endDate"></param>
        /// <param name="search_deleteFlag"></param>
        /// <param name="hdnOrderBy"></param>
        /// <param name="hdnOrderType"></param>
        /// <param name="search_groupId"></param>
        /// <param name="search_contractTypeID"></param>
        /// <param name="search_locationID"></param>
        /// <param name="TAB_ID"></param>
        /// <returns>CSV file export</returns>
        public ActionResult ExportCsvListSalesCustomer(string search_customerId, string search_tagId, string search_startDate, string search_endDate, bool search_deleteFlag, string hdnOrderBy, string hdnOrderType, string search_groupId, string search_contractTypeID, string search_locationID, string TAB_ID)
        {
            Condition condition = new Condition();
            if (!string.IsNullOrEmpty(search_customerId))
            {
                condition.CUSTOMER_ID = Convert.ToInt32(search_customerId);
            }
            if (!string.IsNullOrEmpty(search_tagId))
            {
                condition.TAG_ID = Convert.ToInt32(search_tagId);
            }
            condition.START_DATE = search_startDate;
            condition.END_DATE = search_endDate;
            condition.CONTRACT_TYPE_ID = search_contractTypeID;
            condition.DELETED_INCLUDE = search_deleteFlag;
            if (!string.IsNullOrEmpty(search_groupId))
            {
                condition.GROUP_ID = Convert.ToInt32(search_groupId);
            }
            if (!string.IsNullOrEmpty(search_locationID))
            {
                condition.LOCATION_ID = search_locationID;
            }

            var results = new List<SalesCustomerPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as List<SalesCustomerPlus>;
            }
            else
            {
                results = this.service.GetListSalesCustomer(condition, GetLoginUser().CompanyCode, hdnOrderBy, hdnOrderType);
            }

            List<SalesCustomerPlusExport> dataExport = new List<SalesCustomerPlusExport>();
            string[] columns = new[] {
                    "No.",
                    "取引先",
                    "売上件数",
                    "売上金額",
                    "構成比",
                    "粗利金額",
                    "粗利率"
            };
            decimal total_sale_proceeds = 0;

            for (int i = 0; i < results.Count; i++)
            {
                results[i].peta_rn = i + 1;
                total_sale_proceeds += Utility.RoundNumber(results[i].total_sales, GetLoginUser().DecimalCalculationType, false);
            }

            foreach (var r in results)
            {
                SalesCustomerPlusExport tmpData = new SalesCustomerPlusExport();
                tmpData.customer_id = r.peta_rn;
                tmpData.display_name = r.display_name;
                tmpData.records = r.records.ToString();
                tmpData.total_sales = Utility.RoundNumber(r.total_sales, GetLoginUser().DecimalCalculationType, false).ToString("#,##0");
                tmpData.sale_proceeds = r.total_sales == 0 ? "0%" : Utility.RoundNumber(r.total_sales / total_sale_proceeds, "03", true).ToString("#,##0.00") + "%";
                tmpData.gross_profit = Utility.RoundNumber(r.gross_profit, GetLoginUser().DecimalCalculationType, false).ToString("#,##0");
                tmpData.gross_profit_rate = r.gross_profit_rate == 0 ? "0%" : Utility.RoundNumber(r.gross_profit_rate, "03", true) + "%";
                dataExport.Add(tmpData);
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "SalesCustomer_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }

        /// <summary>
        /// Sale Tag by Customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SaleTagByCustomer()
        {
            if ((!IsInFunctionList(Constant.FunctionID.SalesCustomer_Admin)) && (!IsInFunctionList(Constant.FunctionID.SalesCustomer)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS09003/SaleProjectByCustomer"))
            {
                var condition = Session["PMS09003_SaleTagByCustomer_Condition"] as ConditionSaleTag;
                var customerName = Session["PMS09003_SaleTagByCustomer_CustomerName"] as string;
                var locationName = Session["PMS09003_PMS09003_SaleTagByCustomer_LocationName"] as string;
                var groupName = Session["PMS09003_SaleTagByCustomer_GroupName"] as string;
                var tagName = Session["PMS09003_SaleTagByCustomer_TagName"] as string;
                var contractTypeName = Session["PMS09003_SaleTagByCustomer_ContractTypeName"] as string;
                var model = new PMS09003SalesTagByCustomerViewMode
                {
                    Condition = condition,
                    Customer_Name = customerName,
                    Location_Name = locationName,
                    Group_Name = groupName,
                    Tag_Name = tagName,
                    Contract_Type_Name = contractTypeName
                };
                return this.View("SaleTagByCustomer", model);
            }
            else
            {
                return new EmptyResult();
            }
        }
        /// <summary>
        /// Detail
        /// </summary>
        /// <param name="customer_Id">customer_Id</param>
        /// <param name="customer_Name">customer_Name</param>
        /// <param name="start_Date">start_Date</param>
        /// <param name="end_Date">end_Date</param>
        /// <param name="locationId">locationId</param>
        /// <param name="locationName">locationName</param>
        /// <param name="groupId">groupId</param>
        /// <param name="groupName">groupName</param>
        /// <param name="tagId">tagId</param>
        /// <param name="tagName">tagName</param>
        /// <param name="contract_type_id">contract_type_id</param>
        /// <param name="contract_type_name">contract_type_name</param>
        /// <returns>Sale Tag By Customer View</returns>
        [HttpPost]
        public ActionResult SaleTagByCustomer(int customer_Id, string customer_Name, DateTime start_Date, DateTime end_Date, string locationId, string locationName, string groupId, string groupName, int? tagId, string tagName, string contract_type_id, string contract_type_name)
        {
            if ((!IsInFunctionList(Constant.FunctionID.SalesCustomer_Admin)) && (!IsInFunctionList(Constant.FunctionID.SalesCustomer)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var condition = new ConditionSaleTag()
            {
                CUSTOMER_ID = customer_Id,
                START_DATE = start_Date,
                END_DATE = end_Date,
                LOCATION_ID = locationId,
                GROUP_ID = groupId,
                TAG_ID = tagId,
                CONTRACT_TYPE_ID = contract_type_id
            };

            var model = new PMS09003SalesTagByCustomerViewMode
            {
                Customer_Name = customer_Name,
                Condition = condition,
                Location_Name = locationName,
                Group_Name = groupName,
                Tag_Name = tagName,
                Contract_Type_Name = contract_type_name
            };

            Session["PMS09003_SaleTagByCustomer_Condition"] = condition;
            Session["PMS09003_SaleTagByCustomer_CustomerName"] = customer_Name;
            Session["PMS09003_PMS09003_SaleTagByCustomer_LocationName"] = locationName;
            Session["PMS09003_SaleTagByCustomer_GroupName"] = groupName;
            Session["PMS09003_SaleTagByCustomer_TagName"] = tagName;
            Session["PMS09003_SaleTagByCustomer_ContractTypeName"] = contract_type_name;

            return this.View("SaleTagByCustomer", model);
        }

        /// <summary>
        /// Display information Sale tag list by customer
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Sale Tag List By Customer</returns>
        public ActionResult SaleTagListByCustomer(DataTablesModel model, ConditionSaleTag condition, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                var pageInfo = this.service.SearchTagByCustomer(model, GetLoginUser().CompanyCode, condition);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = pageInfo.Items;
                decimal totalSales = 0;
                decimal totalProfit = 0;

                foreach (var data in pageInfo.Items)
                {
                    totalSales += data.total_sales;
                    totalProfit += data.gross_profit;
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
                            t.tag_id,
                            t.peta_rn,
                            HttpUtility.HtmlEncode(t.display_name),
                            t.total_sales.ToString("#,##0") + "円",
                            (totalSales > 0 ? (t.total_sales / totalSales * 100) : 0).ToString("#,##0.00") + "%",
                            t.gross_profit.ToString("#,##0") + "円",
                            t.total_sales == 0 ? "0.00%" : (t.gross_profit_rate * 100).ToString("#,##0.00") + "%"
                        }).ToList(),
                        totalSales = totalSales.ToString("#,##0") + "円",
                        totalProfit = totalProfit.ToString("#,##0") + "円"
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export to csv Sales tag by customer
        /// </summary>
        /// <param name="condition_customerId"></param>
        /// <param name="condition_startDate"></param>
        /// <param name="condition_endDate"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="TAB_ID"></param>
        /// <returns>CSV file export</returns>
        [HttpPost]
        public ActionResult ExportCsvListSalesTagByCustomer(
            string condition_customerId,
            string condition_startDate,
            string condition_endDate,
            int sortCol = 0,
            string sortDir = "asc",
            string TAB_ID = "")
        {
            ConditionSaleTag condition = new ConditionSaleTag();
            if (!string.IsNullOrEmpty(condition_customerId))
            {
                condition.CUSTOMER_ID = Convert.ToInt32(condition_customerId);
            }
            condition.START_DATE = Convert.ToDateTime(condition_startDate);
            condition.END_DATE = Convert.ToDateTime(condition_endDate);

            DataTablesModel model = new DataTablesModel
            {
                sEcho = "1",
                iColumns = 7,
                sColumns = "tag_id,tag_id,display_name,total_sales,total_sales,gross_profit,gross_profit",
                iDisplayStart = 0,
                iDisplayLength = int.MaxValue,
                iSortCol_0 = sortCol,
                sSortDir_0 = sortDir,
                iSortingCols = 1
            };

            var results = new List<SalesTagByCustomerPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as List<SalesTagByCustomerPlus>;
            }
            else
            {
                results = this.service.SearchTagByCustomer(model, GetLoginUser().CompanyCode, condition).Items;
            }

            List<SalesTagByCustomerPlusExport> dataExport = new List<SalesTagByCustomerPlusExport>();
            string[] columns = new[] {
                    "No.",
                    "タグ名",
                    "売上金額",
                    "構成比",
                    "粗利金額",
                    "粗利率"
            };

            decimal totalSales = 0;
            decimal totalProfit = 0;
            int i = 0;

            foreach (var data in results)
            {
                data.peta_rn = i + 1;
                totalSales += data.total_sales;
                totalProfit += data.gross_profit;
                i++;
            }

            foreach (var r in results)
            {
                SalesTagByCustomerPlusExport tmpData = new SalesTagByCustomerPlusExport();
                tmpData.tag_id = r.peta_rn;
                tmpData.display_name = (r.display_name == null) ? "未設定" : r.display_name;
                tmpData.total_sales = r.total_sales.ToString("#,##0");
                tmpData.sale_proceeds = (totalSales > 0 ? (r.total_sales / totalSales * 100) : 0).ToString("#,##0.00") + "%";
                tmpData.gross_profit = r.gross_profit.ToString("#,##0");
                tmpData.gross_profit_rate = r.gross_profit_rate == 0 ? "0%" : Utility.RoundNumber(r.gross_profit_rate, "03", true) + "%";
                dataExport.Add(tmpData);
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "SalesTagByCustomer_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }

        /// <summary>
        /// Detail
        /// </summary>
        /// <param name="customer_Id">customer_Id</param>
        /// <param name="customer_Name">customer_Name</param>
        /// <param name="start_Date">start_Date</param>
        /// <param name="end_Date">end_Date</param>
        /// <param name="tag_Id">tag_Id</param>
        /// <param name="tag_Name">tag_Name</param>
        /// <param name="location_id">location_id</param>
        /// <param name="location_name">location_name</param>
        /// <returns>Sale Project By Customer View</returns>
        public ActionResult SaleProjectByCustomer(int customer_Id, string customer_Name, DateTime start_Date, DateTime end_Date, int tag_Id, string tag_Name, string location_id, string location_name, string group_id, string group_name, string contract_type_id, string contract_type_name)
        {
            if ((!IsInFunctionList(Constant.FunctionID.SalesCustomer_Admin)) && (!IsInFunctionList(Constant.FunctionID.SalesCustomer)))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var condition = new ConditionSaleProject()
            {
                CUSTOMER_ID = customer_Id,
                START_DATE = start_Date,
                END_DATE = end_Date,
                TAG_ID = tag_Id,
                LOCATION_ID = location_id,
                GROUP_ID = group_id,
                CONTRACT_TYPE_ID = contract_type_id
            };

            var model = new PMS09003SalesProjectByCustomerViewModel()
            {
                Customer_Name = customer_Name,
                Tag_Name = tag_Name,
                Condition = condition,
                Location_Name = location_name,
                Group_Name = group_name,
                Contract_Type_Name = contract_type_name
            };

            return this.View("SaleProjectByCustomer", model);
        }

        /// <summary>
        /// Display infomation Sale project list by customer
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Sale Project List By Customer</returns>
        public ActionResult SaleProjectListByCustomer(DataTablesModel model, ConditionSaleProject condition, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                var pageInfo = this.service.SearchProjectByCustomer(model, GetLoginUser().CompanyCode, condition);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = pageInfo.Items;
                decimal totalSales = 0;
                decimal totalProfit = 0;

                foreach (var data in pageInfo.Items)
                {
                    totalSales += data.total_sales;
                    totalProfit += data.gross_profit;
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
                            t.peta_rn,
                            HttpUtility.HtmlEncode(t.display_name),
                            HttpUtility.HtmlEncode(t.contract_type),
                            t.total_sales.ToString("#,##0") + "円",
                            (totalSales > 0 ? (t.total_sales / totalSales * 100) : 0).ToString("#,##0.00") + "%",
                            t.gross_profit.ToString("#,##0") + "円",
                            t.total_sales == 0 ? "0.00%" : (t.gross_profit_rate * 100).ToString("#,##0.00") + "%"
                        }).ToList(),
                        totalSales = totalSales.ToString("#,##0") + "円",
                        totalProfit = totalProfit.ToString("#,##0") + "円"
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export to csv Sales tag by customer
        /// </summary>
        /// <param name="condition_customerId"></param>
        /// <param name="condition_tagId"></param>
        /// <param name="condition_startDate"></param>
        /// <param name="condition_endDate"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="TAB_ID"></param>
        /// <returns>CSV file export</returns>
        [HttpPost]
        public ActionResult ExportCsvSaleListProjectByCustomer(
            string condition_customerId,
            string condition_tagId,
            string condition_startDate,
            string condition_endDate,
            int sortCol = 0,
            string sortDir = "asc",
            string TAB_ID = "")
        {
            ConditionSaleProject condition = new ConditionSaleProject();
            if (!string.IsNullOrEmpty(condition_customerId))
            {
                condition.CUSTOMER_ID = Convert.ToInt32(condition_customerId);
            }

            if (!string.IsNullOrEmpty(condition_tagId))
            {
                condition.TAG_ID = Convert.ToInt32(condition_tagId);
            }
            condition.START_DATE = Convert.ToDateTime(condition_startDate);
            condition.END_DATE = Convert.ToDateTime(condition_endDate);

            DataTablesModel model = new DataTablesModel
            {
                sEcho = "1",
                iColumns = 7,
                sColumns = "project_sys_id,tag_id,display_name,contract_type,total_sales,total_sales,gross_profit,gross_profit",
                iDisplayStart = 0,
                iDisplayLength = int.MaxValue,
                iSortCol_0 = sortCol,
                sSortDir_0 = sortDir,
                iSortingCols = 1
            };

            var results = new List<SalesProjectByCustomerPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as List<SalesProjectByCustomerPlus>;
            }
            else
            {
                results = this.service.SearchProjectByCustomer(model, GetLoginUser().CompanyCode, condition).Items;
            }

            List<SalesProjectByCustomerPlusExport> dataExport = new List<SalesProjectByCustomerPlusExport>();
            string[] columns = new[] {
                    "No.",
                    "プロジェクト名",
                    "契約種別",
                    "売上金額",
                    "構成比",
                    "粗利金額",
                    "粗利率"
            };

            decimal totalSales = 0;
            decimal totalProfit = 0;
            int i = 0;
            foreach (var data in results)
            {
                data.peta_rn = i + 1;
                totalSales += data.total_sales;
                totalProfit += data.gross_profit;
                i++;
            }

            foreach (var r in results)
            {
                SalesProjectByCustomerPlusExport tmpData = new SalesProjectByCustomerPlusExport();
                tmpData.project_sys_id = r.peta_rn;
                tmpData.display_name = (r.display_name == null) ? "未設定" : r.display_name;
                tmpData.contract_type = (r.contract_type == null) ? "" : r.contract_type;
                tmpData.total_sales = r.total_sales.ToString("#,##0");
                tmpData.sale_proceeds = (totalSales > 0 ? (r.total_sales / totalSales * 100) : 0).ToString("#,##0.00") + "%";
                tmpData.gross_profit = r.gross_profit.ToString("#,##0");
                tmpData.gross_profit_rate = r.gross_profit_rate == 0 ? "0%" : Utility.RoundNumber(r.gross_profit_rate, "03", true) + "%";
                dataExport.Add(tmpData);
            }

            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            string fileName = "SalesProjectByCustomer_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv";
            Utility.ExportToCsvData(this, dt, fileName);

            return new EmptyResult();
        }
    }
}
