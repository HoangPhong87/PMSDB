using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS01001;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.WorkerServices
{
    /// <summary>
    /// ログイン画面サービスインターフェイス
    /// </summary>
    public interface IPMS01001Service
    {
        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="loginId">ログインID</param>
        /// <param name="password">パスワード</param>
        /// <returns>ログインユーザ情報</returns>
        LoginUser Login(string companyCode, string userAccount, string password);

        IList<UserEmail> CheckEmail(string email);
        UserEmail CheckEmail(string email, string companyCode);

        ResetPass GetPasswordResetInfo(string param);

        int UpdatePassword(int userId, string password, string password_lock_target, string company_code);

        int? IsExistedUserId(string companyCode, string userAccount);

        bool IsLockedUser(string companyCode, string userAccount);

        int GetLimitInputPassword(string companyCode);

        int LockUser(string companyCode,int userId);

        bool IsPasswordExpired(string companyCode, int userId);

        int GetPasswordEffectiveMonth(string companyCode);

        int UpdatePasswordResetManagement(string mail_address, string parameter_value, DateTime apply_time, string company_Code);

        int GetReissueMailEffectiveTime(string companyCode);

        int DeletePasswordResetInfo(string email, string companyCode);

        int CheckLicense(string companyCode);
    }
}
