using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS01002;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    public class PMS01002Service : IPMS01002Service
    {
        /// <summary>
        /// </summary>
        private IPMS01002Repository _repository;

        /// <summary>
        /// </summary>
        public PMS01002Service()
            : this(new PMS01002Repository())
        {
        }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_repository"></param>
        public PMS01002Service(IPMS01002Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get list user
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List user</returns>
        public PageInfo<UserPlus> Search(DataTablesModel model, Condition condition, string companyCode)
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
        /// Get a user info
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="userId">user id</param>
        /// <returns>user info</returns>
        public UserPlus GetUserInfo(string cCode, int userId)
        {
            return this._repository.GetUserInfo(cCode, userId);
        }

        /// <summary>
        /// Get a unit price history info
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="userId">user id</param>
        /// <param name="startDate">apply start date</param>
        /// <returns>user info</returns>
        public IList<UnitPriceHistoryPlus> GetUnitPriceHistoryInfo(string cCode, int userId)
        {
            return this._repository.GetUnitPriceHistoryInfo(cCode, userId);
        }

        /// <summary>
        /// Get data_editable_time from company setting to delete progress
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetDataEditTableTime(string companyCode)
        {
            return this._repository.GetDataEditTableTime(companyCode);
        }

        /// <summary>
        /// Get List group
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns>List group</returns>
        public IEnumerable<Group> GetGroupList(string cCode)
        {
            return this._repository.GetGroupList(cCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IEnumerable<Position> GetPositionList(string cCode)
        {
            return this._repository.GetPositionList(cCode);
        }

        public IEnumerable<BusinessLocation> GetBranchList(string cCode)
        {
            return this._repository.GetBranchList(cCode);
        }

        /// <summary>
        /// Get list Authority role 
        /// </summary>
        /// <param name="cCode">cCode</param>
        /// <returns>List Authority role</returns>
        public IEnumerable<AuthorityRole> GetAuthorityRoleList(string cCode)
        {
            return this._repository.GetAuthorityRoleList(cCode);
        }

        /// <summary>
        /// Get list languae
        /// </summary>
        /// <returns>List language</returns>
        public IEnumerable<Language> GetLanguageList()
        {
            return this._repository.GetLanguageList();
        }

        /// <summary>
        /// Edit user
        /// </summary>
        /// <param name="data"></param>
        /// <returns>number user is edit</returns>
        public int EditUserData(UserPlus data)
        {
            int res = 0;
            using (TransactionScope transaction = new TransactionScope())
            {
                res = _repository.EditUserData(data);

                if (res > 0)
                    transaction.Complete();
            }
            return res;
        }

        /// <summary>
        /// Update Unit Price History
        /// </summary>
        /// <param name="data"></param>
        /// <param name="editor_id"></param>
        /// <param name="addNewUser"></param>
        public int UpdateUnitPriceHistory(UserPlus data, int editor_id)
        {
            return _repository.UpdateUnitPriceHistory(data, editor_id);
        }

        /// <summary>
        /// Update info of user
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>Number user is update</returns>
        public int PersonalSettingUserData(UserPlus data)
        {
            int res = 0;
            using (TransactionScope transaction = new TransactionScope())
            {
                res = _repository.PersonalSettingUserData(data);

                if (res > 0)
                    transaction.Complete();
            }
            return res;
        }

        /// <summary>
        /// Check exist email address
        /// </summary>
        /// <param name="email1">Email address 1</param>
        /// <param name="email2">Email address 2</param>
        /// <param name="userId">User ID</param>
        /// <param name="companyCode">Company Code</param>
        /// <returns>1 OR 0</returns>
        public int CheckUserEmail(string email1, string email2, int userId, string companyCode)
        {
            return _repository.CheckUserEmail(email1, email2, userId, companyCode);
        }

        /// <summary>
        /// Check exist user account
        /// </summary>
        /// <param name="userAcount">User Account</param>
        /// <param name="companyCode">company Code</param>
        /// <param name="userId">User Id</param>
        /// <returns>1 OR 0</returns>
        public int CheckUserAccount(string userAcount, string companyCode, int userId)
        {
            return _repository.CheckUserAccount(userAcount, companyCode, userId);
        }

        /// <summary>
        /// Get member assignment list
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <param name="pCode">Project code</param>
        /// <returns>List of member assignment</returns>
        public IList<UserPlus> GetMemberAssignmentList(string cCode, int pCode)
        {
            return _repository.GetMemberAssignmentList(cCode, pCode);
        }

        /// <summary>
        /// Get list user
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List user</returns>
        public List<UserPlus> GetListUser(Condition condition, string companycode, string orderBy, string orderType)
        {
            var listUser = this._repository.GetListUser(
                condition,
                companycode,
                orderBy,
                orderType);
            return listUser;
        }

        /// <summary>
        /// Check password
        /// </summary>
        /// <param name="userAcount">userAcount</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userID">userID</param>
        /// <returns>User</returns>
        public User CheckPassword(string userAcount, string companyCode, int userID)
        {
            return _repository.CheckPassword(userAcount, companyCode, userID);
        }

        public List<UnitPriceHistory> GetListUnitCost(string companyCode, int userId)
        {
            return _repository.GetListUnitCost(companyCode, userId);
        }
    }
}