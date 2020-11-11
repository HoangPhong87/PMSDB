using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10005;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS10005Repository : Repository, IPMS10005Repository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10005Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS10005Repository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// Get list overhead cost
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <returns>List overhead cost</returns>
        public PageInfo<OverHeadCostPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition)
        {
            var sql = new Sql("SELECT * FROM ( ");

            sql.Append(BuildSqlOverheadCostList(condition));
            sql.Append(") AS tbResult ");

            if (sortCol == 0)
            {
                sortCol = 4;
                sortDir = "desc";
            }

            var pageInfo = Page<OverHeadCostPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get Overhead Cost Info
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>Overhead Cost Info</returns>
        public OverHeadCostPlus GetOverheadCostInfo(string company_code, int overhead_cost_id)
        {
            var sql = new Sql();
            sql.Append(@"
                SELECT
                    *,
                    (SELECT display_name from m_user WHERE user_sys_id = m_overhead_cost.ins_id ) [user_regist]
                FROM
                    m_overhead_cost
                WHERE
                    company_code = @company_code
                    AND overhead_cost_id = @overhead_cost_id
            ",
             new
             {
                 company_code = company_code,
                 overhead_cost_id = overhead_cost_id
             });
            return this._database.FirstOrDefault<OverHeadCostPlus>(sql);
        }

        /// <summary>
        /// Overhead Cost
        /// </summary>
        /// <param name="overheadCost">overheadCost</param>
        /// <returns>number of Overhead cost is update</returns>
        public int EditOverheadCostData(OverHeadCostPlus overheadCost)
        {
            int res = 0;
            Sql sql;

            if (overheadCost.overhead_cost_id > 0)
            {
                IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                    { "overhead_cost_type", overheadCost.overhead_cost_type.Trim() }
                    , { "remarks", overheadCost.remarks }
                    , { "ins_date", overheadCost.ins_date }
                    , { "ins_id", overheadCost.ins_id }
                };

                IDictionary<string, object> condition = new Dictionary<string, object>()
                {
                    { "overhead_cost_id", overheadCost.overhead_cost_id },
                    { "company_code", overheadCost.company_code }
                };


                if (Update<OverHeadCostPlus>(columns, condition) > 0)
                {
                    res = overheadCost.overhead_cost_id;
                }
            }
            else
            {
                sql = new Sql(@"
                INSERT INTO m_overhead_cost
                       (company_code
                       ,overhead_cost_type
                       ,remarks
                       ,ins_date
                       ,ins_id)
                 VALUES
                       (
                       @company_code,
                       @overhead_cost_type,
                       @remarks,
                       @ins_date,
                       @ins_id)"
                    , new { company_code = overheadCost.company_code }
                    , new { overhead_cost_type = overheadCost.overhead_cost_type.Trim() }
                    , new { remarks = overheadCost.remarks }
                    , new { ins_date = overheadCost.ins_date }
                    , new { ins_id = overheadCost.ins_id }
                 );

                if (_database.Execute(sql) > 0)
                {
                    var query = "SELECT ident_current('m_overhead_cost')";
                    res = _database.ExecuteScalar<int>(query);
                }
            }

            return res;
        }

        /// <summary>
        /// Get list overhead cost
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>list overhead cost</returns>
        public List<OverHeadCostPlus> GetOverheadCostList(Condition condition, string orderBy, string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlOverheadCostList(condition));
            sql.Append(") AS tbResult ");

            return this._database.Fetch<OverHeadCostPlus>(sql);
        }

        /// <summary>
        /// Delete overhead cost
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>number of overhead cost is delete</returns>
        public int DeleteOverHeadCode(string companyCode, int overhead_cost_id)
        {
            var sql = new Sql(@"
                DELETE
                FROM
                    m_overhead_cost
                WHERE
                    company_code = @company_code
                    AND overhead_cost_id = @overhead_cost_id "
                , new { company_code = companyCode, overhead_cost_id = overhead_cost_id });

            return this._database.Execute(sql);
        }

        /// <summary>
        /// Check exist overhead cost
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>number Overhead cost is get</returns>
        public int CheckExistOfOverHeadCode(string companyCode, int overhead_cost_id)
        {
            int result = 0;
            if (CheckOverHeadCostInTableOverHeadCodeHistory(companyCode, overhead_cost_id) >= 1 || 
                CheckOverHeadCostInTableOverHeadCode(companyCode, overhead_cost_id) >= 1)
            {
                result = 1;
            }
            return result;
        }

        /// <summary>
        /// Get number Overheadcost is exist in Table OverHeadCode History
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns>number Overheadcost is exist in Table OverHeadCode History</returns>
        private int CheckOverHeadCostInTableOverHeadCodeHistory(string companyCode, int overhead_cost_id)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT Count(*) from overhead_cost_history WHERE company_code = @company_code
                    AND overhead_cost_id = @overhead_cost_id "
                , new { company_code = companyCode, overhead_cost_id = overhead_cost_id });

            return _database.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Get number Overheadcost is exist in Table OverHeadCode
        /// </summary>
        /// <param name="companyCode">companyCode</param>
        /// <param name="overhead_cost_id">overhead_cost_id</param>
        /// <returns></returns>
        private int CheckOverHeadCostInTableOverHeadCode(string companyCode, int overhead_cost_id)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT Count(*) from overhead_cost WHERE company_code = @company_code
                    AND overhead_cost_id = @overhead_cost_id "
                , new { company_code = companyCode, overhead_cost_id = overhead_cost_id });

            return _database.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Build SQL overhead cost list
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Sql BuildSqlOverheadCostList(Condition condition)
        {
            var sql = Sql.Builder.Append(@"
                        SELECT
                            tblCost.overhead_cost_id,
                            tblCost.overhead_cost_type,
                            tblCost.remarks,
                            tblCost.ins_date,
                            (SELECT display_name from m_user WHERE user_sys_id = tblCost.ins_id ) [user_regist]
                        FROM
                            m_overhead_cost tblCost
                        WHERE
                            tblCost.company_code = @company_code
                 ", new { company_code = condition.COMPANY_CODE });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.OVERHEAD_COST_TYPE))
                    sql.Append(string.Format("AND tblCost.overhead_cost_type LIKE '%{0}%' ESCAPE '\\' ", replaceWildcardCharacters(condition.OVERHEAD_COST_TYPE)));
            }

            return sql;
        }
    }
}