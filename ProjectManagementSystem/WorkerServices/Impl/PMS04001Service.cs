#region License
/// <copyright file="PMS04001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/20</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS04001;
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
    public class PMS04001Service : IPMS04001Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS04001Repository _repository;

        /// <summary>
        /// Contractor
        /// </summary>
        public PMS04001Service()
            : this(new PMS04001Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS04001Service(IPMS04001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Search phase by condition
        /// </summary>
        /// <param name="model">Datatable model</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<PhasePlus> Search(DataTablesModel model, string companyCode, Condition condition)
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
        /// Export Phase List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Phase list</returns>
        public IList<PhasePlus> ExportPhaseListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            return this._repository.ExportPhaseListToCSV(companyCode, condition, orderBy, orderType);
        }

        /// <summary>
        /// Get phase info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="phaseID">Phase ID</param>
        /// <returns>Phase info</returns>
        public PhasePlus GetPhaseInfo(string companyCode, int phaseID)
        {
            return this._repository.GetPhaseInfo(companyCode, phaseID);
        }

        /// <summary>
        /// Edit phase info
        /// </summary>
        /// <param name="data">Phase info</param>
        /// <param name="phaseID">Phase ID output</param>
        /// <returns>Action result</returns>
        public bool EditPhaseInfo(PhasePlus data, out int phaseID)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = this._repository.EditPhaseInfo(data, out phaseID);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
