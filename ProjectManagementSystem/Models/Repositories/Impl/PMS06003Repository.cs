#region License
/// <copyright file="PMS06003Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>TrungNT</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS06003;


namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Repository for PMS06003
    /// </summary>
    public class PMS06003Repository : Repository, IPMS06003Repository
    {
        private Database _database;

        /// <summary>
        /// Constructor 
        /// </summary>
        public PMS06003Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco</param>
        public PMS06003Repository(PMSDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// getStringColumnNames
        /// </summary>
        /// <param name="sStartYearMonth"></param>
        /// <param name="sEndYearMonth"></param>
        /// <returns></returns>
        private string getStringColumnNames(string sStartYearMonth, string sEndYearMonth)
        {
            int iStartY = Convert.ToInt32(sStartYearMonth.Substring(0, 4));
            int iStartM = Convert.ToInt32(sStartYearMonth.Substring(4));
            int iEndY = Convert.ToInt32(sEndYearMonth.Substring(0, 4));
            int iEndM = Convert.ToInt32(sEndYearMonth.Substring(4));

            string cols = string.Empty;
            for (int y = iStartY, m = iStartM, i = 1; y < iEndY || (y == iEndY && m <= iEndM); m++, i++)
            {
                if (m == 13)
                {
                    m = 1;
                    y++;
                }
                cols += string.Format("[{0,4:D4}/{1,2:D2}],", y, m);
            }
            cols = cols.Substring(0, cols.Length - 1);
            return cols;
        }


        /// <summary>
        /// Search page plan by Project
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        public PageInfo<dynamic> SearchPlanByProject(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, ProjectCondition condition, string companycode)
        {
            Sql sql = buildSelectPlanQueryByProject(condition, companycode, false, 0, string.Empty);
            var pageInfo = Page<dynamic>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Search list plan by Project
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        public IList<dynamic> GetListPlanByProject(ProjectCondition condition, string companycode, int sort_colum, string sort_type)
        {
            Sql sql = buildSelectPlanQueryByProject(condition, companycode, true, sort_colum, sort_type);
            var pageInfo = this._database.Fetch<dynamic>(sql);

            return pageInfo;
        }

        /// <summary>
        /// build query for select plan
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        private Sql buildSelectPlanQueryByProject(ProjectCondition condition, string companycode, bool isExport, int sort_colum, string sort_type)
        {
            string sStartYearMonth = condition.START_DATE.Length == 7 ? condition.START_DATE.Replace("/", string.Empty) : condition.START_DATE.Replace("/", "0");
            string sEndYearMonth = condition.END_DATE.Length == 7 ? condition.END_DATE.Replace("/", string.Empty) : condition.END_DATE.Replace("/", "0");
            string cols = getStringColumnNames(sStartYearMonth, sEndYearMonth);
            StringBuilder query = new StringBuilder();

            query.Append(@"
                SELECT  
                    piv.*	
                FROM
                (
                    SELECT
                        pi.project_sys_id,
                        pi.del_flg,
                        pi.company_code,
                        mr.display_order AS rank_order,
                        us.display_name as person_in_charge,
                        pi.project_name,
                        mr.rank,
                        pi.total_sales,
                        cast( mad.target_year as VARCHAR(4)) + '/' + RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)),2) AS [Month],  ");
            switch (condition.EFF_TYPE)
            {
                case 1:
                    query.AppendFormat(" dbo.Day2Hour(@company_code, plan_man_days) as plan_man_days ");
                    break;
                case 2:
                    query.Append(" plan_man_days ");
                    break;
                case 3:
                    query.AppendFormat(" dbo.Day2Month(@company_code, mad.target_month, mad.target_year,plan_man_days) as plan_man_days ");
                    break;
            }
            query.Append(@"
                    FROM project_info pi
                        INNER JOIN m_status 
                            ON pi.company_code = m_status.company_code
                            AND pi.status_id = m_status.status_id
                        INNER JOIN m_user us 
                            ON us.company_code = pi.company_code
                            AND us.user_sys_id = pi.charge_person_id
                        LEFT JOIN m_rank mr 
                            ON pi.company_code = mr.company_code
                            AND pi.rank_id = mr.rank_id
                        LEFT JOIN member_assignment_detail mad
                            ON pi.company_code = mad.company_code
                            AND pi.project_sys_id = mad.project_sys_id ");
            if (condition.CUSTOMER_ID != null)
            {
                query.Append(@" LEFT JOIN sales_payment sp ON mad.project_sys_id = sp.project_sys_id ");
            }
            query.Append(@"
                    WHERE 
                        (CAST( mad.target_year AS VARCHAR(4)) + RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)),2))
                            BETWEEN @startDate AND @endDate
                            AND pi.company_code = @company_code ");
            if(!string.IsNullOrEmpty(condition.STATUS_ID))
            {
                query.Append(" AND pi.status_id in ("+ condition.STATUS_ID + ") ");
            }
                            
            if (condition.PROJECT_NAME != null)
                query.AppendFormat(@" AND pi.project_name LIKE '%{0}%' ESCAPE '\' ", replaceWildcardCharacters(condition.PROJECT_NAME));

            if (condition.CUSTOMER_ID != null)
            {
                query.Append(@" AND sp.ordering_flg = 1
                                AND sp.customer_id = @customer_id ");
                if (condition.TAG_ID != null)
                {
                    query.Append(@" AND sp.tag_id = @tag_id ");
                }
            }

            if (condition.GROUP_ID != null)
                query.Append(@" AND us.group_id = @group_id ");
            
            if (!condition.DELETE_FLG)
            {
                query.Append(" AND pi.del_flg = @del_flg ");
            }

            if (!condition.INACTIVE_FLG)
            {
                query.Append(" AND m_status.sales_type <> '2' ");
            }

            query.Append(@"
                )x
                PIVOT
                (
                    SUM(plan_man_days) for Month in (" + cols + @")
                ) piv
            ");

            string sort_column_name = "project_sys_id";
            if (isExport)
            {
                switch(sort_colum)
                {
                    case 1:
                        sort_column_name = "person_in_charge";
                        break;
                    case 2:
                        sort_column_name = "project_name";
                        break;
                    case 3:
                        sort_column_name = "rank";
                        break;
                    case 4:
                        sort_column_name = "total_sales";
                        break;
                    default:
                        sort_column_name = "project_sys_id";
                        break;
                }
                query.Append(@" ORDER BY " + sort_column_name + " " + sort_type);
            }

            Sql sql = new Sql(query.ToString(),
                new { startDate = sStartYearMonth },
                new { endDate = sEndYearMonth },
                new { company_code = companycode },
                new { project_name = condition.PROJECT_NAME },
                new { customer_id = condition.CUSTOMER_ID },
                new { tag_id = condition.TAG_ID },
                new { group_id = condition.GROUP_ID },
                new { del_flg = Constant.DeleteFlag.NON_DELETE }
            ); 
            return sql;
        }


        /// <summary>
        /// Search By User
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        public PageInfo<dynamic> SearchAssignmentByUser(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            UserCondition condition,
            string companycode)
        {
            Sql sql = buildSelectQueryByUser(condition, companycode, false, 0, string.Empty);
            var pageInfo = Page<dynamic>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);
            return pageInfo;
        }

        /// <summary>
        /// GetListAssignmentByUser
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns>List of assignment</returns>
        public IList<dynamic> GetListAssignmentByUser(UserCondition condition, string companycode, int sort_colum, string sort_type)
        {
            var sql = buildSelectQueryByUser(condition, companycode, true, sort_colum, sort_type);
            return this._database.Fetch<dynamic>(sql);
        }

        /// <summary>
        /// Search Individual Sales ByUser
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <param name="sortCol"></param>
        /// <param name="sortDir"></param>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        public IList<dynamic> SearchPriceInforByUser(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            UserCondition condition,
            string companycode)
        {
            Sql sql = SearchPriceInforByUser(condition, companycode, 0, string.Empty,startItem,itemsPerPage,columns,sortCol,sortDir);
            var pageInfo = _database.Fetch<dynamic>(sql);
            return pageInfo;
        }


        /// <summary>
        /// GetListAssignmentByUser
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns>List of assignment</returns>
        public IList<dynamic> GetListPriceInfor(UserCondition condition, string companycode, int sort_colum, string sort_type)
        {
            var sql = SearchAllPriceInforByUser(condition, companycode, sort_colum, sort_type);
            return this._database.Fetch<dynamic>(sql);
        }

        /// <summary>
        /// Build search query by Project
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        private Sql buildSelectQueryByProject(ProjectCondition condition, string companycode, bool isExport = false)
        {
            string sStartYearMonth = condition.START_DATE.Length == 7 ? condition.START_DATE.Replace("/", string.Empty) : condition.START_DATE.Replace("/", "0");
            string sEndYearMonth = condition.END_DATE.Length == 7 ? condition.END_DATE.Replace("/", string.Empty) : condition.END_DATE.Replace("/", "0");
            string cols = getStringColumnNames(sStartYearMonth, sEndYearMonth);
            StringBuilder query = new StringBuilder();

            query.Append(@"
            SELECT
                *
            FROM
                (
                    SELECT  
                        piv.*
                    FROM
                    (
                        SELECT pi.project_sys_id, ");
            if(!isExport)
            {
                query.Append(@" 
                            pi.company_code, ");
            }                

            query.Append(@" pi.project_no,
                            pi.project_name,
							mr.rank,
                            pi.total_sales,
                            cast( mad.target_year as VARCHAR(4)) + '/' + RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)),2) AS [Month], ");
            switch (condition.EFF_TYPE)
            {
                case 1:
                    query.AppendFormat(" dbo.Day2Hour(@companycode, plan_man_days) as plan_man_days ");                    
                    break;
                case 2:
                    query.Append(" plan_man_days ");
                    break;
                case 3:
                    query.AppendFormat(" dbo.Day2Month(@companycode, mad.target_month, mad.target_year,plan_man_days) as plan_man_days ");
                    break;
            }
            query.Append(" ,mr.display_order AS rank_order ");

            query.Append(@"
                            FROM 
					        project_info pi 
							LEFT JOIN m_rank mr ON pi.rank_id = mr.rank_id
							LEFT JOIN member_assignment_detail mad ON pi.project_sys_id = mad.project_sys_id ");
            if (condition.CUSTOMER_ID != null)
            { 
                query.Append(@"
							LEFT JOIN sales_payment sp ON mad.project_sys_id = sp.project_sys_id ");
            }

            query.Append(@"
                        WHERE 
                            (CAST( mad.target_year AS VARCHAR(4)) + RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)),2))
                                BETWEEN @startDate AND @endDate
                                AND pi.company_code = @companycode ");
            if (condition.PROJECT_NAME != null)
                query.AppendFormat(@" AND pi.project_name LIKE '%{0}%' ESCAPE '\' ", replaceWildcardCharacters(condition.PROJECT_NAME));

            if (condition.CUSTOMER_ID != null)
                query.Append(@" AND sp.ordering_flg = 1
                                AND sp.customer_id = @customer_id ");

            query.Append(@" 
                )x
                PIVOT
                (
                    SUM(plan_man_days) for Month in (" + cols + @")
                ) piv 
            ) tblProject
                ");

            Sql sql = new Sql(query.ToString(),
                new { startDate = sStartYearMonth },
                new { endDate = sEndYearMonth },
                new { companycode = companycode },
                new { project_name = condition.PROJECT_NAME },
                new { customer_id = condition.CUSTOMER_ID }
                );

            return sql;
        }

        /// <summary>
        /// build Query Select Top Price Information
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        private string buildQuerySelectTopPriceInfor(string cols)
        {
            var arrCols = cols.Split(',');
            string selectCols = "";
            for (int i = 0; i < arrCols.Length; i++)
            {
                var each = arrCols[i].Replace("[", string.Empty).Replace("]", string.Empty).Split('/');
                var selectUnitPriceQuery = @"CAST((SELECT TOP(1) base_unit_cost
									               FROM unit_price_history
									               WHERE company_code = piv.company_code
									                     AND user_sys_id = piv.user_sys_id
									                     AND (YEAR(apply_start_date) < " + each[0] + @" OR (YEAR(apply_start_date) = " + each[0] + @" AND MONTH(apply_start_date) <= " + each[1] + @"))
									               ORDER BY apply_start_date DESC
								                  ) AS varchar(10) )";
                selectCols += @",
                                   (CASE WHEN 
                                        " + selectUnitPriceQuery + @" IS NULL
								   THEN '-' 
								   ELSE 
                                        " + selectUnitPriceQuery + @"
								   END)
                                    +'/'+
                                    (CASE WHEN " + arrCols[i] + @" IS NULL
								    THEN '-'
								    ELSE CAST(" + arrCols[i] + @" AS varchar(10)) END)
                                    AS '" + each[0] + @"/" + each[1] + @"'
                                ";
            }
            return selectCols;
        }

        /// <summary>
        /// build Query Select Individual Sales
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private StringBuilder buildQuerySelectIndividualSales(UserCondition condition)
        {
            StringBuilder selectIndividualSales = new StringBuilder(); //Get Individual Sales Data Query
            selectIndividualSales.AppendFormat(@"
                        SELECT us.user_sys_id
                            , us.company_code
                            ,gr.display_name as group_name
                            ,us.display_name as user_name
                            , mad.target_year
						    , mad.target_month
                            , SUM(CASE WHEN m_status.sales_type = '2'
                                THEN 0
                                ELSE individual_sales END) AS individual_sales
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
                        LEFT JOIN m_group gr
                            ON us.company_code = gr.company_code
                            AND us.group_id = gr.group_id
                        LEFT JOIN m_business_location location
							ON us.company_code = location.company_code
							AND us.location_id = location.location_id
                    WHERE 
                        (CAST( mad.target_year AS VARCHAR(4)) + RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)),2))
                            BETWEEN @startDate AND @endDate
                            AND us.company_code = @companycode 
                            AND pi.del_flg = '0'
            ");
            if (condition.USER_NAME != null)
                selectIndividualSales.AppendFormat(@" AND (
                                        us.display_name LIKE '%{0}%'  ESCAPE '\' 
                                        OR us.user_name_sei LIKE '%{0}%' ESCAPE '\' 
                                        OR us.user_name_mei LIKE '%{0}%' ESCAPE '\' 
                                        OR us.furigana_sei LIKE '%{0}%' ESCAPE '\' 
                                        OR us.furigana_mei LIKE '%{0}%' ESCAPE '\' 
                                    )", replaceWildcardCharacters(condition.USER_NAME));

            if (condition.GROUP_ID != null)
                selectIndividualSales.Append(@" AND gr.group_id = @group_id ");

            if (!string.IsNullOrEmpty(condition.STATUS_ID))
            {
                selectIndividualSales.Append(" AND pi.status_id in (" + condition.STATUS_ID + ") ");
            }

            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                selectIndividualSales.Append(" AND location.location_id in (" + condition.LOCATION_ID + ") ");
            }

            if (!condition.RETIRED_INCLUDE)
                selectIndividualSales.Append(@" AND (us.retirement_date IS NULL OR us.retirement_date >= @CurrentDate)");

            selectIndividualSales.Append(@" GROUP BY us.user_sys_id
                                                    , us.company_code
                                                    , gr.display_name
                                                    , us.display_name
                                                    , mad.target_year
						                            , mad.target_month");

            return selectIndividualSales;
        }


        /// <summary>
        /// build Query Select Sales Payment
        /// </summary>
        /// <returns></returns>
        private StringBuilder buildQuerySelectSalesPayment()
        {
            StringBuilder selectSelectSalesPayment = new StringBuilder(); //Get Individual Sales Data Query
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
							) AS spCounting
							GROUP BY
								spCounting.company_code
								, spCounting.charge_person_id
								, spCounting.target_year
								, spCounting.target_month
							--get sales payment-- end");

            return selectSelectSalesPayment;
        }

        /// <summary>
        /// build Query Select Over Head Cost
        /// </summary>
        /// <returns></returns>
        private StringBuilder buildQuerySelectOverHeadCost()
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
							) AS ocCounting
							GROUP BY
								ocCounting.company_code
								, ocCounting.charge_person_id
								, ocCounting.target_year
								, ocCounting.target_month
								--get overhead cost-- end");

            return selectSelectOverHeadCost;
        }

        /// <summary>
        /// Search Individual Sales ByUser
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        private Sql SearchPriceInforByUser(UserCondition condition, string companycode, int sort_column, string sort_type,int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir)
        {
            string sStartYearMonth = condition.START_DATE.Length == 7 ? condition.START_DATE.Replace("/", string.Empty) : condition.START_DATE.Replace("/", "0");
            string sEndYearMonth = condition.END_DATE.Length == 7 ? condition.END_DATE.Replace("/", string.Empty) : condition.END_DATE.Replace("/", "0");
            string cols = getStringColumnNames(sStartYearMonth, sEndYearMonth);

            string orderBy = string.Empty;

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

            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"SELECT * FROM (
                SELECT ROW_NUMBER() OVER (ORDER BY {0} {1}) peta_rn, tbData.*", orderBy, sortDir);

            query.Append(@"FROM (
                SELECT piv.user_sys_id");
            query.Append(@",piv.company_code");

            query.AppendFormat(@",piv.group_name,piv.[user_name]" + buildQuerySelectTopPriceInfor(cols) 
                        + @"
                    FROM
                    (
                        SELECT 
							ins.user_sys_id
							, ins.company_code
							, ins.group_name
							, ins.user_name
							, cast( ins.target_year as VARCHAR(4)) + '/' + RIGHT('0' + CAST(ins.target_month AS VARCHAR(2)),2) AS [Month]
							, ISNULL(ins.individual_sales,0) + ISNULL(sp.amount,0) + ISNULL(oc.amount,0) AS individual_sales
						FROM
						(
                            " + buildQuerySelectIndividualSales(condition) + @"
                            ) AS ins
						    LEFT JOIN(
                            " + buildQuerySelectSalesPayment() + @"
                            ) AS sp
						ON ins.company_code = sp.company_code
							AND ins.target_month = sp.target_month
							AND ins.target_year = sp.target_year
							AND ins.user_sys_id = sp.charge_person_id

                            LEFT JOIN(
                            " + buildQuerySelectOverHeadCost() + @"
                            ) AS oc
						ON ins.company_code = oc.company_code
							AND ins.target_month = oc.target_month
							AND ins.target_year = oc.target_year
							AND ins.user_sys_id = oc.charge_person_id
                    )x
                    PIVOT
                    (
                        SUM(individual_sales) for Month in (" + cols + @")
                    ) piv ) AS tbData) AS tbPaging
                    WHERE peta_rn > {0} AND peta_rn <= {1} 
            ", startItem, itemsPerPage + startItem);
            Sql sql = new Sql(query.ToString(),
               new { startDate = sStartYearMonth },
               new { endDate = sEndYearMonth },
               new { companycode = companycode },
               new { display_name = condition.USER_NAME },
               new { group_id = condition.GROUP_ID },
               new { CurrentDate = Utility.GetCurrentDateTime() }
               );
            return sql;
        }

        /// <summary>
        /// Search All Individual Sales ByUser
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        private Sql SearchAllPriceInforByUser(UserCondition condition, string companycode, int sort_column, string sort_type)
        {
            string sStartYearMonth = condition.START_DATE.Length == 7 ? condition.START_DATE.Replace("/", string.Empty) : condition.START_DATE.Replace("/", "0");
            string sEndYearMonth = condition.END_DATE.Length == 7 ? condition.END_DATE.Replace("/", string.Empty) : condition.END_DATE.Replace("/", "0");
            string cols = getStringColumnNames(sStartYearMonth, sEndYearMonth);

            StringBuilder query = new StringBuilder();
            string sort_column_name = string.Empty;
            switch (sort_column)
            {
                case 3:
                    sort_column_name = "group_name";
                    break;
                case 4:
                    sort_column_name = "user_name";
                    break;
                default:
                    sort_column_name = "user_sys_id";
                    break;
            }
            query.Append(@"	SELECT 
                ROW_NUMBER() OVER (order by " + sort_column_name + " " + sort_type + ") [No], ");


            query.Append(@"piv.user_sys_id");
            query.Append(@",piv.group_name,piv.[user_name]" + buildQuerySelectTopPriceInfor(cols)
                        + @"
                    FROM
                    (
                        SELECT 
							ins.user_sys_id
							, ins.company_code
							, ins.group_name
							, ins.user_name
							, cast( ins.target_year as VARCHAR(4)) + '/' + RIGHT('0' + CAST(ins.target_month AS VARCHAR(2)),2) AS [Month]
							, ISNULL(ins.individual_sales,0) + ISNULL(sp.amount,0) + ISNULL(oc.amount,0) AS individual_sales
						FROM
						(
                            " + buildQuerySelectIndividualSales(condition) + @"
                            ) AS ins
						    LEFT JOIN(
                            " + buildQuerySelectSalesPayment() + @"
                            ) AS sp
						ON ins.company_code = sp.company_code
							AND ins.target_month = sp.target_month
							AND ins.target_year = sp.target_year
							AND ins.user_sys_id = sp.charge_person_id

                            LEFT JOIN(
                            " + buildQuerySelectOverHeadCost() + @"
                            ) AS oc
						ON ins.company_code = oc.company_code
							AND ins.target_month = oc.target_month
							AND ins.target_year = oc.target_year
							AND ins.user_sys_id = oc.charge_person_id
                    )x
                    PIVOT
                    (
                        SUM(individual_sales) for Month in (" + cols + @")
                    ) piv
            ");
            Sql sql = new Sql(query.ToString(),
               new { startDate = sStartYearMonth },
               new { endDate = sEndYearMonth },
               new { companycode = companycode },
               new { display_name = condition.USER_NAME },
               new { group_id = condition.GROUP_ID },
               new { CurrentDate = Utility.GetCurrentDateTime() }
               );
            return sql;
        }

        /// <summary>
        /// Build search query by User
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="companycode"></param>
        /// <returns></returns>
        private Sql buildSelectQueryByUser(UserCondition condition, string companycode, bool isExport, int sort_column, string sort_type)
        {
            string sStartYearMonth = condition.START_DATE.Length == 7 ? condition.START_DATE.Replace("/", string.Empty) : condition.START_DATE.Replace("/", "0");
            string sEndYearMonth = condition.END_DATE.Length == 7 ? condition.END_DATE.Replace("/", string.Empty) : condition.END_DATE.Replace("/", "0");
            string cols = getStringColumnNames(sStartYearMonth, sEndYearMonth);
            StringBuilder query = new StringBuilder();

            query.Append(@"
                SELECT  ");
            if(isExport)
            {
                if (isExport)
                {
                    string sort_column_name = string.Empty;
                    switch (sort_column)
                    {
                        case 3:
                            sort_column_name = "group_name";
                            break;
                        case 4:
                            sort_column_name = "user_name";
                            break;
                        default:
                            sort_column_name = "user_sys_id";
                            break;
                    }
                    query.Append(@"	
                        ROW_NUMBER() OVER (order by " + sort_column_name + " " + sort_type + ") [No], ");
                }
            }

            query.Append(@" 
                        piv.* 
                    FROM
                    (
                        SELECT us.user_sys_id, ");
            if (!isExport)
            { 
                query.Append(@" us.company_code, ");
            }

            query.Append(@" gr.display_name as group_name,
                            us.display_name as user_name,
                            cast( mad.target_year as VARCHAR(4)) + '/' + RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)),2) AS [Month],
                            CASE WHEN m_status.sales_type = '2'
                                THEN 0
                                ELSE ");
            switch (condition.EFF_TYPE)
            {
                case 1:
                    query.AppendFormat(" dbo.Day2Hour(@companycode, plan_man_days)");
                    break;
                case 2:
                    query.Append(" plan_man_days ");
                    break;
                case 3:
                    query.AppendFormat(" dbo.Day2Month(@companycode, mad.target_month, mad.target_year,plan_man_days)");
                    break;
            }
            query.Append(@" END AS plan_man_days
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
                        LEFT JOIN m_group gr
                            ON us.company_code = gr.company_code
                            AND us.group_id = gr.group_id
                        LEFT JOIN m_business_location location
							ON us.company_code = location.company_code
							AND us.location_id = location.location_id
                    WHERE 
                        (CAST( mad.target_year AS VARCHAR(4)) + RIGHT('0' + CAST(mad.target_month AS VARCHAR(2)),2))
                            BETWEEN @startDate AND @endDate
                            AND us.company_code = @companycode 
                            AND pi.del_flg = '0'
            ");
            if (condition.USER_NAME != null)
                query.AppendFormat(@" AND (
                                        us.display_name LIKE '%{0}%'  ESCAPE '\' 
                                        OR us.user_name_sei LIKE '%{0}%' ESCAPE '\' 
                                        OR us.user_name_mei LIKE '%{0}%' ESCAPE '\' 
                                        OR us.furigana_sei LIKE '%{0}%' ESCAPE '\' 
                                        OR us.furigana_mei LIKE '%{0}%' ESCAPE '\' 
                                    )", replaceWildcardCharacters(condition.USER_NAME));

            if (condition.GROUP_ID != null)
                query.Append(@" AND gr.group_id = @group_id ");

            if (!string.IsNullOrEmpty(condition.STATUS_ID))
            {
                query.Append(" AND pi.status_id in (" + condition.STATUS_ID + ") ");
            }

            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                query.Append(" AND location.location_id in (" + condition.LOCATION_ID + ") ");
            }

            if (!condition.RETIRED_INCLUDE)
                query.Append(@" AND (us.retirement_date IS NULL OR us.retirement_date >= @CurrentDate)");

            query.Append(@" 
                )x
                PIVOT
                (
                    SUM(plan_man_days) for Month in (" + cols + @")
                ) piv 
            ");
            Sql sql = new Sql(query.ToString(),
               new { startDate = sStartYearMonth },
               new { endDate = sEndYearMonth },
               new { companycode = companycode },
               new { display_name = condition.USER_NAME },
               new { group_id = condition.GROUP_ID },
               new { CurrentDate = Utility.GetCurrentDateTime() }
               );
            return sql;
        }
    }
}