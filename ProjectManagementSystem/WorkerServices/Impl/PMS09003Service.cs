using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS09003;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    public class PMS09003Service : IPMS09003Service
    {
        /// <summary>
        /// </summary>
        private IPMS09003Repository _repository;

        /// <summary>
        /// </summary>
        public PMS09003Service()
            : this(new PMS09003Repository())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="_repository"></param>
        public PMS09003Service(IPMS09003Repository _repository)
        {
            this._repository = _repository;
        }

        public IEnumerable<Group> GetGroupList(string cCode)
        {
            return _repository.GetGroupList(cCode);
        }

        /// <summary>
        /// Get list sale customer
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>list sale customer</returns>
        public PageInfo<SalesCustomerPlus> Search(DataTablesModel model, Condition condition, string companyCode)
        {
            var pageInfo = this._repository.Search(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                condition,
                companyCode);
            return pageInfo;
        }

        /// <summary>
        /// Get list Sale Customer
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List sale customer</returns>
        public IList<SalesCustomerPlus> SearchAll(string companyCode, Condition condition)
        {
            return _repository.SearchAll(companyCode, condition);
        }

        /// <summary>
        /// Get list sale customer
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List Sale Customer</returns>
        public List<SalesCustomerPlus> GetListSalesCustomer(Condition condition, string companyCode, string orderBy, string orderType)
        {
            return _repository.GetListSalesCustomer(condition, companyCode, orderBy, orderType);
        }

        /// <summary>
        /// Get list tag by customer
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List tag by customer</returns>
        public PageInfo<SalesTagByCustomerPlus> SearchTagByCustomer(DataTablesModel model, string companyCode, ConditionSaleTag condition)
        {
            var pageInfo = this._repository.SearchTagByCustomer(
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
        /// Get list project by customer
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List project by customer</returns>
        public PageInfo<SalesProjectByCustomerPlus> SearchProjectByCustomer(DataTablesModel model, string companyCode, ConditionSaleProject condition)
        {
            var pageInfo = this._repository.SearchProjectByCustomer(
                 model.iDisplayStart,
                 model.iDisplayLength,
                 model.sColumns,
                 model.iSortCol_0,
                 model.sSortDir_0,
                 companyCode,
                 condition);

            return pageInfo;
        }
    }
}