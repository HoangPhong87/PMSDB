#region License
/// <copyright file="PMS11002Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS11002;
using System.Collections.Generic;
using System.Text;
using System;
using ProjectManagementSystem.Models.PMS11003;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Group repository class
    /// </summary>
    public class PMS11003Repository : Repository, IPMS11003Repository
    {
        #region Constructor
        /// <summary>
        /// Database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11003Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS11003Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Public
        /// <summary>
        /// Get list group to use in Search function
        /// </summary>
        /// <param name="group"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IEnumerable<Group> GetListGroupBySearch(string group, string companyCode)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"SELECT * FROM m_group 
                                WHERE company_code = @company_code 
                                AND del_flg = @del_flg AND budget_setting_flg =@budget_setting_flg ");

            if (!string.IsNullOrEmpty(group))
            {
                sb.AppendFormat(" AND group_id IN({0})", group);
            }

            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCode,
                del_flg = Constant.DeleteFlag.NON_DELETE,
                budget_setting_flg = Constant.BudgetSettingFlag.OBJECT
            });

            return this._database.Fetch<Group>(query);
        }
        /// <summary>
        /// Get list Contract type to use in Search function 
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IEnumerable<ContractType> GetListContactTypeBySearch(string contractType, string companyCode)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"SELECT * FROM m_contract_type 
                            WHERE company_code = @company_code 
                            AND del_flg = @del_flg AND budget_setting_flg =@budget_setting_flg ");

            if (!string.IsNullOrEmpty(contractType))
            {
                sb.AppendFormat("AND contract_type_id IN({0})", contractType);
            }

            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCode,
                del_flg = Constant.DeleteFlag.NON_DELETE,
                budget_setting_flg = Constant.BudgetSettingFlag.OBJECT
            });

            return this._database.Fetch<ContractType>(query);
        }
        /// <summary>
        /// Get account closing month
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetAccountClosingMonth(string companyCode)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    SELECT account_closing_month FROM m_company_setting 
                        WHERE company_code = @company_code ");

            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCode
            });
            return this._database.SingleOrDefault<int>(query);
        }
        #region Sale mode
        /// <summary>
        /// Build query for select Sale data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        StringBuilder buildQueryCore(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType = "0")
        {
            var salesType = "0";
            if (checkSalesType == "1")
            {
                salesType = "0,1";
            }
            var yearStart = timeStart.Split('/')[0];
            var monthStart = timeStart.Split('/')[1];
            var yearEnd = timeEnd.Split('/')[0];
            var monthEnd = timeEnd.Split('/')[1];
            var Operation = "AND";
            if(yearEnd != yearStart)
            {
                Operation = "OR";
            }

            var sb = new StringBuilder();
            sb.AppendFormat(@"
                   SELECT tbg.target_year
                           , tbg.target_month
                           , tbg.group_name
                           , tbg.contract_type
                           , tbg.sales_budget
                           , tbg.contract_type_id 
                           , tbg.group_id
                           , ISNULL(toc.overhead_cost,0) + ISNULL(tma.assignment_sales,0) + ISNULL(tsp.payment_sales,0) as total_sales
                           , CASE tbg.sales_budget WHEN 0 THEN -1
                                                   ELSE (((ISNULL(toc.overhead_cost,0) + ISNULL(tma.assignment_sales,0) + ISNULL(tsp.payment_sales,0))/tbg.sales_budget) * 100)
                                               END AS profit
                            --(((ISNULL(toc.overhead_cost,0) + ISNULL(tma.assignment_sales,0) + ISNULL(tsp.payment_sales,0))/tbg.sales_budget) * 100) AS profit
                           , tbg.gr_order
                           , tbg.ct_order
                    FROM(
                    ---***-----------------
                    --Member_assignment (sales)
                      SELECT SUM(maResult.individual_sales) AS assignment_sales
                          , maResult.target_year
                          , maResult.target_month
                          , maResult.group_id
                          , maResult.contract_type_id
                      FROM(
                            SELECT 
                                  (SELECT TOP(1) group_id
                                        FROM enrollment_history
                                        WHERE company_code = '{0}'
                                        AND user_sys_id = mad.user_sys_id
                                        AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                        ORDER BY actual_work_year DESC, actual_work_month DESC
                                  ) group_id
                                  , mad.target_year
                                  , mad.target_month
                                  , mad.individual_sales
                                  , pri.contract_type_id
                            FROM member_assignment_detail AS mad 
                                    INNER JOIN project_info AS pri ON mad.project_sys_id = pri.project_sys_id AND mad.company_code = pri.company_code
                                    INNER JOIN m_status as mst 
                                        ON pri.company_code = mst.company_code
                                        AND pri.status_id = mst.status_id
                                        AND mst.sales_type IN (" + salesType+@")
                            WHERE mad.company_code = '{0}'-- Company Code {0}
                                    AND ((mad.target_month >= {1} AND mad.target_year = {2})-- {1} month start {2} year start
                                    " + Operation + @"(mad.target_month <= {3} AND mad.target_year = {4} ))-- {3} month end {4} year end
                                    AND pri.del_flg = '0'
                       ) AS maResult
                       GROUP BY
                          maResult.target_year
                        , maResult.target_month
                        , maResult.group_id
                        , maResult.contract_type_id
                        ) as tma
                     ---------------------
                     FULL JOIN(

                    ---***-----------------
                    --Sales payment
                    SELECT spResult.group_id
                            , SUM(spResult.amount) as payment_sales
                            , spResult.target_month
                            , spResult.target_year 
                            , spResult.contract_type_id 
                    FROM(
                            SELECT 
                                (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = '{0}'
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) group_id
                                , spd.amount
                                , spd.target_month
                                , spd.target_year 
                                , pri.contract_type_id 
                            FROM sales_payment_detail AS spd
                            INNER JOIN sales_payment AS sp 
                                ON spd.company_code = sp.company_code
                                AND spd.project_sys_id= sp.project_sys_id
                                AND spd.ordering_flg = sp.ordering_flg
                                AND spd.customer_id = sp.customer_id
                            INNER JOIN project_info AS pri ON sp.project_sys_id = pri.project_sys_id AND sp.company_code = pri.company_code
                            INNER JOIN m_status as mst 
                                ON pri.company_code = mst.company_code
                                AND pri.status_id = mst.status_id
                                AND mst.sales_type IN (" + salesType + @")
                            WHERE spd.company_code = '{0}' -- Company Code
                                AND spd.ordering_flg = 2
                                AND ((spd.target_month >= {1} AND spd.target_year = {2}) -- {1} month start {2} year start
                                    " + Operation + @"(spd.target_month <= {3} AND spd.target_year = {4} )) -- month end {4} year end
                                AND pri.del_flg = '0'
                     ) AS spResult
                     GROUP BY
                        spResult.target_month
                        , spResult.target_year
                        , spResult.group_id
                        , spResult.contract_type_id
                     ) as tsp
                     ON tsp.group_id = tma.group_id
                     AND tsp.target_year = tma.target_year
                     AND tsp.target_month = tma.target_month
                     AND tsp.group_id = tma.group_id
                     AND tsp.contract_type_id = tma.contract_type_id
                     FULL JOIN (
                    ---***-----------------
                    --Overhead cost
                    SELECT ocdResult.group_id
                            , SUM(ocdResult.amount) as overhead_cost
                            , ocdResult.target_month
                            , ocdResult.target_year 
                            , ocdResult.contract_type_id 
                    FROM(
                        SELECT 
                            (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = '{0}'
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                            ) group_id
                            , ocd.amount
                            , ocd.target_month
                            , ocd.target_year 
                            , pri.contract_type_id 
                        FROM overhead_cost_detail AS ocd
                        INNER JOIN overhead_cost AS oc 
                            ON ocd.company_code = oc.company_code
                            AND ocd.project_sys_id= oc.project_sys_id
                            AND ocd.detail_no = oc.detail_no
                        INNER JOIN project_info AS pri ON oc.project_sys_id = pri.project_sys_id AND oc.company_code = pri.company_code
                        INNER JOIN m_status as mst 
                            ON pri.company_code = mst.company_code
                            AND pri.status_id = mst.status_id
                            AND mst.sales_type IN (" + salesType + @")
                        WHERE ocd.company_code = '{0}' -- Company Code
                            AND ((ocd.target_month >= {1} AND ocd.target_year = {2}) -- {1} month start {2} year start
                            " + Operation + @"(ocd.target_month <= {3} AND ocd.target_year = {4} )) -- {3} month end {4} year end 
                            AND pri.del_flg = '0'
                     ) AS ocdResult
                     GROUP BY
                        ocdResult.target_month
                        , ocdResult.target_year
                        , ocdResult.group_id
                        , ocdResult.contract_type_id
                     ---------------------
                     ) as toc
                     ON (toc.group_id = tma.group_id OR toc.group_id = tsp.group_id)
                     AND (toc.target_year = tma.target_year OR toc.target_year = tsp.target_year)
                     AND (toc.target_month = tma.target_month OR toc.target_month = tsp.target_month )
                     AND (toc.contract_type_id = tma.contract_type_id OR toc.contract_type_id = tsp.contract_type_id )

                     FULL JOIN( 
                     -----***-----------------
                    ----budget and group, ct
                     SELECT mbg.company_code
                            , mbg.target_year
                            , mbg.target_month
                            , mgr.display_name as group_name
                            , mct.contract_type
                            , mct.budget_setting_flg
                            , mbg.sales_budget
                            , mct.contract_type_id 
                            , mgr.group_id
                            , mgr.display_order as gr_order
                            , mct.display_order as ct_order
                      FROM m_budget AS mbg 
                      INNER JOIN m_group AS mgr ON mbg.group_id = mgr.group_id AND mgr.del_flg = '0'
                      INNER JOIN m_contract_type AS mct ON mbg.contract_type_id = mct.contract_type_id AND mct.del_flg = '0'
                      WHERE mct.budget_setting_flg = '1' AND mbg.sales_budget IS NOT NULL
                        AND mgr.budget_setting_flg = '1'
                        AND mbg.company_code = '{0}' -- Company Code
                         AND ((mbg.target_month >= {1} AND mbg.target_year = {2}) -- {1} month start {2} year start
                         " + Operation + @"(mbg.target_month <= {3} AND mbg.target_year = {4} )) -- {3} month end {4} year end
                     ) AS tbg
                     ON (tbg.contract_type_id = tma.contract_type_id OR tbg.contract_type_id = tsp.contract_type_id OR tbg.contract_type_id = toc.contract_type_id )
                     AND (tbg.target_year = tma.target_year OR tbg.target_year = tsp.target_year OR tbg.target_year = toc.target_year )
                     AND (tbg.target_month = tma.target_month OR tbg.target_month = tsp.target_month OR tbg.target_month = toc.target_month )
                     AND (tbg.group_id = tma.group_id OR tbg.group_id = tsp.group_id OR tbg.group_id = toc.group_id )
                     -------------------------
                     WHERE tbg.sales_budget IS NOT NULL
            
            ", companyCode, monthStart, yearStart, monthEnd, yearEnd);

            if (!string.IsNullOrEmpty(group_id))
            {
                sb.Append(" AND tbg.group_id in (" + group_id + ")");
            }

            if (!string.IsNullOrEmpty(contract_type_id))
            {
                sb.Append("  AND tbg.contract_type_id in (" + contract_type_id + ")");
            }

            return sb;
        }

        /// <summary>
        /// Get list of sale data
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        public IList<dynamic> GetListSaleData(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType = "0")
        {
            StringBuilder sb = buildQueryCore(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType);
            sb.Append("  ORDER BY  tbg.target_year ,tbg.target_month");
            var query = new Sql(sb.ToString());
            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Get list sale data of total group
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        public IList<dynamic> GetListTotalGroup(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType = "0")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT totalGroup.target_year
                               , totalGroup.target_month
                               , totalGroup.group_id
                               , SUM(sales_budget) AS tgrBudget
                               , SUM(total_sales) AS tgrSales
                               , CASE SUM(sales_budget) WHEN 0 THEN -1
                                                        ELSE (ISNULL(SUM(total_sales), 0) / SUM(sales_budget)) * 100
                                 END AS tgrProfit
                         FROM(");
            sb.Append(buildQueryCore(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType));
            sb.Append(@") AS totalGroup
                        GROUP BY totalGroup.target_year
                               , totalGroup.target_month
                               , totalGroup.group_id");
            var query = new Sql(sb.ToString());
            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Get list sale data of total contract type
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        public IList<dynamic> GetListTotalCT(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType = "0")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT totalGroup.target_year
                               , totalGroup.target_month
                               , totalGroup.contract_type_id
                               , SUM(sales_budget) AS tgrBudget
                               , SUM(total_sales) AS tgrSales
                               , CASE SUM(sales_budget) WHEN 0 THEN -1
                                                        ELSE (ISNULL(SUM(total_sales), 0) / SUM(sales_budget)) * 100
                                 END AS tgrProfit
                         FROM(");
            sb.Append(buildQueryCore(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType));
            sb.Append(@") AS totalGroup
                        GROUP BY totalGroup.target_year
                               , totalGroup.target_month
                               , totalGroup.contract_type_id");
            var query = new Sql(sb.ToString());
            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Get list sale data when charge_person_id is null
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        public IList<dynamic> GetListChargePersonNull(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType = "0")
        {
            var salesType = "0";
            if (checkSalesType == "1")
            {
                salesType = "0,1";
            }
            var yearStart = timeStart.Split('/')[0];
            var monthStart = timeStart.Split('/')[1];
            var yearEnd = timeEnd.Split('/')[0];
            var monthEnd = timeEnd.Split('/')[1];
            var Operation = "AND";
            if (yearEnd != yearStart)
            {
                Operation = "OR";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT 
                             spd.target_month
                            , spd.target_year
                            , SUM(spd.amount) AS sp_amount
                            --, pgr.charge_person_id AS pgr_cid
                            , pgr.contract_type_id
                            , pgr.group_id
                        FROM sales_payment AS sp 
                        LEFT JOIN sales_payment_detail AS spd 
                             ON spd.company_code = sp.company_code
                             AND spd.project_sys_id= sp.project_sys_id
                             AND spd.ordering_flg = sp.ordering_flg
                             AND spd.customer_id = sp.customer_id
                        LEFT JOIN 
                        (SELECT
                                distinct pri.* 
                               --pri.project_sys_id
                               --, pri.charge_person_id
                               --, pri.company_code
                               --, pri.contract_type_id
                               , enhs.group_id
                             FROM project_info AS pri
                                INNER JOIN enrollment_history as enhs
                                ON pri.charge_person_id = enhs.user_sys_id
                                INNER JOIN m_status as mst 
                                    ON pri.company_code = mst.company_code
                                    AND pri.status_id = mst.status_id
                                    AND mst.sales_type IN (" + salesType + @")
                            ) AS pgr
                        ON sp.project_sys_id = pgr.project_sys_id AND sp.company_code = pgr.company_code
                        WHERE sp.charge_person_id IS NULL 
                              AND spd.amount IS NOT NULL
                              AND sp.company_code = '{0}'
                              AND pgr.contract_type_id IS NOT NULL --if null this mean sales_type is not found by condition
                              AND pgr.group_id IS NOT NULL --if null this mean sales_type is not found by condition
                              AND ((spd.target_month >= {1} AND spd.target_year = {2}) -- {1} month start {2} year start
                                   " + Operation + @" (spd.target_month <= {3} AND spd.target_year = {4} )) -- {3} month end {4} year end 
                        " , companyCode, monthStart, yearStart, monthEnd, yearEnd);

            if (!string.IsNullOrEmpty(contract_type_id))
            {
                sb.Append(" AND pgr.contract_type_id in (" + contract_type_id + ")");
            }

            if (!string.IsNullOrEmpty(group_id))
            {
                sb.Append(" AND pgr.group_id in (" + group_id + ")");
            }


            sb.Append(@" GROUP BY
                            spd.target_month
                           , spd.target_year
                           --, pgr.charge_person_id
                           , pgr.contract_type_id
                           , pgr.group_id");
            var query = new Sql(sb.ToString());
            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        ///  Get list sale data of all group in year
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <param name="checkSalesType"></param>
        /// <returns></returns>
        public IList<dynamic> GetListTotalAllYearGroup(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType="0")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        SELECT  totalAllYearGr.group_id
                                , SUM(tgrBudget) AS tgrBudget
                                , SUM(tgrSales) AS tgrSales
                                , CASE SUM(tgrBudget) WHEN 0 THEN -1
                                                    ELSE (ISNULL(SUM(tgrSales), 0) / SUM(tgrBudget)) * 100
                                 END AS tgrProfit
                        FROM(
                            SELECT totalGroup.target_year
                                   , totalGroup.target_month
                                   , totalGroup.group_id
                                   , SUM(sales_budget) AS tgrBudget
                                   , SUM(total_sales) AS tgrSales
                                   , (ISNULL(SUM(total_sales), 0) / SUM(sales_budget)) * 100 AS tgrProfit
                             FROM(");
            sb.Append(buildQueryCore(timeStart, timeEnd, companyCode, contract_type_id, group_id, checkSalesType));
            sb.Append(@") AS totalGroup
                        GROUP BY totalGroup.target_year
                               , totalGroup.target_month
                               , totalGroup.group_id
                        ) AS totalAllYearGr

                        GROUP BY totalAllYearGr.group_id");
            var query = new Sql(sb.ToString());
            return this._database.Fetch<dynamic>(query);
        }

        #endregion

        #region Profit mode
        StringBuilder buildQueryCoreProfit(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType = "0")
        {
            var salesType = "0";
            if (checkSalesType == "1")
            {
                salesType = "0,1";
            }
            var yearStart = timeStart.Split('/')[0];
            var monthStart = timeStart.Split('/')[1];
            var yearEnd = timeEnd.Split('/')[0];
            var monthEnd = timeEnd.Split('/')[1];
            var Operation = "AND";
            if (yearEnd != yearStart)
            {
                Operation = "OR";
            }

            var sb = new StringBuilder();
            sb.AppendFormat(@"
                   SELECT tbg.target_year
                           , tbg.target_month
                           , tbg.group_name
                           , tbg.contract_type
                           , tbg.sales_budget
                           , tbg.contract_type_id 
                           , tbg.group_id
                           , ISNULL(toc.overhead_cost,0) + ISNULL(tma.assignment_sales,0) + ISNULL(tsp.payment_sales,0) as total_sales
                           --, (((ISNULL(toc.overhead_cost,0) + ISNULL(tma.assignment_sales,0) + ISNULL(tsp.payment_sales,0))/tbg.sales_budget) * 100) AS profit
                           , tbg.gr_order as group_display_order
                           , tbg.ct_order as contract_type_display_order
                    FROM(
                    ---***-----------------
                    --Member_assignment (sales)
                      SELECT SUM(maResult.individual_sales) AS assignment_sales
                          , maResult.target_year
                          , maResult.target_month
                          , maResult.group_id
                          , maResult.contract_type_id
                      FROM(
                            SELECT 
                                  (SELECT TOP(1) group_id
                                        FROM enrollment_history
                                        WHERE company_code = '{0}'
                                        AND user_sys_id = mad.user_sys_id
                                        AND (actual_work_year < mad.target_year OR (actual_work_year = mad.target_year AND actual_work_month <= mad.target_month))
                                        ORDER BY actual_work_year DESC, actual_work_month DESC
                                  ) group_id
                                  , mad.target_year
                                  , mad.target_month
                                  , mad.individual_sales
                                  , pri.contract_type_id
                            FROM member_assignment_detail AS mad 
                                    INNER JOIN project_info AS pri ON mad.project_sys_id = pri.project_sys_id AND mad.company_code = pri.company_code
                                    INNER JOIN m_status as mst 
                                        ON pri.company_code = mst.company_code
                                        AND pri.status_id = mst.status_id
                                        AND mst.sales_type IN (" + salesType + @")
                            WHERE mad.company_code = '{0}'-- Company Code {0}
                                    AND ((mad.target_month >= {1} AND mad.target_year = {2})-- {1} month start {2} year start
                                    " + Operation + @"(mad.target_month <= {3} AND mad.target_year = {4} ))-- {3} month end {4} year end
                                    AND pri.del_flg = '0'
                       ) AS maResult
                       GROUP BY
                          maResult.target_year
                        , maResult.target_month
                        , maResult.group_id
                        , maResult.contract_type_id
                        ) as tma
                     ---------------------
                     FULL JOIN(

                    ---***-----------------
                    --Sales payment
                    SELECT spResult.group_id
                            , SUM(spResult.amount) as payment_sales
                            , spResult.target_month
                            , spResult.target_year 
                            , spResult.contract_type_id 
                    FROM(
                            SELECT 
                                (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = '{0}'
                                    AND user_sys_id = sp.charge_person_id
                                    AND (actual_work_year < spd.target_year OR (actual_work_year = spd.target_year AND actual_work_month <= spd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                                ) group_id
                                , spd.amount
                                , spd.target_month
                                , spd.target_year 
                                , pri.contract_type_id 
                            FROM sales_payment_detail AS spd
                            INNER JOIN sales_payment AS sp 
                                ON spd.company_code = sp.company_code
                                AND spd.project_sys_id= sp.project_sys_id
                                AND spd.ordering_flg = sp.ordering_flg
                                AND spd.customer_id = sp.customer_id
                            INNER JOIN project_info AS pri ON sp.project_sys_id = pri.project_sys_id AND sp.company_code = pri.company_code
                            INNER JOIN m_status as mst 
                                ON pri.company_code = mst.company_code
                                AND pri.status_id = mst.status_id
                                AND mst.sales_type IN (" + salesType + @")
                            WHERE spd.company_code = '{0}' -- Company Code
                                AND spd.ordering_flg = 2
                                AND ((spd.target_month >= {1} AND spd.target_year = {2}) -- {1} month start {2} year start
                                    " + Operation + @"(spd.target_month <= {3} AND spd.target_year = {4} )) -- month end {4} year end
                                AND pri.del_flg = '0'
                     ) AS spResult
                     GROUP BY
                        spResult.target_month
                        , spResult.target_year
                        , spResult.group_id
                        , spResult.contract_type_id
                     ) as tsp
                     ON tsp.group_id = tma.group_id
                     AND tsp.target_year = tma.target_year
                     AND tsp.target_month = tma.target_month
                     AND tsp.group_id = tma.group_id
                     AND tsp.contract_type_id = tma.contract_type_id
                     FULL JOIN (
                    ---***-----------------
                    --Overhead cost
                    SELECT ocdResult.group_id
                            , SUM(ocdResult.amount) as overhead_cost
                            , ocdResult.target_month
                            , ocdResult.target_year 
                            , ocdResult.contract_type_id 
                    FROM(
                        SELECT 
                            (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = '{0}'
                                    AND user_sys_id = oc.charge_person_id
                                    AND (actual_work_year < ocd.target_year OR (actual_work_year = ocd.target_year AND actual_work_month <= ocd.target_month))
                                    ORDER BY actual_work_year DESC, actual_work_month DESC
                            ) group_id
                            , ocd.amount
                            , ocd.target_month
                            , ocd.target_year 
                            , pri.contract_type_id 
                        FROM overhead_cost_detail AS ocd
                        INNER JOIN overhead_cost AS oc 
                            ON ocd.company_code = oc.company_code
                            AND ocd.project_sys_id= oc.project_sys_id
                            AND ocd.detail_no = oc.detail_no
                        INNER JOIN project_info AS pri ON oc.project_sys_id = pri.project_sys_id AND oc.company_code = pri.company_code
                        INNER JOIN m_status as mst 
                            ON pri.company_code = mst.company_code
                            AND pri.status_id = mst.status_id
                            AND mst.sales_type IN (" + salesType + @")
                        WHERE ocd.company_code = '{0}' -- Company Code
                            AND ((ocd.target_month >= {1} AND ocd.target_year = {2}) -- {1} month start {2} year start
                            " + Operation + @"(ocd.target_month <= {3} AND ocd.target_year = {4} )) -- {3} month end {4} year end 
                            AND pri.del_flg = '0'
                     ) AS ocdResult
                     GROUP BY
                        ocdResult.target_month
                        , ocdResult.target_year
                        , ocdResult.group_id
                        , ocdResult.contract_type_id
                     ---------------------
                     ) as toc
                     ON (toc.group_id = tma.group_id OR toc.group_id = tsp.group_id)
                     AND (toc.target_year = tma.target_year OR toc.target_year = tsp.target_year)
                     AND (toc.target_month = tma.target_month OR toc.target_month = tsp.target_month )
                     AND (toc.contract_type_id = tma.contract_type_id OR toc.contract_type_id = tsp.contract_type_id )

                     FULL JOIN( 
                     -----***-----------------
                    ----budget and group, ct
                     SELECT mbg.company_code
                            , mbg.target_year
                            , mbg.target_month
                            , mgr.display_name as group_name
                            , mct.contract_type
                            , mct.budget_setting_flg
                            , mbg.sales_budget
                            , mbg.profit_budget
                            , mct.contract_type_id 
                            , mgr.group_id
                            , mgr.display_order as gr_order
                            , mct.display_order as ct_order
                      FROM m_budget AS mbg 
                      INNER JOIN m_group AS mgr ON mbg.group_id = mgr.group_id  AND mgr.del_flg = '0'
                      INNER JOIN m_contract_type AS mct ON mbg.contract_type_id = mct.contract_type_id AND mct.del_flg = '0'
                      WHERE mct.budget_setting_flg = '1' AND mbg.profit_budget IS NOT NULL 
                        AND mgr.budget_setting_flg = '1'
                        AND mbg.company_code = '{0}' -- Company Code
                         AND ((mbg.target_month >= {1} AND mbg.target_year = {2}) -- {1} month start {2} year start
                         " + Operation + @"(mbg.target_month <= {3} AND mbg.target_year = {4} )) -- {3} month end {4} year end
                     ) AS tbg
                     ON (tbg.contract_type_id = tma.contract_type_id OR tbg.contract_type_id = tsp.contract_type_id OR tbg.contract_type_id = toc.contract_type_id )
                     AND (tbg.target_year = tma.target_year OR tbg.target_year = tsp.target_year OR tbg.target_year = toc.target_year )
                     AND (tbg.target_month = tma.target_month OR tbg.target_month = tsp.target_month OR tbg.target_month = toc.target_month )
                     AND (tbg.group_id = tma.group_id OR tbg.group_id = tsp.group_id OR tbg.group_id = toc.group_id )
                     -------------------------

                     WHERE tbg.profit_budget IS NOT NULL            
            ", companyCode, monthStart, yearStart, monthEnd, yearEnd);

            if (!string.IsNullOrEmpty(group_id))
            {
                sb.Append(" AND tbg.group_id in (" + group_id + ")");
            }

            if (!string.IsNullOrEmpty(contract_type_id))
            {
                sb.Append("  AND tbg.contract_type_id in (" + contract_type_id + ")");
            }

            return sb;
        }

        /// <summary>
        /// GetListProfitBudget
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public IList<ProfitBudget> GetListProfitBudget(string timeStart, string timeEnd, string companyCode,string contract_type_id,string group_id)
        {
            StringBuilder sb = new StringBuilder();
            var yearStart = timeStart.Split('/')[0];
            var monthStart = timeStart.Split('/')[1];
            var yearEnd = timeEnd.Split('/')[0];
            var monthEnd = timeEnd.Split('/')[1];
            var Operation = "AND";
            if (yearEnd != yearStart)
            {
                Operation = "OR";
            }
            sb.Append(@"
                SELECT mbg.target_year 
                  , mbg.target_month 
                  , mgr.display_name as group_name 
                  , mct.contract_type 
                  , mbg.profit_budget 
                  , mct.contract_type_id 
                  , mgr.group_id 
                    FROM m_budget AS mbg 
                    INNER JOIN m_group AS mgr ON mbg.group_id = mgr.group_id AND mgr.del_flg = '0'
                    INNER JOIN m_contract_type AS mct ON mbg.contract_type_id = mct.contract_type_id AND mct.del_flg = '0'
                    WHERE mct.budget_setting_flg = '1' and mbg.profit_budget IS NOT NULL 
                    AND mgr.budget_setting_flg = '1'
                    AND mbg.company_code = @company_code
                    AND ((mbg.target_month >= @start_month AND mbg.target_year = @start_year)
                    " + Operation + @" (mbg.target_month <= @end_month AND mbg.target_year = @end_year ))
            ");
            if (!string.IsNullOrEmpty(group_id))
            {
                sb.Append(" AND mgr.group_id in (" + group_id + ")");
            }

            if (!string.IsNullOrEmpty(contract_type_id))
            {
                sb.Append("  AND mct.contract_type_id in (" + contract_type_id + ")");
            }
            sb.Append(" ORDER BY mbg.target_year, mbg.target_month");
            Sql sql = new Sql(sb.ToString(),
               new { company_code = companyCode },
               new { start_year = yearStart },
               new { start_month = monthStart },
               new { end_year = yearEnd },
               new { end_month = monthEnd },
               new { group_id = group_id },
               new { contract_type = contract_type_id }
           );
            return this._database.Fetch<ProfitBudget>(sql);
        }

        /// <summary>
        /// GetListCost
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public IList<CostPrice> GetListCost(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType)
        {
            StringBuilder sb = new StringBuilder();
            var yearStart = timeStart.Split('/')[0];
            var monthStart = timeStart.Split('/')[1];
            var yearEnd = timeEnd.Split('/')[0];
            var monthEnd = timeEnd.Split('/')[1];
            var Operation = "AND";
            if (yearEnd != yearStart)
            {
                Operation = "OR";
            }

            sb.Append(@"
                select
                    cost_data.actual_work_year as target_year,
                    cost_data.actual_work_month as target_month,
                    cost_data.group_id,
                    pi.contract_type_id,
                    sum(cost_amount) as cost_price
                    from 
                    (
                    select
                    mawd.company_code,
                    mawd.actual_work_year,
                    mawd.actual_work_month,
                    mawd.project_sys_id,
                    mawd.user_sys_id,
                    (SELECT TOP(1) group_id
                            FROM enrollment_history
                            WHERE company_code = mawd.company_code
                            AND user_sys_id = mawd.user_sys_id
                            AND CONVERT(varchar(4), actual_work_year) + format(actual_work_month,'00') <= CONVERT(varchar(4), mawd.actual_work_year) + format(mawd.actual_work_month,'00') 
                            ORDER BY actual_work_year DESC, actual_work_month DESC) group_id,
                    dbo.RoundNumber(@company_code,(assign.unit_cost * (sum(mawd.actual_work_time) / dbo.GetBusinessHourInMonth(@company_code,mawd.actual_work_year,mawd.actual_work_month)))) as cost_amount
                    from
                    member_actual_work_detail as mawd
                    left outer join
                    (
                        -- アサインされたプロジェクトで設定された原価を算出
                        select
                            mad.company_code,
                            mad.project_sys_id,
                            mad.user_sys_id,
                            mad.target_year,
                            mad.target_month,
                            (SELECT TOP 1 base_unit_cost 
				             FROM unit_price_history 
				             WHERE company_code = mad.company_code
					            AND user_sys_id = mad.user_sys_id
					            AND apply_start_date <= CONVERT(date, CAST(mad.target_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)), 2) + '/01')
					            AND del_flg = '0'
				             ORDER BY apply_start_date DESC) unit_cost
                        from member_assignment_detail as mad  
                    ) as assign
                    on mawd.company_code = assign.company_code
                    and mawd.project_sys_id = assign.project_sys_id
                    and mawd.user_sys_id = assign.user_sys_id
                    and mawd.actual_work_year = assign.target_year
                    and mawd.actual_work_month = assign.target_month
                    where
                        mawd.company_code = @company_code");
            sb.Append(@"
                    and
                    (mawd.actual_work_year = @start_year and mawd.actual_work_month >= @start_month 
                    " + Operation + @" mawd.actual_work_year = @end_year and mawd.actual_work_month <= @end_month)
            ");
            sb.Append(@"
                    group by
                        mawd.company_code,
                        mawd.actual_work_year,
                        mawd.actual_work_month,
                        mawd.project_sys_id,
                        mawd.user_sys_id,
                        assign.unit_cost
                    ) as cost_data
                    inner join project_info as pi
                    on cost_data.company_code = pi.company_code
                    and cost_data.project_sys_id = pi.project_sys_id
                    and pi.del_flg = '0'
                    inner join m_contract_type as mct
                    on pi.company_code = mct.company_code
                    and pi.contract_type_id = mct.contract_type_id
                    and mct.budget_setting_flg = '1'
                    and mct.del_flg = '0'
                    inner join m_status as ms
                    on pi.company_code = ms.company_code
                    and pi.status_id = ms.status_id");

            if(checkSalesType == "0")
            {
                sb.Append(@" and ms.sales_type = '0'");
            }
            else if(checkSalesType == "1")
            {
                sb.Append(@" and (ms.sales_type = '0' or ms.sales_type = '1') ");
            }

            sb.Append(@"
                    where
                    (cost_data.actual_work_year = @start_year and cost_data.actual_work_month >= @start_month 
                    " + Operation + @" cost_data.actual_work_year = @end_year and cost_data.actual_work_month <= @end_month)
            ");
            sb.Append(" and (SELECT budget_setting_flg FROM m_group WHERE group_id = cost_data.group_id) = '1'");
            sb.Append(" and (SELECT del_flg FROM m_group WHERE group_id = cost_data.group_id) = '0'");
            if (!string.IsNullOrEmpty(group_id))
            {
                sb.Append(" and cost_data.group_id in (" + group_id + ")");
            }

            if (!string.IsNullOrEmpty(contract_type_id))
            {
                sb.Append(" and mct.contract_type_id in (" + contract_type_id + ")");
            }

            sb.Append(@"group by
                        cost_data.actual_work_year,
                        cost_data.actual_work_month,
                        cost_data.group_id,
                        pi.contract_type_id
            ");

            Sql sql = new Sql(sb.ToString(),
               new { company_code = companyCode },
               new { start_year = yearStart },
               new { start_month = monthStart },
               new { end_year = yearEnd },
               new { end_month = monthEnd },
               new { group_id = group_id },
               new { contract_type = contract_type_id }
           );
            var result = this._database.Fetch<CostPrice>(sql);
            return result;
        }

        /// <summary>
        /// GetListSaleActual
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public IList<SalesPrice> GetListSaleActual(string timeStart, string timeEnd, string companyCode, string contract_type_id, string group_id, string checkSalesType)
        {
            StringBuilder sb = buildQueryCoreProfit(timeStart, timeEnd, companyCode, contract_type_id, group_id,checkSalesType);
            sb.Append("  ORDER BY  tbg.target_year ,tbg.target_month");
            var query = new Sql(sb.ToString());
            return this._database.Fetch<SalesPrice>(query);
        }

        /// <summary>
        /// Get group name 
        /// </summary>
        /// <param name="companyCd"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public string GetGroupName(string companyCd, int groupId)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"SELECT group_name FROM m_group 
                                WHERE company_code = @company_code 
                                AND group_id = @group_id");

            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCd,
                group_id = groupId
            });

            return this._database.FirstOrDefault<string>(query);
        }

        /// <summary>
        /// Get contract type name 
        /// </summary>
        /// <param name="companyCd"></param>
        /// <param name="contractTypeId"></param>
        /// <returns></returns>
        public string GetContractTypeName(string companyCd, int contractTypeId)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"SELECT contract_type FROM m_contract_type 
                                WHERE company_code = @company_code 
                                AND contract_type_id = @contract_type_id");

            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCd,
                contract_type_id = contractTypeId
            });

            return this._database.FirstOrDefault<string>(query);
        }
        #endregion

        #endregion
    }
}
