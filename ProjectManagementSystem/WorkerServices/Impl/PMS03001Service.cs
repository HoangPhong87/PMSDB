#region License
/// <copyright file="PMS03001Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS03001;
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
    public class PMS03001Service : IPMS03001Service
    {
        /// <summary>
        /// interface
        /// </summary>
        private IPMS03001Repository _repository;

        /// <summary>
        /// contractor
        /// </summary>
        public PMS03001Service()
            : this(new PMS03001Repository())
        {
        }

        /// <summary>
        /// bind
        /// </summary>
        /// <param name="_repository"></param>
        public PMS03001Service(IPMS03001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Search contractType by condition
        /// </summary>
        /// <param name="model">Datatable model</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<ContractTypePlus> Search(DataTablesModel model, string companyCode, Condition condition)
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
        /// Export ContractType List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>ContractType list</returns>
        public IList<ContractTypePlus> ExportContractTypeListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            return this._repository.ExportContractTypeListToCSV(companyCode, condition, orderBy, orderType);
        }

        /// <summary>
        /// Get contractType info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>ContractType info</returns>
        public ContractTypePlus GetContractTypeInfo(string companyCode, int contractTypeID)
        {
            return this._repository.GetContractTypeInfo(companyCode, contractTypeID);
        }

        /// <summary>
        /// Get contract type phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>List of contract type phase</returns>
        public IList<ContractTypePhasePlus> GetContractTypePhaseList(string companyCode, int contractTypeID)
        {
            return this._repository.GetContractTypePhaseList(companyCode, contractTypeID);
        }

        /// <summary>
        /// Get contract type category list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>List of contract type category</returns>
        public IList<ContractTypeCategoryPlus> GetContractTypeCategoryList(string companyCode, int contractTypeID)
        {
            return this._repository.GetContractTypeCategoryList(companyCode, contractTypeID);
        }

        /// <summary>
        /// Get phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of phase</returns>
        public IList<Phase> GetPhaseList(string companyCode, int contractTypeID)
        {
            return this._repository.GetPhaseList(companyCode, contractTypeID);
        }

        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of category</returns>
        public IList<Category> GetCategoryList(string companyCode, int contractTypeID)
        {
            return this._repository.GetCategoryList(companyCode, contractTypeID);
        }


        /// <summary>
        /// Edit contractType info
        /// </summary>
        /// <param name="data">ContractType info</param>
        /// <param name="contractTypePhaseList">Contract type phase list</param>
        /// <param name="contractTypeCategoryList">Contract type category list</param>
        /// <param name="contractTypeID">ContractType ID output</param>
        /// <returns>Action result</returns>
        public bool EditContractTypeInfo(ContractTypePlus data, IList<ContractTypePhasePlus> contractTypePhaseList, IList<ContractTypeCategoryPlus> contractTypeCategoryList, out int contractTypeID)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = this._repository.EditContractTypeInfo(data, contractTypePhaseList, contractTypeCategoryList, out contractTypeID);

                if (res)
                    transaction.Complete();
            }

            return res;
        }

        /// <summary>
        /// Get target phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="phaseID">Phase ID</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of target phase</returns>
        public IList<TargetPhase> GetTargetPhaseList(string companyCode, int phaseID, int contractTypeID)
        {
            return this._repository.GetTargetPhaseList(companyCode, phaseID, contractTypeID);
        }

        /// <summary>
        /// Get target category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="categoryID">Category ID</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of target category</returns>
        public IList<TargetCategory> GetTargetCategoryList(string companyCode, int categoryID, int contractTypeID)
        {
            return this._repository.GetTargetCategoryList(companyCode, categoryID, contractTypeID);
        }
    }
}
