using ProjectManagementSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10001;
using PetaPoco;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS10001Repository
    {
        /// <summary>
        /// Get list Customer Tag
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List Customer Tag</returns>
        PageInfo<CustomerTagPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, string companyCode);

        /// <summary>
        /// Get list tag
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>CustomerTagPlus</returns>
        List<CustomerTagPlus> GetListTag(Condition condition, string companycode, string orderBy, string orderType);

        /// <summary>
        /// Get Tag info
        /// </summary>
        /// <param name="cCode">cCode</param>
        /// <param name="tagId">tagId</param>
        /// <returns>Tag info</returns>
        CustomerTagPlus GetTagInfo(string cCode, int tagId);

        /// <summary>
        /// Edit Tag info
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>number of record is edit</returns>
        int EditTagData(CustomerTag data);

        /// <summary>
        /// Get Sale Payment
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="customer_id">customer_id</param>
        /// <param name="tag_id">tag_id</param>
        /// <returns>Number of record is get</returns>
        int GetDataSalesPayment(string company_code, int customer_id, int tag_id);

        /// <summary>
        /// Build SQL tag list
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        Sql BuildSqlTagList(string companyCode, Condition condition);
    }
}
