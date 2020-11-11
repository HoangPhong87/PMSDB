using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS01002;
using PetaPoco;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS01002Repository
    {
        /// <summary>
        /// Search user
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>Json list user</returns>
        PageInfo<UserPlus> Search(int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            Condition condition,
            string companyCode);

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

        /// <summary>
        /// Get all branch
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
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
        /// Edit user infor
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>record is edited</returns>
        int EditUserData(UserPlus data);

        /// <summary>
        /// Update Unit Price History
        /// </summary>
        /// <param name="data"></param>
        /// <param name="editor_id"></param>
        /// <param name="addNewUser"></param>
        int UpdateUnitPriceHistory(UserPlus data, int editor_id);

        /// <summary>
        /// Update user infor
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>record is update</returns>
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
        /// <param name="userId">User Id</param>
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
        /// Get list user
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>UserPlus</returns>
        List<UserPlus> GetListUser(Condition condition, string companycode, string orderBy, string orderType);

        /// <summary>
        /// Check password
        /// </summary>
        /// <param name="userAcount">userAcount</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userID">userID</param>
        /// <returns>User</returns>
        User CheckPassword(string userAcount, string companyCode, int userID);

        /// <summary>
        /// Build SQL user list
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        Sql BuildSqlUserList(string companyCode, Condition condition);


        List<UnitPriceHistory> GetListUnitCost(string companyCode, int userId);

    }
}
