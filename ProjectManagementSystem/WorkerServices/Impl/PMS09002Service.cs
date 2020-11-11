#region License
/// <copyright file="PMS09002Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09002;
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
    public class PMS09002Service : IPMS09002Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS09002Repository _repository;

        /// <summary>
        /// Contractor
        /// </summary>
        public PMS09002Service()
            : this(new PMS09002Repository())
        {
        }

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS09002Service(IPMS09002Repository _repository)
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
        /// Get Sales detail list by personal
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns>List of sales detail by personal</returns>
        public PageInfo<SalesDetailByPersonal> GetSalesDetailByPersonal(DataTablesModel model, SalesDetailByPersonalCondition condition)
        {
            var pageInfo = this._repository.GetSalesDetailByPersonal(
                 model.iDisplayStart,
                 model.iDisplayLength,
                 model.sColumns,
                 model.iSortCol_0,
                 model.sSortDir_0,
                 condition);

            return pageInfo;
        }

        /// <summary>
        /// Get user name
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="userID">User ID</param>
        /// <returns>User name</returns>
        public string GetUserName(string companyCode, int userID)
        {
            return this._repository.GetUserName(companyCode, userID);
        }
    }
}
