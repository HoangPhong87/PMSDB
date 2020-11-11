#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="IPMS06002Repository.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>02-06-2014</createdDate>
// //<summary>
// // TODO: Update summary.
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion
namespace ProjectManagementSystem.Models.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06002;

    public interface IPMS06002Repository
    {
        #region ActualWorkList
        /// <summary>
        /// Get the actual work of employees
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="userIds">userIds</param>
        /// <returns>Json list actual work list</returns>
        PageInfo<dynamic> GetActualWorkList(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, string userIds);

        /// <summary>
        /// Get list to export ActualWorkList
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <returns>List work export</returns>
        IList<dynamic> GetWorkListExport(Condition condition, int sort_column, string sort_type);
        #endregion

        #region ActualWorkDetail
        /// <summary>
        /// get detail total info
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns>Work Detail Info</returns>
        WorkDetailInfo GetDetailInfo(DetailCondition condition, string companyCode);

        /// <summary>
        /// get user_name, group_name
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>User Base Info</returns>
        UserBaseInfo GetUserBaseInfo(string userId);

        /// <summary>
        /// get update, insert information 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companyCode"></param>
        /// <returns>User Info</returns>
        UpdateInfo GetUpdateInfor(DetailCondition condition, string companyCode);

        /// <summary>
        ///  Get table work detail
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>Json Actual wordk detail</returns>
        PageInfo<UserActualWorkDetailPlus> GetWorkDetail(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, string companyCode, DetailCondition condition);

        /// <summary>
        /// get list work detail for export
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List Actual work detail</returns>
        IList<UserActualWorkDetail> GetWorkDetailExport(DetailCondition condition, string companyCode);
        #endregion

        #region ActualWorkregist
        /// <summary>
        /// Get user work info
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <returns>User Work Info</returns>
        UserWorkInfoPlus GetUserWorkInfo(int userId, int year, int month);

        /// <summary>
        /// Get the list of actual work detail of an user in a month
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="userId">userId</param>
        /// <param name="targetYear">targetYear</param>
        /// <param name="targetMonth">targetMonth</param>
        /// <returns>List Actual work detail</returns>
        IList<ActualWorkPlus> GetActualWorkDetail(int userId, int targetYear, int targetMonth);

        /// <summary>
        /// Get Attendance detail
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="targetYear">targetYear</param>
        /// <param name="targetMonth">targetMonth</param>
        /// <param name="dt">dt</param>
        /// <returns>List Work Attendance Detail</returns>
        IList<WorkAttendanceDetailPlus> GetWorkAttendanceDetail(int userId, int targetYear, int targetMonth, DataTable dt);

        /// <summary>
        /// Put the actual work detail list to database
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>bool : true/false</returns>
        bool PutActualWorkDetailList(
            IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            IList<AttendanceRecord> dataListAttendanceRecord);

        /// <summary>
        /// Get all Attendance Type
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of Attendance Type</returns>
        IEnumerable<AttendanceType> GetAttendanceTypeList(string cCode);

        /// <summary>
        /// Get list of weekly holiday
        /// </summary>
        /// <param name="company_code"></param>
        /// <returns>List weekly holiday</returns>
        IList<DayOfWeek> GetWeeklyHoliday(string company_code);

        /// <summary>
        /// Get list of special holiday
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>List Special holiday</returns>
        IList<DateTime> GetSpecialHoliday(string company_code);
        #endregion

        #region ActualWorkDetailNew
        /// <summary>
        /// Get list actual work detail
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_id">user_id</param>
        /// <param name="selected_year">selected_year</param>
        /// <param name="selected_month">selected_month</param>
        /// <returns>List actual work detail</returns>
        IList<dynamic> GetActualWorkDetailNew(string company_code, string user_id, int selected_year, int selected_month);

        /// <summary>
        /// Get user actual work detail
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>List actual work detail</returns>
        IList<UserActualWorkDetailPlus> GetUserActualWorkDetailPlus(DetailCondition condition, string companycode);

        /// <summary>
        /// Check valid data input
        /// </summary>
        /// <param name="dataMemberActualWork">dataMemberActualWork</param>
        /// <returns>bool: true/false</returns>
        bool CheckDataValid(MemberActualWork dataMemberActualWork);

        /// <summary>
        /// Get work clossing date
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>number of records is get</returns>
        int GetWorkClosingDate(string company_code);

        /// <summary>
        /// Get list Attendance info
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_sys_id">user_sys_id</param>
        /// <param name="FromDate">FromDate</param>
        /// <param name="ToDate">ToDate</param>
        /// <returns>List Attendance info</returns>
        IList<AttendanceDetail> GetAttendanceInfor(string company_code, int user_sys_id, DateTime FromDate, DateTime ToDate);

        /// <summary>
        /// get list actual work exist in month
        /// </summary>
        /// <param name="user_sys_id"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="company_code"></param>
        /// <returns></returns>
        List<int> ActualWorkDateList(int user_sys_id, int year, int month, string company_code);

        #endregion

        #region ActualWorkRegistNew
        /// <summary>
        /// Get information for regist new screen
        /// </summary>
        /// <param name="day">day</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <param name="userId">userId</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>Attendance Record</returns>
        AttendanceRecord GetAttendanceRecordInfor(int day, int month, int year, int userId, string companyCode);

        /// <summary>
        /// Get list of member actual work
        /// </summary>
        /// <param name="day">day</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <param name="userId">userId</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List Member Actual Work</returns>
        IList<MemberActualWorkListPlus> GetMemberActualWorkList(int day, int month, int year, int userId, string companyCode);

        /// <summary>
        /// Get regist type
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_id">user_id</param>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <returns>Regist Type</returns>
        string GetRegistType(string company_code, int user_id, int year, int month);

        /// <summary>
        /// Get Adjustment Time
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="attendance_type_id">attendance_type_id</param>
        /// <returns>Adjustment Time</returns>
        decimal GetAdjustmentTime(string company_code, int attendance_type_id);

        /// <summary>
        /// Put the actual work detail list to database
        /// </summary>
        /// <param name="dataListMemberActualWorkDetail"></param>
        /// <param name="dataMemberActualWork"></param>
        /// <param name="dataAttendanceRecord"></param>
        /// <param name="isExceedMaximumActualWorkTime">out param</param>
        /// <returns>bool</returns>
        bool PutActualWorkDetailListNew(IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            AttendanceRecord dataAttendanceRecord, out bool isExceedMaximumActualWorkTime);

        /// <summary>
        /// Get Total Actual Work Time
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="userId"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        decimal GetTotalActualWorkTime(int month, int year, int userId, string companyCode);
        #endregion

        #region ActualWorkListByIndividualPhase
        /// <summary>
        /// Get the actual work by individual phase
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="phaseByContracts">List Phase ID</param>
        /// <param name="userIds">userIds</param>
        /// <returns>Json list actual work list</returns>
        PageInfo<dynamic> GetActualWorkListByIndividualPhase(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, List<PhaseByContract> phaseByContracts, string userIds);

        /// <summary>
        /// Get info of work list by individual phase
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="phaseByContracts">List Phase ID</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <returns>List info of work</returns>
        IList<dynamic> GetWorkListByIndividualPhaseExport(Condition condition, List<PhaseByContract> phaseByContracts, int sort_column, string sort_type);

        /// <summary>
        /// Get phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeId">Contract type ID</param>
        /// <returns>List of phase</returns>
        IList<PhasePlus> GetPhaseList(string companyCode, string[] contractTypeId);
        #endregion
    }
}