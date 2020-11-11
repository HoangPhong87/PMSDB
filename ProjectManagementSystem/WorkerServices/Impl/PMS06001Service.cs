#region License
/// <copyright file="PMS06001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS06001;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Service class
    /// </summary>
    public class PMS06001Service : IPMS06001Service
    {
        /// <summary>
        /// interface
        /// </summary>
        private IPMS06001Repository _repository;

        /// <summary>
        /// contractor
        /// </summary>
        public PMS06001Service()
            : this(new PMS06001Repository())
        {
        }

        /// <summary>
        /// declare
        /// </summary>
        /// <param name="_repository"></param>
        public PMS06001Service(IPMS06001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Search project by condition
        /// </summary>
        /// <param name="model">Datatable model</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public IList<ProjectInfoPlus> Search(DataTablesModel model, string companyCode, Condition condition)
        {
            var pageInfo = this._repository.Search(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                companyCode,
                condition);
            return pageInfo;
        }

        /// <summary>
        /// Search all project by condition
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public IList<ProjectInfoPlus> SearchAll(string companyCode, Condition condition)
        {
            return this._repository.SearchAll(companyCode, condition);
        }

        /// <summary>
        /// Export Project List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Project list</returns>
        public IList<ProjectInfoPlus> ExportProjectListToCSV(string companyCode, Condition condition, string orderBy, string orderType)
        {
            return this._repository.ExportProjectListToCSV(companyCode, condition, orderBy, orderType);
        }

        /// <summary>
        /// Get a project info
        /// </summary>
        /// <param name="companyCode">company code</param>
        /// <param name="projectID">project code</param>
        /// <param name="isToCopy">Check is action copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>Project info</returns>
        public ProjectInfoPlus GetProjectInfo(string companyCode, int projectID, bool isToCopy, int copyType)
        {
            return this._repository.GetProjectInfo(companyCode, projectID, isToCopy, copyType);
        }

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
        public bool EditProjectInfo(ProjectInfoPlus data,
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
            out string error)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = this._repository.EditProjectInfo(
                    data,
                    phaseList,
                    targetCategoryList,
                    outsourcerList,
                    subcontractorList,
                    paymentDetailList,
                    overheadCostList,
                    overheadCostDetailList,
                    memberAssignmentList,
                    memberAssignmentDetailList,
                    progressHistoryList,
                    fileList,
                    allowRegistHistory,
                    out projectID,
                    out error);

                if (res)
                    transaction.Complete();
            }

            return res;
        }

        /// <summary>
        /// Get all contract type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="isCreateCopy">Is function copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>List of contract type</returns>
        public IList<ContractType> GetContractTypeList(string companyCode, int projectID, bool isCreateCopy, int copyType)
        {
            return this._repository.GetContractTypeList(companyCode, projectID, isCreateCopy, copyType);
        }

        /// <summary>
        /// Get all phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type code</param>
        /// <returns>List of phase</returns>
        public IList<PhasePlus> GetPhaseList(string companyCode, int contractTypeID)
        {
            return this._repository.GetPhaseList(companyCode, contractTypeID);
        }

        /// <summary>
        /// Get target phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of target phase</returns>
        public IList<TargetPhasePlus> GetTargetPhaseList(string companyCode, int projectID)
        {
            return this._repository.GetTargetPhaseList(companyCode, projectID);
        }

        /// <summary>
        /// Get default tax rate
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="fromDate">Project from date</param>
        /// <returns>Default tax rate</returns>
        public ConsumptionTax GetDefaultTaxRate(string companyCode, DateTime fromDate)
        {
            return this._repository.GetDefaultTaxRate(companyCode, fromDate);
        }

        /// <summary>
        /// Get Customer
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <param name="orderingflag">Orderingflag</param>
        /// <returns>List of Customer</returns>
        public IList<SalesPaymentPlus> GetCustomerList(string companyCode, int projectID, string orderingflag)
        {
            return this._repository.GetCustomerList(companyCode, projectID, orderingflag);
        }

        /// <summary>
        /// Get overhead cost list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of overhead cost</returns>
        public IList<OverheadCostPlus> GetOverheadCostList(string companyCode, int projectID)
        {
            return this._repository.GetOverheadCostList(companyCode, projectID);
        }

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="isCreateCopy">Is function copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>List of category</returns>
        public IList<Category> GetCategoryList(string companyCode, int projectID, bool isCreateCopy, int copyType)
        {
            return this._repository.GetCategoryList(companyCode, projectID, isCreateCopy, copyType);
        }

        /// <summary>
        /// Get all sub category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="categoryID">Category ID</param>
        /// <returns>List of sub category</returns>
        public IList<SubCategory> GetSubCategoryList(string companyCode, int projectID, int categoryID)
        {
            return this._repository.GetSubCategoryList(companyCode, projectID, categoryID);
        }

        /// <summary>
        /// Get all target category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of target category</returns>
        public IList<TargetCategoryPlus> GetTargetCategoryList(string companyCode, int projectID)
        {
            return this._repository.GetTargetCategoryList(companyCode, projectID);
        }

        /// <summary>
        /// Get category list by contract type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeId">Contract type ID</param>
        /// <returns>List of category</returns>
        public IList<int> GetDefaultCategoryListByContract(string companyCode, int contractTypeId)
        {
            return this._repository.GetDefaultCategoryListByContract(companyCode, contractTypeId).Select(e=> e.category_id.Value).ToList();
        }

        /// <summary>
        /// Get all phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of progress</returns>
        public IList<ProgressHistoryPlus> GetProgressList(string companyCode, int projectID)
        {
            return this._repository.GetProgressList(companyCode, projectID);
        }

        /// <summary>
        /// Get all attach file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of attach file</returns>
        public IList<ProjectAttachFilePlus> GetFileList(string companyCode, int projectID)
        {
            return this._repository.GetFileList(companyCode, projectID);
        }

        /// <summary>
        /// Get all overhead cost type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of overhead cost type</returns>
        public IList<OverheadCostType> GetOverheadCostTypeList(string companyCode)
        {
            return this._repository.GetOverheadCostTypeList(companyCode);
        }

        /// <summary>
        /// Get Sale Payment list
        /// </summary>
        /// <param name="startDate">Start date of Project</param>
        /// <param name="endDate">End date of Project</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <param name="orderingflag">Orderingflag</param>
        /// <returns>List of Sale Payment</returns>
        public IList<dynamic> GetSalePaymentList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            int projectID,
            string orderingflag)
        {
            return this._repository.GetSalePaymentList(startDate, endDate, companyCode, projectID, orderingflag);
        }

        /// <summary>
        /// Get member assignment list
        /// </summary>
        /// <param name="startDate">Start date of Project</param>
        /// <param name="endDate">End date of Project</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of member assignment</returns>
        public IList<dynamic> GetMemberAssignmentList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            int projectID)
        {
            return this._repository.GetMemberAssignmentList(startDate, endDate, companyCode, projectID);
        }

        /// <summary>
        /// Get overhead cost detail list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of overhead cost detail</returns>
        public IList<OverheadCostDetailPlus> GetOverheadCostDetailList(string companyCode, int projectID)
        {
            return this._repository.GetOverheadCostDetailList(companyCode, projectID);
        }

        /// <summary>
        /// Get effort member in project detail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<dynamic> GetProjectMemberDetail(
            DataTablesModel model,
            string companyCode,
            DetailCondition condition)
        {
            var pageInfo = this._repository.GetProjectMemberDetail(
               model.iDisplayStart,
               model.iDisplayLength,
               model.sColumns,
               model.iSortCol_0,
               model.sSortDir_0,
               companyCode,
               condition);
            return pageInfo;
        }

        /// <summary>
        /// Get profit member in project detail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<dynamic> GetProjectMemberProfitDetail(
            DataTablesModel model,
            string companyCode,
            DetailCondition condition)
        {
            var pageInfo = this._repository.GetProjectMemberProfitDetail(
               model.iDisplayStart,
               model.iDisplayLength,
               model.sColumns,
               model.iSortCol_0,
               model.sSortDir_0,
               companyCode,
               condition);
            return pageInfo;
        }

        /// <summary>
        /// Get work day of month
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="month">Month</param>
        /// <param name="year">Year</param>
        /// <returns>Work day of month</returns>
        public IList<int> GetWorkDayOfMonth(string companyCode, int fromYear, int fromMonth, int toYear, int toMonth)
        {
            return this._repository.GetWorkDayOfMonth(companyCode, fromYear, fromMonth, toYear, toMonth);
        }

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
        public IList<MemberActualWorkDetail> GetMemberActualWorkDay(string companyCode, int projectID, int userID, int fromYear, int fromMonth, int toYear, int toMonth)
        {
            return this._repository.GetMemberActualWorkDay(companyCode, projectID, userID, fromYear, fromMonth, toYear, toMonth);
        }

        /// <summary>
        /// Get actual work time of member by user
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="userID">User ID</param>
        /// <returns>Actual work time of member</returns>
        public MemberActualWorkDetail GetActualWorkTimeByUser(string companyCode, int projectID, int userID)
        {
            return this._repository.GetActualWorkTimeByUser(companyCode, projectID, userID);
        }

        /// <summary>
        /// Get actual work time of member by phase ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="phaseID">Phase ID</param>
        /// <returns>Actual work time of member</returns>
        public MemberActualWorkDetail GetActualWorkTimeByPhase(string companyCode, int projectID, int userID)
        {
            return this._repository.GetActualWorkTimeByPhase(companyCode, projectID, userID);
        }

        /// <summary>
        /// Get actual work time of member by month year
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="timeArr">Time array to check</param>
        /// <returns>Actual work time by month year</returns>
        public MemberActualWorkDetail GetActualWorkTimeByMonthYear(string companyCode, int projectID, List<string> timeArr)
        {
            return this._repository.GetActualWorkTimeByMonthYear(companyCode, projectID, timeArr);
        }

        /// <summary>
        /// Get work time of member by month year
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectDurationArr">Project duration list</param>
        /// <param name="userID">User ID</param>
        /// <returns>Member work time by month year</returns>
        public IList<MemberWorkTime> GetMemberWorkTimeByMonthYear(string companyCode, List<string> projectDurationArr, int userID)
        {
            return this._repository.GetMemberWorkTimeByMonthYear(companyCode, projectDurationArr, userID);
        }

        /// <summary>
        /// Get all history of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of project history</returns>
        public IList<ProjectInfoHistory> GetHistoryOfProject(string companyCode, int projectID)
        {
            return this._repository.GetHistoryOfProject(companyCode, projectID);
        }

        /// <summary>
        /// Get history of project info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>History of project info</returns>
        public ProjectInfoHistoryPlus GetProjectInfoHistory(string companyCode, int projectID, int historyID)
        {
            return this._repository.GetProjectInfoHistory(companyCode, projectID, historyID);
        }

        /// <summary>
        /// Get target time of project info history
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of target time</returns>
        public IList<string> GetTargetTimeHistory(string companyCode, int projectID, int historyID)
        {
            return this._repository.GetTargetTimeHistory(companyCode, projectID, historyID);
        }

        /// <summary>
        /// Get history of member assignment info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of member assignment info history</returns>
        public IList<MemberAssignmentHistoryPlus> GetMemberAssignmentHistory(string companyCode,
            int projectID,
            int historyID)
        {
            return this._repository.GetMemberAssignmentHistory(companyCode, projectID, historyID);
        }

        /// <summary>
        /// Get history of member assignment detail info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of member assignment detail info history</returns>
        public IList<MemberAssignmentDetailHistoryPlus> GetMemberAssignmentDetailHistory(string companyCode,
            int projectID,
            int historyID)
        {
            return this._repository.GetMemberAssignmentDetailHistory(companyCode, projectID, historyID);
        }

        /// <summary>
        /// Get history of payment info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of payment info history</returns>
        public IList<SalesPaymentHistoryPlus> GetPaymentHistory(string companyCode,
            int projectID,
            int historyID)
        {
            return this._repository.GetPaymentHistory(companyCode, projectID, historyID);
        }

        /// <summary>
        /// Get history of payment detail info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of payment detail info history</returns>
        public IList<PaymentDetailHistoryPlus> GetPaymentDetailHistory(string companyCode,
            int projectID,
            int historyID)
        {
            return this._repository.GetPaymentDetailHistory(companyCode, projectID, historyID);
        }

        /// <summary>
        /// Get history of overhead cost
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of overhead cost history</returns>
        public IList<OverheadCostHistoryPlus> GetOverheadCostHistory(string companyCode,
            int projectID,
            int historyID)
        {
            return this._repository.GetOverheadCostHistory(companyCode, projectID, historyID);
        }

        /// <summary>
        /// Get history of overhead cost detail
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of overhead cost detail history</returns>
        public IList<OverheadCostDetailHistoryPlus> GetOverheadCostDetailHistory(string companyCode,
            int projectID,
            int historyID)
        {
            return this._repository.GetOverheadCostDetailHistory(companyCode, projectID, historyID);
        }

        /// Get Project Plan Information by Project ID and Company Code
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <param name="companyCode">Company code</param>
        /// <returns>Project Plan Information</returns>
        public ProjectPlanInfoPlus GetProjectPlanInfo(int projectID, string companyCode)
        {
            return this._repository.GetProjectPlanInfo(projectID, companyCode);
        }

        /// <summary>
        /// function insert/update project plan information
        /// </summary>
        /// <param name="data">ProjectPlanInfo</param>
        /// <returns>number of record is update/insert</returns>
        public int EditProjectPlanData(ProjectPlanInfo data)
        {
            return this._repository.EditProjectPlanData(data);
        }

        /// <summary>
        /// function delete project information
        /// </summary>
        /// <param name="dataListProjectId">list project id</param>
        /// <returns></returns>
        public bool DeleteProject(IList<string> dataListProjectId)
        {
            return this._repository.DeleteProject(dataListProjectId);
        }

        /// <summary>
        /// Update project status by ID
        /// </summary>
        /// <param name="dataListProjectId"></param>
        /// <param name="statusID"></param>
        /// <param name="updateUserID"></param>
        /// <returns></returns>
        public bool UpdateStatusProject(IList<string> dataListProjectId, int statusID, int updateUserID)
        {
            return this._repository.UpdateStatusProject(dataListProjectId, statusID, updateUserID);
        }

        /// <summary>
        /// Get data_editable_time from company setting to delete progress
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetDataEditTableTime(string companyCode)
        {
            return this._repository.GetDataEditTableTime(companyCode);
        }

        /// <summary>
        /// Get actual worktime in project by phase
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public IList<TargetPhasePlus> GetWorkTimeByPhase(string companyCode, int projectID, string timeUnit)
        {
            return this._repository.GetWorkTimeByPhase(companyCode, projectID, timeUnit);
        }

        /// <summary>
        /// Get user actual work time by phase in project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ProjectInfoPlus GetActualWorkDetailInfo(string companyCode, int projectID, int userID)
        {
            return this._repository.GetActualWorkDetailInfo(companyCode, projectID, userID);
        }

        /// <summary>
        /// Get actual work time in project by phase list
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<dynamic> GetUserWorkTimeByPhase(DataTablesModel model, string companyCode, ActualWorkCondition condition)
        {
            return this._repository.GetUserWorkTimeByPhase(model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0, companyCode, condition);
        }

        /// <summary>
        /// Get actual work time by project ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>Actual work time of member</returns>
        public decimal GetActualWorkTimeByProjectID(string companyCode, int projectID)
        {
            return this._repository.GetActualWorkTimeByProjectID(companyCode, projectID);
        }

        /// <summary>
        /// Get operation target flag
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="statusID"></param>
        /// <returns></returns>
        public string GetOperationTargetFlag(string companyCode,int statusID)
        {
            return this._repository.GetOperationTargetFlag(companyCode,statusID);
        }

        /// <summary>
        /// Get plan cost from history
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public IList<MemberAssignmentDetailHistoryPlus> GetPlanCostHistory(string companyCode, int projectID)
        {
            return this._repository.GetPlanCostHistory(companyCode, projectID);
        }

    }
}