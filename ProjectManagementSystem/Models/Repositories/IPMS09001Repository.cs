#region License
/// <copyright file="IPMS09001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09001;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Project screen repository interface class
    /// </summary>
    public interface IPMS09001Repository
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

        /// <summary>
        /// Get project detail basic info
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="project_id"></param>
        /// <returns></returns>
        SalesProjectDetailBasicInfo GetProjectDetailBasicInfo(int group_id, int project_id);

        /// <summary>
        /// Sales list by group detail
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<ProjectSalesInfo> GetListSalesGroupDetail(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, string companyCode, SalesGroupDetailCondition condition);

        /// <summary>
        /// Sales list by project detail
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<PersonalSalesInfo> GetListSalesProjectDetail(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, SalesProjectDetailCondition condition);
    }
}
