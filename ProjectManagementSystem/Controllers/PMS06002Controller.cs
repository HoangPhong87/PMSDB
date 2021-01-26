using System;
using System.Data;
using System.Web;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS06002;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS06002;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;
using ProjectManagementSystem.Resources;
using Newtonsoft.Json;

namespace ProjectManagementSystem.Controllers
{
    /// <summary>
    /// Controller class for the PMS06002 controller
    /// </summary>
    public class PMS06002Controller : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// Common service
        /// </summary>
        private readonly IPMSCommonService commonService;

        /// <summary>
        /// Service instance reference
        /// </summary>
        private readonly IPMS06002Service service;

        /// <summary>
        /// Service instance reference
        /// </summary>
        private readonly IPMS10003Service serviceCompanySetting;

        /// <summary>
        /// Template old userID
        /// </summary>
        private string TEMPDATA_EDIT = "PMS06002EDIT";
        /// <summary>
        /// TempData storage
        /// </summary>
        [System.Serializable]
        private class TmpValues
        {
            public int UserID { get; set; }
            public int Day { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
            public string RegisterType { get; set; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PMS06002Controller()
            : this(new PMS06002Service(), new PMSCommonService(), new PMS10003Service())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service">service instance</param>
        public PMS06002Controller(IPMS06002Service service, IPMSCommonService commonservice, IPMS10003Service serviceCompanySetting)
        {
            this.service = service;
            this.commonService = commonservice;
            this.serviceCompanySetting = serviceCompanySetting;
        }

        #endregion

        #region ActualWorkList
        /// <summary>
        /// Default action, display the ActualWorkList
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if (!IsInFunctionList(Constant.FunctionID.ActualWorkList))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var currentUser = GetLoginUser();
            var model = new PMS06002ListViewModel();

            model.GroupList = this.commonService.GetUserGroupSelectList(currentUser.CompanyCode);
            model.Condition.CompanyCode = currentUser.CompanyCode;
            model.Condition.GroupId = currentUser.GroupId;
            model.BranchList = this.commonService.GetBranchSelectList(currentUser.CompanyCode);

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("List", model);
        }

        /// <summary>
        /// Action return the ActualWorkList to display
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="sortColumn">sort column</param>
        /// <param name="sortType">sort type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Actual work list</returns>
        public ActionResult ActualWorkList(DataTablesModel model, Condition condition, int sortColumn, string sortType, string TAB_ID)
        {
            if (ModelState.IsValid)
            {
                var currentUser = GetLoginUser();
                condition.CompanyCode = currentUser.CompanyCode;

                if (currentUser.PlanFunctionList.Contains(Constant.FunctionID.CheckInputStatus) && currentUser.FunctionList.Contains(Constant.FunctionID.CheckInputStatus))
                {
                    condition.Filterable = 1;
                }
                else
                {
                    condition.Filterable = 0;
                }

                var pageInfo = this.service.GetActualWorkList(model, condition, null);
                var results = this.service.GetWorkListExport(condition, sortColumn, sortType);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = Utility.SerializeDynamicToJson(results);

                if (condition.Filterable == 1)
                {
                    string userIds = "";
                    // Get list of userId in result page
                    foreach (var data in pageInfo.Items)
                    {
                        var tempDictionary = (IDictionary<string, object>)data;
                        string userId = tempDictionary["user_sys_id"].ToString();
                        userIds += userId + ",";
                    }
                    if (userIds != "")
                    {
                        userIds = userIds.Substring(0, userIds.Length - 1);
                        DataTablesModel newModel = model;
                        newModel.iDisplayStart = 0;
                        var inputStatus = this.service.GetActualWorkList(newModel, condition, userIds);
                        //update info of input status
                        pageInfo.Items = inputStatus.Items;
                    }
                }

                IList<object> dataList = new List<object>();

                int countMonth = ((condition.EndMonth.Year - condition.StartMonth.Year) * 12) + condition.EndMonth.Month - condition.StartMonth.Month;

                foreach (var data in pageInfo.Items)
                {
                    IList<string> dataItem = new List<string>();
                    var tempDictionary = (IDictionary<string, object>)data;
                    for (int i = 4; i <= (countMonth + 4); i++)
                    {
                        if (Convert.ToInt32(tempDictionary.Keys.ElementAt(i).ToString().Replace("/", "")) > Convert.ToInt32(Utility.GetCurrentDateTime().ToString("yyyyMM")))
                        {
                            var tempDateString = (tempDictionary.Values.ElementAt(i) ?? "").ToString();
                            if (tempDateString.EndsWith("(0)"))
                            {
                                tempDateString = tempDateString.Remove(tempDateString.Length - 3);
                                tempDateString += "(1)";
                            }

                            tempDictionary[tempDictionary.Keys.ElementAt(i).ToString()] = tempDateString;
                        }
                        if (tempDictionary.Values.ElementAt(i) == null || string.IsNullOrWhiteSpace(tempDictionary.Values.ElementAt(i).ToString()))
                        {
                            tempDictionary[tempDictionary.Keys.ElementAt(i).ToString()] = "0.00/0.00(0)(1)";
                        }
                        dataItem.Add(this.FormatData(tempDictionary.ToList()[i].Value));
                    }
                    dataList.Add(dataItem);
                }

                var result =
                    Json(data: new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData =
                                    (from t in pageInfo.Items
                                     select new object[]
                                        {
                                            ((IDictionary<string, object>)t).ToList()[3].Value,
                                            ((IDictionary<string, object>)t).ToList()[0].Value,
                                            this.EncodeData(((IDictionary<string, object>)t).ToList()[1].Value),
                                            this.EncodeData(((IDictionary<string, object>)t).ToList()[2].Value)
                                        }
                                    ).ToList(),
                        datalist = dataList
                    });
                return result;

            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export Csv Actual List
        /// </summary>
        /// <param name="start_date">start_date</param>
        /// <param name="end_date">end_date</param>
        /// <param name="user_name">user_name</param>
        /// <param name="group_id">group_id</param>
        /// <param name="eff_type">eff_type</param>
        /// <param name="sort_colum">sort_colum</param>
        /// <param name="sort_type">sort_type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>CSV File</returns>
        public ActionResult ExportCsvActualList(DateTime start_date, DateTime end_date, string user_name, int? group_id, string search_locationID, string eff_type, bool deleteFlag, bool retirementFlag, int sort_colum, string sort_type, string TAB_ID)
        {
            Condition listCondition = new Condition()
            {
                CompanyCode = GetLoginUser().CompanyCode,
                StartMonth = start_date,
                EndMonth = end_date,
                GroupId = group_id,
                DisplayName = user_name,
                WorkTimeUnit = eff_type,
                DELETED_INCLUDE = deleteFlag,
                RETIREMENT_INCLUDE = retirementFlag,
                LOCATION_ID = search_locationID
            };

            IList<string> jsonResults = new List<string>();
            IList<dynamic> results = new List<dynamic>();

            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                jsonResults = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<string>;
                results = Utility.DeserializeJsonToDynamic(jsonResults);
            }
            else
            {
                results = this.service.GetWorkListExport(listCondition, sort_colum, sort_type);
            }

            List<string> fixColumns = new List<string>() {
                "No.",
                "所属",
                "ユーザー名","gfuhgu"
            };

            List<string> listMonth = Utility.getListMonth(listCondition.StartMonth.ToString("yyyy/MM"), listCondition.EndMonth.ToString("yyyy/MM"));
            string[] columns = fixColumns.Concat(listMonth).ToArray();

            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
            foreach (var t in results)
            {
                list.Add(t);
            }
            // Convert to Datatable
            DataTable dataTable = new DataTable(list.ToString());

            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(columns[i]);
            }

            foreach (IDictionary<string, object> item in list)
            {
                var values = new object[item.Values.Count];
                for (int i = 0; i < item.Values.Count; i++)
                {
                    if (i <= 3)
                    {
                        values[i] = item.Values.ElementAt(i);
                    }
                    else if (i > 4)
                            {
                                values[i] = this.FormatData(item.Values.ElementAt(i), isExport: true);
                            }
                    else
                    {
                        values[i] = this.FormatData(item.Values.ElementAt(i), isExport: false);
                    }
                }
                dataTable.Rows.Add(values);
            }

            // Export Table to CSV file
            Utility.ExportToCsvData(this, dataTable, "ActualWorkList_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv");
            return new EmptyResult();
        }

