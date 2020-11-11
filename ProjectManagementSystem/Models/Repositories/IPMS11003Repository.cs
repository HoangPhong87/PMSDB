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
using ProjectManagementSystem.Models.PMS11003;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Branch repository interface class
    /// </summary>
    public interface IPMS11003Repository
    {
        /// <summary>
        /// get list group by search
        /// </summary>
        /// <param name="group"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IEnumerable<Group> GetListGroupBySearch(string group, string companyCode);
        /// <summary>
        /// get contract by search
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        IEnumerable<ContractType> GetListContactTypeBySearch(string contractType, string companyCode);
        /// <summary>
        /// Get Acount closing month
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetAccountClosingMonth(string companyCode);

        /// <summary>
        /// Get list of sale data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type"></param>
        /// <param name="group"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<dynamic> GetListSaleData(string timeStart, string timeEnd, string companyCode, string contract_type, string group, string checkSalesType = "0");

        /// <summary>
        /// Get list sale data of total group
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<dynamic> GetListTotalGroup(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get list sale data of total contract type
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<dynamic> GetListTotalCT(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get list sale data when charge_person_id is null
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<dynamic> GetListChargePersonNull(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType = "0");

        /// <summary>
        /// Get list sale data of all group in year
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<dynamic> GetListTotalAllYearGroup(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get list of profit budget
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        IList<ProfitBudget> GetListProfitBudget(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "");

        /// <summary>
        /// Get list of cost
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<CostPrice> GetListCost(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get list of sales actual
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<SalesPrice> GetListSaleActual(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get group name
        /// </summary>
        /// <param name="companyCd"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        string GetGroupName(string companyCd, int groupId);

        /// <summary>
        /// Get contract type name
        /// </summary>
        /// <param name="companyCd"></param>
        /// <param name="contractTypeId"></param>
        /// <returns></returns>
        string GetContractTypeName(string companyCd, int contractTypeId);
    }
}
