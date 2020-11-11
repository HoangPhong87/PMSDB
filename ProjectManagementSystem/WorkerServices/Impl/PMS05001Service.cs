#region License
/// <copyright file="PMS05001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/22</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS05001;
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
    public class PMS05001Service : IPMS05001Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS05001Repository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS05001Service()
            : this(new PMS05001Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS05001Service(IPMS05001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Search group by condition
        /// </summary>
        /// <param name="model">Datatable model</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<GroupPlus> Search(DataTablesModel model, string companyCode, Condition condition)
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
        /// Export Group List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Group list</returns>
        public IList<GroupPlus> ExportGroupListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            return this._repository.ExportGroupListToCSV(companyCode, condition, orderBy, orderType);
        }

        /// <summary>
        /// Get group info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="groupID">Group ID</param>
        /// <returns>Group info</returns>
        public GroupPlus GetGroupInfo(string companyCode, int groupID)
        {
            return this._repository.GetGroupInfo(companyCode, groupID);
        }

        /// <summary>
        /// Edit group info
        /// </summary>
        /// <param name="data">Group info</param>
        /// <param name="groupID">Group ID output</param>
        /// <returns>Action result</returns>
        public bool EditGroupInfo(GroupPlus data, out int groupID)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = this._repository.EditGroupInfo(data, out groupID);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
