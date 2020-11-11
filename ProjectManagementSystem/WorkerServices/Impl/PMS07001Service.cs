#region License
/// <copyright file="PMS07001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/23</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS07001;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Service class
    /// </summary>
    public class PMS07001Service : IPMS07001Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS07001Repository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS07001Service()
            : this(new PMS07001Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS07001Service(IPMS07001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Search consumption tax by condition
        /// </summary>
        /// <param name="model">Datatable model</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<ConsumptionTaxPlus> Search(DataTablesModel model, string companyCode, Condition condition)
        {
            var pageInfo = this._repository.Search(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                companyCode,
                condition);

            return pageInfo;
        }

        /// <summary>
        /// Export ConsumptionTax List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>ConsumptionTax list</returns>
        public IList<ConsumptionTaxPlus> ExportConsumptionTaxListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            return this._repository.ExportConsumptionTaxListToCSV(companyCode, condition, orderBy, orderType);
        }

        /// <summary>
        /// Get consumption tax info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>ConsumptionTax info</returns>
        public ConsumptionTaxPlus GetConsumptionTaxInfo(string companyCode, DateTime applyStartDate)
        {
            return this._repository.GetConsumptionTaxInfo(companyCode, applyStartDate);
        }

        /// <summary>
        /// Edit consumption tax info
        /// </summary>
        /// <param name="data">ConsumptionTax info</param>
        /// <param name="consumption taxID">ConsumptionTax ID output</param>
        /// <returns>Action result</returns>
        public bool EditConsumptionTaxInfo(ConsumptionTaxPlus data)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = this._repository.EditConsumptionTaxInfo(data);

                if (res)
                    transaction.Complete();
            }

            return res;
        }

        /// <summary>
        /// Count consumption tax by apply start date
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>Number of consumption tax by apply start date</returns>
        public int CountConsumptionTax(string companyCode, DateTime applyStartDate)
        {
            return this._repository.CountConsumptionTax(companyCode, applyStartDate);
        }

        /// <summary>
        /// Delete consumption tax by apply start date
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>Delete consumption tax by apply start date</returns>
        public int DeleteConsumptionTax(string companyCode, DateTime applyStartDate)
        {
            return this._repository.DeleteConsumptionTax(companyCode, applyStartDate);
        }
    }
}
