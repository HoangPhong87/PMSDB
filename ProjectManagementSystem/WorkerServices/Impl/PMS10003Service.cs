using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10003;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using ProjectManagementSystem.ViewModels;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    public class PMS10003Service : IPMS10003Service
    {
        /// <summary>
        /// </summary>
        private IPMS10003Repository _repository;

        /// <summary>
        /// </summary>
        public PMS10003Service()
            : this(new PMS10003Repository())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="_repository"></param>
        public PMS10003Service(IPMS10003Repository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get basic infomation of company
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>Company info</returns>
        public CompanyPlus GetCompanyInfo(string company_code)
        {
            return this._repository.GetCompanyInfo(company_code);
        }

        /// <summary>
        /// Update company info
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>bool : true/false</returns>
        public bool EditCompanyInfo(Company company)
        {
            return this._repository.EditCompanyInfo(company);
        }

        /// <summary>
        /// Get company setting
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>Company Setting</returns>
        public CompanySettingPlus GetCompanySetting(string company_code)
        {
            return this._repository.GetCompanySetting(company_code);
        }

        /// <summary>
        /// Get list of special holiday
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>List Special Holiday</returns>
        public IList<Holiday> GetListSpecialHoliday(string company_code)
        {
            return this._repository.GetListSpecialHoliday(company_code);
        }

        /// <summary>
        /// Edit company detail
        /// </summary>
        /// <param name="companySetting">companySetting</param>
        /// <param name="holidays">holidays</param>
        /// <returns>bool : true/false</returns>
        public bool EditCompanyDetail(CompanySettingPlus companySetting, IList<Holiday> holidays)
        {
            bool res, res2;
            using (var transaction = new TransactionScope())
            {
                res = this._repository.EditCompanySetting(companySetting);
                res2 = this._repository.EditSpecialHoliday(holidays, companySetting.company_code);

                if (res && res2)
                    transaction.Complete();
            }

            return res && res2;
        }

        /// <summary>
        /// Check company email existed
        /// </summary>
        /// <param name="mail_address">mail_address</param>
        /// <param name="company_code">company_code</param>
        /// <returns>record email company</returns>
        public int CheckCompanyEmail(string mail_address, string company_code)
        {
            return this._repository.CheckCompanyEmail(mail_address, company_code);
        }
    }
}