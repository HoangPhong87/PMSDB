#region License
/// <copyright file="IPMS09001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09001;
using ProjectManagementSystem.ViewModels;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS09001Service
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
        /// Get Sales Group Detail Summary
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        string GetGroupName(int group_id);

        PageInfo<ProjectSalesInfo> GetListSalesGroupDetail(DataTablesModel model, string companyCode, SalesGroupDetailCondition condition);

        /// <summary>
        /// Get List Sales Project Detail
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sort_colum"></param>
        /// <param name="sort_type"></param>
        /// <returns></returns>
        PageInfo<PersonalSalesInfo> GetListSalesProjectDetail(DataTablesModel model, SalesProjectDetailCondition condition);

        /// <summary>
        ///  Get project detail basic info
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        SalesProjectDetailBasicInfo GetProjectDetailBasicInfo(int group_id, int project_id);
    }
}
