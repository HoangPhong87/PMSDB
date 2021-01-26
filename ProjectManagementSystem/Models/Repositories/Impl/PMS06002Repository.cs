#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="PMS06002Repository.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>04-06-2014</createdDate>
// //<summary>
// // TODO: Update summary.
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using PetaPoco;
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06002;

    /// <summary>
    /// Repository class for PMS06002 controller
    /// </summary>
    public class PMS06002Repository : Repository, IPMS06002Repository
    {

        #region Members and constructors
        /// <summary>
        /// Actual work time zero
        /// </summary>
        private const int ACTUAL_WORK_TIME_ZERO = 0;

        /// <summary>
        /// Database object instant
        /// </summary>
        private readonly Database database;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PMS06002Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {

        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="database"></param>
        public PMS06002Repository(PMSDatabase database)
        {
            this.database = database;
        }
        #endregion

        #region ActualWorkList methods

        /// <summary>
        /// Get Info of Actual work list
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <returns>List info of actual work</returns>
        public PageInfo<dynamic> GetActualWorkList(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            Condition condition,
            string userIds)
        {
            var sql = buildSelectQueryList(condition, 0, sortDir, false, userIds);
            Mappers.Revoke(typeof(System.Dynamic.ExpandoObject));
            var pageInfo = Page<dynamic>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get info of work list
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <returns>List info of work</returns>
        public IList<dynamic> GetWorkListExport(Condition condition, int sort_column, string sort_type)
        {
            var sql = this.buildSelectQueryList(condition, sort_column, sort_type, true, null);
            return this.database.Fetch<dynamic>(sql);
        }

        /// <summary>
        /// SQL Query get info of actual work
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <param name="isExport">isExport</param>
        /// <returns>SQL query</returns>
        private Sql buildSelectQueryList(Condition condition, int sort_column, string sort_type, bool isExport = false, string userIds = null)
        {
            string cols = this.BuildColumnNames(condition.StartMonth, condition.EndMonth);

            var sb = new StringBuilder();

            sb.Append(@" SELECT ");
            if (isExport)
            {
                string sort_column_name = string.Empty;
                switch (sort_column)
                {
                    case 2:
                        sort_column_name = "group_name";
                        break;
                    case 3:
                        sort_column_name = "user_name";
                        break;
                    default:
                        sort_column_name = "user_sys_id";
                        break;
                }
                sb.Append(@"
                    ROW_NUMBER() OVER (order by " + sort_column_name + " " + sort_type + @") [No], 
                    group_name,
                    user_name,
user_sys_id,
                ");
            }
            else
            {
                sb.Append(@"
                    group_name,
                    user_name,
                    user_sys_id, ");
            }
            sb.Append(@"
            " + cols + @" 
        FROM
            (
                SELECT
                    result.group_name AS group_name,
                    result.user_name AS user_name,
                    result.user_sys_id AS user_sys_id,
                    dbo.Date2String(result.target_month, result.target_year) AS [Month],");
            if (condition.Filterable == 1 && userIds != null)
            {
                sb.Append(@"
                    CASE WHEN ((SELECT COUNT(DISTINCT mawd.actual_work_date) FROM member_actual_work_detail mawd
                        INNER JOIN project_info pi
                        ON mawd.project_sys_id = pi.project_sys_id
                        LEFT JOIN (
                            SELECT ar.* FROM attendance_record ar
                            INNER JOIN m_attendance_type mat
                            ON ar.company_code = mat.company_code
                            AND ar.attendance_type_id = mat.attendance_type_id
                            WHERE mat.non_operational_flg = '0'
                            AND mat.del_flg = '0'
                        ) as arTbl
                        ON mawd.user_sys_id = arTbl.user_sys_id
                        AND mawd.company_code = arTbl.company_code 
                        AND mawd.actual_work_year = arTbl.actual_work_year
                        AND mawd.actual_work_month = arTbl.actual_work_month
                        AND mawd.actual_work_date = arTbl.actual_work_date
                        WHERE mawd.user_sys_id = result.user_sys_id
                        AND mawd.actual_work_year =  result.target_year
                        AND mawd.actual_work_month = result.target_month
                        AND pi.del_flg = 0
                        AND mawd.actual_work_time <> 0
                        AND CHARINDEX('@'+CONVERT(nvarchar(MAX),mawd.actual_work_date)+'@',days) <> 0)
                        = (SELECT COUNT(*) FROM dbo.[Split](days,','))
                        OR (days ='')
                        OR (
                            (SELECT COUNT(*) FROM member_assignment_detail as mad
                                INNER JOIN (SELECT pi.* FROM project_info pi
                                      INNER JOIN m_status ms
                                      ON pi.status_id = ms.status_id
                                      AND pi.company_code = ms.company_code
                                      WHERE ms.company_code = @company_code
                                      AND ms.operation_target_flg = '1' 
                                      AND ms.sales_type <> '2'
                                      AND pi.del_flg = '0'
                                  ) AS prjTbl
                                      ON mad.project_sys_id = prjTbl.project_sys_id
                                      WHERE mad.company_code = @company_code
                                      AND mad.target_year = result.target_year
                                      AND mad.target_month = result.target_month
                                      AND mad.user_sys_id = result.user_sys_id
                            ) = 0
                        ))
                        THEN CONCAT(result.total_work,'(1)') 
                        ELSE CONCAT(result.total_work,'(0)') 
                    END AS total_work
                ");
            }
            else
            {
                sb.Append(@"
                    result.total_work AS total_work
                ");
            }
            sb.Append(@"
                FROM
                    (
                        SELECT
                            group_name,
                            user_name,
                            user_sys_id,
                            target_month, 
                            target_year,
                            CONCAT(SUM(actual_work_time),'/',SUM(plan_effort),'(',ISNULL(regist_type,0),')') AS total_work,
                            (SELECT dbo.[GetWorkDaysInMonth](@company_code, user_sys_id, target_month, target_year)) AS days
                        FROM
                            (
                                SELECT
                                    mu.company_code,
                                    (SELECT display_name FROM m_group WHERE group_id =
                                        (SELECT TOP(1) group_id
                                        FROM enrollment_history
                                        WHERE company_code = mu.company_code
                                        AND user_sys_id = mu.user_sys_id
                                        ORDER BY actual_work_year DESC, actual_work_month DESC)) AS group_name,
                                    mu.display_name AS user_name,
                                    mu.user_sys_id,
                                    assign.target_year,
                                    assign.target_month,
                                    CASE WHEN assign.project_sys_id IN (SELECT project_info.project_sys_id FROM project_info INNER JOIN m_status ON project_info.status_id = m_status.status_id AND project_info.company_code = m_status.company_code WHERE m_status.sales_type = '2' OR m_status.operation_target_flg <> '1') 
                                        THEN 0
                                        ELSE ");
            switch (condition.WorkTimeUnit)
            {
                case Constant.TimeUnit.HOUR:
                    sb.Append(@" dbo.Day2Hour(@company_code, assign.plan_man_days) ");
                    break;
                case Constant.TimeUnit.DAY:
                    sb.Append(@" assign.plan_man_days ");
                    break;
                case Constant.TimeUnit.MONTH:
                    sb.Append(@" dbo.Day2Month(assign.company_code, assign.target_month, assign.target_year, assign.plan_man_days) ");
                    break;
            }

            sb.Append(@"
                            END AS plan_effort,
                            ISNULL(actual.actual_work_time, 0) AS actual_work_time,
                            (
                                SELECT
                                    regist_type
                                FROM
                                    member_actual_work
                                WHERE
                                        company_code = assign.company_code
                                    AND user_sys_id = assign.user_sys_id
                                    AND actual_work_year = assign.target_year
                                    AND actual_work_month = assign.target_month
                            ) AS regist_type
                        FROM m_user AS mu 
                            INNER JOIN
                            member_assignment_detail AS assign
                            ON
                                assign.company_code = mu.company_code
                            AND assign.user_sys_id = mu.user_sys_id ");
            if (userIds != null)
            {
                sb.Append(" AND assign.user_sys_id IN (" + userIds.ToString() + ")");
            }

            if (!condition.DELETED_INCLUDE)
            {
                sb.Append(" AND mu.del_flg = '0' ");
            }

            if (!condition.RETIREMENT_INCLUDE)
            {
                sb.Append(@" AND (mu.retirement_date IS NULL
                            OR  @CurrentDate <= mu.retirement_date) ");
            }

            sb.Append(@"
                        INNER JOIN
                            project_info AS project
                        ON
                                assign.company_code = project.company_code
                            AND assign.project_sys_id = project.project_sys_id
                            AND project.del_flg = '0'
                        LEFT OUTER JOIN
                            (
                                SELECT
                                    company_code,
                                    project_sys_id,
                                    user_sys_id,
                                    actual_work_year,
                                    actual_work_month, ");
            switch (condition.WorkTimeUnit)
            {
                case Constant.TimeUnit.HOUR:
                    sb.Append(@"
                                    SUM(dbo.RemoveRoundDecimal(actual_work_time)) AS actual_work_time ");
                    break;
                case Constant.TimeUnit.DAY:
                    sb.Append(@"
                                    dbo.Hour2Day(company_code, SUM(actual_work_time)) AS actual_work_time ");
                    break;
                case Constant.TimeUnit.MONTH:
                    sb.Append(@"
                                    dbo.Hour2Month(company_code, actual_work_month, actual_work_year, SUM(actual_work_time)) AS actual_work_time ");
                    break;
            }

            sb.Append(@"
                                FROM
                                    member_actual_work_detail
                                GROUP BY
                                    company_code,
                                    project_sys_id,
                                    user_sys_id,
                                    actual_work_year,
                                    actual_work_month
                            ) AS actual
                        ON
                                assign.company_code = actual.company_code
                            AND assign.project_sys_id = actual.project_sys_id
                            AND assign.user_sys_id = actual.user_sys_id
                            AND assign.target_year = actual.actual_work_year
                            AND assign.target_month = actual.actual_work_month");
            if (userIds != null)
            {
                sb.Append(" AND assign.user_sys_id IN (" + userIds.ToString() + ")");
            }
            sb.Append(@"
                        WHERE
                                assign.company_code =  @company_code
                            AND CONVERT(varchar,dbo.Date2Number(assign.target_month, assign.target_year)) BETWEEN  @fromDate AND  @toDate ");

            if (!string.IsNullOrEmpty(condition.DisplayName))
            {
                sb.AppendFormat(@"
                        AND (mu.display_name LIKE '%{0}%' ESCAPE '\'
                            OR mu.user_name_sei LIKE '%{0}%' ESCAPE '\'
                            OR mu.user_name_mei LIKE '%{0}%' ESCAPE '\'
                            OR mu.furigana_sei LIKE '%{0}%' ESCAPE '\'
                            OR mu.furigana_mei LIKE '%{0}%' ESCAPE '\' )
                ", replaceWildcardCharacters(condition.DisplayName));
            }
            if (condition.GroupId != null)
            {
                sb.AppendFormat(" AND mu.group_id = {0} ", condition.GroupId.Value);
            }
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(" AND mu.location_id IN ({0}) ", condition.LOCATION_ID);
            }
            sb.Append(@"
                UNION ALL
                    SELECT mu.company_code, (SELECT display_name FROM m_group WHERE group_id =
                                (SELECT TOP(1) group_id
                                FROM enrollment_history
                                WHERE company_code = mu.company_code
                                AND user_sys_id = mu.user_sys_id
                                ORDER BY actual_work_year DESC, actual_work_month DESC)) AS group_name,
                            mu.display_name AS user_name,
                            mu.user_sys_id,
                            NULL as target_year,
                            NULL as target_month,
                            NULL as plan_effort,
                            NULL as actual_work_time,
                            NULL as regist_type
                    FROM m_user mu 
                    WHERE mu.company_code = @company_code
            ");
            if (!condition.DELETED_INCLUDE)
            {
                sb.Append(" AND mu.del_flg = '0' ");
            }

            if (!condition.RETIREMENT_INCLUDE)
            {
                sb.Append(@" AND (mu.retirement_date IS NULL
                            OR  @CurrentDate <= mu.retirement_date) ");
            }
            if (userIds != null)
            {
                sb.Append(" AND mu.user_sys_id IN (" + userIds.ToString() + ")");
            }

            if (!string.IsNullOrEmpty(condition.DisplayName))
            {
                sb.AppendFormat(@"
                        AND (mu.display_name LIKE '%{0}%' ESCAPE '\'
                            OR mu.user_name_sei LIKE '%{0}%' ESCAPE '\'
                            OR mu.user_name_mei LIKE '%{0}%' ESCAPE '\'
                            OR mu.furigana_sei LIKE '%{0}%' ESCAPE '\'
                            OR mu.furigana_mei LIKE '%{0}%' ESCAPE '\' )
                ", replaceWildcardCharacters(condition.DisplayName));
            }
            if (condition.GroupId != null)
            {
                sb.AppendFormat(" AND mu.group_id = {0} ", condition.GroupId.Value);
            }
            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(" AND mu.location_id IN ({0}) ", condition.LOCATION_ID);
            }
            sb.Append(@"
                    ) AS tbResult
                GROUP BY
                    company_code,
                    group_name,
                    user_name,
                    user_sys_id,
                    target_year,
                    target_month,
                    regist_type
                )AS result
            ) AS R pivot (MAX(total_work) for Month IN (" + cols + ")) AS P ");

            Sql sql = new Sql(sb.ToString(),
               new { company_code = condition.CompanyCode },
               new { displayName = condition.DisplayName },
               new { fromDate = this.Date2Number(condition.StartMonth) },
               new { toDate = this.Date2Number(condition.EndMonth) },
               new { CurrentDate = Utility.GetCurrentDateTime() }
            );

            return sql;
        }
        /// <summary>
        /// Construct a string representing name of columns to be selected on pivot query
        /// </summary>
        /// <param name="startDate">startDate</param>
        /// <param name="endDate">endDate</param>
        /// <returns>name of colum</returns>
        private string BuildColumnNames(DateTime startDate, DateTime endDate)
        {
            var sb = new StringBuilder();
            var y = startDate.Year;
            var m = startDate.Month;

            while (y < endDate.Year || (y == endDate.Year && m <= endDate.Month))
            {
                sb.AppendFormat("[{0}/{1:00}],", y, m);
                m++;
                if (m == 13)
                {
                    m = 1;
                    y++;
                }
            }
            return sb.ToString(0, sb.Length - 1);
        }


        /// <summary>
        /// Convert month and year value to the form of yyyymm, 
        /// used to compare between other values
        /// </summary>
        /// <param name="date">date</param>
        /// <returns>Format year month</returns>
        private string Date2Number(DateTime date)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}{1:00}", date.Year, date.Month);
            return sb.ToString();
        }

        #endregion

        #region ActualWorkDetailNew methods

        /// <summary>
        /// Get info of Actual work detail new
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_id">user_id</param>
        /// <param name="selected_year">selected_year</param>
        /// <param name="selected_month">selected_month</param>
        /// <returns>List info of Actual work detail</returns>
        public IList<dynamic> GetActualWorkDetailNew(string company_code, string user_id, int selected_year, int selected_month)
        {
            int user_sys_id = Convert.ToInt32(user_id);
            var sql = Sql.Builder.Append(@";EXEC    [dbo].[GetDetailData]
                        @company_code,
                        @user_sys_id,
                        @actual_work_year,
                        @actual_work_month"
          , new
          {
              company_code = company_code,
              user_sys_id = user_sys_id,
              actual_work_year = selected_year,
              actual_work_month = selected_month
          });
            Mappers.Revoke(typeof(System.Dynamic.ExpandoObject));
            var result = database.Fetch<dynamic>(sql);
            return result;
        }

        /// <summary>
        /// Get list actual work detail
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <returns>List actual work detail</returns>
        public IList<UserActualWorkDetailPlus> GetUserActualWorkDetailPlus(DetailCondition condition, string companycode)
        {
            Sql sql = buildSelectQueryDetail(condition, companycode, false, true);

            var results = database.Fetch<UserActualWorkDetailPlus>(sql);

            return results;
        }

        /// <summary>
        /// Check valid data actual work
        /// </summary>
        /// <param name="dataMemberActualWork">dataMemberActualWork</param>
        /// <returns>bool : true/false</returns>
        public bool CheckDataValid(MemberActualWork dataMemberActualWork)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"
                SELECT
                    COUNT(*)
                FROM
                    member_assignment_detail mad
                WHERE
                    (
                        mad.company_code = @company_code
                        AND mad.user_sys_id = @user_sys_id
                        AND mad.target_year = @actual_work_year
                        AND mad.target_month = @actual_work_month
                    )
            ");
            Sql sql = new Sql(query.ToString(),
                new { company_code = dataMemberActualWork.company_code },
                new { user_sys_id = dataMemberActualWork.user_sys_id },
                new { actual_work_year = dataMemberActualWork.actual_work_year },
                new { actual_work_month = dataMemberActualWork.actual_work_month }
               );
            int results1 = database.FirstOrDefault<int>(sql);
            if (results1 == 0)
            {
                return true;
            }

            query = new StringBuilder();
            query.Append(@"
                SELECT
                    SUM(CASE WHEN ROUND(tb.total_work_time,2) = tb.attendance_time THEN 0 ELSE 1 END) count_not_match
                FROM
                (
                    SELECT
                        ISNULL(mawd.company_code, ar.company_code) company_code, 
                        ISNULL(mawd.user_sys_id, ar.user_sys_id) user_sys_id, 
                        ISNULL(mawd.actual_work_year, ar.actual_work_year) actual_work_year, 
                        ISNULL(mawd.actual_work_month, ar.actual_work_month) actual_work_month,
                        ISNULL(mawd.actual_work_date, ar.actual_work_date) actual_work_date,
                        SUM(dbo.RemoveRoundDecimal(ISNULL(mawd.actual_work_time, 0 ))) total_work_time, 
                        ROUND((ROUND(ISNULL(ar.work_end_time, 0) * 60, 0) - ROUND(ISNULL(ar.work_start_time, 0) * 60, 0) - ROUND(ISNULL(ar.rest_time, 0) * 60, 0)) / 60, 2) attendance_time
                    FROM 
                        member_actual_work_detail mawd
                        INNER JOIN member_assignment_detail mad
                            ON mawd.company_code = mad.company_code
                            AND mawd.user_sys_id = mad.user_sys_id
                            AND mawd.project_sys_id = mad.project_sys_id
                            AND mawd.actual_work_year = mad.target_year
                            AND mawd.actual_work_month = mad.target_month
                        INNER JOIN (SELECT * FROM project_info pi 
                                WHERE company_code = @company_code AND del_flg = '0') AS pi
                            ON mawd.company_code = pi.company_code
                            AND mawd.project_sys_id = pi.project_sys_id
                        FULL JOIN attendance_record ar
                            ON mawd.company_code = ar.company_code
                            AND mawd.user_sys_id = ar.user_sys_id
                            AND mawd.actual_work_year = ar.actual_work_year
                            AND mawd.actual_work_month = ar.actual_work_month
                            AND mawd.actual_work_date = ar.actual_work_date
                    WHERE
                        (
                            mawd.company_code = @company_code
                            AND mawd.user_sys_id = @user_sys_id
                            AND mawd.actual_work_year = @actual_work_year
                            AND mawd.actual_work_month = @actual_work_month
                        )
                        OR
                        (
                            ar.company_code = @company_code
                            AND ar.user_sys_id = @user_sys_id
                            AND ar.actual_work_year = @actual_work_year
                            AND ar.actual_work_month = @actual_work_month
                        )
                    GROUP BY mawd.company_code, 
                            mawd.user_sys_id, 
                            mawd.actual_work_year, 
                            mawd.actual_work_month,
                            mawd.actual_work_date, 
                            ar.company_code, 
                            ar.user_sys_id, 
                            ar.actual_work_year, 
                            ar.actual_work_month,
                            ar.actual_work_date,
                            work_end_time, 
                            work_start_time, 
                            rest_time
                ) tb ");
            sql = new Sql(query.ToString(),
                new { company_code = dataMemberActualWork.company_code },
                new { user_sys_id = dataMemberActualWork.user_sys_id },
                new { actual_work_year = dataMemberActualWork.actual_work_year },
                new { actual_work_month = dataMemberActualWork.actual_work_month }
               );

            int? results = database.FirstOrDefault<int?>(sql);

            return (results == 0 || results == null);
        }

        /// <summary>
        /// Get work closing date
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>number closing date is get</returns>
        public int GetWorkClosingDate(string company_code)
        {
            var sql = Sql.Builder.Append(@"
                SELECT 
                    work_closing_date
                FROM 
                    m_company_setting
                WHERE 
                    company_code = @company_code "
            , new { company_code = company_code });
            var result = database.SingleOrDefault<int>(sql);
            return result;
        }

        /// <summary>
        /// Get list Attendance Detail info
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_sys_id">user_sys_id</param>
        /// <param name="FromDate">FromDate</param>
        /// <param name="ToDate">ToDate</param>
        /// <returns>List Attendance Detail info</returns>
        public IList<AttendanceDetail> GetAttendanceInfor(string company_code, int user_sys_id, DateTime FromDate, DateTime ToDate)
        {
            StringBuilder query = new StringBuilder();

            query.Append(@"
SELECT
    calendar.targetDate as working_date,
	isnull(attendanceData.attendance_type_name, '') as attendance_type_name,
	isnull(attendanceData.work_start_time, 0) as work_start_time,
	isnull(attendanceData.work_end_time, 0) as work_end_time,
	isnull(attendanceData.clock_in_start_time, 0) as clock_in_start_time,
	isnull(attendanceData.clock_in_end_time, 0) as clock_in_end_time,
	isnull(attendanceData.rest_time, 0) as rest_time,
	isnull(attendanceData.allowed_cost_time, 0) as allowed_cost_time,
	isnull(attendanceData.remarks, '') as remarks,
    isnull(mh.holiday_name, '') as holiday_name
FROM
	dbo.GetCalendar(@FromDate,@ToDate) as calendar
	LEFT OUTER JOIN
		(
		SELECT
			DATEFROMPARTS(actual_work_year,actual_work_month,actual_work_date) AS working_date,
			mat.attendance_type AS attendance_type_name,
			work_start_time,
			work_end_time,
			clock_in_start_time,
			clock_in_end_time,
			rest_time,
			ar.allowed_cost_time,
			ar.remarks
		FROM
			attendance_record ar
			INNER JOIN m_user us
				ON ar.company_code = us.company_code
				AND ar.user_sys_id = us.user_sys_id
			LEFT JOIN m_group gr 
				ON us.company_code = gr.company_code
				AND us.group_id = gr.group_id
			INNER JOIN m_attendance_type mat
				ON ar.company_code = mat.company_code
				AND ar.attendance_type_id = mat.attendance_type_id
		WHERE
			ar.company_code = @company_code
			AND ar.user_sys_id = @user_sys_id
			AND DATEFROMPARTS(actual_work_year,actual_work_month,actual_work_date) BETWEEN @FromDate2 AND @ToDate2
		) as attendanceData
		ON calendar.targetDate = attendanceData.working_date
		LEFT OUTER JOIN m_holiday as mh
			ON mh.company_code = @company_code2
			and calendar.targetDate = mh.holiday_date");

            Sql sql = new Sql(query.ToString(),
                new { FromDate = FromDate },
                new { ToDate = ToDate },
                new { company_code = company_code },
                new { user_sys_id = user_sys_id },
                new { FromDate2 = FromDate },
                new { ToDate2 = ToDate },
                new { company_code2 = company_code }
               );

            var attendanceInfo = database.Fetch<AttendanceDetail>(sql);

            return attendanceInfo;
        }
        #endregion

        #region ActualWorkDetail methods
        /// <summary>
        /// get work detail
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <returns>List work detail</returns>
        public PageInfo<UserActualWorkDetailPlus> GetWorkDetail(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            DetailCondition condition)
        {
            Sql sql = this.buildSelectQueryDetail(condition, companyCode, false); 

            var pageInfo = Page<UserActualWorkDetailPlus>(startItem, int.MaxValue, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get list work detail
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <param name="projectId">projectId</param>
        /// <param name="targetPhase">targetPhase</param>
        /// <returns>List work detail</returns>
        private IList<WorkDetailPlus> GetWorkDetail(int userId, int year, int month, int projectId, int targetPhase)
        {
            var sql = new Sql(@"SELECT detail_no, actual_work_date, actual_work_time 
                              from member_actual_work_detail mawd
                                WHERE user_sys_id = @user_sys_id
                                    AND actual_work_year = @actual_work_year
                                    AND actual_work_month = @actual_work_month
                                    AND project_sys_id = @project_sys_id
                                    AND phase_id = @phase_id
                                order by actual_work_date ASC"
                                , new
                                {
                                    user_sys_id = userId,
                                    actual_work_year = year,
                                    actual_work_month = month,
                                    project_sys_id = projectId,
                                    phase_id = targetPhase
                                });

            var result = database.Query<WorkDetailPlus>(sql);

            return result.ToList();
        }
        /// <summary>
        /// get work detail for export
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List work detail</returns>
        public IList<UserActualWorkDetail> GetWorkDetailExport(DetailCondition condition, string companyCode)
        {
            var sql = this.buildSelectQueryDetail(condition, companyCode, true);
            return this.database.Fetch<UserActualWorkDetail>(sql);
        }

        /// <summary>
        /// get detail total info
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns>Work detail info</returns>
        public WorkDetailInfo GetDetailInfo(DetailCondition condition, string companycode)
        {
            var sql = new Sql();
            sql = buildQueryGetInfo(condition, companycode);
            return database.FirstOrDefault<WorkDetailInfo>(sql);
        }

        /// <summary>
        /// get user_name, group_name
        /// </summary>
        /// <param name="UserId">UserId</param>
        /// <returns>User info</returns>
        public UserBaseInfo GetUserBaseInfo(string UserId)
        {
            int user_sys_id = Convert.ToInt32(UserId);
            StringBuilder query = new StringBuilder();
            query.Append(@"
                SELECT 
                    ISNULL(gr.display_name,'') as group_name,
                    us.user_name_sei,
                    us.user_name_mei,
                    us.display_name,
                    us.company_code,
                    us.employee_no,
					us.entry_date,
					loc.location_name
                FROM m_user us 
                    LEFT JOIN m_group gr
                    ON us.group_id = gr.group_id
				    LEFT JOIN m_business_location loc 
				    ON us.location_id = loc.location_id
                WHERE us.user_sys_id = @user_sys_id ");
            Sql sql = new Sql(query.ToString(),
                new { user_sys_id = user_sys_id }
               );
            return database.FirstOrDefault<UserBaseInfo>(sql);
        }

        /// <summary>
        /// Get Update info
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="company_code">company_code</param>
        /// <returns>Update info</returns>
        public UpdateInfo GetUpdateInfor(DetailCondition condition, string company_code)
        {
            int user_sys_id = Convert.ToInt32(condition.UserId);
            StringBuilder query = new StringBuilder();
            query.Append(@"
                SELECT
                    CASE maw.regist_type
                        WHEN 0 THEN N'本登録'
                        ELSE N'仮登録'
                    END regist_type,
                    maw.ins_date insert_date,
                    ins_us.display_name insert_person,
                    maw.upd_date last_update_date,
                    upd_us.display_name last_update_person
                FROM member_actual_work maw
                    LEFT JOIN m_user upd_us ON maw.upd_id = upd_us.user_sys_id
                    LEFT JOIN m_user ins_us ON maw.ins_id = ins_us.user_sys_id
                WHERE  maw.user_sys_id = @user_sys_id
                    AND maw.actual_work_year = @selected_year
                    AND maw.actual_work_month = @selected_month
                    AND maw.company_code = @company_code ");
            Sql sql = new Sql(query.ToString(),
                new { user_sys_id = user_sys_id },
                new { selected_year = condition.SelectedYear },
                new { selected_month = condition.SelectedMonth },
                new { company_code = company_code }
               );
            return database.FirstOrDefault<UpdateInfo>(sql);
        }

        /// <summary>
        /// build query for get total detail
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns>Query SQL</returns>
        private Sql buildQueryGetInfo(DetailCondition condition, string companycode)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"
            SELECT 
                ISNULL(group_name, ''),
                user_name,
                SUM(plan_effort) plan_effort,
                SUM(actual_effort) actual_effort,
                SUM(estimate_cost) estimate_cost,
                SUM(actual_cost) actual_cost,
                CASE MAX(regist_type)
                    WHEN 0 THEN N'本登録'
                    ELSE N'仮登録'
                END regist_type
             FROM 
             (
                
                SELECT 
                        gr.display_name as group_name,
                        us.display_name as user_name, 
            ");
            switch (condition.WorkTimeUnit)
            {
                case "1":
                    query.Append(@" SUM(ISNULL(tb_plan.plan_man_hours, 0)) as plan_effort,
                                    SUM(ISNULL(tb_actual.actual_man_hours, 0)) as actual_effort, ");
                    break;
                case "2":
                    query.Append(@" SUM(ISNULL(tb_plan.plan_man_days, 0)) as plan_effort,        
                                    SUM(ISNULL(tb_actual.actual_man_days, 0)) as actual_effort, ");
                    break;
                case "3":
                    query.Append(@" SUM(ISNULL(tb_plan.plan_man_months, 0)) as plan_effort,
                                    SUM(ISNULL(tb_actual.actual_man_months, 0)) as actual_effort,");
                    break;
            }
            query.Append(@"
                    tb_unit_price.base_unit_cost * SUM(ISNULL(tb_plan.plan_man_days, 0)) / dbo.GetNumberOfWorkDaysInMonth(pi.company_code, @month, @year) AS estimate_cost,
                    tb_unit_price.base_unit_cost * SUM(ISNULL(tb_actual.actual_man_days, 0)) / dbo.GetNumberOfWorkDaysInMonth(pi.company_code, @month, @year) AS actual_cost,
                    maw.regist_type
                FROM 
                    project_info pi 
                    JOIN 
                    (
                        SELECT 
                            mad.user_sys_id,
                            mad.project_sys_id,
                            mad.target_month,
                            mad.target_year,
                            CASE WHEN m_status.sales_type = '2' OR m_status.operation_target_flg <> '1'
                                THEN 0
                                ELSE dbo.Day2Hour(mad.company_code,SUM(mad.plan_man_days)) 
                            END AS plan_man_hours,
                            CASE WHEN m_status.sales_type = '2' OR m_status.operation_target_flg <> '1'
                                THEN 0
                                ELSE SUM(mad.plan_man_days)
                            END AS plan_man_days,
                            CASE WHEN m_status.sales_type = '2' OR m_status.operation_target_flg <> '1'
                                THEN 0
                                ELSE dbo.Day2Month(mad.company_code, mad.target_month, mad.target_year, SUM(mad.plan_man_days))
                            END AS plan_man_months
                        FROM member_assignment_detail mad
                            JOIN project_info pi 
                                ON pi.company_code = mad.company_code
                                AND pi.project_sys_id = mad.project_sys_id
                            INNER JOIN m_status
                                ON pi.company_code = m_status.company_code
                                AND pi.status_id = m_status.status_id
                        WHERE
                            mad.user_sys_id = @userid
                            AND mad.target_year = @year 
                            AND mad.target_month = @month
                            AND pi.del_flg = '0'
                        GROUP BY mad.user_sys_id, mad.company_code,mad.target_month, mad.target_year, mad.project_sys_id, m_status.sales_type, m_status.operation_target_flg
                    ) tb_plan
                    ON pi.project_sys_id = tb_plan.project_sys_id
                    LEFT JOIN
                    (
                        SELECT 
                            mawd.user_sys_id,
                            mawd.project_sys_id,
                            mawd.actual_work_month,
                            mawd.actual_work_year, 
                            sum(mawd.actual_work_time) as actual_man_hours,
                            dbo.Hour2Day(mawd.company_code, SUM(mawd.actual_work_time)) as actual_man_days,
                            dbo.Hour2Month(mawd.company_code, mawd.actual_work_month, mawd.actual_work_year, SUM(mawd.actual_work_time)) as actual_man_months
                        FROM member_actual_work_detail mawd
                            JOIN project_info pi 
                                ON pi.company_code = mawd.company_code
                                AND pi.project_sys_id = mawd.project_sys_id
                        WHERE mawd.user_sys_id = @userid
                            AND mawd.actual_work_year = @year AND mawd.actual_work_month = @month
                        GROUP BY mawd.user_sys_id, mawd.company_code, mawd.actual_work_month, mawd.actual_work_year,mawd.project_sys_id
                    ) tb_actual
                    ON pi.project_sys_id = tb_actual.project_sys_id
                    JOIN m_user us
                    ON us.user_sys_id = tb_plan.user_sys_id
                    LEFT JOIN member_actual_work maw
                    ON us.user_sys_id = maw.user_sys_id
                        AND tb_plan.target_month = maw.actual_work_month
                        AND tb_plan.target_year = maw.actual_work_year
                    LEFT JOIN (
                                SELECT TOP 1 user_sys_id, base_unit_cost 
								FROM unit_price_history 
								WHERE company_code = @company_code
									AND user_sys_id = @userid
									AND apply_start_date <= CONVERT(date, CAST(@year AS varchar(4)) + '/'+ RIGHT('0' + CAST(@month AS VARCHAR(2)), 2) + '/01')
									AND del_flg = '0'
								ORDER BY apply_start_date DESC) tb_unit_price
						ON us.user_sys_id = tb_unit_price.user_sys_id
                    LEFT JOIN m_group gr
                    ON us.group_id = gr.group_id
                WHERE pi.company_code = @company_code
                    AND pi.del_flg = '0'
                GROUP BY gr.display_name, us.display_name , maw.regist_type, pi.project_sys_id, tb_unit_price.base_unit_cost, pi.company_code
            )tb 
            GROUP BY group_name, user_name ");

            int user_sys_id = Convert.ToInt32(condition.UserId);
            Sql sql = new Sql(query.ToString(),
                new { userid = user_sys_id },
                new { year = condition.SelectedYear },
                new { month = condition.SelectedMonth },
                new { company_code = companycode }
               );
            return sql;
        }

        /// <summary>
        /// build query for select table detail
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companycode">companycode</param>
        /// <param name="isExport">isExport</param>
        /// <returns>Build query sql</returns>
        private Sql buildSelectQueryDetail(DetailCondition condition, string companycode, bool isExport = false, bool isNewScreen = false)
        {
            StringBuilder query = new StringBuilder();

            query.Append(@"
            SELECT 
                * 
            FROM 
            (
                SELECT ");
            if (!isExport)
            {
                query.Append(@"
                    pi.project_sys_id,
                    pi.company_code, ");
            }
            query.Append(@"
                    pi.project_no,
                    pi.project_name,
                    mr.rank,
                    pgr.progress, 
                ");
            switch (condition.WorkTimeUnit)
            {
                case "1":
                    query.Append(@" CASE WHEN m_status.sales_type = '2'
                                        THEN 0
                                        ELSE ISNULL(tb_plan.plan_man_hours, 0)
                                    END AS plan_effort,
                                    ISNULL(tb_actual.actual_man_hours, 0) AS actual_effort,
                                ");
                    break;
                case "2":
                    query.Append(@" CASE WHEN m_status.sales_type = '2'
                                        THEN 0
                                        ELSE ISNULL(tb_plan.plan_man_days, 0)
                                    END AS plan_effort,
                                    ISNULL(tb_actual.actual_man_days, 0) AS actual_effort,
                                ");
                    break;
                case "3":
                    query.Append(@" CASE WHEN m_status.sales_type = '2'
                                        THEN 0
                                        ELSE ISNULL(tb_plan.plan_man_months, 0)
                                    END AS plan_effort,
                                    ISNULL(tb_actual.actual_man_months, 0) AS actual_effort,
                                ");
                    break;
            }
            query.Append(@"
                    CASE WHEN tb_plan.plan_man_days = 0 OR m_status.sales_type = '2' 
                        THEN 0
                        ELSE (ISNULL(tb_plan.plan_man_hours, 0) - ISNULL(tb_actual.actual_man_hours, 0)) / ISNULL(tb_plan.plan_man_hours, 0)
                    END AS personal_profit_rate,
                    CASE
                        WHEN pi.total_sales = 0 THEN 0
                        ELSE ((pi.total_sales - ISNULL(pi.total_payment, 0) - ISNULL(pj_actual.actual_cost, 0)) / pi.total_sales)
                    END AS [project_actual_profit],
                    pi.del_flg,
                    m_status.sales_type,
                    m_status.status,
                    (SELECT COUNT(pli.company_code)
                        FROM project_plan_info AS pli
                        WHERE pli.company_code = pi.company_code
                            AND pli.project_sys_id = pi.project_sys_id
                            AND pli.del_flg ='0')
                        AS count_project_plan
                FROM 
                    project_info pi
                    INNER JOIN m_status 
                        ON pi.status_id = m_status.status_id 
                        AND pi.company_code = m_status.company_code
                    INNER JOIN 
                    (
                        SELECT 
                            mad.user_sys_id,
                            mad.project_sys_id, 
                            dbo.Day2Hour(mad.company_code,SUM(mad.plan_man_days)) as plan_man_hours,
                            SUM(mad.plan_man_days) as plan_man_days,
                            dbo.Day2Month(mad.company_code, mad.target_month, mad.target_year, SUM(mad.plan_man_days)) as plan_man_months
                        FROM member_assignment_detail mad
                            JOIN project_info pi 
                                ON pi.company_code = mad.company_code
                                AND pi.project_sys_id = mad.project_sys_id
                        WHERE mad.user_sys_id = @userid
                            AND mad.target_year = @year
                            AND mad.target_month = @month
                            AND pi.del_flg = '0'
                        group by mad.user_sys_id, mad.company_code,mad.target_month, mad.target_year, mad.project_sys_id
                    ) tb_plan
                        ON pi.project_sys_id = tb_plan.project_sys_id
                    LEFT JOIN
                    (
                        SELECT 
                            mawd.user_sys_id,
                            mawd.project_sys_id,
                            sum(mawd.actual_work_time) as actual_man_hours,
                            dbo.Hour2Day(mawd.company_code, SUM(mawd.actual_work_time)) as actual_man_days,
                            dbo.Hour2Month(mawd.company_code, mawd.actual_work_month, mawd.actual_work_year, SUM(mawd.actual_work_time)) as actual_man_months
                        FROM member_actual_work_detail mawd
                            JOIN project_info pi 
                                ON pi.company_code = mawd.company_code
                                AND pi.project_sys_id = mawd.project_sys_id
                        WHERE mawd.user_sys_id = @userid
                            AND mawd.actual_work_year = @year 
                            AND mawd.actual_work_month = @month
                            AND pi.del_flg = '0'
                        GROUP BY mawd.user_sys_id, mawd.company_code, mawd.actual_work_month, mawd.actual_work_year,mawd.project_sys_id
                    ) tb_actual
                        ON pi.project_sys_id = tb_actual.project_sys_id
                    LEFT JOIN 
                    (
                        SELECT
                            tb_tmp.company_code,
                            tb_tmp.project_sys_id,
                            SUM(tb_tmp.total_work_time) [running_man_days],
                            SUM(tb_tmp.unit_cost * tb_tmp.total_work_time / tb_tmp.work_day) [actual_cost]
                        FROM 
                            (
                            SELECT
                                mawd.company_code, 
                                mawd.project_sys_id, 
                                (SELECT TOP 1 base_unit_cost FROM unit_price_history uph 
                                 WHERE uph.user_sys_id = mawd.user_sys_id 
                                    AND uph.apply_start_date <= CONVERT(date, CAST(mawd.actual_work_year AS varchar(4)) + '/'+ RIGHT('0' + CAST(mawd.actual_work_month AS VARCHAR(2)), 2) + '/01') ORDER BY apply_start_date DESC) [unit_cost],                               
                                dbo.Hour2Day(mawd.company_code, SUM(mawd.actual_work_time)) [total_work_time],
                                dbo.GetNumberOfWorkDaysInMonth(mawd.company_code, mawd.actual_work_month, mawd.actual_work_year) [work_day]
                            FROM
                                member_actual_work_detail mawd 
                            GROUP BY
                                mawd.company_code,
                                mawd.project_sys_id,
								mawd.user_sys_id,
                                mawd.actual_work_month,
                                mawd.actual_work_year
                            ) tb_tmp
                        GROUP BY
                            tb_tmp.company_code,
                            tb_tmp.project_sys_id
                    ) pj_actual
                        ON pj_actual.company_code = pi.company_code
                        AND pj_actual.project_sys_id = pi.project_sys_id
                    JOIN m_user us
                        ON us.user_sys_id = tb_plan.user_sys_id
                    LEFT JOIN member_assignment ma
                        ON us.user_sys_id = ma.user_sys_id
                        AND pi.project_sys_id = ma.project_sys_id
                    LEFT JOIN m_group gr
                        ON us.group_id = gr.group_id
                    LEFT JOIN m_rank mr
                        ON pi.rank_id = mr.rank_id 
                    LEFT JOIN 
                    (
                        SELECT prgh.*
                        FROM progress_history prgh
                        INNER JOIN
                        (
                            SELECT project_sys_id, MAX(regist_date) AS MaxDateTime
                            FROM progress_history
                            WHERE company_code = @company_code
                            GROUP BY project_sys_id
                        ) tmp 
                        ON prgh.project_sys_id = tmp.project_sys_id 
                        AND prgh.regist_date = tmp.MaxDateTime
                    ) pgr
                        ON pi.project_sys_id = pgr.project_sys_id
                WHERE us.company_code = @company_code
                    AND pi.del_flg = '0'
                    AND (
						(m_status.sales_type <> '2' AND m_status.operation_target_flg = '1')
						OR (SELECT SUM(actual_work_time) FROM member_actual_work_detail mawd WHERE mawd.user_sys_id = @userid
							AND mawd.project_sys_id = pi.project_sys_id
							AND mawd.actual_work_year = @year
							AND mawd.actual_work_month = @month) > 0
					)
            )tblProject ");

            if (isNewScreen)
            {
                query.Append(" order by project_no ");
            }

            int user_sys_id = Convert.ToInt32(condition.UserId);
            Sql sql = new Sql(query.ToString(),
                new { userid = user_sys_id },
                new { year = condition.SelectedYear },
                new { month = condition.SelectedMonth },
                new { company_code = companycode }
               );
            return sql;
        }

        /// <summary>
        /// get list actual work exist in month
        /// </summary>
        /// <param name="user_sys_id"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<int> ActualWorkDateList(int user_sys_id, int year, int month, string companyCode)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT
                    actual_work_date
                FROM
                    attendance_record
                WHERE
                       user_sys_id = @user_sys_id
                                            AND actual_work_year = @year
                                            AND actual_work_month = @month
                                            AND company_code = @companyCode"
                                            , new
                                            {
                                                user_sys_id = user_sys_id,
                                                year = year,
                                                month = month,
                                                companyCode = companyCode
                                            });

            return database.Fetch<int>(sql);
        }

        #endregion

        #region ActualWorkRegist methods
        /// <summary>
        /// Get the general actual work info of an user in a month
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <returns>User work info</returns>
        public UserWorkInfoPlus GetUserWorkInfo(int userId, int year, int month)
        {
            var sql = Sql.Builder.Append(@"
                SELECT 
                    ISNULL(g.display_name,' ') [group_name],
                    u.display_name [user_name],
                    u.employee_no,
                    (
                        SELECT  SUM(mawd.actual_work_time) total_work 
                        FROM member_actual_work_detail mawd
                        WHERE mawd.user_sys_id = @user_sys_id 
                            AND mawd.actual_work_year = @actual_work_year 
                            AND mawd.actual_work_month = @actual_work_month
                    ) as total_work 
                FROM m_user u LEFT JOIN m_group g 
                    ON u.group_id = g.group_id
                WHERE u.user_sys_id = @user_sys_id"
            , new
            {
                user_sys_id = userId,
                actual_work_year = year,
                actual_work_month = month
            });

            var result = database.Single<UserWorkInfoPlus>(sql);

            return result;
        }
        /// <summary>
        /// Get List Actual work
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="targetYear">targetYear</param>
        /// <param name="targetMonth">targetMonth</param>
        /// <returns>List Actual work</returns>
        public IList<ActualWorkPlus> GetActualWorkDetail(int userId, int targetYear, int targetMonth)
        {
            var sql = new Sql(@"
                SELECT
                    mad.company_code,
                    pi.project_sys_id,
                    pi.project_name,
                    p.phase_id,
                    p.display_name phase_name,
                    u.user_sys_id,
                    mad.target_month,
                    mad.target_year
                FROM 
                    member_assignment_detail mad
                    JOIN project_info pi on mad.project_sys_id = pi.project_sys_id
                    JOIN m_user u on mad.user_sys_id = u.user_sys_id
                    JOIN target_phase tp ON pi.project_sys_id = tp.project_sys_id
                    JOIN m_phase p ON tp.phase_id = p.phase_id
                    LEFT JOIN m_status s ON pi.status_id = s.status_id AND pi.company_code = s.company_code
                WHERE u.user_sys_id = @user_sys_id 
                    AND mad.target_year = @target_year
                    AND mad.target_month = @target_month
                    AND pi.del_flg = 0
                    AND ((s.sales_type <> '2'
                    AND s.operation_target_flg = '1')
                    OR (SELECT SUM(actual_work_time) FROM member_actual_work_detail mawd WHERE mawd.user_sys_id = @user_sys_id
							AND mawd.project_sys_id = pi.project_sys_id
							AND mawd.actual_work_year = @target_year
							AND mawd.actual_work_month = @target_month) > 0)
                ORDER BY pi.project_no, p.display_order
            ", new
            {
                user_sys_id = userId,
                target_year = targetYear,
                target_month = targetMonth
            });

            var result = database.Query<ActualWorkPlus>(sql).ToList();

            foreach (var aw in result)
            {
                int n = this.GetNumberOfDayInMonth(targetYear, targetMonth);

                aw.workDetails = new WorkDetailPlus[n];

                var list = this.GetWorkDetail(
                    aw.user_sys_id.Value,
                    aw.target_year.Value,
                    aw.target_month.Value,
                    aw.project_sys_id.Value,
                    aw.phase_id.Value);

                //database.CloseSharedConnection();

                //put available work detail in the list
                foreach (var workDetail in list)
                {
                    if (workDetail.actual_work_date != null)
                    {
                        aw.workDetails[workDetail.actual_work_date.Value - 1] = workDetail;
                    }
                }

                //assign the remaining item with an empty work details
                for (int i = 0; i < aw.workDetails.Count(); i++)
                {
                    if (aw.workDetails[i] == null)
                        aw.workDetails[i] = new WorkDetailPlus();
                }
            }

            return result.ToList();
        }
        /// <summary>
        /// Select start , end .. time
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <returns>List Work Attendance Detail</returns>
        public IList<WorkAttendanceDetailPlus> GetWorkAttendanceDetail(int userId, int year, int month, DataTable dt)
        {
            var sql = new Sql(@"
                SELECT DISTINCT 
                    ar.actual_work_date as actual_work_date, 
                    ar.work_start_time as work_start_time, 
                    ar.work_end_time as work_end_time, 
                    ar.rest_time as rest_time, 
                    ar.allowed_cost_time, 
                    ar.attendance_type_id as attendance_type_id, 
                    mat.display_name  as attendance_type_name,
                    maw.regist_type as regist_type
                FROM attendance_record ar
                    RIGHT JOIN member_actual_work maw
                    ON ar.user_sys_id = maw.user_sys_id
                        AND ar.actual_work_month = maw.actual_work_month
                        AND ar.actual_work_year = maw.actual_work_year
                    LEFT JOIN m_attendance_type mat
					ON ar.attendance_type_id = mat.attendance_type_id
                WHERE maw.user_sys_id = @user_sys_id
                    AND maw.actual_work_year = @actual_work_year
                    AND maw.actual_work_month = @actual_work_month
                ORDER BY ar.actual_work_date ASC
            ", new
            {
                user_sys_id = userId,
                actual_work_year = year,
                actual_work_month = month,
            });

            var result = database.Query<WorkAttendanceDetailPlus>(sql);
            int n = this.GetNumberOfDayInMonth(year, month);
            var workAttendanceDetails = new WorkAttendanceDetailPlus[n];
            // string registType = string.Empty;
            foreach (var workAttendanceDetail in result)
            {

                if (workAttendanceDetail.actual_work_date != null)
                {
                    workAttendanceDetails[workAttendanceDetail.actual_work_date.Value - 1] = workAttendanceDetail;
                }
            }
            //assign the remaining item with an empty work details
            for (int i = 0; i < workAttendanceDetails.Count(); i++)
            {
                if (workAttendanceDetails[i] == null)
                {
                    workAttendanceDetails[i] = new WorkAttendanceDetailPlus();
                }
            }

            if (dt != null)
            {
                DateTime date;
                foreach (DataRow dtRow in dt.Rows)
                {
                    for (int i = 0; i < workAttendanceDetails.Count(); i++)
                    {
                        if (workAttendanceDetails[i].actual_work_date == null)
                        {
                            date = DateTime.Parse("" + year + "/" + month + "/" + (i + 1));
                            if (date == DateTime.Parse(dtRow[2].ToString()))
                            {
                                workAttendanceDetails[i].work_start_time = formatHour(dtRow[3].ToString());
                                workAttendanceDetails[i].work_end_time = formatHour(dtRow[4].ToString());
                            }
                        }
                        else
                        {
                            date = DateTime.Parse("" + year + "/" + month + "/" + workAttendanceDetails[i].actual_work_date);
                            if (date == DateTime.Parse(dtRow[2].ToString()))
                            {
                                workAttendanceDetails[i].work_start_time = formatHour(dtRow[3].ToString());
                                workAttendanceDetails[i].work_end_time = formatHour(dtRow[4].ToString());
                            }
                        }
                    }


                }
            }

            return workAttendanceDetails.ToList();
        }
        /// <summary>
        /// Calculate the number of days in a specific month
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <returns>number of day in month</returns>
        private int GetNumberOfDayInMonth(int year, int month)
        {
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) return 31;
            if (month == 4 || month == 6 || month == 9 || month == 11) return 30;

            if (year % 4 == 0 && year % 100 != 0 || year % 400 == 0)
                return 29;
            else
                return 28;
        }

        /// <summary>
        /// Convert hour format to decimal
        /// </summary>
        /// <param name="hour_minute">hour_minute</param>
        /// <returns>hour</returns>
        public decimal formatHour(string hour_minute)
        {
            decimal hour = Convert.ToInt32(hour_minute.Split(':')[0]);
            decimal minute = Convert.ToInt32(hour_minute.Split(':')[1]);
            return hour + minute / 60;
        }

        /// <summary>
        /// Put the list of actual work detail to database
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>bool: true/false</returns>
        public bool PutActualWorkDetailList(
            IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            IList<AttendanceRecord> dataListAttendanceRecord)
        {
            var result = false;

            if (dataListMemberActualWorkDetail != null)
            {
                foreach (var memberActualWorkDetail in dataListMemberActualWorkDetail)
                {
                    result = PutActualWorkDetailNew(memberActualWorkDetail);

                    if (!result) return false;
                }
            }

            if (dataMemberActualWork != null)
            {
                this.UpdateMemberActualWork(dataMemberActualWork);
            }

            if (dataListAttendanceRecord != null)
            {
                foreach (var attendanceRecord in dataListAttendanceRecord)
                {
                    result = UpdateAttendanceRecordNew(attendanceRecord);

                    if (!result) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Update the member_actual_work table as the member_actual_work_detail table changed
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="userId">userId</param>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <returns>bool: true/false</returns>
        private bool UpdateMemberActualWork(MemberActualWork data)
        {
            //UpdateMemberActualWork

            var sql = Sql.Builder.Append(@"declare @@rowcount int;");
            sql.Append(
                @"exec @@rowcount = UpdateMemberActualWork
                                                @company_code,
                                                @user_sys_id ,
                                                @actual_work_year ,
                                                @actual_work_month,
                                                @regist_type,
                                                @ins_date,
                                                @ins_id,
                                                @upd_date,
                                                @upd_id;",
                                            new
                                            {
                                                company_code = data.company_code,
                                                user_sys_id = data.user_sys_id,
                                                actual_work_year = data.actual_work_year,
                                                actual_work_month = data.actual_work_month,
                                                regist_type = data.regist_type,
                                                ins_date = data.ins_date,
                                                ins_id = data.ins_id,
                                                upd_date = data.upd_date,
                                                upd_id = data.upd_id
                                            });
            sql.Append("SELECT @@rowcount");

            var result = database.ExecuteScalar<int>(sql);
            return result == 1;
        }
        /// <summary>
        /// Get all Attendance Type
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>List of Attendance Type</returns>
        public IEnumerable<AttendanceType> GetAttendanceTypeList(string cCode)
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

            return this.Select<AttendanceType>(condition, "display_order");
        }
        /// <summary>
        /// Get list of weekly holiday
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>List weekly holiday</returns>
        public IList<DayOfWeek> GetWeeklyHoliday(string company_code)
        {
            var sql = Sql.Builder.Append(@"
                SELECT default_holiday_type from m_company_setting WHERE company_code = @company_code
            ", new { company_code = company_code });

            var listOfWeek = database.ExecuteScalar<string>(sql);

            if (!string.IsNullOrEmpty(listOfWeek))
            {
                return database.ExecuteScalar<string>(sql).Split(',').Select(x => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), x)).ToList();
            }

            return new List<DayOfWeek>();
        }
        /// <summary>
        /// Get list of special holiday
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <returns>List specia holiday</returns>
        public IList<DateTime> GetSpecialHoliday(string company_code)
        {
            var sql = Sql.Builder.Append(@"
                SELECT holiday_date FROM m_holiday WHERE company_code = @company_code
            ", new { company_code = company_code });
            return database.Query<DateTime>(sql).ToList();
        }
        #endregion

        #region ActualWorkRegistNew methods
        /// <summary>
        /// Get regist type
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="user_id">user_id</param>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <returns>Regist Type</returns>
        public string GetRegistType(string company_code, int user_id, int year, int month)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT regist_type FROM member_actual_work WHERE company_code = @company_code
                AND user_sys_id = @user_sys_id AND actual_work_year = @actual_work_year AND actual_work_month = @actual_work_month"
            , new
            {
                company_code = company_code,
                user_sys_id = user_id,
                actual_work_year = year,
                actual_work_month = month
            });

            return database.SingleOrDefault<string>(sql);
        }
        /// <summary>
        /// Get information for regist new screen
        /// </summary>
        /// <param name="day">day</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <param name="userId">userId</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>Attendance Record</returns>
        public AttendanceRecord GetAttendanceRecordInfor(int day, int month, int year, int userId, string companyCode)
        {
            var sql = new Sql(@"Select tb1.work_start_time, tb1.work_end_time, tb1.rest_time, tb1.allowed_cost_time, tb1.attendance_type_id, tb2.display_name , tb1.remarks
                                from attendance_record tb1 LEFT JOIN m_attendance_type tb2
                                ON tb1.attendance_type_id = tb2.attendance_type_id
                                Where tb1.user_sys_id = @userId
                                AND tb1.actual_work_date = @day
                                AND tb1.actual_work_month = @month
                                AND tb1.actual_work_year = @year
                                AND tb1.company_code = @companyCode"
                                , new
                                {
                                    userId = userId,
                                    day = day,
                                    month = month,
                                    year = year,
                                    companyCode = companyCode
                                });
            var result = database.SingleOrDefault<AttendanceRecord>(sql);

            if (result == null)
            {
                result = new AttendanceRecord();
            }

            return result;
        }
        /// <summary>
        /// Get list of member actual work
        /// </summary>
        /// <param name="day">day</param>
        /// <param name="month">month</param>
        /// <param name="year">year</param>
        /// <param name="userId">userId</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List Member Actual Work</returns>
        public IList<MemberActualWorkListPlus> GetMemberActualWorkList(int day, int month, int year, int userId, string companyCode)
        {
            var sql = new Sql(@"SELECT 
                                        tb2.project_sys_id, 
                                        tb3.project_name,
                                        tb2.phase_id,
                                        tb4.display_name,
                                        tb5.[status],
                                        tb5.sales_type,
                                        MAX(actual_work_time) actual_work_time
                                    FROM 
                                    (
                                        SELECT 
                                            tb.company_code,
                                            tb.project_sys_id, 
                                            tb.phase_id,
                                            CASE WHEN 
                                                tb.actual_work_date != @date 
                                                AND tb.actual_work_time != 0 
                                            THEN NULL 
                                            ELSE tb.actual_work_time 
                                            END actual_work_time
                                        FROM
                                        (
                                            SELECT mad.project_sys_id, tp.phase_id, ISNULL(mawd.actual_work_time, 0) actual_work_time, ISNULL(mawd.actual_work_date, 0) actual_work_date, mad.target_year, mad.target_month, mad.company_code
                                            FROM 
                                            member_assignment_detail mad
                                            JOIN target_phase tp 
                                                ON mad.company_code = tp.company_code
                                                AND mad.project_sys_id = tp.project_sys_id
                                            LEFT JOIN member_actual_work_detail mawd
                                                ON mad.company_code = mawd.company_code
                                                AND mad.project_sys_id = mawd.project_sys_id
                                                AND mad.target_year = mawd.actual_work_year
                                                AND mad.target_month = mawd.actual_work_month
                                                AND mad.user_sys_id = mawd.user_sys_id
                                                AND tp.phase_id = mawd.phase_id
                                                AND tp.project_sys_id = mawd.project_sys_id
                                            WHERE mad.user_sys_id = @user_sys_id
                                        ) AS tb
                                        WHERE
                                            tb.target_month = @month
                                            AND tb.target_year = @year
                                            AND tb.company_code = @company_code
                                    ) tb2 
                                    LEFT JOIN project_info tb3 ON tb2.company_code = tb3.company_code
                                    AND tb2.project_sys_id = tb3.project_sys_id
                                    AND tb3.del_flg = @non_del
                                    LEFT JOIN m_phase tb4 ON tb2.company_code = tb4.company_code
                                    AND tb2.phase_id = tb4.phase_id
                                    LEFT JOIN m_status tb5 ON tb5.company_code = tb3.company_code AND tb5.status_id = tb3.status_id
                                    WHERE tb3.del_flg = @non_del
                                    AND ((tb5.sales_type <> '2'
                                    AND tb5.operation_target_flg = '1')
                                    OR (SELECT SUM(actual_work_time) FROM member_actual_work_detail mawd WHERE mawd.user_sys_id = @user_sys_id
                                    AND mawd.project_sys_id = tb2.project_sys_id
                                    AND mawd.actual_work_year = @year
                                    AND mawd.actual_work_month = @month) > 0)
                                    GROUP BY tb2.project_sys_id, tb2.phase_id, tb3.project_name, tb4.display_name, tb5.[status], tb5.sales_type
                                    ORDER BY tb2.project_sys_id, tb2.phase_id, tb3.project_name,tb4.display_name"
                                 , new
                                 {
                                     date = day,
                                     month = month,
                                     year = year,
                                     user_sys_id = userId,
                                     company_code = companyCode,
                                     non_del = Constant.DeleteFlag.NON_DELETE
                                 });

            var result = database.Query<MemberActualWorkListPlus>(sql).ToList();

            return result.ToList();
        }
        /// <summary>
        /// Update to database
        /// </summary>
        /// <param name="dataListMemberActualWorkDetail">dataListMemberActualWorkDetail</param>
        /// <param name="dataMemberActualWork">dataMemberActualWork</param>
        /// <param name="dataAttendanceRecord">dataAttendanceRecord</param>
        /// <returns>bool: true/false</returns>
        public bool PutActualWorkDetailListNew(IList<MemberActualWorkDetail> dataListMemberActualWorkDetail,
            MemberActualWork dataMemberActualWork,
            AttendanceRecord dataAttendanceRecord, out bool isExceedMaximumActualWorkTime)
        {
            var result = false;
            isExceedMaximumActualWorkTime = false;

            if (dataListMemberActualWorkDetail != null)
            {
                foreach (var memberActualWorkDetail in dataListMemberActualWorkDetail)
                {
                    result = PutActualWorkDetailNew(memberActualWorkDetail);

                    if (!result) return false;
                }
            }
            //check current total actual work time not exceed 744 hour
            decimal totalActualWorkTime = GetTotalActualWorkTime(dataMemberActualWork.actual_work_month, dataMemberActualWork.actual_work_year,
                dataMemberActualWork.user_sys_id, dataMemberActualWork.company_code);
            if (totalActualWorkTime > Constant.TOTAL_ACTUAL_WORK_TIME)
            {
                isExceedMaximumActualWorkTime = true;
                return false;
            }

            if (dataMemberActualWork != null)
            {
                this.UpdateMemberActualWorkNew(dataMemberActualWork);
            }

            if (dataAttendanceRecord != null)
            {
                result = UpdateAttendanceRecordNew(dataAttendanceRecord);

                if (!result) return false;
            }

            return true;
        }
        /// <summary>
        /// Update Member Actual Work New
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>bool : true/false</returns>
        private bool UpdateMemberActualWorkNew(MemberActualWork data)
        {
            //UpdateMemberActualWorkNew

            var sql = Sql.Builder.Append(@"declare @@rowcount int;");
            sql.Append(
                @"exec @@rowcount = UpdateMemberActualWorkNew 
                                                @company_code,
                                                @user_sys_id ,
                                                @actual_work_year ,
                                                @actual_work_month,
                                                @ins_date,
                                                @regist_type,
                                                @ins_id,
                                                @upd_date,
                                                @upd_id;",
                                            new
                                            {
                                                company_code = data.company_code,
                                                user_sys_id = data.user_sys_id,
                                                actual_work_year = data.actual_work_year,
                                                actual_work_month = data.actual_work_month,
                                                ins_date = data.ins_date,
                                                regist_type = data.regist_type,
                                                ins_id = data.ins_id,
                                                upd_date = data.upd_date,
                                                upd_id = data.upd_id
                                            });
            sql.Append("SELECT @@rowcount");

            var result = database.ExecuteScalar<int>(sql);
            return result == 1;
        }
        /// <summary>
        /// Update Attendance recode to DB
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>bool:true/false</returns>
        private bool UpdateAttendanceRecordNew(AttendanceRecord data)
        {
            //UpdateMemberActualWork

            var sql = Sql.Builder.Append(@"declare @@rowcount int;");
            sql.Append(
                @"exec @@rowcount = UpdateAttendanceRecordNew 
                                                @company_code,
                                                @user_sys_id ,
                                                @actual_work_year ,
                                                @actual_work_month,
                                                @actual_work_date,
                                                @work_start_time,
                                                @work_end_time,
                                                @clock_in_start_time,
                                                @clock_in_end_time,
                                                @rest_time,
                                                @allowed_cost_time,
                                                @attendance_type_id,
                                                @remarks,
                                                @ins_date,
                                                @ins_id,
                                                @upd_date,
                                                @upd_id;",
                                            new
                                            {
                                                company_code = data.company_code,
                                                user_sys_id = data.user_sys_id,
                                                actual_work_year = data.actual_work_year,
                                                actual_work_month = data.actual_work_month,
                                                actual_work_date = data.actual_work_date,
                                                work_start_time = data.work_start_time,
                                                work_end_time = data.work_end_time,
                                                clock_in_start_time = data.clock_in_start_time,
                                                clock_in_end_time = data.clock_in_end_time,
                                                rest_time = data.rest_time,
                                                allowed_cost_time = data.allowed_cost_time,
                                                attendance_type_id = data.attendance_type_id,
                                                remarks = data.remarks,
                                                ins_date = data.ins_date,
                                                ins_id = data.ins_id,
                                                upd_date = data.upd_date,
                                                upd_id = data.upd_id
                                            });
            sql.Append("SELECT @@rowcount");

            var result = database.ExecuteScalar<int>(sql);
            return result == 1;
        }
        /// <summary>
        /// Insert/Update Actual work detail new
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>bool: true/false</returns>
        private bool PutActualWorkDetailNew(MemberActualWorkDetail data)
        {

            var sql = Sql.Builder.Append(@"declare @@rowcount int;");
            sql.Append(@"exec @@rowcount = PutActualWorkDetailNew
                                                @company_code,
                                                @user_sys_id,
                                                @actual_work_year,
                                                @actual_work_month,
                                                @actual_work_date,
                                                @detail_no,
                                                @project_sys_id,
                                                @phase_id, 
                                                @actual_work_time,
                                                @remark,
                                                @ins_date,
                                                @ins_id,
                                                @upd_date,
                                                @upd_id;",
                                                        new
                                                        {
                                                            company_code = data.company_code,
                                                            user_sys_id = data.user_sys_id,
                                                            actual_work_year = data.actual_work_year,
                                                            actual_work_month = data.actual_work_month,
                                                            actual_work_date = data.actual_work_date,
                                                            detail_no = data.detail_no,
                                                            project_sys_id = data.project_sys_id,
                                                            phase_id = data.phase_id,
                                                            actual_work_time = data.actual_work_time,
                                                            remark = string.Empty,
                                                            ins_date = data.ins_date,
                                                            ins_id = data.ins_id,
                                                            upd_date = data.upd_date,
                                                            upd_id = data.upd_id
                                                        });
            sql.Append(@"SELECT @@rowcount");

            var result = database.ExecuteScalar<int>(sql);

            return result == 1;
        }
        #endregion

        #region ActualWorkListByIndividualPhase methods
        /// <summary>
        /// Get Info of Actual work list by individual phase
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="phaseByContracts">List Phase ID</param>
        /// <returns>List info of actual work</returns>
        public PageInfo<dynamic> GetActualWorkListByIndividualPhase(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            Condition condition,
            List<PhaseByContract> phaseByContracts,
            string userIds
        )
        {
            var sql = buildSelectQueryListByIndividualPhase(condition, phaseByContracts, 0, sortDir, false, userIds);
            Mappers.Revoke(typeof(System.Dynamic.ExpandoObject));
            var pageInfo = Page<dynamic>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get info of work list by individual phase
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="phaseByContracts">List Phase ID</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <returns>List info of work</returns>
        public IList<dynamic> GetWorkListByIndividualPhaseExport(Condition condition, List<PhaseByContract> phaseByContracts, int sort_column, string sort_type)
        {
            var sql = this.buildSelectQueryListByIndividualPhase(condition, phaseByContracts, sort_column, sort_type, true, null);
            return this.database.Fetch<dynamic>(sql);
        }


        /// <summary>
        /// SQL Query get info of actual work
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="sort_column">sort_column</param>
        /// <param name="sort_type">sort_type</param>
        /// <param name="isExport">isExport</param>
        /// <returns>SQL query</returns>
        private Sql buildSelectQueryListByIndividualPhase(Condition condition, List<PhaseByContract> phaseByContracts, int sort_column, string sort_type, bool isExport = false, string userIds = null)
        {
            string cols = this.BuildColumnNames(condition.StartMonth, condition.EndMonth);

            var sb = new StringBuilder();

            sb.Append(@" SELECT ");
            if (isExport)
            {
                string sort_column_name = string.Empty;
                switch (sort_column)
                {
                    case 2:
                        sort_column_name = "group_name";
                        break;
                    case 3:
                        sort_column_name = "user_name";
                        break;
                    case 4:
                        sort_column_name = "location_name";
                        break;
                    case 5:
                        sort_column_name = "project_name";
                        break;
                    case 6:
                        sort_column_name = "contract_type";
                        break;
                    case 7:
                        sort_column_name = "phase_name";
                        break;
                    default:
                        sort_column_name = "user_sys_id";
                        break;
                }
                sb.Append(@"
                    ROW_NUMBER() OVER (order by " + sort_column_name + " " + sort_type + @") [No], 
                    group_name,
                    user_name,
                    location_name,
                    project_name,
					contract_type,
					phase_name,
                ");
            }
            else
            {
                sb.Append(@"
                    group_name,
                    user_name,
                    user_sys_id,
                    location_name,
                    project_sys_id,
                    project_name,
					contract_type,
					phase_name, ");
            }
            sb.Append(@"
            " + cols + @"
        FROM
            (
                SELECT
                    result.group_name AS group_name,
                    result.user_name AS user_name,
                    result.user_sys_id AS user_sys_id,
                    result.location_name AS location_name,
                    result.project_sys_id AS project_sys_id,
                    result.project_name AS project_name,
					result.contract_type AS contract_type,
					result.phase_name AS phase_name,
                    dbo.Date2String(result.target_month, result.target_year) AS [Month],");
            //if (condition.Filterable == 1)
            //{
            //    sb.Append(@"
            //        CASE WHEN ((SELECT COUNT(DISTINCT mawd.actual_work_date) FROM member_actual_work_detail mawd
            //            INNER JOIN project_info pi
            //            ON mawd.project_sys_id = pi.project_sys_id
            //            LEFT JOIN (
            //                SELECT ar.* FROM attendance_record ar
            //                INNER JOIN m_attendance_type mat
            //                ON ar.company_code = mat.company_code
            //                AND ar.attendance_type_id = mat.attendance_type_id
            //                WHERE mat.non_operational_flg = '0'
            //                AND mat.del_flg = '0'
            //            ) as arTbl
            //            ON mawd.user_sys_id = arTbl.user_sys_id
            //            AND mawd.company_code = arTbl.company_code 
            //            AND mawd.actual_work_year = arTbl.actual_work_year
            //            AND mawd.actual_work_month = arTbl.actual_work_month
            //            AND mawd.actual_work_date = arTbl.actual_work_date
            //            WHERE mawd.user_sys_id = result.user_sys_id
            //            AND mawd.actual_work_year =  result.target_year
            //            AND mawd.actual_work_month = result.target_month
            //            AND pi.del_flg = 0
            //            AND mawd.actual_work_time <> 0
            //            AND CHARINDEX('@'+CONVERT(nvarchar(MAX),mawd.actual_work_date)+'@',days) <> 0)
            //            = (SELECT COUNT(*) FROM dbo.[Split](days,','))
            //            OR (days ='')
            //            OR (
            //                (SELECT COUNT(*) FROM member_assignment_detail as mad
            //                    INNER JOIN (SELECT pi.* FROM project_info pi
            //                          INNER JOIN m_status ms
            //                          ON pi.status_id = ms.status_id
            //                          AND pi.company_code = ms.company_code
            //                          WHERE ms.company_code = @company_code
            //                          AND ms.operation_target_flg = '1' 
            //                          AND ms.sales_type <> '2'
            //                          AND pi.del_flg = '0'
            //                      ) AS prjTbl
            //                          ON mad.project_sys_id = prjTbl.project_sys_id
            //                          WHERE mad.company_code = @company_code
            //                          AND mad.target_year = result.target_year
            //                          AND mad.target_month = result.target_month
            //                          AND mad.user_sys_id = result.user_sys_id
            //                ) = 0
            //            ))
            //            THEN CONCAT(result.total_work,'(1)') 
            //            ELSE CONCAT(result.total_work,'(0)') 
            //        END AS total_work
            //    ");
            //}
            //else
            //{
            //    sb.Append(@"
            //        result.total_work AS total_work
            //    ");
            //}

            sb.Append(@"
                    result.total_work AS total_work
                ");

            sb.Append(@"
                FROM
                    (
                        SELECT
                            group_name,
                            user_name,
                            user_sys_id,
                            location_name,
                            project_sys_id,
                            project_name,
							contract_type,
							phase_name,
                            target_month, 
                            target_year, ");
            if (condition.WorkTimeUnit == Constant.TimeUnit.HOUR)
            {
                sb.Append(@"
                     CONCAT(SUM(dbo.RemoveRoundDecimal(actual_work_time)),'(',ISNULL(regist_type,0),')') AS total_work, ");
            }
            else
            {
                sb.Append(@"
                     CONCAT(SUM(actual_work_time),'(',ISNULL(regist_type,0),')') AS total_work, ");
            }
            sb.Append(@"
                            (SELECT dbo.[GetWorkDaysInMonth](@company_code, user_sys_id, target_month, target_year)) AS days
                        FROM
                            (
                                SELECT
                                    mu.company_code,
                                    (SELECT display_name FROM m_group WHERE group_id =
                                        (SELECT TOP(1) group_id
                                        FROM enrollment_history
                                        WHERE company_code = mu.company_code
                                        AND user_sys_id = mu.user_sys_id
                                        ORDER BY actual_work_year DESC, actual_work_month DESC)) AS group_name,
                                    mu.display_name AS user_name,
                                    mu.user_sys_id,
                                    location.display_name AS location_name,
                                    project.project_sys_id,
                                    project.project_name,
                                    [contract].contract_type_id,
									[contract].contract_type,
                                    actual.phase_id,
									actual.phase_name,
                                    assign.target_year,
                                    assign.target_month,
                                    CASE WHEN assign.project_sys_id IN (SELECT project_info.project_sys_id FROM project_info INNER JOIN m_status ON project_info.status_id = m_status.status_id AND project_info.company_code = m_status.company_code WHERE m_status.sales_type = '2' OR m_status.operation_target_flg <> '1') 
                                        THEN 0
                                        ELSE ");
            switch (condition.WorkTimeUnit)
            {
                case Constant.TimeUnit.HOUR:
                    sb.Append(@" dbo.Day2Hour(@company_code, assign.plan_man_days) ");
                    break;
                case Constant.TimeUnit.DAY:
                    sb.Append(@" assign.plan_man_days ");
                    break;
                case Constant.TimeUnit.MONTH:
                    sb.Append(@" dbo.Day2Month(assign.company_code, assign.target_month, assign.target_year, assign.plan_man_days) ");
                    break;
            }

            sb.Append(@"
                            END AS plan_effort,
                            ISNULL(actual.actual_work_time, 0) AS actual_work_time,
                            (
                                SELECT
                                    regist_type
                                FROM
                                    member_actual_work
                                WHERE
                                        company_code = assign.company_code
                                    AND user_sys_id = assign.user_sys_id
                                    AND actual_work_year = assign.target_year
                                    AND actual_work_month = assign.target_month
                            ) AS regist_type
                        FROM m_user AS mu 
                            INNER JOIN
                            member_assignment_detail AS assign
                            ON
                                assign.company_code = mu.company_code
                            AND assign.user_sys_id = mu.user_sys_id ");
            //if (userIds != null)
            //{
            //    sb.Append(" AND assign.user_sys_id IN (" + userIds.ToString() + ")");
            //}

            if (!condition.DELETED_INCLUDE)
            {
                sb.Append(" AND mu.del_flg = '0' ");
            }

            if (!condition.RETIREMENT_INCLUDE)
            {
                sb.Append(@" AND (mu.retirement_date IS NULL
                            OR  @CurrentDate <= mu.retirement_date) ");
            }

            sb.Append(@"
                        INNER JOIN
                            project_info AS project
                        ON
                                assign.company_code = project.company_code
                            AND assign.project_sys_id = project.project_sys_id
                            AND project.del_flg = '0'
                        LEFT OUTER JOIN m_business_location location
						ON 
								mu.company_code = location.company_code
							AND mu.location_id = location.location_id
                        INNER JOIN 
                            m_contract_type [contract]
						ON
								[contract].company_code = project.company_code
							AND [contract].contract_type_id = project.contract_type_id
                        LEFT OUTER JOIN
                            (
                                SELECT
                                    mawd.company_code,
                                    mawd.project_sys_id,
                                    phase.phase_id,
                                    phase.display_name AS phase_name,
                                    mawd.user_sys_id,
                                    actual_work_year,
                                    actual_work_month, ");
            switch (condition.WorkTimeUnit)
            {
                case Constant.TimeUnit.HOUR:
                    sb.Append(@"
                                    SUM(dbo.RemoveRoundDecimal(mawd.actual_work_time)) AS actual_work_time ");
                    break;
                case Constant.TimeUnit.DAY:
                    sb.Append(@"
                                    dbo.Hour2Day(mawd.company_code, SUM(dbo.RemoveRoundDecimal(actual_work_time))) AS actual_work_time ");
                    break;
                case Constant.TimeUnit.MONTH:
                    sb.Append(@"
                                    dbo.Hour2Month(mawd.company_code, actual_work_month, actual_work_year, SUM(dbo.RemoveRoundDecimal(actual_work_time))) AS actual_work_time ");
                    break;
            }

            sb.Append(@"
                                FROM
                                    target_phase tgp left outer join member_actual_work_detail mawd
                                    ON
									        tgp.company_code = mawd.company_code
									    AND tgp.project_sys_id = mawd.project_sys_id
									    AND tgp.phase_id = mawd.phase_id
                                    inner join m_phase phase
									ON 
                                            tgp.company_code = phase.company_code
									    AND tgp.phase_id = phase.phase_id
                                WHERE mawd.actual_work_time <> @actual_work_time
                                GROUP BY
                                    mawd.company_code,
                                    mawd.project_sys_id,
                                    mawd.user_sys_id,
                                    phase.phase_id,
									phase.display_name,
                                    actual_work_year,
                                    actual_work_month
                            ) AS actual
                        ON
                                assign.company_code = actual.company_code
                            AND assign.project_sys_id = actual.project_sys_id
                            AND assign.user_sys_id = actual.user_sys_id
                            AND assign.target_year = actual.actual_work_year
                            AND assign.target_month = actual.actual_work_month");
            //if (userIds != null)
            //{
            //    sb.Append(" AND assign.user_sys_id IN (" + userIds.ToString() + ")");
            //}
            sb.Append(@"
                        WHERE
                                assign.company_code =  @company_code
                            AND CONVERT(varchar,dbo.Date2Number(assign.target_month, assign.target_year)) BETWEEN  @fromDate AND  @toDate 
                            AND phase_id IS NOT NULL ");

            if (!string.IsNullOrEmpty(condition.DisplayName))
            {
                sb.AppendFormat(@"
                        AND (mu.display_name LIKE N'%{0}%' ESCAPE '\' {1} 
                            OR mu.user_name_sei LIKE N'%{0}%' ESCAPE '\' {1} 
                            OR mu.user_name_mei LIKE N'%{0}%' ESCAPE '\' {1} 
                            OR mu.furigana_sei LIKE N'%{0}%' ESCAPE '\' {1} 
                            OR mu.furigana_mei LIKE N'%{0}%' ESCAPE '\' {1})
                ", replaceWildcardCharacters(condition.DisplayName), "COLLATE " + Constant.SQL_COLLATION_LATIN);
            }
            if (condition.GroupId != null)
            {
                sb.AppendFormat(" AND mu.group_id = {0} ", condition.GroupId.Value);
            }

            if (!string.IsNullOrEmpty(condition.PROJECT_NAME))
            {
                sb.AppendFormat(@" AND project.project_name LIKE '%{0}%' ESCAPE '\'", replaceWildcardCharacters(condition.PROJECT_NAME));
            }

            if (!string.IsNullOrEmpty(condition.LOCATION_ID))
            {
                sb.AppendFormat(" AND mu.location_id IN ({0}) ", condition.LOCATION_ID);
            }

            if (!string.IsNullOrEmpty(condition.CONTRACT_TYPE_ID) || phaseByContracts.Count > 0)
            {
                if (phaseByContracts.Count > 0)
                {
                    sb.Append(" AND (");

                    var isFirst = true;
                    foreach (var item in phaseByContracts.GroupBy(x => x.ContractTypeId).Select(e => e.First()))
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                        }
                        else
                        {
                            sb.Append(" OR ");
                        }

                        var phaseId = string.Join(",", phaseByContracts.Where(x => x.ContractTypeId == item.ContractTypeId).Select(e => e.PhaseId.Value));

                        sb.AppendFormat("([contract].contract_type_id = {0} AND actual.phase_id IN ({1})) ", item.ContractTypeId, phaseId);
                    }
                    sb.Append(") ");

                }
                else
                {
                    sb.AppendFormat(" AND [contract].contract_type_id IN ({0}) ", condition.CONTRACT_TYPE_ID);
                }

            }

            sb.Append(@"
                    ) AS tbResult
                GROUP BY
                    company_code,
                    group_name,
                    user_name,
                    user_sys_id,
                    location_name,
                    project_sys_id,
                    project_name,
					contract_type,
					phase_name,
                    target_year,
                    target_month,
                    regist_type
                )AS result
            ) AS R pivot (MAX(total_work) for Month IN (" + cols + ")) AS P ");

            Sql sql = new Sql(sb.ToString(),
               new { company_code = condition.CompanyCode },
               new { displayName = condition.DisplayName },
               new { fromDate = this.Date2Number(condition.StartMonth) },
               new { toDate = this.Date2Number(condition.EndMonth) },
               new { CurrentDate = Utility.GetCurrentDateTime() },
               new { actual_work_time = ACTUAL_WORK_TIME_ZERO }
            );

            return sql;
        }

        /// <summary>
        /// Get phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeId">Contract type ID</param>
        /// <returns>List of phase</returns>
        public IList<PhasePlus> GetPhaseList(string companyCode, string[] contractTypeId)
        {
            var sb = new StringBuilder();
            sb.Append(@"
            SELECT
                tb3.contract_type_id,
				tb3.contract_type,
                tb1.phase_id,
                tb1.phase_name,
                tb1.display_name
            FROM
                m_phase tb1 INNER JOIN r_contract_type_phase tb2 
                    ON tb1.company_code = tb2.company_code
                    AND tb1.phase_id = tb2.phase_id 
                    INNER JOIN m_contract_type tb3 
					ON tb2.company_code = tb3.company_code 
					AND tb2.contract_type_id = tb3.contract_type_id
            WHERE tb1.company_code = @company_code ");

            if (contractTypeId != null)
            {
                sb.Append(@" 
                    AND tb2.contract_type_id IN (@contract_type_id) ");
            }

            sb.Append(@"AND tb1.del_flg = @del_flg 
                        GROUP BY tb3.contract_type_id, tb3.contract_type, tb1.phase_id, tb1.phase_name, tb1.display_name, tb3.display_order, tb2.display_order
			            ORDER BY tb3.display_order, tb3.contract_type_id, tb2.display_order");

            var sql = new Sql(sb.ToString(),
                 new
                 {
                     company_code = companyCode,
                     contract_type_id = contractTypeId,
                     del_flg = Constant.DeleteFlag.NON_DELETE
                 });

            return this.database.Fetch<PhasePlus>(sql);
        }
        #endregion

        /// <summary>
        /// Get Adjustment Time 
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="attendance_type_id">attendance_type_id</param>
        /// <returns>Adjustment Time </returns>
        public decimal GetAdjustmentTime(string company_code, int attendance_type_id)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT
                    adjustment_time
                FROM
                    m_attendance_type
                WHERE company_code = @company_code
                    AND attendance_type_id = @attendance_type_id"
           , new
           {
               company_code = company_code,
               attendance_type_id = attendance_type_id,
           });

            return database.SingleOrDefault<decimal>(sql);
        }

        /// <summary>
        /// Get Total Actual Work Time
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="userId"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public decimal GetTotalActualWorkTime(int month, int year, int userId, string companyCode)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT
                    ISNULL(SUM(actual_work_time),0)
                FROM
                    member_actual_work_detail
                WHERE company_code = @company_code
                    AND user_sys_id = @user_sys_id
                    AND actual_work_year = @actual_work_year
                    AND actual_work_month = @actual_work_month"
           , new
           {
               company_code = companyCode,
               user_sys_id = userId,
               actual_work_year = year,
               actual_work_month = month
           });

            return database.SingleOrDefault<decimal>(sql);
        }
    }
}