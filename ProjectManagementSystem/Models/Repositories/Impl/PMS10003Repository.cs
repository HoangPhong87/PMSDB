using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10003;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS10003Repository : Repository, IPMS10003Repository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10003Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS10003Repository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// Get basic infomation of company
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>Company Info</returns>
        public CompanyPlus GetCompanyInfo(string company_code)
        {
            var sql = new Sql(@"
            SELECT 
                comp.*,
                (SELECT display_name FROM m_user WHERE user_sys_id = comp.upd_id ) [user_update],
                (SELECT display_name FROM m_user WHERE user_sys_id = comp.ins_id ) [user_regist]
            FROM
                m_company comp
            WHERE
                comp.company_code = @company_code",
                new
                {
                    company_code = company_code
                }
            );
            return this._database.SingleOrDefault<CompanyPlus>(sql);
        }

        /// <summary>
        /// Update company info
        /// </summary>
        /// <param name="data">company</param>
        /// <returns>bool: true/false</returns>
        public bool EditCompanyInfo(Company company)
        {
            IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                    { "company_name", string.IsNullOrEmpty(company.company_name) ? null : company.company_name }
                    , { "company_name_kana", string.IsNullOrEmpty(company.company_name_kana) ? null : company.company_name_kana }
                    , { "company_name_en", string.IsNullOrEmpty(company.company_name_en) ? null : company.company_name_en }
                    , { "display_name", string.IsNullOrEmpty(company.display_name) ? null : company.display_name }
                    , { "zip_code", string.IsNullOrEmpty(company.zip_code) ? null : company.zip_code }
                    , { "prefecture_code", string.IsNullOrEmpty(company.prefecture_code) ? null : company.prefecture_code }
                    , { "city", string.IsNullOrEmpty(company.city) ? null : company.city }
                    , { "address_1", string.IsNullOrEmpty(company.address_1) ? null : company.address_1 }
                    , { "address_2", string.IsNullOrEmpty(company.address_2) ? null : company.address_2 }
                    , { "tel_no", string.IsNullOrEmpty(company.tel_no) ? null : company.tel_no }
                    , { "fax_no", string.IsNullOrEmpty(company.fax_no) ? null : company.fax_no }
                    , { "mail_address", string.IsNullOrEmpty(company.mail_address) ? null : company.mail_address }
                    , { "ip_address_1", string.IsNullOrEmpty(company.ip_address_1) ? null : company.ip_address_1 }
                    , { "ip_address_2", string.IsNullOrEmpty(company.ip_address_2) ? null : company.ip_address_2 }
                    , { "url", string.IsNullOrEmpty(company.url) ? null : company.url }
                    , { "establishment_date", company.establishment_date }
                    , { "capital", company.capital}
                    , { "representative", string.IsNullOrEmpty(company.representative) ? null : company.representative }
                    , { "employee_number", company.employee_number}
                    , { "logo_image_file_path", string.IsNullOrEmpty(company.logo_image_file_path) ? null : company.logo_image_file_path }
                    , { "remarks", string.IsNullOrEmpty(company.remarks) ? null : company.remarks }
                    , { "upd_date", company.upd_date }
                    , { "upd_id", company.upd_id }
                    , { "del_flg", company.del_flg }
                };

            IDictionary<string, object> condition;
            condition = new Dictionary<string, object>()
                {
                    { "company_code", company.company_code }
                };

            return Update<Company>(columns, condition) > 0;
        }

        /// <summary>
        /// Get company setting
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>Company Setting</returns>
        public CompanySettingPlus GetCompanySetting(string company_code)
        {
            var sql = new Sql(@"
            SELECT
                *,
                (SELECT display_name FROM m_user WHERE user_sys_id = msc.upd_id ) [user_update],
                (SELECT display_name FROM m_user WHERE user_sys_id = msc.ins_id ) [user_regist]
            FROM
                m_company_setting msc
            WHERE msc.company_code = @company_code",
                new
                {
                    company_code = company_code
                }
            );
            return this._database.SingleOrDefault<CompanySettingPlus>(sql);
        }

        /// <summary>
        /// Get list of special holiday
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>List Special holiday</returns>
        public IList<Holiday> GetListSpecialHoliday(string company_code)
        {
            var sql = new Sql(@"
            SELECT
                *
            FROM
                m_holiday
            WHERE company_code = @company_code",
                new
                {
                    company_code = company_code
                }
            );
            return this._database.Fetch<Holiday>(sql);
        }

        /// <summary>
        /// Update company setting
        /// </summary>
        /// <param name="data">companySetting</param>
        /// <returns>bool : true/false</returns>
        public bool EditCompanySetting(CompanySettingPlus companySetting)
        {
            IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                    { "default_holiday_type", string.IsNullOrEmpty(companySetting.default_holiday_type) ? null : companySetting.default_holiday_type }
                    , { "check_point_week", companySetting.check_point_week }
                    , { "default_work_time_days", companySetting.default_work_time_days }
                    , { "password_input_limit", companySetting.password_input_limit }
                    , { "password_effective_month", companySetting.password_effective_month }
                    , { "decimal_calculation_type", companySetting.decimal_calculation_type }
                    , { "working_time_unit_minute", companySetting.working_time_unit_minute }
                    , { "account_closing_month", companySetting.account_closing_month }
                    , { "work_closing_date", companySetting.work_closing_date }
                    , { "upd_date", companySetting.upd_date }
                    , { "upd_id", companySetting.upd_id }
                };

            IDictionary<string, object> condition;
            condition = new Dictionary<string, object>()
                {
                    { "company_code", companySetting.company_code }
                };

            return Update<CompanySetting>(columns, condition) > 0;
        }

        /// <summary>
        /// Update special holiday
        /// </summary>
        /// <param name="holidays">holidays</param
        /// <param name="company_code">company_code</param
        /// <returns>bool: true/false</returns>
        public bool EditSpecialHoliday(IList<Holiday> holidays, string company_code)
        {
            var result = true;
            // Delete old data
            var sqlDelete = new Sql(
               @"
                DELETE FROM
                    m_holiday
                WHERE
                    company_code = @company_code",
               new
               {
                   company_code = company_code
               });
            this._database.Execute(sqlDelete);

            // Insert new data
            var sqlInsert = new Sql();
            foreach (var holiday in holidays)
            {
                if(holiday.holiday_date != null)
                {
                    sqlInsert.Append(@"
                    INSERT INTO 
                        m_holiday
                        (company_code,
                        holiday_date,
                        holiday_name,
                        remarks,
                        ins_date,
                        ins_id)
                    VALUES
                        (@company_code, @holiday_date, @holiday_name, @remarks, @ins_date, @ins_id);",
                    new
                    {
                        company_code = company_code,
                        holiday_date = holiday.holiday_date,
                        holiday_name = holiday.holiday_name,
                        remarks = holiday.remarks,
                        ins_date = holiday.ins_date,
                        ins_id = holiday.ins_id
                    });
                }
            }
            if(sqlInsert.SQL != ""){
                result = this._database.Execute(sqlInsert) > 0;
            }

            return result;
        }

        /// <summary>
        /// Check company email existed
        /// </summary>
        /// <param name="mail_address">mail_address</param>
        /// <param name="company_code">company_code</param>
        /// <returns>number of record company email</returns>
        public int CheckCompanyEmail(string mail_address, string company_code)
        {
            Sql sql = new Sql();
            sql.Append(@"
                SELECT
                    COUNT(*)
                FROM
                    m_company
                WHERE
                    mail_address = @mail_address
                    AND company_code <> @company_code
            ", new { 
                mail_address = mail_address,
                company_code = company_code
             });

            return _database.SingleOrDefault<int>(sql);
        }
    }
}