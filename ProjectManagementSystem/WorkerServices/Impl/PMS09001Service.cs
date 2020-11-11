#region License
/// <copyright file="PMS09001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09001;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Service class
    /// </summary>
    public class PMS09001Service : IPMS09001Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS09001Repository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09001Service()
            : this(new PMS09001Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS09001Service(IPMS09001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get individual sales list
        /// </summary>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>List of individual sales</returns>
        public IList<dynamic> GetIndividualSalesList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            Condition condition)
        {
            return this._repository.GetIndividualSalesList(startDate, endDate, companyCode, condition);
        }

        /// <summary>
        /// Get actual sales list
        /// </summary>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>List of actual sales</returns>
        public IList<dynamic> GetActualSalesList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            Condition condition)
        {
            return this._repository.GetActualSalesList(startDate, endDate, companyCode, condition);
        }

        /// <summary>
        /// Get Sales Group Detail Summary
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string GetGroupName(int group_id)
        {
            return this._repository.GetGroupName(group_id);
        }

        /// <summary>
        /// Sales list by group detail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<ProjectSalesInfo> GetListSalesGroupDetail(DataTablesModel model, string companyCode, SalesGroupDetailCondition condition)
        {
            var pageInfo = this._repository.GetListSalesGroupDetail(
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
        /// Sales list by project detail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<PersonalSalesInfo> GetListSalesProjectDetail(DataTablesModel model, SalesProjectDetailCondition condition)
        {
            var pageInfo = this._repository.GetListSalesProjectDetail(
                 model.iDisplayStart,
                 model.iDisplayLength,
                 model.sColumns,
                 model.iSortCol_0,
                 model.sSortDir_0,
                 condition);

            return pageInfo;
        }

        /// <summary>
        /// Get project detail basic info
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public SalesProjectDetailBasicInfo GetProjectDetailBasicInfo(int group_id, int project_id)
        {
            return this._repository.GetProjectDetailBasicInfo(group_id, project_id);
        }
    }
}
