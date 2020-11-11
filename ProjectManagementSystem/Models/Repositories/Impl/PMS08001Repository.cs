#region License
/// <copyright file="PMS08001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/20</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS08001;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Information screen repository class
    /// </summary>
    public class PMS08001Repository : Repository, IPMS08001Repository
    {
        /// <summary>
        /// Database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS08001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS08001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// GetCheckDateStart
        /// </summary>
        /// <param name="checkDayofWeek"></param>
        /// <param name="todayofWeek"></param>
        /// <returns></returns>
        public string GetCheckDateStart(int checkDayofWeek, int todayofWeek)
        {
            int backStepNumber;
            if(checkDayofWeek > @todayofWeek)
            {
                backStepNumber = 14;
            }
            else
            {
                backStepNumber = 7;
            }

            var sql = new Sql(
                @"
                    SELECT CONVERT(varchar, DATEADD(dw, -@backStepNumber -(@todayofWeek - @checkDayofWeek), GETDATE()), 111) AS checkDateStart;
                ",
                new
                {
                    checkDayofWeek = checkDayofWeek,
                    todayofWeek = todayofWeek,
                    backStepNumber = backStepNumber
                });

            return this._database.FirstOrDefault<string>(sql);
        }

        /// <summary>
        /// GetCheckDateEnd
        /// </summary>
        /// <param name="checkDayofWeek"></param>
        /// <param name="todayofWeek"></param>
        /// <returns></returns>
        public string GetCheckDateEnd(int checkDayofWeek, int todayofWeek)
        {
            int backStepNumber;
            if (checkDayofWeek > @todayofWeek)
            {
                backStepNumber = 8;
            }
            else
            {
                backStepNumber = 1;
            }

            var sql = new Sql(
                @"
                    SELECT CONVERT(varchar, DATEADD(dw, -@backStepNumber -(@todayofWeek - @checkDayofWeek), GETDATE()), 111) AS checkDateEnd;
                ",
                new
                {
                    checkDayofWeek = checkDayofWeek,
                    todayofWeek = todayofWeek,
                    backStepNumber = backStepNumber
                });

            return this._database.FirstOrDefault<string>(sql);
        }

        /// <summary>
        /// GetCheckPointWeek
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public int GetCheckPointWeek(string cCode)
        {
            var sql = new Sql(
                @"
                    SELECT check_point_week 
                    FROM m_company_setting
                    WHERE company_code = @company_code
                ",
                new
                {
                    company_code = cCode
                });

            return this._database.FirstOrDefault<int>(sql);
        }


        /// <summary>
        /// GetTodayWeek
        /// </summary>
        /// <returns></returns>
        public int GetTodayWeek()
        {
            var sql = new Sql(
                @"
                    SELECT DATEPART(dw,GETDATE())
                ");

            return this._database.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// PlanList Check 
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IList<ProjectInfor> CheckPlanList(string cCode)
        {
            var sql = new Sql(
                @"
                SELECT  pi.project_name
                        , pi.project_sys_id
                        , ISNULL((SELECT display_name FROM m_group WHERE group_id =
                            (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = @company_code
                                    AND user_sys_id = mu.user_sys_id
                                    ORDER BY actual_work_year DESC, actual_work_month DESC)),'所属無し') AS group_name
                        , mu.display_name as person_in_charge
                FROM project_info pi
                LEFT JOIN project_plan_info pli
                    ON pi.company_code = pli.company_code
                    AND pi.project_sys_id = pli.project_sys_id
                LEFT JOIN m_user mu
                    ON pi.company_code = mu.company_code
                    AND pi.charge_person_id = mu.user_sys_id
                WHERE pi.company_code = @company_code
                    AND (pli.project_sys_id IS NULL OR pli.del_flg = '1')
                    AND pi.del_flg = '0'
                    AND pi.status_id IN (SELECT status_id FROM m_status
                                         WHERE status_check_flg = '1' 
                                         AND company_code = @company_code)
                ORDER BY mu.group_id ASC, mu.display_name ASC, mu.user_sys_id ASC
                ",
                new
                {
                    company_code = cCode
                });

            return this._database.Fetch<ProjectInfor>(sql);
        }

        /// <summary>
        /// CheckPeriodList
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IList<ProjectInfor> CheckPeriodList(string cCode)
        {
            var sql = new Sql(
                @"
                SELECT  pi.project_name
                        , pi.project_sys_id
                        , ISNULL((SELECT display_name FROM m_group WHERE group_id =
                            (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = @company_code
                                    AND user_sys_id = mu.user_sys_id
                                    ORDER BY actual_work_year DESC, actual_work_month DESC)),'所属無し') AS group_name
                        , mu.display_name as person_in_charge
                FROM project_info pi
                INNER JOIN m_contract_type mct
                    ON pi.company_code = mct.company_code
                    AND pi.contract_type_id = mct.contract_type_id
                LEFT JOIN m_user mu
                    ON pi.company_code = mu.company_code
                    AND pi.charge_person_id = mu.user_sys_id
                WHERE pi.company_code = @company_code
                    AND pi.del_flg = '0'
                    AND pi.end_date < CAST(getdate() AS date)
                    AND pi.status_id IN (SELECT status_id FROM m_status
                                         WHERE status_check_flg = '1' 
                                         AND company_code = @company_code)
                    AND mct.check_period_flg = '1'
                ORDER BY mu.group_id ASC, mu.display_name ASC, mu.user_sys_id ASC
                ",
                new
                {
                    company_code = cCode
                });

            return this._database.Fetch<ProjectInfor>(sql);
        }

        /// <summary>
        /// CheckSalesList
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IList<ProjectInfor> CheckSalesList(string cCode)
        {
            var sql = new Sql(
                @"
                SELECT  pi.project_name
                        , pi.project_sys_id
                        , ISNULL((SELECT display_name FROM m_group WHERE group_id =
                            (SELECT TOP(1) group_id
                                    FROM enrollment_history
                                    WHERE company_code = @company_code
                                    AND user_sys_id = mu.user_sys_id
                                    ORDER BY actual_work_year DESC, actual_work_month DESC)),'所属無し') AS group_name
                        , mu.display_name as person_in_charge
                FROM project_info pi
                INNER JOIN m_contract_type mct
                    ON pi.company_code = mct.company_code
                    AND pi.contract_type_id = mct.contract_type_id
                LEFT JOIN m_user mu
                    ON pi.company_code = mu.company_code
                    AND pi.charge_person_id = mu.user_sys_id
                WHERE pi.company_code = @company_code
                    AND pi.del_flg = '0'
                    AND pi.status_id IN (SELECT status_id FROM m_status
                                            WHERE sales_type = '1' AND operation_target_flg = '0' AND company_code = @company_code)
                    AND (SELECT SUM(actual_work_time) FROM member_actual_work_detail mawd WHERE mawd.project_sys_id = pi.project_sys_id) > 0
                    AND mct.check_sales_flg = '1'
                ORDER BY mu.group_id ASC, mu.display_name ASC, mu.user_sys_id ASC
                ",
                new
                {
                    company_code = cCode
                });

            return this._database.Fetch<ProjectInfor>(sql);
        }

        /// <summary>
        /// CheckProgressList
        /// </summary>
        /// <param name="cCode"></param>
        /// <param name="progressUpdatedList"></param>
        /// <param name="checkDateStart"></param>
        /// <returns></returns>
        public IList<ProjectInfor> CheckProgressList(string cCode, string progressUpdatedList, string checkDateStart)
        {
            var sql = new Sql(
                @"
                SELECT  pi.project_name
                        , pi.project_sys_id
                        , ISNULL((SELECT display_name FROM m_group WHERE group_id =
                            (SELECT TOP(1) group_id
                                        FROM enrollment_history
                                        WHERE company_code = @company_code
                                        AND user_sys_id = mu.user_sys_id
                                        ORDER BY actual_work_year DESC, actual_work_month DESC)),'所属無し') AS group_name
                        , mu.display_name as person_in_charge
                FROM project_info pi
                INNER JOIN (SELECT company_code,contract_type_id,check_progress_flg FROM m_contract_type
                                                WHERE company_code = @company_code) as mctTbl
                    ON pi.company_code = mctTbl.company_code
                    AND pi.contract_type_id = mctTbl.contract_type_id
                LEFT JOIN m_user mu
                    ON pi.company_code = mu.company_code
                    AND pi.charge_person_id = mu.user_sys_id
                WHERE pi.company_code = @company_code
                    AND pi.del_flg = '0'
                    AND pi.status_id IN (SELECT status_id FROM m_status
                                         WHERE status_check_flg = '1' 
                                         AND company_code = @company_code)
                    AND pi.start_date < @checkDateStart
                    AND mctTbl.check_progress_flg = '1'
                    AND ISNULL((SELECT TOP(1) progress
                                    FROM progress_history
                                    WHERE company_code = @company_code
                                    AND project_sys_id = pi.project_sys_id
                                    ORDER BY regist_date DESC), 0) <> 1
                ",
                new
                {
                    company_code = cCode,
                    checkDateStart = checkDateStart
                });
            if (progressUpdatedList != string.Empty)
            {
                sql.Append(@"
                    AND pi.project_sys_id NOT IN 
                    " + progressUpdatedList
                );
            }

            sql.Append(@"
                ORDER BY mu.group_id ASC, mu.display_name ASC, mu.user_sys_id ASC
            ");

            return this._database.Fetch<ProjectInfor>(sql);
        }

        /// <summary>
        /// GetProgressUpdatedProjectList
        /// </summary>
        /// <param name="cCode"></param>
        /// <param name="checkDateStart"></param>
        /// <param name="checkDateEnd"></param>
        /// <returns></returns>
        public IList<int> GetProgressUpdatedProjectList(string cCode, string checkDateStart, string checkDateEnd)
        {
            var sql = new Sql(
               @"
                    SELECT project_sys_id FROM progress_history 
                          WHERE company_code = @company_code AND regist_date BETWEEN @checkDateStart AND @checkDateEnd
                ",
                new
                {
                    company_code = cCode,
                    checkDateStart = checkDateStart,
                    checkDateEnd = checkDateEnd
                });

            return this._database.Fetch<int>(sql);
        }

        /// <summary>
        /// GetInformationList
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IList<Information> GetInformationList(string cCode)
        {
            var sql = new Sql(
                @"
                SELECT
                    TOP (10) *
                FROM
                    information
                WHERE
                    company_code = @company_code
                    AND CONVERT(date, @CurrentDate) BETWEEN publish_start_date AND publish_end_date
                    AND del_flg = @del_flg
                ORDER BY
                    publish_start_date DESC
                ",
                new
                {
                    company_code = cCode,
                    CurrentDate = Utility.GetCurrentDateTime(),
                    del_flg = Constant.DeleteFlag.NON_DELETE
                });

            return this._database.Fetch<Information>(sql);
        }

        /// <summary>
        /// GetActualWorkData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<MemberActualWorkDetail> GetActualWorkData(string companyCode)
        {
            var sql = new Sql(
                @"
                SELECT mawd.* FROM member_actual_work_detail mawd
                INNER JOIN member_assignment_detail mad
                        ON mawd.company_code = mad.company_code
                        AND mawd.user_sys_id = mad.user_sys_id
                        AND mawd.project_sys_id = mad.project_sys_id
                        AND mawd.actual_work_year = mad.target_year
                        AND mawd.actual_work_month = mad.target_month
                INNER JOIN (SELECT pi.* FROM project_info pi
                                    INNER JOIN m_status ms
                                    ON pi.status_id = ms.status_id
                                    AND pi.company_code = ms.company_code
                                    WHERE ms.company_code = @company_code
                                    AND pi.del_flg = '0'
                            ) as prjTbl
                            ON mawd.project_sys_id = prjTbl.project_sys_id
                 INNER JOIN (
                            SELECT *
                            FROM dbo.GetWorkDayInCurrentWeek(@company_code)
                            ) AS tbWorkDay
                            ON mawd.company_code = tbWorkDay.company_code
                            AND mawd.actual_work_year = tbWorkDay.target_year
                            AND mawd.actual_work_month = tbWorkDay.target_month
                            AND mawd.actual_work_date = tbWorkDay.target_date
                            WHERE mawd.company_code = @company_code
                            AND mawd.actual_work_time <> 0
                ",
                new
                {
                    company_code = companyCode
                });
            return this._database.Fetch<MemberActualWorkDetail>(sql);
        }

        /// <summary>
        /// GetAttendanceData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<AttendanceRecordPlus> GetAttendanceData(string companyCode)
        {
            var sql = new Sql(
                @"
                SELECT ar.*,mat.non_operational_flg FROM attendance_record ar
                    INNER JOIN m_attendance_type mat
                        ON ar.company_code = mat.company_code
                        AND ar.attendance_type_id = mat.attendance_type_id
                    INNER JOIN (
                        SELECT *
                        FROM dbo.GetWorkDayInCurrentWeek(@company_code)
                    ) AS tbWorkDay
                        ON ar.company_code = tbWorkDay.company_code
                        AND ar.actual_work_year = tbWorkDay.target_year
                        AND ar.actual_work_month = tbWorkDay.target_month
                        AND ar.actual_work_date = tbWorkDay.target_date
                    WHERE ar.company_code = @company_code
                ",
                new
                {
                    company_code = companyCode
                });
            return this._database.Fetch<AttendanceRecordPlus>(sql);
        }

        /// <summary>
        /// GetDayList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<WorkingDay> GetDayList(string companyCode)
        {
            var sql = new Sql(
                @"
                    SELECT *
                        FROM dbo.GetWorkDayInCurrentWeek(@company_code)
                ",
                new
                {
                    company_code = companyCode
                });
            return this._database.Fetch<WorkingDay>(sql);
        }

        /// <summary>
        /// GetUserList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<UserInfor> GetUserList(string companyCode)
        {
            var sql = new Sql(
                @"
                    SELECT DISTINCT mu.user_sys_id AS USER_ID
                    ,mu.display_name AS USER_NAME
                    ,(SELECT TOP(1) group_id
                                        FROM enrollment_history
                                        WHERE company_code = @company_code
                                        AND user_sys_id = mu.user_sys_id
                                        ORDER BY actual_work_year DESC, actual_work_month DESC) AS GROUP_ID
                    ,(SELECT MONTH(GETDATE())) AS SELECTED_MONTH, (SELECT YEAR(GETDATE())) AS SELECTED_YEAR
                    FROM m_user mu
                    INNER JOIN ( 
                        SELECT mad.* FROM member_assignment_detail mad
                        INNER HASH JOIN (
                            SELECT *
                            FROM dbo.GetWorkDayInCurrentWeek(@company_code)
                        ) AS tbWorkDay
                        ON mad.company_code = tbWorkDay.company_code
                        AND mad.target_year = tbWorkDay.target_year
                        AND mad.target_month = tbWorkDay.target_month) AS madTbl
                    ON mu.company_code = madTbl.company_code
                    AND mu.user_sys_id = madTbl.user_sys_id
                    INNER JOIN (SELECT pi.* FROM project_info pi
                        INNER JOIN m_status ms
                        ON pi.status_id = ms.status_id
                        AND pi.company_code = ms.company_code
                        WHERE ms.company_code = @company_code
                        AND ms.operation_target_flg = '1' 
                        AND ms.sales_type <> '2'
                        AND pi.del_flg = '0'
                    ) AS prjTbl
                        ON madTbl.project_sys_id = prjTbl.project_sys_id
                    WHERE mu.company_code = @company_code
                    AND mu.temporary_retirement_flg = '0'
                    AND mu.del_flg = '0'
                    AND (mu.retirement_date IS NULL
                        OR GETDATE() <= mu.retirement_date)
                ",
                new
                {
                    company_code = companyCode
                });
            return this._database.Fetch<UserInfor>(sql);
        }

        /// <summary>
        /// GetMemberAssignmentData
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<MemberAssignmentDetail> GetMemberAssignmentData(string companyCode)
        {
            var sql = new Sql(
                @"SELECT DISTINCT mad.* FROM member_assignment_detail mad
                INNER JOIN (SELECT pi.* FROM project_info pi
                                    INNER JOIN m_status ms
                                    ON pi.status_id = ms.status_id
                                    AND pi.company_code = ms.company_code
                                    WHERE ms.company_code = @company_code
                                    AND pi.del_flg = '0'
                                    AND ms.operation_target_flg = '1' 
                                    AND ms.sales_type <> '2'
                            ) as prjTbl
                            ON mad.project_sys_id = prjTbl.project_sys_id
                 INNER JOIN (
                            SELECT *
                            FROM dbo.GetWorkDayInCurrentWeek(@company_code)
                            ) AS tbWorkDay
                            ON mad.company_code = tbWorkDay.company_code
                            AND mad.target_year = tbWorkDay.target_year
                            AND mad.target_month = tbWorkDay.target_month
                            WHERE mad.company_code = @company_code
                ",
                new
                {
                    company_code = companyCode
                });
            return this._database.Fetch<MemberAssignmentDetail>(sql);
        }

        /// <summary>
        /// GetGroupList
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IList<Group> GetGroupList(string companyCode)
        { 
            var sql = new Sql(
                @"SELECT * FROM m_group 
                    WHERE company_code = @company_code
                    AND check_actual_work_flg = '1'
                    AND del_flg = '0'
                ",
                new
                {
                    company_code = companyCode
                });
            return this._database.Fetch<Group>(sql);
        }

        /// <summary>
        /// Get number of contract type for check plan of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetContractTypeCountForCheckPlan(string companyCode)
        {
            var sql = new Sql(
               @"SELECT COUNT(contract_type_id) FROM m_contract_type
                    WHERE company_code = @company_code
                    AND check_plan_flg = '1'
                    AND del_flg = '0' 
                ",
               new
               {
                   company_code = companyCode
               });
            return this._database.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// Get number of contract type for check progress of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetContractTypeCountForCheckProgress(string companyCode)
        {
            var sql = new Sql(
               @"SELECT COUNT(contract_type_id) FROM m_contract_type
                    WHERE company_code = @company_code
                    AND check_progress_flg = '1'
                    AND del_flg = '0' 
                ",
               new
               {
                   company_code = companyCode
               });
            return this._database.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// Get number of contract type for check period of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetContractTypeCountForCheckPeriod(string companyCode)
        {
            var sql = new Sql(
               @"SELECT COUNT(contract_type_id) FROM m_contract_type
                    WHERE company_code = @company_code
                    AND check_period_flg = '1'
                    AND del_flg = '0' 
                ",
               new
               {
                   company_code = companyCode
               });
            return this._database.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// Get number of contract type for check sales of project
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetContractTypeCountForCheckSales(string companyCode)
        {
            var sql = new Sql(
               @"SELECT COUNT(contract_type_id) FROM m_contract_type
                    WHERE company_code = @company_code
                    AND check_sales_flg = '1'
                    AND del_flg = '0' 
                ",
               new
               {
                   company_code = companyCode
               });
            return this._database.FirstOrDefault<int>(sql);
        }
    }
}
