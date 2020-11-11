using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10002;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    public class PMS10002Service : IPMS10002Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS10002Repository _repository;

        /// <summary>
        /// Contractor
        /// </summary>
        public PMS10002Service()
            : this(new PMS10002Repository())
        {
        }

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS10002Service(IPMS10002Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get infomartion list by page
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public PageInfo<InformationPlus> Search(DataTablesModel model, Condition condition, string companyCode)
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
        /// Get infomartion list display to top
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="company_code"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public List<InformationPlus> GetListInfomation(Condition condition, string company_code, string orderBy, string orderType)
        {
            var listInfo = this._repository.GetListInfomation(
                condition,
                company_code,
                orderBy,
                orderType);
            return listInfo;
        }

        /// <summary>
        /// Get information detail
        /// </summary>
        /// <param name="company_code"></param>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public InformationPlus GetInformation(string company_code, int infoId)
        {
            return this._repository.GetInformation(company_code, infoId);
        }

        /// <summary>
        /// Edit information data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int EditInformationData(Information data)
        {
            return _repository.EditInformationData(data);
        }
    }
}