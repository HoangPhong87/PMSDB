#region License
/// <copyright file="IPMS09002Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09002;
using ProjectManagementSystem.ViewModels;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS09002Service
    {
        /// <summary>
        /// Get individual sales list
        /// </summary>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>List of individual sales</returns>
        IList<dynamic> GetIndividualSalesList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            Condition condition);

        /// <summary>
        /// Get actual sales list
        /// </summary>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>List of actual sales</returns>
        IList<dynamic> GetActualSalesList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            Condition condition);

        /// <summary>
        /// Get Sales detail list by personal
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="userID">User ID</param>
        /// <param name="targetYear">Target year</param>
        /// <param name="targetMonth">Target month</param>
        /// <returns>List of sales detail by personal</returns>
        PageInfo<SalesDetailByPersonal> GetSalesDetailByPersonal(DataTablesModel model, SalesDetailByPersonalCondition condition);

        /// <summary>
        /// Get user name
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="userID">User ID</param>
        /// <returns>User name</returns>
        string GetUserName(string companyCode, int userID);
    }
}
