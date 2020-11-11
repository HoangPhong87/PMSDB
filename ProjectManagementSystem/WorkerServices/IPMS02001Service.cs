using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS02001;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices
{
    public interface IPMS02001Service
    {
        PageInfo<CustomerPlus> Search(DataTablesModel model, Condition condition, string companyCode);

        /// <summary>
        /// Get a user info
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="customerId">customer Id</param>
        /// <returns>customer info</returns>
        CustomerPlus GetCustomerInfo(string cCode, int userId);

        /// <summary>
        /// Get all Prefecture
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of Prefecture</returns>
        IEnumerable<Prefecture> GetPrefectureList();

        int EditCustomerData(Customer data);

        /// <summary>
        /// Check exist email address
        /// </summary>
        /// <param name="email">Email address</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>1 OR 0</returns>
        int CheckCustomerEmail(string email, int customerId);

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
        /// Import data cutomer from csv
        /// </summary>
        /// <param name="customerList"></param>
        /// <returns></returns>
        int ImportCustomerData(IList<Customer> customerList);
    }
}
