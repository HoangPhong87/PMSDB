using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS09003;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS09003Repository
    {
        /// <summary>
        /// Get group list
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <returns>List Group</returns>
        IEnumerable<Group> GetGroupList(string cCode);

        /// <summary>
        /// Get list Sale Customer
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List Sale Customer</returns>
        PageInfo<SalesCustomerPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, string companyCode);

        /// <summary>
        /// Get list Sale Customer
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List sale customer</returns>
        IList<SalesCustomerPlus> SearchAll(string companyCode, Condition condition);

        /// <summary>
        /// Get list sale Customer
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List Sale Customer</returns>
        List<SalesCustomerPlus> GetListSalesCustomer(Condition condition, string companycode, string orderBy, string orderType);

        /// <summary>
        /// Get list tag by customer
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List Sale Tag by Customer</returns>
        PageInfo<SalesTagByCustomerPlus> SearchTagByCustomer(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, string companyCode, ConditionSaleTag condition);

        /// <summary>
        /// Get list sale project by Customer
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List Sale project by customer</returns>
        PageInfo<SalesProjectByCustomerPlus> SearchProjectByCustomer(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, string companyCode, ConditionSaleProject condition);
    }
}
