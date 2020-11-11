using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10005;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    public class PMS10005Service : IPMS10005Service
    {
        /// <summary>
        /// </summary>
        private IPMS10005Repository _repository;

        /// <summary>
        /// </summary>
        public PMS10005Service()
            : this(new PMS10005Repository())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="_repository"></param>
        public PMS10005Service(IPMS10005Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get list Overhead cost
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <returns>List overhead cost</returns>
        public PageInfo<OverHeadCostPlus> Search(DataTablesModel model, Condition condition)
        {
            var pageInfo = this._repository.Search(
                model.iDisplayStart,
                model.iDisplayLength,
                model.sColumns,
                model.iSortCol_0,
                model.sSortDir_0,
                condition);
            return pageInfo;
        }

        public List<OverHeadCostPlus> GetOverheadCostList(Condition condition, string orderBy, string orderType)
        {
            var list = this._repository.GetOverheadCostList(
                condition,
                orderBy,
                orderType);
            return list;
        }

        /// <summary>
        /// Get overhead cost info
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>OverHead Cost</returns>
        public OverHeadCostPlus GetOverheadCostInfo(string company_code, int overhead_cost_id)
        {
            return this._repository.GetOverheadCostInfo(company_code, overhead_cost_id);
        }

        /// <summary>
        /// Edit Overhead Cost
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>record is edit</returns>
        public int EditOverheadCostData(OverHeadCostPlus data)
        {
            return this._repository.EditOverheadCostData(data);
        }

        /// <summary>
        /// Delete overhead cost
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>record is delete</returns>
        public int DeleteOverHeadCode(string companyCode, int overhead_cost_id)
        {
            return this._repository.DeleteOverHeadCode(companyCode, overhead_cost_id);
        }

        /// <summary>
        /// Check exist of overhead cost
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>record is get</returns>
        public int CheckExistOfOverHeadCode(string companyCode, int overhead_cost_id)
        {
            return this._repository.CheckExistOfOverHeadCode(companyCode, overhead_cost_id);
        }
    }
}