#region License
/// <copyright file="PMS09004Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/07/15</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS09004;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Sales by Personal repository class
    /// </summary>
    public class PMS09004Repository : Repository, IPMS09004Repository
    {
        #region Constructor
        /// <summary>
        /// Databse
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS09004Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS09004Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Method
        /// <summary>
        /// Get sales payment list
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<PaymentSalesDetail> GetSalesPaymentDetailList(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition)
        {
            var sql = buidQuerySalesPaymentDetail(condition);

            var pageInfo = Page<PaymentSalesDetail>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get sales payment list export
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IList<PaymentSalesDetail> GetSalesPaymentDetailListExport(Condition condition)
        {
            var sql = buidQuerySalesPaymentDetail(condition);

            return this._database.Fetch<PaymentSalesDetail>(sql);
        }

        private Sql buidQuerySalesPaymentDetail(Condition condition)
        {
            var sb = new StringBuilder();
            var targetTimeStart = Convert.ToDateTime(condition.TARGET_TIME_START + "/01");
            var targetTimeEnd = Convert.ToDateTime(condition.TARGET_TIME_END + "/01");
            sb.AppendFormat(@"
                SELECT
                     data.company_code
                    ,data.target_year
                    ,data.target_month
                    ,sales_company_info.customer_id
                    ,sales_company_info.sales_company
                    ,(SELECT display_name FROM m_customer WHERE company_code = sales_company_info.company_code AND customer_id = sales_company_info.end_user_id) AS end_user_name
                    ,data.project_sys_id
                    ,project_info.project_no
                    , (SELECT tag_name FROM m_customer_tag
                        WHERE company_code = data.company_code AND tag_id = sales_company_info.tag_id) AS tag_name
                    ,project_info.project_name
                    ,project_info.end_date
                    ,project_info.acceptance_date
                    ,project_info.contract_type
                    ,project_info.total_sales
                    ,(SELECT display_name FROM m_user WHERE company_code = project_info.company_code and user_sys_id = project_info.charge_person_id) AS charge_person
					,mg.group_id
					,mg.display_name AS group_name
                    ,user_info.user_sys_id
                    ,user_info.display_name AS [user_name]
                    ,data.sales_amount
                    ,data.unit_cost
                    ,data.plan_man_times
                    ,data.plan_man_days
                    ,data.plan_man_month
                    ,data.plan_cost
                    ,data.actual_work_time
                    ,data.actual_man_month
                    ,data.actual_man_days
                    ,data.actual_cost
                    ,data.payment_company
                    ,data.amount
                    ,user_info.del_flg
                    ,data.payment_del_flg
                    ,project_info.sales_type
                    ,STUFF((SELECT '、' + msc.sub_category FROM target_category tc
                        INNER JOIN m_sub_category msc
                            ON tc.company_code = msc.company_code
                            AND tc.sub_category_id = msc.sub_category_id
                        WHERE tc.company_code = data.company_code
                            AND tc.project_sys_id = data.project_sys_id
                        ORDER BY msc.display_order
                        FOR XML PATH('')),1,1,'') AS sub_category
                FROM (
                    SELECT
                         mad.company_code
                        ,mad.target_year
                        ,mad.target_month
                        ,mad.project_sys_id
                        ,mad.user_sys_id
                        ,'1' as ordering_flg
                        ,mad.individual_sales AS sales_amount
                        ,mad.unit_cost
                        ,mad.plan_man_days * mcs.default_work_time_days AS plan_man_times
                        ,mad.plan_man_days
                        ,cast(mad.plan_man_days as float) / calendar.NumberOfWorkDaysInMonth AS plan_man_month
                        ,dbo.RoundNumber(mad.company_code, mad.unit_cost * (mad.plan_man_days / calendar.NumberOfWorkDaysInMonth)) AS plan_cost
                        ,actual_info.actual_work_time
                        ,cast(actual_info.actual_work_time as float) / mcs.default_work_time_days / calendar.NumberOfWorkDaysInMonth AS actual_man_month
                        ,cast(actual_info.actual_work_time as float) / mcs.default_work_time_days AS actual_man_days
                        ,dbo.RoundNumber(mad.company_code, mad.unit_cost * (cast(actual_info.actual_work_time as float) / (mcs.default_work_time_days * calendar.NumberOfWorkDaysInMonth))) AS actual_cost
                        ,null AS payment_company
                        ,null AS amount
                        ,'0' AS payment_del_flg
                    FROM 
                        (SELECT tbMad.target_year,tbMad.target_month,tbMad.individual_sales,tbMad.plan_man_days,tbMad.company_code,tbMad.project_sys_id,tbMad.user_sys_id, (SELECT TOP 1 base_unit_cost FROM unit_price_history WHERE company_code = tbMad.company_code AND user_sys_id = tbMad.user_sys_id AND apply_start_date <= CONVERT(date, CAST(tbMad.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(tbMad.target_month AS VARCHAR(2)), 2) + '/01') AND del_flg = '0' ORDER BY apply_start_date DESC) AS unit_cost
                            FROM member_assignment_detail tbMad
                            WHERE company_code= {0} AND CAST(target_year AS varchar) + RIGHT('00' + CAST(target_month AS varchar), 2) BETWEEN {1} AND {2}) AS mad
                        LEFT OUTER JOIN (
                            SELECT
                                 mawd.company_code
                                ,mawd.user_sys_id
                                ,mawd.project_sys_id
                                ,mawd.actual_work_year
                                ,mawd.actual_work_month
                                ,sum(mawd.actual_work_time) AS actual_work_time
                            FROM member_actual_work_detail AS mawd
                            WHERE mawd.company_code= {0} AND CAST(mawd.actual_work_year AS varchar) + RIGHT('00' + CAST(mawd.actual_work_month AS varchar), 2) BETWEEN {1} AND {2}
                            GROUP BY
                                 mawd.company_code
                                ,mawd.user_sys_id
                                ,mawd.project_sys_id
                                ,mawd.actual_work_year
                                ,mawd.actual_work_month) AS actual_info
                        ON mad.company_code = actual_info.company_code
                        AND mad.project_sys_id = actual_info.project_sys_id
                        AND mad.target_year = actual_info.actual_work_year
                        AND mad.target_month = actual_info.actual_work_month
                        AND mad.user_sys_id = actual_info.user_sys_id
                        LEFT OUTER JOIN (SELECT company_code, target_year, target_month, dbo.GetNumberOfWorkDaysInMonth(company_code, target_month, target_year) AS NumberOfWorkDaysInMonth 
						    FROM member_assignment_detail
						    WHERE company_code= {0} AND CAST(target_year AS varchar) + RIGHT('00' + CAST(target_month AS varchar), 2) BETWEEN {1} AND {2}
						    GROUP BY company_code, target_year, target_month) AS calendar
						        ON mad.company_code = calendar.company_code
						        AND mad.target_year = calendar.target_year
						        AND mad.target_month = calendar.target_month
						        LEFT OUTER JOIN m_company_setting AS mcs
						        ON mad.company_code = mcs.company_code
                    UNION ALL
                    SELECT
                         sp.company_code
                        ,spd.target_year
                        ,spd.target_month
                        ,sp.project_sys_id
                        ,sp.charge_person_id
                        ,sp.ordering_flg
                        ,ISNULL(spd.amount, 0) AS sales_amount
                        ,null AS unit_cost
                        ,null AS plan_man_times
                        ,null AS plan_man_days
                        ,null AS plan_man_month
                        ,null AS plan_cost
                        ,null AS actual_work_time
                        ,null AS actual_man_month
                        ,null AS actual_man_days
                        ,null AS actual_cost
                        ,mc.display_name AS payment_company
                        ,ISNULL(spd.amount, 0) AS amount
                        ,mc.del_flg AS payment_del_flg
                    FROM sales_payment AS sp
                        INNER JOIN (
                        SELECT company_code,target_year,target_month,amount,project_sys_id,ordering_flg,customer_id
                            FROM sales_payment_detail
                            WHERE company_code = {0} AND ordering_flg = '2' AND CAST(target_year AS varchar) + RIGHT('00' + CAST(target_month AS varchar), 2) BETWEEN {1} AND {2}
                        ) AS spd
                            ON sp.company_code = spd.company_code
                            AND sp.project_sys_id = spd.project_sys_id
                            AND sp.ordering_flg = spd.ordering_flg
                            AND sp.customer_id = spd.customer_id
                            AND sp.ordering_flg = spd.ordering_flg

                        LEFT OUTER JOIN m_customer AS mc
                        ON spd.company_code = mc.company_code
                        AND spd.customer_id = mc.customer_id
                        AND sp.ordering_flg = '2'
                    UNION ALL
                    SELECT
                         oc.company_code
                        ,ocd.target_year
                        ,ocd.target_month
                        ,oc.project_sys_id
                        ,oc.charge_person_id
                        ,'3' AS ordering_flg
                        ,ISNULL(ocd.amount, 0) AS sales_amount
                        ,null AS unit_cost
                        ,null AS plan_man_times
                        ,null AS plan_man_days
                        ,null AS plan_man_month
                        ,null AS plan_cost
                        ,null AS actual_work_time
                        ,null AS actual_man_month
                        ,null AS actual_man_days
                        ,null AS actual_cost
                        ,'【諸経費】' + ISNULL(oc.overhead_cost_detail, '') AS payment_company
                        ,ISNULL(ocd.amount, 0) AS amount
                        ,'0' AS payment_del_flg
                    FROM overhead_cost AS oc
                    INNER JOIN overhead_cost_detail AS ocd
                    ON oc.company_code = ocd.company_code
                    AND oc.project_sys_id = ocd.project_sys_id
                    AND oc.detail_no = ocd.detail_no) AS data

                INNER JOIN (
                    --SA remarks as user_info: list of combine group, location, and user info
                    SELECT
                         mu.company_code
                        ,mu.user_sys_id
                        ,mu.display_name
                        ,mu.user_name_sei
                        ,mu.user_name_mei
                        ,mu.furigana_sei
                        ,mu.furigana_mei
                        ,mu.del_flg
                    FROM m_user AS mu
                    ) AS user_info
                ON data.company_code = user_info.company_code
                AND data.user_sys_id = user_info.user_sys_id

                INNER JOIN (
                   SELECT
                         sp.company_code
                        ,sp.customer_id
                        ,sp.project_sys_id
                        ,sp.end_user_id
                        ,sp.tag_id
                        ,mc.display_name AS sales_company
                    FROM sales_payment AS sp
                    INNER JOIN m_customer AS mc
                    ON sp.company_code = mc.company_code
                    AND sp.customer_id = mc.customer_id
                    AND sp.ordering_flg = '1') AS sales_company_info
                ON data.company_code = sales_company_info.company_code
                AND data.project_sys_id = sales_company_info.project_sys_id

                INNER JOIN (
                    SELECT
                         pi.company_code
                        ,pi.project_sys_id
                        ,pi.project_no
                        ,pi.project_name
                        ,pi.end_date
                        ,pi.acceptance_date
                        ,pi.contract_type_id
                        ,(SELECT contract_type from m_contract_type WHERE contract_type_id = pi.contract_type_id) AS contract_type
                        ,pi.total_sales
                        ,pi.charge_person_id
                        ,pi.del_flg
                        ,ms.sales_type
                    FROM project_info AS pi
                    INNER JOIN m_status AS ms
                    ON pi.company_code = ms.company_code
                    AND pi.status_id = ms.status_id
                    ) AS project_info
                ON data.company_code = project_info.company_code
                AND data.project_sys_id = project_info.project_sys_id

				LEFT OUTER JOIN m_group AS mg
				ON data.company_code = mg.company_code
				AND mg.group_id = (SELECT TOP(1) group_id
					FROM enrollment_history
					WHERE company_code = data.company_code
					AND user_sys_id = data.user_sys_id
					AND (actual_work_year < data.target_year OR (actual_work_year = data.target_year AND actual_work_month <= data.target_month))
					ORDER BY actual_work_year DESC, actual_work_month DESC)

				LEFT OUTER JOIN m_business_location AS ml
				ON data.company_code = ml.company_code
				AND ml.location_id = (SELECT TOP(1) location_id
					FROM enrollment_history
					WHERE company_code = data.company_code
					AND user_sys_id = data.user_sys_id
					AND (actual_work_year < data.target_year OR (actual_work_year = data.target_year AND actual_work_month <= data.target_month))
					ORDER BY actual_work_year DESC, actual_work_month DESC)
                WHERE
                    data.company_code = {0}
                    AND CAST(data.target_year AS varchar) + RIGHT('00' + CAST(data.target_month AS varchar), 2) BETWEEN {1} AND {2}
                    AND user_info.del_flg = '0' AND data.payment_del_flg = '0' ",
                "'" + condition.COMPANY_CODE + "'",
                "'" + condition.TARGET_TIME_START.Remove(4, 1) + "'",
                "'" + condition.TARGET_TIME_END.Remove(4, 1) + "'"
                );

            if (condition.ESTIMATE_DISPLAY)
            {
                sb.AppendFormat(@"
                    AND (project_info.sales_type = '0' OR project_info.sales_type = '1')
                ");
            }
            else
            {
                sb.AppendFormat(@"
                    AND project_info.sales_type = '0'
                ");
            }

            if (condition.CUSTOMER_ID != null)
            {
                sb.AppendFormat(@"
                    AND sales_company_info.customer_id = {0}", condition.CUSTOMER_ID.ToString());
            }

            if (condition.GROUP_ID != null)
            {
                sb.AppendFormat(@"
                    AND mg.group_id = {0}", condition.GROUP_ID.ToString());
            }

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID))
            {
                sb.AppendFormat(@" 
                    AND contract_type_id IN ({0}) ", condition.CONTRACT_TYPE_ID);
            }


            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(@" 
                    AND (ml.location_id IN ({0}))", condition.LOCATION_ID);
            }

            if (!string.IsNullOrEmpty(condition.USER_NAME))
            {
                sb.AppendFormat(" AND (user_info.display_name LIKE '{0}' ESCAPE '\\' OR user_info.user_name_sei LIKE '{0}' ESCAPE '\\' OR user_info.user_name_mei LIKE '{0}' ESCAPE '\\' OR user_info.furigana_sei LIKE '{0}' ESCAPE '\\' OR user_info.furigana_mei LIKE '{0}' ESCAPE '\\' ) ", "%" + replaceWildcardCharacters(condition.USER_NAME) + "%");
            }

            if (condition.PLAN_DISPLAY)
            {
                //#70219
                if (condition.PLANNED_MEMBER_INCLUDE)
                {
                    sb.Append(@" AND (data.sales_amount > 0 OR data.actual_cost > 0 OR data.plan_cost > 0 OR data.plan_man_times > 0) ");
                }
                else
                {
                    sb.Append(@" AND (data.sales_amount > 0 OR data.actual_cost > 0 OR data.plan_cost > 0 ) ");
                }
            }
            else
            {
                //#70219
                if (condition.PLANNED_MEMBER_INCLUDE)
                {
                    sb.Append(@" AND (data.sales_amount > 0 OR data.actual_cost > 0 OR data.plan_man_times > 0) ");
                }
                else
                {
                    sb.Append(@" 
                    AND (data.sales_amount > 0 OR data.actual_cost > 0) ");
                }
            }

            if (!condition.DELETE_FLG)
            {
                sb.Append("AND project_info.del_flg = '0'");
            }

            sb.Append(@"
                ORDER BY
                     data.target_year
                    ,data.target_month
                    ,sales_company_info.sales_company
                    ,data.project_sys_id
                    ,mg.group_name
                    ,user_info.display_name
                    ,data.ordering_flg");

            return new Sql(sb.ToString());
        }

        #endregion
    }
}
