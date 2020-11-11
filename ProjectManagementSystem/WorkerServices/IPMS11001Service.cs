#region License
/// <copyright file="IPMS11001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ</author>
/// <createdDate>2017/06/28</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS11001;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS11001Service
    {
        /// <summary>
        /// Search Branch by condition
        /// </summary>
        /// <param name="model">Datatable model</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        PageInfo<BranchPlus> Search(DataTablesModel model, string companyCode, Condition condition);

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
        /// <param name="BranchID">Group ID</param>
        /// <returns>Group info</returns>
        BranchPlus GetBranchInfo(string companyCode, int groupID);

        /// <summary>
        /// Edit Branch info
        /// </summary>
        /// <param name="data">Branch info</param>
        /// <param name="BranchID">Branch ID output</param>
        /// <returns>Action result</returns>
        bool EditBranchInfo(BranchPlus data, out int BranchID);
    }
}
