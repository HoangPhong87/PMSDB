#region License
/// <copyright file="PMS09002Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09002;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Sales by Personal repository class
    /// </summary>
    public class PMS09002Repository : Repository, IPMS09002Repository
    {
        #region Constructor
        /// <summary>
        /// Database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09002Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS09002Repository(PMSDatabase database)
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
            var currentUser = HttpContext.Current.Session[Constant.SESSION_LOGIN_USER] as LoginUser;

            sb.AppendFormat(@"
                SELECT 
                    ISNULL(tbSales.group_id, 0) group_id
                    , ISNULL(tbSales.location_id,0) location_id
                    , tbUserData.user_sys_id
                    , (SELECT display_order FROM m_group WHERE company_code = tbUserData.company_code AND group_id = tbUserData.group_id) AS group_display_order
                    , (SELECT display_name FROM m_group WHERE company_code = tbUserData.company_code AND group_id = tbSales.group_id) AS group_name
                    , tbUserData.display_name
                    , tbUserData.del_flg
                    , (SELECT display_name FROM m_business_location WHERE company_code = tbUserData.company_code AND location_id = tbSales.location_id) AS location_name
                    , {0}
                FROM (
                SELECT
                    tbResult.company_code
                    , tbResult.user_sys_id
                    , tbResult.sales
                    , (CAST(tbResult.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbResult.target_month AS VARCHAR(2)),2)) AS month
                    , (SELECT TOP(1) group_id
                        FROM enrollment_history
                        WHERE company_code = tbResult.company_code
                        AND user_sys_id = tbResult.user_sys_id
                        AND (actual_work_year < tbResult.target_year OR (actual_work_year = tbResult.target_year AND actual_work_month <= tbResult.target_month))
                        ORDER BY actual_work_year DESC, actual_work_month DESC
                    ) group_id 
                    , (SELECT TOP(1) location_id
                        FROM enrollment_history
                        WHERE company_code = tbResult.company_code
                        AND user_sys_id = tbResult.user_sys_id
                        AND (actual_work_year < tbResult.target_year OR (actual_work_year = tbResult.target_year AND actual_work_month <= tbResult.target_month))
                        ORDER BY actual_work_year DESC, actual_work_month DESC
                    ) location_id 
                FROM (
                    SELECT 
                        ISNULL((ISNULL(tbIndividual.company_code, tbPayment.company_code)), tbOvc.company_code) AS company_code
                        , ISNULL((ISNULL(tbIndividual.user_sys_id, tbPayment.charge_person_id)), tbOvc.charge_person_id) AS user_sys_id
                        , (ISNULL(tbIndividual.individual_sales, 0) + ISNULL(tbPayment.amount, 0) + ISNULL(tbOvc.amount, 0)) AS sales
                        , ISNULL((ISNULL(tbIndividual.target_year, tbPayment.target_year)), tbOvc.target_year) AS target_year
                        , ISNULL((ISNULL(tbIndividual.target_month, tbPayment.target_month)), tbOvc.target_month) AS target_month
                    FROM (SELECT tbAssignment.company_code
                            , tbAssignment.project_sys_id
                            , tbAssignment.user_sys_id
                            , SUM(tbAssignment.individual_sales) AS individual_sales
                            , tbAssignment.target_year
                            , tbAssignment.target_month
                        FROM member_assignment_detail AS tbAssignment
                        INNER JOIN project_info AS tbProject
                                                ON tbProject.company_code = tbAssignment.company_code
                                                AND tbProject.project_sys_id = tbAssignment.project_sys_id
                                                AND tbProject.del_flg = '0'
                                                AND (SELECT sales_type
                                                    FROM m_status
                                                    WHERE company_code = tbProject.company_code AND status_id = tbProject.status_id) = '0' ", cols);

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sb.AppendFormat("AND tbProject.contract_type_id IN ({0}) ", condition.CONTRACT_TYPE_ID);

            sb.AppendFormat(@"
                        WHERE tbAssignment.company_code = '{0}'
                            AND (tbAssignment.target_year > {1} OR (tbAssignment.target_year = {1} AND tbAssignment.target_month >= {2}))
                            AND (tbAssignment.target_year < {3} OR (tbAssignment.target_year = {3} AND tbAssignment.target_month <= {4}))
                        GROUP BY tbAssignment.company_code
                            , tbAssignment.project_sys_id
                            , tbAssignment.user_sys_id
                            , tbAssignment.target_year
                            , tbAssignment.target_month
                        ) AS tbIndividual
                        FULL JOIN (
                            SELECT tbPaymentResult.company_code
                                , tbPaymentResult.project_sys_id
                                , tbPaymentResult.charge_person_id
                                , SUM(tbPaymentResult.amount) AS amount
                                , tbPaymentResult.target_year
                                , tbPaymentResult.target_month
                            FROM (SELECT tbPayment.company_code
                                    , tbPayment.project_sys_id
                                    , ISNULL(tbPayment.charge_person_id
                                        , (SELECT charge_person_id
                                            FROM project_info
                                            WHERE company_code = tbPayment.company_code
                                                AND project_sys_id = tbPayment.project_sys_id)
                                        ) AS charge_person_id
                                    , tbPaymentDetail.amount
                                    , tbPaymentDetail.target_year
                                    , tbPaymentDetail.target_month
                                FROM sales_payment_detail AS tbPaymentDetail
                                    INNER JOIN sales_payment AS tbPayment
                                        ON tbPayment.company_code = tbPaymentDetail.company_code
                                        AND tbPayment.project_sys_id = tbPaymentDetail.project_sys_id
                                        AND tbPayment.customer_id = tbPaymentDetail.customer_id
                                        AND tbPayment.ordering_flg = tbPaymentDetail.ordering_flg
                                    INNER JOIN project_info AS tbProject
                                                    ON tbProject.company_code = tbPayment.company_code
                                                    AND tbProject.project_sys_id = tbPayment.project_sys_id
                                                    AND tbProject.del_flg = '0'
                                                    AND (SELECT sales_type
                                                        FROM m_status
                                                        WHERE company_code = tbProject.company_code AND status_id = tbProject.status_id) = '0' ", companyCode, sY, sM, eY, eM);

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sb.AppendFormat("AND tbProject.contract_type_id IN ({0}) ", condition.CONTRACT_TYPE_ID);

            sb.AppendFormat(@"
                                WHERE
                                    tbPayment.company_code = '{0}'
                                    AND (tbPaymentDetail.target_year > {1} OR (tbPaymentDetail.target_year = {1} AND tbPaymentDetail.target_month >= {2}))
                                    AND (tbPaymentDetail.target_year < {3} OR (tbPaymentDetail.target_year = {3} AND tbPaymentDetail.target_month <= {4}))
                            ) AS tbPaymentResult
                            GROUP BY tbPaymentResult.company_code
                                , tbPaymentResult.project_sys_id
                                , tbPaymentResult.charge_person_id
                                , tbPaymentResult.target_year
                                , tbPaymentResult.target_month
                        ) AS tbPayment
                            ON tbIndividual.company_code = tbPayment.company_code
                            AND tbIndividual.project_sys_id = tbPayment.project_sys_id
                            AND tbIndividual.user_sys_id = tbPayment.charge_person_id
                            AND tbIndividual.target_year = tbPayment.target_year
                            AND tbIndividual.target_month = tbPayment.target_month
                        FULL JOIN (
                            SELECT tbOvc.company_code
                                , tbOvc.project_sys_id
                                , tbOvc.charge_person_id
                                , SUM(tbOvcDetail.amount) AS amount
                                , tbOvcDetail.target_year
                                , tbOvcDetail.target_month
                            FROM overhead_cost_detail AS tbOvcDetail
                                INNER JOIN overhead_cost AS tbOvc
                                    ON tbOvc.company_code = tbOvcDetail.company_code
                                    AND tbOvc.project_sys_id = tbOvcDetail.project_sys_id
                                    AND tbOvc.detail_no = tbOvcDetail.detail_no
                                INNER JOIN project_info AS tbProject
                                        ON tbProject.company_code = tbOvc.company_code
                                        AND tbProject.project_sys_id = tbOvc.project_sys_id
                                        AND tbProject.del_flg = '0'
                                        AND (SELECT sales_type
                                            FROM m_status
                                            WHERE company_code = tbProject.company_code AND status_id = tbProject.status_id) = '0' ", companyCode, sY, sM, eY, eM);

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
                sb.AppendFormat("AND tbProject.contract_type_id IN ({0}) ", condition.CONTRACT_TYPE_ID);

            sb.AppendFormat(@"
                            WHERE tbOvc.company_code = '{0}'
                                AND (tbOvcDetail.target_year > {1} OR (tbOvcDetail.target_year = {1} AND tbOvcDetail.target_month >= {2}))
                                AND (tbOvcDetail.target_year < {3} OR (tbOvcDetail.target_year = {3} AND tbOvcDetail.target_month <= {4}))
                            GROUP BY tbOvc.company_code
                                , tbOvc.project_sys_id
                                , tbOvc.charge_person_id
                                , tbOvcDetail.target_year
                                , tbOvcDetail.target_month
                        ) AS tbOvc
                            ON (tbOvc.company_code = tbPayment.company_code OR tbOvc.company_code = tbIndividual.company_code)
                            AND (tbOvc.project_sys_id = tbPayment.project_sys_id OR tbOvc.project_sys_id = tbIndividual.project_sys_id)
                            AND (tbOvc.charge_person_id = tbPayment.charge_person_id OR tbOvc.charge_person_id = tbIndividual.user_sys_id)
                            AND (tbOvc.target_year = tbPayment.target_year OR tbOvc.target_year = tbIndividual.target_year)
                            AND (tbOvc.target_month = tbPayment.target_month OR tbOvc.target_month = tbIndividual.target_month)"
                            ,companyCode, sY, sM, eY, eM);

            sb.AppendFormat(@"
                            UNION ALL
                            (SELECT mu.company_code, mu.user_sys_id, 0 as sales, fixedTable.target_year ,fixedTable.target_month FROM m_user AS mu
							 CROSS JOIN(" + BuildFixedDataTableByMonth(startDate,endDate)+
                             @"
                            ) fixedTable
								where mu.company_code = '{0}' and mu.del_flg = '0'
								AND (mu.retirement_date IS NULL OR mu.retirement_date >= @CurrentDate))
                            ",companyCode);

            sb.AppendFormat(@"
                    ) tbResult 
                    ) AS x
                    PIVOT (
                        SUM(x.sales)
                        FOR [month] IN ({1})
                    ) AS tbSales
                    INNER JOIN m_user AS tbUserData
                        ON tbSales.company_code = tbUserData.company_code
                        AND tbSales.user_sys_id = tbUserData.user_sys_id
                WHERE
                    (tbUserData.company_code = '{0}' OR tbSales.company_code = '{0}') ", companyCode, cols);

            if (condition.GROUP_ID != null)
                sb.AppendFormat(" AND tbSales.group_id = {0} ", condition.GROUP_ID.Value);
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
                sb.AppendFormat(" AND tbSales.location_id IN({0}) ", condition.LOCATION_ID.ToString());

            if (!string.IsNullOrEmpty(condition.USER_NAME))
                sb.AppendFormat(" AND (tbUserData.display_name LIKE '{0}' ESCAPE '\\' OR tbUserData.user_name_sei LIKE '{0}' ESCAPE '\\' OR tbUserData.user_name_mei LIKE '{0}' ESCAPE '\\' OR tbUserData.furigana_sei LIKE '{0}' ESCAPE '\\' OR tbUserData.furigana_mei LIKE '{0}' ESCAPE '\\' ) ", "%" + replaceWildcardCharacters(condition.USER_NAME) + "%");

            if (!condition.DELETE_FLG)
                sb.Append(" AND tbUserData.del_flg = '0' ");

            if (!condition.RETIREMENT_INCLUDE)
                sb.Append(" AND (tbUserData.retirement_date IS NULL OR tbUserData.retirement_date >= @CurrentDate) ");

            if (condition.IS_PRIVATE)
                sb.AppendFormat(" AND tbUserData.user_sys_id = {0} ", currentUser.UserId);

            if (condition.SORT_COL.Value == 1)
                sb.AppendFormat(" ORDER BY group_name {0} ", condition.SORT_TYPE);

            if (condition.SORT_COL.Value == 2 || condition.SORT_COL.Value == 0)
                sb.AppendFormat(" ORDER BY tbUserData.display_name {0} ", condition.SORT_TYPE);

            if (condition.SORT_COL.Value == 3)
                sb.AppendFormat(" ORDER BY location_name {0} ", condition.SORT_TYPE);

            if (condition.SORT_COL.Value > 3)
            {
                sb.AppendFormat(" ORDER BY {0} {1} ", cols.Split(',')[condition.SORT_COL.Value - 3], condition.SORT_TYPE);
            }

            var query = new Sql(sb.ToString(), new { CurrentDate = Utility.GetCurrentDateTime() });

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
            int sY = startDate.Year;
            int sM = startDate.Month;
            int eY = endDate.Year;
            int eM = endDate.Month;
            var currentUser = HttpContext.Current.Session[Constant.SESSION_LOGIN_USER] as LoginUser;

            sb.AppendFormat(@"
                 SELECT 
                    ISNULL(tblActualCost.group_id, 0) group_id
                    , ISNULL(tblActualCost.location_id, 0) location_id
                    , tbUser.user_sys_id
                    , (SELECT display_order FROM m_group WHERE company_code = tbUser.company_code AND group_id = tbUser.group_id) AS group_display_order
                    , (SELECT display_name FROM m_group WHERE company_code = tbUser.company_code AND group_id = tblActualCost.group_id) AS group_name
                    , tbUser.display_name
                    , tbUser.del_flg
                    , (SELECT display_name FROM m_business_location WHERE company_code = tbUser.company_code AND location_id = tblActualCost.location_id) AS location_name
                    , {0}
                FROM (
                SELECT tbTempData.company_code
                        , tbTempData.user_sys_id
                        , (CAST(tbTempData.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbTempData.target_month AS VARCHAR(2)),2)) AS month
                        , (SELECT TOP(1) group_id
                            FROM enrollment_history
                            WHERE company_code = tbTempData.company_code
                            AND user_sys_id = tbTempData.user_sys_id
                            AND (actual_work_year < tbTempData.target_year OR (actual_work_year = tbTempData.target_year AND actual_work_month <= tbTempData.target_month))
                            ORDER BY actual_work_year DESC, actual_work_month DESC
                        ) group_id 
                        , (SELECT TOP(1) location_id
                            FROM enrollment_history
                            WHERE company_code = tbTempData.company_code
                            AND user_sys_id = tbTempData.user_sys_id
                            AND (actual_work_year < tbTempData.target_year OR (actual_work_year = tbTempData.target_year AND actual_work_month <= tbTempData.target_month))
                            ORDER BY actual_work_year DESC, actual_work_month DESC
                        ) location_id
                        , tbTempData.actual_cost
                    FROM (SELECT ISNULL(tbWorkCost.company_code, ISNULL(tbPaymentCost.company_code, tbOverheadCost.company_code)) AS company_code
                            , ISNULL(tbWorkCost.project_sys_id, ISNULL(tbPaymentCost.project_sys_id, tbOverheadCost.project_sys_id)) AS project_sys_id
                            , ISNULL(tbWorkCost.user_sys_id, ISNULL(tbPaymentCost.charge_person_id, tbOverheadCost.charge_person_id)) AS user_sys_id
                            , (ISNULL(tbWorkCost.actual_cost, 0) + ISNULL(tbPaymentCost.amount, 0) + ISNULL(tbOverheadCost.amount, 0)) AS actual_cost
                            , ISNULL(tbWorkCost.actual_work_year, ISNULL(tbPaymentCost.target_year, tbOverheadCost.target_year)) AS target_year
                            , ISNULL(tbWorkCost.actual_work_month, ISNULL(tbPaymentCost.target_month, tbOverheadCost.target_month)) AS target_month
                        FROM (SELECT tbWorkTime.company_code
                            , tbWorkTime.project_sys_id
                            , tbWorkTime.user_sys_id
                            , ISNULL(dbo.RoundNumber('{1}', (
                                (SELECT TOP 1 base_unit_cost FROM unit_price_history WHERE company_code = '{1}' AND user_sys_id = tbWorkTime.user_sys_id AND apply_start_date <= CONVERT(date, CAST(tbWorkTime.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbWorkTime.actual_work_month AS VARCHAR(2)), 2) + '/01') AND del_flg = '0' ORDER BY apply_start_date DESC)
                                * (tbWorkTime.total_work_time / tbOperatingDay.operating_days))), 0) AS actual_cost
                                ,tbWorkTime.actual_work_year
                                ,tbWorkTime.actual_work_month
                            FROM (SELECT company_code
                                    , project_sys_id
                                    , user_sys_id
                                    , actual_work_month
                                    , actual_work_year
                                    , dbo.Hour2Day('{1}', SUM(actual_work_time)) AS total_work_time", cols, companyCode);
            if (condition.GROUP_ID != null)
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
                                    FROM dbo.GetTableDaysInMonths('{1}', {2}, {3}, {4}, {5})
                                ) AS tbOperatingDay
                                    ON tbWorkTime.company_code = tbOperatingDay.company_code
                                    AND tbWorkTime.actual_work_year = tbOperatingDay.target_year
                                    AND tbWorkTime.actual_work_month = tbOperatingDay.target_month
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
                                        ,tbPaymentDetail.target_year
                                        ,tbPaymentDetail.target_month
                                    FROM sales_payment AS tbPayment
                                        LEFT JOIN sales_payment_detail AS tbPaymentDetail
                                            ON tbPayment.company_code = tbPaymentDetail.company_code
                                            AND tbPayment.project_sys_id = tbPaymentDetail.project_sys_id
                                            AND tbPayment.customer_id = tbPaymentDetail.customer_id
                                            AND tbPayment.ordering_flg = tbPaymentDetail.ordering_flg
                                    WHERE tbPayment.company_code = '{1}'
                                        AND tbPayment.ordering_flg = '2'
                                        AND (tbPaymentDetail.target_year > {2} OR (tbPaymentDetail.target_year = {2} AND tbPaymentDetail.target_month >= {3}))
                                        AND (tbPaymentDetail.target_year < {4} OR (tbPaymentDetail.target_year = {4} AND tbPaymentDetail.target_month <= {5}))
                                    ) AS tbPaymentResult
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
                            FULL JOIN (SELECT tbOvc.company_code
                                    , tbOvc.project_sys_id
                                    , tbOvc.charge_person_id
                                    , SUM(tbOvcDetail.amount) AS amount
                                    ,tbOvcDetail.target_year
                                    ,tbOvcDetail.target_month
                                FROM overhead_cost AS tbOvc
                                    LEFT JOIN overhead_cost_detail AS tbOvcDetail
                                        ON tbOvc.company_code = tbOvcDetail.company_code
                                        AND tbOvc.project_sys_id = tbOvcDetail.project_sys_id
                                        AND tbOvc.detail_no = tbOvcDetail.detail_no
                                WHERE tbOvc.company_code = '{1}'
                                    AND (tbOvcDetail.target_year > {2} OR (tbOvcDetail.target_year = {2} AND tbOvcDetail.target_month >= {3}))
                                    AND (tbOvcDetail.target_year < {4} OR (tbOvcDetail.target_year = {4} AND tbOvcDetail.target_month <= {5}))
                                GROUP BY tbOvc.company_code
                                    , tbOvc.project_sys_id
                                    , tbOvc.charge_person_id
                                    , tbOvcDetail.target_year
                                    , tbOvcDetail.target_month
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
                sb.AppendFormat("AND tbProject.contract_type_id IN ({0}) ", condition.CONTRACT_TYPE_ID);

            sb.AppendFormat(@"
                    ) AS x PIVOT (
                        SUM(x.actual_cost)
                        FOR [month] IN ({0})
                    ) AS tblActualCost
                    FULL JOIN m_user AS tbUser
                        ON tblActualCost.company_code = tbUser.company_code
                        AND tblActualCost.user_sys_id = tbUser.user_sys_id
                WHERE
                    (tbUser.company_code = '{1}' OR tblActualCost.company_code = '{1}') ", cols, companyCode);

            if (condition.GROUP_ID != null)
                sb.AppendFormat(" AND tblActualCost.group_id = {0} ", condition.GROUP_ID.Value);

            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
                sb.AppendFormat(" AND tblActualCost.location_id IN({0}) ", condition.LOCATION_ID.ToString());

            if (!string.IsNullOrEmpty(condition.USER_NAME))
                sb.AppendFormat(" AND (tbUser.display_name LIKE '{0}' ESCAPE '\\' OR tbUser.user_name_sei LIKE '{0}' ESCAPE '\\' OR tbUser.user_name_mei LIKE '{0}' ESCAPE '\\' OR tbUser.furigana_sei LIKE '{0}' ESCAPE '\\' OR tbUser.furigana_mei LIKE '{0}' ESCAPE '\\' ) ", "%" + replaceWildcardCharacters(condition.USER_NAME) + "%");

            if (!condition.DELETE_FLG)
                sb.Append(" AND tbUser.del_flg = '0' ");

            if (!condition.RETIREMENT_INCLUDE)
                sb.Append(" AND (tbUser.retirement_date IS NULL OR tbUser.retirement_date >= @CurrentDate) ");

            if (condition.IS_PRIVATE)
                sb.AppendFormat(" AND tbUser.user_sys_id = {0} ", currentUser.UserId);

            if (condition.SORT_COL.Value == 1)
                sb.AppendFormat(" ORDER BY group_name {0} ", condition.SORT_TYPE);

            if (condition.SORT_COL.Value == 2 || condition.SORT_COL.Value == 0)
                sb.AppendFormat(" ORDER BY tbUser.display_name {0} ", condition.SORT_TYPE);

            if (condition.SORT_COL.Value == 3)
                sb.AppendFormat(" ORDER BY location_name {0} ", condition.SORT_TYPE);

            var query = new Sql(sb.ToString(), new { CurrentDate = Utility.GetCurrentDateTime() });

            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Get Sales detail list by personal
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<SalesDetailByPersonal> GetSalesDetailByPersonal(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            SalesDetailByPersonalCondition condition)
        {
            var sb = new StringBuilder();

            sb.Append(@"
                SELECT * FROM (SELECT *, (tbData.project_sales - tbData.actual_cost) as profit FROM (SELECT tbProject.project_no, tbProject.project_name, (paymentCost + individual_sales) AS project_sales, ");
            sb.AppendFormat(@"
                (paymentCost + (ISNULL(dbo.RoundNumber('{0}', (tbSales.unit_cost * (tbWorkTime.total_work_day / dbo.GetNumberOfWorkDaysInMonth('{0}', {1}, {2})))), 0))) AS actual_cost ", condition.CompanyCode, condition.SelectedMonth, condition.SelectedYear);
            sb.Append(@"
                FROM ( ");
            sb.Append(@"
                    SELECT ISNULL(tbIndividual.project_sys_id, ISNULL(tbSupplier.project_sys_id, tbOverheadCost.project_sys_id)) AS project_sys_id, ");
            sb.Append(@"
                            ISNULL(tbIndividual.individual_sales, 0) AS individual_sales, ");
            sb.Append(@"
                        (ISNULL(tbSupplier.amount, 0) + ISNULL(tbOverheadCost.amount, 0)) AS paymentCost, ");
            sb.Append(@"
                        ISNULL(tbIndividual.unit_cost, 0) AS unit_cost ");
            sb.Append(@"
                        FROM ( ");
            sb.Append(@"
                            SELECT tbMaDetail.project_sys_id, SUM(tbMaDetail.individual_sales) AS individual_sales, ");
            sb.Append(@"
                                    (SELECT TOP 1 base_unit_cost ");
            sb.Append(@"
                                     FROM unit_price_history ");
            sb.AppendFormat(@"
                                     WHERE company_code = '{0}' ", condition.CompanyCode);
            sb.AppendFormat(@"
                                        AND user_sys_id ={0} ", condition.UserID);
            sb.AppendFormat(@"
                                        AND apply_start_date <= CONVERT(date, CAST({0} AS varchar(4)) + '/'+ RIGHT('0' + CAST({1} AS VARCHAR(2)), 2) + '/01') ", condition.SelectedYear, condition.SelectedMonth);
            sb.AppendFormat(@"
                                        AND del_flg = '0' ", Constant.DeleteFlag.NON_DELETE);
            sb.Append(@"
                                    ORDER BY apply_start_date DESC) AS unit_cost ");
            sb.Append(@"
                                FROM member_assignment_detail AS tbMaDetail ");
            sb.AppendFormat(@"
                                WHERE tbMaDetail.company_code = '{0}' ", condition.CompanyCode);
            sb.AppendFormat(@"
                                AND tbMaDetail.user_sys_id = {0} ", condition.UserID);
            sb.AppendFormat(@"
                                AND tbMaDetail.target_year = {0} ", condition.SelectedYear);
            sb.AppendFormat(@"
                                AND tbMaDetail.target_month = {0} ", condition.SelectedMonth);
            sb.Append(@"
                                GROUP BY tbMaDetail.project_sys_id ");
            sb.Append(@"
                            ) AS tbIndividual FULL JOIN ( ");
            sb.Append(@"
                            SELECT tbSupDetail.project_sys_id, SUM(tbSupDetail.amount) AS amount ");
            sb.Append(@"
                                FROM sales_payment_detail AS tbSupDetail INNER JOIN sales_payment AS tbSup ");
            sb.Append(@"
                                ON tbSupDetail.company_code = tbSup.company_code ");
            sb.Append(@"
                                AND tbSupDetail.project_sys_id = tbSup.project_sys_id ");
            sb.Append(@"
                                AND tbSupDetail.customer_id = tbSup.customer_id ");
            sb.Append(@"
                                AND tbSupDetail.ordering_flg = tbSup.ordering_flg ");
            sb.AppendFormat(@"
                                WHERE tbSupDetail.company_code = '{0}' ", condition.CompanyCode);
            sb.Append(@"
                                AND tbSupDetail.ordering_flg = '2' ");
            sb.Append(@"
                                AND (ISNULL(tbSup.charge_person_id, ");
            sb.Append(@"
                                (SELECT charge_person_id FROM project_info WHERE company_code = tbSup.company_code AND project_sys_id = tbSup.project_sys_id)) ");
            sb.AppendFormat(@"
                                ) = {0}", condition.UserID);
            sb.AppendFormat(@"
                                AND tbSupDetail.target_year = {0} ", condition.SelectedYear);
            sb.AppendFormat(@"
                                AND tbSupDetail.target_month = {0} ", condition.SelectedMonth);
            sb.Append(@"
                                GROUP BY tbSupDetail.project_sys_id ");
            sb.Append(@"
                            ) AS tbSupplier ");
            sb.Append(@"
                            ON tbIndividual.project_sys_id = tbSupplier.project_sys_id ");
            sb.Append(@"
                            FULL JOIN ( ");
            sb.Append(@"
                            SELECT tbOvcDetail.project_sys_id, SUM(tbOvcDetail.amount) AS amount ");
            sb.Append(@"
                                FROM overhead_cost_detail AS tbOvcDetail INNER JOIN overhead_cost AS tbOvc ");
            sb.Append(@"
                                ON tbOvcDetail.company_code = tbOvc.company_code ");
            sb.Append(@"
                                AND tbOvcDetail.project_sys_id = tbOvc.project_sys_id ");
            sb.Append(@"
                                AND tbOvcDetail.detail_no = tbOvc.detail_no ");
            sb.AppendFormat(@"
                                WHERE tbOvcDetail.company_code = '{0}' ", condition.CompanyCode);
            sb.AppendFormat(@"
                                AND tbOvc.charge_person_id = {0} ", condition.UserID);
            sb.AppendFormat(@"
                                AND tbOvcDetail.target_year = {0} ", condition.SelectedYear);
            sb.AppendFormat(@"
                                AND tbOvcDetail.target_month = {0} ", condition.SelectedMonth);
            sb.Append(@"
                                GROUP BY tbOvcDetail.project_sys_id ");
            sb.Append(@"
                            ) AS tbOverheadCost ");
            sb.Append(@"
                            ON tbIndividual.project_sys_id = tbOverheadCost.project_sys_id ");
            sb.Append(@"
                            OR tbSupplier.project_sys_id = tbOverheadCost.project_sys_id ");
            sb.Append(@"
                            ) AS tbSales ");
            sb.Append(@"
                            LEFT JOIN ( ");
            sb.AppendFormat(@"
                            SELECT project_sys_id, dbo.Hour2Day('{0}', SUM(actual_work_time)) AS total_work_day ", condition.CompanyCode);
            sb.Append(@"
                                FROM member_actual_work_detail ");
            sb.AppendFormat(@"
                                WHERE company_code = '{0}' ", condition.CompanyCode);
            sb.AppendFormat(@"
                                AND user_sys_id = {0} ", condition.UserID);
            sb.AppendFormat(@"
                                AND actual_work_year = {0} ", condition.SelectedYear);
            sb.AppendFormat(@"
                                AND actual_work_month = {0} ", condition.SelectedMonth);
            sb.Append(@"
                                GROUP BY project_sys_id ");
            sb.Append(@"
                            ) AS tbWorkTime ");
            sb.Append(@"
                            ON tbSales.project_sys_id = tbWorkTime.project_sys_id ");
            sb.Append(@"
                            INNER JOIN project_info AS tbProject ");
            sb.Append(@"
                            ON tbSales.project_sys_id = tbProject.project_sys_id ");
            sb.AppendFormat(@"
                            AND tbProject.company_code = '{0}' ", condition.CompanyCode);
            sb.Append(@"
                            AND tbProject.del_flg = '0' ");

            if (!string.IsNullOrEmpty(condition.ContractTypeID))
                sb.AppendFormat(@"
                            AND tbProject.contract_type_id IN ({0}) ", condition.ContractTypeID);

            sb.Append(@"
                            AND (SELECT sales_type FROM m_status WHERE company_code = tbProject.company_code AND status_id = tbProject.status_id) = '0' ");
            sb.Append(@"
                            ) tbData ) tbFinal");

            var query = new Sql(sb.ToString());
            var pageInfo = Page<SalesDetailByPersonal>(startItem, int.MaxValue, columns, sortCol, sortDir, query);

            return pageInfo;
        }

        /// <summary>
        /// Get user name
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="userID">User ID</param>
        /// <returns>User name</returns>
        public string GetUserName(string companyCode, int userID)
        {
            var sql = new Sql(@"
                SELECT
                    display_name
                FROM
                    m_user
                WHERE
                    company_code = @company_code
                    AND user_sys_id = @user_sys_id",
                 new
                 {
                     company_code = companyCode,
                     user_sys_id = userID
                 });

            return this._database.FirstOrDefault<string>(sql);
        }

        /// <summary>
        /// Create a fixed data table to be used in SQL query
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private string BuildFixedDataTableByMonth(DateTime startDate, DateTime endDate)
        {
            var sb = new StringBuilder();
            var y = startDate.Year;
            var m = startDate.Month;
            sb.AppendFormat("SELECT '{0}' as target_year, '{1}' as target_month", y, m);
            while (y < endDate.Year || (y == endDate.Year && m <= endDate.Month))
            {
                sb.AppendFormat(@"
                                UNION ALL SELECT '{0}' , '{1}'", y, m);
                m++;
                if (m == 13)
                {
                    m = 1;
                    y++;
                }
            }

            return sb.ToString();
        }
        #endregion
    }
}
