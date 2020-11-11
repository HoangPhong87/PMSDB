#region License
/// <copyright file="PMS08001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/20</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.Models.PMS08001;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Information screen service class
    /// </summary>
    public class PMS08001Service : IPMS08001Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS08001Repository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS08001Service()
            : this(new PMS08001Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS08001Service(IPMS08001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get Information List
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of Information</returns>
        public IList<Information> GetInformationList(string cCode)
        {
            return this._repository.GetInformationList(cCode);
        }

        /// <summary>
        /// GetCheckDateStart
        /// </summary>
        /// <param name="checkDayofWeek"></param>
        /// <param name="todayofWeek"></param>
        /// <returns></returns>
        public string GetCheckDateStart(int checkDayofWeek, int todayofWeek)
        {
            return this._repository.GetCheckDateStart(checkDayofWeek, todayofWeek);
        }

        /// <summary>
        /// GetCheckDateEnd
        /// </summary>
        /// <param name="checkDayofWeek"></param>
        /// <param name="todayofWeek"></param>
        /// <returns></returns>
        public string GetCheckDateEnd(int checkDayofWeek, int todayofWeek)
        {
            return this._repository.GetCheckDateEnd(checkDayofWeek, todayofWeek);
        }

        /// <summary>
        /// GetCheckPointWeek
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public int GetCheckPointWeek(string cCode)
        {
            return this._repository.GetCheckPointWeek(cCode);
        }

        /// <summary>
        /// GetTodayWeek
        /// </summary>
        /// <returns></returns>
        public int GetTodayWeek()
        {
            return this._repository.GetTodayWeek();
        }

        /// <summary>
        /// CheckPlanList
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IList<ProjectInfor> CheckPlanList(string cCode)
        {
            return this._repository.CheckPlanList(cCode);
        }

        /// <summary>
        /// CheckPeriodList
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IList<ProjectInfor> CheckPeriodList(string cCode)
        {
            return this._repository.CheckPeriodList(cCode);
        }

        /// <summary>
        /// CheckSalesList
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IList<ProjectInfor> CheckSalesList(string cCode)
        {
            return this._repository.CheckSalesList(cCode);
        }

        /// <summary>
        /// CheckProgressList
        /// </summary>
        /// <param name="cCode"></param>
        /// <param name="progressUpdatedList"></param>
        /// <param name="checkDateStart"></param>
        /// <returns></returns>
        public IList<ProjectInfor> CheckProgressList(string cCode, string progressUpdatedList, string checkDateStart)
        {
            return this._repository.CheckProgressList(cCode, progressUpdatedList, checkDateStart);
        }

        /// <summary>
        /// GetProgressUpdatedProjectList
        /// </summary>
        /// <param name="cCode"></param>
        /// <param name="checkDateStart"></param>
        /// <param name="checkDateEnd"></param>
        /// <returns></returns>
        public IList<int> GetProgressUpdatedProjectList(string cCode, string checkDateStart, string checkDateEnd)
        {
            return this._repository.GetProgressUpdatedProjectList(cCode, checkDateStart, checkDateEnd);
        }

        /// <summary>
        /// GetActualWorkData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<MemberActualWorkDetail> GetActualWorkData(string companyCode)
        {
            return this._repository.GetActualWorkData(companyCode);
        }

        /// <summary>
        /// GetAttendanceData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<AttendanceRecordPlus> GetAttendanceData(string companyCode)
        {
            return this._repository.GetAttendanceData(companyCode);
        }

        /// <summary>
        /// GetDayList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<WorkingDay> GetDayList(string companyCode)
        {
            return this._repository.GetDayList(companyCode);
        }

        /// <summary>
        /// GetUserList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<UserInfor> GetUserList(string companyCode)
        {
            return this._repository.GetUserList(companyCode);
        }

        /// <summary>
        /// GetMemberAssignmentData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<MemberAssignmentDetail> GetMemberAssignmentData(string companyCode)
        {
            return this._repository.GetMemberAssignmentData(companyCode);
        }

        /// <summary>
        /// GetGroupList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<Group> GetGroupList(string companyCode)
        {
            return this._repository.GetGroupList(companyCode);
        }

        /// <summary>
        /// Get number of contract type for check plan of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetContractTypeCountForCheckPlan(string companyCode)
        {
            return this._repository.GetContractTypeCountForCheckPlan(companyCode);
        }

        /// <summary>
        /// Get number of contract type for check progress of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetContractTypeCountForCheckProgress(string companyCode)
        {
            return this._repository.GetContractTypeCountForCheckProgress(companyCode);
        }
        /// <summary>
        /// Get number of contract type for check period of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetContractTypeCountForCheckPeriod(string companyCode)
        {
            return this._repository.GetContractTypeCountForCheckPeriod(companyCode);
        }

        /// <summary>
        /// Get number of contract type for check sales of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetContractTypeCountForCheckSales(string companyCode)
        {
            return this._repository.GetContractTypeCountForCheckSales(companyCode);
        }
    }
}
