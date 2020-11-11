#region License
/// <copyright file="IPMS05001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/22</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS05001;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Group repository interface class
    /// </summary>
    public interface IPMS05001Repository
    {
        /// <summary>
        /// Search group by condition
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns>Search result</returns>
        PageInfo<GroupPlus> Search(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition);

        /// <summary>
        /// Export Group List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Group list</returns>
        IList<GroupPlus> ExportGroupListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType);

        /// <summary>
        /// Get group info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="groupID">Group ID</param>
        /// <returns>Group info</returns>
        GroupPlus GetGroupInfo(string companyCode, int groupID);

        /// <summary>
        /// Edit group info
        /// </summary>
        /// <param name="data">Group info</param>
        /// <param name="groupID">Group ID output</param>
        /// <returns>Action result</returns>
        bool EditGroupInfo(GroupPlus data, out int groupID);
    }
}
