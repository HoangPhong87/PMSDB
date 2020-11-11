using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10002;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS10002Repository : Repository, IPMS10002Repository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10002Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS10002Repository(PMSDatabase database)
        {
            this._database = database;
        }

        public PageInfo<InformationPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition, string companyCode)
        {
            var sql = new Sql("SELECT * FROM ( ");

            sql.Append(BuildSqlInformationList(companyCode, condition));
            sql.Append(") AS tbResult ");

            if (sortCol == 0)
            {
                sortCol = 5;
                sortDir = "desc";
            }

            var pageInfo = Page<InformationPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        public List<InformationPlus> GetListInfomation(Condition condition, string companyCode, string orderBy, string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlInformationList(companyCode, condition));
            sql.Append(") AS tbResult ");

            return this._database.Fetch<InformationPlus>(sql);
        }

        public InformationPlus GetInformation(string company_code, int infoId)
        {
            var sql = new Sql(
                @"
                SELECT 
                    info.*,
                    (SELECT display_name from m_user WHERE user_sys_id = info.upd_id ) [user_update],
                    (SELECT display_name from m_user WHERE user_sys_id = info.ins_id ) [user_regist]
                FROM
                    information info 
                WHERE
                    info.company_code = @company_code
                    and info.info_id = @info_id",
                new
                {
                    company_code = company_code,
                    info_id = infoId
                }

            );
            return this._database.SingleOrDefault<InformationPlus>(sql);
        }

        public int EditInformationData(Information data)
        {
            int res = 0;
            Sql sql;

            if (data.info_id > 0)
            {
                IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                    { "company_code", data.company_code }
                    , { "content", string.IsNullOrEmpty(data.content) ? null : data.content.Trim() }
                    , { "publish_start_date", data.publish_start_date }
                    , { "publish_end_date", data.publish_end_date }
                    , { "display_order", data.display_order }
                    , { "upd_date", data.upd_date }
                    , { "upd_id", data.upd_id }
                    , { "del_flg", data.del_flg }
                };

                IDictionary<string, object> condition = new Dictionary<string, object>()
                {
                    { "info_id", data.info_id }
                };


                if (Update<Information>(columns, condition) > 0)
                {
                    res = (int)data.info_id;
                }
            }
            else
            {
                data.ins_date = data.upd_date;
                data.ins_id = data.upd_id;

                sql = new Sql(@"
                INSERT INTO [dbo].[information]
                       ([company_code]
                       ,[publish_start_date]
                       ,[publish_end_date]
                       ,[content]
                       ,[display_order]
                       ,[ins_date]
                       ,[ins_id]
                       ,[upd_date]
                       ,[upd_id]
                       ,[del_flg])
                 VALUES
                       (
                       @company_code,
                       @publish_start_date,
                       @publish_end_date,
                       @content,
                       @display_order,
                       @ins_date,
                       @ins_id,
                       @upd_date,
                       @upd_id,
                       @del_flg)"
                    , new { company_code = data.company_code }
                    , new { publish_start_date = data.publish_start_date }
                    , new { publish_end_date = data.publish_end_date }
                    , new { content = string.IsNullOrEmpty(data.content) ? null : data.content.Trim() }
                    , new { display_order = data.display_order }
                    , new { ins_date = data.ins_date }
                    , new { ins_id = data.ins_id }
                    , new { upd_date = data.upd_date }
                    , new { upd_id = data.upd_id }
                    , new { del_flg = Constant.DeleteFlag.NON_DELETE }
                 );

                if (_database.Execute(sql) > 0)
                {
                    var query = "select ident_current('information')";
                    res = _database.ExecuteScalar<int>(query);
                }
            }

            return res;
        }

        /// <summary>
        /// Build SQL infomation list
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Sql BuildSqlInformationList(string companyCode, Condition condition)
        {
            var sql = Sql.Builder.Append(@"
                        SELECT 
                            info_id,
                            content,
                            publish_start_date,
                            publish_end_date,
                            info.upd_date,
                            m_user.display_name,
                            info.del_flg
                        FROM 
                            information info 
                            LEFT JOIN m_user
                                ON info.company_code = m_user.company_code
                                AND info.upd_id = m_user.user_sys_id
                        WHERE
                            info.company_code = @company_code
                 ", new { company_code = companyCode });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.INFORMATION_CONTENT))
                {
                    sql.Append(string.Format("AND info.content LIKE '%{0}%'  ESCAPE '\\' ", replaceWildcardCharacters(condition.INFORMATION_CONTENT)));
                }
                if (!string.IsNullOrEmpty(condition.START_DATE))
                {
                    sql.Append(string.Format("AND info.publish_end_date >= '{0}'", condition.START_DATE));
                }
                if (!string.IsNullOrEmpty(condition.END_DATE))
                {
                    sql.Append(string.Format("AND info.publish_start_date <= '{0}'", condition.END_DATE));
                }
                if (!condition.DELETED_INCLUDE)
                {
                    sql.Append(string.Format("AND info.del_flg = '{0}'", Constant.DeleteFlag.NON_DELETE));
                }
            }

            return sql;
        }
    }
}