#region License
/// <copyright file="PMS06003Service.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>TrungNT</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

using System.Collections.Generic;
using System.Transactions;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS06003;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Service for PMS06003
    /// </summary>
    public class PMS06003Service : IPMS06003Service
    {
        private readonly IPMS06003Repository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS06003Service() : this(new PMS06003Repository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository"></param>
        public PMS06003Service(IPMS06003Repository repository)
        {
            _repository = repository;
        }
        

        /// <summary>
        /// Search page plan by Project
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>List plan by project</returns>
        public PageInfo<dynamic> SearchPlanByProject(DataTablesModel model, ProjectCondition condition, string companycode)
        {
            var pagePlan = this._repository.SearchPlanByProject(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                condition,
                companycode);
            return pagePlan;
        }

        /// <summary>
        /// Search list plan by Project
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>List Plan By Project</returns>
        public IList<dynamic> GetListPlanByProject(ProjectCondition condition, string companycode, int sort_colum, string sort_type)
        {
            var listPlan = this._repository.GetListPlanByProject(
                condition,
                companycode,
                sort_colum,
                sort_type);
            return listPlan;
        }

        /// <summary>
        /// Search project by condition
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <returns>Search result</returns>
        public PageInfo<dynamic> SearchAssignmentByUser(DataTablesModel model, UserCondition condition, string companycode)
        {
            var pageInfo = this._repository.SearchAssignmentByUser(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                condition,
                companycode);
            return pageInfo;
        }

        /// <summary>
        /// Search Individual Sales By User
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="companycode">condition</param>
        /// <returns>pageInfo</returns>
        public IList<dynamic> SearchPriceInforByUser(DataTablesModel model, UserCondition condition, string companycode)
        {
            var pageInfo = this._repository.SearchPriceInforByUser(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                condition,
                companycode);
            return pageInfo;
        }

        /// <summary>
        /// Get List Assignment By User
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns>List of assignment</returns>
        public IList<dynamic> GetListAssignmentByUser(UserCondition condition, string companycode, int sort_colum, string sort_type)
        {
            var listAssignment = this._repository.GetListAssignmentByUser(
                condition,
                companycode,
                sort_colum,
                sort_type);
            return listAssignment;
        }

        /// <summary>
        /// Get List Assignment By User
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns>List of assignment</returns>
        public IList<dynamic> GetListPriceInfor(UserCondition condition, string companycode, int sort_colum, string sort_type)
        {
            var listAssignment = this._repository.GetListPriceInfor(
                condition,
                companycode,
                sort_colum,
                sort_type);
            return listAssignment;
        }
    }
}