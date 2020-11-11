#region License
/// <copyright file="IPMS11002Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS11002;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS11002Service
    {
        /// <summary>
        /// Get list group by search
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        IEnumerable<Group> GetListGroupBySearch(string groupId, string companyCode = "");

        /// <summary>
        /// Get list contract type by search
        /// </summary>
        /// <param name="contractType"></param>
        /// <returns></returns>
        IEnumerable<ContractType> GetListContractTypeBySearch(string contractType, string companyCode = "");

        /// <summary>
        /// Check count data budget in database
        /// </summary>
        /// <returns></returns>
        bool ProcessUpdateBudget(IList<Budget> listBudget);

        /// <summary>
        /// Get list budget data to use in Search function
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IEnumerable<BudgetPlus> GetListBudgetBySearch(string contractType, string group, string month, string year, string companyCode = "", List<TimeListBudget> timeList = null);

        /// <summary>
        /// Get List Total profit and sales budget of year
        /// </summary>
        /// <param name="target_year"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IList<dynamic> GetListTotalYearBySearch(string year, string companyCode = "",string contract_type = "");

        int GetAccountClosingMonth(string companyCode);
    }
}
