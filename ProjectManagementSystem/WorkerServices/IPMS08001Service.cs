#region License
/// <copyright file="IPMS08001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/20</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS08001;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Information screen service interface class
    /// </summary>
    public interface IPMS08001Service
    {
        /// <summary>
        /// Get Information List
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of Information</returns>
        IList<Information> GetInformationList(string cCode);

        /// <summary>
        /// Get DateStart of Week Check
        /// </summary>
        /// <param name="checkDayofWeek"></param>
        /// <param name="todayofWeek"></param>
        /// <returns></returns>
        string GetCheckDateStart(int checkDayofWeek, int todayofWeek);

        /// <summary>
        /// Get DateEnd of Week Check
        /// </summary>
        /// <param name="checkDayofWeek"></param>
        /// <param name="todayofWeek"></param>
        /// <returns></returns>
        string GetCheckDateEnd(int checkDayofWeek, int todayofWeek);

        /// <summary>
        /// Get Check Point Week
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        int GetCheckPointWeek(string cCode);

        /// <summary>
        /// Get Today
        /// </summary>
        /// <returns></returns>
        int GetTodayWeek();

        /// <summary>
        /// Get list of Plan to check
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        IList<ProjectInfor> CheckPlanList(string cCode);

        /// <summary>
        /// Get list of Period to check
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        IList<ProjectInfor> CheckPeriodList(string cCode);

        /// <summary>
        /// Get list of Sales to check
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        IList<ProjectInfor> CheckSalesList(string cCode);

        /// <summary>
        /// Get list of Progress to check
        /// </summary>
        /// <param name="cCode"></param>
        /// <param name="progressUpdatedList"></param>
        /// <param name="checkDateStart"></param>
        /// <returns></returns>
        IList<ProjectInfor> CheckProgressList(string cCode, string progressUpdatedList, string checkDateStart);

        /// <summary>
        /// Get list of progress updated projects
        /// </summary>
        /// <param name="cCode"></param>
        /// <param name="checkDateStart"></param>
        /// <param name="checkDateEnd"></param>
        /// <returns></returns>
        IList<int> GetProgressUpdatedProjectList(string cCode, string checkDateStart, string checkDateEnd);

        /// <summary>
        /// GetActualWorkData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<MemberActualWorkDetail> GetActualWorkData(string companyCode);

        /// <summary>
        /// GetAttendanceData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<AttendanceRecordPlus> GetAttendanceData(string companyCode);

        /// <summary>
        /// GetDayList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<WorkingDay> GetDayList(string companyCode);

        /// <summary>
        /// GetUserList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<UserInfor> GetUserList(string companyCode);

        /// <summary>
        /// GetMemberAssignmentData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<MemberAssignmentDetail> GetMemberAssignmentData(string companyCode);

        /// <summary>
        /// GetGroupList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<Group> GetGroupList(string companyCode);

        /// <summary>
        /// Get number of contract type for check plan of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetContractTypeCountForCheckPlan(string companyCode);

        /// <summary>
        /// Get number of contract type for check progress of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetContractTypeCountForCheckProgress(string companyCode);

        /// <summary>
        /// Get number of contract type for check period of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetContractTypeCountForCheckPeriod(string companyCode);

        /// <summary>
        /// Get number of contract type for check sales of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetContractTypeCountForCheckSales(string companyCode);
    }
}
