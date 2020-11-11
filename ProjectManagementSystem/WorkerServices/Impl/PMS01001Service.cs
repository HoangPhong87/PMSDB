using System.Collections.Generic;
using System.Transactions;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.Models.PMS01001;
using System;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// ログイン画面サービスクラス
    /// </summary>
    public class PMS01001Service : IPMS01001Service
    {
        private readonly IPMS01001Repository _repository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PMS01001Service()
            : this(new PMS01001Repository())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository">リポジトリクラス</param>
        public PMS01001Service(IPMS01001Repository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="loginId">ログインID</param>
        /// <param name="password">パスワード</param>
        /// <returns>ログインユーザ情報</returns>
        public LoginUser Login(string companyCode, string userAccount, string password)
        {
            LoginUser loginUser = null;

            // ログインIDに該当する担当者情報を取得する
            var user = _repository.GetUserInfo(companyCode, userAccount, password);

            if (user != null)
            {
                var functionList = _repository.getFunctionList(companyCode, user.user_sys_id);
                var planFunctionList = _repository.GetPlanFunctionList(companyCode);

                loginUser = new LoginUser()
                {
                    UserId = user.user_sys_id,
                    CompanyCode = user.company_code,
                    UserAccount = user.user_account,
                    Password = user.password,
                    UserNameMei = user.user_name_mei,
                    UserNameSei = user.user_name_sei,
                    FuriganaMei = user.furigana_mei,
                    FuriganaSei = user.furigana_sei,
                    DisplayName = user.display_name,
                    GroupId = user.group_id,
                    GroupName = user.group_name,
                    PositionId = user.position_id,
                    CompanyLogoImgPath = user.company_logo_img_path,
                    RoleId = user.role_id,
                    DecimalCalculationType = user.decimal_calculation_type,
                    FunctionList = functionList,
                    PlanFunctionList = planFunctionList,
                    ActualWorkInputMode = user.actual_work_input_mode,
                    Working_time_unit_minute = user.working_time_unit_minute,
                    Is_expired_password = (user.is_expired_password == 1),
                    CompanyName = user.company_name,
                    DataEditTableTime = user.data_editable_time,
                    ImageFilePath = user.image_file_path
                };
            }
            return loginUser;
        }

        /// <summary>
        /// Lock password user
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <returns>bool : true/fals</returns>
        public int LockUser(string companyCode, int userId)
        {
            return this._repository.LockUser(companyCode, userId);
        }

        /// <summary>
        /// Check exist user id by company code and user account
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <returns>number of record is existed</returns>
        public int? IsExistedUserId(string companyCode, string userAccount)
        {
            return this._repository.IsExistedUserId(companyCode, userAccount);
        }

        /// <summary>
        /// Lock password user
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <returns>bool : true/fals</returns>
        public bool IsLockedUser(string companyCode, string userAccount)
        {
            return this._repository.IsLockedUser(companyCode, userAccount);
        }

        /// <summary>
        /// Get limit input password
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>number record is get</returns>
        public int GetLimitInputPassword(string companyCode)
        {
            return this._repository.GetLimitInputPassword(companyCode);
        }

        /// <summary>
        /// Get user by mail
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>List User</returns>
        public IList<UserEmail> CheckEmail(string email)
        {
            return _repository.GetEmailUser(email);
        }

        /// <summary>
        /// Get user by mail and company code
        /// </summary>
        /// <param name="email">mail</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>User</returns>
        public UserEmail CheckEmail(string email, string companyCode)
        {
            return _repository.GetEmailUser(email, companyCode);
        }

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="password">password</param>
        /// <param name="password_lock_target">password_lock_target</param>
        /// <param name="company_code">company_code</param>
        /// <returns>number of record is update</returns>
        public int UpdatePassword(int userId, string password, string password_lock_target, string company_code)
        {
            return _repository.UpdatePassword(userId, password, password_lock_target, company_code);
        }

        /// <summary>
        /// Check password is expired
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userId">userId</param>
        /// <returns>bool : true/false</returns>
        public bool IsPasswordExpired(string companyCode, int userId)
        {
            return _repository.IsPasswordExpired(companyCode, userId);
        }

        /// <summary>
        /// Get password effective month
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetPasswordEffectiveMonth(string companyCode)
        {
            return _repository.GetPasswordEffectiveMonth(companyCode);
        }

        /// <summary>
        /// Get password
        /// </summary>
        /// <param name="param">param</param>
        /// <returns>Resset pass</returns>
        public ResetPass GetPasswordResetInfo(string param)
        {
            return _repository.GetPasswordResetInfo(param);
        }

        /// <summary>
        /// Update Password 
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>number of record is update</returns>
        public int UpdatePasswordResetManagement(string mail_address, string parameter_value, DateTime apply_time, string company_Code)
        {
            PasswordResetManagement data = new PasswordResetManagement()
            {
                mail_address = mail_address,
                parameter_value = parameter_value,
                apply_time = apply_time,
                company_code = company_Code
            };
            return _repository.UpdatePasswordResetManagement(data);
        }

        /// <summary>
        /// Get reissue mail effective time
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>number of record is get</returns>
        public int GetReissueMailEffectiveTime(string companyCode)
        {
            return _repository.GetReissueMailEffectiveTime(companyCode);
        }

        /// <summary>
        /// Delete password
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="company_code">company_code</param>
        /// <returns>number of record is deleted</returns>
        public int DeletePasswordResetInfo(string email, string company_code)
        {
            return _repository.DeletePasswordResetInfo(email, company_code);
        }

        /// <summary>
        /// Check License
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>number of license</returns>
        public int CheckLicense(string companyCode)
        {
            return _repository.CheckLicense(companyCode);
        }
    }
}