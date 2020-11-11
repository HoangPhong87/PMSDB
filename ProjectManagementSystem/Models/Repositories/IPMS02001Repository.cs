using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS02001;
using PetaPoco;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS02001Repository
    {
        /// <summary>
        /// Get list customer
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>Json list customer</returns>
        PageInfo<CustomerPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, string companyCode);

        /// <summary>
        /// Get a user info
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="customerId">customerId</param>
        /// <returns>customer info</returns>
        CustomerPlus GetCustomerInfo(string cCode, int customerId);

        /// <summary>
        /// Get all Prefecture
        /// </summary>
        /// <returns>List of Prefecture</returns>
        IEnumerable<Prefecture> GetPrefectureList();

        /// <summary>
        /// Check exist email address
        /// </summary>
        /// <param name="email">Email address</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>1 OR 0</returns>
        int CheckCustomerEmail(string email, int customerId);

        /// <summary>
        /// Edit customer info
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int EditCustomerData(Customer data);

        /// <summary>
        /// Get list Customer
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>CustomerPlus</returns>
        List<CustomerPlus> GetListCustomer(Condition condition, string companycode, string orderBy, string orderType);

        /// <summary>
        /// Build SQL customer list
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        Sql BuildSqlCustomerList(string companyCode, Condition condition);
    }
}
