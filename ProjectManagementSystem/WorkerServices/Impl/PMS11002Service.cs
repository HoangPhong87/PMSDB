#region License
/// <copyright file="PMS11002Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS11002;
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
    public class PMS11002Service : IPMS11002Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS11002Repository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11002Service()
            : this(new PMS11002Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS11002Service(IPMS11002Repository _repository)
        {
            this._repository = _repository;
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

        /// <summary>
        /// Process update function for list of budget data
        /// </summary>
        /// <param name="listBudget"></param>
        /// <returns></returns>
        public bool ProcessUpdateBudget(IList<Budget> listBudget)
        {
            var result = false;
            using (var trans = new TransactionScope())
            {
                result = this._repository.ProcessUpdateBudget(listBudget);
                if (result)
                {
                    trans.Complete();
                }
            }
            return result;
        }

        /// <summary>
        ///  Get list Budget data to use in Search function
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IEnumerable<BudgetPlus> GetListBudgetBySearch(string contractType,string group, string month, string year, string companyCode = "", List<TimeListBudget> timeList = null)
        {
            return this._repository.GetListBudgetBySearch(contractType, group, month, year, companyCode, timeList);
        }

        /// <summary>
        /// Get List Total profit and sales budget of year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<dynamic> GetListTotalYearBySearch(string year, string companyCode = "", string contract_type = "")
        {
            return this._repository.GetListTotalYearBySearch(year,companyCode, contract_type);
        }

        public int GetAccountClosingMonth(string companyCode)
        {
            return this._repository.GetAccountClosingMonth(companyCode);
        }
    }
}
