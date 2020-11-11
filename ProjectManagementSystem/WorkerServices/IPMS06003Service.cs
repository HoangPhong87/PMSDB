#region License
/// <copyright file="IPMS06003Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>TrungNT</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

using System.Collections.Generic;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS06003;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// interface service for PMS06003
    /// </summary>
    public interface IPMS06003Service
    {
        /// <summary>
        /// Search page plan by Project
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        PageInfo<dynamic> SearchPlanByProject(DataTablesModel model, ProjectCondition condition, string companycode);

        /// <summary>
        /// Search list plan by Project
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        IList<dynamic> GetListPlanByProject(ProjectCondition condition, string companycode, int sort_colum, string sort_type);

        /// <summary>
        /// Search project by condition
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns>Search result</returns>
        PageInfo<dynamic> SearchAssignmentByUser(DataTablesModel model, UserCondition condition, string companycode);

        /// <summary>
        /// Search project by condition
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns>Search result</returns>
        IList<dynamic> SearchPriceInforByUser(DataTablesModel model, UserCondition condition, string companycode);

        /// <summary>
        /// Get List Assignment By User
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns>List of assignment</returns>
        IList<dynamic> GetListAssignmentByUser(UserCondition condition, string companycode, int sort_colum, string sort_type);

        /// <summary>
        /// Get List Assignment By User
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns>List of assignment</returns>
        IList<dynamic> GetListPriceInfor(UserCondition condition, string companycode, int sort_colum, string sort_type);
    }
}
