using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10004;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS10004Repository : Repository, IPMS10004Repository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS10004Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS10004Repository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// Search data category list
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PageInfo<CategoryPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, Condition condition)
        {
            var sql = buildQueryListCategory(condition);
            var pageInfo = Page<CategoryPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);
            return pageInfo;
        }

        /// <summary>
        /// Get list category
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IList<CategoryPlus> GetListCategory(Condition condition, string hdnOrderBy, string hdnOrderType)
        {
            string sortCommand = string.Format(" ORDER BY {0} {1}", hdnOrderBy, hdnOrderType);
            var sql = buildQueryListCategory(condition, sortCommand);
            return this._database.Fetch<CategoryPlus>(sql);
        }

        private Sql buildQueryListCategory(Condition condition, string sortCommand = null)
        {
            Sql sql = Sql.Builder.Append(@"
            SELECT
                *
            FROM
            (
                SELECT
                    catg.category_id,
                    catg.category,
                    sub_catg.sub_category,
                    sub_catg.upd_date,
                    (SELECT display_name from m_user WHERE user_sys_id = sub_catg.upd_id ) [user_update],
                    catg.del_flg,
                    sub_catg.del_flg AS sub_del_flg
                FROM
                    m_category catg
                    INNER JOIN m_sub_category sub_catg
                        ON catg.company_code = sub_catg.company_code
                        AND catg.category_id = sub_catg.category_id
                WHERE
                    catg.company_code = @company_code
            ", new { company_code = condition.COMPANY_CODE });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.CATEGORY_NAME))
                    sql.Append(string.Format("AND catg.category LIKE '%{0}%' ESCAPE '\\' ", replaceWildcardCharacters(condition.CATEGORY_NAME)));

                if (!string.IsNullOrEmpty(condition.SUB_CATEGORY_NAME))
                    sql.Append(string.Format("AND sub_catg.sub_category LIKE '%{0}%' ESCAPE '\\' ", replaceWildcardCharacters(condition.SUB_CATEGORY_NAME)));

                if (!condition.DELETED_INCLUDE)
                    sql.Append("AND (catg.del_flg = 0 AND sub_catg.del_flg = 0)");
            }
            sql.Append(") tb_CategoryWithSub");
            if (sortCommand != null)
            {
                sql.Append(sortCommand);
            }
            return sql;
        }

        /// <summary>
        /// Get category Info
        /// </summary>
        /// <param name="company_code"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CategoryPlus GetCategoryInfo(string company_code, int categoryId)
        {
            var sql = new Sql();
            sql.Append(@"
                SELECT
                    *,
                    (SELECT display_name from m_user WHERE user_sys_id = m_category.upd_id ) [user_update],
                    (SELECT display_name from m_user WHERE user_sys_id = m_category.ins_id ) [user_regist]
                FROM
                    m_category
                WHERE
                    company_code = @company_code
                    AND category_id = @category_id
            ",
             new
             {
                 company_code = company_code,
                 category_id = categoryId
             });
            return this._database.FirstOrDefault<CategoryPlus>(sql);
        }

        /// <summary>
        /// Get list of subcategory
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public List<SubCategoryPlus> GetListSubCategory(string company_code, int categoryId)
        {
            var sql = new Sql();
            sql.Append(@"
                SELECT
                    msc.sub_category_id,
                    msc.sub_category,
                    ISNULL(msc.remarks,'') AS remarks,
                    msc.sub_category AS sub_category_old,
                    ISNULL(msc.remarks,'') AS remarks_old,
                    msc.display_order,
                    ISNULL(tc.project_count, 0) AS project_count,
                    msc.del_flg
                FROM
                    m_sub_category msc
                    LEFT JOIN (
                        SELECT company_code, sub_category_id, COUNT(project_sys_id) AS project_count
                        FROM target_category
                        WHERE company_code = @company_code
                        GROUP BY company_code, sub_category_id
                    ) AS tc
                       ON msc.company_code = tc.company_code
                       AND msc.sub_category_id = tc.sub_category_id
                WHERE
                    msc.company_code = @company_code
                    AND msc.category_id = @category_id
                    AND msc.del_flg = @del_flg
                ORDER BY msc.sub_category_id
               ",
            new
            {
                company_code = company_code,
                category_id = categoryId,
                del_flg = Constant.DeleteFlag.NON_DELETE
            });
            return this._database.Fetch<SubCategoryPlus>(sql);
        }

        /// <summary>
        /// Edit Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int EditCategory(CategoryPlus category, IList<SubCategoryPlus> subCategoryList)
        {
            int res = 0;
            Sql sql;

            if (category.category_id > 0)
            {
                IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                    { "category", category.category.Trim() }
                    , { "remarks", category.remarks }
                    , { "display_order", category.display_order }
                    , { "upd_date", category.upd_date }
                    , { "upd_id", category.upd_id }
                    , { "del_flg", category.del_flg }
                };

                IDictionary<string, object> condition = new Dictionary<string, object>()
                {
                    { "category_id", category.category_id },
                    { "company_code", category.company_code }
                };


                if (Update<Category>(columns, condition) > 0)
                {
                    res = category.category_id;
                }
            }
            else
            {
                category.ins_date = category.upd_date;
                category.ins_id = category.upd_id;

                sql = new Sql(@"
                INSERT INTO m_category
                       (company_code
                       ,category
                       ,remarks
                       ,display_order
                       ,ins_date
                       ,ins_id
                       ,upd_date
                       ,upd_id
                       ,del_flg)
                 VALUES
                       (
                       @company_code,
                       @category,
                       @remarks,
                       @display_order,
                       @ins_date,
                       @ins_id,
                       @upd_date,
                       @upd_id,
                       @del_flg)"
                    , new { company_code = category.company_code }
                    , new { category = category.category.Trim() }
                    , new { remarks = category.remarks }
                    , new { display_order = category.display_order }
                    , new { ins_date = category.ins_date }
                    , new { ins_id = category.ins_id }
                    , new { upd_date = category.upd_date }
                    , new { upd_id = category.upd_id }
                    , new { del_flg = Constant.DeleteFlag.NON_DELETE }
                 );

                if (_database.Execute(sql) > 0)
                {
                    var query = "select ident_current('m_category')";
                    res = _database.ExecuteScalar<int>(query);
                }
            }

            if (res > 0) {
                if (!EditListSubCategory(category.company_code, res, category.upd_date, category.upd_id, subCategoryList))
                    res = 0;
            }

            return res;
        }

        private bool EditListSubCategory(string companyCode, int categoryID, DateTime updDate, int updUserID, IList<SubCategoryPlus> subCategoryList)
        {
            int res = 1;

            if (subCategoryList.Count > 0) {
                foreach(var data in subCategoryList) {
                    if (data.sub_category_id > 0) {
                        if (data.Delete)
                        {
                            var sqlDelete = new Sql(@"
                            UPDATE 
                                m_sub_category
                            SET del_flg = @del_flg
                            WHERE
                                company_code = @company_code
                                AND category_id = @category_id
                                AND sub_category_id = @sub_category_id
                            ;", new
                              {
                                  del_flg = Constant.DeleteFlag.DELETE,
                                  company_code = companyCode,
                                  category_id = categoryID,
                                  sub_category_id = data.sub_category_id
                              });

                            if (this._database.Execute(sqlDelete) == 0)
                            {
                                res = 0;
                                break;
                            }
                        }
                        else if (!data.sub_category_old.Equals(data.sub_category) || !data.remarks_old.Equals(data.remarks))
                        {
                            var sqlUpdate = new Sql(@"
                            UPDATE
                                m_sub_category
                            SET
                                sub_category = @sub_category
                                , remarks = @remarks
                                , display_order = @display_order
                                , upd_date = @upd_date
                                , upd_id = @upd_id
                            WHERE
                                company_code = @company_code
                                AND category_id = @category_id
                                AND sub_category_id = @sub_category_id"
                                    , new { sub_category = data.sub_category.Trim() }
                                    , new { remarks = data.remarks }
                                    , new { display_order = data.display_order }
                                    , new { upd_date = updDate }
                                    , new { upd_id = updUserID }
                                    , new { company_code = companyCode }
                                    , new { category_id = categoryID }
                                    , new { sub_category_id = data.sub_category_id });

                            if (this._database.Execute(sqlUpdate) == 0)
                            {
                                res = 0;
                                break;
                            }
                        }
                    } else {
                        var sqlInsert = new Sql(@"
                            INSERT INTO m_sub_category
                                   (company_code
                                   ,category_id
                                   ,sub_category
                                   ,remarks
                                   ,display_order
                                   ,ins_date
                                   ,ins_id
                                   ,upd_date
                                   ,upd_id
                                   ,del_flg)
                             VALUES
                                   (
                                   @company_code,
                                   @category_id,
                                   @sub_category,
                                   @remarks,
                                   @display_order,
                                   @ins_date,
                                   @ins_id,
                                   @upd_date,
                                   @upd_id,
                                   @del_flg)"
                                    , new { company_code = companyCode }
                                    , new { category_id = categoryID }
                                    , new { sub_category = data.sub_category.Trim() }
                                    , new { remarks = data.remarks }
                                    , new { display_order = data.display_order }
                                    , new { ins_date = updDate }
                                    , new { ins_id = updUserID }
                                    , new { upd_date = updDate }
                                    , new { upd_id = updUserID }
                                    , new { del_flg = Constant.DeleteFlag.NON_DELETE }
                                 );

                        if (this._database.Execute(sqlInsert) == 0)
                        {
                            res = 0;
                            break;
                        }
                    }
                }
            }

            return (res > 0);
        }

        public IList<TargetCategory> GetTargetCategory(string companyCode, int subCategoryID, int categoryID)
        {
            var sql = new Sql(@"
                SELECT
                   sub_category_id
                FROM
                    target_category
                WHERE
                    company_code = @company_code
                    AND sub_category_id = @sub_category_id
                    AND (SELECT count(*)
                        FROM project_info
                        WHERE company_code = @company_code
                        AND project_sys_id = target_category.project_sys_id) > 0"
                , new
                {
                    company_code = companyCode,
                    sub_category_id = subCategoryID,
                    category_id = categoryID
                });

            return this._database.Fetch<TargetCategory>(sql);
        }
    }
}