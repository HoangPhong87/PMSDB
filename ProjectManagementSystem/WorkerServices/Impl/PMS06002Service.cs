#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="PMS06002Service.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>02-06-2014</createdDate>
// //<summary>
// // Service class for PMS06002
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion

namespace ProjectManagementSystem.WorkerServices.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Transactions;
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06002;
    using ProjectManagementSystem.Models.Repositories;
    using ProjectManagementSystem.Models.Repositories.Impl;
    using ProjectManagementSystem.ViewModels;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Service implemetation class for the PMS06002 controller
    /// </summary>
    public class PMS06002Service : IPMS06002Service
    {
        #region Constructor
        private readonly IPMS06002Repository repository;
        /// <summary>
        /// Default constructor
        /// </summary>
        public PMS06002Service()
            : this(new PMS06002Repository())
        {
        }
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="repository"></param>
        public PMS06002Service(IPMS06002Repository repository)
        {
            this.repository = repository;
        }
        #endregion

        #region ActualWorkList
        /// <summary>
        /// get actual work list
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <returns>Get List Actual work</returns>
        public PageInfo<dynamic> GetActualWorkList(DataTablesModel model, Condition condition, string userIds)
        {
            return repository.GetActualWorkList(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                condition,
                userIds);
        }

        /// <summary>
        /// Get Work List for Export
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <returns>List Work</returns>
        public IList<dynamic> GetWorkListExport(Condition condition, int sort_column, string sort_type)
        {
            return repository.GetWorkListExport(condition, sort_column, sort_type);
        }
        #endregion

        #region ActualWorkDetail

        /// <summary>
        /// Get Actual work detail info
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns>Work Detail</returns>
        public WorkDetailInfo GetDetailInfo(DetailCondition condition, string companyCode)
        {
            return repository.GetDetailInfo(condition, companyCode);
        }

        /// <summary>
        /// Get user base info
        /// </summary>
        /// <param name="UserId">UserId</param>
        /// <returns>User Info</returns>
        public UserBaseInfo GetUserBaseInfo(string UserId)
        {
            return repository.GetUserBaseInfo(UserId);
        }
        /// <summary>
        /// get update, insert information 
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>Update info</returns>
        public UpdateInfo GetUpdateInfor(DetailCondition condition, string companyCode)
        {
            return repository.GetUpdateInfor(condition, companyCode);
        }

        /// <summary>
        /// get work detail
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>List User Actual Work Detail</returns>
        public PageInfo<UserActualWorkDetailPlus> GetWorkDetail(DataTablesModel model, DetailCondition condition, string companycode)
        {
            var pageInfo = this.repository.GetWorkDetail(
               model.iDisplayStart,
               model.iDisplayLength,
               model.sColumns,
               model.iSortCol_0,
               model.sSortDir_0,
               companycode,
               condition);

            return pageInfo;
        }

        /// <summary>
        /// get work detail for export
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List work detail</returns>
        public IList<UserActualWorkDetail> GetWorkDetailExport(DetailCondition condition, string companyCode)
        {
            return repository.GetWorkDetailExport(condition, companyCode);
        }

        /// <summary>
        /// get list actual work exist in month
        /// </summary>
        /// <param name="user_sys_id"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="company_code"></param>
        /// <returns></returns>
        public List<int> ActualWorkDateList(int user_sys_id, int year, int month, string company_code)
        {
            return repository.ActualWorkDateList(user_sys_id, year, month, company_code);
        }

        #endregion

        #region ActualWorkRegist
        /// <summary>
        /// Retrieving the general user work information
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <returns>User Work Info</returns>
        public UserWorkInfoPlus GetUserWorkInfo(int userId, int year, int month)
        {
            return repository.GetUserWorkInfo(userId, year, month);
        }

        /// <summary>
        /// Get the list of actual work time of an user in a month
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="targetYear">targetYear</param>
        /// <param name="targetMonth">targetMonth</param>
        /// <returns>List Actual work Detail</returns>
        public IList<ActualWorkPlus> GetActualWorkDetail(int userId, int targetYear, int targetMonth)
        {
            return repository.GetActualWorkDetail(userId, targetYear, targetMonth);
        }

        /// <summary>
        /// Get the list of actual work time of an user in a month
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="targetYear">targetYear</param>
        /// <param name="targetMonth">targetMonth</param>
        /// <returns>List Work Attendance Detail</returns>
        public IList<WorkAttendanceDetailPlus> GetWorkAttendanceDetail(int userId, int targetYear, int targetMonth, DataTable dt)
        {
            return repository.GetWorkAttendanceDetail(userId, targetYear, targetMonth, dt);
        }

        /// <summary>
        /// Put the Actual work detail list to database
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>bool : true/false</returns>
        public bool PutActualWorkDetailList(
            IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            IList<AttendanceRecord> dataListAttendanceRecord)
        {
            var result = false;
            using (var trans = new TransactionScope())
            {
                result = repository.PutActualWorkDetailList(dataListMemberActualWorkDetail, dataMemberActualWork, dataListAttendanceRecord);
                if (result)
                    trans.Complete();
            }
            return result;
        }

        /// <summary>
        /// Put the actual work detail list to database
        /// </summary>
        /// <param name="dataListMemberActualWorkDetail"></param>
        /// <param name="dataMemberActualWork"></param>
        /// <param name="dataAttendanceRecord"></param>
        /// <param name="isExceedMaximumActualWorkTime">out param</param>
        /// <returns>bool</returns>
        public bool PutActualWorkDetailListNew(IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            AttendanceRecord dataAttendanceRecord, out bool isExceedMaximumActualWorkTime)
        {
            var result = false;
            using (var trans = new TransactionScope())
            {
                result = repository.PutActualWorkDetailListNew(dataListMemberActualWorkDetail, dataMemberActualWork, dataAttendanceRecord, out isExceedMaximumActualWorkTime);
                if (result)
                    trans.Complete();
            }
            return result;
        }

        /// <summary>
        /// Get all Attendance Type
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of Attendance Type</returns>
        public IEnumerable<AttendanceType> GetAttendanceTypeList(string cCode)
        {
            return this.repository.GetAttendanceTypeList(cCode);
        }

        /// <summary>
        /// Get Holiday information
        /// </summary>
        /// <param name="company_code"></param>
        /// <returns></returns>
        public HolidayInfo GetHolidayInfo(string company_code)
        {
            return new HolidayInfo()
            {
                weekly_holiday = repository.GetWeeklyHoliday(company_code),
                special_holiday = repository.GetSpecialHoliday(company_code)
            };

        }
        #endregion

        #region ActualWorkRegistNew
        /// <summary>
        /// Get regist type
        /// </summary>
        /// <param name="company_code"></param>
        /// <param name="user_id"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public string GetRegistType(string company_code, int user_id, int year, int month)
        {
            return repository.GetRegistType(company_code, user_id, year, month);
        }

        /// <summary>
        /// Get information for regist new screen
        /// </summary>
        /// <param name="day">day</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <param name="userId">userId</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns></returns>
        public AttendanceRecord GetAttendanceRecordInfor(int day, int month, int year, int userId, string companyCode)
        {
            return repository.GetAttendanceRecordInfor(day, month, year, userId, companyCode);
        }

        /// <summary>
        /// Get list of member actual work
        /// </summary>
        /// <param name="day">day</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <param name="userId">userId</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>Get member actual work list</returns>
        public IList<MemberActualWorkListPlus> GetMemberActualWorkList(int day, int month, int year, int userId, string companyCode)
        {
            return repository.GetMemberActualWorkList(day, month, year, userId, companyCode);
        }
        #endregion

        #region ActualWorkDetailNew
        /// <summary>
        /// Get list actual work detail new
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="company_code">company_code</param>
        /// <returns>list actual work detail new</returns>
        public IList<dynamic> GetActualWorkDetailNew(DetailCondition condition, string company_code)
        {
            return repository.GetActualWorkDetailNew(company_code, condition.UserId, condition.SelectedYear, condition.SelectedMonth);
        }

        /// <summary>
        /// Get List User Actual Work Detail
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>List User Actual Work Detail</returns>
        public IList<UserActualWorkDetailPlus> GetUserActualWorkDetailPlus(DetailCondition condition, string companycode)
        {
            return repository.GetUserActualWorkDetailPlus(condition, companycode);
        }

        /// <summary>
        /// Check Data valid
        /// </summary>
        /// <param name="dataMemberActualWork">dataMemberActualWork</param>
        /// <returns>bool : true/false</returns>
        public bool CheckDataValid(MemberActualWork dataMemberActualWork)
        {
            return repository.CheckDataValid(dataMemberActualWork);
        }

        /// <summary>
        /// Get work closing date
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>record</returns>
        public int GetWorkClosingDate(string company_code)
        {
            return repository.GetWorkClosingDate(company_code);
        }

        /// <summary>
        /// Get List Attendance Detail
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_sys_id">user_sys_id</param>
        /// <param name="FromDate">FromDate</param>
        /// <param name="ToDate">ToDate</param>
        /// <returns>List Attendance Detail</returns>
        public IList<AttendanceDetail> GetAttendanceInfor(string company_code, int user_sys_id, DateTime FromDate, DateTime ToDate)
        {
            return repository.GetAttendanceInfor(company_code, user_sys_id, FromDate, ToDate);
        }
        #endregion

        #region ActualWorkListByIndividualPhase
        /// <summary>
        /// get actual work list by induvidual phase
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="phaseByContracts">List phase ID</param>
        /// <param name="userIds">userIds</param>
        /// <returns>Get List Actual work</returns>
        public PageInfo<dynamic> GetActualWorkListByIndividualPhase(DataTablesModel model, Condition condition, List<PhaseByContract> phaseByContracts, string userIds)
        {
            return repository.GetActualWorkListByIndividualPhase(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                condition,
                phaseByContracts,
                userIds);
        }

        /// <summary>
        /// Get Work List by individual for Export
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="phaseByContracts">List phase ID</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <returns>List Work</returns>
        public IList<dynamic> GetWorkListByIndividualPhaseExport(Condition condition, List<PhaseByContract> phaseByContracts, int sort_column, string sort_type)
        {
            return repository.GetWorkListByIndividualPhaseExport(condition, phaseByContracts, sort_column, sort_type);
        }

        /// <summary>
        /// Get phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of phase</returns>
        public IList<PhasePlus> GetPhaseList(string companyCode, string[] contractTypeID)
        {
            //var phaseList = this.repository.GetPhaseList(companyCode, contractTypeID);

            //var listContract = phaseList.Select(x => new { x.contract_type_id, x.contract_type }).Distinct();

            //var result = new List<PhaseInContractType>();
            //foreach (var item in listContract)
            //{
            //    var contractType = new PhaseInContractType { contract_type_id = item.contract_type_id, contract_type = item.contract_type };
            //    var listPhaseOfContract = phaseList.Where(x => x.contract_type_id == item.contract_type_id)
            //                                       .Select(e => new KeyValuePair<int, string>(e.phase_id, e.phase_name)).ToDictionary(k => k.Key, k => k.Value);
            //    contractType.list_phase = listPhaseOfContract;
            //    result.Add(contractType);
            //}

            //return result;
            return this.repository.GetPhaseList(companyCode, contractTypeID);
        }
        #endregion

        /// <summary>
        /// Get Adjustment Time
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="attendance_type_id">attendance_type_id</param>
        /// <returns>Adjustment Time</returns>
        public decimal GetAdjustmentTime(string company_code, int attendance_type_id)
        {
            return repository.GetAdjustmentTime(company_code, attendance_type_id);
        }
    }
}