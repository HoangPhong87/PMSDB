using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10004;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    public interface IPMS10004Service
    {
        /// <summary>
        /// Search data category list
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<CategoryPlus> Search(DataTablesModel model, Condition condition);

        /// <summary>
        /// Get list category
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IList<CategoryPlus> GetListCategory(Condition condition, string hdnOrderBy, string hdnOrderType);

        /// <summary>
        /// Get category Info
        /// </summary>
        /// <param name="company_code"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        CategoryPlus GetCategoryInfo(string company_code, int categoryId);

        /// <summary>
        /// Get list of subcategory
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        List<SubCategoryPlus> GetListSubCategory(string company_code, int categoryId);

        /// <summary>
        /// Edit category data
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        int EditCategoryData(CategoryPlus category, IList<SubCategoryPlus> subCategoryList);

        /// <summary>
        /// Get sub category list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="subCategoryID">sub category ID</param>
        /// <param name="categoryID">category ID</param>
        /// <returns>List of sub category</returns>
        IList<TargetCategory> GetTargetCategory(string companyCode, int subCategoryID, int categoryID);
    }
}
