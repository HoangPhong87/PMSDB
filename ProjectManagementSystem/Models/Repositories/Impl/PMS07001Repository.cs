#region License
/// <copyright file="PMS07001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/23</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS07001;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// ConsumptionTax repository class
    /// </summary>
    public class PMS07001Repository : Repository, IPMS07001Repository
    {
        #region Constructor
        /// <summary>
        /// Database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS07001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS07001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Public
        /// <summary>
        /// Search consumption tax by condition
        /// </summary>
        /// <param name="startItem">Start item</param>
        /// <param name="itemsPerPage">Item per page</param>
        /// <param name="columns">List of colum name</param>
        /// <param name="sortCol">Sort by colum</param>
        /// <param name="sortDir">Sort type</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<ConsumptionTaxPlus> Search(int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition)
        {
            var sql = new Sql("SELECT * FROM ( ");

            sql.Append(BuildSqlGetConsumptionTax(companyCode, condition, null));
            sql.Append(") AS tbResult ");

            if (sortCol == 0)
            {
                sortCol = 5;
                sortDir = "desc";
            }

            var pageInfo = Page<ConsumptionTaxPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Export ConsumptionTax List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>ConsumptionTax list</returns>
        public IList<ConsumptionTaxPlus> ExportConsumptionTaxListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlGetConsumptionTax(companyCode, condition, null));
            sql.Append(") AS tbResult ");

            return this._database.Fetch<ConsumptionTaxPlus>(sql);
        }

        /// <summary>
        /// Get consumption tax info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>ConsumptionTax info</returns>
        public ConsumptionTaxPlus GetConsumptionTaxInfo(string companyCode, DateTime applyStartDate)
        {
            var sql = BuildSqlGetConsumptionTax(companyCode, null, applyStartDate);

            return this._database.FirstOrDefault<ConsumptionTaxPlus>(sql);
        }

        /// <summary>
        /// Edit consumption tax info
        /// </summary>
        /// <param name="data">Consumption tax info</param>
        /// <returns>Action result</returns>
        public bool EditConsumptionTaxInfo(ConsumptionTaxPlus data)
        {
            int result = 0;

            if (data.old_apply_start_date.HasValue)
            {
                var sqlUpdate = new Sql(@"
                    UPDATE
                        m_consumption_tax
                    SET
                        apply_start_date = @apply_start_date,
                        tax_rate = @tax_rate,
                        remarks = @remarks,
                        ins_date = @ins_date,
                        ins_id = @ins_id
                    WHERE
                        apply_start_date = @old_apply_start_date
                        AND company_code = @company_code;",
                    new
                    {
                        apply_start_date = data.apply_start_date,
                        tax_rate = data.tax_rate / 100,
                        remarks = data.remarks,
                        ins_date = data.ins_date,
                        ins_id = data.ins_id,
                        company_code = data.company_code,
                        old_apply_start_date = data.old_apply_start_date.Value.ToString("yyyy/MM/dd"),
                    });
                result = this._database.Execute(sqlUpdate);
            }
            else
            {
                var sqlInsert = new Sql(@"
                    INSERT INTO
                        m_consumption_tax
                        (company_code,
                        apply_start_date,
                        tax_rate,
                        remarks,
                        ins_date,
                        ins_id)
                    VALUES
                        (@company_code, @apply_start_date, @tax_rate, @remarks, @ins_date, @ins_id);",
                    new
                    {
                        company_code = data.company_code,
                        apply_start_date = data.apply_start_date,
                        tax_rate = data.tax_rate / 100,
                        remarks = data.remarks,
                        ins_date = data.ins_date,
                        ins_id = data.ins_id
                    });

                result = this._database.Execute(sqlInsert);
            }

            return (result > 0);
        }

        /// <summary>
        /// Count consumption tax by apply start date
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>Number of consumption tax by apply start date</returns>
        public int CountConsumptionTax(string companyCode, DateTime applyStartDate)
        {
            var sql = new Sql(@"
                SELECT
                    COUNT(apply_start_date)
                FROM
                    m_consumption_tax
                WHERE
                    company_code = @company_code
                    AND apply_start_date = @apply_start_date "
                , new { company_code = companyCode, apply_start_date = applyStartDate });

            return this._database.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// Delete consumption tax by apply start date
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="applyStartDate">Apply start date</param>
        /// <returns>Delete consumption tax by apply start date</returns>
        public int DeleteConsumptionTax(string companyCode, DateTime applyStartDate)
        {
            var sql = new Sql(@"
                DELETE
                FROM
                    m_consumption_tax
                WHERE
                    company_code = @company_code
                    AND apply_start_date = @apply_start_date "
                , new { company_code = companyCode, apply_start_date = applyStartDate });

            return this._database.Execute(sql);
        }

        #endregion

        #region Private

        /// <summary>
        /// Build sql get consumption tax list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Sql get consumption tax list</returns>
        private Sql BuildSqlGetConsumptionTax(string companyCode, Condition condition, DateTime? applyStartDate)
        {
            var sql = new Sql(@"
              SELECT 
                tbConsumptionTax.apply_start_date,
                tbConsumptionTax.tax_rate,
                tbConsumptionTax.remarks,
                tbConsumptionTax.ins_date,
                (SELECT display_name FROM m_user WHERE company_code = tbConsumptionTax.company_code AND user_sys_id = tbConsumptionTax.ins_id) AS ins_user
            FROM
                m_consumption_tax AS tbConsumptionTax
            WHERE
                tbConsumptionTax.company_code = @company_code ", new { company_code = companyCode });

            if (condition != null && condition.ApplyStartDate.HasValue)
                sql.Append(string.Format("AND tbConsumptionTax.apply_start_date >= '{0}' ", condition.ApplyStartDate));

            if (applyStartDate != null)
                sql.Append(string.Format("AND tbConsumptionTax.apply_start_date = '{0}' ", applyStartDate.Value.ToString("yyyy/MM/dd")));

            return sql;
        }

        #endregion
    }
}
