using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS02001;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    public class PMS02001Service : IPMS02001Service
    {
         /// <summary>
        /// </summary>
        private IPMS02001Repository _repository;

        /// <summary>
        /// </summary>
        public PMS02001Service()
            : this(new PMS02001Repository())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="_repository"></param>
        public PMS02001Service(IPMS02001Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get list Customer
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>list Customer</returns>
        public PageInfo<CustomerPlus> Search(DataTablesModel model, Condition condition, string companyCode)
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
        /// Get customer info
        /// </summary>
        /// <param name="cCode">cCode</param>
        /// <param name="userId">userId</param>
        /// <returns>Customer info</returns>
        public CustomerPlus GetCustomerInfo(string cCode, int userId)
        {
            return this._repository.GetCustomerInfo(cCode, userId);
        }

        /// <summary>
        /// Get Prefecture List
        /// </summary>
        /// <returns>List Prefecture</returns>
        public IEnumerable<Prefecture> GetPrefectureList()
        {
            return this._repository.GetPrefectureList();
        }

        /// <summary>
        /// Edit Customer
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>record is edit</returns>
        public int EditCustomerData(Customer data)
        {
            int res = 0;
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    res = _repository.EditCustomerData(data);

                    if (res > 0)
                        transaction.Complete();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    res = 0;
                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return res;
        }

        /// <summary>
        /// Check exist email address
        /// </summary>
        /// <param name="email">Email address</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>1 OR 0</returns>
        public int CheckCustomerEmail(string email, int customerId)
        {
            return _repository.CheckCustomerEmail(email, customerId);
        }

        /// <summary>
        /// Get list customer
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List customer</returns>
        public List<CustomerPlus> GetListCustomer(Condition condition, string companycode, string orderBy, string orderType)
        {
            var listCustomer = this._repository.GetListCustomer(
                condition,
                companycode,
                orderBy,
                orderType);
            return listCustomer;
        }

        /// <summary>
        /// Import data cutomer from csv
        /// </summary>
        /// <param name="customerList"></param>
        /// <returns></returns>
        public int ImportCustomerData(IList<Customer> customerList)
        {
            int res = 0;

            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    foreach (var customer in customerList)
                    {
                        res = _repository.EditCustomerData(customer);

                        if (res == 0)
                            break;
                    }

                    if (res > 0)
                        transaction.Complete();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    res = 0;
                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return res;
        }
    }
}