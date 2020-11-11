#region License
/// <copyright file="IPMS11002Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ - Clone</author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS11002;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Branch repository interface class
    /// </summary>
    public interface IPMS11002Repository
    {
        IEnumerable<Group> GetListGroupBySearch(string group, string companyCode);
        IEnumerable<ContractType> GetListContactTypeBySearch(string contractType, string companyCode);

        bool CheckCountDataBudget(string companyCode);

        bool CheckDataValid(Budget budget);
        int InsertBudgetInfo(Budget budget);
        int UpdateBudgetInfo(Budget budget);

        bool ProcessUpdateBudget(IList<Budget> listBudget);

        IEnumerable<BudgetPlus> GetListBudgetBySearch(string contractType, string group, string month, string year, string companyCode = "", List<TimeListBudget> timeList = null);
        IList<dynamic> GetListTotalYearBySearch(string year, string companyCode,string contract_type);
        int GetAccountClosingMonth(string companyCode);
    }
}
