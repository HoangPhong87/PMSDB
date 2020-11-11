using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS01002;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices
{
    public interface IPMS01002Service
    {
        PageInfo<UserPlus> Search(DataTablesModel model, Condition condition, string companyCode);

        /// <summary>
        /// Get a user info
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="userId">user id</param>
        /// <returns>user info</returns>
        UserPlus GetUserInfo(string cCode, int userId);

        /// <summary>
        /// Get a unit price history info
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="userId">user id</param>
        /// <param name="startDate">apply start date</param>
        /// <returns>user info</returns>
        IList<UnitPriceHistoryPlus> GetUnitPriceHistoryInfo(string cCode, int userId);


        /// <summary>
        /// Get data_editable_time from company setting to delete progress
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetDataEditTableTime(string companyCode);

        /// <summary>
        /// Get all group
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of group</returns>
        IEnumerable<Group> GetGroupList(string cCode);

        /// <summary>
        /// Get all position
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of position</returns>
        IEnumerable<Position> GetPositionList(string cCode);

        IEnumerable<BusinessLocation> GetBranchList(string cCode);

        /// <summary>
        /// Get all role
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of role</returns>
        IEnumerable<AuthorityRole> GetAuthorityRoleList(string cCode);

        /// <summary>
        /// Get all language
        /// </summary>
        /// <returns>List of language</returns>
        IEnumerable<Language> GetLanguageList();

        /// <summary>
        /// Edit User Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int EditUserData(UserPlus data);

        /// <summary>
        /// Update Unit Price History
        /// </summary>
        /// <param name="data"></param>
        /// <param name="editor_id"></param>
        /// <param name="addNewUser"></param>
        int UpdateUnitPriceHistory(UserPlus data, int editor_id);

        /// <summary>
        /// Personal Setting User Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int PersonalSettingUserData(UserPlus data);

        /// <summary>
        /// Check exist email address
        /// </summary>
        /// <param name="email1">Email address 1</param>
        /// <param name="email2">Email address 2</param>
        /// <param name="userId">User ID</param>
        /// <param name="companyCode">Company Code</param>
        /// <returns>1 OR 0</returns>
        int CheckUserEmail(string email1, string email2, int userId, string companyCode);

        /// <summary>
        /// Check exist user account
        /// </summary>
        /// <param name="userAcount">User Account</param>
        /// <param name="companyCode">company Code</param>
        /// <param name="userId">User ID</param>
        /// <returns>1 OR 0</returns>
        int CheckUserAccount(string userAcount, string companyCode, int userId);

        /// <summary>
        /// Get member assignment list
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <param name="pCode">Project code</param>
        /// <returns>List of member assignment</returns>
        IList<UserPlus> GetMemberAssignmentList(string cCode, int pCode);


        /// <summary>
        /// Get list Customer
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>UserPlus</returns>
        List<UserPlus> GetListUser(Condition condition, string companycode, string orderBy, string orderType);

        User CheckPassword(string userAcount, string companyCode, int userID);

        List<UnitPriceHistory> GetListUnitCost(string companyCode, int userId);
    }
}
