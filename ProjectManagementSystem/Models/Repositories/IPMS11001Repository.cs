#region License
/// <copyright file="IPMS11001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ - Clone</author>
/// <createdDate>2017/06/28</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS11001;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Branch repository interface class
    /// </summary>
    public interface IPMS11001Repository
    {
        /// <summary>
        /// Search Branch by condition
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns>Search result</returns>
        PageInfo<BranchPlus> Search(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition);

        /// <summary>
        /// Export Branch List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Branch list</returns>
        IList<BranchPlus> ExportBranchListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType);

        /// <summary>
        /// Get Branch info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="BranchID">Branch ID</param>
        /// <returns>Branch info</returns>
        BranchPlus GetBranchInfo(string companyCode, int branchID);

        /// <summary>
        /// Edit Branch info
        /// </summary>
        /// <param name="data">Branch info</param>
        /// <param name="BranchID">Branch ID output</param>
        /// <returns>Action result</returns>
        bool EditBranchInfo(BranchPlus data, out int branchID);
    }
}
