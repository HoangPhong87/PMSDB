#region License
/// <copyright file="PMS11001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ</author>
/// <createdDate>2017/06/28</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS11001;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Service class
    /// </summary>
    public class PMS11001Service : IPMS11001Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS11001Repository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11001Service()
            : this(new PMS11001Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS11001Service(IPMS11001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Search Branch by condition
        /// </summary>
        /// <param name="model">Datatable model</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<BranchPlus> Search(DataTablesModel model, string companyCode, Condition condition)
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
        /// Export Branch List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Branch list</returns>
        public IList<BranchPlus> ExportBranchListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            return this._repository.ExportBranchListToCSV(companyCode, condition, orderBy, orderType);
        }

        /// <summary>
        /// Get Branch info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="BranchID">Branch ID</param>
        /// <returns>Branch info</returns>
        public BranchPlus GetBranchInfo(string companyCode, int BranchID)
        {
            return this._repository.GetBranchInfo(companyCode, BranchID);
        }

        /// <summary>
        /// Edit Branch info
        /// </summary>
        /// <param name="data">Branch info</param>
        /// <param name="BranchID">Branch ID output</param>
        /// <returns>Action result</returns>
        public bool EditBranchInfo(BranchPlus data, out int BranchID)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = this._repository.EditBranchInfo(data, out BranchID);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
