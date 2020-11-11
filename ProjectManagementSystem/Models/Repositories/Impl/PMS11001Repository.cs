#region License
/// <copyright file="PMS11001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ - Clone</author>
/// <createdDate>2017/06/28</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.PMS11001;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Group repository class
    /// </summary>
    public class PMS11001Repository : Repository, IPMS11001Repository
    {
        #region Constructor
        /// <summary>
        /// Database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS11001Repository(PMSDatabase database)
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
        public PageInfo<BranchPlus> Search(int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition)
        {
            var sql = new Sql("SELECT * FROM ( ");

            sql.Append(BuildSqlBranchList(companyCode, condition));
            sql.Append(") AS tbResult ");

            if (sortCol == 0)
            {
                sortCol = 4;
                sortDir = "asc";
            }

            var pageInfo = Page<BranchPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Export Branch List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>Branch list</returns>
        public IList<BranchPlus> ExportBranchListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlBranchList(companyCode, condition));
            sql.Append(") AS tbResult ");
            return this._database.Fetch<BranchPlus>(sql);
        }

        /// <summary>
        /// Get Branch info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="groupID">Branch ID</param>
        /// <returns>Branch info</returns>
        public BranchPlus GetBranchInfo(string companyCode, int branchID)
        {
            var sql = new Sql(@"
              SELECT 
                tbBranch.location_id,
                tbBranch.location_name,
                tbBranch.display_name,
                tbBranch.remarks,
                tbBranch.display_order,
                tbBranch.ins_date,
                (SELECT display_name FROM m_user WHERE company_code = tbBranch.company_code AND user_sys_id = tbBranch.ins_id) AS ins_user,--get user insert name
                tbBranch.upd_date,
                (SELECT display_name FROM m_user WHERE company_code = tbBranch.company_code AND user_sys_id = tbBranch.upd_id) AS upd_user,--get user update name
                tbBranch.del_flg
            FROM
                m_business_location AS tbBranch
            WHERE
                tbBranch.company_code = @company_code
                AND tbBranch.location_id = @location_id ", new { company_code = companyCode, location_id = branchID });

            return this._database.FirstOrDefault<BranchPlus>(sql);
        }

        /// <summary>
        /// Edit group info
        /// </summary>
        /// <param name="data">Branch info</param>
        /// <param name="BranchID">Branch ID output</param>
        /// <returns>Action result</returns>
        public bool EditBranchInfo(BranchPlus data, out int branchID)
        {
            int result = 0;
            branchID = data.location_id;

            if (data.location_id == 0)
            {
                var sqlInsert = new Sql(@"
                    INSERT INTO
                        m_business_location
                        (company_code,
                        location_name,
                        display_name,
                        remarks,
                        display_order,
                        ins_date,
                        ins_id,
                        upd_date,
                        upd_id,
                        del_flg)
                    VALUES
                        (@company_code, @branch_name, @display_name, @remarks,@display_order,
                         @ins_date, @ins_id, @upd_date, @upd_id, @del_flg);
                    SELECT
                        SCOPE_IDENTITY();",
                    new
                    {
                        company_code = data.company_code,
                        branch_name = data.location_name.Trim(),
                        display_name = data.display_name.Trim(),
                        remarks = data.remarks,
                        display_order = data.display_order,
                        ins_date = data.upd_date,
                        ins_id = data.upd_id,
                        upd_date = data.upd_date,
                        upd_id = data.upd_id,
                        del_flg = Constant.DeleteFlag.NON_DELETE
                    });

                branchID = this._database.ExecuteScalar<int>(sqlInsert);

                if (branchID > 0)
                    result = branchID;
            }
            else
            {
                var sqlUpdate = new Sql(@"
                    UPDATE
                        m_business_location
                    SET
                        location_name = @branch_name,
                        display_name = @display_name,
                        remarks = @remarks,
                        display_order = @display_order,
                        upd_date = @upd_date,
                        upd_id = @upd_id,
                        del_flg = @del_flg
                    WHERE
                        location_id = @branch_id
                        AND company_code = @company_code;",
                     new
                     {
                         branch_name = data.location_name.Trim(),
                         display_name = data.display_name.Trim(),
                         remarks = data.remarks,
                         display_order = data.display_order,
                         upd_date = data.upd_date,
                         upd_id = data.upd_id,
                         del_flg = data.del_flg,
                         branch_id = data.location_id,
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
        /// <returns>Sql get Branch list</returns>
        private Sql BuildSqlBranchList(string companyCode, Condition condition)
        {

            var sql = new Sql(@"
              SELECT 
                tbBranch.location_id,
                tbBranch.location_name,
                tbBranch.display_name,
                tbBranch.display_order,
                tbBranch.remarks,
                tbBranch.upd_date,
                (SELECT display_name FROM m_user WHERE company_code = tbBranch.company_code AND user_sys_id = tbBranch.upd_id) AS upd_user,
                tbBranch.del_flg
            FROM
                m_business_location AS tbBranch
            WHERE
                tbBranch.company_code = @company_code ", new { company_code = companyCode });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.BranchName))
                    sql.Append(string.Format("AND (tbBranch.location_name LIKE '{0}' ESCAPE '\\' OR tbBranch.display_name LIKE '{0}' ESCAPE '\\')", "%" + replaceWildcardCharacters(condition.BranchName) + "%"));

                if (!condition.DeleteFlag)
                    sql.Append("AND tbBranch.del_flg = '0'");
            }

            return sql;
        }

        #endregion
    }
}
