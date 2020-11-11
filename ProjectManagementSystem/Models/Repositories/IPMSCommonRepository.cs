#region License
/// <copyright file="IPMSCommonRepository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/11/05</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Common repository interface class
    /// </summary>
    public interface IPMSCommonRepository
    {
        /// <summary>
        /// Get all status
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of status</returns>
        IEnumerable<Status> GetStatusList(string companyCode);

        /// <summary>
        /// Get all contract type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="isFilter">Filter by budget_setting_flg or not</param>
        /// <returns>List of contract type</returns>
        IEnumerable<ContractType> GetContractTypeList(string companyCode, bool isFilter = false);

        /// <summary>
        /// Get all project rank
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of project rank</returns>
        IEnumerable<Rank> GetRankList(string companyCode);

        /// <summary>
        /// Get user group list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="isFilter">Filter by budget_setting_flg or not</param>
        /// <returns>List of user group</returns>
        IEnumerable<Group> GetUserGroupList(string companyCode, bool isFilter = false);

        /// <summary>
        /// Get Branch List
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        IEnumerable<BusinessLocation> GetBranchList(string cCode);

        /// <summary>
        /// Get tag list of customer
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IList<CustomerTag> GetTagList(string companyCode, int customerId);

        /// <summary>
        /// Get all Prefecture
        /// </summary>
        /// <returns>List of Prefecture</returns>
        IEnumerable<Prefecture> GetPrefectureList();

        /// <summary>
        /// Check limit data by license
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="dataType"></param>
        /// <param name="dataImportQuantity"></param>
        /// <returns></returns>
        int CheckValidUpdateData(string companyCode, string dataType, int? dataImportQuantity);

        /// <summary>
        /// get project sys id by project no
        /// </summary>
        /// <param name="projectNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        string getProjectIdByProjectNo(string projectNo, string companyCode);
    }
}