        #endregion

        #region ActualWorkDetailNew
        /// <summary>
        /// Action return DetailNew screen
        /// </summary>
        /// <param name="user_id">user_id</param>
        /// <param name="selected_year">selected_year</param>
        /// <param name="selected_month">selected_month</param>
        /// <returns>Detail New</returns>
        public ActionResult DetailNew(string user_id = "", int selected_year = -1, int selected_month = -1)
        {
            try
            {
                var model = new PMS06002DetailNewViewModel();
                if (string.IsNullOrEmpty(user_id))
                {
                    model.Condition.UserId = GetLoginUser().UserId.ToString();
                }
                else
                {
                    int value = Convert.ToInt32(user_id);
                    if (value == -1)
                        model.Condition.UserId = Session["ACTUAL_WORK_DETAIL_LAST_USER_ID"].ToString();
                    else
                        model.Condition.UserId = user_id;
                }

                Session["ACTUAL_WORK_DETAIL_LAST_USER_ID"] = model.Condition.UserId;

                if (selected_year != -1)
                {
                    model.Condition.SelectedYear = selected_year;
                }

                if (selected_month != -1)
                {
                    model.Condition.SelectedMonth = selected_month;
                }

                model.CurrentYearMonth = string.Format("{0}年{1}月", model.Condition.SelectedYear, model.Condition.SelectedMonth.ToString("00"));
                // todo GET INFO
                DetailCondition condition = model.Condition;
                var currentUser = GetLoginUser();

                #region Init Header Info
                var userinfo = this.service.GetUserBaseInfo(condition.UserId);
                if (userinfo != null)
                {
                    model.GroupName = string.IsNullOrEmpty(userinfo.group_name) ? " " : userinfo.group_name;
                    model.UserName = string.IsNullOrEmpty(userinfo.display_name) ? " " : userinfo.display_name;
                    model.EmployeeNo = string.IsNullOrEmpty(userinfo.employee_no) ? " " : userinfo.employee_no;
                    var results = this.service.GetDetailInfo(condition, userinfo.company_code);
                    if (results != null)
                    {
                        model.EstimatedTime = results.plan_effort.ToString("#,##0.00");
                        model.ActualTime = results.actual_effort.ToString("#,##0.00");

                        results.estimate_cost = Utility.RoundNumber(results.estimate_cost, currentUser.DecimalCalculationType, false);
                        model.EstimatedTimeTotal = results.estimate_cost.ToString("#,##0");

                        results.actual_cost = Utility.RoundNumber(results.actual_cost, currentUser.DecimalCalculationType, false);
                        model.TotalCost = results.actual_cost.ToString("#,##0");

                        model.TotalIncome = (results.estimate_cost - results.actual_cost).ToString("#,##0");

                    }

                    var updateInfo = this.service.GetUpdateInfor(condition, userinfo.company_code);
                    if (updateInfo == null)
                    {
                        updateInfo = new UpdateInfo();
                    }
                    else
                    {
                        if (updateInfo.regist_type.Equals(Constant.RegistType.ACTUAL_REGIST))
                        {
                            model.CbRegistType = false;
                        }
                        else
                        {
                            model.CbRegistType = true;
                        }
                        updateInfo = new UpdateInfo()
                        {
                            regist_type = updateInfo.regist_type,
                            insert_date = DateTime.Parse(updateInfo.insert_date).ToString("yyyy/MM/dd HH:mm"),
                            insert_person = updateInfo.insert_person,
                            last_update_date = DateTime.Parse(updateInfo.last_update_date).ToString("yyyy/MM/dd HH:mm"),
                            last_update_person = updateInfo.last_update_person
                        };
                    }
                    model.UpdateInfo = updateInfo;
                }
                #endregion

                #region InitDetailWorkingTime
                List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
                var items = service.GetActualWorkDetailNew(condition, currentUser.CompanyCode);
                foreach (var t in items)
                {
                    list.Add(t);
                }
                List<IDictionary<string, object>> list2 = new List<IDictionary<string, object>>();
                Dictionary<string, object> emptyItem = new Dictionary<string, object>();

                var daysInMonth = DateTime.DaysInMonth(model.Condition.SelectedYear, model.Condition.SelectedMonth);
                if (items.Count == 0)
                {
                    for (int i = 1; i <= daysInMonth; i++)
                    {
                        emptyItem.Add("actual_work_date", i);
                        emptyItem.Add("work_start_time", null);
                        emptyItem.Add("work_end_time", null);
                        emptyItem.Add("rest_time", null);
                        emptyItem.Add("display_name", null);
                        emptyItem.Add("total_work_time", null);
                        list2.Add(emptyItem);
                        emptyItem = new Dictionary<string, object>();
                    }
                }
                else
                {
                    for (int i = 1; i <= daysInMonth; i++)
                    {
                        var existData = false;
                        foreach (var l in list)
                        {
                            if ((int)l.Values.ElementAt(0) == i)
                            {
                                existData = true;
                                list2.Add(l);
                            }
                        }
                        if (!existData)
                        {
                            foreach (var t in list[0].Keys)
                            {
                                if (t == list[0].Keys.ElementAt(0))
                                {
                                    emptyItem.Add(list[0].Keys.ElementAt(0), i);
                                }
                                else
                                {
                                    emptyItem.Add(t, null);
                                }
                            }
                            list2.Add(emptyItem);
                            emptyItem = new Dictionary<string, object>();
                        }
                    }
                }
                model.HolidayInfo = service.GetHolidayInfo(currentUser.CompanyCode);
                #endregion

                #region Init Summary Info
                model.UserActualWorkDetailPlus = service.GetUserActualWorkDetailPlus(condition, currentUser.CompanyCode);
                #endregion
                #region Remove project in ActualWorkDetailNew not exist in UserActualWorkDetailPlus
                List<string> projectNos = model.UserActualWorkDetailPlus.Select(x => x.project_no).ToList();
                List<string> missingProjectNos = new List<string>();
                for (var i = 6; i < list2[0].Keys.Count(); i++)
                {
                    if (!projectNos.Contains(list2[0].Keys.ElementAt(i)))
                    {
                        missingProjectNos.Add(list2[0].Keys.ElementAt(i));
                    }
                }
                foreach (var missingProjectNo in missingProjectNos)
                {
                    foreach (var list2Item in list2)
                    {
                        list2Item.Remove(missingProjectNo);
                    }
                }
                model.ActualWorkDetailNew = list2;
                #endregion
                Session["PMS06001_Plan_From"] = "PersonalRecord";
                Session["user_id"] = model.Condition.UserId;
                Session["selected_year"] = model.Condition.SelectedYear;
                Session["selected_month"] = model.Condition.SelectedMonth;

                return this.View("DetailNew", model);
            }
            catch
            {
                return this.RedirectToAction("Index", "Error");
            }
        }

