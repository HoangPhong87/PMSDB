using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS01001;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS01001Repository : Repository, IPMS01001Repository
    {
        private Database _database;

        /// <summary>
        /// Contructor
        /// </summary>
        public PMS01001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">database</param>
        public PMS01001Repository(PMSDatabase database)
        {
            _database = database;
        }

        /// <summary>
        ///  Get information user - login into system
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <param name="password">password</param>
        /// <returns>UserPlus</returns>
        public UserPlus GetUserInfo(string companyCode, string userAccount, string password)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT
                    ts1.*,
                    ts2.logo_image_file_path [company_logo_img_path],
                    ts3.decimal_calculation_type,
                    ts3.working_time_unit_minute,
                    ts3.data_editable_time,
                    (
                        SELECT 
                            COUNT(*)
                        FROM 
                            m_user mu JOIN m_company_setting mcs
                            ON mu.company_code = mcs.company_code
                        WHERE
                            DATEADD(MONTH,mcs.password_effective_month,mu.password_last_update) <= @CurrentDate
                            AND mu.company_code = @company_code 
                            AND mu.user_account = @user_account
                    ) AS is_expired_password,
                    ts2.display_name AS company_name,
                    tbGroup.group_name
                FROM
                    m_user ts1 LEFT JOIN m_company ts2
                    ON ts2.company_code = ts1.company_code INNER JOIN m_company_setting ts3
                    ON ts3.company_code = ts2.company_code
                    LEFT JOIN m_group tbGroup
                        ON tbGroup.company_code = ts1.company_code
                        AND tbGroup.group_id = ts1.group_id
                WHERE ts1.company_code = @company_code
                    AND ts1.user_account = @user_account
                    AND ts1.password = @password
                    AND ts1.del_flg = @del_flg
                    AND (ts1.retirement_date IS NULL OR ts1.retirement_date > @CurrentDate)"
            , new
            {
                company_code = companyCode,
                user_account = userAccount,
                password = password,
                CurrentDate = Utility.GetCurrentDateTime(),
                del_flg = Constant.DeleteFlag.NON_DELETE
            });

            return _database.SingleOrDefault<UserPlus>(sql);
        }

        /// <summary>
        /// Get list user search by mail
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>IList</returns>
        public IList<UserEmail> GetEmailUser(string email)
        {
            Sql sql = Sql.Builder.Append(@"Select u.user_sys_id, u.company_code, u.user_account, u.mail_address_1, u.mail_address_2 , m.password_lock_target from m_user u
                                            INNER JOIN m_authority_role m
	                                            ON u.company_code = m.company_code AND u.role_id = m.role_id AND m.del_flg = 0
                                            WHERE (u.mail_address_1 = @mail_address_1 or u.mail_address_2 = @mail_address_2) and u.del_flg = @del_flg
                                            AND (u.retirement_date IS NULL OR u.retirement_date >= @CurrentDate)"
           , new { mail_address_1 = email, mail_address_2 = email, CurrentDate = Utility.GetCurrentDateTime(), del_flg = Constant.DeleteFlag.NON_DELETE });

            return _database.Fetch<UserEmail>(sql);
        }

        /// <summary>
        /// Get user search by mail
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="company_code">company_code</param>
        /// <returns>UserEmail</returns>
        public UserEmail GetEmailUser(string email, string company_code)
        {
            Sql sql = Sql.Builder.Append(@"Select u.user_sys_id, u.company_code, u.user_account, u.mail_address_1, u.mail_address_2 , m.password_lock_target from m_user u
                                            INNER JOIN m_authority_role m
	                                            ON u.company_code = m.company_code AND u.role_id = m.role_id AND m.del_flg = 0
                                            WHERE u.company_code = @company_code
                                            AND (u.mail_address_1 = @mail_address_1 or u.mail_address_2 = @mail_address_2) and u.del_flg = @del_flg
                                            AND (u.retirement_date IS NULL OR u.retirement_date >= @CurrentDate)"
           , new { mail_address_1 = email, mail_address_2 = email, del_flg = Constant.DeleteFlag.NON_DELETE, CurrentDate = Utility.GetCurrentDateTime(), company_code = company_code });

            return _database.SingleOrDefault<UserEmail>(sql);
        }

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="password">password</param>
        /// <param name="password_lock_target">password_lock_target</param>
        /// <param name="company_code">company_code</param>
        /// <returns>int</returns>
        public int UpdatePassword(int userId, string password, string password_lock_target, string company_code)
        {
            IDictionary<string, object> columns = null;

            if (password_lock_target == "0")
            {
                columns = new Dictionary<string, object>()
                {
                    { "password", password },
                    { "password_last_update", Utility.GetCurrentDateTime()},
                    { "password_lock_flg", "0"}
                };
            }
            else
            {
                columns = new Dictionary<string, object>()
                {
                    { "password", password },
                    { "password_last_update", Utility.GetCurrentDateTime()}
                };
            }

            IDictionary<string, object> condition = new Dictionary<string, object>()
            {
                { "user_sys_id", userId },
                { "company_code", company_code }
            };

            return Update<User>(columns, condition);
        }

        /// <summary>
        /// Check exist of user
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <returns>IsExistedUserId</returns>
        public int? IsExistedUserId(string companyCode, string userAccount)
        {
            Sql sql = Sql.Builder.Append(@"SELECT user_sys_id from m_user WHERE company_code = @company_code and user_account = @user_account and del_flg = @del_flg"
           , new { company_code = companyCode, user_account = userAccount, del_flg = Constant.DeleteFlag.NON_DELETE });

            return _database.SingleOrDefault<int?>(sql);
        }

        /// <summary>
        /// Check password of user is locked
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userAccount">userAccount</param>
        /// <returns>bool: true/false</returns>
        public bool IsLockedUser(string companyCode, string userAccount)
        {
            Sql sql = Sql.Builder.Append(@"SELECT password_lock_flg FROM m_user WHERE company_code = @companyCode and user_account = @user_account and del_flg = @del_flg"
           , new { companyCode = companyCode, user_account = userAccount, del_flg = Constant.DeleteFlag.NON_DELETE });

            return _database.SingleOrDefault<int>(sql) == 1;
        }

        /// <summary>
        /// Get number password is locked of user
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>int</returns>
        public int GetLimitInputPassword(string companyCode)
        {
            Sql sql = Sql.Builder.Append(@"SELECT password_input_limit from m_company_setting WHERE company_code = @companyCode "
           , new { companyCode = companyCode });

            return _database.SingleOrDefault<int>(sql);
        }

        /// <summary>
        /// Lock user
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userId">userId</param>
        /// <returns>int</returns>
        public int LockUser(string companyCode, int userId)
        {
            IDictionary<string, object> columns = new Dictionary<string, object>()
            {
                { "password_lock_flg", Constant.UserLockFlag.LOCKED }
            };

            IDictionary<string, object> condition = new Dictionary<string, object>()
            {
                { "company_code", companyCode } , 
                { "user_sys_id", userId } , 
                { "del_flg", Constant.DeleteFlag.NON_DELETE }
            };

            // 担当者マスタのログイン日時を更新
            return Update<User>(columns, condition);
        }

        /// <summary>
        /// Check user is expired
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="userId">userId</param>
        /// <returns>bool: true/false</returns>
        public bool IsPasswordExpired(string companyCode, int userId)
        {
            Sql sql = Sql.Builder.Append(
                @"
                SELECT COUNT(*)
                from m_user mu 
                    join m_company_setting mcs
                on mu.company_code = mcs.company_code
                WHERE 
                    DATEADD(MONTH,mcs.password_effective_month,mu.password_last_update) <= @CurrentDate 
                    and mu.company_code = @company_code 
                    and mu.user_sys_id = @user_sys_id",
                new
                {
                    company_code = companyCode,
                    user_sys_id = userId,
                    CurrentDate = Utility.GetCurrentDateTime()
                }
        );

            return _database.SingleOrDefault<int>(sql) == 1;
        }

        /// <summary>
        /// Get password effective with month
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>int</returns>
        public int GetPasswordEffectiveMonth(string companyCode)
        {
            Sql sql = Sql.Builder.Append(@"
                    SELECT password_effective_month
                    from m_company_setting ");
            sql.Where("company_code = @company_code", new { @company_code = companyCode });

            return _database.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Get function list of user witch login
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_id">user_id</param>
        /// <returns>List<int></returns>
        public List<int> getFunctionList(string company_code, int user_id)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT
                    function_id
                from
                    target_function
                WHERE
                    company_code = @company_code
                and role_id =  (SELECT role_id from m_user WHERE user_sys_id = @userSysId)
            ", new { @company_code = company_code }, new { @userSysId = user_id });

            return _database.Fetch<int>(sql);
        }

        /// <summary>
        /// Get Plan Function List
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>List<int></returns>
        public List<int> GetPlanFunctionList(string company_code)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT
                    tbFunction.function_id
                FROM
                    contract_plan_info tbContract
                    LEFT JOIN plan_function_setting tbFunction
                        ON tbContract.plan_id = tbFunction.plan_id
                        AND tbContract.version_no = tbFunction.version_no
                WHERE
                    tbContract.company_code = @company_code
            ", new { @company_code = company_code });

            return _database.Fetch<int>(sql);
        }

        /// <summary>
        /// Get Password Reset Info
        /// </summary>
        /// <param name="param">param</param>
        /// <returns>Obj ResetPass</returns>
        public ResetPass GetPasswordResetInfo(string param)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT mail_address, apply_time, company_code from password_reset_management
                WHERE parameter_value = @param
            ", new { param = param });

            return _database.SingleOrDefault<ResetPass>(sql);
        }

        /// <summary>
        /// Get Email In PasswordResetManagement Table
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>number of record with email</returns>
        private int GetEmailInPasswordResetManagementTable(string email)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT Count(*) from password_reset_management WHERE mail_address = @mail_address
            ", new { mail_address = email });

            return _database.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Update Password ResetManagement table
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>Number record is update</returns>
        public int UpdatePasswordResetManagement(PasswordResetManagement data)
        {
            int res = 0;
            Sql sql;

            if (GetEmailInPasswordResetManagementTable(data.mail_address) > 0)
            {
                IDictionary<string, object> condition = new Dictionary<string, object>()
                 {
                     { "mail_address", data.mail_address}
                 };

                if (Delete<PasswordResetManagement>(condition) > 0)
                {
                    sql = new Sql(@"
                        INSERT INTO [dbo].[password_reset_management]
                        (
                            mail_address
                            ,parameter_value
                            ,apply_time
                            ,company_code
                        )
                         VALUES
                        (
                            @mail_address
                            ,@parameter_value
                            ,@apply_time
                            ,@company_code
                        )"
                        , new { mail_address = data.mail_address }
                        , new { parameter_value = data.parameter_value }
                        , new { apply_time = data.apply_time }
                        , new { company_code = data.company_code }
                    );

                    if (_database.Execute(sql) > 0)
                    {
                        var query = "select ident_current('password_reset_management')";
                        res = _database.ExecuteScalar<int>(query);
                    }
                }
            }
            else
            {
                sql = new Sql(@"
                    INSERT INTO [dbo].[password_reset_management]
                    (
                        mail_address
                        ,parameter_value
                        ,apply_time
                        ,company_code
                    )
                     VALUES
                    (
                        @mail_address
                        ,@parameter_value
                        ,@apply_time
                        ,@company_code
                    )"
                    , new { mail_address = data.mail_address }
                    , new { parameter_value = data.parameter_value }
                    , new { apply_time = data.apply_time }
                    , new { company_code = data.company_code }
                );

                if (_database.Execute(sql) > 0)
                {
                    var query = "select ident_current('password_reset_management')";
                    res = _database.ExecuteScalar<int>(query);
                }
            }

            return res;
        }

        /// <summary>
        /// Get Reissue Mail Effective with Time
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>number of record</returns>
        public int GetReissueMailEffectiveTime(string companyCode)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT reissue_mail_effective_time from m_company_setting WHERE company_code = @companyCode
            ", new { companyCode = companyCode });

            return _database.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Delete Password Reset Info
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="company_code">company_code</param>
        /// <returns>number of record is deleted</returns>
        public int DeletePasswordResetInfo(string email, string company_code)
        {
            IDictionary<string, object> condition = new Dictionary<string, object>()
             {
                 { "mail_address", email},
                 { "company_code", company_code}
             };

            return Delete<PasswordResetManagement>(condition);
        }

        /// <summary>
        /// Check License
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <returns>number</returns>
        public int CheckLicense(string companyCode)
        {
            Sql sql = Sql.Builder.Append(@"
                 SELECT COUNT(*) FROM contract_plan_info");

            sql.Where("company_code = @company_code", new { company_code = companyCode });
            sql.Where("invalid_flg = @invalid_flg", new { invalid_flg = Constant.DeleteFlag.NON_DELETE });
            sql.Where("contract_start_time <= @CurrentDate", new { CurrentDate = Utility.GetCurrentDateTime()});
            sql.Where("contract_end_time >= @CurrentDate", new { CurrentDate = Utility.GetCurrentDateTime() });

            return _database.ExecuteScalar<int>(sql);
        }
    }
}