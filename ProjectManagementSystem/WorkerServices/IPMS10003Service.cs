using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10003;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices
{
    public interface IPMS10003Service
    {
        /// <summary>
        /// Get basic infomation of company
        /// </summary>
        /// <param name="company_code"></param>
        /// <returns></returns>
        CompanyPlus GetCompanyInfo(string company_code);

        /// <summary>
        /// Update company info
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool EditCompanyInfo(Company company);

        /// <summary>
        /// Get company setting
        /// </summary>
        /// <param name="company_code"></param>
        /// <returns></returns>
        CompanySettingPlus GetCompanySetting(string company_code);

        /// <summary>
        /// Get list of special holiday
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        IList<Holiday> GetListSpecialHoliday(string company_code);

        /// <summary>
        /// Edit company detail
        /// </summary>
        /// <param name="companySetting"></param>
        /// <param name="holidays"></param>
        /// <returns></returns>
        bool EditCompanyDetail(CompanySettingPlus companySetting, IList<Holiday> holidays);

        /// <summary>
        /// Check company email existed
        /// </summary>
        /// <param name="mail_address"></param>
        /// <param name="company_code"></param>
        /// <returns></returns>
        int CheckCompanyEmail(string mail_address, string company_code);
    }
}