        /// <summary>
        /// Action for downloading Xlsx file
        /// </summary>
        /// <param name="user_id">user_id</param>
        /// <param name="selected_year">selected_year</param>
        /// <param name="selected_month">selected_month</param>
        /// <returns>File Download </returns>
        public ActionResult DownloadXlsxFile(string user_id = "", int selected_year = -1, int selected_month = -1)
        {
            var AttInfo = new AttendanceInfo();
            // ExcelCreator
            int WorkClosingDay = service.GetWorkClosingDate(GetLoginUser().CompanyCode);
            int YearOfPreMonth = selected_month == 1 ? selected_year - 1 : selected_year;
            int MonthOfPreMonth = selected_month == 1 ? 12 : selected_month - 1;

            int DaysInPreMonth = DateTime.DaysInMonth(YearOfPreMonth, MonthOfPreMonth);
            int DaysInMonth = DateTime.DaysInMonth(selected_year, selected_month);


            int DayToExport = WorkClosingDay > DaysInPreMonth ? DaysInPreMonth : (WorkClosingDay == 30 && DaysInPreMonth == 31 ? 31 : WorkClosingDay);
            DateTime FromDate = new DateTime(YearOfPreMonth, MonthOfPreMonth, DayToExport).AddDays(1);

            DayToExport = WorkClosingDay > DaysInMonth ? DaysInMonth : (WorkClosingDay == 30 && DaysInMonth == 31 ? 31 : WorkClosingDay);
            DateTime ToDate = new DateTime(selected_year, selected_month, DayToExport);


            AttInfo.UserInfo = this.service.GetUserBaseInfo(user_id);
            AttInfo.AttDetail = new List<AttendanceDetail>();
            AttInfo.WorkClosingDay = DayToExport;
            AttInfo.SelectedYear = selected_year;
            AttInfo.SelectedMonth = selected_month;

            var attDetailResults = service.GetAttendanceInfor(GetLoginUser().CompanyCode, Convert.ToInt32(user_id), FromDate, ToDate).ToList();

            for (DateTime i = FromDate; i <= ToDate; i = i.AddDays(1))
            {
                bool existData = false;
                foreach (var att in attDetailResults)
                {
                    if (att.working_date.Equals(i))
                    {
                        AttInfo.AttDetail.Add(att);
                        attDetailResults.Remove(att);
                        existData = true;
                        break;
                    }
                }
                if (!existData)
                {
                    AttInfo.AttDetail.Add(new AttendanceDetail()
                    {
                        allowed_cost_time = 0,
                        remarks = "",
                        rest_time = 0,
                        work_end_time = 0,
                        work_start_time = 0,
                        working_date = i,
                        EmptyData = true
                    }
                    );
                }
            }

            Utility.DownloadXlsxFile(this, AttInfo, "勤務表.xlsx", "勤務表(" + AttInfo.UserInfo.user_name_sei + AttInfo.UserInfo.user_name_mei + selected_year.ToString() + selected_month.ToString().PadLeft(2, '0') + ").xlsx");

            return new EmptyResult();
        }
        #endregion

        #region ActualWorkDetail
        /// <summary>
        /// Detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail()
        {
            if (!IsInFunctionList(Constant.FunctionID.ActualWorkDetail))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var model = new PMS06002DetailViewModel();

            model.Condition.UserId = GetLoginUser().UserId.ToString();
            if (GetLoginUser().ActualWorkInputMode == Constant.ActualWorkModeFlag.MODE_NEW)
            {
                return this.DetailNew();
            }

            Session["ACTUAL_WORK_DETAIL_LAST_USER_ID"] = model.Condition.UserId;
            string month = string.Empty;
            if (model.Condition.SelectedMonth < 10)
            {
                month = "0" + model.Condition.SelectedMonth;
            }
            else
            {
                month = model.Condition.SelectedMonth.ToString();
            }
            model.CurrentYearMonth = string.Format("{0}年{1}月", model.Condition.SelectedYear, month);
            // todo GET INFO
            var condition = model.Condition;
            var userinfo = this.service.GetUserBaseInfo(condition.UserId);
            if (userinfo != null)
            {
                model.GroupName = string.IsNullOrEmpty(userinfo.group_name) ? " " : userinfo.group_name;
                model.UserName = string.IsNullOrEmpty(userinfo.display_name) ? " " : userinfo.display_name;
                model.EmployeeNo = string.IsNullOrEmpty(userinfo.employee_no) ? " " : userinfo.employee_no;
                var results = this.service.GetDetailInfo(condition, userinfo.company_code);
                if (results != null)
                {

                    model.EstimatedTime = results.plan_effort.ToString("#,##0.00");
                    model.ActualTime = results.actual_effort.ToString("#,##0.00");

                    results.estimate_cost = Utility.RoundNumber(results.estimate_cost, GetLoginUser().DecimalCalculationType, false);
                    model.EstimatedTimeTotal = results.estimate_cost.ToString("#,##0");

                    results.actual_cost = Utility.RoundNumber(results.actual_cost, GetLoginUser().DecimalCalculationType, false);
                    model.TotalCost = results.actual_cost.ToString("#,##0");

                    model.TotalIncome = (results.estimate_cost - results.actual_cost).ToString("#,##0");
                    model.RegistType = results.regist_type;
                }

                var updateInfo = this.service.GetUpdateInfor(condition, userinfo.company_code);
                if (updateInfo == null)
                {
                    updateInfo = new UpdateInfo();
                }
                else
                {
                    if (updateInfo.regist_type.Equals(Constant.RegistType.ACTUAL_REGIST))
                    {
                        model.CbRegistType = false;
                    }
                    else
                    {
                        model.CbRegistType = true;
                    }

                    updateInfo = new UpdateInfo()
                    {
                        regist_type = updateInfo.regist_type,
                        insert_date = DateTime.Parse(updateInfo.insert_date).ToString("yyyy/MM/dd HH:mm"),
                        insert_person = updateInfo.insert_person,
                        last_update_date = DateTime.Parse(updateInfo.last_update_date).ToString("yyyy/MM/dd HH:mm"),
                        last_update_person = updateInfo.last_update_person
                    };
                }
                model.UpdateInfo = updateInfo;
            }

            Session["PMS06001_Plan_From"] = "PersonalRecord";
            Session["user_id"] = model.Condition.UserId;
            Session["selected_year"] = model.Condition.SelectedYear;
            Session["selected_month"] = model.Condition.SelectedMonth;
            return this.View("Detail", model);
        }
        /// <summary>
        /// Detail
        /// </summary>
        /// <param name="id">user_id</param>
        /// <param name="key">selected_year</param>
        /// <param name="key2">selected_month</param>
        /// <param name="key3">time_unit</param>
        /// <returns>Detail View</returns>
        [HttpPost]
        public ActionResult Detail(string user_id = "", int selected_year = -1, int selected_month = -1, string time_unit = "")
        {
            if (!IsInFunctionList(Constant.FunctionID.ActualWorkDetail))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            if (GetLoginUser().ActualWorkInputMode == Constant.ActualWorkModeFlag.MODE_NEW)
            {
                return this.DetailNew(user_id, selected_year, selected_month);
            }

            var model = new PMS06002DetailViewModel();

            if (string.IsNullOrEmpty(user_id))
            {
                model.Condition.UserId = GetLoginUser().UserId.ToString();
            }
            else
            {
                int value = Convert.ToInt32(user_id);
                if (value == -1)
                    model.Condition.UserId = Session["ACTUAL_WORK_DETAIL_LAST_USER_ID"].ToString();
                else
                    model.Condition.UserId = user_id;
            }

            Session["ACTUAL_WORK_DETAIL_LAST_USER_ID"] = model.Condition.UserId;

            if (selected_year != -1)
            {
                model.Condition.SelectedYear = selected_year;
            }

            if (selected_month != -1)
            {
                model.Condition.SelectedMonth = selected_month;
            }

            if (!string.IsNullOrEmpty(time_unit))
            {
                model.Condition.WorkTimeUnit = time_unit;
            }

            string month = string.Empty;
            if (model.Condition.SelectedMonth < 10)
            {
                month = "0" + model.Condition.SelectedMonth;
            }
            else
            {
                month = model.Condition.SelectedMonth.ToString();
            }
            model.CurrentYearMonth = string.Format("{0}年{1}月", model.Condition.SelectedYear, month);
            // todo GET INFO
            DetailCondition condition = model.Condition;

            var userinfo = this.service.GetUserBaseInfo(condition.UserId);
            if (userinfo != null)
            {
                model.GroupName = string.IsNullOrEmpty(userinfo.group_name) ? " " : userinfo.group_name;
                model.UserName = string.IsNullOrEmpty(userinfo.display_name) ? " " : userinfo.display_name;
                model.EmployeeNo = string.IsNullOrEmpty(userinfo.employee_no) ? " " : userinfo.employee_no;
                var results = this.service.GetDetailInfo(condition, userinfo.company_code);
                if (results != null)
                {

                    model.EstimatedTime = results.plan_effort.ToString("#,##0.00");
                    model.ActualTime = results.actual_effort.ToString("#,##0.00");

                    results.estimate_cost = Utility.RoundNumber(results.estimate_cost, GetLoginUser().DecimalCalculationType, false);
                    model.EstimatedTimeTotal = results.estimate_cost.ToString("#,##0");

                    results.actual_cost = Utility.RoundNumber(results.actual_cost, GetLoginUser().DecimalCalculationType, false);
                    model.TotalCost = results.actual_cost.ToString("#,##0");

                    model.TotalIncome = (results.estimate_cost - results.actual_cost).ToString("#,##0");
                    model.RegistType = results.regist_type;
                }

                var updateInfo = this.service.GetUpdateInfor(condition, userinfo.company_code);
                if (updateInfo == null)
                {
                    updateInfo = new UpdateInfo();
                }
                else
                {
                    if (updateInfo.regist_type.Equals(Constant.RegistType.ACTUAL_REGIST))
                    {
                        model.CbRegistType = false;
                    }
                    else
                    {
                        model.CbRegistType = true;
                    }

                    updateInfo = new UpdateInfo()
                    {
                        regist_type = updateInfo.regist_type,
                        insert_date = DateTime.Parse(updateInfo.insert_date).ToString("yyyy/MM/dd HH:mm"),
                        insert_person = updateInfo.insert_person,
                        last_update_date = DateTime.Parse(updateInfo.last_update_date).ToString("yyyy/MM/dd HH:mm"),
                        last_update_person = updateInfo.last_update_person
                    };
                }
                model.UpdateInfo = updateInfo;
            }
            Session["PMS06001_Plan_From"] = "PersonalRecord";
            Session["user_id"] = model.Condition.UserId;
            Session["selected_year"] = model.Condition.SelectedYear;
            Session["selected_month"] = model.Condition.SelectedMonth;

            return this.View(model);
        }

