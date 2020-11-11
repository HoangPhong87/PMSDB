using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10004;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    public class PMS10004Service : IPMS10004Service
    {
        /// <summary>
        /// Interface
        /// </summary>
        private IPMS10004Repository _repository;

        /// <summary>
        /// Contractor
        /// </summary>
        public PMS10004Service()
            : this(new PMS10004Repository())
        {
        }

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS10004Service(IPMS10004Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Search data category list
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<CategoryPlus> Search(DataTablesModel model, Condition condition)
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

        /// <summary>
        /// Get list category
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IList<CategoryPlus> GetListCategory(Condition condition, string hdnOrderBy, string hdnOrderType)
        {
            return this._repository.GetListCategory(condition, hdnOrderBy, hdnOrderType);
        }

        /// <summary>
        /// Get category Info
        /// </summary>
        /// <param name="company_code"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CategoryPlus GetCategoryInfo(string company_code, int categoryId)
        {
            return this._repository.GetCategoryInfo(company_code, categoryId);
        }

        /// <summary>
        /// Get list of subcategory
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public List<SubCategoryPlus> GetListSubCategory(string company_code, int categoryId)
        {
            return this._repository.GetListSubCategory(company_code, categoryId);
        }

        /// <summary>
        /// Edit category data
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public int EditCategoryData(CategoryPlus category, IList<SubCategoryPlus> subCategoryList)
        {
            int res;

            using (var transaction = new TransactionScope())
            {
                res = this._repository.EditCategory(category, subCategoryList);

                if (res > 0)
                    transaction.Complete();
            }

            return res;
        }

        /// <summary>
        /// Get target category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="subCategoryID">sub category ID</param>
        /// <param name="categoryID">category ID</param>
        /// <returns>List of sub category</returns>
        public IList<TargetCategory> GetTargetCategory(string companyCode, int subCategoryID, int categoryID)
        {
            return this._repository.GetTargetCategory(companyCode, subCategoryID, categoryID);
        }
    }
}