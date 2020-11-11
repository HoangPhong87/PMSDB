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
using ProjectManagementSystem.Models.PMS11003;
using ProjectManagementSystem.ViewModels;
using ProjectManagementSystem.ViewModels.PMS11003;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IPMS11003Service
    {
        /// <summary>
        /// get account closing month
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetAccountClosingMonth(string companyCode);

        /// <summary>
        /// Get list sale data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        IList<dynamic> GetListSaleData(string timeStart, string timeEnd, string companyCode = "", string contract_type = "", string group = "", string checkSalesType = "0");

        /// <summary>
        /// Get List Total Group
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        IList<dynamic> GetListTotalGroup(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "" , string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get List Total Contract Type
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        IList<dynamic> GetListTotalCT(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get List Charge Person Null
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        IList<dynamic> GetListChargePersonNull(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get List Total Contract Type
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        IList<dynamic> GetListTotalAllYearGroup(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0");

        /// <summary>
        /// Get List of profit budget data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        IList<ProfitBudget> GetListProfitBudget(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id);

        /// <summary>
        /// Get list of cost data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<CostPrice> GetListCost(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType);

        /// <summary>
        /// Get list of sales actual data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        IList<SalesPrice> GetListSaleActual(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType);

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
        IEnumerable<ContractType> GetListContractTypeBySearch(string contractType, string companyCode = "");
        IEnumerable<Group> GetListGroupBySearch(string groupId, string companyCode = "");
    }
}
