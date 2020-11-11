using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS01001;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS01001Repository
    {
        /// <summary>
        /// Get info of user (login)
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <param name="password">password</param>
        /// <returns>User Plus</returns>
        UserPlus GetUserInfo(string companyCode, string userAccount, string password);

        /// <summary>
        /// Get user by mail
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>List User</returns>
        IList<UserEmail> GetEmailUser(string email);

        /// <summary>
        /// Get user by mail and company code
        /// </summary>
        /// <param name="email">mail</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>User</returns>
        UserEmail GetEmailUser(string email, string companyCode);

        /// <summary>
        /// Get password
        /// </summary>
        /// <param name="param">param</param>
        /// <returns>Resset pass</returns>
        ResetPass GetPasswordResetInfo(string param);

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="password">password</param>
        /// <param name="password_lock_target">password_lock_target</param>
        /// <param name="company_code">company_code</param>
        /// <returns>number of record is update</returns>
        int UpdatePassword(int userId, string password, string password_lock_target, string company_code);

        /// <summary>
        /// Check exist user id by company code and user account
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <returns>number of record is existed</returns>
        int? IsExistedUserId(string companyCode, string userAccount);

        /// <summary>
        /// Lock password user
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <returns>bool : true/fals</returns>
        bool IsLockedUser(string companyCode, string userAccount);

        /// <summary>
        /// Get limit input password
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>number record is get</returns>
        int GetLimitInputPassword(string companyCode);

        /// <summary>
        /// Lock user
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userId">userId</param>
        /// <returns>number record is locked</returns>
        int LockUser(string companyCode, int userId);

        /// <summary>
        /// Check password is expired
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userId">userId</param>
        /// <returns>bool : true/false</returns>
        bool IsPasswordExpired(string companyCode, int userId);

        /// <summary>
        /// Get password effective month
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetPasswordEffectiveMonth(string companyCode);

        /// <summary>
        /// Get list function
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_id">user_id</param>
        /// <returns>List function id</returns>
        List<int> getFunctionList(string company_code, int user_id);

        /// <summary>
        /// Get list plan function
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>List plan function id</returns>
        List<int> GetPlanFunctionList(string company_code);

        /// <summary>
        /// Update Password 
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>number of record is update</returns>
        int UpdatePasswordResetManagement(PasswordResetManagement data);

        /// <summary>
        /// Get reissue mail effective time
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>number of record is get</returns>
        int GetReissueMailEffectiveTime(string companyCode);

        /// <summary>
        /// Delete password
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="company_code">company_code</param>
        /// <returns>number of record is deleted</returns>
        int DeletePasswordResetInfo(string email, string company_code);

        /// <summary>
        /// Check License
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>number of license</returns>
        int CheckLicense(string companyCode);
    }
}