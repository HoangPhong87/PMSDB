#region License
/// <copyright file="PMS06003Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>TrungNT</author>
/// <createdDate>2014/05/09</createdDate>
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS06003;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS06003;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;

namespace ProjectManagementSystem.Controllers
{
    using System.Configuration;
    using System.Text;

    public class PMS06003Controller : ControllerBase
    {
        /// <summary>
        /// Common service
        /// </summary>
        private readonly IPMSCommonService _commonService;

        /// <summary>
        /// Service for PMS06003
        /// </summary>
        private readonly IPMS06003Service _service;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS06003Controller()
            : this(new PMS06003Service(), new PMSCommonService())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">IPMS06003Service</param>
        /// <param name="commonservice">IPMSCommonService</param>
        public PMS06003Controller(IPMS06003Service service, IPMSCommonService commonservice)
        {
            _service = service;
            this._commonService = commonservice;
        }


        #region AssignmentByProject
        /// <summary>
        /// Clear save condition
        /// </summary>
        /// <returns>Redirect to AssignmentByProject</returns>
        [HttpGet]
        public ActionResult ClearSaveCondition()
        {
            base.ClearRestoreData();
            return this.RedirectToAction("AssignmentByProject");
        }

        /// <summary>
        /// Action controller AssignmentByProject
        /// </summary>
        /// <returns>Assignemt By Project View</returns>
        public ActionResult AssignmentByProject()
        {
            if (!IsInFunctionList(Constant.FunctionID.AssignmentByProject))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = MakeAssignmentByProjectViewModel();
            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS06001/Detail"))
            {
                var tmp = GetRestoreData() as ProjectCondition;
                if (tmp != null)
                {
                    model.Condition = tmp;
                }
            }

            var iDisplayLength = Session["PMS06003_AssignmentByProject"] as int?;
            if (iDisplayLength != null)
            {
                ViewBag.iDisplayLength = iDisplayLength;
            }
            else
            {
                ViewBag.iDisplayLength = ConfigurationManager.AppSettings[ConfigurationKeys.LIST_ITEMS_PER_PAGE];
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return View(model);
        }

        /// <summary>
        /// Search Assignment by project
        /// </summary>
        /// <param name="model">DataTablesModel</param>
        /// <param name="condition">ProjectCondition</param>
        /// <param name="SORT_COLUMN">Sort column</param>
        /// <param name="SORT_TYPE">Sort type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Assignment By Project</returns>
        public ActionResult SearchAssignmentByProject(DataTablesModel model, ProjectCondition condition, int SORT_COLUMN, string SORT_TYPE, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    Session["PMS06003_AssignmentByProject"] = model.iDisplayLength;

                    var pagePlan = this._service.SearchPlanByProject(model, condition, GetLoginUser().CompanyCode);
                    var listPlan = this._service.GetListPlanByProject(condition, GetLoginUser().CompanyCode, SORT_COLUMN, SORT_TYPE);
                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = Utility.SerializeDynamicToJson(listPlan);
                    var results = buildTableResults(condition, pagePlan.Items);
                    var result = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = pagePlan.TotalItems,
                            iTotalDisplayRecords = pagePlan.TotalItems,
                            aaData = results
                        });

