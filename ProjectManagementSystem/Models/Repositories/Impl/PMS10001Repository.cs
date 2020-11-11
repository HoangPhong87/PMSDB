using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10001;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS10001Repository : Repository, IPMS10001Repository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS10001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// Get list Customer Tag
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List Customer Tag</returns>
        public PageInfo<CustomerTagPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, string companyCode)
        {
            var sql = new Sql("SELECT * FROM ( ");

            sql.Append(BuildSqlTagList(companyCode, condition));
            sql.Append(") AS tbResult ");

            if (sortCol == 0)
            {
                sortCol = 5;
                sortDir = "desc";
            }

            var pageInfo = Page<CustomerTagPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get list tag
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List tag</returns>
        public List<CustomerTagPlus> GetListTag(Condition condition, string companyCode, string orderBy, string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlTagList(companyCode, condition));
            sql.Append(") AS tbResult ");

            return this._database.Fetch<CustomerTagPlus>(sql);
        }

        /// <summary>
        /// Get tag info
        /// </summary>
        /// <param name="cCode">cCode</param>
        /// <param name="tagId">tagId</param>
        /// <returns>Customer Tag</returns>
        public CustomerTagPlus GetTagInfo(string cCode, int tagId)
        {
            var sql = new Sql(
                @"
                SELECT 
                    tb1.*,
                    tb2.display_name,
                    (SELECT display_name from m_user WHERE user_sys_id = tb1.upd_id ) [user_update],
                    (SELECT display_name from m_user WHERE user_sys_id = tb1.ins_id ) [user_regist]
                FROM
                    m_customer_tag tb1 LEFT JOIN m_customer tb2
                ON tb2.company_code = tb1.company_code
                AND tb2.customer_id = tb1.customer_id
                WHERE
                    tb1.company_code = @company_code
                    and tb1.tag_id = @tagId",
                new
                {
                    company_code = cCode,
                    tagId = tagId
                }

            );
            return this._database.SingleOrDefault<CustomerTagPlus>(sql);
        }

        /// <summary>
        /// Edit Tag
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>number of Tag is update</returns>
        public int EditTagData(CustomerTag data)
        {
            int res = 0;
            Sql sql;

            if (data.tag_id > 0)
            {
                IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                    { "company_code", data.company_code }
                    , { "customer_id", data.customer_id}
                    , { "tag_name", string.IsNullOrEmpty(data.tag_name) ? null : data.tag_name }
                    , { "display_order", data.display_order }
                    , { "remarks", string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                    , { "upd_date", data.upd_date }
                    , { "upd_id", data.upd_id }
                    , { "del_flg", data.del_flg }
                };

                IDictionary<string, object> condition = new Dictionary<string, object>()
                {
                    { "tag_id", data.tag_id }
                };

                if (Update<CustomerTag>(columns, condition) > 0)
                {
                    res = data.tag_id;
                }
            }
            else
            {
                data.ins_date = data.upd_date;
                data.ins_id = data.upd_id;

                sql = new Sql(@"
                INSERT INTO [dbo].[m_customer_tag]
                           (company_code
                           ,customer_id
                           ,tag_name
                           ,display_order
                           ,remarks
                           ,ins_date
                           ,ins_id
                           ,upd_date
                           ,upd_id
                           ,del_flg)
                     VALUES
                           (
                            @company_code
                           ,@customer_id
                           ,@tag_name
                           ,@display_order
                           ,@remarks
                           ,@ins_date
                           ,@ins_id
                           ,@upd_date
                           ,@upd_id
                           ,@del_flg)"
                    , new { company_code = data.company_code }
                    , new { customer_id = data.customer_id }
                    , new { tag_name = string.IsNullOrEmpty(data.tag_name) ? null : data.tag_name }
                    , new { display_order = data.display_order }
                    , new { remarks = string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                    , new { ins_date = data.ins_date }
                    , new { ins_id = data.ins_id }
                    , new { upd_date = data.upd_date }
                    , new { upd_id = data.upd_id }
                    , new { del_flg = Constant.DeleteFlag.NON_DELETE }
                 );

                if (_database.Execute(sql) > 0)
                {
                    var query = "SELECT ident_current('m_customer_tag')";
                    res = _database.ExecuteScalar<int>(query);
                }
            }

            return res;
        }

        /// <summary>
        /// Get count Sales payment
        /// </summary>
        /// <param name="company_code">company_code</param>
        /// <param name="customer_id">customer_id</param>
        /// <param name="tag_id">tag_id</param>
        /// <returns>number sale payment</returns>
        public int GetDataSalesPayment(string company_code, int customer_id, int tag_id)
        {
            Sql sql = Sql.Builder.Append(@"
                SELECT
                    COUNT(*)
                FROM
                    sales_payment
                WHERE
                    company_code = @company_code
                    AND ordering_flg = '1'
                    AND customer_id = @customer_id
                    AND tag_id = @tag_id
                 ", new { 
                      company_code = company_code,
                      customer_id = customer_id,
                      tag_id = tag_id
                    });
            return this._database.SingleOrDefault<int>(sql);
        }

        /// <summary>
        /// Build SQL tag list
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Sql BuildSqlTagList(string companyCode, Condition condition)
        {
            var sql = Sql.Builder.Append(@"
                        SELECT
                            tl1.tag_id, tl1.tag_name,  tl2.display_name, tl2.customer_id, tl1.upd_date, tl1.del_flg, (SELECT display_name from m_user WHERE user_sys_id = tl1.upd_id ) [user_update], tl1.display_order
                        FROM
                            m_customer_tag tl1 LEFT JOIN m_customer tl2
                            ON tl2.company_code = tl1.company_code
                            AND tl2.customer_id = tl1.customer_id
                        WHERE
                            tl1.company_code = @company_code
                 ", new { company_code = companyCode });

            if (condition != null)
            {
                if (condition.CUSTOMER_ID != null)
                    sql.Append(string.Format(" AND tl2.customer_id = '{0}'", condition.CUSTOMER_ID.Value));

                if (!string.IsNullOrEmpty(condition.TAG_NAME))
                    sql.Append(string.Format(" AND (tl1.tag_name LIKE '{0}' ESCAPE '\\') ", "%" + replaceWildcardCharacters(condition.TAG_NAME) + "%"));

                if (!condition.DELETED_INCLUDE)
                    sql.Append(string.Format(" AND tl1.del_flg = " + Constant.DeleteFlag.NON_DELETE));
            }

            return sql;
        }
    }
}