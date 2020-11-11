#region License
/// <copyright file="IPMSCommonService.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/11/05</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// Common service interface class
    /// </summary>
    public interface IPMSCommonService
    {
        /// <summary>
        /// Get all status
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of status</returns>
        IEnumerable<Status> GetStatusList(string companyCode);

        /// <summary>
        /// Get all project rank
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of project rank</returns>
        IEnumerable<Rank> GetRankList(string companyCode);

        /// <summary>
        /// Get all tag link by compnay code
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of tag link</returns>
        IList<CustomerTag> GetTagList(string companyCode, int customerId);

        /// <summary>
        /// Get select list of status by company code
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>Select list of status</returns>
        IList<SelectListItem> GetStatusSelectList(string companyCode);

        /// <summary>
        /// Get select list of contract type by company code
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="isFilter">Filter by budget_setting_flg or not</param>
        /// <returns>Select list of contract type</returns>
        IList<SelectListItem> GetContractTypeSelectList(string companyCode, bool isFilter = false);

        /// <summary>
        /// Get select list of project rank
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>Select list of project rank</returns>
        IList<SelectListItem> GetRankSelectList(string companyCode);

        /// <summary>
        /// Get select list of user group by company code
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="isFilter">Filter by budget_setting_flg or not</param>
        /// <returns>Select list of user group</returns>
        IList<SelectListItem> GetUserGroupSelectList(string companyCode, bool isFilter = false);

        /// <summary>
        /// Getselect list of branch 
        /// </summary>
        /// <returns></returns>
        IList<SelectListItem> GetBranchSelectList(string cCode);

        /// <summary>
        /// Get select list of tag link by customer
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="customerID">Customer ID</param>
        /// <returns>Select list of tag link</returns>
        IList<SelectListItem> GetTagSelectList(string companyCode, int customerID);

        /// <summary>
        /// Get select list of prefecture
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="customerID">Customer ID</param>
        /// <returns>Select list of tag link</returns>
        IList<SelectListItem> GetPrefectureList();

        /// <summary>
        /// Check limit data by license
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        bool CheckValidUpdateData(string companyCode, string dataType);

        /// <summary>
        /// Check limit data by license
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="dataType"></param>
        /// <param name="dataImportQuantity"></param>
        /// <returns></returns>
        bool CheckValidUpdateData(string companyCode, string dataType, int? dataImportQuantity);

        /// <summary>
        /// get project sys id by project no
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        string getProjectIdByProjectNo(string projectNo, string companyCode);
    }
}
