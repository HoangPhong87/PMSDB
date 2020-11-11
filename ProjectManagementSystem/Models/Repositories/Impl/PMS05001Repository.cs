#region License
/// <copyright file="PMS05001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/22</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS05001;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Group repository class
    /// </summary>
    public class PMS05001Repository : Repository, IPMS05001Repository
    {
        #region Constructor
        /// <summary>
        /// Database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS05001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS05001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Public
        /// <summary>
        /// Search group by condition
        /// </summary>
        /// <param name="startItem">Start item</param>
        /// <param name="itemsPerPage">Item per page</param>
        /// <param name="columns">List of colum name</param>
        /// <param name="sortCol">Sort by colum</param>
        /// <param name="sortDir">Sort type</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<GroupPlus> Search(int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition)
        {
            var sql = new Sql("SELECT * FROM ( ");

            sql.Append(BuildSqlGroupList(companyCode, condition));
            sql.Append(") AS tbResult ");

            if (sortCol == 0)
            {
                sortCol = 5;
                sortDir = "desc";
            }

            var pageInfo = Page<GroupPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Export Group List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Group list</returns>
        public IList<GroupPlus> ExportGroupListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));
            
            sql.Append(BuildSqlGroupList(companyCode, condition));
            sql.Append(") AS tbResult ");

            return this._database.Fetch<GroupPlus>(sql);
        }

        /// <summary>
        /// Get group info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="groupID">Group ID</param>
        /// <returns>Group info</returns>
        public GroupPlus GetGroupInfo(string companyCode, int groupID)
        {
            var sql = new Sql(@"
              SELECT 
                tbGroup.group_id,
                tbGroup.group_name,
                tbGroup.display_name,
                budget_setting_flg = ISNULL(tbGroup.budget_setting_flg,0),
                tbGroup.check_actual_work_flg,
                tbGroup.remarks,
                tbGroup.ins_date,
                (SELECT display_name FROM m_user WHERE company_code = tbGroup.company_code AND user_sys_id = tbGroup.ins_id) AS ins_user,
                tbGroup.upd_date,
                (SELECT display_name FROM m_user WHERE company_code = tbGroup.company_code AND user_sys_id = tbGroup.upd_id) AS upd_user,
                tbGroup.del_flg
            FROM
                m_group AS tbGroup
            WHERE
                tbGroup.company_code = @company_code
                AND tbGroup.group_id = @group_id ", new { company_code = companyCode, group_id = groupID });

            return this._database.FirstOrDefault<GroupPlus>(sql);
        }

        /// <summary>
        /// Edit group info
        /// </summary>
        /// <param name="data">Group info</param>
        /// <param name="groupID">Group ID output</param>
        /// <returns>Action result</returns>
        public bool EditGroupInfo(GroupPlus data, out int groupID)
        {
            int result = 0;
            groupID = data.group_id;

            if (data.group_id == 0)
            {
                var sqlInsert= new Sql(@"
                    INSERT INTO
                        m_group
                        (company_code,
                        group_name,
                        display_name,
                        budget_setting_flg,
                        check_actual_work_flg,
                        remarks,
                        ins_date,
                        ins_id,
                        upd_date,
                        upd_id,
                        del_flg)
                    VALUES
                        (@company_code, @group_name, @display_name, @budget_setting_flg, @check_actual_work_flg, @remarks,
                         @ins_date, @ins_id, @upd_date, @upd_id, @del_flg);
                    SELECT
                        SCOPE_IDENTITY();",
                    new
                    {
                        company_code = data.company_code,
                        group_name = data.group_name.Trim(),
                        display_name = data.display_name.Trim(),
                        budget_setting_flg = data.budget_setting_flg,
                        check_actual_work_flg = data.check_actual_work_flg,
                        remarks = data.remarks,
                        ins_date = data.upd_date,
                        ins_id = data.upd_id,
                        upd_date = data.upd_date,
                        upd_id = data.upd_id,
                        del_flg = Constant.DeleteFlag.NON_DELETE
                    });

                groupID = this._database.ExecuteScalar<int>(sqlInsert);

                if (groupID > 0)
                    result = groupID;
            }
            else
            {
                var sqlUpdate = new Sql(@"
                    UPDATE
                        m_group
                    SET
                        group_name = @group_name,
                        display_name = @display_name,
                        budget_setting_flg = @budget_setting_flg,
                        check_actual_work_flg = @check_actual_work_flg,
                        remarks = @remarks,
                        upd_date = @upd_date,
                        upd_id = @upd_id,
                        del_flg = @del_flg
                    WHERE
                        group_id = @group_id
                        AND company_code = @company_code;",
                     new
                     {
                         group_name = data.group_name.Trim(),
                         display_name = data.display_name.Trim(),
                         budget_setting_flg = data.budget_setting_flg,
                         check_actual_work_flg = data.check_actual_work_flg,
                         remarks = data.remarks,
                         upd_date = data.upd_date,
                         upd_id = data.upd_id,
                         del_flg = data.del_flg,
                         group_id = data.group_id,
                         company_code = data.company_code
                     });
                result = this._database.Execute(sqlUpdate);
            }

            return (result > 0);
        }

        #endregion

        #region Private

        /// <summary>
        /// Build sql get group list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Sql get group list</returns>
        private Sql BuildSqlGroupList(string companyCode, Condition condition)
        {
            var sql = new Sql(@"
              SELECT 
                tbGroup.group_id,
                tbGroup.group_name,
                tbGroup.display_name,
                budget_setting_flg = ISNULL(tbGroup.budget_setting_flg,0),
                tbGroup.remarks,
                tbGroup.upd_date,
                (SELECT display_name FROM m_user WHERE company_code = tbGroup.company_code AND user_sys_id = tbGroup.upd_id) AS upd_user,
                tbGroup.del_flg
            FROM
                m_group AS tbGroup
            WHERE
                tbGroup.company_code = @company_code ", new { company_code = companyCode });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.GroupName))
                    sql.Append(string.Format("AND (tbGroup.group_name LIKE '{0}' ESCAPE '\\' OR tbGroup.display_name LIKE '{0}' ESCAPE '\\')", "%" + replaceWildcardCharacters(condition.GroupName) + "%"));

                if (!condition.DeleteFlag)
                    sql.Append("AND tbGroup.del_flg = '0'");
            }

            return sql;
        }

        #endregion
    }
}
