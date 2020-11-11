#region License
/// <copyright file="IPMS07001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/23</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS07001;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// ConsumptionTax repository interface class
    /// </summary>
    public interface IPMS07001Repository
    {
        /// <summary>
        /// Search consumption tax by condition
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns>Search result</returns>
        PageInfo<ConsumptionTaxPlus> Search(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition);

        /// <summary>
        /// Export ConsumptionTax List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>ConsumtionTax list</returns>
        IList<ConsumptionTaxPlus> ExportConsumptionTaxListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType);

        /// <summary>
        /// Get consumption tax info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>ConsumtionTax info</returns>
        ConsumptionTaxPlus GetConsumptionTaxInfo(string companyCode, DateTime applyStartDate);

        /// <summary>
        /// Edit consumption tax info
        /// </summary>
        /// <param name="data">ConsumtionTax info</param>
        /// <returns>Action result</returns>
        bool EditConsumptionTaxInfo(ConsumptionTaxPlus data);

        /// <summary>
        /// Count consumption tax by apply start date
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>Number of consumption tax by apply start date</returns>
        int CountConsumptionTax(string companyCode, DateTime applyStartDate);

        /// <summary>
        /// Delete consumption tax by apply start date
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>Delete consumption tax by apply start date</returns>
        int DeleteConsumptionTax(string companyCode, DateTime applyStartDate);
    }
}
