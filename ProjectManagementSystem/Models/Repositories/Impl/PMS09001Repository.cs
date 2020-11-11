#region License
/// <copyright file="PMS09001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09001;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Sales by Group repository class
    /// </summary>
    public class PMS09001Repository : Repository, IPMS09001Repository
    {
        #region Constructor
        /// <summary>
        /// Database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS09001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Method
        /// <summary>
        /// Get individual sales list
        /// </summary>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>List of individual sales</returns>
        public IList<dynamic> GetIndividualSalesList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            Condition condition)
        {
            string cols = this.BuildColumnByMonth(startDate, endDate);
            var sb = new StringBuilder();
            int sY = startDate.Year;
            int sM = startDate.Month;
            int eY = endDate.Year;
            int eM = endDate.Month;

            sb.AppendFormat(@"
                SELECT ISNULL(tbGroup.group_id, 0) AS group_id
                    , ISNULL(tbGroup.display_name, '該当なし') AS display_name
                    , tbGroup.display_order
                    , {0}
                FROM (SELECT
                        tbSalesByUser.company_code
                        , (SELECT TOP(1) group_id
                            FROM enrollment_history
                            WHERE company_code = tbSalesByUser.company_code
                            AND user_sys_id = tbSalesByUser.user_sys_id
                            AND (actual_work_year < tbSalesByUser.target_year OR (actual_work_year = tbSalesByUser.target_year AND actual_work_month <= tbSalesByUser.target_month))
                            ORDER BY actual_work_year DESC, actual_work_month DESC
                        ) group_id
                        , tbSalesByUser.sales
                        , (CAST(tbSalesByUser.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbSalesByUser.target_month AS VARCHAR(2)),2)) AS [month]
                    FROM (SELECT ISNULL((ISNULL(tbIndividual.company_code, tbPayment.company_code)), tbOvc.company_code) AS company_code
                            , ISNULL((ISNULL(tbIndividual.project_sys_id, tbPayment.project_sys_id)), tbOvc.project_sys_id) AS project_sys_id
                            , ISNULL((ISNULL(tbIndividual.user_sys_id, tbPayment.charge_person_id)), tbOvc.charge_person_id) AS user_sys_id
                            , (ISNULL(tbIndividual.individual_sales, 0) + ISNULL(tbPayment.amount, 0) + ISNULL(tbOvc.amount, 0)) AS sales
                            , ISNULL((ISNULL(tbIndividual.target_year, tbPayment.target_year)), tbOvc.target_year) AS target_year
                            , ISNULL((ISNULL(tbIndividual.target_month, tbPayment.target_month)), tbOvc.target_month) AS target_month
                        FROM (SELECT * FROM(
                                SELECT company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , SUM(individual_sales) AS individual_sales
                                    , target_year
                                    , target_month",cols);
            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.Append(@"
                    , (SELECT TOP(1) group_id
						FROM enrollment_history
						WHERE company_code = member_assignment_detail.company_code
						AND user_sys_id = member_assignment_detail.user_sys_id
						AND (actual_work_year < member_assignment_detail.target_year OR (actual_work_year = member_assignment_detail.target_year AND actual_work_month <= member_assignment_detail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) group_id 
                ");
            }
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.Append(@"
					, (SELECT TOP(1) location_id
						FROM enrollment_history
						WHERE company_code = member_assignment_detail.company_code
						AND user_sys_id = member_assignment_detail.user_sys_id
						AND (actual_work_year < member_assignment_detail.target_year OR (actual_work_year = member_assignment_detail.target_year AND actual_work_month <= member_assignment_detail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) location_id 
                ");
            }
            sb.AppendFormat(@"
                                FROM member_assignment_detail
                                WHERE company_code = '{0}'
                                    AND (target_year > {1} OR (target_year = {1} AND target_month >= {2}))
                                    AND (target_year < {3} OR (target_year = {3} AND target_month <= {4}))
                                GROUP BY company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , target_year
                                    , target_month
                                )AS resultIndividual", companyCode, sY, sM, eY, eM);
            if (!string.IsNullOrEmpty(condition.GROUP_ID) && string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultIndividual.group_id IN ({0})
                ", condition.GROUP_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID)&& string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultIndividual.location_id IN ({0})
                ", condition.LOCATION_ID);
            }
            else if(!string.IsNullOrEmpty(condition.LOCATION_ID) && !string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultIndividual.group_id IN ({0})
                    AND resultIndividual.location_id IN ({1})
                ", condition.GROUP_ID, condition.LOCATION_ID);
            }
            sb.Append(@"
                            ) AS tbIndividual FULL JOIN (
                                    SELECT * FROM(
                                        SELECT tbPayment.company_code
                                        , tbPayment.project_sys_id
                                        , ISNULL(tbPayment.charge_person_id
                                        , (SELECT charge_person_id FROM project_info WHERE company_code = tbPayment.company_code AND project_sys_id = tbPayment.project_sys_id)) AS charge_person_id
                                        , SUM(tbPaymentDetail.amount) AS amount
                                        , tbPaymentDetail.target_year
                                        , tbPaymentDetail.target_month");
            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.Append(@"
                    , (SELECT TOP(1) group_id
						FROM enrollment_history
						WHERE company_code = tbPayment.company_code
						AND user_sys_id = tbPayment.charge_person_id
						AND (actual_work_year < tbPaymentDetail.target_year OR (actual_work_year = tbPaymentDetail.target_year AND actual_work_month <= tbPaymentDetail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) group_id 
                ");
            }
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.Append(@"
					, (SELECT TOP(1) location_id
						FROM enrollment_history
						WHERE company_code = tbPayment.company_code
						AND user_sys_id = tbPayment.charge_person_id
						AND (actual_work_year < tbPaymentDetail.target_year OR (actual_work_year = tbPaymentDetail.target_year AND actual_work_month <= tbPaymentDetail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) location_id 
                ");
            }
            sb.AppendFormat(@"
                                    FROM sales_payment_detail AS tbPaymentDetail INNER JOIN sales_payment AS tbPayment
                                        ON tbPayment.company_code = tbPaymentDetail.company_code
                                        AND tbPayment.project_sys_id = tbPaymentDetail.project_sys_id
                                        AND tbPayment.customer_id = tbPaymentDetail.customer_id
                                        AND tbPayment.ordering_flg = tbPaymentDetail.ordering_flg
                                    WHERE tbPayment.company_code = '{0}'
                                        AND tbPayment.ordering_flg = '2'
                                        AND (tbPaymentDetail.target_year > {1} OR (tbPaymentDetail.target_year = {1} AND tbPaymentDetail.target_month >= {2}))
                                        AND (tbPaymentDetail.target_year < {3} OR (tbPaymentDetail.target_year = {3} AND tbPaymentDetail.target_month <= {4}))
                                    GROUP BY tbPayment.company_code
                                        , tbPayment.project_sys_id
                                        , tbPayment.charge_person_id
                                        , tbPaymentDetail.target_year
                                        , tbPaymentDetail.target_month
                                )AS resultPayment", companyCode, sY, sM, eY, eM);
            if (!string.IsNullOrEmpty(condition.GROUP_ID) && string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultPayment.group_id IN ({0})
                ", condition.GROUP_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultPayment.location_id IN ({0})
                ", condition.LOCATION_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && !string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultPayment.group_id IN ({0})
                    AND resultPayment.location_id IN ({1})
                ", condition.GROUP_ID, condition.LOCATION_ID);
            }
            sb.Append(@"
                            ) AS tbPayment
                                ON tbIndividual.company_code = tbPayment.company_code
                                AND tbIndividual.project_sys_id = tbPayment.project_sys_id
                                AND tbIndividual.user_sys_id = tbPayment.charge_person_id
                                AND tbIndividual.target_year = tbPayment.target_year
                                AND tbIndividual.target_month = tbPayment.target_month
                            FULL JOIN (SELECT * FROM(
                                    SELECT tbOvc.company_code
                                    , tbOvc.project_sys_id
                                    , tbOvc.charge_person_id
                                    , SUM(tbOvcDetail.amount) AS amount
                                    , tbOvcDetail.target_year
                                    , tbOvcDetail.target_month");
            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.Append(@"
                    , (SELECT TOP(1) group_id
						FROM enrollment_history
						WHERE company_code = tbOvc.company_code
						AND user_sys_id = tbOvc.charge_person_id
						AND (actual_work_year < tbOvcDetail.target_year OR (actual_work_year = tbOvcDetail.target_year AND actual_work_month <= tbOvcDetail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) group_id 
                ");
            }
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.Append(@"
					, (SELECT TOP(1) location_id
						FROM enrollment_history
						WHERE company_code = tbOvc.company_code
						AND user_sys_id = tbOvc.charge_person_id
						AND (actual_work_year < tbOvcDetail.target_year OR (actual_work_year = tbOvcDetail.target_year AND actual_work_month <= tbOvcDetail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) location_id 
                ");
            }
            sb.AppendFormat(@"
                                    FROM overhead_cost_detail AS tbOvcDetail
                                        INNER JOIN overhead_cost AS tbOvc
                                            ON tbOvc.company_code = tbOvcDetail.company_code
                                            AND tbOvc.project_sys_id = tbOvcDetail.project_sys_id
                                            AND tbOvc.detail_no = tbOvcDetail.detail_no
                                    WHERE tbOvc.company_code = '{0}'
                                        AND (tbOvcDetail.target_year > {1} OR (tbOvcDetail.target_year = {1} AND tbOvcDetail.target_month >= {2}))
                                        AND (tbOvcDetail.target_year < {3} OR (tbOvcDetail.target_year = {3} AND tbOvcDetail.target_month <= {4}))
                                    GROUP BY tbOvc.company_code
                                        , tbOvc.project_sys_id
                                        , tbOvc.charge_person_id
                                        , tbOvcDetail.target_year
                                        , tbOvcDetail.target_month
                                    )AS resultOvc", companyCode, sY, sM, eY, eM);
            if (!string.IsNullOrEmpty(condition.GROUP_ID) && string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultOvc.group_id IN ({0})
                ", condition.GROUP_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultOvc.location_id IN ({0})
                ", condition.LOCATION_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && !string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE resultOvc.group_id IN ({0})
                    AND resultOvc.location_id IN ({1})
                ", condition.GROUP_ID, condition.LOCATION_ID);
            }
            sb.AppendFormat(@"
                            ) AS tbOvc
                                ON (tbOvc.company_code = tbPayment.company_code OR tbOvc.company_code = tbIndividual.company_code)
                                AND (tbOvc.project_sys_id = tbPayment.project_sys_id OR tbOvc.project_sys_id = tbIndividual.project_sys_id)
                                AND (tbOvc.charge_person_id = tbPayment.charge_person_id OR tbOvc.charge_person_id = tbIndividual.user_sys_id)
                                AND (tbOvc.target_year = tbPayment.target_year OR tbOvc.target_year = tbIndividual.target_year)
                                AND (tbOvc.target_month = tbPayment.target_month OR tbOvc.target_month = tbIndividual.target_month)
                        ) AS tbSalesByUser
                            INNER HASH JOIN project_info AS tbProject
                                ON tbSalesByUser.company_code = tbProject.company_code
                                AND tbSalesByUser.project_sys_id = tbProject.project_sys_id
                    WHERE tbSalesByUser.company_code = '{1}'
                        AND tbProject.del_flg ='0'
                        AND (SELECT sales_type
                            FROM m_status
                            WHERE company_code = tbProject.company_code
                                AND status_id = tbProject.status_id) = '0' ", cols, companyCode, sY, sM, eY, eM);

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sb.AppendFormat("AND tbProject.contract_type_id IN ({0}) ", condition.CONTRACT_TYPE_ID);


            sb.AppendFormat(@"
                    ) AS x PIVOT (
                        SUM(x.sales)
                        FOR [month] IN ({0})
                    ) AS tbSales
                        FULL JOIN (
							SELECT group_id,display_name,display_order,company_code,del_flg FROM m_group", cols);

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE group_id IN ({0})
                ", condition.GROUP_ID);
            }

            sb.AppendFormat(@"
						)AS tbGroup
                            ON tbSales.company_code = tbGroup.company_code	
							AND tbSales.group_id = tbGroup.group_id
                WHERE
                    (tbGroup.company_code = '{0}' OR tbSales.company_code = '{0}')
                    AND (tbGroup.del_flg = '0' OR tbGroup.del_flg IS NULL) ", companyCode);

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
                sb.AppendFormat("AND (tbGroup.group_id IN ({0}) OR tbSales.group_id IN ({0})) ", condition.GROUP_ID);

            if (condition.SORT_COL == 0)
            {
                sb.AppendFormat("ORDER BY tbGroup.display_order {0}", condition.SORT_TYPE);
            }
            else
            {
                sb.AppendFormat("ORDER BY tbGroup.display_name {0}", condition.SORT_TYPE);
            }

            var query = new Sql(sb.ToString());



            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Get actual sales list
        /// </summary>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>List of actual sales</returns>
        public IList<dynamic> GetActualSalesList(
            DateTime startDate,
            DateTime endDate,
            string companyCode,
            Condition condition)
        {
            string cols = this.BuildColumnByMonth(startDate, endDate);
            var sb = new StringBuilder();

            sb.AppendFormat(@"SELECT ISNULL(tbGroup.group_id, 0) AS group_id
                    , ISNULL(tbGroup.display_name, '該当なし') AS display_name
                    , tbGroup.display_order
                    , {0}
                FROM (SELECT tbTempData.company_code
                        , tbTempData.group_id
                        , (CAST(tbTempData.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbTempData.target_month AS VARCHAR(2)),2)) AS month
                        , tbTempData.actual_cost
                    FROM (SELECT ISNULL(tbWorkCost.company_code, ISNULL(tbPaymentCost.company_code, tbOverheadCost.company_code)) AS company_code
                            , ISNULL(tbWorkCost.project_sys_id, ISNULL(tbPaymentCost.project_sys_id, tbOverheadCost.project_sys_id)) AS project_sys_id
                            , (SELECT TOP(1) group_id
                                FROM enrollment_history
                                WHERE (company_code = tbWorkCost.company_code OR company_code = tbPaymentCost.company_code OR company_code = tbOverheadCost.company_code)
                                    AND (user_sys_id = tbWorkCost.user_sys_id OR user_sys_id = tbPaymentCost.charge_person_id OR user_sys_id = tbOverheadCost.charge_person_id)
                                    AND ((actual_work_year < tbWorkCost.actual_work_year OR (actual_work_year = tbWorkCost.actual_work_year AND actual_work_month <= tbWorkCost.actual_work_month))
                                        OR (actual_work_year < tbPaymentCost.target_year OR (actual_work_year = tbPaymentCost.target_year AND actual_work_month <= tbPaymentCost.target_month))
                                        OR (actual_work_year < tbOverheadCost.target_year OR (actual_work_year = tbOverheadCost.target_year AND actual_work_month <= tbOverheadCost.target_month)))
                                ORDER BY actual_work_year DESC, actual_work_month DESC
                            ) AS group_id
                            , ISNULL(tbWorkCost.user_sys_id, ISNULL(tbPaymentCost.charge_person_id, tbOverheadCost.charge_person_id)) AS user_sys_id
                            , ISNULL(tbWorkCost.actual_work_year, ISNULL(tbPaymentCost.target_year, tbOverheadCost.target_year)) AS target_year
                            , ISNULL(tbWorkCost.actual_work_month, ISNULL(tbPaymentCost.target_month, tbOverheadCost.target_month)) AS target_month
                            , (ISNULL(tbWorkCost.actual_cost, 0) + ISNULL(tbPaymentCost.amount, 0) + ISNULL(tbOverheadCost.amount, 0)) AS actual_cost
                        FROM (SELECT tbWorkTime.company_code
                                , tbWorkTime.project_sys_id
                                , tbWorkTime.user_sys_id
                                , ISNULL(dbo.RoundNumber('{1}', (
                                    (SELECT TOP 1 base_unit_cost FROM unit_price_history WHERE company_code = '{1}' AND user_sys_id = tbWorkTime.user_sys_id AND apply_start_date <= CONVERT(date, CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)), 2) + '/01')	AND del_flg = '0' ORDER BY apply_start_date DESC)
                                    * (tbWorkTime.total_work_time / tbOperatingDay.operating_days))), 0) AS actual_cost
                                , tbWorkTime.actual_work_year
                                , tbWorkTime.actual_work_month
                            FROM (SELECT company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , actual_work_month
                                    , actual_work_year
                                    , dbo.Hour2Day('{1}', SUM(actual_work_time)) AS total_work_time", cols,companyCode);
            if(!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.Append(@"
                    , (SELECT TOP(1) group_id
						FROM enrollment_history
						WHERE company_code = member_actual_work_detail.company_code
						AND user_sys_id = member_actual_work_detail.user_sys_id
						AND (actual_work_year < member_actual_work_detail.actual_work_year OR (actual_work_year = member_actual_work_detail.actual_work_year AND actual_work_month <= member_actual_work_detail.actual_work_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) group_id 
                ");
            }
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.Append(@"
					, (SELECT TOP(1) location_id
						FROM enrollment_history
						WHERE company_code = member_actual_work_detail.company_code
						AND user_sys_id = member_actual_work_detail.user_sys_id
						AND (actual_work_year < member_actual_work_detail.actual_work_year OR (actual_work_year = member_actual_work_detail.actual_work_year AND actual_work_month <= member_actual_work_detail.actual_work_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) location_id 
                ");
            }
            sb.AppendFormat(@"
                        FROM member_actual_work_detail
                                WHERE company_code = '{1}'
                                    AND (actual_work_year > {2} OR (actual_work_year = {2} AND actual_work_month >= {3}))
                                    AND (actual_work_year < {4} OR (actual_work_year = {4} AND actual_work_month <= {5}))
                                GROUP BY company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , actual_work_month, actual_work_year
                                ) AS tbWorkTime
                                INNER HASH JOIN (
                                    SELECT *
                                    FROM dbo.GetTableDaysInMonths('{1}', {2} , {3}, {4}, {5})
                                ) AS tbOperatingDay
                                    ON tbWorkTime.company_code = tbOperatingDay.company_code
                                    AND tbWorkTime.actual_work_year = tbOperatingDay.target_year
                                    AND tbWorkTime.actual_work_month = tbOperatingDay.target_month"
            , cols, companyCode, startDate.Year, startDate.Month, endDate.Year, endDate.Month);

            if (!string.IsNullOrEmpty(condition.GROUP_ID) && string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbWorkTime.group_id IN ({0})
                ", condition.GROUP_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbWorkTime.location_id IN ({0})
                ", condition.LOCATION_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && !string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbWorkTime.group_id IN ({0})
                    AND tbWorkTime.location_id IN ({1})
                ", condition.GROUP_ID, condition.LOCATION_ID);
            }
            sb.Append(@"
                            ) AS tbWorkCost
                            FULL JOIN(SELECT tbPaymentResult.company_code
                                    , tbPaymentResult.project_sys_id
                                    , tbPaymentResult.charge_person_id
                                    , SUM(tbPaymentResult.amount) AS amount
                                    , tbPaymentResult.target_year
                                    , tbPaymentResult.target_month
                                FROM ( SELECT tbPayment.company_code
                                        , tbPayment.project_sys_id
                                        , ISNULL(tbPayment.charge_person_id
                                        , (SELECT charge_person_id
                                            FROM project_info
                                            WHERE company_code = tbPayment.company_code
                                                AND project_sys_id = tbPayment.project_sys_id)
                                            ) AS charge_person_id
                                        , tbPaymentDetail.amount
                                        , tbPaymentDetail.target_year
                                        , tbPaymentDetail.target_month");
            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.Append(@"
                    , (SELECT TOP(1) group_id
						FROM enrollment_history
						WHERE company_code = tbPayment.company_code
						AND user_sys_id = tbPayment.charge_person_id
						AND (actual_work_year < tbPaymentDetail.target_year OR (actual_work_year = tbPaymentDetail.target_year AND actual_work_month <= tbPaymentDetail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) group_id 
                ");
            }
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.Append(@"
					, (SELECT TOP(1) location_id
						FROM enrollment_history
						WHERE company_code = tbPayment.company_code
						AND user_sys_id = tbPayment.charge_person_id
						AND (actual_work_year < tbPaymentDetail.target_year OR (actual_work_year = tbPaymentDetail.target_year AND actual_work_month <= tbPaymentDetail.target_month))
					    ORDER BY actual_work_year DESC, actual_work_month DESC
					) location_id 
                ");
            }
            sb.AppendFormat(@"
                                    FROM sales_payment AS tbPayment
                                        LEFT JOIN sales_payment_detail AS tbPaymentDetail
                                            ON tbPayment.company_code = tbPaymentDetail.company_code
                                            AND tbPayment.project_sys_id = tbPaymentDetail.project_sys_id
                                            AND tbPayment.customer_id = tbPaymentDetail.customer_id
                                            AND tbPayment.ordering_flg = tbPaymentDetail.ordering_flg
                                    WHERE tbPayment.company_code = '{0}'
                                        AND tbPayment.ordering_flg = '2'
                                        AND (tbPaymentDetail.target_year > {1} OR (tbPaymentDetail.target_year = {1} AND tbPaymentDetail.target_month >= {2}))
                                        AND (tbPaymentDetail.target_year < {3} OR (tbPaymentDetail.target_year = {3} AND tbPaymentDetail.target_month <= {4}))
                                    ) AS tbPaymentResult"
            , companyCode, startDate.Year, startDate.Month, endDate.Year, endDate.Month);

            if (!string.IsNullOrEmpty(condition.GROUP_ID) && string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbPaymentResult.group_id IN ({0})
                ", condition.GROUP_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbPaymentResult.location_id IN ({0})
                ", condition.LOCATION_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && !string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbPaymentResult.group_id IN ({0})
                    AND tbPaymentResult.location_id IN ({1})
                ", condition.GROUP_ID, condition.LOCATION_ID);
            }

            sb.Append(@"
                                GROUP BY tbPaymentResult.company_code
                                    , tbPaymentResult.project_sys_id
                                    , tbPaymentResult.charge_person_id
                                    , tbPaymentResult.target_year
                                    , tbPaymentResult.target_month
                            ) AS tbPaymentCost
                                ON tbWorkCost.company_code = tbPaymentCost.company_code
                                AND tbWorkCost.project_sys_id = tbPaymentCost.project_sys_id
                                AND tbWorkCost.user_sys_id = tbPaymentCost.charge_person_id
                                AND tbWorkCost.actual_work_year = tbPaymentCost.target_year
                                AND tbWorkCost.actual_work_month = tbPaymentCost.target_month
                            FULL JOIN (SELECT * FROM(
                                SELECT tbOvc.company_code
                                    , tbOvc.project_sys_id
                                    , tbOvc.charge_person_id
                                    , SUM(tbOvcDetail.amount) AS amount
                                    , tbOvcDetail.target_year
                                    , tbOvcDetail.target_month");

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.Append(@"
                    , (SELECT TOP(1) group_id
						FROM enrollment_history
						WHERE company_code = tbOvc.company_code
						AND user_sys_id = tbOvc.charge_person_id
						AND (actual_work_year < tbOvcDetail.target_year OR (actual_work_year = tbOvcDetail.target_year AND actual_work_month <= tbOvcDetail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) group_id 
                ");
            }
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.Append(@"
					, (SELECT TOP(1) location_id
						FROM enrollment_history
						WHERE company_code = tbOvc.company_code
						AND user_sys_id = tbOvc.charge_person_id
						AND (actual_work_year < tbOvcDetail.target_year OR (actual_work_year = tbOvcDetail.target_year AND actual_work_month <= tbOvcDetail.target_month))
						ORDER BY actual_work_year DESC, actual_work_month DESC
					) location_id 
                ");
            }
            sb.AppendFormat(@"
                                FROM overhead_cost AS tbOvc
                                    LEFT JOIN overhead_cost_detail AS tbOvcDetail
                                        ON tbOvc.company_code = tbOvcDetail.company_code
                                        AND tbOvc.project_sys_id = tbOvcDetail.project_sys_id
                                        AND tbOvc.detail_no = tbOvcDetail.detail_no
                                WHERE tbOvc.company_code = '{0}'
                                    AND (tbOvcDetail.target_year > {1} OR (tbOvcDetail.target_year = {1} AND tbOvcDetail.target_month >= {2}))
                                    AND (tbOvcDetail.target_year < {3} OR (tbOvcDetail.target_year = {3} AND tbOvcDetail.target_month <= {4}))
                                GROUP BY tbOvc.company_code
                                    , tbOvc.project_sys_id
                                    , tbOvc.charge_person_id
                                    , tbOvcDetail.target_year
                                    , tbOvcDetail.target_month
                                ) AS tbOverheadCostResult"
            , companyCode, startDate.Year, startDate.Month, endDate.Year, endDate.Month);

            if (!string.IsNullOrEmpty(condition.GROUP_ID) && string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbOverheadCostResult.group_id IN ({0})
                ", condition.GROUP_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbOverheadCostResult.location_id IN ({0})
                ", condition.LOCATION_ID);
            }
            else if (!string.IsNullOrEmpty(condition.LOCATION_ID) && !string.IsNullOrEmpty(condition.GROUP_ID))
            {
                sb.AppendFormat(@"
                    WHERE tbOverheadCostResult.group_id IN ({0})
                    AND tbOverheadCostResult.location_id IN ({1})
                ", condition.GROUP_ID, condition.LOCATION_ID);
            }

            sb.AppendFormat(@"
                            ) AS tbOverheadCost
                                ON (tbWorkCost.company_code = tbOverheadCost.company_code OR tbPaymentCost.company_code = tbOverheadCost.company_code)
                                AND (tbWorkCost.project_sys_id = tbOverheadCost.project_sys_id OR tbPaymentCost.project_sys_id = tbOverheadCost.project_sys_id)
                                AND (tbWorkCost.user_sys_id = tbOverheadCost.charge_person_id OR tbPaymentCost.charge_person_id = tbOverheadCost.charge_person_id)
                                AND (tbWorkCost.actual_work_year = tbOverheadCost.target_year OR tbPaymentCost.target_year = tbOverheadCost.target_year)
                                AND (tbWorkCost.actual_work_month = tbOverheadCost.target_month OR tbPaymentCost.target_month = tbOverheadCost.target_month)
                        ) AS tbTempData
                        INNER JOIN project_info AS tbProject
                            ON tbTempData.company_code = tbProject.company_code
                            AND tbTempData.project_sys_id = tbProject.project_sys_id
                    WHERE tbTempData.company_code = '{1}'
                        AND tbProject.del_flg ='0'
                        AND (SELECT sales_type
                            FROM m_status
                            WHERE company_code = tbProject.company_code AND status_id = tbProject.status_id) = '0' "
                , cols, companyCode, startDate.Year, startDate.Month, endDate.Year, endDate.Month);

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sb.AppendFormat("AND tbProject.contract_type_id IN ({0})", condition.CONTRACT_TYPE_ID);

            sb.AppendFormat(@"
                    ) AS x PIVOT (
                        SUM(x.actual_cost)
                        FOR [month] IN ({0})
                    ) AS tblActualCost
                    FULL JOIN m_group AS tbGroup
                        ON tblActualCost.company_code = tbGroup.company_code
                        AND tblActualCost.group_id = tbGroup.group_id
                WHERE (tbGroup.company_code = '{1}' OR tblActualCost.company_code = '{1}')
                    AND (tbGroup.del_flg = '0' OR tbGroup.del_flg IS NULL) ", cols, companyCode);

            if (!string.IsNullOrEmpty(condition.GROUP_ID))
                sb.AppendFormat("AND (tbGroup.group_id IN ({0}) OR tblActualCost.group_id IN ({0})) ", condition.GROUP_ID);

            if (condition.SORT_COL == 0)
            {
                sb.AppendFormat("ORDER BY tbGroup.display_order {0}", condition.SORT_TYPE);
            }
            else
            {
                sb.AppendFormat("ORDER BY tbGroup.display_name {0}", condition.SORT_TYPE);
            }

            var query = new Sql(sb.ToString());

            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Build query for select sales group details
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private Sql buildQuerySalesGroupDetail(SalesGroupDetailCondition condition)
        {
            var sb = new StringBuilder();
            sb.Append(@"
            SELECT tbData.* FROM (
            SELECT
                tb_final.*,
                (sales_amount - cost) AS profit
            FROM
            (
                SELECT 
                    pi.project_sys_id,
                    pi.project_no,
                    pi.project_name,
                    SUM(sales_amount) AS sales_amount,
                    SUM(cost) AS cost
                FROM
                (
                    SELECT
                        tb_sales.company_code,
                        tb_sales.project_sys_id,
                        tb_sales.user_sys_id,
                        dbo.RoundNumber(@company_code, SUM(tb_sales.sales_amount)) AS sales_amount,
                        dbo.RoundNumber(@company_code, SUM(ISNULL(tb_sales.payment_cost, 0))) + dbo.RoundNumber(@company_code, SUM(ISNULL(tb_actual_cost.actual_cost, 0))) AS cost
                    FROM
                    (
                        SELECT
                            ISNULL(tb_individual_sales.company_code, ISNULL(tb_payment.company_code, tb_overhead_cost.company_code)) AS company_code,
                            ISNULL(tb_individual_sales.project_sys_id, ISNULL(tb_payment.project_sys_id, tb_overhead_cost.project_sys_id)) AS project_sys_id,
                            ISNULL(tb_individual_sales.user_sys_id, ISNULL(tb_payment.charge_person_id, tb_overhead_cost.charge_person_id)) AS user_sys_id,
                            ISNULL(individual_sales, 0) + ISNULL(payment_amount, 0) + ISNULL(overhead_cost_amount, 0) AS sales_amount,
                            ISNULL(payment_amount, 0) + ISNULL(overhead_cost_amount, 0) AS payment_cost
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
                                mad.company_code = @company_code
                                AND mad.target_year = @selected_year
                                AND mad.target_month = @selected_month ");

            if (condition.GroupId == 0)
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IS NULL ");
            }
            else
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id ");
            }

            //Sa comment: condition of filter by branch of member_assignment_detail table
            if (!string.IsNullOrEmpty(condition.LocationID))
            {
                sb.Append(@"    AND (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in ("+ condition.LocationID + ")");
            }

            sb.Append(@"
                                AND mad.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = 0");

            if (!string.IsNullOrEmpty(condition.ContractTypeID))
                sb.AppendFormat(" AND pi.contract_type_id IN ({0}) ", condition.ContractTypeID);

            sb.Append(@" ) GROUP BY company_code, project_sys_id, user_sys_id
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
                                        ISNULL(charge_person_id, (SELECT charge_person_id FROM project_info pi WHERE pi.project_sys_id = sales_payment.project_sys_id)) AS charge_person_id
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
                                spd.company_code = @company_code
                                AND spd.ordering_flg = '2'
                                AND spd.target_year = @selected_year
                                AND spd.target_month = @selected_month
            ");

            if (condition.GroupId == 0)
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IS NULL ");
            }
            else
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id ");
            }

            //Sa comment: condition of filter by branch of sales_payment_detail table
            if (!string.IsNullOrEmpty(condition.LocationID))
            {
                sb.Append(@"    AND (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.LocationID + ")");
            }

            sb.Append(@" 
                                AND sp.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = 0");

            if (!string.IsNullOrEmpty(condition.ContractTypeID))
                sb.AppendFormat(" AND pi.contract_type_id IN ({0}) ", condition.ContractTypeID);

            sb.Append(@" ) GROUP BY sp.company_code, sp.project_sys_id, sp.charge_person_id
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
                                ocd.company_code = @company_code
                                AND ocd.target_year = @selected_year
                                AND ocd.target_month = @selected_month ");

            if (condition.GroupId == 0)
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = ocd.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IS NULL ");
            }
            else
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = ocd.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id ");
            }

            //Sa comment: condition of filter by branch of overhead_cost_detail table
            if (!string.IsNullOrEmpty(condition.LocationID))
            {
                sb.Append(@"    AND (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = ocd.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.LocationID + ")");
            }

            sb.Append(@"
                                AND oc.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = 0");

            if (!string.IsNullOrEmpty(condition.ContractTypeID))
                sb.AppendFormat(" AND pi.contract_type_id IN ({0}) ", condition.ContractTypeID);

            sb.Append(@" ) GROUP BY ocd.company_code, ocd.project_sys_id, oc.charge_person_id
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
                            mawd.company_code,
                            mawd.project_sys_id,
                            mawd.user_sys_id,
                            (
                                (SELECT TOP 1 base_unit_cost FROM unit_price_history WHERE company_code = mawd.company_code AND user_sys_id = mawd.user_sys_id AND apply_start_date <= CONVERT(date, CAST(mawd.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mawd.actual_work_month AS VARCHAR(2)), 2) + '/01')	AND del_flg = '0' ORDER BY apply_start_date DESC) * 
                                dbo.Hour2Day(@company_code, SUM(actual_work_time)) /
                                dbo.GetNumberOfWorkDaysInMonth(@company_code, @selected_month, @selected_year)
                            ) AS actual_cost
                        FROM
                            member_actual_work_detail AS mawd
                        WHERE
                            mawd.company_code = @company_code
                            AND mawd.actual_work_year = @selected_year
                            AND mawd.actual_work_month = @selected_month ");

            //Sa comment: condition of filter by group of member_actual_work_detail table
            if (condition.GroupId == 0)
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IS NULL ");
            }
            else
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id ");
            }

            //Sa comment: condition of filter by branch of member_actual_work_detail table
            if (!string.IsNullOrEmpty(condition.LocationID))
            {
                sb.Append(@"    AND (SELECT TOP(1) location_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) in (" + condition.LocationID + ")");
            }


            sb.Append(@"
                        GROUP BY mawd.company_code, mawd.user_sys_id, mawd.project_sys_id, mawd.actual_work_year, mawd.actual_work_month
                    ) AS tb_actual_cost
                        ON tb_sales.company_code = tb_actual_cost.company_code
                        AND tb_sales.project_sys_id = tb_actual_cost.project_sys_id
                        AND tb_sales.user_sys_id = tb_actual_cost.user_sys_id
                    GROUP BY tb_sales.company_code, tb_sales.project_sys_id, tb_sales.user_sys_id
                )AS tb_temp
                INNER JOIN project_info AS pi
                    ON tb_temp.company_code = pi.company_code
                    AND tb_temp.project_sys_id = pi.project_sys_id");

            if (!string.IsNullOrEmpty(condition.ContractTypeID))
                sb.AppendFormat(" WHERE pi.contract_type_id IN ({0}) ", condition.ContractTypeID);

            sb.Append(@" GROUP BY pi.project_sys_id, pi.project_no, pi.project_name
            ) AS tb_final ) AS tbData
            ");
            Sql sql = new Sql(sb.ToString(),
               new { company_code = condition.CompanyCode },
               new { group_id = condition.GroupId },
               new { selected_year = condition.SelectedYear },
               new { selected_month = condition.SelectedMonth },
               new { location_id = condition.LocationID }
            );

            return sql;
        }

        /// <summary>
        /// Sales list by group detail
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<ProjectSalesInfo> GetListSalesGroupDetail(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            SalesGroupDetailCondition condition)
        {
            condition.CompanyCode = companyCode;
            var sql = buildQuerySalesGroupDetail(condition);

            var pageInfo = Page<ProjectSalesInfo>(startItem, int.MaxValue, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get Sales Group Detail Summary
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string GetGroupName(int group_id)
        {
            var sb = new StringBuilder();
            sb.Append(@" 
                SELECT
                    display_name
                FROM
                    m_group
                WHERE
                    group_id = @group_id 
            ");

            Sql sql = new Sql(sb.ToString(),
               new { group_id = group_id }
            );
            return _database.FirstOrDefault<string>(sql);
        }

        /// <summary>
        /// Build query for select sales group details
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private Sql buildQuerySalesProjectDetail(SalesProjectDetailCondition condition)
        {
            var sb = new StringBuilder();
            sb.Append(@"
            SELECT tbData.* FROM (
            SELECT
                user_sys_id,
                user_name,
                sales_amount,
                cost,
                sales_amount - cost AS profit
            FROM
            (
                SELECT
                    m_user.user_sys_id,
                    m_user.display_name AS user_name,
                    dbo.RoundNumber(@company_code, SUM(tb_sales.sales_amount)) AS sales_amount,
                    dbo.RoundNumber(@company_code, SUM(ISNULL(tb_sales.payment_cost, 0) + ISNULL(tb_actual_cost.actual_cost, 0))) AS cost
                FROM
                (
                    SELECT
                        ISNULL(tb_individual_sales.company_code, ISNULL(tb_payment.company_code, tb_overhead_cost.company_code)) AS company_code,
                        ISNULL(tb_individual_sales.project_sys_id, ISNULL(tb_payment.project_sys_id, tb_overhead_cost.project_sys_id)) AS project_sys_id,
                        ISNULL(tb_individual_sales.user_sys_id, ISNULL(tb_payment.charge_person_id, tb_overhead_cost.charge_person_id)) AS user_sys_id,
                        ISNULL(individual_sales, 0) + ISNULL(payment_amount, 0) + ISNULL(overhead_cost_amount, 0) AS sales_amount,
                        ISNULL(payment_amount, 0) + ISNULL(overhead_cost_amount, 0) AS payment_cost
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
                                mad.company_code = @company_code
                                AND mad.target_year = @selected_year
                                AND mad.target_month = @selected_month ");

            if (condition.GroupId == 0)
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IS NULL ");
            }
            else
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mad.company_code
                                    AND user_sys_id = mad.user_sys_id
                                    AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id ");
            }

            sb.Append(@"        AND mad.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = 0)
                                AND mad.project_sys_id = @project_id
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
                                    ISNULL(charge_person_id, (SELECT charge_person_id FROM project_info pi WHERE pi.project_sys_id = sales_payment.project_sys_id)) AS charge_person_id
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
                            spd.company_code = @company_code
                            AND spd.ordering_flg = '2'
                            AND spd.target_year = @selected_year
                            AND spd.target_month = @selected_month
            ");

            if (condition.GroupId == 0)
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IS NULL ");
            }
            else
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = spd.company_code
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id ");
            }

            sb.Append(@"    AND sp.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = 0)
                            AND sp.project_sys_id = @project_id
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
                            ocd.company_code = @company_code
                            AND ocd.target_year = @selected_year
                            AND ocd.target_month = @selected_month ");

            if (condition.GroupId == 0)
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = ocd.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IS NULL ");
            }
            else
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = ocd.company_code
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id ");
            }

            sb.Append(@"    AND oc.project_sys_id IN (SELECT project_sys_id FROM project_info AS pi INNER JOIN m_status ON pi.status_id = m_status.status_id WHERE m_status.sales_type = '0' AND pi.del_flg = 0)
                            AND oc.project_sys_id = @project_id
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
                        mawd.company_code,
                        mawd.project_sys_id,
                        mawd.user_sys_id,
                        (
                            (SELECT TOP 1 base_unit_cost FROM unit_price_history WHERE company_code = mawd.company_code AND user_sys_id = mawd.user_sys_id AND apply_start_date <= CONVERT(date, CAST(mawd.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mawd.actual_work_month AS VARCHAR(2)), 2) + '/01')	AND del_flg = '0' ORDER BY apply_start_date DESC) * 
                            dbo.Hour2Day(@company_code, SUM(actual_work_time)) /
                            dbo.GetNumberOfWorkDaysInMonth(@company_code,  @selected_month, @selected_year)
                        )  AS actual_cost
                    FROM
                        member_actual_work_detail AS mawd
                    WHERE
                        mawd.company_code = @company_code
                        AND mawd.actual_work_year = @selected_year
                        AND mawd.actual_work_month = @selected_month
                        AND mawd.project_sys_id = @project_id ");

            if (condition.GroupId == 0)
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) IS NULL ");
            }
            else
            {
                sb.Append(@"    AND (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = mawd.company_code
                                    AND user_sys_id = mawd.user_sys_id
                                    AND (actual_work_year < mawd.actual_work_year OR (actual_work_year = mawd.actual_work_year AND actual_work_month <= mawd.actual_work_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) = @group_id ");
            }

            sb.Append(@"
                    GROUP BY mawd.company_code, mawd.user_sys_id, mawd.project_sys_id, mawd.actual_work_year, mawd.actual_work_month
                ) AS tb_actual_cost
                    ON tb_sales.company_code = tb_actual_cost.company_code
                    AND tb_sales.project_sys_id = tb_actual_cost.project_sys_id
                    AND tb_sales.user_sys_id = tb_actual_cost.user_sys_id
                INNER JOIN m_user
                    ON tb_sales.company_code = m_user.company_code
                    AND tb_sales.user_sys_id = m_user.user_sys_id
                GROUP BY m_user.user_sys_id, m_user.display_name
            ) AS tb_final ) AS tbData
            ");
            Sql sql = new Sql(sb.ToString(),
               new { company_code = condition.CompanyCode },
               new { group_id = condition.GroupId },
               new { project_id = condition.ProjectId },
               new { selected_year = condition.SelectedYear },
               new { selected_month = condition.SelectedMonth }
            );

            return sql;
        }

        /// <summary>
        /// Sales list by project detail
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<PersonalSalesInfo> GetListSalesProjectDetail(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            SalesProjectDetailCondition condition)
        {
            var sql = buildQuerySalesProjectDetail(condition);
            var pageInfo = Page<PersonalSalesInfo>(startItem, int.MaxValue, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get project detail basic info
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public SalesProjectDetailBasicInfo GetProjectDetailBasicInfo(int group_id, int project_id)
        {
            var sb = new StringBuilder();
            sb.Append(@" 
                SELECT
                    pi.project_name,
                    pi.start_date,
                    pi.end_date,
                    ISNULL((SELECT display_name FROM m_group WHERE group_id = @group_id), '該当なし') AS group_name
                FROM
                    project_info AS pi
                WHERE
                    project_sys_id = @project_sys_id 
            ");

            Sql sql = new Sql(sb.ToString(),
               new { group_id = group_id },
               new { project_sys_id = project_id }
            );
            return _database.FirstOrDefault<SalesProjectDetailBasicInfo>(sql);
        }
        #endregion
    }
}
