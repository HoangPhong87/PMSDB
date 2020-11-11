#region License
/// <copyright file="IPMS04001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/20</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS04001;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Phase repository interface class
    /// </summary>
    public interface IPMS04001Repository
    {
        /// <summary>
        /// Search phase by condition
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns>Search result</returns>
        PageInfo<PhasePlus> Search(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition);

        /// <summary>
        /// Export Phase List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Phase list</returns>
        IList<PhasePlus> ExportPhaseListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType);

        /// <summary>
        /// Get phase info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="phaseID">Phase ID</param>
        /// <returns>Phase info</returns>
        PhasePlus GetPhaseInfo(string companyCode, int phaseID);

        /// <summary>
        /// Edit phase info
        /// </summary>
        /// <param name="data">Phase info</param>
        /// <param name="phaseID">Phase ID output</param>
        /// <returns>Action result</returns>
        bool EditPhaseInfo(PhasePlus data, out int phaseID);
    }
}
