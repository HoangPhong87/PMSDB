#region License
/// <copyright file="IPMS06001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS06001;
using ProjectManagementSystem.ViewModels;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS06001Service
    {
        /// <summary>
        /// Search project by condition
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns>Search result</returns>
        IList<ProjectInfoPlus> Search(DataTablesModel model, string companyCode, Condition condition);

        /// <summary>
        /// Search all project by condition
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        IList<ProjectInfoPlus> SearchAll(string companyCode, Condition condition);

        /// <summary>
        /// Export Project List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Project list</returns>
        IList<ProjectInfoPlus> ExportProjectListToCSV(string companyCode, Condition condition, string orderBy, string orderType);

        /// <summary>
        /// Get a project info
        /// </summary>
        /// <param name="companyCode">company code</param>
        /// <param name="projectID">project code</param>
        /// <param name="isToCopy">Check is action copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>Project info</returns>
        ProjectInfoPlus GetProjectInfo(string companyCode, int projectID, bool isToCopy, int copyType);

        /// <summary>
        ///  Edit a project info
        /// </summary>
        /// <param name="data">Project info</param>
        /// <param name="phaseList">Target phase list</param>
        /// <param name="targetCategoryList">Target category list</param>
        /// <param name="outsourcerList">Outsourcer list</param>
        /// <param name="subcontractorList">Subcontractor list</param>
        /// <param name="paymentDetailList">Payment detail list</param>
        /// <param name="overheadCostList">Overhead cost list</param>
        /// <param name="overheadCostDetailList">Overhead cost detail list</param>
        /// <param name="memberAssignmentList">Member assignment list</param>
        /// <param name="memberAssignmentDetailList">Member assignment detail list</param>
        /// <param name="progressHistoryList">Progress history list</param>
        /// <param name="fileList">Attach file list</param>
        /// <param name="allowRegistHistory">Allow regist history</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="error">Error message</param>
        /// <returns>True/False</returns>
        bool EditProjectInfo(ProjectInfoPlus data,
            IList<PhasePlus> phaseList,
            IList<TargetCategoryPlus> targetCategoryList,
            IList<SalesPaymentPlus> outsourcerList,
            IList<SalesPaymentPlus> subcontractorList,
            IList<SalesPaymentDetailPlus> paymentDetailList,
            IList<OverheadCostPlus> overheadCostList,
            IList<OverheadCostDetailPlus> overheadCostDetailList,
            IList<MemberAssignmentPlus> memberAssignmentList,
            IList<MemberAssignmentDetailPlus> memberAssignmentDetailList,
            IList<ProgressHistoryPlus> progressHistoryList,
            IList<ProjectAttachFilePlus> fileList,
            bool allowRegistHistory,
            out int projectID,
            out string error);

        /// <summary>
        /// Get all contract type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="isCreateCopy">Is function copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>List of contract type</returns>
        IList<ContractType> GetContractTypeList(string companyCode, int projectID, bool isCreateCopy, int copyType);

        /// <summary>
        /// Get all phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type code</param>
        /// <returns>List of phase</returns>
        IList<PhasePlus> GetPhaseList(string companyCode, int contractTypeID);

        /// <summary>
        /// Get target phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of target phase</returns>
        IList<TargetPhasePlus> GetTargetPhaseList(string companyCode, int projectID);

        /// <summary>
        /// Get default tax rate
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="fromDate">Project from date</param>
        /// <returns>Default tax rate</returns>
        ConsumptionTax GetDefaultTaxRate(string companyCode, DateTime fromDate);

        /// <summary>
        /// Get Customer
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <param name="orderingflag">Orderingflag</param>
        /// <returns>List of Customer</returns>
        IList<SalesPaymentPlus> GetCustomerList(string companyCode, int projectID, string orderingflag);

        /// <summary>
        /// Get overhead cost list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of overhead cost</returns>
        IList<OverheadCostPlus> GetOverheadCostList(string companyCode, int projectID);

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="isCreateCopy">Is function copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>List of category</returns>
        IList<Category> GetCategoryList(string companyCode, int projectID, bool isCreateCopy, int copyType);

        /// <summary>
        /// Get all sub category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="categoryID">Category ID</param>
        /// <returns>List of sub category</returns>
        IList<SubCategory> GetSubCategoryList(string companyCode, int projectID, int categoryID);

        /// <summary>
        /// Get all target category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of target category</returns>
        IList<TargetCategoryPlus> GetTargetCategoryList(string companyCode, int projectID);

        /// <summary>
        /// Get category list by contract type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeId">Contract type ID</param>
        /// <returns>List of category</returns>
        IList<int> GetDefaultCategoryListByContract(string companyCode, int contractTypeId);

        /// <summary>
        /// Get all phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of progress</returns>
        IList<ProgressHistoryPlus> GetProgressList(string companyCode, int projectID);

        /// <summary>
        /// Get all attach file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of attach file</returns>
        IList<ProjectAttachFilePlus> GetFileList(string companyCode, int projectID);

        /// <summary>
        /// Get all overhead cost type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of overhead cost type</returns>
        IList<OverheadCostType> GetOverheadCostTypeList(string companyCode);

        /// <summary>
        /// Get work time of member by month year
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectDurationArr">Project duration list</param>
        /// <param name="userID">User ID</param>
        /// <returns>Member work time by month year</returns>
        IList<MemberWorkTime> GetMemberWorkTimeByMonthYear(string companyCode, List<string> projectDurationArr, int userID);

        /// <summary>
        /// Get Sale Payment list
        /// </summary>
        /// <param name="startDate">Start date of Project</param>
        /// <param name="endDate">End date of Project</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <param name="orderingflag">Orderingflag</param>
        /// <returns>List of Sale Payment</returns>
        IList<dynamic> GetSalePaymentList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            int projectID,
            string orderingflag);

        /// <summary>
        /// Get member assignment list
        /// </summary>
        /// <param name="startDate">Start date of Project</param>
        /// <param name="endDate">End date of Project</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of member assignment</returns>
        IList<dynamic> GetMemberAssignmentList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            int projectID);

        /// <summary>
        /// Get overhead cost detail list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of overhead cost detail</returns>
        IList<OverheadCostDetailPlus> GetOverheadCostDetailList(string companyCode, int projectID);

        /// <summary>
        /// Get effort member in project detail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<dynamic> GetProjectMemberDetail(
            DataTablesModel model,
            string companyCode,
            DetailCondition condition);

        /// <summary>
        /// Get profit of member in project detail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<dynamic> GetProjectMemberProfitDetail(
            DataTablesModel model,
            string companyCode,
            DetailCondition condition);

        /// <summary>
        /// Get work day of month
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="month">Month</param>
        /// <param name="year">Year</param>
        /// <returns>Work day of month</returns>
        IList<int> GetWorkDayOfMonth(string companyCode, int fromYear, int fromMonth, int toYear, int toMonth);

        /// <summary>
        /// Get member actual workday
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <param name="fromYear"></param>
        /// <param name="fromMonth"></param>
        /// <param name="toYear"></param>
        /// <param name="toMonth"></param>
        /// <returns></returns>
        IList<MemberActualWorkDetail> GetMemberActualWorkDay(string companyCode, int projectID, int userID, int fromYear, int fromMonth, int toYear, int toMonth);

        /// <summary>
        /// Get actual work time of member by userID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="userID">User ID</param>
        /// <returns>Actual work time of member</returns>
        MemberActualWorkDetail GetActualWorkTimeByUser(string companyCode, int projectID, int userID);

        /// <summary>
        /// Get actual work time of member by phase ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="phaseID">Phase ID</param>
        /// <returns>Actual work time of member</returns>
        MemberActualWorkDetail GetActualWorkTimeByPhase(string companyCode, int projectID, int phaseID);

        /// <summary>
        /// Get actual work time of member by month year
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="timeArr">Time array to check</param>
        /// <returns>Actual work time by month year</returns>
        MemberActualWorkDetail GetActualWorkTimeByMonthYear(string companyCode, int projectID, List<string> timeArr);

        /// <summary>
        /// Get all history of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of project history</returns>
        IList<ProjectInfoHistory> GetHistoryOfProject(string companyCode, int projectID);

        /// <summary>
        /// Get history of project info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>History of project info</returns>
        ProjectInfoHistoryPlus GetProjectInfoHistory(string companyCode, int projectID, int historyID);

        /// <summary>
        /// Get target time of project info history
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of target time</returns>
        IList<string> GetTargetTimeHistory(string companyCode, int projectID, int historyID);

        /// <summary>
        /// Get history of member assignment info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of member assignment info history</returns>
        IList<MemberAssignmentHistoryPlus> GetMemberAssignmentHistory(string companyCode,
            int projectID,
            int historyID);

        /// <summary>
        /// Get history of member assignment detail info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of member assignment detail info history</returns>
        IList<MemberAssignmentDetailHistoryPlus> GetMemberAssignmentDetailHistory(string companyCode,
            int projectID,
            int historyID);

        /// <summary>
        /// Get history of payment info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of payment info history</returns>
        IList<SalesPaymentHistoryPlus> GetPaymentHistory(string companyCode,
            int projectID,
            int historyID);

        /// <summary>
        /// Get history of payment detail info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of payment detail info history</returns>
        IList<PaymentDetailHistoryPlus> GetPaymentDetailHistory(string companyCode,
            int projectID,
            int historyID);

        /// <summary>
        /// Get history of overhead cost
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of overhead cost history</returns>
        IList<OverheadCostHistoryPlus> GetOverheadCostHistory(string companyCode,
            int projectID,
            int historyID);

        /// <summary>
        /// Get history of overhead cost detail
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of overhead cost detail history</returns>
        IList<OverheadCostDetailHistoryPlus> GetOverheadCostDetailHistory(string companyCode,
            int projectID,
            int historyID);

        /// Get Project Plan Information by Project ID and Company Code
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <param name="companyCode">Company code</param>
        /// <returns>Project Plan Information</returns>
        ProjectPlanInfoPlus GetProjectPlanInfo(int projectID, string companyCode);

        /// <summary>
        /// function insert/update project plan information
        /// </summary>
        /// <param name="data">ProjectPlanInfo</param>
        /// <returns>number of record is update/insert</returns>
        int EditProjectPlanData(ProjectPlanInfo data);

        /// <summary>
        /// function delete project information
        /// </summary>
        /// <param name="dataListProjectId">list project id</param>
        /// <returns></returns>
        bool DeleteProject(IList<string> dataListProjectId);

        /// <summary>
        /// Update project status by ID
        /// </summary>
        /// <param name="dataListProjectId"></param>
        /// <param name="statusID"></param>
        /// <param name="updateUserID"></param>
        /// <returns></returns>
        bool UpdateStatusProject(IList<string> dataListProjectId, int statusID, int updateUserID);

        /// <summary>
        /// Get data_editable_time from company setting to delete progress
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetDataEditTableTime(string companyCode);

        /// <summary>
        /// Get actual worktime in project by phase
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        IList<TargetPhasePlus> GetWorkTimeByPhase(string companyCode, int projectID, string timeUnit);

        /// <summary>
        /// Get actual work time in project by phase list
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<dynamic> GetUserWorkTimeByPhase(DataTablesModel model, string companyCode, ActualWorkCondition condition);

        /// <summary>
        /// Get user actual work time by phase in project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ProjectInfoPlus GetActualWorkDetailInfo(string companyCode, int projectID, int userID);

        /// <summary>
        /// Get actual work time by Project ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>Actual work time</returns>
        decimal GetActualWorkTimeByProjectID(string companyCode, int projectID);

        /// <summary>
        /// Get operation target flag
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="statusID"></param>
        /// <returns></returns>
        string GetOperationTargetFlag(string companyCode, int statusID);

        /// <summary>
        /// Get plan cost from history
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        IList<MemberAssignmentDetailHistoryPlus> GetPlanCostHistory(string companyCode, int projectID);

    }
}
