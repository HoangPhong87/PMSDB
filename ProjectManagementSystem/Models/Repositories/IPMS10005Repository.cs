using ProjectManagementSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10005;
using PetaPoco;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS10005Repository
    {
        /// <summary>
        /// Search data M_OverheadCost list
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <returns>List over head cost</returns>
        PageInfo<OverHeadCostPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition);

        /// <summary>
        /// Get category Info
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="categoryId">categoryId</param>
        /// <returns>Over head cost</returns>
        OverHeadCostPlus GetOverheadCostInfo(string company_code, int overhead_cost_id);

        /// <summary>
        /// Edit Category
        /// </summary>
        /// <param name="category">category</param>
        /// <returns>number of record is edit</returns>
        int EditOverheadCostData(OverHeadCostPlus overheadCost);

        /// <summary>
        /// Get list overhead cost
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List overhead cost</returns>
        List<OverHeadCostPlus> GetOverheadCostList(Condition condition, string orderBy, string orderType);

        /// <summary>
        /// Delete overhead cost
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>number of record is delete</returns>
        int DeleteOverHeadCode(string companyCode, int overhead_cost_id);

        /// <summary>
        /// Check exist overhead cost
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>number of record is get</returns>
        int CheckExistOfOverHeadCode(string companyCode, int overhead_cost_id);

        /// <summary>
        /// Build SQL overhead cost list
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        Sql BuildSqlOverheadCostList(Condition condition);
    }
}
