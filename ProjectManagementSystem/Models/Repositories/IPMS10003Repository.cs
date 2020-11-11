using ProjectManagementSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10003;

namespace ProjectManagementSystem.Models.Repositories
{
    public interface IPMS10003Repository
    {
         /// <summary>
        /// Get basic infomation of company
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>Company Info</returns>
        CompanyPlus GetCompanyInfo(string company_code);

        /// <summary>
        /// Update company info
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>bool : true/false</returns>
        bool EditCompanyInfo(Company company);

        /// <summary>
        /// Get company setting
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>Company Setting</returns>
        CompanySettingPlus GetCompanySetting(string company_code);

        /// <summary>
        /// Get list of special holiday
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>List Specia holiday</returns>
        IList<Holiday> GetListSpecialHoliday(string company_code);

        /// <summary>
        /// Update company setting
        /// </summary>
        /// <param name="data">companySetting</param>
        /// <returns>bool: true/false</returns>
        bool EditCompanySetting(CompanySettingPlus companySetting);

        /// <summary>
        /// Update special holiday
        /// </summary>
        /// <param name="holidays">holidays</param>
        /// <param name="company_code">company_code</param>
        /// <returns>bool: true/false</returns>
        bool EditSpecialHoliday(IList<Holiday> holidays, string company_code);

        /// <summary>
        /// Check company email existed
        /// </summary>
        /// <param name="mail_address">mail_address</param>
        /// <param name="company_code">company_code</param>
        /// <returns>number of record is get</returns>
        int CheckCompanyEmail(string mail_address, string company_code);
    }
}