        /// <summary>
        /// Format
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>string format</returns>
        private string FormatToHour(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                decimal dValue = Convert.ToDecimal(value);
                int hour = (int)Math.Floor(dValue);
                int min = (int)Math.Round((dValue - hour) * 60);
                return string.Format("{0:00}:{1:00}h", hour, min);
            }
            else
            {
                return "00:00h";
            }
        }
        /// <summary>
        /// Action return the ActualWorkList to display
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Actual work detail</returns>
        public ActionResult SearchActualWorkDetail(DataTablesModel model, DetailCondition condition, string TAB_ID)
        {
            if (Request.IsAjaxRequest())
            {
                var pageInfo = this.service.GetWorkDetail(model, condition, GetLoginUser().CompanyCode);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = pageInfo.Items;
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
                            (t.count_project_plan > 0) ? true : false,
                            t.rank != null ? HttpUtility.HtmlEncode(t.rank): "",
                            Utility.RoundNumber(t.progress, GetLoginUser().DecimalCalculationType, true).ToString() + "%",
                            t.actual_effort,
                            Utility.RoundNumber(t.personal_profit_rate, GetLoginUser().DecimalCalculationType, true).ToString() + "%",
                            Utility.RoundNumber(t.project_actual_profit, GetLoginUser().DecimalCalculationType, true).ToString() + "%",
                            t.plan_effort
                        }).ToList()
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export to CSV
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="selected_year"></param>
        /// <param name="selected_month"></param>
        /// <param name="time_unit"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="TAB_ID"></param>
        /// <returns>CSV file export</returns>
        [HttpPost]
        public ActionResult ExportCsv(string user_id = "", int selected_year = -1, int selected_month = -1, string time_unit = "", int sortCol = 0, string sortDir = "asc", string TAB_ID = "")
        {
            DetailCondition condition = new DetailCondition()
            {
                UserId = user_id,
                SelectedYear = selected_year,
                SelectedMonth = selected_month,
                WorkTimeUnit = time_unit
            };

            DataTablesModel model = new DataTablesModel
            {
                sEcho = "1",
                iColumns = 4,
                sColumns = "project_sys_id,project_name,rank,progress",
                iDisplayStart = 0,
                iDisplayLength = int.MaxValue,
                iSortCol_0 = sortCol,
                sSortDir_0 = sortDir,
                iSortingCols = 1
            };

            IList<UserActualWorkDetailPlus> results = new List<UserActualWorkDetailPlus>();
            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                results = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<UserActualWorkDetailPlus>;
            }
            else
            {
                results = this.service.GetWorkDetail(model, condition, GetLoginUser().CompanyCode).Items;
            }
            List<UserActualWorkDetailExport> dataExport = new List<UserActualWorkDetailExport>();
            foreach (var r in results)
            {
                UserActualWorkDetailExport tmpData = new UserActualWorkDetailExport();
                tmpData.project_name = r.project_name;
                tmpData.rank = r.rank;
                tmpData.progress = Utility.RoundNumber(r.progress, GetLoginUser().DecimalCalculationType, true).ToString() + "%";
                tmpData.actual_plan_effort = r.actual_effort + "/" + r.plan_effort;
                tmpData.personal_profit_rate = Utility.RoundNumber(r.personal_profit_rate, GetLoginUser().DecimalCalculationType, true).ToString() + "%";
                tmpData.project_actual_profit = Utility.RoundNumber(r.project_actual_profit, GetLoginUser().DecimalCalculationType, true).ToString() + "%";
                dataExport.Add(tmpData);
            }

            string[] columns = new[]
            {
                    "プロジェクト名",
                    "ランク",
                    "進捗",
                    "実績工数/予定工数",
                    "個人利益率",
                    "PJ利益率"
            };
            DataTable dt = Utility.ToDataTableT(dataExport, columns.ToArray());
            Utility.ExportToCsvData(this, dt, "ActualWorkDetail_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv");

