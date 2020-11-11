#region License
/// <copyright file="PMS11003Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS11003;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Transactions;
using System;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Service class
    /// </summary>
    public class PMS11003Service : IPMS11003Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS11003Repository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11003Service()
            : this(new PMS11003Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS11003Service(IPMS11003Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get Account Closing Month
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetAccountClosingMonth(string companyCode)
        {
            return this._repository.GetAccountClosingMonth(companyCode);
        }

        /// <summary>
        /// Get List Sale Data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public IList<dynamic> GetListSaleData(string timeStart, string timeEnd, string companyCode = "", string contract_type = "", string group = "", string checkSalesType = "0")
        {
            return this._repository.GetListSaleData(timeStart, timeEnd,companyCode, contract_type, group, checkSalesType);
        }
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
        public IList<dynamic> GetListTotalGroup(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0")
        {
            return this._repository.GetListTotalGroup(timeStart,timeEnd,companyCode,contract_type_id,group_id, checkSalesType);
        }

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
        public IList<dynamic> GetListTotalCT(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0")
        {
            return this._repository.GetListTotalCT(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType);
        }

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
        public IList<dynamic> GetListChargePersonNull(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0")
        {
            return this._repository.GetListChargePersonNull(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType);
        }

        /// <summary>
        /// Get list sale data of group in a year
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        public IList<dynamic> GetListTotalAllYearGroup(string timeStart, string timeEnd, string companyCode = "", string contract_type_id = "", string group_id = "", string checkSalesType = "0")
        {
            return this._repository.GetListTotalAllYearGroup(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType);
        }

        /// <summary>
        /// Get list of profit budget data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public IList<ProfitBudget> GetListProfitBudget(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id)
        {
            return this._repository.GetListProfitBudget(timeStart, timeEnd, companyCode, contract_type_id, group_id);
        }

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
        public IList<CostPrice> GetListCost(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType)
        {
            return this._repository.GetListCost(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType);
        }

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
        public IList<SalesPrice> GetListSaleActual(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id,string checkSalesType)
        {
            return this._repository.GetListSaleActual(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType);
        }

        /// <summary>
        /// Get group name
        /// </summary>
        /// <param name="companyCd"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public string GetGroupName(string companyCd, int groupId)
        {
            return this._repository.GetGroupName(companyCd, groupId);
        }

        /// <summary>
        /// Get contract type name
        /// </summary>
        /// <param name="companyCd"></param>
        /// <param name="contractTypeId"></param>
        /// <returns></returns>
        public string GetContractTypeName(string companyCd, int contractTypeId)
        {
            return this._repository.GetContractTypeName(companyCd, contractTypeId);
        }
        /// <summary>
        /// Get list Contract type to use in Search function 
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IEnumerable<ContractType> GetListContractTypeBySearch(string contractType, string companyCode = "")
        {
            return this._repository.GetListContactTypeBySearch(contractType, companyCode);
        }

        /// <summary>
        /// Get list group to use in Search function
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IEnumerable<Group> GetListGroupBySearch(string groupId, string companyCode = "")
        {
            return this._repository.GetListGroupBySearch(groupId, companyCode);
        }

    }
}
