using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10004;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS10004Repository
    {
        /// <summary>
        /// Search data category list
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PageInfo<CategoryPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition);

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
        /// Edit Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        int EditCategory(CategoryPlus category, IList<SubCategoryPlus> subCategoryList);

        /// <summary>
        /// Get target category
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="subCategoryID"></param>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        IList<TargetCategory> GetTargetCategory(string companyCode, int subCategoryID, int categoryID);
    }
}
