#region License
/// <copyright file="IPMS03001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS03001;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// ContractType repository interface class
    /// </summary>
    public interface IPMS03001Repository
    {
        /// <summary>
        /// Search contractType by condition
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns>Search result</returns>
        PageInfo<ContractTypePlus> Search(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition);

        /// <summary>
        /// Export ContractType List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>ContractType list</returns>
        IList<ContractTypePlus> ExportContractTypeListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType);

        /// <summary>
        /// Get contractType info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>ContractType info</returns>
        ContractTypePlus GetContractTypeInfo(string companyCode, int contractTypeID);

        /// <summary>
        /// Get contract type phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>List of contract type phase</returns>
        IList<ContractTypePhasePlus> GetContractTypePhaseList(string companyCode, int contractTypeID);

        /// <summary>
        /// Get contract type category list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>List of contract type category</returns>
        IList<ContractTypeCategoryPlus> GetContractTypeCategoryList(string companyCode, int contractTypeID);

        /// <summary>
        /// Get phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of phase</returns>
        IList<Phase> GetPhaseList(string companyCode, int contractTypeID);

        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of phase</returns>
        IList<Category> GetCategoryList(string companyCode, int contractTypeID);

        /// <summary>
        /// Edit contractType info
        /// </summary>
        /// <param name="data">ContractType info</param>
        /// <param name="contractTypePhaseList">Contract type phase list</param>
        /// <param name="contractTypePhaseList">Contract type phase list</param>
        /// <param name="contractTypeID">ContractType ID output</param>
        /// <returns>Action result</returns>
        bool EditContractTypeInfo(ContractTypePlus data, IList<ContractTypePhasePlus> contractTypePhaseList, IList<ContractTypeCategoryPlus> contractTypeCategoryList, out int contractTypeID);

        /// <summary>
        /// Get target phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="phaseID">Phase ID</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of target phase</returns>
        IList<TargetPhase> GetTargetPhaseList(string companyCode, int phaseID, int contractTypeID);

        /// <summary>
        /// Get target category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="categoryID">Category ID</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of target category</returns>
        IList<TargetCategory> GetTargetCategoryList(string companyCode, int categoryID, int contractTypeID);
    }
}
