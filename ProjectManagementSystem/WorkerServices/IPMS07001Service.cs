#region License
/// <copyright file="IPMS07001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/23</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS07001;
using ProjectManagementSystem.ViewModels;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS07001Service
    {
        /// <summary>
        /// Search consumption tax by condition
        /// </summary>
        /// <param name="model">Datatable model</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        PageInfo<ConsumptionTaxPlus> Search(DataTablesModel model, string companyCode, Condition condition);

        /// <summary>
        /// Export ConsumptionTax List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>ConsumptionTax list</returns>
        IList<ConsumptionTaxPlus> ExportConsumptionTaxListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType);

        /// <summary>
        /// Get consumption tax info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>ConsumptionTax info</returns>
        ConsumptionTaxPlus GetConsumptionTaxInfo(string companyCode, DateTime applyStartDate);

        /// <summary>
        /// Edit consumption tax info
        /// </summary>
        /// <param name="data">ConsumptionTax info</param>
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
