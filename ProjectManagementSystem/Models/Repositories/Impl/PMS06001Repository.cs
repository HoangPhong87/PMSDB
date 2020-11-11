#region License
/// <copyright file="PMS06001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    using PetaPoco;
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06001;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;

    /// <summary>
    /// Project screen repository class
    /// </summary>
    public class PMS06001Repository : Repository, IPMS06001Repository
    {
        #region Constructor
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS06001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS06001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Public Method
        #region Project List
        /// <summary>
        /// Search project by condition
        /// </summary>
        /// <param name="startItem">Start item</param>
        /// <param name="itemsPerPage">Item per page</param>
        /// <param name="columns">List of colum name</param>
        /// <param name="sortCol">Sort by colum</param>
        /// <param name="sortDir">Sort type</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public IList<ProjectInfoPlus> Search(int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition)
        {
            string orderBy = string.Empty;

            string firstColumns = "project_sys_id,status_order,project_name,";

            string myString = columns.Remove(columns.IndexOf(firstColumns), firstColumns.Length);

            columns = firstColumns + myString;

            if (sortCol.HasValue && !string.IsNullOrEmpty(columns))
            {
                if (sortCol == 0)
                {
                    sortDir = "asc";
                    sortCol = 1;
                }

                string[] sCol = columns.Split(',');

                if (sortCol < sCol.Length && !string.IsNullOrEmpty(sCol[sortCol.Value]))
                    orderBy = sCol[sortCol.Value];
            }

            var sb = new StringBuilder();

            sb.AppendFormat(@"SELECT * FROM (
                SELECT ROW_NUMBER() OVER (ORDER BY {0} {1}) peta_rn, tbData.*", orderBy, sortDir);
            sb.AppendFormat(@"FROM (
                SELECT
                    tbProject.project_sys_id
                    , tbProject.project_no
                    , tbStatus.sales_type
                    , tbProject.upd_date
                    , tbProject.project_name
                    , (SELECT contract_type
                        FROM m_contract_type
                        WHERE company_code = tbProject.company_code
                            AND contract_type_id = tbProject.contract_type_id
                        ) AS contract_type
                    , (SELECT rank 
                        FROM m_rank
                        WHERE company_code = tbProject.company_code
                            AND rank_id = tbProject.rank_id
                        ) AS rank
                    , tbProject.start_date
                    , tbProject.end_date
                    , tbProject.acceptance_date
                    , tbUser.display_name AS charge_person
                    , tbStatus.status
                    , tbProject.total_sales
                    --HLQ Update
                    , tbPlan.total_plan_cost
                    , tbActual.actual_cost
                    , tbProject.total_payment
                    --HLQ Update end
                    , tbProject.estimate_man_days
                    , ISNULL(tbActual.actual_man_day, 0) AS actual_man_day
                    , (SELECT TOP(1) progress
                        FROM progress_history
                        WHERE company_code = tbProject.company_code
                            AND project_sys_id = tbProject.project_sys_id
                        ORDER BY regist_date DESC
                        ) AS progress
                    , (SELECT TOP(1) regist_date
                        FROM progress_history
                        WHERE company_code = tbProject.company_code
                            AND project_sys_id = tbProject.project_sys_id
                        ORDER BY regist_date DESC
                        ) AS progress_regist_date
                    , CASE
                        WHEN tbProject.total_sales = 0 THEN 0 
                        ELSE ((tbProject.total_sales - ISNULL(tbProject.total_payment, 0) - ISNULL(tbPlan.total_plan_cost, 0)) / tbProject.total_sales)
                        END AS plan_profit
                    , CASE
                        WHEN tbProject.total_sales = 0 THEN 0
                        ELSE ((tbProject.total_sales - ISNULL(tbProject.total_payment, 0) - ISNULL(tbActual.actual_cost, 0)) / tbProject.total_sales)
                        END AS actual_profit
                    , (SELECT display_name
                        FROM m_customer
                        WHERE company_code = tbProject.company_code
                            AND customer_id = tbSales.customer_id
                        ) AS customer_name
                    , (SELECT display_name
                        FROM m_user
                        WHERE company_code = tbProject.company_code
                            AND user_sys_id = tbProject.upd_id
                        ) AS upd_user
                    , tbProject.del_flg
                    , (SELECT display_order
                        FROM m_contract_type
                        WHERE company_code = tbProject.company_code
                            AND contract_type_id = tbProject.contract_type_id
                        ) AS contract_type_order
                    , (SELECT display_order
                        FROM m_rank
                        WHERE company_code = tbProject.company_code
                            AND rank_id = tbProject.rank_id
                        ) AS rank_order
                    , ISNULL(tbUser.furigana_sei, '') + ISNULL(tbUser.furigana_mei,'') + ISNULL(tbUser.display_name,'') AS pic_order
                    , tbStatus.display_order AS status_order
                    , tbProject.assign_fix_date
                    , tbProjectHistory.estimate_man_days AS history_estimate_man_days
                    , tbProjectHistory.total_sales AS history_total_sales
                    , tbProjectHistory.gross_profit AS history_gross_profit
                    , (select count(ppi.company_code) from project_plan_info as ppi where tbProject.company_code = ppi.company_code and tbProject.project_sys_id = ppi.project_sys_id  and  ppi.del_flg ='0') as count_plan
                FROM
                    project_info AS tbProject
                    INNER JOIN m_user AS tbUser
                        ON tbProject.company_code = tbUser.company_code
                        AND tbProject.charge_person_id = tbUser.user_sys_id
                    INNER JOIN m_status AS tbStatus
                        ON tbProject.company_code = tbStatus.company_code
                        AND tbProject.status_id = tbStatus.status_id
                    LEFT JOIN sales_payment AS tbSales
                        ON tbProject.company_code = tbSales.company_code
                        AND tbProject.project_sys_id = tbSales.project_sys_id
                        AND tbSales.ordering_flg = '1'
                    LEFT JOIN (SELECT company_code
                            , project_sys_id
                            , SUM(work_time_in_month) AS actual_man_day
                            , SUM(actual_cost_in_month) AS actual_cost
                        FROM (SELECT tbWorkTime.company_code
                                , tbWorkTime.project_sys_id
                                , tbWorkTime.user_sys_id
                                , tbWorkTime.work_time_in_month
                                , ISNULL(dbo.RoundNumber('{0}', (
                                    (SELECT TOP 1 base_unit_cost 
				                     FROM unit_price_history 
				                     WHERE company_code = '{0}'
					                    AND user_sys_id = tbWorkTime.user_sys_id 
					                    AND apply_start_date <= CONVERT(date, CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)), 2) + '/01')
					                    AND del_flg = '0'
				                     ORDER BY apply_start_date DESC)
                                    * (tbWorkTime.work_time_in_month / tbOperatingDay.operating_days)))
                                    , 0) AS actual_cost_in_month
                            FROM (SELECT company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , actual_work_month
                                    , actual_work_year
                                    ,  dbo.Hour2Day('{0}', SUM(actual_work_time)) AS work_time_in_month
                                FROM member_actual_work_detail
                                WHERE company_code = '{0}'
                                GROUP BY company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , actual_work_month, actual_work_year
                                ) AS tbWorkTime
                                INNER HASH JOIN (
                                    SELECT *
                                    FROM dbo.GetTableDaysInMonths('{0}'
                                        , (SELECT MIN(actual_work_year) FROM member_actual_work_detail WHERE company_code = '{0}')
                                        , (SELECT MIN(actual_work_month) FROM member_actual_work_detail WHERE company_code = '{0}')
                                        , (SELECT MAX(actual_work_year) FROM member_actual_work_detail WHERE company_code = '{0}')
                                        , (SELECT MAX(actual_work_month) FROM member_actual_work_detail WHERE company_code = '{0}'))
                                ) AS tbOperatingDay
                                    ON tbWorkTime.company_code = tbOperatingDay.company_code
                                    AND tbWorkTime.actual_work_year = tbOperatingDay.target_year
                                    AND tbWorkTime.actual_work_month = tbOperatingDay.target_month
                            ) AS tbActualData
                        GROUP BY company_code, project_sys_id
                    ) AS tbActual
                        ON tbProject.company_code = tbActual.company_code
                        AND tbProject.project_sys_id = tbActual.project_sys_id
                    LEFT JOIN (SELECT tbPlanCost.company_code ,
                                       tbPlanCost.project_sys_id ,
                                       SUM(tbPlanCost.plan_cost) AS total_plan_cost
                                FROM
                                  (SELECT mad.company_code ,
                                          mad.project_sys_id ,
                                          mad.user_sys_id ,
                                          mad.target_month ,
                                          mad.target_year ,
                                          plan_man_days ,
                                          ISNULL((dbo.RoundNumber('{0}',
                                                                    (SELECT TOP 1 base_unit_cost
                                                                     FROM unit_price_history
                                                                     WHERE company_code = '{0}'
                                                                       AND user_sys_id = mad.user_sys_id
                                                                       AND apply_start_date <= CONVERT(date, CAST(mad.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)), 2) + '/01')
                                                                       AND del_flg = '0'
                                                                     ORDER BY apply_start_date DESC) * (mad.plan_man_days / tbOperatingDay.operating_days))),0) AS plan_cost
                                   FROM member_assignment_detail mad INNER HASH
                                   JOIN
                                     (SELECT *
                                      FROM dbo.GetTableDaysInMonths('{0}' ,
                                                                      (SELECT MIN(actual_work_year)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = '{0}') ,
                                                                      (SELECT MIN(actual_work_month)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = '{0}') ,
                                                                      (SELECT MAX(actual_work_year)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = '{0}') ,
                                                                      (SELECT MAX(actual_work_month)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = '{0}'))) AS tbOperatingDay ON mad.company_code = tbOperatingDay.company_code
                                   AND mad.target_year = tbOperatingDay.target_year
                                   AND mad.target_month = tbOperatingDay.target_month
                                   WHERE mad.company_code = '{0}'
                                   GROUP BY mad.company_code ,
                                            mad.project_sys_id ,
                                            mad.user_sys_id ,
                                            mad.target_month ,
                                            mad.target_year ,
                                            mad.plan_man_days ,
                                            tbOperatingDay.operating_days) AS tbPlanCost
                                WHERE company_code = '{0}'
                                GROUP BY tbPlanCost.company_code,
                                         tbPlanCost.project_sys_id
                    ) AS tbPlan
                        ON tbProject.company_code = tbPlan.company_code
                        AND tbProject.project_sys_id = tbPlan.project_sys_id
                    LEFT JOIN project_info_history AS tbProjectHistory
                        ON tbProject.company_code = tbProjectHistory.company_code
                        AND tbProject.project_sys_id = tbProjectHistory.project_sys_id
                        AND tbProjectHistory.history_no = (
                            SELECT history_no 
                            FROM (SELECT
                                    ROW_NUMBER() OVER (ORDER BY history_no DESC) AS rownumber
                                    , history_no
                                FROM (SELECT TOP(2) history_no
                                    FROM project_info_history
                                    WHERE company_code = tbProject.company_code
                                        AND project_sys_id = tbProject.project_sys_id
                                    ORDER BY history_no DESC
                                    ) AS tbFilter
                                ) AS tbHistoryNo
                            WHERE rownumber = (CASE
                                WHEN tbStatus.sales_type = '0' THEN 2
                                ELSE 1 END))
                WHERE tbProject.company_code = '{0}' ", companyCode);

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.PROJECT_NAME))
                    sb.AppendFormat("AND tbProject.project_name LIKE '{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.PROJECT_NAME) + "%");

                if (!string.IsNullOrEmpty(condition.FROM_DATE) || !string.IsNullOrEmpty(condition.TO_DATE))
                {
                    string lastDayOfTo = "";

                    if (!string.IsNullOrEmpty(condition.TO_DATE))
                    {
                        var toDate = DateTime.Parse(condition.TO_DATE);
                        lastDayOfTo = DateTime.DaysInMonth(toDate.Year, toDate.Month).ToString();
                    }

                    if (condition.TIME_CONDITION_TYPE == 0)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.start_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.end_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 1)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.start_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.start_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 2)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.end_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.end_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 3)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.acceptance_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.acceptance_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                }

                if (condition.CUSTOMER_ID != null)
                {
                    sb.AppendFormat("AND tbSales.customer_id = {0} ", condition.CUSTOMER_ID.Value);

                    if (condition.TAG_ID != null)
                        sb.AppendFormat("AND tbSales.tag_id = {0} ", condition.TAG_ID.Value);
                }

                if (condition.CONTRACT_TYPE_ID != null)
                    sb.AppendFormat("AND tbProject.contract_type_id = {0} ", condition.CONTRACT_TYPE_ID.Value);

                if (condition.CHARGE_PERSON_ID != null)
                {
                    sb.AppendFormat("AND tbProject.charge_person_id = {0}", condition.CHARGE_PERSON_ID.Value);
                }

                if (condition.GROUP_ID.Count > 0 && condition.GROUP_ID[0] != "")
                {
                    string groupIds = string.Join(",", condition.GROUP_ID);
                    sb.AppendFormat("AND tbUser.group_id IN ({0}) ", groupIds);
                }

                if (condition.STATUS_ID.HasValue && 2 != condition.STATUS_ID)
                    sb.AppendFormat("AND tbStatus.sales_type = '{0}' ", condition.STATUS_ID.Value);

                if (condition.COMPLETE_ID.HasValue && 2 != condition.COMPLETE_ID)
                    sb.AppendFormat("AND tbStatus.completed_flg = '{0}' ", condition.COMPLETE_ID.Value);

                if (!condition.DELETE_FLG)
                    sb.Append("AND tbProject.del_flg = '0'");
            }

            sb.AppendFormat(@"
                ) AS tbData ) AS tbPaging
            WHERE peta_rn > {0} AND peta_rn <= {1} ", startItem, itemsPerPage + startItem);

            var sql = new Sql(sb.ToString());

            return this._database.Fetch<ProjectInfoPlus>(sql);
        }

        /// <summary>
        /// Search all project by condition
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public IList<ProjectInfoPlus> SearchAll(string companyCode, Condition condition)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"SELECT 
                tbProject.project_sys_id, tbProject.total_sales, tbProject.total_payment, tbPlan.total_plan_cost AS total_cost 
                FROM
                project_info AS tbProject LEFT JOIN (
                 SELECT company_code, project_sys_id, SUM(ISNULL(total_plan_cost, 0)) AS total_plan_cost
                FROM member_assignment WHERE company_code = '{0}'
                 GROUP BY company_code, project_sys_id
                ) AS tbPlan
                ON tbProject.company_code = tbPlan.company_code
                AND tbProject.project_sys_id = tbPlan.project_sys_id LEFT JOIN sales_payment AS tbSales
                ON tbProject.company_code = tbSales.company_code
                AND tbProject.project_sys_id = tbSales.project_sys_id
                AND tbSales.ordering_flg = '1' INNER JOIN m_status AS tbStatus
                ON tbProject.company_code = tbStatus.company_code
                AND tbProject.status_id = tbStatus.status_id
                WHERE tbProject.company_code = '{0}' ", companyCode);

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.PROJECT_NAME))
                    sb.AppendFormat("AND tbProject.project_name LIKE '{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.PROJECT_NAME) + "%");

                if (!string.IsNullOrEmpty(condition.FROM_DATE) || !string.IsNullOrEmpty(condition.TO_DATE))
                {
                    string lastDayOfTo = "";

                    if (!string.IsNullOrEmpty(condition.TO_DATE))
                    {
                        var toDate = DateTime.Parse(condition.TO_DATE);
                        lastDayOfTo = DateTime.DaysInMonth(toDate.Year, toDate.Month).ToString();
                    }

                    if (condition.TIME_CONDITION_TYPE == 0)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.start_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.end_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 1)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.start_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.start_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 2)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.end_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.end_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 3)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.acceptance_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.acceptance_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                }

                if (condition.CUSTOMER_ID != null)
                {
                    sb.AppendFormat("AND tbSales.customer_id = {0} ", condition.CUSTOMER_ID.Value);

                    if (condition.TAG_ID != null)
                        sb.AppendFormat("AND tbSales.tag_id = {0} ", condition.TAG_ID.Value);
                }

                if (condition.CONTRACT_TYPE_ID != null)
                    sb.AppendFormat("AND tbProject.contract_type_id = {0} ", condition.CONTRACT_TYPE_ID.Value);

                if (condition.CHARGE_PERSON_ID != null)
                {
                    sb.AppendFormat("AND tbProject.charge_person_id = {0}", condition.CHARGE_PERSON_ID.Value);
                }

                if (condition.GROUP_ID.Count > 0 && condition.GROUP_ID[0] != "")
                {
                    string groupIds = string.Join(",", condition.GROUP_ID);
                    sb.Append("AND ( ");
                    sb.Append("SELECT group_id ");
                    sb.Append("FROM m_user ");
                    sb.Append("WHERE company_code = tbProject.company_code ");
                    sb.AppendFormat("AND user_sys_id = tbProject.charge_person_id) IN ({0}) ", groupIds);
                }

                if (condition.STATUS_ID.HasValue && 2 != condition.STATUS_ID)
                    sb.AppendFormat("AND tbStatus.sales_type = '{0}' ", condition.STATUS_ID.Value);

                if (condition.COMPLETE_ID.HasValue && 2 != condition.COMPLETE_ID)
                    sb.AppendFormat("AND tbStatus.completed_flg = '{0}' ", condition.COMPLETE_ID.Value);

                if (!condition.DELETE_FLG)
                    sb.Append("AND tbProject.del_flg = '0'");
            }

            var query = new Sql(sb.ToString());

            return this._database.Fetch<ProjectInfoPlus>(query);
        }

        /// <summary>
        /// Export Project List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Project list</returns>
        public IList<ProjectInfoPlus> ExportProjectListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"SELECT * FROM (
                SELECT tbProject.project_sys_id
                    , tbProject.project_no
                    , (SELECT tag_name FROM m_customer_tag
                        WHERE company_code = tbProject.company_code AND tag_id = tbSales.tag_id) AS tag_name
                    , tbProject.upd_date
                    , tbProject.project_name
                    , tbContractType.contract_type
                    , tbRank.rank
                    , tbProject.start_date
                    , tbProject.end_date
                    , tbProject.acceptance_date
                    , tbUser.display_name AS charge_person
                    , (SELECT display_name FROM m_user WHERE
                        company_code = tbProject.company_code AND user_sys_id = tbProject.charge_of_sales_id) AS charge_of_sales
                    , tbStatus.status
                    , tbPlan.total_plan_cost
                    , tbActual.actual_cost
                    , tbProject.total_sales
                    , tbProject.total_payment
                    , tbProject.estimate_man_days
                    , ISNULL(tbActual.actual_man_day, 0) AS actual_man_day
                    , (SELECT TOP(1) progress 
                        FROM progress_history 
                        WHERE company_code = tbProject.company_code 
                            AND project_sys_id = tbProject.project_sys_id 
                        ORDER BY regist_date DESC
                        ) AS progress
                    , CASE WHEN tbProject.total_sales = 0 THEN 0 
                        ELSE ((tbProject.total_sales - ISNULL(tbProject.total_payment, 0) - ISNULL(tbPlan.total_plan_cost, 0)) / tbProject.total_sales) 
                        END AS plan_profit
                    , CASE WHEN tbProject.total_sales = 0 THEN 0 
                        ELSE ((tbProject.total_sales - ISNULL(tbProject.total_payment, 0) - ISNULL(tbActual.actual_cost, 0)) / tbProject.total_sales) 
                        END AS actual_profit
                    , (SELECT display_name FROM m_customer WHERE company_code = tbProject.company_code AND customer_id = tbSales.customer_id) AS customer_name
                    , (SELECT display_name FROM m_user WHERE company_code = tbProject.company_code AND user_sys_id = tbProject.upd_id) AS upd_user
                    , tbProject.del_flg
                    , tbContractType.display_order AS contract_type_order
                    , tbRank.display_order AS rank_order
                    , ISNULL(tbUser.furigana_sei, '') + ISNULL(tbUser.furigana_mei,'') + ISNULL(tbUser.display_name,'') AS pic_order
                    , tbStatus.display_order AS status_order
                    , tbProject.remarks
                FROM
                    project_info AS tbProject
                    INNER JOIN m_contract_type AS tbContractType
                        ON tbProject.company_code = tbContractType.company_code
                        AND tbProject.contract_type_id = tbContractType.contract_type_id
                    LEFT JOIN m_rank AS tbRank
                        ON tbProject.company_code = tbRank.company_code
                        AND tbProject.rank_id = tbRank.rank_id
                    INNER JOIN m_user AS tbUser
                        ON tbProject.company_code = tbUser.company_code
                        AND tbProject.charge_person_id = tbUser.user_sys_id
                    INNER JOIN m_status AS tbStatus
                        ON tbProject.company_code = tbStatus.company_code
                        AND tbProject.status_id = tbStatus.status_id
                    LEFT JOIN sales_payment AS tbSales
                        ON tbProject.company_code = tbSales.company_code
                        AND tbProject.project_sys_id = tbSales.project_sys_id
                        AND tbSales.ordering_flg = '1'
                    LEFT JOIN (SELECT company_code
                    , project_sys_id
                    , SUM(work_time_in_month) AS actual_man_day
                    , SUM(actual_cost_in_month) AS actual_cost
                    FROM (SELECT tbWorkTime.company_code
                            , tbWorkTime.project_sys_id
                            , tbWorkTime.user_sys_id
                            , tbWorkTime.work_time_in_month
                            , ISNULL(dbo.RoundNumber('{0}', (
                                (SELECT TOP 1 base_unit_cost 
				                 FROM unit_price_history 
				                 WHERE company_code = '{0}'
					                AND user_sys_id = tbWorkTime.user_sys_id 
					                AND apply_start_date <= CONVERT(date, CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)), 2) + '/01')
					                AND del_flg = '0'
				                 ORDER BY apply_start_date DESC)
                                * (tbWorkTime.work_time_in_month / tbOperatingDay.operating_days)))
                                , 0) AS actual_cost_in_month
                        FROM (SELECT company_code
                                , project_sys_id
                                , user_sys_id
                                , actual_work_month
                                , actual_work_year
                                , dbo.Hour2Day('{0}', SUM(actual_work_time)) AS work_time_in_month
                            FROM member_actual_work_detail
                            WHERE company_code = '{0}'
                            GROUP BY company_code
                                , project_sys_id
                                , user_sys_id
                                , actual_work_month, actual_work_year
                            ) AS tbWorkTime
                            INNER HASH JOIN (
                                SELECT *
                                FROM dbo.GetTableDaysInMonths('{0}'
                                    , (SELECT MIN(actual_work_year) FROM member_actual_work_detail WHERE company_code = '{0}')
                                    , (SELECT MIN(actual_work_month) FROM member_actual_work_detail WHERE company_code = '{0}')
                                    , (SELECT MAX(actual_work_year) FROM member_actual_work_detail WHERE company_code = '{0}')
                                    , (SELECT MAX(actual_work_month) FROM member_actual_work_detail WHERE company_code = '{0}'))
                            ) AS tbOperatingDay
                                ON tbWorkTime.company_code = tbOperatingDay.company_code
                                AND tbWorkTime.actual_work_year = tbOperatingDay.target_year
                                AND tbWorkTime.actual_work_month = tbOperatingDay.target_month
                        ) AS tbActualData
                        GROUP BY company_code, project_sys_id
                    ) AS tbActual 
                        ON tbProject.company_code = tbActual.company_code 
                        AND tbProject.project_sys_id = tbActual.project_sys_id 
                    LEFT JOIN (SELECT tbPlanCost.company_code ,
                                       tbPlanCost.project_sys_id ,
                                       SUM(tbPlanCost.plan_cost) AS total_plan_cost
                                FROM
                                  (SELECT mad.company_code ,
                                          mad.project_sys_id ,
                                          mad.user_sys_id ,
                                          mad.target_month ,
                                          mad.target_year ,
                                          plan_man_days ,
                                          ISNULL((dbo.RoundNumber('{0}',
                                                                    (SELECT TOP 1 base_unit_cost
                                                                     FROM unit_price_history
                                                                     WHERE company_code = '{0}'
                                                                       AND user_sys_id = mad.user_sys_id
                                                                       AND apply_start_date <= CONVERT(date, CAST(mad.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)), 2) + '/01')
                                                                       AND del_flg = '0'
                                                                     ORDER BY apply_start_date DESC) * (mad.plan_man_days / tbOperatingDay.operating_days))),0) AS plan_cost
                                   FROM member_assignment_detail mad INNER HASH
                                   JOIN
                                     (SELECT *
                                      FROM dbo.GetTableDaysInMonths('{0}' ,
                                                                      (SELECT MIN(actual_work_year)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = '{0}') ,
                                                                      (SELECT MIN(actual_work_month)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = '{0}') ,
                                                                      (SELECT MAX(actual_work_year)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = '{0}') ,
                                                                      (SELECT MAX(actual_work_month)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = '{0}'))) AS tbOperatingDay ON mad.company_code = tbOperatingDay.company_code
                                   AND mad.target_year = tbOperatingDay.target_year
                                   AND mad.target_month = tbOperatingDay.target_month
                                   WHERE mad.company_code = '{0}'
                                   GROUP BY mad.company_code ,
                                            mad.project_sys_id ,
                                            mad.user_sys_id ,
                                            mad.target_month ,
                                            mad.target_year ,
                                            mad.plan_man_days ,
                                            tbOperatingDay.operating_days) AS tbPlanCost
                                WHERE company_code = '{0}'
                                GROUP BY tbPlanCost.company_code,
                                         tbPlanCost.project_sys_id
                    ) AS tbPlan 
                        ON tbProject.company_code = tbPlan.company_code 
                        AND tbProject.project_sys_id = tbPlan.project_sys_id 
                WHERE tbProject.company_code = '{0}' ", companyCode);

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.PROJECT_NAME))
                    sb.AppendFormat("AND tbProject.project_name LIKE '{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.PROJECT_NAME) + "%");

                if (!string.IsNullOrEmpty(condition.FROM_DATE) || !string.IsNullOrEmpty(condition.TO_DATE))
                {
                    string lastDayOfTo = "";

                    if (!string.IsNullOrEmpty(condition.TO_DATE))
                    {
                        var toDate = DateTime.Parse(condition.TO_DATE);
                        lastDayOfTo = DateTime.DaysInMonth(toDate.Year, toDate.Month).ToString();
                    }

                    if (condition.TIME_CONDITION_TYPE == 0)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.start_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.end_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 1)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.start_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.start_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 2)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.end_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.end_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                    else if (condition.TIME_CONDITION_TYPE == 3)
                    {
                        if (!string.IsNullOrEmpty(condition.FROM_DATE))
                            sb.AppendFormat("AND tbProject.acceptance_date >= '{0}' ", DateTime.Parse(condition.FROM_DATE + "/01"));

                        if (!string.IsNullOrEmpty(condition.TO_DATE))
                            sb.AppendFormat("AND tbProject.acceptance_date <= '{0}' ", DateTime.Parse(condition.TO_DATE + "/" + lastDayOfTo));
                    }
                }

                if (condition.CUSTOMER_ID != null)
                {
                    sb.AppendFormat("AND tbSales.customer_id = {0} ", condition.CUSTOMER_ID.Value);

                    if (condition.TAG_ID != null)
                        sb.AppendFormat("AND tbSales.tag_id = {0} ", condition.TAG_ID.Value);
                }

                if (condition.CONTRACT_TYPE_ID != null)
                    sb.AppendFormat("AND tbProject.contract_type_id = {0} ", condition.CONTRACT_TYPE_ID.Value);

                if (condition.CHARGE_PERSON_ID != null)
                    sb.AppendFormat("AND tbProject.charge_person_id = {0} ", condition.CHARGE_PERSON_ID.Value);

                if (condition.GROUP_ID.Count > 0 && condition.GROUP_ID[0] != "")
                {
                    string groupIds = string.Join(",", condition.GROUP_ID);
                    sb.AppendFormat("AND tbUser.group_id IN ({0}) ", groupIds);
                }
                if (condition.STATUS_ID.HasValue && 2 != condition.STATUS_ID)
                    sb.AppendFormat("AND tbStatus.sales_type = '{0}' ", condition.STATUS_ID.Value);

                if (condition.COMPLETE_ID.HasValue && 2 != condition.COMPLETE_ID)
                    sb.AppendFormat("AND tbStatus.completed_flg = '{0}' ", condition.COMPLETE_ID.Value);

                if (!condition.DELETE_FLG)
                    sb.Append("AND tbProject.del_flg = '0'");
            }

            sb.Append(") tbFinal");
            sb.Append(" ORDER BY tbFinal." + orderBy + " " + orderType);

            var query = new Sql(sb.ToString());

            return this._database.Fetch<ProjectInfoPlus>(query);
        }

        #endregion

        #region Project Edit
        /// <summary>
        /// Get a project info
        /// </summary>
        /// <param name="companyCode">company code</param>
        /// <param name="projectID">project code</param>
        /// <param name="isToCopy">Check is action copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>Project info</returns>
        public ProjectInfoPlus GetProjectInfo(string companyCode, int projectID, bool isToCopy, int copyType)
        {
            var sql = new Sql(@"
                SELECT
                    ts1.company_code,
                    ts1.project_sys_id,
                    ts1.project_name,
                    ts1.contract_type_id,
                    (SELECT
                        display_name
                    FROM
                        m_group
                    WHERE
                        company_code = ts1.company_code
                        AND group_id = ts14.group_id) AS group_sales_pic,
                    ts14.display_name AS charge_of_sales,
                    ts14.group_id AS group_sales_pic_id,
                    ts1.charge_of_sales_id,
                    ts1.tax_rate,
                    ts1.charge_person_id,
                    ts2.display_name AS charge_person,
                    ts2.group_id,
                    (SELECT
                        display_name
                    FROM
                        m_group
                    WHERE
                        company_code = ts1.company_code
                        AND group_id = ts2.group_id) AS group_name,
                    (SELECT
                        display_name
                    FROM
                        m_customer
                    WHERE
                        company_code = ts1.company_code
                        AND customer_id = ts8.customer_id) AS customer_name,
                    (SELECT TOP 1 base_unit_cost 
				     FROM unit_price_history 
				     WHERE company_code = ts1.company_code
					    AND user_sys_id = ts2.user_sys_id 
                        AND (YEAR(apply_start_date) < YEAR(GETDATE()) OR (YEAR(apply_start_date) = YEAR(GETDATE()) AND MONTH(apply_start_date) <= MONTH(GETDATE())))					    
                        AND del_flg = '0'
				     ORDER BY apply_start_date DESC) AS base_unit_cost");

            if (!(isToCopy && copyType == Constant.CopyType.NORMAL))
            {
                sql.Append(@"
                    , ts1.project_no,
                    ts1.start_date,
                    ts1.end_date,
                    ts1.acceptance_date,
                    ts1.estimate_man_days,
                    (SELECT TOP(1) progress FROM progress_history WHERE company_code = ts1.company_code AND project_sys_id = ts1.project_sys_id ORDER BY regist_date DESC ) AS progress,
                    ts1.assign_fix_date,
                    (SELECT sales_type
                    FROM m_status
                    WHERE company_code = ts1.company_code
                    AND status_id = ts1.status_id) AS sales_type,
                    ts1.total_sales,
                    ts1.total_payment,
                    ts1.del_flg,
                    ts1.row_version,
                    ts1.ins_date,
                    ts1.upd_date,
                    (SELECT
                        display_name
                    FROM
                        m_user
                    WHERE
                        company_code = ts1.company_code
                        AND user_sys_id = ts1.ins_id) AS ins_user,
                    (SELECT
                        display_name
                    FROM
                        m_user
                    WHERE
                        company_code = ts1.company_code
                        AND user_sys_id = ts1.upd_id) AS upd_user,
                    (SELECT
                        data_editable_time
                    FROM
                        m_company_setting
                    WHERE
                        company_code = ts1.company_code) AS data_editable_time,
                    tbActual.actual_man_day,
                    tbActual.actual_cost,
                    tbPlan.total_plan_man_days,
                    tbPlan.total_plan_cost
                    , CASE
                        WHEN ts1.total_sales = 0 THEN 0 
                        ELSE ((ts1.total_sales - ISNULL(ts1.total_payment, 0) - ISNULL(tbPlan.total_plan_cost, 0)) / ts1.total_sales)
                        END AS plan_profit
                    , CASE
                        WHEN ts1.total_sales = 0 THEN 0
                        ELSE ((ts1.total_sales - ISNULL(ts1.total_payment, 0) - ISNULL(tbActual.actual_cost, 0)) / ts1.total_sales)
                        END AS actual_profit
                    , (select count(ppi.company_code) from project_plan_info as ppi where ppi.company_code = ts1.company_code and ppi.project_sys_id = ts1.project_sys_id  and  ppi.del_flg ='0') as count_plan
                ");
            }

            if (!isToCopy)
            {
                sql.Append(@"
                    , ts1.status_id,
                    ts1.rank_id,
                    ts1.status_note,
                    ts1.remarks
                ");
            }

            sql.Append(@"
                FROM
                    project_info ts1 INNER JOIN m_user ts2
                    ON ts2.company_code = ts1.company_code
                    AND ts2.user_sys_id = ts1.charge_person_id LEFT JOIN sales_payment ts8
                    ON ts8.company_code = ts1.company_code
                    AND ts8.project_sys_id = ts1.project_sys_id
                    AND ts8.ordering_flg = '1' LEFT JOIN m_user ts14
                    ON ts14.company_code = ts1.company_code
                    AND ts14.user_sys_id = ts1.charge_of_sales_id
                    LEFT JOIN (SELECT company_code
                            , project_sys_id
                            , SUM(work_time_in_month) AS actual_man_day
                            , SUM(actual_cost_in_month) AS actual_cost
                        FROM (SELECT tbWorkTime.company_code
                                , tbWorkTime.project_sys_id
                                , tbWorkTime.user_sys_id
                                , tbWorkTime.work_time_in_month
                                , ISNULL(dbo.RoundNumber(@company_code, (
                                    (SELECT TOP 1 base_unit_cost 
				                     FROM unit_price_history 
				                     WHERE company_code = @company_code
					                    AND user_sys_id = tbWorkTime.user_sys_id 
					                    AND apply_start_date <= CONVERT(date, CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)), 2) + '/01')
					                    AND del_flg = '0'
				                     ORDER BY apply_start_date DESC)
                                    * (tbWorkTime.work_time_in_month / tbOperatingDay.operating_days)))
                                    , 0) AS actual_cost_in_month
                            FROM (SELECT company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , actual_work_month
                                    , actual_work_year
                                    , dbo.Hour2Day(@company_code, SUM(actual_work_time)) AS work_time_in_month
                                FROM member_actual_work_detail
                                WHERE company_code = @company_code
                                GROUP BY company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , actual_work_month, actual_work_year
                                ) AS tbWorkTime
                                INNER HASH JOIN (
                                    SELECT *
                                    FROM dbo.GetTableDaysInMonths(@company_code
                                        , (SELECT MIN(actual_work_year) FROM member_actual_work_detail WHERE company_code = @company_code)
                                        , (SELECT MIN(actual_work_month) FROM member_actual_work_detail WHERE company_code = @company_code)
                                        , (SELECT MAX(actual_work_year) FROM member_actual_work_detail WHERE company_code = @company_code)
                                        , (SELECT MAX(actual_work_month) FROM member_actual_work_detail WHERE company_code = @company_code))
                                ) AS tbOperatingDay
                                    ON tbWorkTime.company_code = tbOperatingDay.company_code
                                    AND tbWorkTime.actual_work_year = tbOperatingDay.target_year
                                    AND tbWorkTime.actual_work_month = tbOperatingDay.target_month
                            ) AS tbActualData
                        GROUP BY company_code, project_sys_id
                    ) AS tbActual
                        ON ts1.company_code = tbActual.company_code
                        AND ts1.project_sys_id = tbActual.project_sys_id
                    LEFT JOIN (SELECT tbPlanCost.company_code ,
                                       tbPlanCost.project_sys_id ,
                                       SUM(tbPlanCost.plan_cost) AS total_plan_cost,
                                       SUM(tbPlanCost.plan_man_days) AS total_plan_man_days 
                                FROM
                                  (SELECT mad.company_code,
                                          mad.project_sys_id,
                                          mad.user_sys_id,
                                          mad.target_month,
                                          mad.target_year,
                                          plan_man_days,
                                          ISNULL((dbo.RoundNumber(@company_code,
                                                                    (SELECT TOP 1 base_unit_cost
                                                                     FROM unit_price_history
                                                                     WHERE company_code = @company_code
                                                                       AND user_sys_id = mad.user_sys_id
                                                                       AND apply_start_date <= CONVERT(date, CAST(mad.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)), 2) + '/01')
                                                                       AND del_flg = '0'
                                                                     ORDER BY apply_start_date DESC) * (mad.plan_man_days / tbOperatingDay.operating_days))),0) AS plan_cost
                                   FROM member_assignment_detail mad INNER HASH
                                   JOIN
                                     (SELECT *
                                      FROM dbo.GetTableDaysInMonths(@company_code ,
                                                                      (SELECT MIN(actual_work_year)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = @company_code) ,
                                                                      (SELECT MIN(actual_work_month)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = @company_code) ,
                                                                      (SELECT MAX(actual_work_year)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = @company_code) ,
                                                                      (SELECT MAX(actual_work_month)
                                                                       FROM member_actual_work_detail
                                                                       WHERE company_code = @company_code))) AS tbOperatingDay ON mad.company_code = tbOperatingDay.company_code
                                   AND mad.target_year = tbOperatingDay.target_year
                                   AND mad.target_month = tbOperatingDay.target_month
                                   WHERE mad.company_code = @company_code
                                   GROUP BY mad.company_code ,
                                            mad.project_sys_id ,
                                            mad.user_sys_id ,
                                            mad.target_month ,
                                            mad.target_year ,
                                            mad.plan_man_days ,
                                            tbOperatingDay.operating_days) AS  tbPlanCost
                                WHERE company_code = @company_code
                                GROUP BY tbPlanCost.company_code,
                                         tbPlanCost.project_sys_id
                    ) AS tbPlan
                        ON ts1.company_code = tbPlan.company_code
                        AND ts1.project_sys_id = tbPlan.project_sys_id
                WHERE
                    ts1.company_code = @company_code
                    AND ts1.project_sys_id = @project_sys_id
                ", new
            {
                company_code = companyCode,
                project_sys_id = Convert.ToInt32(projectID)
            });

            return this._database.FirstOrDefault<ProjectInfoPlus>(sql);
        }

        /// <summary>
        ///  Edit a project info
        /// </summary>
        /// <param name="data">Project info</param>
        /// <param name="phaseList">Target phase list</param>
        /// <param name="targetCategoryList">Target category list</param>
        /// <param name="outsourcerList">Outsourcer list</param>
        /// <param name="subcontractorList">Subcontractor list</param>
        /// <param name="paymentDetailList">Payment detail list</param>
        /// <param name="overheadCostList">Overhead cost list</param>
        /// <param name="overheadCostDetailList">Overhead cost detail list</param>
        /// <param name="memberAssignmentList">Member assignment list</param>
        /// <param name="memberAssignmentDetailList">Member assignment detail list</param>
        /// <param name="progressHistoryList">Progress history list</param>
        /// <param name="fileList">Attach file list</param>
        /// <param name="allowRegistHistory">Allow regist history</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="error">Error message</param>
        /// <returns>True/False</returns>
        public bool EditProjectInfo(ProjectInfoPlus data,
            IList<PhasePlus> phaseList,
            IList<TargetCategoryPlus> targetCategoryList,
            IList<SalesPaymentPlus> outsourcerList,
            IList<SalesPaymentPlus> subcontractorList,
            IList<SalesPaymentDetailPlus> paymentDetailList,
            IList<OverheadCostPlus> overheadCostList,
            IList<OverheadCostDetailPlus> overheadCostDetailList,
            IList<MemberAssignmentPlus> memberAssignmentList,
            IList<MemberAssignmentDetailPlus> memberAssignmentDetailList,
            IList<ProgressHistoryPlus> progressHistoryList,
            IList<ProjectAttachFilePlus> fileList,
            bool allowRegistHistory,
            out int projectID,
            out string error)
        {
            int res = 0;
            projectID = 0;
            error = string.Empty;

            // Update
            if (data.project_sys_id != 0)
            {
                projectID = data.project_sys_id;

                var sqlUpdateProjectInfo = new Sql(@"
                    UPDATE
                        project_info
                    SET
                        project_name = @project_name,
                        contract_type_id = @contract_type_id,
                        charge_of_sales_id = @charge_of_sales_id,
                        estimate_man_days = @estimate_man_days,
                        rank_id = @rank_id,
                        start_date = @start_date,
                        end_date = @end_date,
                        acceptance_date = @acceptance_date,
                        status_id = @status_id,
                        assign_fix_date = @assign_fix_date,
                        charge_person_id = @charge_person_id,
                        total_sales = @total_sales,
                        total_payment = @total_payment,
                        tax_rate = @tax_rate,
                        status_note = @status_note,
                        remarks = @remarks,
                        upd_date = @upd_date,
                        upd_id = @upd_id,
                        del_flg = @del_flg
                    WHERE
                        project_sys_id = @project_sys_id
                        AND company_code = @company_code
                        AND row_version = @row_version;",
                     new
                     {
                         project_name = string.IsNullOrEmpty(data.project_name) ? null : data.project_name.Trim(),
                         contract_type_id = data.contract_type_id,
                         charge_of_sales_id = data.charge_of_sales_id ?? null,
                         estimate_man_days = data.estimate_man_days,
                         rank_id = data.rank_id ?? null,
                         start_date = data.start_date ?? null,
                         end_date = data.end_date ?? null,
                         acceptance_date = data.acceptance_date ?? null,
                         status_id = data.status_id,
                         assign_fix_date = data.assign_fix_date ?? null,
                         charge_person_id = data.charge_person_id,
                         total_sales = data.total_sales,
                         total_payment = data.total_payment ?? null,
                         tax_rate = data.tax_rate,
                         status_note = string.IsNullOrEmpty(data.status_note) ? null : data.status_note.Trim(),
                         remarks = string.IsNullOrEmpty(data.remarks) ? null : data.remarks.Trim(),
                         upd_date = data.upd_date,
                         upd_id = data.upd_id,
                         del_flg = data.del_flg,
                         project_sys_id = data.project_sys_id,
                         company_code = data.company_code,
                         row_version = data.row_version
                     });
                res = this._database.Execute(sqlUpdateProjectInfo);
            }
            else // Insert
            {
                // Set project no auto increase
                int currentYear = data.upd_date.Value.Year;
                int currentMonth = data.upd_date.Value.Month;
                string strMonth = currentMonth.ToString().Length == 1 ? "0" + currentMonth.ToString() : currentMonth.ToString();
                string newProjectNo = currentYear.ToString() + strMonth + "0001";
                string latestProjectNo = this.GetLatestProjectNo(data.company_code).ToString();

                if (latestProjectNo.Length == Constant.PROJECT_NO_MAX_LENGTH)
                {
                    int year = Convert.ToInt32(latestProjectNo.Substring(0, 4));
                    int month = Convert.ToInt32(latestProjectNo.Substring(4, 2));

                    if (currentYear == year && currentMonth == month)
                    {
                        newProjectNo = (Convert.ToInt32(latestProjectNo) + 1).ToString();
                    }
                }

                var sqlInsertProjectInfo = new Sql(@"
                    INSERT INTO
                        project_info
                        (company_code,
                        project_no,
                        project_name,
                        contract_type_id,
                        charge_of_sales_id,
                        estimate_man_days,
                        rank_id,
                        start_date,
                        end_date,
                        acceptance_date,
                        status_id,
                        assign_fix_date,
                        charge_person_id,
                        total_sales,
                        total_payment,
                        tax_rate,
                        status_note,
                        remarks,
                        ins_date,
                        ins_id,
                        upd_date,
                        upd_id,
                        del_flg)
                    VALUES
                        (@company_code, @project_no, @project_name, @contract_type_id, @charge_of_sales_id, @estimate_man_days,
                        @rank_id, @start_date, @end_date, @acceptance_date, @status_id, @assign_fix_date, @charge_person_id, @total_sales,
                        @total_payment, @tax_rate, @status_note, @remarks, @upd_date, @upd_id, @upd_date, @upd_id, @del_flg);
                    SELECT
                        SCOPE_IDENTITY();",
                    new
                    {
                        company_code = data.company_code,
                        project_no = newProjectNo,
                        project_name = string.IsNullOrEmpty(data.project_name) ? null : data.project_name.Trim(),
                        contract_type_id = data.contract_type_id,
                        charge_of_sales_id = data.charge_of_sales_id ?? null,
                        estimate_man_days = data.estimate_man_days,
                        rank_id = data.rank_id ?? null,
                        start_date = data.start_date ?? null,
                        end_date = data.end_date ?? null,
                        acceptance_date = data.acceptance_date ?? null,
                        status_id = data.status_id,
                        assign_fix_date = data.assign_fix_date,
                        charge_person_id = data.charge_person_id,
                        total_sales = data.total_sales,
                        total_payment = data.total_payment ?? null,
                        tax_rate = data.tax_rate,
                        status_note = string.IsNullOrEmpty(data.status_note) ? null : data.status_note.Trim(),
                        remarks = string.IsNullOrEmpty(data.remarks) ? null : data.remarks.Trim(),
                        upd_date = data.upd_date,
                        upd_id = data.upd_id,
                        del_flg = Constant.DeleteFlag.NON_DELETE
                    });

                int newProjectID = this._database.ExecuteScalar<int>(sqlInsertProjectInfo);

                if (newProjectID > 0)
                {
                    projectID = newProjectID;
                    data.project_sys_id = newProjectID;
                    data.del_flg = Constant.DeleteFlag.NON_DELETE;
                    res = 1;

                    if (latestProjectNo.Length == Constant.PROJECT_NO_MAX_LENGTH)
                    {
                        this.UpdateLatestProjectNo(data.company_code, newProjectNo, data.upd_date, data.upd_id);
                    }
                    else
                    {
                        this.InsertLatestProjectNo(data.company_code, newProjectNo, data.upd_date, data.upd_id);
                    }
                }
            }

            if (res == 1)
            {
                // Edit target phase
                this.EditTargetPhase(data.company_code,
                    projectID,
                    data.upd_date.Value,
                    data.upd_id,
                    phaseList);

                // Edit target category
                this.EditTargetCategory(data.company_code,
                    projectID,
                    data.upd_date.Value,
                    data.upd_id,
                    targetCategoryList);

                // Edit member assignment
                this.EditMemberAssignment(data.company_code,
                    projectID,
                    data.upd_date.Value,
                    data.upd_id,
                    memberAssignmentList,
                    memberAssignmentDetailList);

                // Edit outsourcer
                this.EditSalesPayment(data.company_code,
                    projectID,
                    Constant.OrderingFlag.SALES,
                    data.upd_date.Value,
                    data.upd_id,
                    outsourcerList,
                    null);

                // Edit subcontractor
                this.EditSalesPayment(data.company_code,
                    projectID,
                    Constant.OrderingFlag.PAYMENT,
                    data.upd_date.Value,
                    data.upd_id,
                    subcontractorList,
                    paymentDetailList);

                IList<OverheadCostPlus> newOverheadCostList = null;
                IList<OverheadCostDetailPlus> newOverheadCostDetailList = null;

                // Edit overhead cost
                this.EditOverheadCost(data.company_code,
                    projectID,
                    data.upd_date.Value,
                    data.upd_id,
                    overheadCostList,
                    overheadCostDetailList,
                    out newOverheadCostList,
                    out newOverheadCostDetailList);

                // Edit progress history
                if (progressHistoryList != null && progressHistoryList.Count > 0)
                {
                    this.EditProgressHistory(data.company_code,
                    projectID,
                    data.upd_date.Value,
                    data.upd_id,
                    progressHistoryList);
                }

                if (allowRegistHistory)
                {
                    int historyID = this.InsertProjectInfoHistory(data);

                    if (historyID > 0)
                    {
                        // Insert history of member assignment
                        this.InsertMemberAssignmentHistory(data.company_code,
                            projectID,
                            historyID,
                            data.upd_date.Value,
                            data.upd_id,
                            memberAssignmentList,
                            memberAssignmentDetailList);

                        // Insert history of outsourcer
                        this.InsertSalesPaymentHistory(data.company_code,
                            projectID,
                            historyID,
                            Constant.OrderingFlag.SALES,
                            data.upd_date.Value,
                            data.upd_id,
                            outsourcerList,
                            null);

                        // Insert history of subcontractor
                        this.InsertSalesPaymentHistory(data.company_code,
                            projectID,
                            historyID,
                            Constant.OrderingFlag.PAYMENT,
                            data.upd_date.Value,
                            data.upd_id,
                            subcontractorList,
                            paymentDetailList);

                        // Insert history of overheadcost
                        this.InsertOverheadCostHistory(data.company_code,
                            projectID,
                            historyID,
                            data.upd_date.Value,
                            data.upd_id,
                            newOverheadCostList,
                            newOverheadCostDetailList);
                    }
                }
            }

            return (res == 1);
        }

        /// <summary>
        /// Get all contract type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="isCreateCopy">Is function copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>List of contract type</returns>
        public IList<ContractType> GetContractTypeList(string companyCode, int projectID, bool isCreateCopy, int copyType)
        {
            Sql sql = new Sql();

            if (projectID > 0 && !(isCreateCopy && copyType == Constant.CopyType.NORMAL))
            {
                sql = Sql.Builder.Append(@"
                    SELECT
                        ISNULL(tbContractType.contract_type_id, tbTargetContractType.contract_type_id) AS contract_type_id,
                        ISNULL(tbContractType.contract_type, tbTargetContractType.contract_type) AS contract_type,
                        ISNULL(tbContractType.charge_of_sales_flg, tbTargetContractType.charge_of_sales_flg) AS charge_of_sales_flg,
                        ISNULL(tbContractType.exceptional_calculate_flg, tbTargetContractType.exceptional_calculate_flg) AS exceptional_calculate_flg,
                        ISNULL(tbContractType.display_order, tbTargetContractType.display_order) AS display_order
                    FROM (
                        SELECT
                            contract_type_id,
                            contract_type,
                            charge_of_sales_flg,
                            exceptional_calculate_flg,
                            display_order
                        FROM
                            m_contract_type
                        WHERE
                            company_code = @company_code
                            AND del_flg = '0'
                        ) AS tbContractType
                        FULL JOIN (
                            SELECT
                                project_info.contract_type_id,
                                m_contract_type.contract_type,
                                m_contract_type.charge_of_sales_flg,
                                m_contract_type.exceptional_calculate_flg,
                                m_contract_type.display_order
                            FROM project_info INNER JOIN m_contract_type
                                ON project_info.company_code = m_contract_type.company_code
                                AND project_info.contract_type_id = m_contract_type.contract_type_id
                            WHERE project_info.company_code = @company_code
                                AND project_sys_id = @project_sys_id
                        ) AS tbTargetContractType
                        ON tbContractType.contract_type_id = tbTargetContractType.contract_type_id
                    ORDER BY 
                        display_order",
                    new
                    {
                        company_code = companyCode,
                        project_sys_id = projectID
                    });
            }
            else
            {
                sql = Sql.Builder.Append(@"
                    SELECT
                        contract_type_id,
                        contract_type,
                        charge_of_sales_flg,
                        exceptional_calculate_flg,
                        display_order
                    FROM
                        m_contract_type
                    WHERE
                        company_code = @company_code
                        AND del_flg = '0'
                    ORDER BY 
                        display_order
                    ",
                     new
                     {
                         company_code = companyCode
                     });
            }

            return this._database.Fetch<ContractType>(sql);
        }

        /// <summary>
        /// Get all phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type code</param>
        /// <returns>List of phase</returns>
        public IList<PhasePlus> GetPhaseList(string companyCode, int contractTypeID)
        {
            var sql = new Sql(@"
                SELECT
                    tb1.phase_id,
                    tb2.display_name,
                    tb2.estimate_target_flg
                FROM
                    r_contract_type_phase tb1 INNER JOIN m_phase tb2
                    ON tb2.company_code = tb1.company_code
                    AND tb2.phase_id = tb1.phase_id 
                WHERE
                    tb1.company_code = @company_code
                    AND tb1.contract_type_id = @contract_type_id
                    AND tb2.del_flg = @del_flg
                ORDER BY 
                    tb1.display_order ",
                 new
                 {
                     company_code = companyCode,
                     contract_type_id = contractTypeID,
                     del_flg = Constant.DeleteFlag.NON_DELETE
                 });

            return this._database.Fetch<PhasePlus>(sql);
        }

        /// <summary>
        /// Get target phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of target phase</returns>
        public IList<TargetPhasePlus> GetTargetPhaseList(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                SELECT
                    phase_id, 
                    (SELECT display_name FROM m_phase WHERE company_code = target_phase.company_code AND phase_id = target_phase.phase_id) AS phase_name,
                    estimate_man_days
                FROM 
                    target_phase
                WHERE 
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id ", new { company_code = companyCode, project_sys_id = projectID });

            return this._database.Fetch<TargetPhasePlus>(sql);
        }

        /// <summary>
        /// Get default tax rate
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="fromDate">Project from date</param>
        /// <returns>Default tax rate</returns>
        public ConsumptionTax GetDefaultTaxRate(string companyCode, DateTime fromDate)
        {
            var sql = new Sql(@"
                SELECT TOP 1
                    tax_rate
                FROM
                    m_consumption_tax
                WHERE
                    company_code = @company_code
                    AND apply_start_date <= @apply_start_date
                ORDER BY apply_start_date DESC
                ",
                 new
                 {
                     company_code = companyCode,
                     apply_start_date = fromDate
                 });

            return this._database.FirstOrDefault<ConsumptionTax>(sql);
        }

        /// <summary>
        /// Get Customer
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <param name="orderingFlag">Orderingflag</param>
        /// <returns>List of Customer</returns>
        public IList<SalesPaymentPlus> GetCustomerList(string companyCode, int projectID, string orderingFlag)
        {
            var sql = new Sql(@"
                SELECT
                    customer_id,
                    total_amount,
                    charge_person_id,
                    (SELECT
                        display_name
                    FROM
                        m_customer
                    WHERE
                        company_code = sales_payment.company_code
                        AND customer_id = sales_payment.customer_id) AS customer_name,
                    end_user_id,
                    (SELECT
                        display_name
                    FROM
                        m_customer
                    WHERE
                        company_code = sales_payment.company_code
                        AND customer_id = sales_payment.end_user_id) AS end_user_name,
                    (SELECT
                        display_name
                    FROM
                        m_user
                    WHERE
                        company_code = sales_payment.company_code
                        AND user_sys_id = sales_payment.charge_person_id) AS charge_person_name,
                    tag_id
                FROM
                    sales_payment
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND ordering_flg = @ordering_flg
                ORDER BY
                    customer_id
                ", new
            {
                company_code = companyCode,
                project_sys_id = projectID,
                ordering_flg = orderingFlag
            });

            return this._database.Fetch<SalesPaymentPlus>(sql);
        }

        /// <summary>
        /// Get overhead cost list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of overhead cost</returns>
        public IList<OverheadCostPlus> GetOverheadCostList(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                SELECT
                    detail_no,
                    overhead_cost_id,
                    overhead_cost_detail,
                    charge_person_id,
                    total_amount,
                    (SELECT
                        overhead_cost_type
                    FROM
                        m_overhead_cost
                    WHERE
                        company_code = overhead_cost.company_code
                        AND overhead_cost_id = overhead_cost.overhead_cost_id) AS type_name,
                    (SELECT
                        display_name
                    FROM
                        m_user
                    WHERE
                        company_code = overhead_cost.company_code
                        AND user_sys_id = overhead_cost.charge_person_id) AS charge_person_name
                FROM
                    overhead_cost
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                ORDER BY
                    detail_no
                ", new
            {
                company_code = companyCode,
                project_sys_id = projectID
            });

            return this._database.Fetch<OverheadCostPlus>(sql);
        }

        /// <summary>
        /// Get all category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="isCreateCopy">Is function copy project info</param>
        /// <param name="copyType">type of copy project info action. 1: normal, 2: all information</param>
        /// <returns>List of category</returns>
        public IList<Category> GetCategoryList(string companyCode, int projectID, bool isCreateCopy, int copyType)
        {
            Sql sql = new Sql();

            if (projectID > 0 && !(isCreateCopy && copyType == Constant.CopyType.NORMAL))
            {
                sql = Sql.Builder.Append(@"
                    SELECT
                        ISNULL(tbCategory.category_id, tbTargetCategory.category_id) AS category_id,
                        ISNULL(tbCategory.category, tbTargetCategory.category) AS category,
                        ISNULL(tbCategory.display_order, tbTargetCategory.display_order) AS display_order
                    FROM (
                        SELECT
                            category_id,
                            category,
                            display_order
                        FROM
                            m_category
                        WHERE
                            company_code = @company_code
                            AND del_flg = '0'
                        ) AS tbCategory
                        FULL JOIN (
                            SELECT
                                target_category.category_id,
                                m_category.category,
                                m_category.display_order
                            FROM target_category INNER JOIN m_category
                                ON target_category.company_code = m_category.company_code
                                AND target_category.category_id = m_category.category_id
                            WHERE target_category.company_code = @company_code
                                AND project_sys_id = @project_sys_id
                        ) AS tbTargetCategory
                        ON tbCategory.category_id = tbTargetCategory.category_id
                    ORDER BY 
                        display_order", new
                {
                    company_code = companyCode,
                    project_sys_id = projectID
                });
            }
            else
            {
                sql = Sql.Builder.Append(@"
                    SELECT
                        category_id,
                        category
                    FROM
                        m_category
                    WHERE
                        company_code = @company_code
                        AND del_flg = '0'
                    ORDER BY 
                        display_order", new
                {
                    company_code = companyCode
                });
            }

            return this._database.Fetch<Category>(sql);
        }

        /// <summary>
        /// Get all sub category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="categoryID">Category ID</param>
        /// <returns>List of sub category</returns>
        public IList<SubCategory> GetSubCategoryList(string companyCode, int projectID, int categoryID)
        {
            Sql sql = new Sql();

            if (projectID > 0)
            {
                sql = Sql.Builder.Append(@"
                    SELECT
                        ISNULL(tbSubCategory.sub_category_id, tbTargetCategory.sub_category_id) AS sub_category_id,
                        ISNULL(tbSubCategory.sub_category, tbTargetCategory.sub_category) AS sub_category,
                        ISNULL(tbSubCategory.display_order, tbTargetCategory.display_order) AS display_order
                    FROM (
                        SELECT
                            sub_category_id,
                            sub_category,
                            display_order
                        FROM
                            m_sub_category
                        WHERE
                            company_code = @company_code
                            AND category_id = @category_id
                            AND del_flg = '0'
                        ) AS tbSubCategory
                        FULL JOIN (
                            SELECT
                                target_category.sub_category_id,
                                m_sub_category.sub_category,
                                m_sub_category.display_order
                            FROM target_category INNER JOIN m_sub_category
                                ON target_category.company_code = m_sub_category.company_code
                                AND target_category.category_id = m_sub_category.category_id
                                AND target_category.sub_category_id = m_sub_category.sub_category_id
                            WHERE target_category.company_code = @company_code
                                AND project_sys_id = @project_sys_id
                                AND target_category.category_id = @category_id
                        ) AS tbTargetCategory
                        ON tbSubCategory.sub_category_id = tbTargetCategory.sub_category_id
                    ORDER BY 
                        display_order", new
                {
                    company_code = companyCode,
                    category_id = categoryID,
                    project_sys_id = projectID
                });
            }
            else
            {
                sql = Sql.Builder.Append(@"
                    SELECT
                        sub_category_id,
                        sub_category
                    FROM
                        m_sub_category
                    WHERE
                        company_code = @company_code
                        AND category_id = @category_id
                        AND del_flg = '0'
                    ORDER BY 
                        display_order", new
                {
                    company_code = companyCode,
                    category_id = categoryID
                });
            }

            return this._database.Fetch<SubCategory>(sql);
        }

        /// <summary>
        /// Get all target category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of target category</returns>
        public IList<TargetCategoryPlus> GetTargetCategoryList(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                SELECT
                    category_id,
                    sub_category_id,
                    (SELECT category FROM m_category WHERE company_code = target_category.company_code AND category_id = target_category.category_id) AS category_name,
                    (SELECT sub_category FROM m_sub_category WHERE company_code = target_category.company_code AND category_id = target_category.category_id AND sub_category_id = target_category.sub_category_id) AS sub_category_name
                FROM
                    target_category
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id ", new
            {
                company_code = companyCode,
                project_sys_id = projectID
            });

            return this._database.Fetch<TargetCategoryPlus>(sql);
        }

        /// <summary>
        /// Get category list by contract type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeId">Contract type ID</param>
        /// <returns>List of category</returns>
        public IEnumerable<ContractTypeCategory> GetDefaultCategoryListByContract(string companyCode, int contractTypeId)
        {
            var condition = new Dictionary<string, object>
                                {
                                    {
                                        "company_code", companyCode
                                    },
                                    {
                                        "contract_type_id", contractTypeId
                                    }
                                };

            return this.Select<ContractTypeCategory>(condition, "display_order");
        }

        /// <summary>
        /// Get all phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of progress</returns>
        public IList<ProgressHistoryPlus> GetProgressList(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                SELECT
                    regist_date,
                    CONVERT(INT, (ISNULL(progress, 0) * 100)) [progress],
                    remarks,
                    ins_date,
                    ins_id
                FROM
                    progress_history
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                ORDER BY
                    regist_date DESC
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID
                 });

            return this._database.Fetch<ProgressHistoryPlus>(sql);
        }

        /// <summary>
        /// Get all attach file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of attach file</returns>
        public IList<ProjectAttachFilePlus> GetFileList(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                SELECT
                    file_no,
                    display_title,
                    file_path,
                    file_name,
                    public_flg
                FROM
                    project_attached_file
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                ORDER BY
                    file_no
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID
                 });

            return this._database.Fetch<ProjectAttachFilePlus>(sql);
        }

        /// <summary>
        /// Get all overhead cost type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of overhead cost type</returns>
        public IList<OverheadCostType> GetOverheadCostTypeList(string companyCode)
        {
            var sql = new Sql(@"
                SELECT
                    overhead_cost_id,
                    overhead_cost_type
                FROM
                    m_overhead_cost
                WHERE
                    company_code = @company_code;
                ",
                 new
                 {
                     company_code = companyCode
                 });

            return this._database.Fetch<OverheadCostType>(sql);
        }

        /// <summary>
        /// Get work day of month
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="month">Month</param>
        /// <param name="year">Year</param>
        /// <returns>Work day of month</returns>
        public IList<int> GetWorkDayOfMonth(string companyCode, int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var sql = new Sql(@"
                SELECT operating_days FROM dbo.GetTableDaysInMonths(@company_code, @sY, @sM, @eY, @eM)",
                 new
                 {
                     company_code = companyCode,
                     sY = fromYear,
                     sM = fromMonth,
                     eY = toYear,
                     eM = toMonth
                 });

            return this._database.Fetch<int>(sql);
        }

        /// <summary>
        /// Get member actual workday
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <param name="fromYear"></param>
        /// <param name="fromMonth"></param>
        /// <param name="toYear"></param>
        /// <param name="toMonth"></param>
        /// <returns></returns>
        public IList<MemberActualWorkDetail> GetMemberActualWorkDay(string companyCode, int projectID, int userID, int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var sql = new Sql(@"
                SELECT
                    actual_work_year
                    , actual_work_month
                    , dbo.Hour2Day(@company_code, SUM(actual_work_time)) AS actual_work_time
                FROM member_actual_work_detail
                WHERE company_code = @company_code
                    AND project_sys_id = @projectID
                    AND user_sys_id = @userID
                    AND (actual_work_year > @sY OR (actual_work_year = @sY AND actual_work_month >= @sM))
                    AND (actual_work_year < @eY OR (actual_work_year = @eY AND actual_work_month <= @eM))
                GROUP BY actual_work_month, actual_work_year
                ORDER BY actual_work_year, actual_work_month",
                 new
                 {
                     company_code = companyCode,
                     projectID = projectID,
                     userID = userID,
                     sY = fromYear,
                     sM = fromMonth,
                     eY = toYear,
                     eM = toMonth
                 });

            return this._database.Fetch<MemberActualWorkDetail>(sql);
        }

        /// <summary>
        /// Get actual work time of member by user ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="userID">User ID</param>
        /// <returns>Actual work time of member</returns>
        public MemberActualWorkDetail GetActualWorkTimeByUser(string companyCode, int projectID, int userID)
        {
            var sql = new Sql(@"
                SELECT
                    SUM(actual_work_time) actual_work_time
                FROM
                    member_actual_work_detail
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND user_sys_id = @user_sys_id
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     user_sys_id = userID
                 });

            return this._database.FirstOrDefault<MemberActualWorkDetail>(sql);
        }

        /// <summary>
        /// Get actual work time of member by phase ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="phaseID">Phase ID</param>
        /// <returns>Actual work time of member</returns>
        public MemberActualWorkDetail GetActualWorkTimeByPhase(string companyCode, int projectID, int phaseID)
        {
            var sql = new Sql(@"
                SELECT
                    SUM(actual_work_time) actual_work_time
                FROM
                    member_actual_work_detail
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND phase_id = @phase_id
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     phase_id = phaseID
                 });

            return this._database.FirstOrDefault<MemberActualWorkDetail>(sql);
        }

        /// <summary>
        /// Get actual work time of member by month year
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="timeArr">Time array to check</param>
        /// <returns>Actual work time by month year</returns>
        public MemberActualWorkDetail GetActualWorkTimeByMonthYear(string companyCode, int projectID, List<string> timeArr)
        {
            var sql = new Sql(@"
                SELECT
                    SUM(actual_work_time) actual_work_time
                FROM
                    member_actual_work_detail
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID
                 });

            for (int i = 0; i < timeArr.Count; i++)
            {
                var time = timeArr[i].ToString().Split('/');

                if (i == 0)
                    sql.Append("AND (");

                sql.Append(@"(actual_work_year = @year AND actual_work_month = @month)",
                    new
                    {
                        year = time[0],
                        month = time[1]
                    });

                if (i == timeArr.Count - 1)
                    sql.Append(")");
                else
                    sql.Append(" OR ");
            }

            return this._database.FirstOrDefault<MemberActualWorkDetail>(sql);
        }

        /// <summary>
        /// Get work time of member by month year
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectDurationArr">Project duration list</param>
        /// <param name="userID">User ID</param>
        /// <returns>Member work time by month year</returns>
        public IList<MemberWorkTime> GetMemberWorkTimeByMonthYear(string companyCode, List<string> projectDurationArr, int userID)
        {
            var sqlConSub = new Sql();
            var sqlConChild = new Sql();

            for (int i = 0; i < projectDurationArr.Count; i++)
            {
                var time = projectDurationArr[i].ToString().Split('/');

                if (i == 0)
                {
                    sqlConSub.Append("AND (");
                    sqlConChild.Append("AND (");
                }

                sqlConSub.Append(@"(tb1.actual_work_year = @year AND tb1.actual_work_month = @month)",
                    new
                    {
                        year = time[0],
                        month = time[1]
                    });
                sqlConChild.Append(@"(actual_work_year = @year AND actual_work_month = @month)",
                    new
                    {
                        year = time[0],
                        month = time[1]
                    });

                if (i == projectDurationArr.Count - 1)
                {
                    sqlConSub.Append(")");
                    sqlConChild.Append(")");
                }
                else
                {
                    sqlConSub.Append(" OR ");
                    sqlConChild.Append(" OR ");
                }
            }

            var sql = new Sql(@"
                SELECT
                    tb1.user_sys_id,
                    (SELECT
                        display_name
                    FROM
                        m_user
                    WHERE
                        company_code = tb1.company_code
                        AND user_sys_id = tb1.ins_id) AS display_name,
                    tb1.regist_type,
                    ISNULL(tb1.total_actual_work, 0) total_actual_work,
                    tb2.attendance_time,
                    tb1.actual_work_year,
                    tb1.actual_work_month
                FROM
                    member_actual_work tb1 LEFT JOIN (
                        SELECT
                            company_code,
                            user_sys_id,
                            actual_work_year,
                            actual_work_month,
                            ISNULL((work_end_time - work_start_time - rest_time), 0) attendance_time
                        FROM
                            attendance_record
                        WHERE
                            company_code = @company_code
                            AND user_sys_id = @user_sys_id
                ",
                 new
                 {
                     company_code = companyCode,
                     user_sys_id = userID
                 });

            sql.Append(sqlConChild);
            sql.Append(@") tb2
                    ON tb2.company_code = tb1.company_code
                    AND tb2.user_sys_id = tb1.user_sys_id
                    AND tb2.actual_work_year = tb1.actual_work_year
                    AND tb2.actual_work_month = tb1.actual_work_month
                WHERE
                    tb1.company_code = @company_code
                    AND tb1.user_sys_id = @user_sys_id",
                 new
                 {
                     company_code = companyCode,
                     user_sys_id = userID
                 });
            sql.Append(sqlConSub);

            return this._database.Fetch<MemberWorkTime>(sql);
        }

        /// <summary>
        /// Get Sale Payment list
        /// </summary>
        /// <param name="startDate">Start date of Project</param>
        /// <param name="endDate">End date of Project</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <param name="orderingflag">Orderingflag</param>
        /// <returns>List of Sale Payment</returns>
        public IList<dynamic> GetSalePaymentList(DateTime startDate,
            DateTime endDate,
            string companyCode,
            int projectID,
            string orderingflag)
        {
            string cols = this.BuildColumnByMonth(startDate, endDate);
            var sb = new StringBuilder();

            sb.Append("SELECT * FROM ( ");
            sb.Append("SELECT ts1.customer_id, (SELECT display_name FROM m_customer WHERE company_code = ts1.company_code AND customer_id = ts1.customer_id) AS customer_name, ");
            sb.Append("(cast(ts1.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(ts1.target_month AS VARCHAR(2)),2)) AS [month], amount FROM ");
            sb.Append("sales_payment_detail ts1 INNER JOIN project_info ts3 ");
            sb.Append("ON ts3.company_code = ts1.company_code ");
            sb.Append("AND ts3.project_sys_id = ts1.project_sys_id ");
            sb.AppendFormat("WHERE ts1.company_code = '{0}' ", companyCode);
            sb.AppendFormat("AND ts1.project_sys_id = {0} ", projectID);
            sb.AppendFormat("AND ts1.ordering_flg = '{0}' ", orderingflag);
            sb.Append(")x PIVOT ( ");
            sb.AppendFormat("sum(amount) FOR [month] in ({0}) ", cols);
            sb.Append(") PIV ORDER BY customer_id");

            var query = new Sql(sb.ToString());

            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Get member assignment list
        /// </summary>
        /// <param name="startDate">Start date of Project</param>
        /// <param name="endDate">End date of Project</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of member assignment</returns>
        public IList<dynamic> GetMemberAssignmentList(DateTime startDate,
            DateTime endDate,
            string companyCode,
            int projectID)
        {
            string cols = this.BuildColumnByMonth(startDate, endDate);
            var sb = new StringBuilder();

            sb.AppendFormat(@"SELECT 
                (SELECT display_name FROM m_user WHERE company_code = '{0}' AND user_sys_id = tsSummary.user_sys_id) AS display_name
                , (SELECT display_name FROM m_group WHERE company_code = '{0}' AND group_id = (SELECT group_id FROM m_user WHERE company_code = '{0}' AND user_sys_id = tsSummary.user_sys_id)) AS group_display_name
                , tsSummary.* FROM ( ", companyCode);
            sb.Append("SELECT ts1.user_sys_id, ts5.total_plan_man_days, ts5.total_plan_cost, ");
            sb.Append("(CAST(ts1.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(ts1.target_month AS VARCHAR(2)),2)) AS [month], ");
            sb.Append("(CAST(SUM(ts1.plan_man_days) AS varchar(8)) + '/' + ");
            sb.Append("(CAST(ts1.individual_sales AS varchar(10))) + '/' + ");
            sb.Append("(CAST((SELECT ISNULL((SELECT TOP 1 base_unit_cost ");
            sb.Append("FROM unit_price_history ");
            sb.AppendFormat("WHERE company_code = '{0}' ", companyCode);
            sb.Append("AND user_sys_id = ts1.user_sys_id ");
            sb.Append("AND apply_start_date <= CONVERT(date, CAST(ts1.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(ts1.target_month AS VARCHAR(2)), 2) + '/01') ");
            sb.AppendFormat("AND del_flg = '{0}' ", Constant.DeleteFlag.NON_DELETE);
            sb.Append("ORDER BY apply_start_date DESC),0) AS base_unit_cost) AS varchar(20)))) AS [detail] ");
            sb.Append("FROM member_assignment_detail ts1 INNER JOIN project_info ts3 ");
            sb.Append("ON ts3.project_sys_id = ts1.project_sys_id ");
            sb.Append("AND ts3.company_code = ts1.company_code INNER JOIN member_assignment ts5 ");
            sb.Append("ON ts5.project_sys_id = ts1.project_sys_id ");
            sb.Append("AND ts5.company_code = ts1.company_code ");
            sb.Append("AND ts5.user_sys_id = ts1.user_sys_id ");
            sb.AppendFormat("WHERE ts1.company_code = '{0}' ", companyCode);
            sb.AppendFormat("AND ts1.project_sys_id = {0} ", projectID);
            sb.Append("GROUP BY ts1.user_sys_id, ts5.total_plan_man_days, ");
            sb.Append("ts5.total_plan_cost, ts1.target_year, ts1.target_month, ts1.individual_sales ");
            sb.Append(")x PIVOT ( ");
            sb.AppendFormat("MAX(detail) FOR [month] in ({0}) ", cols);
            sb.Append(") tsSummary");

            var query = new Sql(sb.ToString());

            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Get overhead cost detail list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project code</param>
        /// <returns>List of overhead cost detail</returns>
        public IList<OverheadCostDetailPlus> GetOverheadCostDetailList(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                SELECT
                    detail_no,
                    ISNULL(amount, 0) AS amount,
                    (CAST(target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(target_month AS VARCHAR(2)),2)) AS target_time
                FROM
                    overhead_cost_detail
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                ORDER BY
                    detail_no, target_time;",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID
                 });

            return this._database.Fetch<OverheadCostDetailPlus>(sql);
        }

        /// <summary>
        /// Get all history of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <returns>List of project history</returns>
        public IList<ProjectInfoHistory> GetHistoryOfProject(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                SELECT
                    history_no,
                    ins_date,
                    delete_status
                FROM
                    project_info_history
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                ORDER BY
                    ins_date DESC
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID
                 });

            return this._database.Fetch<ProjectInfoHistory>(sql);
        }

        /// <summary>
        /// Get history of project info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>History of project info</returns>
        public ProjectInfoHistoryPlus GetProjectInfoHistory(string companyCode, int projectID, int historyID)
        {
            var sql = new Sql(@"
                SELECT
                    tbProject.estimate_man_days,
                    tbProject.total_sales,
                    tbProject.total_payment,
                    tbProject.tax_rate,
                    tbProject.gross_profit,
                    tbProject.delete_status,
                    (SELECT display_name FROM m_user WHERE company_code = tbProject.company_code AND user_sys_id = tbProject.ins_id) AS ins_user,
                    ISNULL((SELECT display_name FROM m_customer WHERE company_code = tbProject.company_code AND customer_id = tbSales.customer_id), '') AS customer_name,
                    ISNULL((SELECT display_name FROM m_customer WHERE company_code = tbProject.company_code AND customer_id = tbSales.end_user_id), '') AS end_user_name
                FROM
                    project_info_history AS tbProject LEFT JOIN sales_payment_history AS tbSales
                    ON tbProject.company_code = tbSales.company_code
                    AND tbProject.project_sys_id = tbSales.project_sys_id
                    AND tbProject.history_no = tbSales.history_no
                    AND tbSales.ordering_flg = '1'
                WHERE
                    tbProject.company_code = @company_code
                    AND tbProject.project_sys_id = @project_sys_id
                    AND tbProject.history_no = @history_no
                ORDER BY
                    tbProject.ins_date DESC
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     history_no = historyID
                 });

            return this._database.FirstOrDefault<ProjectInfoHistoryPlus>(sql);
        }

        /// <summary>
        /// Get target time of project info history
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of target time</returns>
        public IList<string> GetTargetTimeHistory(string companyCode, int projectID, int historyID)
        {
            var sql = new Sql(@"
                SELECT
                    *
                FROM (
                    SELECT
                        ISNULL(ISNULL(byMember.target_time, byPayment.target_time), byOverheadCost.target_time) AS target_time 
                    FROM (
                        SELECT
                            (cast(target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(target_month AS VARCHAR(2)),2)) AS target_time
                        FROM
                            member_assignment_detail_history
                        WHERE
                            company_code = @company_code
                            AND project_sys_id = @project_sys_id
                            AND history_no = @history_no
                        GROUP BY
                            target_month, target_year
                        ) AS byMember FULL JOIN (
                        SELECT
                            (cast(target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(target_month AS VARCHAR(2)),2)) AS target_time
                        FROM
                            payment_detail_history
                        WHERE
                            company_code = @company_code
                            AND project_sys_id = @project_sys_id
                            AND history_no = @history_no
                        GROUP BY
                            target_month, target_year
                        ) AS byPayment
                        ON byMember.target_time = byPayment.target_time
                        FULL JOIN (
                            SELECT
                                (cast(target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(target_month AS VARCHAR(2)),2)) AS target_time
                            FROM
                                overhead_cost_detail_history
                            WHERE
                                company_code = @company_code
                                AND project_sys_id = @project_sys_id
                                AND history_no = @history_no
                            GROUP BY
                                target_month, target_year
                        ) AS byOverheadCost
                        ON byMember.target_time = byOverheadCost.target_time
                    ) AS tbResult
                ORDER BY
                    tbResult.target_time
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     history_no = historyID
                 });

            return this._database.Fetch<string>(sql);
        }

        /// <summary>
        /// Get history of member assignment info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of member assignment info history</returns>
        public IList<MemberAssignmentHistoryPlus> GetMemberAssignmentHistory(string companyCode,
            int projectID,
            int historyID)
        {
            var sql = new Sql(@"
                SELECT
                    user_sys_id,
                    (SELECT display_name
                    FROM m_user
                    WHERE company_code = tbMemberAssignment.company_code
                    AND user_sys_id = tbMemberAssignment.user_sys_id) AS display_name,
                    total_plan_man_days,
                    total_plan_cost
                FROM
                    member_assignment_history AS tbMemberAssignment
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND history_no = @history_no
                ORDER BY
                    user_sys_id
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     history_no = historyID
                 });

            return this._database.Fetch<MemberAssignmentHistoryPlus>(sql);
        }

        /// <summary>
        /// Get history of member assignment detail info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of member assignment detail info history</returns>
        public IList<MemberAssignmentDetailHistoryPlus> GetMemberAssignmentDetailHistory(string companyCode,
            int projectID,
            int historyID)
        {
            var sql = new Sql(@"
                SELECT
                    user_sys_id,
                    (cast(target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(target_month AS VARCHAR(2)),2)) AS target_time,
                    plan_man_days,
                    individual_sales,
                    plan_cost
                FROM
                    member_assignment_detail_history
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND history_no = @history_no
                ORDER BY
                    user_sys_id, target_time
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     history_no = historyID
                 });

            return this._database.Fetch<MemberAssignmentDetailHistoryPlus>(sql);
        }

        /// <summary>
        /// Get history of payment info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of payment info history</returns>
        public IList<SalesPaymentHistoryPlus> GetPaymentHistory(string companyCode,
            int projectID,
            int historyID)
        {
            var sql = new Sql(@"
                SELECT
                    customer_id,
                    (SELECT display_name
                    FROM m_customer
                    WHERE company_code = tbPayment.company_code
                    AND customer_id = tbPayment.customer_id) AS customer_name,
                    charge_person_id,
                    (SELECT display_name
                    FROM m_user
                    WHERE company_code = tbPayment.company_code
                    AND user_sys_id = tbPayment.charge_person_id) AS charge_person_name,
                    total_amount
                FROM
                    sales_payment_history AS tbPayment
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND history_no = @history_no
                    AND ordering_flg = '2'
                ORDER BY
                    customer_id
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     history_no = historyID
                 });

            return this._database.Fetch<SalesPaymentHistoryPlus>(sql);
        }

        /// <summary>
        /// Get history of payment detail info
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of payment detail info history</returns>
        public IList<PaymentDetailHistoryPlus> GetPaymentDetailHistory(string companyCode,
            int projectID,
            int historyID)
        {
            var sql = new Sql(@"
                SELECT
                    customer_id,
                    (cast(target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(target_month AS VARCHAR(2)),2)) AS target_time,
                    amount
                FROM
                    payment_detail_history
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND history_no = @history_no
                ORDER BY
                    customer_id, target_time
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     history_no = historyID
                 });

            return this._database.Fetch<PaymentDetailHistoryPlus>(sql);
        }

        /// <summary>
        /// Get history of overhead cost
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of overhead cost history</returns>
        public IList<OverheadCostHistoryPlus> GetOverheadCostHistory(string companyCode,
            int projectID,
            int historyID)
        {
            var sql = new Sql(@"
                SELECT
                    detail_no,
                    (SELECT overhead_cost_type
                    FROM m_overhead_cost
                    WHERE company_code = tbOverheadCost.company_code
                    AND overhead_cost_id = tbOverheadCost.overhead_cost_id) AS type_name,
                    overhead_cost_detail,
                    charge_person_id,
                    (SELECT display_name
                    FROM m_user
                    WHERE company_code = tbOverheadCost.company_code
                    AND user_sys_id = tbOverheadCost.charge_person_id) AS charge_person_name,
                    total_amount
                FROM
                    overhead_cost_history AS tbOverheadCost
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND history_no = @history_no
                ORDER BY
                    detail_no
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     history_no = historyID
                 });

            return this._database.Fetch<OverheadCostHistoryPlus>(sql);
        }

        /// <summary>
        /// Get history of overhead cost detail
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <returns>List of overhead cost detail history</returns>
        public IList<OverheadCostDetailHistoryPlus> GetOverheadCostDetailHistory(string companyCode,
            int projectID,
            int historyID)
        {
            var sql = new Sql(@"
                SELECT
                    detail_no,
                    (cast(target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(target_month AS VARCHAR(2)),2)) AS target_time,
                    amount
                FROM
                    overhead_cost_detail_history
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND history_no = @history_no
                ORDER BY
                    detail_no, target_time
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     history_no = historyID
                 });

            return this._database.Fetch<OverheadCostDetailHistoryPlus>(sql);
        }

        #endregion

        #region Project Detail
        /// <summary>
        /// Get effort member in project detail
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<dynamic> GetProjectMemberDetail(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            DetailCondition condition)
        {
            string cols = this.BuildColumnByMonth(condition.FROM_DATE, condition.TO_DATE);
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                SELECT
                    tbResult.user_sys_id
                    , tbResult.group_name
                    , tbResult.member_name
                    , {1}
                FROM (
                    SELECT
                        tbMember.user_sys_id
                        , (SELECT display_name 
                                FROM m_group 
                                WHERE company_code = '{0}'
                                AND group_id = (SELECT group_id FROM m_user WHERE company_code = '{0}' AND user_sys_id = tbMember.user_sys_id)
                            ) group_name
                        , (SELECT display_name FROM m_user WHERE company_code = '{0}' AND user_sys_id = tbMember.user_sys_id) AS member_name
                        , CONCAT(ISNULL(tbActualWork.actual_work_time, 0), '/', ", companyCode, cols);
            switch (condition.TIME_UNIT)
            {
                case Constant.TimeUnit.HOUR:
                    sb.Append("dbo.Day2Hour(tbMember.company_code, ISNULL(tbMember.plan_man_days, 0)) ");
                    break;
                case Constant.TimeUnit.MONTH:
                    sb.Append("dbo.Day2Month(tbMember.company_code, tbMember.target_month, tbMember.target_year, ISNULL(tbMember.plan_man_days, 0)) ");
                    break;
                default:
                    sb.Append("tbMember.plan_man_days ");
                    break;
            }

            sb.Append(@"
                            ) plan_actual
                        , (CAST(tbMember.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbMember.target_month AS VARCHAR(2)),2)) target_time
                    FROM
                        member_assignment_detail tbMember
                        LEFT JOIN (
                            SELECT
                                company_code
                                , user_sys_id
                                , actual_work_year
                                , actual_work_month ");

            switch (condition.TIME_UNIT)
            {
                case Constant.TimeUnit.HOUR:
                    sb.Append(@", ISNULL(SUM(dbo.RemoveRoundDecimal(actual_work_time)), 0) actual_work_time ");
                    break;
                case Constant.TimeUnit.MONTH:
                    sb.Append(@", dbo.Hour2Month(company_code, actual_work_month, actual_work_year, SUM(actual_work_time)) actual_work_time ");
                    break;
                default:
                    sb.Append(@", dbo.Hour2Day(company_code, SUM(actual_work_time)) actual_work_time ");
                    break;
            }

            sb.AppendFormat(@"
                            FROM
                                member_actual_work_detail
                            WHERE
                                company_code = '{0}'
                                AND project_sys_id = {1}
                            GROUP BY company_code, user_sys_id, actual_work_year, actual_work_month
                        ) tbActualWork
                        ON tbMember.company_code = tbActualWork.company_code
                        AND tbMember.user_sys_id = tbActualWork.user_sys_id
                        AND tbMember.target_year = tbActualWork.actual_work_year
                        AND tbMember.target_month = tbActualWork.actual_work_month
                    WHERE
                        tbMember.company_code = '{0}'
                        AND tbMember.project_sys_id = {1}
                 ) AS tbPivot pivot (MAX(tbPivot.plan_actual) for tbPivot.target_time IN ({2})) AS tbResult", companyCode, condition.PROJECT_ID, cols);

            var query = new Sql(sb.ToString());
            return Page<dynamic>(startItem, int.MaxValue, columns, sortCol, sortDir, query);
        }

        public PageInfo<dynamic> GetProjectMemberProfitDetail(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            DetailCondition condition)
        {
            string cols = this.BuildColumnByMonth(condition.FROM_DATE, condition.TO_DATE);
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                SELECT
                    tbResult.user_sys_id
                    , tbResult.group_name
                    , tbResult.member_name
                    , {1}
                FROM (
                     SELECT
                        tbMember.user_sys_id
                        , (SELECT display_name 
                                FROM m_group 
                                WHERE company_code = '{0}'
                                AND group_id = (SELECT group_id FROM m_user WHERE company_code = '{0}' AND user_sys_id = tbMember.user_sys_id)
                            ) group_name
                        , (SELECT display_name FROM m_user WHERE company_code = '{0}' AND user_sys_id = tbMember.user_sys_id) AS member_name
                        , CONCAT(ISNULL(tbCost.actual_cost,0) , '/', ISNULL(tbSales.individual_sales, 0)", companyCode, cols);

            sb.AppendFormat(@"
                         ) profit_plan_actual
                        , (CAST(tbSales.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbSales.target_month AS VARCHAR(2)),2)) target_time
                    FROM
                        (
                            SELECT DISTINCT mad.company_code,mad.user_sys_id,mad.project_sys_id,mad.target_month,mad.target_year
                            FROM member_assignment_detail mad
                            WHERE mad.company_code = '{0}'
                            AND mad.project_sys_id = {1}
                        ) AS tbMember
                        LEFT JOIN(
            ", companyCode, condition.PROJECT_ID);

            sb.AppendFormat(@"
                        SELECT
                            ins.company_code
                            , ins.user_sys_id
                            , ins.target_year
                            , ins.target_month
                            , ISNULL(ins.individual_sales, 0) + ISNULL(sp.amount, 0) + ISNULL(oc.amount, 0) AS individual_sales
                        FROM
                        (
                            " + buildQuerySelectIndividualSales(companyCode, condition) + @"
                            ) AS ins
                            LEFT JOIN(
                            " + buildQuerySelectSalesPayment(companyCode, condition) + @"
                            ) AS sp
                        ON ins.company_code = sp.company_code
                            AND ins.target_month = sp.target_month
                            AND ins.target_year = sp.target_year
                            AND ins.user_sys_id = sp.charge_person_id
                            LEFT JOIN(
                            " + buildQuerySelectOverHeadCost(companyCode, condition) + @"
                            ) AS oc
                        ON ins.company_code = oc.company_code
                            AND ins.target_month = oc.target_month
                            AND ins.target_year = oc.target_year
                            AND ins.user_sys_id = oc.charge_person_id
                    ) tbSales
                        ON tbMember.company_code = tbSales.company_code
                        AND tbMember.user_sys_id = tbSales.user_sys_id
                        AND tbMember.target_month = tbSales.target_month
                        AND tbMember.target_year = tbSales.target_year
                    LEFT JOIN(
            ", companyCode, condition.PROJECT_ID);

            sb.AppendFormat(@"
            SELECT 
                            mawd.company_code,
                            mawd.user_sys_id,
                            mawd.project_sys_id,
                            mawd.actual_work_month,
                            mawd.actual_work_year,
                            dbo.RoundNumber('{0}', ((
                                SELECT TOP 1 base_unit_cost 
                                FROM unit_price_history uph 
                                WHERE uph.user_sys_id = mawd.user_sys_id 
                                    AND uph.apply_start_date <= CONVERT(date, CAST(mawd.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mawd.actual_work_month AS VARCHAR(2)), 2) + '/01') ORDER BY apply_start_date DESC) 
                                * ISNULL(dbo.Hour2Day(mawd.company_code, SUM(mawd.actual_work_time)), 0) / dbo.GetNumberOfWorkDaysInMonth(mawd.company_code, mawd.actual_work_month,  mawd.actual_work_year)))
                            AS actual_cost
                            FROM member_actual_work_detail mawd
                            INNER JOIN project_info pi 
                                ON pi.company_code = mawd.company_code
                                AND pi.project_sys_id = mawd.project_sys_id
                            WHERE
                                mawd.company_code = '{0}'
                                AND mawd.project_sys_id = {1}
                            GROUP BY mawd.user_sys_id, mawd.company_code, mawd.actual_work_month, mawd.actual_work_year,mawd.project_sys_id
                        )AS tbCost
                        ON tbCost.company_code = tbSales.company_code
                        AND tbCost.user_sys_id = tbSales.user_sys_id
                        AND tbCost.actual_work_month = tbSales.target_month
                        AND tbCost.actual_work_year = tbSales.target_year
                     ) AS tbPivot pivot (MAX(tbPivot.profit_plan_actual) for tbPivot.target_time IN ({2})) AS tbResult
            ", companyCode, condition.PROJECT_ID, cols);
            var query = new Sql(sb.ToString());
            return Page<dynamic>(startItem, int.MaxValue, columns, sortCol, sortDir, query);
        }

        /// <summary>
        /// build Query Select Individual Sales
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private StringBuilder buildQuerySelectIndividualSales(string companyCode, DetailCondition condition)
        {
            StringBuilder selectIndividualSales = new StringBuilder(); //Get Individual Sales Data Query
            selectIndividualSales.AppendFormat(@"
                       SELECT 
                        us.company_code
                        , us.user_sys_id
                        , mad.target_year
                        , mad.target_month 
                        , SUM(CASE WHEN m_status.sales_type = '2'
                            THEN 0
                            ELSE mad.individual_sales END) AS individual_sales
                        FROM m_user us
                            INNER JOIN member_assignment_detail mad
                                ON us.company_code = mad.company_code
                                AND us.user_sys_id = mad.user_sys_id
                            INNER JOIN project_info pi
                                ON mad.company_code = pi.company_code 
                                AND mad.project_sys_id = pi.project_sys_id
                            INNER JOIN m_status 
                                ON pi.company_code = m_status.company_code 
                                AND pi.status_id = m_status.status_id 
                            WHERE
                                mad.company_code = '{0}'
                                AND mad.project_sys_id = {1}
                        GROUP BY us.user_sys_id
                                , us.company_code
                                , mad.target_year
                                , mad.target_month
            ", companyCode, condition.PROJECT_ID);

            return selectIndividualSales;
        }


        /// <summary>
        /// build Query Select Sales Payment
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private StringBuilder buildQuerySelectSalesPayment(string companyCode, DetailCondition condition)
        {
            StringBuilder selectSelectSalesPayment = new StringBuilder(); //Get Sales payment Data Query
            selectSelectSalesPayment.AppendFormat(@"
                        --get sales payment
                            SELECT 
                                spCounting.company_code
                                , spCounting.charge_person_id
                                , spCounting.target_year
                                , spCounting.target_month
                                , SUM(spCounting.amount) AS amount
                            FROM
                            (
                                SELECT spd.company_code
                                        , CASE WHEN sp.charge_person_id IS NULL
                                                THEN pri.charge_person_id
                                                ELSE sp.charge_person_id
                                            END AS charge_person_id
                                        , spd.target_year
                                        , spd.target_month
                                        , CASE WHEN m_status.sales_type = '2'
                                            THEN 0
                                            ELSE spd.amount END AS amount
                                        , sp.project_sys_id
                                FROM sales_payment_detail AS spd 
                                    INNER JOIN sales_payment AS sp
                                    ON sp.company_code=spd.company_code 
                                        AND sp.project_sys_id = spd.project_sys_id
                                        AND sp.ordering_flg = spd.ordering_flg
                                        AND sp.customer_id = spd.customer_id
                                    INNER JOIN project_info AS pri
                                    ON sp.company_code = pri.company_code
                                        AND sp.project_sys_id = pri.project_sys_id
                                    INNER JOIN m_status 
                                    ON pri.company_code = m_status.company_code 
                                        AND pri.status_id = m_status.status_id 
                                    WHERE spd.company_code = '{0}'
                                    AND spd.project_sys_id = {1}
                            ) AS spCounting
                            GROUP BY
                                spCounting.company_code
                                , spCounting.charge_person_id
                                , spCounting.target_year
                                , spCounting.target_month
                            --get sales payment-- end
            ", companyCode, condition.PROJECT_ID);

            return selectSelectSalesPayment;
        }

        /// <summary>
        /// build Query Select Over Head Cost
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private StringBuilder buildQuerySelectOverHeadCost(string companyCode, DetailCondition condition)
        {
            StringBuilder selectSelectOverHeadCost = new StringBuilder(); //Get Over Head Cost Data Query
            selectSelectOverHeadCost.AppendFormat(@"
                        --get overhead cost
                            SELECT 
                                ocCounting.company_code
                                , ocCounting.charge_person_id
                                , ocCounting.target_year
                                , ocCounting.target_month
                                , SUM(ocCounting.amount) AS amount
                                FROM
                                (
                                SELECT ocd.company_code
                                    , CASE WHEN oc.charge_person_id IS NULL
                                            THEN pri.charge_person_id
                                            ELSE oc.charge_person_id
                                        END AS charge_person_id
                                    , ocd.target_year
                                    , ocd.target_month
                                    , CASE WHEN m_status.sales_type = '2'
                                        THEN 0
                                        ELSE ocd.amount END AS amount
                                    , oc.project_sys_id
                                FROM overhead_cost_detail AS ocd 
                                    INNER JOIN overhead_cost AS oc
                                    ON oc.company_code=ocd.company_code 
                                        AND oc.project_sys_id = ocd.project_sys_id
                                        AND oc.detail_no = ocd.detail_no
                                    INNER JOIN project_info AS pri
                                    ON oc.company_code = pri.company_code
                                        AND oc.project_sys_id = pri.project_sys_id
                                    INNER JOIN m_status 
                                    ON pri.company_code = m_status.company_code 
                                        AND pri.status_id = m_status.status_id
                                    WHERE ocd.company_code = '{0}'
                                    AND ocd.project_sys_id = {1}
                                ) AS ocCounting
                                GROUP BY
                                ocCounting.company_code
                                , ocCounting.charge_person_id
                                , ocCounting.target_year
                                , ocCounting.target_month
                                --get overhead cost-- end
            ", companyCode, condition.PROJECT_ID);

            return selectSelectOverHeadCost;
        }

        /// <summary>
        /// Get actual worktime in project by phase
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public IList<TargetPhasePlus> GetWorkTimeByPhase(string companyCode, int projectID, string timeUnit)
        {
            var sql = new Sql();

            if (Constant.TimeUnit.HOUR.Equals(timeUnit))
            {
                sql.Append(@"
                    SELECT
                        tbPhase.phase_id
                        ,dbo.Day2Hour(tbPhase.company_code,tbPhase.estimate_man_days) as estimate_man_days
                        , (SELECT display_name FROM m_phase WHERE company_code = tbPhase.company_code AND phase_id = tbPhase.phase_id) AS phase_name
                        , (SELECT estimate_target_flg FROM m_phase WHERE company_code = tbPhase.company_code AND phase_id = tbPhase.phase_id) AS estimate_target_flg
                        , tbWorkTime.total_actual_work
                    FROM 
                        target_phase tbPhase
                        LEFT JOIN (
                            SELECT
                                company_code
                                , phase_id
                                , SUM(actual_work_time) total_actual_work
                            FROM
                                member_actual_work_detail
                            WHERE
                                company_code = @company_code
                                AND project_sys_id = @project_sys_id
                            GROUP BY
                                company_code
                                , phase_id
                        ) tbWorkTime
                            ON tbWorkTime.company_code = tbPhase.company_code
                            AND tbWorkTime.phase_id = tbPhase.phase_id
                    WHERE 
                        tbPhase.company_code = @company_code
                        AND tbPhase.project_sys_id = @project_sys_id", new { company_code = companyCode, project_sys_id = projectID });
            }
            else if (Constant.TimeUnit.MONTH.Equals(timeUnit))
            {
                sql.Append(@"
                    SELECT
                        tbResult.phase_id
                        , (tbResult.estimate_man_days/30) as estimate_man_days
                        , (SELECT display_name FROM m_phase WHERE company_code = tbResult.company_code AND phase_id = tbResult.phase_id) AS phase_name
                        , (SELECT estimate_target_flg FROM m_phase WHERE company_code = tbResult.company_code AND phase_id = tbResult.phase_id) AS estimate_target_flg
                        , tbResult.total_actual_work
                    FROM (
                        SELECT
                            tbPhase.company_code
                            , tbPhase.phase_id
                            , tbPhase.estimate_man_days
                            , SUM(tbWorkTime.total_actual_work) total_actual_work
                        FROM
                            target_phase tbPhase
                            LEFT JOIN (
                                SELECT
                                    company_code
                                    , phase_id
                                    , dbo.Hour2Month(company_code, actual_work_month, actual_work_year, SUM(actual_work_time)) total_actual_work
                                FROM
                                    member_actual_work_detail
                                WHERE
                                    company_code = @company_code
                                    AND project_sys_id = @project_sys_id
                                GROUP BY
                                    company_code
                                    , phase_id
                                    , actual_work_year
                                    , actual_work_month
                            ) tbWorkTime
                                ON tbWorkTime.company_code = tbPhase.company_code
                                AND tbWorkTime.phase_id = tbPhase.phase_id
                        WHERE
                            tbPhase.company_code = @company_code
                            AND tbPhase.project_sys_id = @project_sys_id
                    GROUP BY
                        tbPhase.company_code
                        , tbPhase.phase_id
                        , tbPhase.estimate_man_days
                    ) tbResult", new { company_code = companyCode, project_sys_id = projectID });
            }
            else
            {
                sql.Append(@"
                    SELECT
                        tbPhase.phase_id
                        ,tbPhase.estimate_man_days
                        , (SELECT display_name FROM m_phase WHERE company_code = tbPhase.company_code AND phase_id = tbPhase.phase_id) AS phase_name
                        , (SELECT estimate_target_flg FROM m_phase WHERE company_code = tbPhase.company_code AND phase_id = tbPhase.phase_id) AS estimate_target_flg
                        , tbWorkTime.total_actual_work
                    FROM 
                        target_phase tbPhase
                        LEFT JOIN (
                            SELECT
                                company_code
                                , phase_id
                                , dbo.Hour2Day(company_code, SUM(actual_work_time)) total_actual_work
                            FROM
                                member_actual_work_detail
                            WHERE
                                company_code = @company_code
                                AND project_sys_id = @project_sys_id
                            GROUP BY
                                company_code
                                , phase_id
                        ) tbWorkTime
                            ON tbWorkTime.company_code = tbPhase.company_code
                            AND tbWorkTime.phase_id = tbPhase.phase_id
                    WHERE 
                        tbPhase.company_code = @company_code
                        AND tbPhase.project_sys_id = @project_sys_id", new { company_code = companyCode, project_sys_id = projectID });
            }

            return this._database.Fetch<TargetPhasePlus>(sql);
        }

        /// <summary>
        /// Get user actual work time by phase in project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ProjectInfoPlus GetActualWorkDetailInfo(string companyCode, int projectID, int userID)
        {
            var sql = new Sql(@"
                SELECT
                    project_name
                    , start_date
                    , end_date
                    , display_name charge_person
                    , (SELECT display_name FROM m_group WHERE company_code = @company_code AND group_id = tbUser.group_id) group_name
                FROM
                    project_info, m_user tbUser
                WHERE
                    (project_info.company_code = @company_code OR tbUser.company_code = @company_code)
                    AND project_sys_id = @project_sys_id
                    AND user_sys_id = @user_sys_id",
                 new { company_code = companyCode, project_sys_id = projectID, user_sys_id = userID });

            return this._database.SingleOrDefault<ProjectInfoPlus>(sql);
        }

        /// <summary>
        /// Get actual work time in project by phase list
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<dynamic> GetUserWorkTimeByPhase(int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            ActualWorkCondition condition)
        {
            string cols = this.BuildColumnByMonth(condition.FROM_DATE, condition.TO_DATE);
            var sb = new StringBuilder();

            sb.AppendFormat(@"SELECT * FROM (
                SELECT
                    tbPhase.phase_id
                    , (SELECT display_order
                        FROM r_contract_type_phase
                        WHERE company_code = tbPhase.company_code
                            AND contract_type_id = (SELECT contract_type_id FROM project_info WHERE company_code = '{1}' AND project_sys_id = {2})
                            AND phase_id = tbPhase.phase_id
                    ) AS display_order
                    , (SELECT display_name FROM m_phase WHERE company_code = tbPhase.company_code AND phase_id = tbPhase.phase_id) AS phase_name
                    , {0}
                FROM 
                    target_phase tbPhase
                    LEFT JOIN (
                            SELECT
                                tbResult.*
                            FROM (
                                SELECT
                                    company_code
                                    , phase_id
                                    , (cast(actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(actual_work_month AS VARCHAR(2)),2)) AS [month]"
                , cols, companyCode, condition.PROJECT_ID);

            switch (condition.TIME_UNIT)
            {
                case Constant.TimeUnit.HOUR:
                    sb.Append(", SUM(dbo.RemoveRoundDecimal(actual_work_time)) total_actual_work ");
                    break;
                case Constant.TimeUnit.MONTH:
                    sb.Append(@", dbo.Hour2Month(company_code, actual_work_month, actual_work_year, SUM(actual_work_time)) total_actual_work ");
                    break;
                default:
                    sb.Append(@", dbo.Hour2Day(company_code, SUM(actual_work_time)) total_actual_work ");
                    break;
            }

            sb.AppendFormat(@"
                                FROM
                                    member_actual_work_detail
                                WHERE
                                    company_code = '{1}'
                                    AND project_sys_id = {2}
                                    AND user_sys_id = {3}
                                GROUP BY
                                    company_code
                                    , phase_id
                                    , actual_work_year
                                    , actual_work_month
                                ) x PIVOT (
                                    SUM(total_actual_work) FOR [month] in ({0})
                            ) tbResult
                        ) tbWorkTime
                        ON tbWorkTime.company_code = tbPhase.company_code
                        AND tbWorkTime.phase_id = tbPhase.phase_id
                WHERE 
                    tbPhase.company_code = '{1}'
                    AND tbPhase.project_sys_id = {2} ) tbResult", cols, companyCode, condition.PROJECT_ID, condition.USER_ID);

            var sql = new Sql(sb.ToString());

            return Page<dynamic>(startItem, int.MaxValue, columns, sortCol, sortDir, sql);
        }

        #endregion

        #region Project Plan
        /// <summary>
        /// Get Project Plan Information by Project ID and Company Code
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <param name="companyCode">Company code</param>
        /// <returns>Project Plan Information</returns>
        public ProjectPlanInfoPlus GetProjectPlanInfo(int projectID, string companyCode)
        {
            var sql = new Sql(@"
               SELECT
                    tblProjectInfo.project_sys_id,
                    tblProjectInfo.project_name,
                    (SELECT display_name FROM m_customer WHERE tblSalesPayment.customer_id = customer_id) AS customer_name,
                    (SELECT display_name FROM m_user WHERE tblProjectInfo.charge_person_id = user_sys_id) AS person_in_charge,
                    (SELECT display_name FROM m_user WHERE tblProjectInfo.charge_of_sales_id = user_sys_id) AS sales_person_in_charge,    
                    tblProjectPlanInfo.*,
                    (SELECT display_name from m_user WHERE user_sys_id = tblProjectPlanInfo.upd_id ) AS user_update,
                    (SELECT display_name from m_user WHERE user_sys_id = tblProjectPlanInfo.ins_id ) AS user_regist
                FROM
                    project_info AS tblProjectInfo
                    LEFT JOIN sales_payment AS tblSalesPayment
                    ON tblProjectInfo.company_code = tblSalesPayment.company_code
                    AND tblProjectInfo.project_sys_id = tblSalesPayment.project_sys_id
                    ANd tblSalesPayment.ordering_flg = '1'
                    LEFT JOIN
                    project_plan_info AS tblProjectPlanInfo
                    ON tblProjectPlanInfo.company_code = tblProjectInfo.company_code
                    AND tblProjectPlanInfo.project_sys_id = tblProjectInfo.project_sys_id
                WHERE
                    tblProjectInfo.company_code = @company_code
                    and tblProjectInfo.project_sys_id = @projectID"
            , new { company_code = companyCode, projectID = projectID });

            return this._database.Single<ProjectPlanInfoPlus>(sql);
        }

        /// <summary>
        /// function insert/update project plan information
        /// </summary>
        /// <param name="data">ProjectPlanInfo</param>
        /// <returns>number of record is update/insert</returns>
        public int EditProjectPlanData(ProjectPlanInfo data)
        {
            int res = 0;
            Sql sql;

            if (GetProjectPlan(data.project_sys_id, data.company_code) > 0)
            {
                IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                    { "company_code", data.company_code }
                    , { "issues", string.IsNullOrEmpty(data.issues) ? null : data.issues }
                    , { "purpose", string.IsNullOrEmpty(data.purpose) ? null : data.purpose }
                    , { "target_01", string.IsNullOrEmpty(data.target_01) ? null : data.target_01 }
                    , { "target_02", string.IsNullOrEmpty(data.target_02) ? null : data.target_02 }
                    , { "target_03", string.IsNullOrEmpty(data.target_03) ? null : data.target_03 }
                    , { "target_04", string.IsNullOrEmpty(data.target_04) ? null : data.target_04 }
                    , { "target_05", string.IsNullOrEmpty(data.target_05) ? null : data.target_05 }
                    , { "target_06", string.IsNullOrEmpty(data.target_06) ? null : data.target_06 }
                    , { "target_07", string.IsNullOrEmpty(data.target_07) ? null : data.target_07 }
                    , { "target_08", string.IsNullOrEmpty(data.target_08) ? null : data.target_08 }
                    , { "target_09", string.IsNullOrEmpty(data.target_09) ? null : data.target_09 }
                    , { "target_10", string.IsNullOrEmpty(data.target_10) ? null : data.target_10 }
                    , { "restriction_01", string.IsNullOrEmpty(data.restriction_01) ? null : data.restriction_01 }
                    , { "restriction_02", string.IsNullOrEmpty(data.restriction_02) ? null : data.restriction_02 }
                    , { "restriction_03", string.IsNullOrEmpty(data.restriction_03) ? null : data.restriction_03 }
                    , { "restriction_04", string.IsNullOrEmpty(data.restriction_04) ? null : data.restriction_04 }
                    , { "restriction_05", string.IsNullOrEmpty(data.restriction_05) ? null : data.restriction_05 }
                    , { "restriction_06", string.IsNullOrEmpty(data.restriction_06) ? null : data.restriction_06 }
                    , { "concerns_01", string.IsNullOrEmpty(data.concerns_01) ? null : data.concerns_01 }
                    , { "measures_01", string.IsNullOrEmpty(data.measures_01) ? null : data.measures_01 }
                    , { "concerns_02", string.IsNullOrEmpty(data.concerns_02) ? null : data.concerns_02 }
                    , { "measures_02", string.IsNullOrEmpty(data.measures_02) ? null : data.measures_02 }
                    , { "concerns_03", string.IsNullOrEmpty(data.concerns_03) ? null : data.concerns_03 }
                    , { "measures_03", string.IsNullOrEmpty(data.measures_03) ? null : data.measures_03 }
                    , { "concerns_04", string.IsNullOrEmpty(data.concerns_04) ? null : data.concerns_04 }
                    , { "measures_04", string.IsNullOrEmpty(data.measures_04) ? null : data.measures_04 }
                    , { "concerns_05", string.IsNullOrEmpty(data.concerns_05) ? null : data.concerns_05 }
                    , { "measures_05", string.IsNullOrEmpty(data.measures_05) ? null : data.measures_05 }
                    , { "support_test_plan_flg", string.IsNullOrEmpty(data.support_test_plan_flg) ? null : data.support_test_plan_flg }
                    , { "support_user_test_flg", string.IsNullOrEmpty(data.support_user_test_flg) ? null : data.support_user_test_flg }
                    , { "support_stress_test_flg", string.IsNullOrEmpty(data.support_stress_test_flg) ? null : data.support_stress_test_flg }
                    , { "support_security_test_flg", string.IsNullOrEmpty(data.support_security_test_flg) ? null : data.support_security_test_flg }
                    , { "remarks", string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                    , { "upd_date", data.upd_date }
                    , { "upd_id", data.upd_id }
                    , { "del_flg", data.del_flg }
                };

                IDictionary<string, object> condition;
                if (data.row_version == null)
                {
                    condition = new Dictionary<string, object>()
                    {
                        { "project_sys_id", data.project_sys_id }
                    };
                }
                else
                {
                    condition = new Dictionary<string, object>()
                    {
                        { "project_sys_id", data.project_sys_id }, {"row_version", data.row_version}
                    };
                }

                if (Update<ProjectPlanInfo>(columns, condition) > 0)
                {
                    res = data.project_sys_id;
                }
            }
            else
            {
                data.ins_date = data.upd_date;
                data.ins_id = data.upd_id;

                sql = new Sql(@"
                    INSERT INTO [dbo].[project_plan_info]
                               (company_code
                               ,project_sys_id
                               ,issues
                               ,purpose
                               ,target_01
                               ,target_02
                               ,target_03
                               ,target_04
                               ,target_05
                               ,target_06
                               ,target_07
                               ,target_08
                               ,target_09
                               ,target_10
                               ,restriction_01
                               ,restriction_02
                               ,restriction_03
                               ,restriction_04
                               ,restriction_05
                               ,restriction_06
                               ,concerns_01
                               ,measures_01
                               ,concerns_02
                               ,measures_02
                               ,concerns_03
                               ,measures_03
                               ,concerns_04
                               ,measures_04
                               ,concerns_05
                               ,measures_05
                               ,support_test_plan_flg
                               ,support_user_test_flg
                               ,support_stress_test_flg
                               ,support_security_test_flg
                               ,remarks
                               ,ins_date
                               ,ins_id
                               ,upd_date
                               ,upd_id
                               ,del_flg)
                         VALUES
                               (
                                @company_code
                               ,@project_sys_id
                               ,@issues
                               ,@purpose
                               ,@target_01
                               ,@target_02
                               ,@target_03
                               ,@target_04
                               ,@target_05
                               ,@target_06
                               ,@target_07
                               ,@target_08
                               ,@target_09
                               ,@target_10
                               ,@restriction_01
                               ,@restriction_02
                               ,@restriction_03
                               ,@restriction_04
                               ,@restriction_05
                               ,@restriction_06
                               ,@concerns_01
                               ,@measures_01
                               ,@concerns_02
                               ,@measures_02
                               ,@concerns_03
                               ,@measures_03
                               ,@concerns_04
                               ,@measures_04
                               ,@concerns_05
                               ,@measures_05
                               ,@support_test_plan_flg
                               ,@support_user_test_flg
                               ,@support_stress_test_flg
                               ,@support_security_test_flg
                               ,@remarks
                               ,@ins_date
                               ,@ins_id
                               ,@upd_date
                               ,@upd_id
                               ,@del_flg)"
                            , new { project_sys_id = data.project_sys_id }
                            , new { company_code = data.company_code }
                            , new { issues = string.IsNullOrEmpty(data.issues) ? null : data.issues }
                            , new { purpose = string.IsNullOrEmpty(data.purpose) ? null : data.purpose }
                            , new { target_01 = string.IsNullOrEmpty(data.target_01) ? null : data.target_01 }
                            , new { target_02 = string.IsNullOrEmpty(data.target_02) ? null : data.target_02 }
                            , new { target_03 = string.IsNullOrEmpty(data.target_03) ? null : data.target_03 }
                            , new { target_04 = string.IsNullOrEmpty(data.target_04) ? null : data.target_04 }
                            , new { target_05 = string.IsNullOrEmpty(data.target_05) ? null : data.target_05 }
                            , new { target_06 = string.IsNullOrEmpty(data.target_06) ? null : data.target_06 }
                            , new { target_07 = string.IsNullOrEmpty(data.target_07) ? null : data.target_07 }
                            , new { target_08 = string.IsNullOrEmpty(data.target_08) ? null : data.target_08 }
                            , new { target_09 = string.IsNullOrEmpty(data.target_09) ? null : data.target_09 }
                            , new { target_10 = string.IsNullOrEmpty(data.target_10) ? null : data.target_10 }
                            , new { restriction_01 = string.IsNullOrEmpty(data.restriction_01) ? null : data.restriction_01 }
                            , new { restriction_02 = string.IsNullOrEmpty(data.restriction_02) ? null : data.restriction_02 }
                            , new { restriction_03 = string.IsNullOrEmpty(data.restriction_03) ? null : data.restriction_03 }
                            , new { restriction_04 = string.IsNullOrEmpty(data.restriction_04) ? null : data.restriction_04 }
                            , new { restriction_05 = string.IsNullOrEmpty(data.restriction_05) ? null : data.restriction_05 }
                            , new { restriction_06 = string.IsNullOrEmpty(data.restriction_06) ? null : data.restriction_06 }
                            , new { concerns_01 = string.IsNullOrEmpty(data.concerns_01) ? null : data.concerns_01 }
                            , new { measures_01 = string.IsNullOrEmpty(data.measures_01) ? null : data.measures_01 }
                            , new { concerns_02 = string.IsNullOrEmpty(data.concerns_02) ? null : data.concerns_02 }
                            , new { measures_02 = string.IsNullOrEmpty(data.measures_02) ? null : data.measures_02 }
                            , new { concerns_03 = string.IsNullOrEmpty(data.concerns_03) ? null : data.concerns_03 }
                            , new { measures_03 = string.IsNullOrEmpty(data.measures_03) ? null : data.measures_03 }
                            , new { concerns_04 = string.IsNullOrEmpty(data.concerns_04) ? null : data.concerns_04 }
                            , new { measures_04 = string.IsNullOrEmpty(data.measures_04) ? null : data.measures_04 }
                            , new { concerns_05 = string.IsNullOrEmpty(data.concerns_05) ? null : data.concerns_05 }
                            , new { measures_05 = string.IsNullOrEmpty(data.measures_05) ? null : data.measures_05 }
                            , new { support_test_plan_flg = string.IsNullOrEmpty(data.support_test_plan_flg) ? null : data.support_test_plan_flg }
                            , new { support_user_test_flg = string.IsNullOrEmpty(data.support_user_test_flg) ? null : data.support_user_test_flg }
                            , new { support_stress_test_flg = string.IsNullOrEmpty(data.support_stress_test_flg) ? null : data.support_stress_test_flg }
                            , new { support_security_test_flg = string.IsNullOrEmpty(data.support_security_test_flg) ? null : data.support_security_test_flg }
                            , new { remarks = string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                            , new { ins_date = data.ins_date }
                            , new { ins_id = data.ins_id }
                            , new { upd_date = data.upd_date }
                            , new { upd_id = data.upd_id }
                            , new { del_flg = data.del_flg }
                         );

                if (_database.Execute(sql) > 0)
                {
                    res = data.project_sys_id;
                }
            }

            return res;
        }

        #endregion

        #endregion

        #region Private Method
        /// <summary>
        /// Get latest project no
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>Latest project no</returns>
        private int GetLatestProjectNo(string companyCode)
        {
            var sql = new Sql(@"
                SELECT 
                    ISNULL(MAX(latest_project_no), 0)
                FROM
                    numbering
                WHERE
                    company_code = @company_code",
                 new
                 {
                     company_code = companyCode
                 });

            return this._database.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// Insert latest project no
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectNo">Latest project no</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert user ID</param>
        private void InsertLatestProjectNo(string companyCode, string projectNo, DateTime? insDate, int insUser)
        {
            var sql = new Sql(@"
                INSERT INTO
                    numbering
                    (company_code,
                    latest_project_no,
                    ins_date,
                    ins_id,
                    upd_date,
                    upd_id)
                VALUES
                    (@company_code,
                    @latest_project_no,
                    @ins_date,
                    @ins_id,
                    @upd_date,
                    @upd_id);",
                 new
                 {
                     company_code = companyCode,
                     latest_project_no = projectNo,
                     ins_date = insDate,
                     ins_id = insUser,
                     upd_date = insDate,
                     upd_id = insUser
                 });

            this._database.Execute(sql);
        }

        /// <summary>
        /// Update latest project no
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectNo">Latest project no</param>
        /// <param name="updDate">Update date</param>
        /// <param name="updUser">Update user ID</param>
        private void UpdateLatestProjectNo(string companyCode, string projectNo, DateTime? updDate, int updUser)
        {
            var sql = new Sql(@"
                UPDATE
                    numbering
                SET
                    latest_project_no = @latest_project_no,
                    upd_date = @upd_date,
                    upd_id = @upd_id
                WHERE
                    company_code = @company_code;",
                 new
                 {
                     latest_project_no = projectNo,
                     upd_date = updDate,
                     upd_id = updUser,
                     company_code = companyCode
                 });

            this._database.Execute(sql);
        }

        /// <summary>
        /// Edit target phase of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="phaseList">Phase list</param>
        private void EditTargetPhase(string companyCode,
            int projectID,
            DateTime? insDate,
            int insUser,
            IList<PhasePlus> phaseList)
        {
            var sqlDelete = new Sql(
                @"
                DELETE FROM
                    target_phase
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id;",
                new
                {
                    company_code = companyCode,
                    project_sys_id = projectID
                });

            this._database.Execute(sqlDelete);

            var sqlInsert = new Sql();

            foreach (PhasePlus phase in phaseList)
            {
                if (phase.check)
                {
                    sqlInsert.Append(@"
                        INSERT INTO 
                            target_phase
                            (company_code,
                            project_sys_id,
                            phase_id,
                            estimate_man_days,
                            ins_date,
                            ins_id)
                        VALUES
                            (@company_code, @project_sys_id, @phase_id, @estimate_man_days, @ins_date, @ins_id);",
                        new
                        {
                            company_code = companyCode,
                            project_sys_id = projectID,
                            phase_id = phase.phase_id,
                            estimate_man_days = phase.estimate_man_days,
                            ins_date = insDate,
                            ins_id = insUser
                        });
                }
            }

            this._database.Execute(sqlInsert);
        }

        /// <summary>
        /// Edit target category of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="targetCategoryList">Target category list</param>
        private void EditTargetCategory(string companyCode,
            int projectID,
            DateTime? insDate,
            int insUser,
            IList<TargetCategoryPlus> targetCategoryList)
        {
            var sqlDelete = new Sql(
                @"
                DELETE FROM
                    target_category
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id;",
                new
                {
                    company_code = companyCode,
                    project_sys_id = projectID
                });

            this._database.Execute(sqlDelete);

            var sqlInsert = new Sql();

            if (targetCategoryList != null)
            {
                foreach (TargetCategory item in targetCategoryList)
                {
                    if (item.category_id != null && item.sub_category_id != null)
                    {
                        sqlInsert.Append(@"
                        INSERT INTO 
                            target_category
                            (company_code,
                            project_sys_id,
                            category_id,
                            sub_category_id,
                            ins_date,
                            ins_id)
                        VALUES
                            (@company_code, @project_sys_id, @category_id, @sub_category_id, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                category_id = item.category_id,
                                sub_category_id = item.sub_category_id,
                                ins_date = insDate,
                                ins_id = insUser
                            });
                    }
                }

                if (!sqlInsert.SQL.Equals(string.Empty))
                    this._database.Execute(sqlInsert);
            }
        }

        /// <summary>
        /// Edit sales payment of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="orderingFlg">Ordering flag</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="salesPaymentList">Sales payment list</param>
        /// <param name="salesPaymentListDetail">Sales payment detail list</param>
        private void EditSalesPayment(string companyCode,
            int projectID,
            string orderingFlg,
            DateTime? insDate,
            int insUser,
            IList<SalesPaymentPlus> salesPaymentList,
            IList<SalesPaymentDetailPlus> salesPaymentListDetail)
        {
            var sqlDeleteDetail = new Sql(@"
                DELETE FROM
                    sales_payment_detail
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND ordering_flg = @ordering_flg;",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID,
                     ordering_flg = orderingFlg
                 });
            this._database.Execute(sqlDeleteDetail);

            var sqlDelete = new Sql(@"
                DELETE FROM
                    sales_payment
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                    AND ordering_flg = @ordering_flg;",
                new
                {
                    company_code = companyCode,
                    project_sys_id = projectID,
                    ordering_flg = orderingFlg
                });

            this._database.Execute(sqlDelete);

            if (salesPaymentList != null)
            {
                foreach (SalesPaymentPlus salesPayment in salesPaymentList)
                {
                    if (salesPayment.customer_id != null)
                    {
                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                sales_payment
                                (company_code,
                                project_sys_id,
                                ordering_flg,
                                customer_id,
                                end_user_id,
                                charge_person_id,
                                total_amount,
                                tag_id,
                                ins_date,
                                ins_id)
                            VALUES
                            (@company_code, @project_sys_id, @ordering_flg, @customer_id, @end_user_id,
                            @charge_person_id, @total_amount, @tag_id, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                ordering_flg = orderingFlg,
                                customer_id = salesPayment.customer_id,
                                end_user_id = salesPayment.end_user_id,
                                charge_person_id = salesPayment.charge_person_id ?? null,
                                total_amount = salesPayment.total_amount ?? 0,
                                tag_id = salesPayment.tag_id ?? null,
                                ins_date = insDate,
                                ins_id = insUser
                            });
                        this._database.Execute(sqlInsert);
                    }
                }
            }

            if (salesPaymentListDetail != null)
            {
                foreach (SalesPaymentDetailPlus salesPaymentDetail in salesPaymentListDetail)
                {
                    if (salesPaymentDetail.customer_id != null && salesPaymentDetail.target_time != null)
                    {
                        int targetYear = this.GetTargetYear(salesPaymentDetail.target_time);
                        int targetMonth = this.GetTargetMonth(salesPaymentDetail.target_time);

                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                sales_payment_detail
                                (company_code,
                                project_sys_id,
                                ordering_flg,
                                customer_id,
                                target_year,
                                target_month,
                                amount,
                                ins_date,
                                ins_id)
                            VALUES
                                (@company_code, @project_sys_id, @ordering_flg, @customer_id, @target_year, @target_month, @amount, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                ordering_flg = orderingFlg,
                                customer_id = salesPaymentDetail.customer_id,
                                target_year = targetYear,
                                target_month = targetMonth,
                                amount = salesPaymentDetail.amount ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });

                        this._database.Execute(sqlInsert);
                    }
                }
            }
        }

        /// <summary>
        /// Edit sales payment of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="overheadCostList">Overhead cost list</param>
        /// <param name="overheadCostDetailList">Overhead cost detail list</param>
        private void EditOverheadCost(string companyCode,
            int projectID,
            DateTime? insDate,
            int insUser,
            IList<OverheadCostPlus> overheadCostList,
            IList<OverheadCostDetailPlus> overheadCostDetailList,
            out IList<OverheadCostPlus> newOverheadCostList,
            out IList<OverheadCostDetailPlus> newOverheadCostDetailList)
        {
            newOverheadCostList = null;
            newOverheadCostDetailList = null;

            var sqlDeleteDetail = new Sql(@"
                DELETE FROM
                    overhead_cost_detail
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id;",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID
                 });
            this._database.Execute(sqlDeleteDetail);

            if (overheadCostList != null)
            {
                foreach (OverheadCostPlus overheadCost in overheadCostList)
                {
                    if (overheadCost.detail_no.HasValue)
                    {
                        if (overheadCost.detail_no > 0)
                        {
                            if (overheadCost.is_delete)
                            {
                                var sqlDelete = new Sql(@"
                                    DELETE FROM
                                        overhead_cost
                                    WHERE
                                        company_code = @company_code
                                        AND project_sys_id = @project_sys_id
                                        AND detail_no = @detail_no;",
                                    new
                                    {
                                        company_code = companyCode,
                                        project_sys_id = projectID,
                                        detail_no = overheadCost.detail_no
                                    });
                                this._database.Execute(sqlDelete);
                            }

                            if (overheadCost.is_change)
                            {
                                var sqlUpdate = new Sql(@"
                                    UPDATE
                                        overhead_cost
                                    SET
                                        overhead_cost_id = @overhead_cost_id,
                                        overhead_cost_detail = @overhead_cost_detail,
                                        charge_person_id = @charge_person_id,
                                        total_amount = @total_amount,
                                        upd_date = @upd_date,
                                        upd_id = @upd_id
                                    WHERE
                                        company_code = @company_code
                                        AND project_sys_id = @project_sys_id
                                        AND detail_no = @detail_no;",
                                    new
                                    {
                                        overhead_cost_id = overheadCost.overhead_cost_id,
                                        overhead_cost_detail = overheadCost.overhead_cost_detail,
                                        charge_person_id = overheadCost.charge_person_id,
                                        total_amount = overheadCost.total_amount,
                                        upd_date = insDate,
                                        upd_id = insUser,
                                        company_code = companyCode,
                                        project_sys_id = projectID,
                                        detail_no = overheadCost.detail_no
                                    });
                                this._database.Execute(sqlUpdate);
                            }
                        }
                        else
                        {
                            var sqlInsert = new Sql(@"
                                INSERT INTO
                                    overhead_cost
                                    (company_code,
                                    project_sys_id,
                                    overhead_cost_id,
                                    overhead_cost_detail,
                                    charge_person_id,
                                    total_amount,
                                    ins_date,
                                    ins_id,
                                    upd_date,
                                    upd_id)
                                VALUES
                                    (@company_code, @project_sys_id, @overhead_cost_id, @overhead_cost_detail,
                                    @charge_person_id, @total_amount, @ins_date, @ins_id, @upd_date, @upd_id)
                                SELECT
                                    SCOPE_IDENTITY();",
                                    new
                                    {
                                        company_code = companyCode,
                                        project_sys_id = projectID,
                                        overhead_cost_id = overheadCost.overhead_cost_id,
                                        overhead_cost_detail = overheadCost.overhead_cost_detail,
                                        charge_person_id = overheadCost.charge_person_id,
                                        total_amount = overheadCost.total_amount,
                                        ins_date = insDate,
                                        ins_id = insUser,
                                        upd_date = insDate,
                                        upd_id = insUser
                                    });

                            int newDetailNo = this._database.ExecuteScalar<int>(sqlInsert);

                            foreach (OverheadCostDetailPlus overheadCostDetail in overheadCostDetailList)
                            {
                                if (overheadCostDetail.detail_no == overheadCost.detail_no)
                                    overheadCostDetail.detail_no = newDetailNo;
                            }

                            overheadCost.detail_no = newDetailNo;
                        }
                    }
                }

                newOverheadCostList = overheadCostList;
                newOverheadCostDetailList = overheadCostDetailList;

                if (overheadCostDetailList != null)
                {
                    foreach (OverheadCostDetailPlus overheadCostDetail in overheadCostDetailList)
                    {
                        if (overheadCostDetail.detail_no > 0)
                        {
                            int targetYear = this.GetTargetYear(overheadCostDetail.target_time);
                            int targetMonth = this.GetTargetMonth(overheadCostDetail.target_time);

                            var sqlInsert = new Sql(@"
                            INSERT INTO 
                                overhead_cost_detail
                                (company_code,
                                project_sys_id,
                                detail_no,
                                target_year,
                                target_month,
                                amount,
                                ins_date,
                                ins_id)
                            VALUES
                            (@company_code, @project_sys_id, @detail_no, @target_year,
                            @target_month, @amount, @ins_date, @ins_id);",
                                new
                                {
                                    company_code = companyCode,
                                    project_sys_id = projectID,
                                    detail_no = overheadCostDetail.detail_no,
                                    target_year = targetYear,
                                    target_month = targetMonth,
                                    amount = overheadCostDetail.amount ?? 0,
                                    ins_date = insDate,
                                    ins_id = insUser
                                });
                            this._database.Execute(sqlInsert);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Edit member assignment of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="memberAssignmentList">Member assignment list</param>
        /// <param name="memberAssignmentDetailList">Member assignment detail list</param>
        private void EditMemberAssignment(string companyCode,
            int projectID,
            DateTime? insDate,
            int insUser,
            IList<MemberAssignmentPlus> memberAssignmentList,
            IList<MemberAssignmentDetailPlus> memberAssignmentDetailList)
        {
            var sqlDeleteDetail = new Sql(
                @"
                DELETE FROM
                    member_assignment_detail
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id;",
                new
                {
                    company_code = companyCode,
                    project_sys_id = projectID
                });

            this._database.Execute(sqlDeleteDetail);

            var sqlDelete = new Sql(@"
                DELETE FROM
                    member_assignment
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id;",
                new
                {
                    company_code = companyCode,
                    project_sys_id = projectID
                });

            this._database.Execute(sqlDelete);

            if (memberAssignmentList != null)
            {
                foreach (MemberAssignmentPlus member in memberAssignmentList)
                {
                    if (member.user_sys_id > 0)
                    {
                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                member_assignment
                                (company_code,
                                project_sys_id,
                                user_sys_id,
                                total_plan_man_days,
                                total_plan_cost,
                                ins_date,
                                ins_id)
                            VALUES
                                (@company_code, @project_sys_id, @user_sys_id, @total_plan_man_days, @total_plan_cost, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                user_sys_id = member.user_sys_id,
                                total_plan_man_days = member.total_plan_man_days ?? 0,
                                total_plan_cost = member.total_plan_cost ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });

                        this._database.Execute(sqlInsert);
                    }
                }
            }

            if (memberAssignmentDetailList != null)
            {
                foreach (MemberAssignmentDetailPlus member in memberAssignmentDetailList)
                {
                    if (member.user_sys_id > 0 && !string.IsNullOrEmpty(member.target_time))
                    {
                        int targetYear = this.GetTargetYear(member.target_time);
                        int targetMonth = this.GetTargetMonth(member.target_time);

                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                member_assignment_detail
                                (company_code,
                                project_sys_id,
                                user_sys_id,
                                target_year,
                                target_month,
                                plan_man_days,
                                individual_sales,
                                ins_date,
                                ins_id)
                            VALUES
                                (@company_code, @project_sys_id, @user_sys_id, @target_year, @target_month, @plan_man_days, @individual_sales, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                user_sys_id = member.user_sys_id,
                                target_year = targetYear,
                                target_month = targetMonth,
                                plan_man_days = member.plan_man_days ?? 0,
                                individual_sales = member.individual_sales ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });

                        this._database.Execute(sqlInsert);
                    }
                }
            }
        }

        /// <summary>
        /// Edit progress history of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="progressHistoryList">Progress history list</param>
        private void EditProgressHistory(string companyCode,
            int projectID,
            DateTime? insDate,
            int insUser,
            IList<ProgressHistoryPlus> progressHistoryList)
        {
            foreach (ProgressHistoryPlus progressHistory in progressHistoryList)
            {
                if (progressHistory.isDelete.HasValue
                    && progressHistory.isDelete.Value
                    && progressHistory.regist_date != null
                    && progressHistory.progress != null)
                {
                    var sqlDelete = new Sql(
                    @"
                    DELETE FROM
                        progress_history
                    WHERE
                        company_code = @company_code
                        AND project_sys_id = @project_sys_id
                        AND regist_date = @regist_date;",
                    new
                    {
                        company_code = companyCode,
                        project_sys_id = projectID,
                        regist_date = progressHistory.regist_date
                    });

                    this._database.Execute(sqlDelete);
                }

                if (progressHistory.isNew.HasValue
                    && progressHistory.isNew.Value
                    && progressHistory.regist_date != null
                    && progressHistory.progress != null)
                {
                    var sqlInsert = new Sql(@"
                        INSERT INTO
                            progress_history
                            (company_code,
                            project_sys_id,
                            regist_date,
                            progress,
                            remarks,
                            ins_date,
                            ins_id)
                        VALUES
                            (@company_code, @project_sys_id, @regist_date, @progress, @remarks, @ins_date, @ins_id);",
                        new
                        {
                            company_code = companyCode,
                            project_sys_id = projectID,
                            regist_date = progressHistory.regist_date,
                            progress = Convert.ToDecimal(progressHistory.progress) / 100,
                            remarks = progressHistory.remarks.Trim(),
                            ins_date = insDate,
                            ins_id = insUser
                        });
                    this._database.Execute(sqlInsert);
                }
            }
        }

        /// <summary>
        /// Edit attach file of project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="fileList">Attach file list</param>
        private void EditAttachFile(string companyCode,
            int projectID,
            DateTime? insDate,
            int insUser,
            IList<ProjectAttachFilePlus> fileList)
        {
            Sql sqlInsert = new Sql();
            Sql sqlDelete = new Sql();
            Sql sqlUpdate = new Sql();
            string srcPath = ConfigurationManager.AppSettings[ConfigurationKeys.ATTACH_FILE_FOLDER]
                + companyCode + "/"
                + projectID + "/";

            foreach (ProjectAttachFilePlus attachFile in fileList)
            {
                if (!string.IsNullOrEmpty(attachFile.display_title))
                {
                    if (attachFile.delete_file)
                    {
                        sqlDelete.Append(@"
                            DELETE FROM
                                project_attached_file
                            WHERE
                                company_code = @company_code
                                AND project_sys_id = @project_sys_id
                                AND file_no = @file_no;",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                file_no = attachFile.file_no
                            });

                        continue;
                    }

                    if (attachFile.file_no != null && (attachFile.change_info || attachFile.change_file))
                    {
                        var sb = new StringBuilder();

                        sb.AppendFormat("UPDATE project_attached_file SET display_title = '{0}', ", attachFile.display_title.Trim());

                        if (attachFile.change_file)
                        {
                            sb.AppendFormat("file_path = '{0}', ", srcPath + attachFile.file_path);
                            sb.AppendFormat("file_name = '{0}', ", attachFile.file_name);
                        }

                        sb.AppendFormat("public_flg = '{0}', upd_date = '{1}', upd_id = {2} ", attachFile.public_flg, insDate, insUser);
                        sb.AppendFormat("WHERE company_code = '{0}' AND project_sys_id = {1} AND file_no = {2};", companyCode, projectID, attachFile.file_no);

                        var query = new Sql(sb.ToString());

                        sqlUpdate.Append(query);

                        continue;
                    }

                    if (attachFile.file_no == null)
                    {
                        sqlInsert.Append(@"
                            INSERT INTO
                                project_attached_file
                                (company_code,
                                project_sys_id,
                                display_title,
                                file_path,
                                file_name,
                                public_flg,
                                ins_date,
                                ins_id,
                                upd_date,
                                upd_id)
                            VALUES
                                (@company_code, @project_sys_id, @display_title, @file_path, @file_name,
                                @public_flg, @ins_date, @ins_id, @upd_date, @upd_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                display_title = attachFile.display_title.Trim(),
                                file_path = srcPath + attachFile.file_path,
                                file_name = attachFile.file_name,
                                public_flg = attachFile.public_flg,
                                ins_date = insDate,
                                ins_id = insUser,
                                upd_date = insDate,
                                upd_id = insUser
                            });
                    }
                }
            }

            if (!sqlDelete.SQL.Equals(string.Empty))
                this._database.Execute(sqlDelete);

            if (!sqlUpdate.SQL.Equals(string.Empty))
                this._database.Execute(sqlUpdate);

            if (!sqlInsert.SQL.Equals(string.Empty))
                this._database.Execute(sqlInsert);
        }

        /// <summary>
        /// Insert history of project info
        /// </summary>
        /// <param name="data">Project info</param>
        /// <returns>New ID of history</returns>
        private int InsertProjectInfoHistory(ProjectInfoPlus data)
        {
            var sqlInsertHistory = new Sql(@"
                    INSERT INTO
                        project_info_history
                        (company_code,
                        project_sys_id,
                        estimate_man_days,
                        total_sales,
                        total_payment,
                        tax_rate,
                        gross_profit,
                        delete_status,
                        ins_date,
                        ins_id)
                    VALUES
                        (@company_code, @project_sys_id, @estimate_man_days, @total_sales, @total_payment,
                        @tax_rate, @gross_profit, @delete_status, @ins_date, @ins_id);
                    SELECT
                        SCOPE_IDENTITY();",
                    new
                    {
                        company_code = data.company_code,
                        project_sys_id = data.project_sys_id,
                        estimate_man_days = data.estimate_man_days,
                        total_sales = data.total_sales,
                        total_payment = data.total_payment ?? 0,
                        tax_rate = data.tax_rate,
                        gross_profit = data.gross_profit ?? 0,
                        delete_status = data.del_flg,
                        ins_date = data.upd_date,
                        ins_id = data.upd_id
                    });

            int newHistoryID = this._database.ExecuteScalar<int>(sqlInsertHistory);

            return newHistoryID;
        }

        /// <summary>
        /// Insert history of member assignment in project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="memberAssignmentList">Member assignment list</param>
        /// <param name="memberAssignmentDetailList">Member assignment detail list</param>
        private void InsertMemberAssignmentHistory(string companyCode,
            int projectID,
            int historyID,
            DateTime? insDate,
            int insUser,
            IList<MemberAssignmentPlus> memberAssignmentList,
            IList<MemberAssignmentDetailPlus> memberAssignmentDetailList)
        {
            if (memberAssignmentList != null)
            {
                foreach (MemberAssignmentPlus member in memberAssignmentList)
                {
                    if (member.user_sys_id > 0)
                    {
                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                member_assignment_history
                                (company_code,
                                project_sys_id,
                                user_sys_id,
                                history_no,
                                total_plan_man_days,
                                total_plan_cost,
                                ins_date,
                                ins_id)
                            VALUES
                                (@company_code, @project_sys_id, @user_sys_id, @history_no,
                                @total_plan_man_days, @total_plan_cost, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                user_sys_id = member.user_sys_id,
                                history_no = historyID,
                                total_plan_man_days = member.total_plan_man_days ?? 0,
                                total_plan_cost = member.total_plan_cost ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });

                        this._database.Execute(sqlInsert);
                    }
                }
            }

            if (memberAssignmentDetailList != null)
            {
                foreach (MemberAssignmentDetailPlus member in memberAssignmentDetailList)
                {
                    if (member.user_sys_id > 0 && !string.IsNullOrEmpty(member.target_time))
                    {
                        int targetYear = this.GetTargetYear(member.target_time);
                        int targetMonth = this.GetTargetMonth(member.target_time);

                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                member_assignment_detail_history
                                (company_code,
                                project_sys_id,
                                user_sys_id,
                                history_no,
                                target_year,
                                target_month,
                                plan_man_days,
                                individual_sales,
                                plan_cost,
                                ins_date,
                                ins_id)
                            VALUES
                                (@company_code, @project_sys_id, @user_sys_id, @history_no, @target_year, 
                                @target_month, @plan_man_days, @individual_sales, @plan_cost, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                user_sys_id = member.user_sys_id,
                                history_no = historyID,
                                target_year = targetYear,
                                target_month = targetMonth,
                                plan_man_days = member.plan_man_days ?? 0,
                                individual_sales = member.individual_sales ?? 0,
                                plan_cost = member.plan_cost ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });

                        this._database.Execute(sqlInsert);
                    }
                }
            }
        }

        /// <summary>
        /// Insert history of sales payment in project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <param name="orderingFlg">Ordering flag</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="salesPaymentList">Sales payment list</param>
        /// <param name="salesPaymentListDetail">Sales payment detail list</param>
        private void InsertSalesPaymentHistory(string companyCode,
            int projectID,
            int historyID,
            string orderingFlg,
            DateTime? insDate,
            int insUser,
            IList<SalesPaymentPlus> salesPaymentList,
            IList<SalesPaymentDetailPlus> salesPaymentListDetail)
        {
            if (salesPaymentList != null)
            {
                foreach (SalesPaymentPlus salesPayment in salesPaymentList)
                {
                    if (salesPayment.customer_id != null)
                    {
                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                sales_payment_history
                                (company_code,
                                project_sys_id,
                                ordering_flg,
                                customer_id,
                                end_user_id,
                                history_no,
                                charge_person_id,
                                total_amount,
                                ins_date,
                                ins_id)
                            VALUES
                            (@company_code, @project_sys_id, @ordering_flg, @customer_id, @end_user_id, @history_no,
                            @charge_person_id, @total_amount, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                ordering_flg = orderingFlg,
                                customer_id = salesPayment.customer_id,
                                end_user_id = salesPayment.end_user_id,
                                history_no = historyID,
                                charge_person_id = salesPayment.charge_person_id ?? null,
                                total_amount = salesPayment.total_amount ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });
                        this._database.Execute(sqlInsert);

                    }
                }
            }

            if (salesPaymentListDetail != null)
            {
                foreach (SalesPaymentDetailPlus salesPaymentDetail in salesPaymentListDetail)
                {
                    if (salesPaymentDetail.customer_id != null && salesPaymentDetail.target_time != null)
                    {
                        int targetYear = this.GetTargetYear(salesPaymentDetail.target_time);
                        int targetMonth = this.GetTargetMonth(salesPaymentDetail.target_time);

                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                payment_detail_history
                                (company_code,
                                project_sys_id,
                                customer_id,
                                history_no,
                                target_year,
                                target_month,
                                amount,
                                ins_date,
                                ins_id)
                            VALUES
                                (@company_code, @project_sys_id, @customer_id, @history_no,
                                @target_year, @target_month, @amount, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                customer_id = salesPaymentDetail.customer_id,
                                history_no = historyID,
                                target_year = targetYear,
                                target_month = targetMonth,
                                amount = salesPaymentDetail.amount ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });

                        this._database.Execute(sqlInsert);
                    }
                }
            }
        }

        /// <summary>
        /// Insert history of sales payment in project
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="projectID">Project ID</param>
        /// <param name="historyID">History ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="overheadCostList">Overhead cost list</param>
        /// <param name="overheadCostListDetail">Overhead cost detail list</param>
        private void InsertOverheadCostHistory(string companyCode,
            int projectID,
            int historyID,
            DateTime? insDate,
            int insUser,
            IList<OverheadCostPlus> overheadCostList,
            IList<OverheadCostDetailPlus> overheadCostListDetail)
        {
            if (overheadCostList != null)
            {
                foreach (OverheadCostPlus overheadCost in overheadCostList)
                {
                    if (overheadCost.detail_no > 0 && !overheadCost.is_delete)
                    {
                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                overhead_cost_history
                                (company_code,
                                project_sys_id,
                                detail_no,
                                history_no,
                                overhead_cost_id,
                                overhead_cost_detail,
                                charge_person_id,
                                total_amount,
                                ins_date,
                                ins_id)
                            VALUES
                            (@company_code, @project_sys_id, @detail_no, @history_no, @overhead_cost_id, 
                            @overhead_cost_detail, @charge_person_id, @total_amount, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                detail_no = overheadCost.detail_no,
                                history_no = historyID,
                                overhead_cost_id = overheadCost.overhead_cost_id,
                                overhead_cost_detail = overheadCost.overhead_cost_detail,
                                charge_person_id = overheadCost.charge_person_id,
                                total_amount = overheadCost.total_amount ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });
                        this._database.Execute(sqlInsert);
                    }
                }
            }

            if (overheadCostListDetail != null)
            {
                foreach (OverheadCostDetailPlus overheadCostDetail in overheadCostListDetail)
                {
                    if (overheadCostDetail.detail_no > 0)
                    {
                        int targetYear = this.GetTargetYear(overheadCostDetail.target_time);
                        int targetMonth = this.GetTargetMonth(overheadCostDetail.target_time);

                        var sqlInsert = new Sql(@"
                            INSERT INTO 
                                overhead_cost_detail_history
                                (company_code,
                                project_sys_id,
                                detail_no,
                                history_no,
                                target_year,
                                target_month,
                                amount,
                                ins_date,
                                ins_id)
                            VALUES
                                (@company_code, @project_sys_id, @detail_no, @history_no,
                                @target_year, @target_month, @amount, @ins_date, @ins_id);",
                            new
                            {
                                company_code = companyCode,
                                project_sys_id = projectID,
                                detail_no = overheadCostDetail.detail_no,
                                history_no = historyID,
                                target_year = targetYear,
                                target_month = targetMonth,
                                amount = overheadCostDetail.amount ?? 0,
                                ins_date = insDate,
                                ins_id = insUser
                            });

                        this._database.Execute(sqlInsert);
                    }
                }
            }
        }

        /// <summary>
        /// Get month from time
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>Month</returns>
        private int GetTargetMonth(string time)
        {
            return Convert.ToInt32(time.Substring(5, 2));
        }

        /// <summary>
        /// Get year from time
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>Year</returns>
        private int GetTargetYear(string time)
        {
            return Convert.ToInt32(time.Substring(0, 4));
        }

        /// <summary>
        /// Check Project Plan Information by Project ID and Company Code is esixt
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <param name="companyCode">Company code</param>
        /// <returns>Project Plan Information</returns>
        public int GetProjectPlan(int projectID, string companyCode)
        {
            var sql = new Sql(@"
               SELECT
                    COUNT(*)
                FROM
                    project_plan_info
                WHERE
                    company_code = @company_code
                    and project_sys_id = @projectID"
            , new { company_code = companyCode, projectID = projectID });

            return _database.ExecuteScalar<int>(sql);
        }

        #endregion

        /// <summary>
        /// Delete project by ID
        /// </summary>
        /// <param name="dataListProjectId"></param>
        /// <returns></returns>
        public bool DeleteProject(IList<string> dataListProjectId)
        {
            var result = false;

            if (dataListProjectId != null && dataListProjectId.Count > 0)
            {
                foreach (var id in dataListProjectId)
                {
                    result = DeleteProjectID(Convert.ToInt32(id));

                    if (!result) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update project status by ID
        /// </summary>
        /// <param name="dataListProjectId"></param>
        /// <param name="statusID"></param>
        /// <param name="updateUserID"></param>
        /// <returns></returns>
        public bool UpdateStatusProject(IList<string> dataListProjectId, int statusID, int updateUserID)
        {
            var result = false;

            if (dataListProjectId != null && dataListProjectId.Count > 0 && statusID > 0)
            {
                foreach (var id in dataListProjectId)
                {
                    result = UpdateStatusProjectByID(Convert.ToInt32(id), statusID, updateUserID);

                    if (!result) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Delete project by ID
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private bool DeleteProjectID(int projectId)
        {

            var sql = new Sql(@"
                UPDATE project_info 
                SET del_flg = @del_flg
                WHERE project_sys_id = @project_sys_id",
                new
                {
                    del_flg = Constant.DeleteFlag.DELETE,
                    project_sys_id = projectId
                });

            var result = _database.Execute(sql);

            return result == 1;
        }

        /// <summary>
        /// Update project status by ID
        /// </summary>
        /// <param name="dataListProjectId"></param>
        /// <param name="statusID"></param>
        /// <param name="updateUserID"></param>
        /// <returns></returns>
        private bool UpdateStatusProjectByID(int projectId, int statusID, int updateUserID)
        {
            var sql = new Sql(@"
                UPDATE project_info 
                SET status_id = @status_id
                    , upd_date = @upd_date
                    , upd_id = @upd_id
                WHERE project_sys_id = @project_sys_id",
                new
                {
                    del_flg = Constant.DeleteFlag.DELETE,
                    project_sys_id = projectId,
                    status_id = statusID,
                    upd_date = Utility.GetCurrentDateTime(),
                    upd_id = updateUserID,
                });

            var result = _database.Execute(sql);

            return result == 1;
        }

        /// <summary>
        /// Get data_editable_time from company setting to delete progress
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetDataEditTableTime(string companyCode)
        {
            var sql = new Sql(@"
                SELECT data_editable_time 
                FROM m_company_setting
                WHERE company_code = @company_code",
                new
                {
                    company_code = companyCode
                });

            return this._database.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// Get actual work time by project ID
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public decimal GetActualWorkTimeByProjectID(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                SELECT
                    ISNULL(SUM(actual_work_time),0) actual_work_time
                FROM
                    member_actual_work_detail
                WHERE
                    company_code = @company_code
                    AND project_sys_id = @project_sys_id
                ",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID
                 });

            return this._database.FirstOrDefault<decimal>(sql);
        }

        /// <summary>
        /// Get operation target flag
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="statusID"></param>
        /// <returns></returns>
        public string GetOperationTargetFlag(string companyCode, int statusID)
        {
            var sql = new Sql(@"
                SELECT 
                    s.operation_target_flg FROM m_status s
                WHERE
                    s.company_code = @company_code
                    AND s.status_id = @status_id
                ",
                 new
                 {
                     company_code = companyCode,
                     status_id = statusID
                 });

            return this._database.FirstOrDefault<string>(sql);
        }

        /// <summary>
        /// Get plan cost from history
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public IList<MemberAssignmentDetailHistoryPlus> GetPlanCostHistory(string companyCode, int projectID)
        {
            var sql = new Sql(@"
                    SELECT 
                        user_sys_id, 
                        history_no,
                        target_year, 
                        target_month, 
                        plan_cost
                    FROM member_assignment_detail_history
                    WHERE company_code = @company_code AND project_sys_id = @project_sys_id
                            AND history_no = (SELECT MAX(history_no) 
                                              FROM member_assignment_detail_history 
                                              WHERE company_code = @company_code AND project_sys_id = @project_sys_id)",
                 new
                 {
                     company_code = companyCode,
                     project_sys_id = projectID
                 });

            return this._database.Fetch<MemberAssignmentDetailHistoryPlus>(sql);
        }
    }
}
