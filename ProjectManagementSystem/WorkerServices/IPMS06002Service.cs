#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="IPMS06002Service.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>02-06-2014</createdDate>
// //<summary>
// // Service interface for PMS06002 service.
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion

namespace ProjectManagementSystem.WorkerServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06002;
    using ProjectManagementSystem.ViewModels;
    using System.Web.Mvc;

    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS06002Service
    {
        #region ActualWorkList
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        PageInfo<dynamic> GetActualWorkList(DataTablesModel model, Condition condition, string userIds);
        /// <summary>
        /// Get Work List for Export
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sort_column"></param>
        /// <param name="sort_type"></param>
        /// <returns></returns>
        IList<dynamic>GetWorkListExport(Condition condition, int sort_column, string sort_type);
        #endregion

        #region ActualWorkDetail
        /// <summary>
        /// get info total
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        WorkDetailInfo GetDetailInfo(DetailCondition condition, string companyCode);
        /// <summary>
        /// get user_name, group_name
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        UserBaseInfo GetUserBaseInfo(string UserId);
        /// <summary>
        /// get update, insert information 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        UpdateInfo GetUpdateInfor(DetailCondition condition, string companyCode);
        /// <summary>
        /// Get table work detail
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        PageInfo<UserActualWorkDetailPlus> GetWorkDetail(DataTablesModel model, DetailCondition condition, string companycode);
        /// <summary>
        /// get work detail for export
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<UserActualWorkDetail> GetWorkDetailExport(DetailCondition condition, string companyCode);
        #endregion

        #region ActualWorkRegist
        /// <summary>
        /// Get the general user work information
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        UserWorkInfoPlus GetUserWorkInfo(int userId, int year, int month);
        /// <summary>
        /// Get the actual work detail of an user in a month
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetYear"></param>
        /// <param name="targetMonth"></param>
        /// <returns></returns>
        IList<ActualWorkPlus> GetActualWorkDetail(int userId, int targetYear, int targetMonth);
        /// <summary>
        /// Get Attendance detail
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetYear"></param>
        /// <param name="targetMonth"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        IList<WorkAttendanceDetailPlus> GetWorkAttendanceDetail(int userId, int targetYear, int targetMonth, DataTable dt);
        /// <summary>
        /// Put the actual work detail list to database
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool PutActualWorkDetailList(
            IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            IList<AttendanceRecord> dataListAttendanceRecord);
        /// <summary>
        /// get list actual work exist in month
        /// </summary>
        /// <param name="user_sys_id"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="company_code"></param>
        /// <returns></returns>
        List<int> ActualWorkDateList(int user_sys_id, int year, int month, string company_code);

        /// <summary>
        /// Get all Attendance Type
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of Attendance Type</returns>
        IEnumerable<AttendanceType> GetAttendanceTypeList(string cCode);
        /// <summary>
        /// Get list of holiday
        /// </summary>
        /// <param name="company_code"></param>
        /// <returns></returns>
        HolidayInfo GetHolidayInfo(string company_code);
        #endregion

        #region ActualWorkDetailNew
        IList<dynamic> GetActualWorkDetailNew(DetailCondition condition, string company_code);
        IList<UserActualWorkDetailPlus> GetUserActualWorkDetailPlus(DetailCondition condition, string companycode);
        bool CheckDataValid(MemberActualWork dataMemberActualWork);
        int GetWorkClosingDate(string company_code);
        IList<AttendanceDetail> GetAttendanceInfor(string company_code, int user_sys_id, DateTime FromDate, DateTime ToDate);

        #endregion

        #region ActualWorkRegistNew
        /// <summary>
        /// Get information for regist new screen
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="userId"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        AttendanceRecord GetAttendanceRecordInfor(int day, int month, int year, int userId, string companyCode);
        /// <summary>
        /// Get list of member actual work
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="userId"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<MemberActualWorkListPlus> GetMemberActualWorkList(int day, int month, int year, int userId, string companyCode);
        /// <summary>
        /// Get regist type
        /// </summary>
        /// <param name="company_code"></param>
        /// <param name="user_id"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        string GetRegistType(string company_code, int user_id, int year, int month);

        /// <summary>
        /// Get Adjustment Time
        /// </summary>
        /// <param name="company_code"></param>
        /// <param name="attendance_type_id"></param>
        /// <returns></returns>
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
        #endregion

        #region ActualWorkListByIndividualPhase
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <param name="phaseByContracts">List phase ID</param>
        /// <param name="userIds">userIds</param>
        /// <returns></returns>
        PageInfo<dynamic> GetActualWorkListByIndividualPhase(DataTablesModel model, Condition condition, List<PhaseByContract> phaseByContracts, string userIds);

        /// <summary>
        /// Get Work List by individual for Export
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="phaseByContracts">List phase ID</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <returns>List Work</returns>
        IList<dynamic> GetWorkListByIndividualPhaseExport(Condition condition, List<PhaseByContract> phaseByContracts, int sort_column, string sort_type);

        /// <summary>
        /// Get phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of phase</returns>
        IList<PhasePlus> GetPhaseList(string companyCode, string[] contractTypeID);
        #endregion
    }

}