                    // save search condition
                    SaveRestoreData(condition);
                    return result;
                }
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Format datatable
        /// </summary>
        /// <param name="condition">ProjectCondition</param>
        /// <param name="pagePlan">IList</param>
        /// <param name="isExportCsv">isExportCsv</param>
        /// <returns>List</returns>
        public IList<object[]> buildTableResults(ProjectCondition condition, IList<dynamic> pagePlan, bool isExportCsv = false)
        {
            List<IDictionary<string, object>> planDetailTemp = new List<IDictionary<string, object>>();
            if (pagePlan != null)
            {
                // Add dynamic data to list
                foreach (var item in pagePlan)
                {
                    planDetailTemp.Add(item);
                }
            }

            var timeUnit = string.Empty;

            if (condition.EFF_TYPE == Convert.ToInt32(Constant.TimeUnit.HOUR))
            {
                timeUnit = Constant.TimeUnit.Items[0].ToString();
            }
            else if (condition.EFF_TYPE == Convert.ToInt32(Constant.TimeUnit.DAY))
            {
                timeUnit = Constant.TimeUnit.Items[1].ToString();
            }
            else
            {
                timeUnit = Constant.TimeUnit.Items[2].ToString();
            }

            List<string> listMonth = Utility.getListMonth(condition.START_DATE, condition.END_DATE);
            List<object[]> resultList = new List<object[]>();

            for (int i = 0; i < planDetailTemp.Count; i++) // plan
            {
                var plan = planDetailTemp[i].Values.ToList();
                decimal totalPlan = 0;
                object[] planActual = new object[plan.Count - 3];
                if (isExportCsv)
                {
                    planActual[0] = plan[4]; // person_in_charge
                    planActual[1] = plan[5]; // project_name
                    planActual[2] = plan[6]; // rank
                    planActual[3] = Convert.ToDecimal(plan[7].ToString()).ToString("#,##0.##") + "円"; // total_sales;
                    for (var k = 8; k < listMonth.Count + 8; k++) // calculate total plan
                    {
                        decimal p = plan[k] != null ? Math.Round(Convert.ToDecimal(plan[k]), 2) : 0;
                        totalPlan += p;
                        planActual[k - 3] = p.ToString("#,##0.00"); // plan in month
                    }
                    planActual[4] = totalPlan.ToString("#,##0.00") + timeUnit; // total plan
                }
                else
                {
                    string hiddenTmp = string.Empty;
                    if (plan[2].ToString().Equals("1"))
                    {
                        hiddenTmp = "<input type='hidden' name='del_flg' value='{0}' />";
                    }
                    planActual[0] = plan[1]; // project_sys_id for initiate sort
                    var project_id_plan = plan[1]; // project_id_temp
                    planActual[1] = HttpUtility.HtmlEncode(plan[5]) + hiddenTmp; // person_in_charge
                    if (IsInFunctionList(Constant.FunctionID.ProjectDetail))
                    {
                        planActual[2] = "<a href='#' title='" + HttpUtility.HtmlEncode(plan[6]) + "' class='project-detail-link' project-id='"
                        + project_id_plan + @"'><label class='longtext'>" + HttpUtility.HtmlEncode(plan[6]) + "</label></a>";
                    }
                    else
                    {
                        planActual[2] = "<div class='short-text text-overflow' title=" + HttpUtility.HtmlEncode(plan[6]) + ">" + HttpUtility.HtmlEncode(plan[6]) + "</div>";
                    }

                    planActual[3] = (plan[7] ?? string.Empty).ToString(); // rank
                    planActual[4] = Convert.ToDecimal(plan[8].ToString()).ToString("#,##0.##") + "円"; // total_sales;

                    for (var k = 9; k < listMonth.Count + 9; k++) // calculate total plan
                    {
                        decimal p = plan[k] != null ? Math.Round(Convert.ToDecimal(plan[k]), 2) : 0;
                        totalPlan += p;
                        planActual[k - 3] = p.ToString("#,##0.00") + hiddenTmp; // plan in month
                    }
                    planActual[5] = totalPlan.ToString("#,##0.00") + timeUnit; // total plan
                }
                resultList.Add(planActual.ToArray());
            }

            return resultList;
        }

        /// <summary>
        /// Export to csv by project
        /// </summary>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="project_name"></param>
        /// <param name="customer_id"></param>
        /// <param name="group_id"></param>
        /// <param name="tag_id"></param>
        /// <param name="eff_type"></param>
        /// <param name="sort_colum"></param>
        /// <param name="sort_type"></param>
        /// <param name="DelFlag"></param>
        /// <param name="status"></param>
        /// <param name="TAB_ID"></param>
        /// <returns>File CSV export</returns>
        public ActionResult ExportCsvByProject(string start_date, string end_date, string project_name, int? customer_id, int? group_id, int? tag_id, int eff_type, int sort_colum, string sort_type, bool DelFlag, string status, string TAB_ID)
        {
            ProjectCondition condition = new ProjectCondition();
            condition.START_DATE = start_date;
            condition.PROJECT_NAME = project_name;
            condition.END_DATE = end_date;
            condition.CUSTOMER_ID = customer_id;
            condition.GROUP_ID = group_id;
            condition.TAG_ID = tag_id;
            condition.EFF_TYPE = eff_type;
            condition.DELETE_FLG = DelFlag;
            condition.STATUS_ID = status;

            IList<string> jsonResults = new List<string>();
            IList<dynamic> listPlan = new List<dynamic>();

            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                jsonResults = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<string>;
                listPlan = Utility.DeserializeJsonToDynamic(jsonResults);
            }
            else
            {
                listPlan = this._service.GetListPlanByProject(condition, GetLoginUser().CompanyCode, sort_colum, sort_type);
            }

