using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10001;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    public class PMS10001Service : IPMS10001Service
    {
        /// <summary>
        /// </summary>
        private IPMS10001Repository _repository;

        /// <summary>
        /// </summary>
        public PMS10001Service()
            : this(new PMS10001Repository())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="_repository"></param>
        public PMS10001Service(IPMS10001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get list customer tag
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>list customer tag</returns>
        public PageInfo<CustomerTagPlus> Search(DataTablesModel model, Condition condition, string companyCode)
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
        /// Get list tag
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List tag</returns>
        public List<CustomerTagPlus> GetListTag(Condition condition, string companycode, string orderBy, string orderType)
        {
            var listTag = this._repository.GetListTag(
                condition,
                companycode,
                orderBy,
                orderType);
            return listTag;
        }

        /// <summary>
        /// Get tag info
        /// </summary>
        /// <param name="cCode">cCode</param>
        /// <param name="tagId">tagId</param>
        /// <returns>Customer tag</returns>
        public CustomerTagPlus GetTagInfo(string cCode, int tagId)
        {
            return this._repository.GetTagInfo(cCode, tagId);
        }

        /// <summary>
        /// Edit Tag
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>record is edit</returns>
        public int EditTagData(CustomerTag data)
        {
            return _repository.EditTagData(data);
        }

        /// <summary>
        /// Get data sales payment
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="customer_id">customer_id</param>
        /// <param name="tag_id">tag_id</param>
        /// <returns>record sales payment</returns>
        public int GetDataSalesPayment(string company_code, int customer_id, int tag_id)
        {
            return _repository.GetDataSalesPayment(company_code, customer_id, tag_id);
        }
    }
}