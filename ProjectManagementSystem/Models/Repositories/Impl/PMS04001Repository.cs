#region License
/// <copyright file="PMS04001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/20</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS04001;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Phase repository class
    /// </summary>
    public class PMS04001Repository : Repository, IPMS04001Repository
    {
        #region Constructor
        /// <summary>
        /// Database contractor
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS04001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS04001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Public
        /// <summary>
        /// Search phase by condition
        /// </summary>
        /// <param name="startItem">Start item</param>
        /// <param name="itemsPerPage">Item per page</param>
        /// <param name="columns">List of colum name</param>
        /// <param name="sortCol">Sort by colum</param>
        /// <param name="sortDir">Sort type</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<PhasePlus> Search(int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition)
        {
            var sql = new Sql("SELECT * FROM ( ");

            sql.Append(BuildSqlPhaseList(companyCode, condition));
            sql.Append(") AS tbResult ");

            if (sortCol == 0)
            {
                sortCol = 6;
                sortDir = "desc";
            }

            var pageInfo = Page<PhasePlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Export Phase List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Phase list</returns>
        public IList<PhasePlus> ExportPhaseListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlPhaseList(companyCode, condition));
            sql.Append(") AS tbResult ");

            return this._database.Fetch<PhasePlus>(sql);
        }

        /// <summary>
        /// Get phase info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="phaseID">Phase ID</param>
        /// <returns>Phase info</returns>
        public PhasePlus GetPhaseInfo(string companyCode, int phaseID)
        {
            var sql = new Sql(@"
              SELECT 
                tbPhase.phase_id,
                tbPhase.phase_name,
                tbPhase.display_name,
                tbPhase.remarks,
                tbPhase.estimate_target_flg,
                tbPhase.ins_date,
                (SELECT display_name FROM m_user WHERE company_code = tbPhase.company_code AND user_sys_id = tbPhase.ins_id) AS ins_user,
                tbPhase.upd_date,
                (SELECT display_name FROM m_user WHERE company_code = tbPhase.company_code AND user_sys_id = tbPhase.upd_id) AS upd_user,
                tbPhase.del_flg
            FROM
                m_phase AS tbPhase
            WHERE
                tbPhase.company_code = @company_code
                AND tbPhase.phase_id = @phase_id ", new { company_code = companyCode, phase_id = phaseID });

            return this._database.FirstOrDefault<PhasePlus>(sql);
        }

        /// <summary>
        /// Edit phase info
        /// </summary>
        /// <param name="data">Phase info</param>
        /// <param name="phaseID">Phase ID output</param>
        /// <returns>Action result</returns>
        public bool EditPhaseInfo(PhasePlus data, out int phaseID)
        {
            int result = 0;
            phaseID = data.phase_id;

            if (data.phase_id == 0)
            {
                var sqlInsert= new Sql(@"
                    INSERT INTO
                        m_phase
                        (company_code,
                        phase_name,
                        display_name,
                        remarks,
                        estimate_target_flg,
                        ins_date,
                        ins_id,
                        upd_date,
                        upd_id,
                        del_flg)
                    VALUES
                        (@company_code, @phase_name, @display_name, @remarks,@estimate_target_flg,
                         @ins_date, @ins_id, @upd_date, @upd_id, @del_flg);
                    SELECT
                        SCOPE_IDENTITY();",
                    new
                    {
                        company_code = data.company_code,
                        phase_name = data.phase_name.Trim(),
                        display_name = data.display_name.Trim(),
                        remarks = data.remarks,
                        estimate_target_flg = data.estimate_target_flg,
                        ins_date = data.upd_date,
                        ins_id = data.upd_id,
                        upd_date = data.upd_date,
                        upd_id = data.upd_id,
                        del_flg = Constant.DeleteFlag.NON_DELETE
                    });

                phaseID = this._database.ExecuteScalar<int>(sqlInsert);

                if (phaseID > 0)
                    result = phaseID;
            }
            else
            {
                var sqlUpdate = new Sql(@"
                    UPDATE
                        m_phase
                    SET
                        phase_name = @phase_name,
                        display_name = @display_name,
                        remarks = @remarks,
                        estimate_target_flg = @estimate_target_flg,
                        upd_date = @upd_date,
                        upd_id = @upd_id,
                        del_flg = @del_flg
                    WHERE
                        phase_id = @phase_id
                        AND company_code = @company_code;",
                     new
                     {
                         phase_name = data.phase_name.Trim(),
                         display_name = data.display_name.Trim(),
                         remarks = data.remarks,
                         estimate_target_flg = data.estimate_target_flg,
                         upd_date = data.upd_date,
                         upd_id = data.upd_id,
                         del_flg = data.del_flg,
                         phase_id = data.phase_id,
                         company_code = data.company_code
                     });
                result = this._database.Execute(sqlUpdate);
            }

            return (result > 0);
        }

        #endregion

        #region Private

        /// <summary>
        /// Build sql get phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Sql get phase list</returns>
        private Sql BuildSqlPhaseList(string companyCode, Condition condition)
        {
            var sql = new Sql(@"
              SELECT 
                tbPhase.phase_id,
                tbPhase.phase_name,
                tbPhase.display_name,
                tbPhase.remarks,
                tbPhase.estimate_target_flg,
                tbPhase.upd_date,
                (SELECT display_name FROM m_user WHERE company_code = tbPhase.company_code AND user_sys_id = tbPhase.upd_id) AS upd_user,
                tbPhase.del_flg
            FROM
                m_phase AS tbPhase
            WHERE
                tbPhase.company_code = @company_code ", new { company_code = companyCode });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.PhaseName))
                    sql.Append(string.Format("AND (tbPhase.phase_name LIKE '{0}' ESCAPE '\\' OR tbPhase.display_name LIKE '{0}' ESCAPE '\\' )", "%" + replaceWildcardCharacters(condition.PhaseName) + "%"));

                if (!condition.DeleteFlag)
                    sql.Append("AND tbPhase.del_flg = '0'");
            }

            return sql;
        }

        #endregion
    }
}
