#region License
/// <copyright file="PMS08001Controller.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/20</createdDate>
#endregion

namespace ProjectManagementSystem.Controllers
{
    using ProjectManagementSystem.ViewModels.PMS08001;
    using ProjectManagementSystem.WorkerServices;
    using ProjectManagementSystem.WorkerServices.Impl;
    using System.Web.Mvc;
    using ProjectManagementSystem.Common;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Collections.Generic;
    using Models.PMS08001;
    using Models.Entities;
    using System.Linq;
    using System.Diagnostics;

    public class PMS08001Controller : ControllerBase
    {
        private readonly IPMS08001Service _service;

        private readonly IPMSCommonService _commonService;

        private PMS08001ListViewModel _viewModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS08001Controller()
            : this(new PMS08001Service(), new PMSCommonService())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service worker</param>
        public PMS08001Controller(IPMS08001Service service, IPMSCommonService commonService)
        {
            this._service = service;
            this._commonService = commonService;
            this._viewModel = new PMS08001ListViewModel();
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>List View</returns>
        public ActionResult Index()
        {
            if (!IsInFunctionList(Constant.FunctionID.Top))
            {
                return this.RedirectToAction("Index", "ErrorAuthent");
            }

            var currentUser = GetLoginUser();

            if (currentUser.PlanFunctionList.Contains(Constant.FunctionID.CheckOperationInputStatus) && currentUser.FunctionList.Contains(Constant.FunctionID.CheckOperationInputStatus))
            {
                LoadGroupData();
            }

            if (currentUser.PlanFunctionList.Contains(Constant.FunctionID.CheckProjectStatus) && currentUser.FunctionList.Contains(Constant.FunctionID.CheckProjectStatus))
            {
              LoadProjectData();
            }

            if (Session[Constant.SESSION_IS_BACK] != null)
            {
                Session[Constant.SESSION_IS_BACK] = null;
            }

            return this.View("List", _viewModel);
        }

        /// <summary>
        /// Get data information
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadData()
        {
            string companyCode = GetLoginUser().CompanyCode;
            var list = this._service.GetInformationList(companyCode);
            var infoList = new List<dynamic>();


            foreach (var data in list)
            {
                infoList.Add(new
                {
                    publish_start_date = data.publish_start_date.ToString("yyyy/MM/dd"),
                    content = ConvertUrlsToLinks(HttpUtility.HtmlEncode(data.content))
                });
            }

            JsonResult result = Json(
                infoList,
                JsonRequestBehavior.AllowGet);
            return result;
        }

        /// <summary>
        /// Convert url to link
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string ConvertUrlsToLinks(string url)
        {
            string regex = @"((www\.|(http|https)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);
            return r.Replace(url, "<a href=\"$1\" title=\"$1\" target=\"&#95;blank\">$1</a>").Replace("href=\"www", "href=\"http://www");
        }

        /// <summary>
        /// Load all data of checked project
        /// </summary>
        private void LoadProjectData()
        {
            string companyCode = GetLoginUser().CompanyCode;
            if (this._service.GetContractTypeCountForCheckPlan(companyCode) > 0)
            {
                _viewModel.PLAN_LIST_VISIBLE = true;
            }

            if (this._service.GetContractTypeCountForCheckProgress(companyCode) > 0)
            {
                _viewModel.PROGRESS_LIST_VISIBLE = true;
            }

            if (this._service.GetContractTypeCountForCheckPeriod(companyCode) > 0)
            {
                _viewModel.PERIOD_LIST_VISIBLE = true;
            }

            if (this._service.GetContractTypeCountForCheckSales(companyCode) > 0)
            {
                _viewModel.SALES_LIST_VISIBLE = true;
            }

            _viewModel.PLAN_LIST = this._service.CheckPlanList(companyCode);
            _viewModel.PERIOD_LIST = this._service.CheckPeriodList(companyCode);
            _viewModel.SALES_LIST = this._service.CheckSalesList(companyCode);
            
            int checkPointWeek = this._service.GetCheckPointWeek(companyCode);
            int checkDayofWeek = checkPointWeek == 0 ? 8 : checkPointWeek + 1;
            int todayofWeek = this._service.GetTodayWeek() == 1 ? 8 : this._service.GetTodayWeek();

            string value = "";
            switch (checkPointWeek)
            {
                case 0:
                    value += "毎週日曜";
                    break;
                case 1:
                    value += "毎週月曜";
                    break;
                case 2:
                    value += "毎週火曜";
                    break;
                case 3:
                    value += "毎週水曜";
                    break;
                case 4:
                    value += "毎週木曜";
                    break;
                case 5:
                    value += "毎週金曜";
                    break;
                case 6:
                    value += "毎週土曜";
                    break;
            }
            ViewBag.CheckDayofWeek = value;

            string checkDateStart = this._service.GetCheckDateStart(checkDayofWeek, todayofWeek);
            string checkDateEnd = this._service.GetCheckDateEnd(checkDayofWeek, todayofWeek);
            var progressUpdatedPrjList = this._service.GetProgressUpdatedProjectList(companyCode, checkDateStart, checkDateEnd);

            string strPrjList = string.Empty;
            if (progressUpdatedPrjList.Count > 0)
            {
                foreach (int projectId in progressUpdatedPrjList)
                {
                    strPrjList += projectId.ToString() + ",";
                }

                strPrjList = strPrjList.Substring(0, strPrjList.Length - 1);
                strPrjList = "(" + strPrjList + ")";
            }
            _viewModel.PROGRESS_LIST = this._service.CheckProgressList(companyCode, strPrjList, checkDateStart);
        }

        /// <summary>
        /// Load all data of operation actual
        /// </summary>
        private void LoadGroupData()
        {
            string companyCode = GetLoginUser().CompanyCode;
            var groupList = _service.GetGroupList(companyCode);
            var actualWorkData = _service.GetActualWorkData(companyCode);
            var attendanceData = _service.GetAttendanceData(companyCode);
            var dayList = _service.GetDayList(companyCode);
            var userList = _service.GetUserList(companyCode);
            var assignmentData = _service.GetMemberAssignmentData(companyCode);
            var checkedUserList = new List<UserInfor>();

            foreach (var userInfor in userList)
            {
                int userId = userInfor.USER_ID;
                int checkedDayCount = 0;

                foreach (var day in dayList)
                {
                    string non_operational_flg = attendanceData.Where(i => (i.user_sys_id == userId && i.actual_work_year == day.target_year && i.actual_work_month == day.target_month && i.actual_work_date == day.target_date))
                        .Select(s => s.non_operational_flg).SingleOrDefault();
                    bool isOnVacationDay = (non_operational_flg == "1");
                    var actualWorkTimeList = actualWorkData.Where(i => (i.user_sys_id == userId && i.actual_work_year == day.target_year && i.actual_work_month == day.target_month && i.actual_work_date == day.target_date))
                        .Select(s => s.actual_work_time);
                    decimal actual_work_time = 0;
                    foreach (var actualWorkTime in actualWorkTimeList)
                    {
                        actual_work_time += actualWorkTime;
                    }
                    int assignedProjectCount = assignmentData.Where(i => (i.user_sys_id == userId && i.target_year == day.target_year && i.target_month == day.target_month))
                        .Select(s => s.project_sys_id).ToList().Count;
                    bool haveAssignedProject = (assignedProjectCount > 0);
                    if (actual_work_time > 0 || isOnVacationDay || !haveAssignedProject)
                    {
                        checkedDayCount++;
                    }
                }

                if (checkedDayCount >= dayList.Count)
                {
                    checkedUserList.Add(userInfor);
                }
            }

            foreach( var checkedUser in checkedUserList)
            {
                userList.Remove(checkedUser);
            }
            _viewModel.GROUP_LIST = new List<GroupInfor>();

            foreach(Models.Entities.Group group in groupList)
            {
                var groupUserList = userList.Where(i => i.GROUP_ID == group.group_id.ToString()).Select(s => new UserInfor {
                    USER_ID = s.USER_ID,
                    USER_NAME = s.USER_NAME,
                    GROUP_ID = s.GROUP_ID,
                    SELECTED_MONTH = s.SELECTED_MONTH,
                    SELECTED_YEAR = s.SELECTED_YEAR
                }).ToList();

                var groupInfor = new GroupInfor()
                {
                    GROUP_NAME = group.group_name,
                    USER_LIST = groupUserList
                };
                _viewModel.GROUP_LIST.Add(groupInfor);
            }

            var noGroupUserList = userList.Where(i=> i.GROUP_ID == null).Select(s => new UserInfor
            {
                USER_ID = s.USER_ID,
                USER_NAME = s.USER_NAME,
                GROUP_ID = s.GROUP_ID,
                SELECTED_MONTH = s.SELECTED_MONTH,
                SELECTED_YEAR = s.SELECTED_YEAR
            }).ToList();

            var unsetGroupInfor = new GroupInfor()
            {
                GROUP_NAME = "所属無し",
                USER_LIST = noGroupUserList
            };

            _viewModel.GROUP_LIST.Add(unsetGroupInfor);
        }
    }
}
