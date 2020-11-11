using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS09003;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS09003Repository : Repository, IPMS09003Repository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09003Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS09003Repository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// Get list group
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <returns></returns>
        public IEnumerable<Group> GetGroupList(string cCode)
        {
            var condition = new Dictionary<string, object>
                            {
                                {
                                    "company_code", cCode
                                },
                                {
                                    "del_flg", Constant.DeleteFlag.NON_DELETE
                                }
                            };

            return this.Select<Group>(condition, "display_order");
        }

        /// <summary>
        /// Build query sql
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>Query SQL</returns>
        private Sql buildQuerySalesCustomer(string companyCode, Condition condition)
        {
            int startYear = Convert.ToDateTime(condition.START_DATE).Year;
            int startMonth = Convert.ToDateTime(condition.START_DATE).Month;
            int endYear = Convert.ToDateTime(condition.END_DATE).Year;
            int endMonth = Convert.ToDateTime(condition.END_DATE).Month;

            Sql sql = Sql.Builder.Append(@"
            SELECT
                *
            FROM
            (
                SELECT
                    customer_id,
                    display_name,
                    records,
                    ISNULL(sales_amount, 0) AS total_sales,
                    ISNULL(profit, 0) AS gross_profit,
                    ISNULL(CASE WHEN sales_amount = 0 THEN 0
                         ELSE ISNULL(profit, 0) / sales_amount
                    END, 0) AS gross_profit_rate,
                    del_flg
                FROM
                (
                    SELECT
                        cus.customer_id,
                        ISNULL(cus.display_name, '未設定') AS display_name,
                        COUNT(tb_summary.project_sys_id) AS records,
                        SUM(sales_amount) AS sales_amount,
                        SUM(individual_sales) AS individual_sales,
                        SUM(profit) AS profit,
                        cus.del_flg
                    FROM
                    (
                        SELECT 
                            tb_project_sales.company_code,
                            tb_project_sales.project_sys_id,
                            SUM(sales_amount) AS sales_amount,
                            SUM(individual_sales) AS individual_sales,
                            SUM(individual_sales) - SUM(cost) AS profit
                        FROM
                        (
                            SELECT
                                tb_sales.company_code,
                                tb_sales.project_sys_id,
                                tb_sales.user_sys_id,
                                dbo.RoundNumber(@company_code, SUM(tb_sales.individual_sales)) AS individual_sales,
                                dbo.RoundNumber(@company_code, SUM(tb_sales.sales_amount)) AS sales_amount,
                                dbo.RoundNumber(@company_code, SUM(ISNULL(tb_actual_cost.actual_cost, 0))) AS cost
                            FROM
                            (
                                SELECT
                                    ISNULL(tb_individual_sales.company_code, ISNULL(tb_payment.company_code, tb_overhead_cost.company_code)) AS company_code,
                                    ISNULL(tb_individual_sales.project_sys_id, ISNULL(tb_payment.project_sys_id, tb_overhead_cost.project_sys_id)) AS project_sys_id,
                                    ISNULL(tb_individual_sales.user_sys_id, ISNULL(tb_payment.charge_person_id, tb_overhead_cost.charge_person_id)) AS user_sys_id,
                                    ISNULL(individual_sales, 0) + ISNULL(payment_amount, 0) + ISNULL(overhead_cost_amount, 0) AS sales_amount,
                                    ISNULL(individual_sales, 0) AS individual_sales
                                FROM
                                (
                                        SELECT 
                                            company_code,
                                            project_sys_id,
                                            user_sys_id,
                                            SUM(individual_sales) AS individual_sales
                                        FROM
                                            member_assignment_detail AS mad
                                        WHERE
                                            mad.company_code = '" + companyCode + @"'
                                            AND (mad.target_year > @sY OR (mad.target_year = @sY AND mad.target_month >= @sM))
                                            AND (mad.target_year < @eY OR (mad.target_year = @eY AND mad.target_month <= @eM))
                                            AND mad.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'",
                new
                {
                    company_code = companyCode,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sql.Append(" AND pi.contract_type_id IN (" + condition.CONTRACT_TYPE_ID + ")");

            sql.Append(@" )");

            if (condition.GROUP_ID != null)
            {
                sql.Append(@"AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id",
                new
                {
                    company_code = companyCode,
                    group_id = condition.GROUP_ID.Value,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });
            }

            // DTQ update search by branch
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sql.Append(@"AND (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IN (" + condition.LOCATION_ID + ")",
                new
                {
                    company_code = companyCode,
                    location_id = condition.LOCATION_ID,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });
            }
            // end DQT update search by branch


            if (condition.TAG_ID != null)
                sql.Append(@"AND mad.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id = @tag_id)", new { company_code = companyCode, tag_id = condition.TAG_ID.Value });

            sql.Append(@"
                                        GROUP BY company_code, project_sys_id, user_sys_id
                                ) AS tb_individual_sales
                                FULL JOIN
                                (
                                    SELECT
                                        sp.company_code,
                                        sp.project_sys_id,
                                        sp.charge_person_id,
                                        SUM(spd.amount) AS payment_amount
                                    FROM
                                        sales_payment_detail spd
                                        INNER JOIN 
                                        (
                                            SELECT
                                                company_code,
                                                project_sys_id,
                                                ordering_flg,
                                                customer_id,
                                                ISNULL(charge_person_id, (SELECT charge_person_id FROM project_info pi WHERE pi.project_sys_id = sales_payment.project_sys_id)) AS charge_person_id,
                                                total_amount,
                                                tag_id,
                                                ins_date,
                                                ins_id
                                            FROM
                                                sales_payment
                                            WHERE 
                                                ordering_flg = '2'
                                        ) sp
                                            ON spd.company_code = sp.company_code
                                            AND spd.project_sys_id = sp.project_sys_id
                                            AND spd.ordering_flg = sp.ordering_flg
                                            AND spd.customer_id = sp.customer_id
                                    WHERE
                                        spd.company_code = '" + companyCode + @"'
                                        AND spd.ordering_flg = '2'
                                        AND (spd.target_year > @sY OR (spd.target_year = @sY AND spd.target_month >= @sM))
                                        AND (spd.target_year < @eY OR (spd.target_year = @eY AND spd.target_month <= @eM))
                                        AND sp.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'",
                new
                {
                    company_code = companyCode,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sql.Append(" AND pi.contract_type_id IN (" + condition.CONTRACT_TYPE_ID + ")");

            sql.Append(@")");

            if (condition.GROUP_ID != null)
            {
                sql.Append(@"AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = sp.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id",
                new
                {
                    company_code = companyCode,
                    group_id = condition.GROUP_ID.Value,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });
            }


            // DTQ update search by branch
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sql.Append(@"AND (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = sp.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IN (" + condition.LOCATION_ID + ")",
                new
                {
                    company_code = companyCode,
                    location_id = condition.LOCATION_ID,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });
            }
            // end DQT update search by branch


            if (condition.TAG_ID != null)
                sql.Append(@"AND sp.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id = @tag_id)", new { company_code = companyCode, tag_id = condition.TAG_ID.Value });

            sql.Append(@"
                                    GROUP BY sp.company_code, sp.project_sys_id, sp.charge_person_id
                                ) AS tb_payment
                                    ON tb_individual_sales.company_code = tb_payment.company_code
                                    AND tb_individual_sales.project_sys_id = tb_payment.project_sys_id
                                    AND tb_individual_sales.user_sys_id = tb_payment.charge_person_id
                                FULL JOIN
                                (
                                    SELECT
                                        ocd.company_code,
                                        ocd.project_sys_id,
                                        oc.charge_person_id,
                                        SUM(ocd.amount) AS overhead_cost_amount
                                    FROM 
                                        overhead_cost_detail AS ocd
                                        INNER JOIN overhead_cost AS oc
                                            ON ocd.company_code = oc.company_code
                                            AND ocd.project_sys_id = oc.project_sys_id
                                            AND ocd.detail_no = oc.detail_no
                                    WHERE
                                        ocd.company_code = '" + companyCode + @"'
                                        AND (ocd.target_year > @sY OR (ocd.target_year = @sY AND ocd.target_month >= @sM))
                                        AND (ocd.target_year < @eY OR (ocd.target_year = @eY AND ocd.target_month <= @eM))
                                        AND oc.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'",
                new
                {
                    company_code = companyCode,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sql.Append(" AND pi.contract_type_id IN (" + condition.CONTRACT_TYPE_ID + ")");

            sql.Append(@")");

            if (condition.GROUP_ID != null)
            {
                sql.Append(@"AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = oc.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id",
                new
                {
                    company_code = companyCode,
                    group_id = condition.GROUP_ID.Value,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });
            }

            // DTQ update search by branch
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sql.Append(@"AND (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = oc.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IN (" + condition.LOCATION_ID + ")",
                new
                {
                    company_code = companyCode,
                    location_id = condition.LOCATION_ID,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });
            }
            // end DQT update search by branch


            if (condition.TAG_ID != null)
                sql.Append(@"AND oc.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id = @tag_id)", new { company_code = companyCode, tag_id = condition.TAG_ID.Value });

            sql.Append(@"
                                    GROUP BY ocd.company_code, ocd.project_sys_id, oc.charge_person_id
                                ) AS tb_overhead_cost
                                    ON 
                                    (tb_individual_sales.company_code = tb_overhead_cost.company_code
                                    OR tb_payment.company_code = tb_overhead_cost.company_code)
                                    AND (tb_individual_sales.project_sys_id = tb_overhead_cost.project_sys_id
                                    OR tb_payment.project_sys_id = tb_overhead_cost.project_sys_id)
                                    AND (tb_individual_sales.user_sys_id = tb_overhead_cost.charge_person_id
                                    OR tb_payment.charge_person_id = tb_overhead_cost.charge_person_id)
                            ) AS tb_sales
                            LEFT JOIN
                            (
                                SELECT
                                    company_code,
                                    project_sys_id,
                                    user_sys_id,
                                    SUM(actual_cost) AS actual_cost
                                FROM
                                (
                                    SELECT
                                        mawd.company_code,
                                        mawd.project_sys_id,
                                        mawd.user_sys_id,
                                        dbo.RoundNumber(@company_code, ISNULL((SELECT TOP 1 base_unit_cost FROM unit_price_history WHERE company_code = @company_code AND user_sys_id = mawd.user_sys_id AND apply_start_date <= CONVERT(date, CAST(mawd.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mawd.actual_work_month AS VARCHAR(2)), 2) + '/01') AND del_flg = '0' ORDER BY apply_start_date DESC)
                                        * actual_work_time
                                        / operating_days, 0)) AS actual_cost
                                    FROM
                                        dbo.GetTableDaysInMonths(@company_code, @sY, @sM, @eY, @eM) nowdim
                                        INNER HASH JOIN 
                                        (
                                            SELECT
                                                company_code,
                                                project_sys_id,
                                                user_sys_id,
                                                actual_work_year,
                                                actual_work_month,
                                                dbo.Hour2Day(@company_code, SUM(actual_work_time)) actual_work_time
                                            FROM
                                            member_actual_work_detail AS mawd
                                            WHERE
                                            mawd.company_code = '" + companyCode + @"'
                                            AND (mawd.actual_work_year > @sY OR (mawd.actual_work_year = @sY AND mawd.actual_work_month >= @sM))
                                            AND (mawd.actual_work_year < @eY OR (mawd.actual_work_year = @eY AND mawd.actual_work_month <= @eM))
                                            AND mawd.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'",
                new
                {
                    company_code = companyCode,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sql.Append(" AND pi.contract_type_id IN (" + condition.CONTRACT_TYPE_ID + ")");

            sql.Append(@")");

            if (condition.GROUP_ID != null)
            {
                sql.Append(@"AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id",
                new
                {
                    company_code = companyCode,
                    group_id = condition.GROUP_ID.Value,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });
            }

            // DTQ update search by branch
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sql.Append(@"AND (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IN (" + condition.LOCATION_ID + ")",
                new
                {
                    company_code = companyCode,
                    location_id = condition.LOCATION_ID,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth
                });
            }
            // end DQT update search by branch

            sql.Append(@" GROUP BY mawd.company_code, mawd.user_sys_id, mawd.actual_work_year, mawd.actual_work_month, mawd.project_sys_id
                                        ) AS mawd
                                            ON nowdim.company_code = mawd.company_code
                                            AND nowdim.target_year = mawd.actual_work_year
                                            AND nowdim.target_month = mawd.actual_work_month
                                ) AS actual_cost_list
                                GROUP BY actual_cost_list.company_code, actual_cost_list.project_sys_id, actual_cost_list.user_sys_id
                            ) AS tb_actual_cost
                                ON tb_sales.company_code = tb_actual_cost.company_code
                                AND tb_sales.project_sys_id = tb_actual_cost.project_sys_id
                                AND tb_sales.user_sys_id = tb_actual_cost.user_sys_id
                            GROUP BY tb_sales.company_code, tb_sales.project_sys_id, tb_sales.user_sys_id
                        )AS tb_project_sales
                        GROUP BY tb_project_sales.company_code, tb_project_sales.project_sys_id
                    ) AS tb_summary
                    LEFT JOIN sales_payment AS sp
                        ON tb_summary.company_code = sp.company_code
                        AND tb_summary.project_sys_id = sp.project_sys_id
                        AND sp.ordering_flg = '1'
                    FULL JOIN m_customer AS cus
                        ON sp.company_code = cus.company_code
                        AND sp.customer_id = cus.customer_id
                    WHERE cus.company_code IS NULL OR cus.company_code = '" + companyCode + @"'
                    GROUP BY cus.customer_id, cus.display_name, cus.del_flg
                ) AS tb_final WHERE tb_final.records >= 1 ");

            if (condition.CUSTOMER_ID != null)
            {
                sql.Append("AND tb_final.customer_id = @customer_id ", new { customer_id = condition.CUSTOMER_ID.Value });
            }

            if (!condition.DELETED_INCLUDE)
            {
                sql.Append("AND (tb_final.del_flg = '0' OR tb_final.del_flg IS NULL)");
            }
            sql.Append(") tb");
            return sql;
        }
        /// <summary>
        /// Get list sale customer
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List Sale Customer</returns>
        public PageInfo<SalesCustomerPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, string companyCode)
        {
            Sql sql = this.buildQuerySalesCustomer(companyCode, condition);
            var pageInfo = Page<SalesCustomerPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get list Sale Customer
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List Sales Customer</returns>
        public IList<SalesCustomerPlus> SearchAll(string companyCode, Condition condition)
        {
            Sql sql = this.buildQuerySalesCustomer(companyCode, condition);

            return this._database.Fetch<SalesCustomerPlus>(sql);
        }

        /// <summary>
        /// Get list Sales Customer
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List sale customer</returns>
        public List<SalesCustomerPlus> GetListSalesCustomer(Condition condition, string companyCode, string orderBy, string orderType)
        {
            Sql sql = this.buildQuerySalesCustomer(companyCode, condition);

            sql.Append(" ORDER BY tb." + orderBy + " " + orderType);

            return this._database.Fetch<SalesCustomerPlus>(sql);
        }

        /// <summary>
        /// Build Query SQL
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>Query SQL</returns>
        private Sql buildQuerySalesTagByCustomer(string companyCode, ConditionSaleTag condition)
        {
            int startYear = Convert.ToDateTime(condition.START_DATE).Year;
            int startMonth = Convert.ToDateTime(condition.START_DATE).Month;
            int endYear = Convert.ToDateTime(condition.END_DATE).Year;
            int endMonth = Convert.ToDateTime(condition.END_DATE).Month;

            string locationOfMAD = null;
            string locationOfSPD = null;
            string locationOfOCD = null;
            string locatioOfMAWD = null;

            string groupOfMAD = null;
            string groupOfSPD = null;
            string groupOfOCD = null;
            string groupOfMAWD = null;

            string contractType = null;

            string tagIdOfMAD = null;
            string tagIdOfSP = null;
            string tagIdOfOC = null;

            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                locationOfMAD = @" AND
                                (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND(actual_work_year < mad.target_year OR(actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.LOCATION_ID + ")";

                locationOfSPD = @" AND
                                (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND(actual_work_year < spd.target_year OR(actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.LOCATION_ID + ")";

                locationOfOCD = @" AND 
								(SELECT TOP(1) location_id
									FROM enrollment_history
									WHERE company_code = ocd.company_code
									AND user_sys_id = oc.charge_person_id
									AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
									ORDER BY actual_work_year DESC, actual_work_month DESC
								) in (" + condition.LOCATION_ID + ")";
                locatioOfMAWD = @" AND
								(SELECT TOP(1) location_id
								FROM enrollment_history
								WHERE company_code = mawd.company_code
								AND user_sys_id = mawd.user_sys_id
								AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
								ORDER BY actual_work_year DESC, actual_work_month DESC
							) in (" + condition.LOCATION_ID + ")";
            }

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                groupOfMAD = @" AND
                                (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND(actual_work_year < mad.target_year OR(actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.GROUP_ID + ")";

                groupOfSPD = @" AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.GROUP_ID + ")";

                groupOfOCD = @" AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = ocd.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.GROUP_ID + ")";

                groupOfMAWD = @" AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.GROUP_ID + ")";
            }

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
            {
                contractType = @" AND pi.contract_type_id IN (" + condition.CONTRACT_TYPE_ID + ")";
            }

            if (condition.TAG_ID != null)
            {
                tagIdOfMAD = @"AND mad.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = " + companyCode + " AND sp.ordering_flg = '1' AND sp.tag_id = " + condition.TAG_ID.Value + ")";
                tagIdOfSP = @"AND sp.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = " + companyCode + " AND sp.ordering_flg = '1' AND sp.tag_id = " + condition.TAG_ID.Value + ")";
                tagIdOfOC = @"AND oc.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = " + companyCode + " AND sp.ordering_flg = '1' AND sp.tag_id = " + condition.TAG_ID.Value + ")";
            }


            Sql sql = Sql.Builder.Append(@"
                SELECT * FROM (
                    SELECT
                        tag_id,
                        tag_name as display_name ,
                        ISNULL(sales_amount, 0) AS total_sales,
                        ISNULL(profit, 0) AS gross_profit,
                        ISNULL(CASE WHEN sales_amount = 0 THEN 0
                             ELSE ISNULL(profit, 0) / sales_amount
                        END, 0) AS gross_profit_rate
                    FROM
                    (
                        SELECT
                            tag.tag_id,
                            tag.tag_name,
                            SUM(sales_amount) AS sales_amount,
                            SUM(individual_sales) AS individual_sales,
                            SUM(profit) AS profit
                        FROM
                        (
                            SELECT 
                                tb_project_sales.company_code,
                                tb_project_sales.project_sys_id,
                                SUM(sales_amount) AS sales_amount,
                                SUM(individual_sales) AS individual_sales,
                                SUM(individual_sales) - SUM(cost) AS profit
                            FROM
                            (
                                SELECT
                                    tb_sales.company_code,
                                    tb_sales.project_sys_id,
                                    tb_sales.user_sys_id,
                                    dbo.RoundNumber(@company_code, SUM(tb_sales.individual_sales)) AS individual_sales,
                                    dbo.RoundNumber(@company_code, SUM(tb_sales.sales_amount)) AS sales_amount,
                                    dbo.RoundNumber(@company_code, SUM(ISNULL(tb_actual_cost.actual_cost, 0))) AS cost
                                FROM
                                (
                                    SELECT
                                        ISNULL(tb_individual_sales.company_code, ISNULL(tb_payment.company_code, tb_overhead_cost.company_code)) AS company_code,
                                        ISNULL(tb_individual_sales.project_sys_id, ISNULL(tb_payment.project_sys_id, tb_overhead_cost.project_sys_id)) AS project_sys_id,
                                        ISNULL(tb_individual_sales.user_sys_id, ISNULL(tb_payment.charge_person_id, tb_overhead_cost.charge_person_id)) AS user_sys_id,
                                        ISNULL(individual_sales, 0) + ISNULL(payment_amount, 0) + ISNULL(overhead_cost_amount, 0) AS sales_amount,
                                        ISNULL(individual_sales, 0) AS individual_sales
                                    FROM
                                    (
                                            SELECT 
                                                company_code,
                                                project_sys_id,
                                                user_sys_id,
                                                SUM(individual_sales) AS individual_sales
                                            FROM
                                                member_assignment_detail AS mad
                                            WHERE
                                                mad.company_code = '" + companyCode + @"'
                                                AND (mad.target_year > @sY OR (mad.target_year = @sY AND mad.target_month >= @sM))
                                                AND (mad.target_year < @eY OR (mad.target_year = @eY AND mad.target_month <= @eM))
--sa: filter by branch id when get data from member_assignment_detail
                                                " + groupOfMAD + locationOfMAD + tagIdOfMAD + @"                                               
                                            AND mad.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0' " + contractType + @")
                                            GROUP BY company_code, project_sys_id, user_sys_id
                                    ) AS tb_individual_sales
                                    FULL JOIN
                                    (
                                        SELECT
                                            sp.company_code,
                                            sp.project_sys_id,
                                            sp.charge_person_id,
                                            SUM(spd.amount) AS payment_amount
                                        FROM
                                            sales_payment_detail spd
                                            INNER JOIN
                                            (
                                                SELECT
                                                    company_code,
                                                    project_sys_id,
                                                    ordering_flg,
                                                    customer_id,
                                                    ISNULL(charge_person_id, (SELECT charge_person_id FROM project_info pi WHERE pi.project_sys_id = sales_payment.project_sys_id)) AS charge_person_id,
                                                    total_amount,
                                                    tag_id,
                                                    ins_date,
                                                    ins_id
                                                FROM
                                                    sales_payment
                                                WHERE
                                                    ordering_flg = '2'
                                            ) sp
                                                ON spd.company_code = sp.company_code
                                                AND spd.project_sys_id = sp.project_sys_id
                                                AND spd.ordering_flg = sp.ordering_flg
                                                AND spd.customer_id = sp.customer_id
                                        WHERE
                                            spd.company_code = '" + companyCode + @"'
                                            AND spd.ordering_flg = '2'
                                            AND(spd.target_year > @sY OR(spd.target_year = @sY AND spd.target_month >= @sM))
                                            AND(spd.target_year < @eY OR(spd.target_year = @eY AND spd.target_month <= @eM))
--sa: Filter by branch id when get data from sales_payment_detail
                                            " + groupOfSPD + locationOfSPD + tagIdOfSP + @"
                                        AND sp.project_sys_id IN(SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'" + contractType + @")
                                        GROUP BY sp.company_code, sp.project_sys_id, sp.charge_person_id
                                    ) AS tb_payment
                                        ON tb_individual_sales.company_code = tb_payment.company_code
                                        AND tb_individual_sales.project_sys_id = tb_payment.project_sys_id
                                        AND tb_individual_sales.user_sys_id = tb_payment.charge_person_id
                                    FULL JOIN
                                    (
                                        SELECT
                                            ocd.company_code,
                                            ocd.project_sys_id,
                                            oc.charge_person_id,
                                            SUM(ocd.amount) AS overhead_cost_amount
                                        FROM
                                            overhead_cost_detail AS ocd
                                            INNER JOIN overhead_cost AS oc
                                                ON ocd.company_code = oc.company_code
                                                AND ocd.project_sys_id = oc.project_sys_id
                                                AND ocd.detail_no = oc.detail_no
                                        WHERE
                                            ocd.company_code = '" + companyCode + @"'
                                            AND (ocd.target_year > @sY OR(ocd.target_year = @sY AND ocd.target_month >= @sM))
                                            AND(ocd.target_year < @eY OR(ocd.target_year = @eY AND ocd.target_month <= @eM))
--sa: Filter by branch id when get data from overhead_cost_detail
                                            " + groupOfOCD + locationOfOCD + tagIdOfOC + @"
                                            AND oc.project_sys_id IN(SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'" + contractType + @")
                                        GROUP BY ocd.company_code, ocd.project_sys_id, oc.charge_person_id
                                    ) AS tb_overhead_cost
                                        ON
                                        (tb_individual_sales.company_code = tb_overhead_cost.company_code
                                        OR tb_payment.company_code = tb_overhead_cost.company_code)
                                        AND(tb_individual_sales.project_sys_id = tb_overhead_cost.project_sys_id
                                        OR tb_payment.project_sys_id = tb_overhead_cost.project_sys_id)
                                        AND(tb_individual_sales.user_sys_id = tb_overhead_cost.charge_person_id
                                        OR tb_payment.charge_person_id = tb_overhead_cost.charge_person_id)
                                ) AS tb_sales
                                LEFT JOIN
                                (
                                    SELECT
                                        company_code
                                        , project_sys_id
                                        , user_sys_id
                                        , SUM (actual_cost) actual_cost
                                    FROM (
                                        SELECT tbWorkTime.company_code
                                            , tbWorkTime.project_sys_id
                                            , tbWorkTime.user_sys_id
                                            , dbo.RoundNumber(@company_code, ISNULL(
                                                (SELECT TOP 1 base_unit_cost FROM unit_price_history WHERE company_code = @company_code AND user_sys_id = tbWorkTime.user_sys_id AND apply_start_date <= CONVERT(date, CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)), 2) + '/01')	AND del_flg = '0' ORDER BY apply_start_date DESC)
                                                *(tbWorkTime.total_work_time / tbOperatingDay.operating_days), 0)) AS actual_cost
                                           , (CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/' + RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)), 2)) AS month
                                        FROM(SELECT company_code
                                                , project_sys_id
                                                , user_sys_id
                                                , actual_work_month
                                                , actual_work_year
                                                , dbo.Hour2Day(@company_code, SUM(actual_work_time)) AS total_work_time
                                            FROM member_actual_work_detail as mawd
                                            WHERE company_code = @company_code
--sa: Filter by branch id when get data from member_actual_work_detail
                                            " + groupOfMAWD + locatioOfMAWD + @"
                                            GROUP BY company_code
                                                , project_sys_id
                                                , user_sys_id
                                                , actual_work_month, actual_work_year
                                            ) AS tbWorkTime
                                        INNER HASH JOIN(
                                            SELECT *
                                            FROM dbo.GetTableDaysInMonths(@company_code, @sY, @sM, @eY, @eM)
                                        ) AS tbOperatingDay
                                        ON tbWorkTime.company_code = tbOperatingDay.company_code
                                        AND tbWorkTime.actual_work_year = tbOperatingDay.target_year
                                        AND tbWorkTime.actual_work_month = tbOperatingDay.target_month
                                    ) tbSumActualCost
                                GROUP BY company_code
                                    , project_sys_id
                                    , user_sys_id
                                ) AS tb_actual_cost
                                    ON tb_sales.company_code = tb_actual_cost.company_code
                                    AND tb_sales.project_sys_id = tb_actual_cost.project_sys_id
                                    AND tb_sales.user_sys_id = tb_actual_cost.user_sys_id
                                GROUP BY tb_sales.company_code, tb_sales.project_sys_id, tb_sales.user_sys_id
                            )AS tb_project_sales
                            GROUP BY tb_project_sales.company_code, tb_project_sales.project_sys_id
                        ) AS tb_summary
                        LEFT JOIN sales_payment AS sp
                            ON tb_summary.company_code = sp.company_code
                            AND tb_summary.project_sys_id = sp.project_sys_id
                            AND sp.ordering_flg = '1'
                        LEFT JOIN m_customer_tag tag
                            ON tag.company_code = sp.company_code
                            AND tag.tag_id = sp.tag_id",
                new
                {
                    customer_id = condition.CUSTOMER_ID,
                    company_code = companyCode,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth,
                    location_id = condition.LOCATION_ID,
                });

            if (condition.CUSTOMER_ID == 0)
                sql.Append(@"WHERE sp.customer_id IS NULL");
            else
                sql.Append(@"WHERE sp.customer_id = @customer_id", new { customer_id = condition.CUSTOMER_ID });

            sql.Append(@"
                        GROUP BY tag.tag_id, tag.tag_name
                    ) AS tb_final ) AS tbData",
                new
                {
                    customer_id = condition.CUSTOMER_ID,
                    company_code = companyCode,
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth,
                    location_id = condition.LOCATION_ID,
                });
            return sql;
        }

        /// <summary>
        /// Get list Sale Tag By Customer
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List tag by customer</returns>
        public PageInfo<SalesTagByCustomerPlus> SearchTagByCustomer(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            ConditionSaleTag condition)
        {
            Sql sql = this.buildQuerySalesTagByCustomer(companyCode, condition);

            var pageInfo = Page<SalesTagByCustomerPlus>(startItem, int.MaxValue, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Build Query SQL
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>Query SQL</returns>
        private Sql buildQuerySaleProjectByCustomer(string companyCode, ConditionSaleProject condition)
        {
            int startYear = Convert.ToDateTime(condition.START_DATE).Year;
            int startMonth = Convert.ToDateTime(condition.START_DATE).Month;
            int endYear = Convert.ToDateTime(condition.END_DATE).Year;
            int endMonth = Convert.ToDateTime(condition.END_DATE).Month;

            StringBuilder query = new StringBuilder();
            query.Append(@"
                SELECT * FROM (
                    SELECT
                        pi.project_sys_id,
                        pi.project_name AS display_name,
                        (SELECT contract_type
                        FROM m_contract_type
                        WHERE company_code = pi.company_code
                            AND contract_type_id = pi.contract_type_id
                        ) AS contract_type,
                        tb_summary.sales_amount AS total_sales,
                        tb_summary.profit AS gross_profit,
                        ISNULL(CASE WHEN sales_amount = 0 THEN 0
                                    ELSE ISNULL(profit, 0) / sales_amount
                                END, 0
                        ) AS gross_profit_rate
                    FROM
                    (
                        SELECT 
                            tb_project_sales.company_code,
                            tb_project_sales.project_sys_id,
                            SUM(sales_amount) AS sales_amount,
                            SUM(individual_sales) AS individual_sales,
                            SUM(individual_sales) - SUM(cost) AS profit
                        FROM
                        (
                            SELECT
                                tb_sales.company_code,
                                tb_sales.project_sys_id,
                                tb_sales.user_sys_id,
                                dbo.RoundNumber(@company_code, SUM(tb_sales.individual_sales)) AS individual_sales,
                                dbo.RoundNumber(@company_code, SUM(tb_sales.sales_amount)) AS sales_amount,
                                dbo.RoundNumber(@company_code, SUM(ISNULL(tb_actual_cost.actual_cost, 0))) AS cost
                            FROM
                            (
                                SELECT
                                    ISNULL(tb_individual_sales.company_code, ISNULL(tb_payment.company_code, tb_overhead_cost.company_code)) AS company_code,
                                    ISNULL(tb_individual_sales.project_sys_id, ISNULL(tb_payment.project_sys_id, tb_overhead_cost.project_sys_id)) AS project_sys_id,
                                    ISNULL(tb_individual_sales.user_sys_id, ISNULL(tb_payment.charge_person_id, tb_overhead_cost.charge_person_id)) AS user_sys_id,
                                    ISNULL(individual_sales, 0) + ISNULL(payment_amount, 0) + ISNULL(overhead_cost_amount, 0) AS sales_amount,
                                    ISNULL(individual_sales, 0) AS individual_sales
                                FROM
                                (
                                    SELECT 
                                        company_code,
                                        project_sys_id,
                                        user_sys_id,
                                        SUM(individual_sales) AS individual_sales
                                    FROM
                                        member_assignment_detail AS mad
                                    WHERE
                                        mad.company_code = '" + companyCode + @"'
                                        AND (mad.target_year > @sY OR (mad.target_year = @sY AND mad.target_month >= @sM))
                                        AND (mad.target_year < @eY OR (mad.target_year = @eY AND mad.target_month <= @eM))
                                        AND mad.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'");

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                query.Append(" AND pi.contract_type_id IN (" + condition.CONTRACT_TYPE_ID + ")");

            query.Append(@" )");

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                query.Append(@" AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = " + condition.GROUP_ID);
            }

            //SA: filter branch by member_assignment_detail (MAD)
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                query.Append(@" AND
                                (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND(actual_work_year < mad.target_year OR(actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.LOCATION_ID + ")");
            }

            if (condition.CUSTOMER_ID == 0)
            {
                query.Append(@" AND mad.project_sys_id NOT IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1') ");
            }
            else if (condition.TAG_ID == 0)
            {
                query.Append(@" AND mad.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id IS NULL) ");
            }
            else
            {
                query.Append(@" AND mad.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id = @tag_id)");
            }

            query.Append(@"
                                    GROUP BY company_code, project_sys_id, user_sys_id
                                ) AS tb_individual_sales
                                FULL JOIN
                                (
                                    SELECT
                                        sp.company_code,
                                        sp.project_sys_id,
                                        sp.charge_person_id,
                                        SUM(spd.amount) AS payment_amount
                                    FROM
                                        sales_payment_detail spd
                                        INNER JOIN
                                        (
                                            SELECT
                                                company_code,
                                                project_sys_id,
                                                ordering_flg,
                                                customer_id,
                                                ISNULL(charge_person_id, (SELECT charge_person_id FROM project_info pi WHERE pi.project_sys_id = sales_payment.project_sys_id)) AS charge_person_id,
                                                total_amount,
                                                tag_id,
                                                ins_date,
                                                ins_id
                                            FROM
                                                sales_payment
                                            WHERE 
                                                ordering_flg = '2'
                                        ) sp
                                            ON spd.company_code = sp.company_code
                                            AND spd.project_sys_id = sp.project_sys_id
                                            AND spd.ordering_flg = sp.ordering_flg
                                            AND spd.customer_id = sp.customer_id
                                    WHERE
                                        spd.company_code = '" + companyCode + @"'
                                        AND spd.ordering_flg = '2'
                                        AND (spd.target_year > @sY OR (spd.target_year = @sY AND spd.target_month >= @sM))
                                        AND (spd.target_year < @eY OR (spd.target_year = @eY AND spd.target_month <= @eM))
                                        AND sp.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'");

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                query.Append(" AND pi.contract_type_id IN (" + condition.CONTRACT_TYPE_ID + ")");

            query.Append(@" )");

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                query.Append(@" AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = " + condition.GROUP_ID);
            }

            //SA: filter branch by sales_payment_detail and sale payment (SPD)
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                query.Append(@" AND
                                (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND(actual_work_year < spd.target_year OR(actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.LOCATION_ID + ")");
            }
            if (condition.CUSTOMER_ID == 0)
            {
                query.Append(@" AND sp.project_sys_id NOT IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1') ");
            }
            else if (condition.TAG_ID == 0)
            {
                query.Append(@" AND sp.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id IS NULL) ");
            }
            else
            {
                query.Append(@" AND sp.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id = @tag_id) ");
            }

            query.Append(@"
                                    GROUP BY sp.company_code, sp.project_sys_id, sp.charge_person_id
                                ) AS tb_payment
                                    ON tb_individual_sales.company_code = tb_payment.company_code
                                    AND tb_individual_sales.project_sys_id = tb_payment.project_sys_id
                                    AND tb_individual_sales.user_sys_id = tb_payment.charge_person_id
                                FULL JOIN
                                (
                                    SELECT
                                        ocd.company_code,
                                        ocd.project_sys_id,
                                        oc.charge_person_id,
                                        SUM(ocd.amount) AS overhead_cost_amount
                                        FROM 
                                        overhead_cost_detail AS ocd
                                        INNER JOIN overhead_cost AS oc
                                            ON ocd.company_code = oc.company_code
                                            AND ocd.project_sys_id = oc.project_sys_id
                                            AND ocd.detail_no = oc.detail_no
                                        WHERE
                                        ocd.company_code = '" + companyCode + @"'
                                        AND (ocd.target_year > @sY OR (ocd.target_year = @sY AND ocd.target_month >= @sM))
                                        AND (ocd.target_year < @eY OR (ocd.target_year = @eY AND ocd.target_month <= @eM))
                                        AND oc.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = '0'");


            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                query.Append(" AND pi.contract_type_id IN (" + condition.CONTRACT_TYPE_ID + ")");

            query.Append(@")");

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                query.Append(@" AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = ocd.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = " + condition.GROUP_ID);
            }

            //SA: filter branch by overhead_cost_detail (OCD)
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                query.Append(@" AND 
								(SELECT TOP(1) location_id
									FROM enrollment_history
									WHERE company_code = ocd.company_code
									AND user_sys_id = oc.charge_person_id
									AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
									ORDER BY actual_work_year DESC, actual_work_month DESC
								) in (" + condition.LOCATION_ID + ")");
            }

            if (condition.CUSTOMER_ID == 0)
            {
                query.Append(@" AND oc.project_sys_id NOT IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1') ");
            }
            else if (condition.TAG_ID == 0)
            {
                query.Append(@" AND oc.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id IS NULL) ");
            }
            else
            {
                query.Append(@" AND oc.project_sys_id IN (SELECT project_sys_id FROM sales_payment AS sp WHERE sp.company_code = @company_code AND sp.ordering_flg = '1' AND sp.tag_id = @tag_id) ");
            }

            query.Append(@"
                                        GROUP BY ocd.company_code, ocd.project_sys_id, oc.charge_person_id
                                ) AS tb_overhead_cost
                                    ON 
                                    (tb_individual_sales.company_code = tb_overhead_cost.company_code
                                    OR tb_payment.company_code = tb_overhead_cost.company_code)
                                    AND (tb_individual_sales.project_sys_id = tb_overhead_cost.project_sys_id
                                    OR tb_payment.project_sys_id = tb_overhead_cost.project_sys_id)
                                    AND (tb_individual_sales.user_sys_id = tb_overhead_cost.charge_person_id
                                    OR tb_payment.charge_person_id = tb_overhead_cost.charge_person_id)
                            ) AS tb_sales
                            LEFT JOIN
                            (
                                SELECT
                                    company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , SUM (actual_cost) actual_cost
                                FROM (
                                    SELECT tbWorkTime.company_code
                                        , tbWorkTime.project_sys_id
                                        , tbWorkTime.user_sys_id
                                        , dbo.RoundNumber(@company_code, ISNULL(
                                            (SELECT TOP 1 base_unit_cost FROM unit_price_history WHERE company_code = @company_code AND user_sys_id = tbWorkTime.user_sys_id AND apply_start_date <= CONVERT(date, CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)), 2) + '/01')	AND del_flg = '0' ORDER BY apply_start_date DESC)
                                            * (tbWorkTime.total_work_time / tbOperatingDay.operating_days), 0)) AS actual_cost
                                        , (CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)),2)) AS month
                                    FROM (SELECT company_code
                                            , project_sys_id
                                            , user_sys_id
                                            , actual_work_month
                                            , actual_work_year
                                            , dbo.Hour2Day(@company_code, SUM(actual_work_time)) AS total_work_time
                                        FROM member_actual_work_detail as mawd
                                        WHERE company_code = @company_code");

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                query.Append(@" AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = " + condition.GROUP_ID);
            }

            //SA: filter branch by member_actual_work_detail (MAWD)
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                query.Append(@" AND
								(SELECT TOP(1) location_id
								FROM enrollment_history
								WHERE company_code = mawd.company_code
								AND user_sys_id = mawd.user_sys_id
								AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
								ORDER BY actual_work_year DESC, actual_work_month DESC
							) in (" + condition.LOCATION_ID + ")");
            }

            query.Append(@"    GROUP BY company_code
                                            , project_sys_id
                                            , user_sys_id
                                            , actual_work_month, actual_work_year
                                        ) AS tbWorkTime
                                    INNER HASH JOIN (
                                        SELECT * 
                                        FROM dbo.GetTableDaysInMonths(@company_code, @sY, @sM, @eY, @eM)
                                    ) AS tbOperatingDay
                                    ON tbWorkTime.company_code = tbOperatingDay.company_code
                                    AND tbWorkTime.actual_work_year = tbOperatingDay.target_year
                                    AND tbWorkTime.actual_work_month = tbOperatingDay.target_month
                                ) tbSumActualCost
                                GROUP BY company_code
                                    , project_sys_id
                                    , user_sys_id
                            ) AS tb_actual_cost
                                ON tb_sales.company_code = tb_actual_cost.company_code
                                AND tb_sales.project_sys_id = tb_actual_cost.project_sys_id
                                AND tb_sales.user_sys_id = tb_actual_cost.user_sys_id
                            GROUP BY tb_sales.company_code, tb_sales.project_sys_id, tb_sales.user_sys_id
                        )AS tb_project_sales
                        GROUP BY tb_project_sales.company_code, tb_project_sales.project_sys_id
                    ) AS tb_summary
                    LEFT JOIN project_info pi
                        ON tb_summary.company_code = pi.company_code
                        AND tb_summary.project_sys_id = pi.project_sys_id
                    LEFT JOIN sales_payment AS sp
                        ON tb_summary.company_code = sp.company_code
                        AND tb_summary.project_sys_id = sp.project_sys_id
                        AND sp.ordering_flg = '1'");

            if (condition.CUSTOMER_ID != 0)
            {
                query.Append(@" WHERE sp.customer_id = @customer_id ");
            }

            query.Append(" ) AS tbData");

            Sql sql = new Sql(query.ToString(),
                new
                {
                    sY = startYear,
                    sM = startMonth,
                    eY = endYear,
                    eM = endMonth,
                    company_code = companyCode,
                    tag_id = condition.TAG_ID,
                    customer_id = condition.CUSTOMER_ID
                });
            return sql;
        }

        /// <summary>
        /// Get list Sales Project By Customer
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns"columns></param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="condition">condition</param>
        /// <returns>List Sales project by customer</returns>
        public PageInfo<SalesProjectByCustomerPlus> SearchProjectByCustomer(
        int startItem,
        int itemsPerPage,
        string columns,
        int? sortCol,
        string sortDir,
        string companyCode,
        ConditionSaleProject condition)
        {
            Sql sql = this.buildQuerySaleProjectByCustomer(companyCode, condition);

            var pageInfo = Page<SalesProjectByCustomerPlus>(startItem, int.MaxValue, columns, sortCol, sortDir, sql);

            return pageInfo;
        }
    }
}