            var results = buildTableResults(condition, listPlan, true);
            List<string> fixColumns = new List<string>() {
                "担当者",
                "プロジェクト名",
                "ランク",
                "受注金額",
                "見積工数"
            };
            List<string> listMonth = Utility.getListMonth(condition.START_DATE, condition.END_DATE);
            string[] columns = fixColumns.Concat(listMonth).ToArray();

            DataTable dataTable = Utility.ToDateTable(results, columns);
            Utility.ExportToCsvData(this, dataTable, "AssignmentByProject_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv");
            return new EmptyResult();
        }

        /// <summary>
        /// Export to csv by user
        /// </summary>
        /// <returns>File CSV</returns>
        public ActionResult ExportCsvByUser(string start_date, string end_date, string user_name, int? group_id, string location, int eff_type, int sort_colum, string sort_type, bool retired_flag, string TAB_ID)
        {
            UserCondition condition = new UserCondition();
            condition.GROUP_ID = group_id;
            condition.USER_NAME = user_name;
            condition.START_DATE = start_date;
            condition.END_DATE = end_date;
            condition.EFF_TYPE = eff_type;
            condition.RETIRED_INCLUDE = retired_flag;
            condition.LOCATION_ID = location;

            IList<string> jsonResults = new List<string>();
            IList<dynamic> results = new List<dynamic>();

            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                jsonResults = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<string>;
                results = Utility.DeserializeJsonToDynamic(jsonResults);
            }
            else
            {
                results = this._service.GetListAssignmentByUser(condition, GetLoginUser().CompanyCode, sort_colum, sort_type);
            }

            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
            foreach (var t in results)
            {
                list.Add(t);
            }
            List<string> fixColumns = new List<string>() {
                "No.",
                "所属",
                "ユーザー名"
            };

            List<string> listMonth = Utility.getListMonth(condition.START_DATE, condition.END_DATE);
            string[] columns = fixColumns.Concat(listMonth).ToArray();

            // Convert to Datatable
            DataTable dataTable = new DataTable(list.ToString());

            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(columns[i]);
            }

            foreach (IDictionary<string, object> item in list)
            {
                var values = new object[item.Values.Count - 1];
                int i = 0;
                values[i] = item.Values.ElementAt(i);
                for (i = 2; i < item.Values.Count; i++)
                {
                    if (i >= 4 && item.Values.ElementAt(i) != null)
                    {
                        values[i - 1] = Convert.ToDecimal(item.Values.ElementAt(i).ToString()).ToString("#,##0.00");
                    }
                    else
                    {
                        values[i - 1] = item.Values.ElementAt(i);
                    }
                }
                dataTable.Rows.Add(values);
            }

            // Export Table to CSV file
            Utility.ExportToCsvData(this, dataTable, "AssignmentByUser_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv");

            return new EmptyResult();
        }

        /// <summary>
        /// Make ViewModel for AssignmentByProject
        /// </summary>
        /// <returns>Assignment By Project View Model</returns>
        private AssignmentByProjectViewModel MakeAssignmentByProjectViewModel()
        {
            var currentUser = GetLoginUser();

            string companyCode = GetLoginUser().CompanyCode;
            var model = new AssignmentByProjectViewModel();

            model.EFFORT_LIST = MakeCmbEffort();
            model.GROUP_LIST = this._commonService.GetUserGroupSelectList(currentUser.CompanyCode);
            model.STATUS_LIST = this._commonService.GetStatusSelectList(currentUser.CompanyCode);
            model.TAG_LIST = new List<SelectListItem>();
            model.Condition = new ProjectCondition();
            model.Condition.GROUP_ID = currentUser.GroupId;


            return model;
        }

        /// <summary>
        /// Create list of select items for Combobox Effort
        /// </summary>
        /// <returns>List Effort</returns>
        private IList<SelectListItem> MakeCmbEffort()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = Constant.TimeUnit.Items[0].ToString(), Value = Constant.TimeUnit.HOUR });
            items.Add(new SelectListItem { Text = Constant.TimeUnit.Items[1].ToString(), Value = Constant.TimeUnit.DAY, Selected = true });
            items.Add(new SelectListItem { Text = Constant.TimeUnit.Items[2].ToString(), Value = Constant.TimeUnit.MONTH });
            return items;
        }
        #endregion

        #region AssignmentByUser
        /// <summary>
        /// Action controller AssignmentByUser
        /// </summary>
        /// <returns>Assignment by user view</returns>
        public ActionResult AssignmentByUser()
        {
            if (!IsInFunctionList(Constant.FunctionID.AssignmentByUser))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = MakeAssignmentByUserViewModel();

            var iDisplayLength = Session["PMS06003_AssignmentByUser"] as int?;
            if (iDisplayLength != null)
            {
                ViewBag.iDisplayLength = iDisplayLength;
            }
            else
            {
                ViewBag.iDisplayLength = ConfigurationManager.AppSettings[ConfigurationKeys.LIST_ITEMS_PER_PAGE];
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return View(model);
        }

        /// <summary>
        /// Make ViewModel for AssignmentByUser
        /// </summary>
        /// <returns>Assignment By User View Model</returns>
        private AssignmentByUserViewModel MakeAssignmentByUserViewModel()
        {
            var model = new AssignmentByUserViewModel();
            var currentUser = GetLoginUser();

            model.EFFORT_LIST = MakeCmbEffort();
            model.GROUP_LIST = this._commonService.GetUserGroupSelectList(currentUser.CompanyCode);
            model.Condition = new UserCondition();
            model.STATUS_LIST = this._commonService.GetStatusSelectList(currentUser.CompanyCode);
            model.Condition.GROUP_ID = currentUser.GroupId;
            model.BRANCH_LIST = this._commonService.GetBranchSelectList(currentUser.CompanyCode);

            return model;
        }

        /// <summary>
        /// Search Assignment by User
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="SORT_COLUMN">Sort column</param>
        /// <param name="SORT_TYPE">Sort type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Assignmen By User</returns>
        public ActionResult SearchAssignmentByUser(DataTablesModel model, UserCondition condition, int SORT_COLUMN, string SORT_TYPE, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    Session["PMS06003_AssignmentByUser"] = model.iDisplayLength;
                    var pi = this._service.SearchAssignmentByUser(model, condition, GetLoginUser().CompanyCode);
                    var priceInfor = this._service.SearchPriceInforByUser(model, condition, GetLoginUser().CompanyCode);
                    var items = pi.Items;

                    var listAssignmentByUser = this._service.GetListAssignmentByUser(condition, GetLoginUser().CompanyCode, SORT_COLUMN, SORT_TYPE);
                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = Utility.SerializeDynamicToJson(listAssignmentByUser);

                    List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
                    List<IDictionary<string, object>> priceInforList = new List<IDictionary<string, object>>();

                    foreach (var t in items)
                    {
                        list.Add(t);
                    }

                    foreach (var t in priceInfor)
                    {
                        priceInforList.Add(t);
                    }

                    List<object[]> listData = new List<object[]>();
                    if (list.Count != 0)
                    {
                        int count = list[0].Keys.Count;
                        object[] arr;
                        var countPriceInforList = 0;
                        foreach (var item in list)
                        {
                            arr = new object[count];
                            arr[0] = item.Values.ElementAt(1); // user_sys_id
                            arr[1] = item.Values.ElementAt(2); // company_code
                            arr[2] = item.Values.ElementAt(0); // No
                            int UserId = GetLoginUser().UserId;
                            for (int i = 3; i < count; i++)
                            {
                                if (i >= 5 && item.Values.ElementAt(i) != null)
                                {
                                    string columnName = item.Keys.ElementAt(i);
                                    string[] parts = columnName.Split("/".ToCharArray());

                                    if (IsInFunctionList(Constant.FunctionID.ActualWorkDetail))
                                    {
                                        //Check Get Over Base Unit Price
                                        var itemTmp = priceInforList[countPriceInforList].Values.ElementAt(i).ToString();
                                        var showNumBreak = itemTmp.Split('/'); //itemTmpBreak[1]: Individual Sales; itemTmpBreak[0]: Base Unit Cost; 
                                        var individualSales = showNumBreak[1];
                                        var baseUnitCost = showNumBreak[0];
                                        var checkGetOverPrice = "";

                                        if (individualSales != "-" && baseUnitCost != "-")
                                        {
                                            var eachIndividualSales = Convert.ToDecimal(individualSales);
                                            var eachBaseUnitCost = Convert.ToDecimal(baseUnitCost);
                                            if (eachBaseUnitCost > eachIndividualSales)
                                            {
                                                checkGetOverPrice = "style='color:red'";//paint red to text
                                            }
                                        }

                                        if (individualSales == "-")
                                        {
                                            if (baseUnitCost != "-")
                                            {
                                                if (Convert.ToDecimal(baseUnitCost) > 0)
                                                {
                                                    checkGetOverPrice = "style='color:red'";//paint red to text
                                                }
                                            }
                                        }
                                        var sb = new StringBuilder();
                                        sb.Append(@"<a href='#' class='detail-link' " + checkGetOverPrice + @" user_id='" + item.Values.ElementAt(1)
                                            + "' selected_year='" + parts[0]
                                            + "' selected_month='" + parts[1] + "'>"
                                            + Convert.ToDecimal(item.Values.ElementAt(i).ToString()).ToString("#,##0.00")
                                            + "</a>");
                                        arr[i] = sb.ToString();
                                    }
                                    else
                                    {
                                        arr[i] = Convert.ToDecimal(item.Values.ElementAt(i).ToString()).ToString("#,##0.00");
                                    }
                                }
                                else if (i == 4 || i == 3)
                                {
                                    var tmpValue = item.Values.ElementAt(i);
                                    arr[i] = HttpUtility.HtmlEncode(tmpValue == null ? string.Empty : tmpValue.ToString());
                                }
                                else
                                {
                                    arr[i] = item.Values.ElementAt(i);
                                }

                            }
                            countPriceInforList++;
                            listData.Add(arr);
                        }
                    }

                    var result2 = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = pi.TotalItems,
                            iTotalDisplayRecords = pi.TotalItems,
                            aaData = listData.ToList<object[]>()
                        });

                    return result2;
                }
            }

            return new EmptyResult();
        }
        #endregion
        #region UnitPriceInfor
        /// <summary>
        /// Action controller AssignmentByUser
        /// </summary>
        /// <returns>Assignment by user view</returns>
        public ActionResult UnitPriceInfor()
        {
            if (!IsInFunctionList(Constant.FunctionID.UnitPriceInfo_List))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = UnitPriceInforViewModel();

            var iDisplayLength = Session["PMS06003_UnitPriceInfor"] as int?;
            if (iDisplayLength != null)
            {
                ViewBag.iDisplayLength = iDisplayLength;
            }
            else
            {
                ViewBag.iDisplayLength = ConfigurationManager.AppSettings[ConfigurationKeys.LIST_ITEMS_PER_PAGE];
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return View(model);
        }

        /// <summary>
        /// Make ViewModel for AssignmentByUser
        /// </summary>
        /// <returns>Assignment By User View Model</returns>
        private AssignmentByUserViewModel UnitPriceInforViewModel()
        {
            var model = new AssignmentByUserViewModel();
            var currentUser = GetLoginUser();

            model.EFFORT_LIST = MakeCmbEffort();
            model.GROUP_LIST = this._commonService.GetUserGroupSelectList(currentUser.CompanyCode);

            model.Condition = new UserCondition();
            var d = Utility.GetCurrentDateTime();
            var endDate = new DateTime(d.Year, d.Month, 15, 0, 0, 0);
            var startDate = endDate.AddMonths(-5);
            model.Condition.END_DATE = endDate.Year.ToString() + "/" + endDate.Month.ToString();
            model.Condition.START_DATE = startDate.Year.ToString() + "/" + startDate.Month.ToString();

            model.STATUS_LIST = this._commonService.GetStatusSelectList(currentUser.CompanyCode);
            model.Condition.GROUP_ID = currentUser.GroupId;
            model.BRANCH_LIST = this._commonService.GetBranchSelectList(currentUser.CompanyCode);

            return model;
        }

        /// <summary>
        /// Action return the ActualWorkList to display
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="SORT_COLUMN">Sort column</param>
        /// <param name="SORT_TYPE">Sort type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Actual work list</returns>
        public ActionResult SearchUnitPriceInfor(DataTablesModel model, UserCondition condition, int SORT_COLUMN, string SORT_TYPE, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid)
                {
                    Session["PMS06003_UnitPriceInfor"] = model.iDisplayLength;
                    var pi = this._service.SearchPriceInforByUser(model, condition, GetLoginUser().CompanyCode);
                    IList<dynamic> listAllPriceInfor = this._service.GetListPriceInfor(condition, GetLoginUser().CompanyCode, (int)model.iSortCol_0, model.sSortDir_0);

                    Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = Utility.SerializeDynamicToJson(listAllPriceInfor);
                    List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();

                    foreach (var t in pi)
                    {
                        list.Add(t);
                    }

                    List<object[]> listData = new List<object[]>();
                    if (list.Count != 0)
                    {
                        int count = list[0].Keys.Count;
                        object[] arr;
                        foreach (var item in list)
                        {
                            arr = new object[count];
                            arr[0] = item.Values.ElementAt(1); // user_sys_id
                            arr[1] = item.Values.ElementAt(2); // company_code
                            arr[2] = item.Values.ElementAt(0); // No
                            int UserId = GetLoginUser().UserId;
                            for (int i = 3; i < count; i++)
                            {
                                if (i >= 5 && item.Values.ElementAt(i) != null)
                                {
                                    string columnName = item.Keys.ElementAt(i);
                                    string[] parts = columnName.Split("/".ToCharArray());

                                    if (IsInFunctionList(Constant.FunctionID.ActualWorkDetail))
                                    {

                                        var sb = new StringBuilder();
                                        var showNum = item.Values.ElementAt(i).ToString();
                                        var showNumBreak = showNum.Split('/'); //showNumBreak[1]: Individual Sales; showNumBreak[0]: Base Unit Cost; 
                                        var individualSales = showNumBreak[1];
                                        var baseUnitCost = showNumBreak[0];
                                        var checkGetOverPrice = "";

                                        if (individualSales != "-" && baseUnitCost != "-")
                                        {
                                            var decIndividualSales = Convert.ToDecimal(individualSales);
                                            var decBaseUnitCost = Convert.ToDecimal(baseUnitCost);
                                            if (decBaseUnitCost > decIndividualSales)
                                            {
                                                checkGetOverPrice = "style='color:red'";//paint red to text
                                            }
                                        }

                                        if (individualSales != "-")
                                        {
                                            individualSales = Convert.ToDecimal(individualSales).ToString("#,##0") + "円";
                                        }
                                        else
                                        {
                                            if (baseUnitCost != "-")
                                            {
                                                if (Convert.ToDecimal(baseUnitCost) > 0)
                                                {
                                                    checkGetOverPrice = "style='color:red'";//paint red to text
                                                }
                                            }
                                        }

                                        if (baseUnitCost != "-")
                                        {
                                            baseUnitCost = Convert.ToDecimal(baseUnitCost).ToString("#,##0") + "円";
                                        }

                                        sb.Append(@"<span user_id='" + item.Values.ElementAt(1)
                                            + "' selected_year='" + parts[0]
                                            + "' selected_month='" + parts[1] + "' " + checkGetOverPrice + ">"
                                            + individualSales + " / " + baseUnitCost
                                            + "</span>");
                                        arr[i] = sb.ToString();
                                    }
                                    else
                                    {
                                        arr[i] = Convert.ToDecimal(item.Values.ElementAt(i).ToString()).ToString();//.ToString("#,##0.00");
                                    }
                                }
                                else if (i == 4 || i == 3)
                                {
                                    var tmpValue = item.Values.ElementAt(i);
                                    arr[i] = HttpUtility.HtmlEncode(tmpValue == null ? string.Empty : tmpValue.ToString());
                                }
                                else
                                {
                                    arr[i] = item.Values.ElementAt(i);
                                }

                            }
                            listData.Add(arr);
                        }
                    }

                    var result2 = Json(
                        new
                        {
                            sEcho = model.sEcho,
                            iTotalRecords = listAllPriceInfor.Count,
                            iTotalDisplayRecords = listAllPriceInfor.Count,
                            aaData = listData.ToList<object[]>()
                        });

                    return result2;
                }
            }

            return new EmptyResult();
        }



        /// <summary>
        /// Export Csv By Unit Price Infor
        /// </summary>
        /// <param name="start_date">start date</param>
        /// <param name="end_date">end date</param>
        /// <param name="user_name">user name</param>
        /// <param name="group_id">group id</param>
        /// <param name="sort_colum"></param>
        /// <param name="sort_type"></param>
        /// <param name="retired_flag"></param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns></returns>
        public ActionResult ExportCsvByUnitPriceInfor(string location, string start_date, string end_date, string status, string user_name, int? group_id, int sort_colum, string sort_type, bool retired_flag, string TAB_ID)
        {
            UserCondition condition = new UserCondition();
            condition.GROUP_ID = group_id;
            condition.USER_NAME = user_name;
            condition.START_DATE = start_date;
            condition.END_DATE = end_date;
            condition.RETIRED_INCLUDE = retired_flag;
            condition.STATUS_ID = status;
            condition.LOCATION_ID = location;

            IList<string> jsonResults = new List<string>();
            IList<dynamic> results = new List<dynamic>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                jsonResults = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<string>;
                results = Utility.DeserializeJsonToDynamic(jsonResults);
            }
            else
            {
                results = this._service.GetListPriceInfor(condition, GetLoginUser().CompanyCode, sort_colum, sort_type);
            }

            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
            foreach (var t in results)
            {
                list.Add(t);
            }
            List<string> fixColumns = new List<string>() {
                "No.",
                "所属",
                "ユーザー名"
            };

            List<string> listMonth = Utility.getListMonth(condition.START_DATE, condition.END_DATE);
            string[] columns = fixColumns.Concat(listMonth).ToArray();

            // Convert to Datatable
            DataTable dataTable = new DataTable(list.ToString());

            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(columns[i]);
            }

            foreach (IDictionary<string, object> item in list)
            {
                var values = new object[item.Values.Count - 1];
                int i = 0;
                values[i] = item.Values.ElementAt(i);
                for (i = 2; i < item.Values.Count; i++)
                {
                    if (i >= 4 && item.Values.ElementAt(i) != null)
                    {
                        var showNum = item.Values.ElementAt(i).ToString();
                        var showNumBreak = showNum.Split('/'); //showNumBreak[1]: Individual Sales; showNumBreak[0]: Base Unit Cost; 
                        var individualSales = showNumBreak[1];
                        var baseUnitCost = showNumBreak[0];

                        if (individualSales != "-")
                        {
                            individualSales = Convert.ToInt32(individualSales).ToString("#,##0") + "円";
                        }

                        if (baseUnitCost != "-")
                        {
                            baseUnitCost = Convert.ToInt32(baseUnitCost).ToString("#,##0") + "円";
                        }

                        values[i - 1] = individualSales + " / " + baseUnitCost;
                    }
                    else
                    {
                        values[i - 1] = item.Values.ElementAt(i);
                    }
                }
                dataTable.Rows.Add(values);
            }

            // Export Table to CSV file
            Utility.ExportToCsvData(this, dataTable, "PriceInfor_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv");

            return new EmptyResult();
        }


        #endregion
    }
}
