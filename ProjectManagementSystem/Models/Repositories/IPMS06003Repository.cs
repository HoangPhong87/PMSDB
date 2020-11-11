#region License
/// <copyright file="IPMS06003Repository.cs" company="i-Enter Asia">
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

namespace ProjectManagementSystem.Models.Repositories
{
    /// <summary>
    /// Interface Repository for PMS06003
    /// </summary>
    public interface IPMS06003Repository
    {
        /// <summary>
        /// Search page plan by Project
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>Json Plan By Project</returns>
        PageInfo<dynamic> SearchPlanByProject(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, ProjectCondition condition, string companycode);

        /// <summary>
        /// Search list plan by Project
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>List Plan By Project</returns>
        IList<dynamic> GetListPlanByProject(ProjectCondition condition, string companycode, int sort_colum, string sort_type);

        /// <summary>
        /// Search project by condition
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>Search result</returns>
        PageInfo<dynamic> SearchAssignmentByUser(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, UserCondition condition, string companycode);

        /// <summary>
        /// Search Individual Sales By User
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>Search result</returns>
        IList<dynamic> SearchPriceInforByUser(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, UserCondition condition, string companycode);

        /// <summary>
        /// GetListAssignmentByUser
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns>List of assignment</returns>
        IList<dynamic> GetListAssignmentByUser(UserCondition condition, string companycode, int sort_colum, string sort_type);

        /// <summary>
        /// GetListAssignmentByUser
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns>List of assignment</returns>
        IList<dynamic> GetListPriceInfor(UserCondition condition, string companycode, int sort_colum, string sort_type);
    }
}