            return new EmptyResult();
        }

        /// <summary>
        /// get list actual work exist in month
        /// </summary>
        /// <param name="user_sys_id"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ActualWorkDateList(int user_sys_id, int year, int month)
        {
            if (!Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 403;
                return new EmptyResult();
            }

            string company_code = GetLoginUser().CompanyCode;
            var data = service.ActualWorkDateList(user_sys_id, year, month, company_code);

            JsonResult result = Json(data, JsonRequestBehavior.AllowGet);

            return result;
        }

        #endregion

        #region ActualWorkRegist
        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="key">key</param>
        /// <param name="key2">key2</param>
        /// <param name="key2">key3</param>
        /// <returns>Edit view</returns>
        public ActionResult Edit(string id, string key, string key2, string key3)
        {
            try
            {
                if (!IsInFunctionList(Constant.FunctionID.ActualWorkRegist))
                {
                    return this.RedirectToAction("Index", "ErrorAuthent");
                }
                string action = Request.HttpMethod.ToString();

                if (action.Equals("GET"))
                {
                    var tmp = TempData[TEMPDATA_EDIT] as TmpValues;

                    if ((string.IsNullOrEmpty(id) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(key2)) && tmp != null)
                    {
                        id = tmp.UserID.ToString();
                        key = tmp.Year.ToString();
                        key2 = tmp.Month.ToString();
                    }
                    else
                    {
                        return this.RedirectToAction("Index", "Error");
                    }
                }

                int userId = Convert.ToInt32(id);
                int year = Convert.ToInt32(key);
                int month = Convert.ToInt32(key2);
                DetailCondition condition = new DetailCondition()
                {
                    UserId = userId.ToString(),
                    SelectedYear = year,
                    SelectedMonth = month
                };

                // get UpdateInfo
                var updateInfo = this.service.GetUpdateInfor(condition, GetLoginUser().CompanyCode);
                if (updateInfo == null)
                {
                    updateInfo = new UpdateInfo();
                }
                else
                {
                    updateInfo = new UpdateInfo()
                    {
                        insert_date = DateTime.Parse(updateInfo.insert_date).ToString("yyyy/MM/dd HH:mm"),
                        insert_person = updateInfo.insert_person,
                        last_update_date = DateTime.Parse(updateInfo.last_update_date).ToString("yyyy/MM/dd HH:mm"),
                        last_update_person = updateInfo.last_update_person
                    };
                }

                var model = new PMS06002ActualWorkRegistModel
                {
                    company_code = this.GetLoginUser().CompanyCode,
                    user_sys_id = userId,
                    target_month = month,
                    target_year = year,
                    ActualWorkList = service.GetActualWorkDetail(userId, year, month),
                    UserWorkInfo = service.GetUserWorkInfo(userId, year, month),
                    WorkAttendanceDetail = service.GetWorkAttendanceDetail(userId, year, month, null),
                    UpdateInfo = updateInfo,
                    HolidayInfo = service.GetHolidayInfo(GetLoginUser().CompanyCode),
                };
                model.regist_type = key3;
                model.AttendanceTypeList = this.GetAttendanceTypeList();

                TempData[TEMPDATA_EDIT] = new TmpValues() { UserID = userId, Month = month, Year = year };
                return this.View("Register", model);

            }
            catch
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
        }

        /// <summary>
        /// Update Actual work detail form Regist screen
        /// </summary>
        /// <param name="dataListMemberActualWorkDetail">dataListMemberActualWorkDetail</param>
        /// <param name="dataMemberActualWork">dataMemberActualWork</param>
        /// <param name="dataListAttendanceRecord">dataListAttendanceRecord</param>
        /// <param name="regist_type">regist_type</param>
        /// <returns>Json update infor</returns>
        [HttpPost]
        public JsonResult UpdateActualWorkDetail(
            IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            IList<AttendanceRecord> dataListAttendanceRecord, string regist_type)
        {
            try
            {

                var currentUser = this.GetLoginUser();
                int userId = currentUser.UserId;

                DateTime now = Utility.GetCurrentDateTime();
                if (dataListMemberActualWorkDetail != null)
                {
                    foreach (var actualWork in dataListMemberActualWorkDetail)
                    {
                        actualWork.company_code = currentUser.CompanyCode;
                        actualWork.ins_date = now;
                        actualWork.ins_id = userId;
                        actualWork.upd_date = now;
                        actualWork.upd_id = userId;
                        actualWork.remarks = null;
                    }
                }

                if (dataMemberActualWork != null)
                {
                    dataMemberActualWork.company_code = currentUser.CompanyCode;
                    dataMemberActualWork.regist_type = regist_type;
                    dataMemberActualWork.ins_date = now;
                    dataMemberActualWork.ins_id = userId;
                    dataMemberActualWork.upd_date = now;
                    dataMemberActualWork.upd_id = userId;
                }

                if (dataListAttendanceRecord != null)
                {
                    foreach (var attendanceRecord in dataListAttendanceRecord)
                    {
                        attendanceRecord.company_code = currentUser.CompanyCode;
                        attendanceRecord.ins_date = now;
                        attendanceRecord.ins_id = userId;
                        attendanceRecord.upd_date = now;
                        attendanceRecord.upd_id = userId;
                        attendanceRecord.remarks = null;
                    }
                }

                var result = service.PutActualWorkDetailList(dataListMemberActualWorkDetail, dataMemberActualWork, dataListAttendanceRecord);
                if (result)
                {
                    return Json(new UpdateInfo()
                    {
                        last_update_date = now.ToString("yyyy/MM/dd HH:mm"),
                        last_update_person = this.GetLoginUser().DisplayName
                    });
                }
                else
                {
                    return Json("E015");
                }
            }
            catch (Exception)
            {
                return Json("E043");
            }
        }
        #endregion

        #region ActualWorkRegistNew
        /// <summary>
        /// Action return Regist new screen
        /// </summary>
        /// <param name="day">day</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <param name="userId">userId</param>
        /// <param name="registerType">registerType</param>
        /// <returns>View Register New</returns>
        public ActionResult RegisterNew(string day, string month, string year, string userId, string registerType)
        {
            if (!IsInFunctionList(Constant.FunctionID.ActualWorkRegist))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }
            string action = Request.HttpMethod.ToString();

            if (action.Equals("GET"))
            {
                var tmp = TempData[TEMPDATA_EDIT] as TmpValues;

                if ((string.IsNullOrEmpty(day) || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(registerType)) && tmp != null)
                {
                    day = tmp.Day.ToString();
                    month = tmp.Month.ToString();
                    year = tmp.Year.ToString();
                    userId = tmp.UserID.ToString();
                    registerType = tmp.RegisterType.ToString();
                }
                else
                {
                    return this.RedirectToAction("Index", "Error");
                }
            }

            int target_date = Convert.ToInt32(day);
            int target_month = Convert.ToInt32(month);
            int target_year = Convert.ToInt32(year);
            int user_Id = Convert.ToInt32(userId);
            string companyCode = this.GetLoginUser().CompanyCode;

            var regist_type = service.GetRegistType(companyCode, user_Id, target_year, target_month);
            if (regist_type == null)
            {
                registerType = "1";
            }
            else
            {
                registerType = regist_type;
            }

            var model = new PMS06002ActualWorkRegistModelNew
            {
                company_code = companyCode,
                user_sys_id = user_Id,
                target_date = target_date,
                target_month = target_month,
                target_year = target_year,
                registerType = registerType,
                AttendanceRecordInfor = service.GetAttendanceRecordInfor(target_date, target_month, target_year, user_Id, companyCode),
                MemberActualWorkList = service.GetMemberActualWorkList(target_date, target_month, target_year, user_Id, companyCode)
            };
            model.AttendanceTypeList = this.GetAttendanceTypeList();
            TempData[TEMPDATA_EDIT] = new TmpValues() { UserID = user_Id, Day = target_date, Month = target_month, Year = target_year, RegisterType = registerType };

            //get minutes unit
            var companySetting = serviceCompanySetting.GetCompanySetting(companyCode);
            ViewBag.Minutes = companySetting.working_time_unit_minute;

            return this.View(model);
        }

        /// <summary>
        ///   Update actual work data from RegistNew screen
        /// </summary>
        /// <param name="dataListMemberActualWorkDetail">dataListMemberActualWorkDetail</param>
        /// <param name="dataMemberActualWork">dataMemberActualWork</param>
        /// <param name="dataAttendanceRecord">dataAttendanceRecord</param>
        /// <param name="regist_type">regist_type</param>
        /// <returns>Json Update info</returns>
        [HttpPost]
        public JsonResult UpdateActualWorkDetailNew(
            IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            AttendanceRecord dataAttendanceRecord, string regist_type)
        {
            try
            {
                if (dataAttendanceRecord != null && dataAttendanceRecord.allowed_cost_time > 0 && string.IsNullOrEmpty(dataAttendanceRecord.remarks))
                {
                    return Json("E010");
                }
                var currentUser = this.GetLoginUser();
                int userId = currentUser.UserId;

                DateTime now = Utility.GetCurrentDateTime();
                if (dataListMemberActualWorkDetail != null)
                {
                    foreach (var actualWork in dataListMemberActualWorkDetail)
                    {
                        actualWork.company_code = currentUser.CompanyCode;
                        actualWork.ins_date = now;
                        actualWork.ins_id = userId;
                        actualWork.upd_date = now;
                        actualWork.upd_id = userId;
                        actualWork.remarks = null;
                    }
                }

                if (dataMemberActualWork != null)
                {
                    dataMemberActualWork.company_code = currentUser.CompanyCode;
                    dataMemberActualWork.regist_type = regist_type;
                    dataMemberActualWork.ins_date = now;
                    dataMemberActualWork.ins_id = userId;
                    dataMemberActualWork.upd_date = now;
                    dataMemberActualWork.upd_id = userId;
                }

                if (dataAttendanceRecord != null)
                {
                    dataAttendanceRecord.company_code = currentUser.CompanyCode;
                    dataAttendanceRecord.ins_date = now;
                    dataAttendanceRecord.ins_id = userId;
                    dataAttendanceRecord.upd_date = now;
                    dataAttendanceRecord.upd_id = userId;
                }
                bool isExceedMaximumActualWorkTime = false;
                var result = service.PutActualWorkDetailListNew(dataListMemberActualWorkDetail, dataMemberActualWork, dataAttendanceRecord, out isExceedMaximumActualWorkTime);
                if (result)
                {
                    return Json(new UpdateInfo()
                    {
                        last_update_date = now.ToString("yyyy/MM/dd HH:mm"),
                        last_update_person = this.GetLoginUser().DisplayName
                    });
                }
                else
                {
                    if (isExceedMaximumActualWorkTime)
                    {
                        return Json("E041");
                    }
                    return Json("E015");
                }
            }
            catch (Exception)
            {
                return Json("E043");
            }
        }

        /// <summary>
        /// Using Ajax to change regist type
        /// </summary>
        /// <param name="attendance_type_id">attendance_type_id</param>
        /// <returns>Json adjustment time</returns>
        [HttpPost]
        public JsonResult GetAdjustmentTime(string attendance_type_id)
        {
            try
            {
                int id = Convert.ToInt32(attendance_type_id);
                var adjustment_time = service.GetAdjustmentTime(GetLoginUser().CompanyCode, id);
                var adjustment_time_hour = "00";
                var adjustment_time_minute = "00";
                if (adjustment_time > 0)
                {
                    adjustment_time_hour = string.Format("{0:00}", Math.Floor(adjustment_time));
                    adjustment_time_minute = string.Format("{0:00}", Math.Round(60 * (adjustment_time - Math.Floor(adjustment_time))));
                }

                Dictionary<String, Object> obj = new Dictionary<String, Object>();
                obj.Add("hour", adjustment_time_hour);
                obj.Add("minute", adjustment_time_minute);
                obj.Add("time", adjustment_time);

                return Json(obj);

            }
            catch
            {
                return Json(Messages.E051);
            }
        }

        /// <summary>
        /// Using Ajax to change regist type
        /// </summary>
        /// <param name="dataMemberActualWork">dataMemberActualWork</param>
        /// <returns>Json update info</returns>
        [HttpPost]
        public JsonResult ChangeRegistType(MemberActualWork dataMemberActualWork)
        {
            try
            {
                int userId = this.GetLoginUser().UserId;
                DateTime now = Utility.GetCurrentDateTime();
                bool valid_data = true;
                string company_code = GetLoginUser().CompanyCode;

                if (dataMemberActualWork != null)
                {
                    dataMemberActualWork.company_code = company_code;
                    dataMemberActualWork.ins_date = now;
                    dataMemberActualWork.ins_id = userId;
                    dataMemberActualWork.upd_date = now;
                    dataMemberActualWork.upd_id = userId;
                }

                if (dataMemberActualWork.regist_type.Equals("0"))
                {
                    // Regist 
                    valid_data = service.CheckDataValid(dataMemberActualWork);
                }
                if (!valid_data)
                {
                    return Json("INVALID");
                }

                var result = service.PutActualWorkDetailList(null, dataMemberActualWork, null);
                if (result)
                {
                    return Json(new UpdateInfo()
                    {
                        last_update_date = now.ToString("yyyy/MM/dd HH:mm"),
                        last_update_person = this.GetLoginUser().DisplayName
                    });
                }
                else
                {
                    return Json(Messages.E051);
                }
            }
            catch
            {
                return Json(Messages.E051);
            }
        }
        #endregion

        #region ActualWorkListByIndividualPhase
        /// <summary>
        /// Default action, display the ActualWorkListBy User
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult ActualWorkListByIndividualPhase()
        {
            if (!IsInFunctionList(Constant.FunctionID.ActualWorkList))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var currentUser = GetLoginUser();
            var model = new PMS06002ListViewModel();

            model.GroupList = this.commonService.GetUserGroupSelectList(currentUser.CompanyCode);
            model.Condition.CompanyCode = currentUser.CompanyCode;
            model.Condition.GroupId = currentUser.GroupId;
            model.BranchList = this.commonService.GetBranchSelectList(currentUser.CompanyCode);
            model.ContractTypeList = this.commonService.GetContractTypeSelectList(GetLoginUser().CompanyCode);

            if (Session[Constant.SESSION_TRANSITION_DESTINATION].ToString().Contains("/PMS06001/ActualWork") && Session[Constant.SESSION_IS_BACK] != null)
            {
                var tmpCondition = GetRestoreData() as Condition;

                if (tmpCondition != null)
                {
                    model.Condition = tmpCondition;
                }
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("ActualListByIndividualPhase", model);
        }

        /// <summary>
        /// Action return the ActualWorkListByUser to display
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="sortColumn">sort column</param>
        /// <param name="sortType">sort type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>Json Actual work list</returns>
        public ActionResult SearchActualWorkListByIndividualPhase(DataTablesModel model, Condition condition, int sortColumn, string sortType, string TAB_ID)
        {
            if (ModelState.IsValid)
            {
                var currentUser = GetLoginUser();
                condition.CompanyCode = currentUser.CompanyCode;

                //if (currentUser.PlanFunctionList.Contains(Constant.FunctionID.CheckInputStatus) && currentUser.FunctionList.Contains(Constant.FunctionID.CheckInputStatus))
                //{
                //    condition.Filterable = 1;
                //}
                //else
                //{
                //    condition.Filterable = 0;
                //}
                var phaseByContracts = JsonConvert.DeserializeObject<List<PhaseByContract>>(condition.PHASE_BY_CONTRACT);
                var pageInfo = this.service.GetActualWorkListByIndividualPhase(model, condition, phaseByContracts, null);
                var results = this.service.GetWorkListByIndividualPhaseExport(condition, phaseByContracts, sortColumn, sortType);
                Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] = Utility.SerializeDynamicToJson(results);

                IList<object> dataList = new List<object>();

                int countMonth = ((condition.EndMonth.Year - condition.StartMonth.Year) * 12) + condition.EndMonth.Month - condition.StartMonth.Month;

                foreach (var data in pageInfo.Items)
                {
                    IList<string> dataItem = new List<string>();
                    var tempDictionary = (IDictionary<string, object>)data;
                    for (int i = 9; i <= (countMonth + 9); i++)
                    {
                        if (Convert.ToInt32(tempDictionary.Keys.ElementAt(i).ToString().Replace("/", "")) > Convert.ToInt32(Utility.GetCurrentDateTime().ToString("yyyyMM")))
                        {
                            var tempDateString = (tempDictionary.Values.ElementAt(i) ?? "").ToString();
                            if (tempDateString.EndsWith("(0)"))
                            {
                                tempDateString = tempDateString.Remove(tempDateString.Length - 3);
                                tempDateString += "(1)";
                            }

                            tempDictionary[tempDictionary.Keys.ElementAt(i).ToString()] = tempDateString;
                        }
                        if (tempDictionary.Values.ElementAt(i) == null || string.IsNullOrWhiteSpace(tempDictionary.Values.ElementAt(i).ToString()))
                        {
                            tempDictionary[tempDictionary.Keys.ElementAt(i).ToString()] = "0.00(0)(1)";
                        }
                        dataItem.Add(this.FormatData(tempDictionary.ToList()[i].Value, isPhaseList: true));
                    }
                    dataList.Add(dataItem);
                }

                var result =
                    Json(data: new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = pageInfo.TotalItems,
                        iTotalDisplayRecords = pageInfo.TotalItems,
                        aaData =
                                    (from t in pageInfo.Items
                                     select new object[]
                                        {
                                            ((IDictionary<string, object>)t).ToList()[3].Value,
                                            ((IDictionary<string, object>)t).ToList()[5].Value,
                                            ((IDictionary<string, object>)t).ToList()[0].Value,
                                            this.EncodeData(((IDictionary<string, object>)t).ToList()[1].Value),
                                            this.EncodeData(((IDictionary<string, object>)t).ToList()[2].Value),
                                            this.EncodeData(((IDictionary<string, object>)t).ToList()[4].Value),
                                            this.EncodeData(((IDictionary<string, object>)t).ToList()[6].Value),
                                            this.EncodeData(((IDictionary<string, object>)t).ToList()[7].Value),
                                            this.EncodeData(((IDictionary<string, object>)t).ToList()[8].Value),
                                        }
                                    ).ToList(),
                        datalist = dataList
                    });

                // Save search condition
                SaveRestoreData(condition);

                return result;

            }
            return new EmptyResult();
        }

        /// <summary>
        /// Export Csv Actual List
        /// </summary>
        /// <param name="start_date">start_date</param>
        /// <param name="end_date">end_date</param>
        /// <param name="user_name">user_name</param>
        /// <param name="group_id">group_id</param>
        /// <param name="eff_type">eff_type</param>
        /// <param name="sort_colum">sort_colum</param>
        /// <param name="sort_type">sort_type</param>
        /// <param name="TAB_ID">Tab id</param>
        /// <returns>CSV File</returns>
        public ActionResult ExportCsvActualListByIndividualPhase(DateTime start_date, DateTime end_date, string user_name, int? group_id, string search_locationID, string project_name, string search_contractTypeId, string search_phaseId, string phase_by_contract, string eff_type, bool deleteFlag, bool retirementFlag, int sort_colum, string sort_type, string TAB_ID)
        {
            Condition listCondition = new Condition()
            {
                CompanyCode = GetLoginUser().CompanyCode,
                StartMonth = start_date,
                EndMonth = end_date,
                GroupId = group_id,
                DisplayName = user_name,
                WorkTimeUnit = eff_type,
                DELETED_INCLUDE = deleteFlag,
                RETIREMENT_INCLUDE = retirementFlag,
                LOCATION_ID = search_locationID,
                PROJECT_NAME = project_name,
                CONTRACT_TYPE_ID = search_contractTypeId,
                PHASE_ID = search_phaseId,
                PHASE_BY_CONTRACT = phase_by_contract
            };

            IList<string> jsonResults = new List<string>();
            IList<dynamic> results = new List<dynamic>();

            if (Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] != null)
            {
                jsonResults = Session[Constant.SESSION_SEARCH_RESULT + TAB_ID] as IList<string>;
                results = Utility.DeserializeJsonToDynamic(jsonResults);
            }
            else
            {
                var phaseByContracts = JsonConvert.DeserializeObject<List<PhaseByContract>>(listCondition.PHASE_BY_CONTRACT);
                results = this.service.GetWorkListByIndividualPhaseExport(listCondition, phaseByContracts, sort_colum, sort_type);
            }

            List<string> fixColumns = new List<string>() {
                "No.",
                "所属",
                "ユーザー名",
                "拠点",
                "プロジェクト名",
                "契約種別",
                "フェーズ"
            };

            List<string> listMonth = Utility.getListMonth(listCondition.StartMonth.ToString("yyyy/MM"), listCondition.EndMonth.ToString("yyyy/MM"));
            string[] columns = fixColumns.Concat(listMonth).ToArray();

            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
            foreach (var t in results)
            {
                list.Add(t);
            }
            // Convert to Datatable
            DataTable dataTable = new DataTable(list.ToString());

            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(columns[i]);
            }

            foreach (IDictionary<string, object> item in list)
            {
                var values = new object[item.Values.Count];
                for (int i = 0; i < item.Values.Count; i++)
                {
                    if (i <= 6)
                    {
                        values[i] = item.Values.ElementAt(i);
                    }
                    else if (i >= 7)
                    {
                        values[i] = this.FormatData(item.Values.ElementAt(i), true, true);
                    }
                }
                dataTable.Rows.Add(values);
            }

            // Export Table to CSV file
            Utility.ExportToCsvData(this, dataTable, "ActualWorkListByIndividualPhase_" + Utility.GetCurrentDateTime().ToString("yyyyMMdd") + ".csv");
            return new EmptyResult();
        }

        /// <summary>
        /// Get list phase by contract type
        /// </summary>
        /// <param name="contractTypeId">Contract type id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetListPhaseByContractType(string contractTypeId)
        {
            if (Request.IsAjaxRequest())
            {
                string[] contractTypeIdArr = null;
                if (!string.IsNullOrEmpty(contractTypeId))
                {
                    contractTypeIdArr = contractTypeId.Split(',');
                }

                var phaseList = this.service.GetPhaseList(GetLoginUser().CompanyCode, contractTypeIdArr).ToList();

                var result = Json(
                   phaseList,
                   JsonRequestBehavior.AllowGet);

                return result;

            }
            return new EmptyResult();
        }
        #endregion

        #region Private members
        /// <summary>
        /// Get the list of Attendance Type in the form of SelectListItems
        /// </summary>
        /// <returns>List attendance type</returns>
        private IList<SelectListItem> GetAttendanceTypeList()
        {
            return this.service.GetAttendanceTypeList(GetLoginUser().CompanyCode)
                .Select(
                f => new SelectListItem
                {
                    Text = f.display_name,
                    Value = f.attendance_type_id.ToString()
                }

                    ).ToList();
        }

        /// <summary>
        /// Function process file CSV to Datatable
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <returns>DataTable</returns>
        private static DataTable ProcessCSV(string fileName)
        {
            //Set up our variables
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();
            DataRow row;
            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //Set the filename in to our stream
            StreamReader sr = new StreamReader(fileName, Encoding.Default, true);
            try
            {
                //Read the first line and split the string at , with our regular expression in to an array
                line = sr.ReadLine();
                strArray = r.Split(line);

                //For each item in the new split array, dynamically builds our Data columns. Save us having to worry about it.
                Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn(s)));

                // 打刻データ保持用のカラム追加
                dt.Columns.Add("clock_in_start_time");
                dt.Columns.Add("clock_in_end_time");

                //Read each line in the CVS file until it’s empty
                while ((line = sr.ReadLine()) != null)
                {
                    row = dt.NewRow();

                    //add our current value to our data row
                    string[] tmpArr = r.Split(line);
                    if (tmpArr.Length > 8)
                    {
                        throw new Exception();
                    }
                    tmpArr[3] = string.IsNullOrEmpty(tmpArr[3]) ? "00:00" : tmpArr[3];
                    tmpArr[4] = string.IsNullOrEmpty(tmpArr[4]) ? "24:00" : tmpArr[4];
                    row.ItemArray = tmpArr;
                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Tidy Streameader up
                sr.Dispose();
            }

            //return a the new DataTable
            return dt;

        }

        /// <summary>
        /// Function covert data form Datatable to Json
        /// </summary>
        /// <param name="dataTable">dataTable</param>
        /// <returns>List</returns>
        private List<Dictionary<String, Object>> ConvertDataTableTojSonString(DataTable dataTable)
        {
            List<Dictionary<String, Object>> tableRows = new List<Dictionary<String, Object>>();

            Dictionary<String, Object> row;

            foreach (DataRow dr in dataTable.Rows)
            {
                row = new Dictionary<String, Object>();
                int count = 1;
                foreach (DataColumn col in dataTable.Columns)
                {
                    row.Add(count.ToString(), dr[col]);
                    count++;
                }
                tableRows.Add(row);
            }
            return tableRows;
        }

        /// <summary>
        /// Check file CSV is imported.
        /// </summary>
        /// <param name="employee_no">employee_no</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult CheckImportCSV(string employee_no, int month, int year)
        {
            string errMess = string.Empty;
            if (Request.Files.Count == 0)
            {
                errMess = String.Format(Messages.E042);
                return Json(errMess);
            }

            DataTable dt = new DataTable();
            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    //Workout our file path
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = "~/App_Data/CSV";
                    if (!Directory.Exists(Server.MapPath(filePath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(filePath));
                    }
                    string path = Path.Combine(Server.MapPath(filePath), fileName);
                    file.SaveAs(path);
                    try
                    {
                        dt = ProcessCSV(path);
                    }
                    catch
                    {
                        errMess = String.Format(Messages.E047);
                        return Json(errMess);
                    }
                    finally
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
                catch
                {
                    errMess = String.Format(Messages.E070);
                    return Json(errMess);
                }
            }

            DateTime tempDate;

            if (dt.Rows.Count == 0)
            {
                errMess = String.Format(Messages.E042);
            }
            else
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                    if (dtRow[0].ToString() == "")
                    {
                        errMess = String.Format(Messages.E036, dt.Columns[0].ToString());
                        break;
                    }
                    if (dtRow[1].ToString() == "")
                    {
                        errMess = String.Format(Messages.E036, dt.Columns[1].ToString());
                        break;
                    }
                    if (dtRow[2].ToString() == "")
                    {
                        errMess = String.Format(Messages.E036, dt.Columns[2].ToString());
                        break;
                    }
                    else if (dtRow[0].ToString() != employee_no)
                    {
                        errMess = String.Format(Messages.E033);
                        break;
                    }
                    else if (!DateTime.TryParse(dtRow[2].ToString(), out tempDate))
                    {
                        errMess = String.Format(Messages.E038, dt.Columns[2].ToString());
                        break;
                    }
                    else if (month != DateTime.Parse(dtRow[2].ToString()).Month || year != DateTime.Parse(dtRow[2].ToString()).Year)
                    {
                        errMess = String.Format(Messages.E038, dt.Columns[2].ToString());
                        break;
                    }
                    else if (!checkTimeInportCSV(dtRow[3].ToString()))
                    {
                        errMess = String.Format(Messages.E038, dt.Columns[3].ToString());
                        break;
                    }
                    else if (!checkTimeInportCSV(dtRow[4].ToString()))
                    {
                        errMess = String.Format(Messages.E038, dt.Columns[4].ToString());
                        break;
                    }
                    else if (!checkStartTimeEndTime(dtRow[3].ToString(), dtRow[4].ToString()))
                    {
                        errMess = String.Format(Messages.E039, dt.Columns[4].ToString(), dt.Columns[3].ToString());
                        break;
                    }
                    else if (checkDuplicateDate(dtRow[2].ToString(), dt))
                    {
                        errMess = String.Format(Messages.E040, dt.Columns[2].ToString());
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(errMess))
            {
                dt = RoundDateTime(dt);
                return Json(ConvertDataTableTojSonString(dt));
            }
            else
            {
                return Json(errMess);
            }
        }

        /// <summary>
        /// Function import CSV in Actual work detail new
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <param name="dataListAttendanceRecord">dataListAttendanceRecord</param>
        /// <param name="regist_type">regist_type</param>
        /// <returns>Json update info</returns>
        [HttpPost]
        public JsonResult ImportCSVNew(int userId, int month, int year, IList<AttendanceRecord> dataListAttendanceRecord, string regist_type)
        {
            int user_Id = this.GetLoginUser().UserId;
            DateTime now = Utility.GetCurrentDateTime();
            MemberActualWork memberActualWork = new MemberActualWork();

            if (dataListAttendanceRecord != null)
            {
                foreach (var attendanceRecord in dataListAttendanceRecord)
                {
                    attendanceRecord.company_code = GetLoginUser().CompanyCode;
                    attendanceRecord.rest_time = 0;
                    attendanceRecord.attendance_type_id = null;
                    attendanceRecord.remarks = null;
                    attendanceRecord.ins_date = now;
                    attendanceRecord.ins_id = user_Id;
                    attendanceRecord.upd_date = now;
                    attendanceRecord.upd_id = user_Id;
                }
            }


            memberActualWork.company_code = GetLoginUser().CompanyCode;
            memberActualWork.user_sys_id = userId;
            memberActualWork.actual_work_year = year;
            memberActualWork.actual_work_month = month;
            memberActualWork.total_actual_work = 0;
            memberActualWork.regist_type = "1";
            memberActualWork.fix_flg = "0";
            memberActualWork.ins_date = now;
            memberActualWork.ins_id = user_Id;
            memberActualWork.upd_date = now;
            memberActualWork.upd_id = user_Id;

            var result = service.PutActualWorkDetailList(null, memberActualWork, dataListAttendanceRecord);
            if (result)
            {
                return Json(new UpdateInfo()
                {
                    last_update_date = now.ToString(),
                    last_update_person = this.GetLoginUser().DisplayName
                });
            }
            else
            {
                return Json(Messages.E043);
            }
        }

        /// <summary>
        /// Function round datetime
        /// </summary>
        /// <param name="dt">dt</param>
        /// <returns>DataTable</returns>
        public DataTable RoundDateTime(DataTable dt)
        {
            int working_time_unit_minute = GetLoginUser().Working_time_unit_minute;
            foreach (DataRow dtRow in dt.Rows)
            {
                string startTime = dtRow[3].ToString();
                int startHour = Convert.ToInt32(startTime.Split(':')[0]);
                int startMinute = Convert.ToInt32(startTime.Split(':')[1]);
                int checkStartTime = startMinute % working_time_unit_minute == 0 ? startMinute : startMinute - (startMinute % working_time_unit_minute) + working_time_unit_minute;
                if (checkStartTime == 60)
                {
                    startMinute = 0;
                    startHour += 1;
                }
                else
                {
                    startMinute = checkStartTime;
                }
                string strStartHour = "";
                string strStartMinute = "";

                if (startHour < 10)
                {
                    strStartHour = "0" + startHour;
                }
                else
                {
                    strStartHour = startHour.ToString();
                }

                if (startMinute < 10)
                {
                    strStartMinute = "0" + startMinute;
                }
                else
                {
                    strStartMinute = startMinute.ToString();
                }

                // 補正前の打刻データを保持
                dtRow[8] = dtRow[3];

                dtRow[3] = strStartHour + ":" + strStartMinute;

                string endTime = dtRow[4].ToString();
                int endHour = Convert.ToInt32(endTime.Split(':')[0]);
                int endMinute = Convert.ToInt32(endTime.Split(':')[1]);
                int checkEndTime = endMinute - (endMinute % working_time_unit_minute);
                endMinute = checkEndTime;
                string strEndHour = "";
                string strEndMinute = "";

                if (endHour < 10)
                {
                    strEndHour = "0" + endHour;
                }
                else
                {
                    strEndHour = endHour.ToString();
                }

                if (endMinute < 10)
                {
                    strEndMinute = "0" + endMinute;
                }
                else
                {
                    strEndMinute = endMinute.ToString();
                }

                // 補正前の打刻データを保持
                dtRow[9] = dtRow[4];

                dtRow[4] = strEndHour + ":" + strEndMinute;
            }

            return dt;
        }

        /// <summary>
        /// Check time inport form CSV
        /// </summary>
        /// <param name="time">time</param>
        /// <returns>bool : true/false</returns>
        public bool checkTimeInportCSV(string time)
        {
            decimal hour = 0;
            decimal minute = 0;
            bool check = true;

            try
            {
                hour = Convert.ToInt32(time.Split(':')[0]);
                minute = Convert.ToInt32(time.Split(':')[1]);
            }
            catch
            {
                check = false;
            }

            if (hour > 99 || hour < 0)
            {
                check = false;
            }
            if (minute > 59 || minute < 0)
            {
                check = false;
            }

            return check;
        }

        /// <summary>
        /// Check and format start time and end time.
        /// </summary>
        /// <param name="startTime">startTime</param>
        /// <param name="endTime">endTime</param>
        /// <returns>bool: true/false</returns>
        public bool checkStartTimeEndTime(string startTime, string endTime)
        {
            bool check = true;
            decimal start = Convert.ToInt32(startTime.Split(':')[0]) + (decimal)Convert.ToInt32(startTime.Split(':')[1]) / 60;
            decimal end = Convert.ToInt32(endTime.Split(':')[0]) + (decimal)Convert.ToInt32(endTime.Split(':')[1]) / 60;
            if (start > end)
            {
                check = false;
            }

            return check;
        }

        /// <summary>
        /// Function check duplicate date
        /// </summary>
        /// <param name="date">date</param>
        /// <param name="dt">dt</param>
        /// <returns>bool: true/false</returns>
        public bool checkDuplicateDate(string date, DataTable dt)
        {
            int check = 0;
            DateTime tempDate;

            foreach (DataRow dtRow in dt.Rows)
            {
                DateTime.TryParse(dtRow[2].ToString(), out tempDate);
                if (tempDate == DateTime.Parse(date))
                {
                    check += 1;
                }
            }
            return check > 1;
        }

        /// <summary>
        /// Encode to display label
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>string encode</returns>
        private string EncodeData(object data)
        {
            var s = (string)data;
            return HttpUtility.HtmlEncode(s);
        }

        /// <summary>
        /// Format data to display on Actual work list
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="isExport">isExport</param>
        /// <returns>string format code</returns>
        private string FormatData(object data, bool isPhaseList = false, bool isExport = false)
        {
            if (data == null)
            {
                return isPhaseList ? "0.00" : "0.00/0.00";
            }
            else
            {
                var sb = new StringBuilder();
                var index = data.ToString().IndexOf("(");
                string check = data.ToString().Substring(index);
                if (isPhaseList)
                {
                    var dec_plan = decimal.Parse(data.ToString().Substring(0, index), NumberStyles.Any);
                    if (isExport)
                    {
                        return dec_plan.ToString("#,##0.00");
                    }
                    else
                    {
                        return dec_plan.ToString("#,##0.00") + check;
                    }
                }
                else
                {
                    int id = data.ToString().IndexOf('/');
                    var dec_plan = decimal.Parse(data.ToString().Substring(0, id), NumberStyles.Any);
                    var dec_actual = decimal.Parse(data.ToString().Substring(id + 1, index - id - 1), NumberStyles.Any);
                    if (isExport)
                    {
                        return dec_plan.ToString("#,##0.00") + "/" + dec_actual.ToString("#,##0.00");
                    }
                    else
                    {
                        return dec_plan.ToString("#,##0.00") + "/" + dec_actual.ToString("#,##0.00") + check;
                    }

                }

            }
        }

        #endregion
    }
}
