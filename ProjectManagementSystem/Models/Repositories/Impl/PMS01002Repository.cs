using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS01002;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;


namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS01002Repository : Repository, IPMS01002Repository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS01002Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS01002Repository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// Search list user
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>PageInfo</returns>
        public PageInfo<UserPlus> Search(
            int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            Condition condition,
            string companyCode)
        {
            Sql sql = new Sql();

            if (condition.PROJECT_ID > 0)
            {
                sql = Sql.Builder.Append(@"
                    SELECT 
                        tbResult.*
                    FROM (
                        SELECT
                            tbUser.company_code,
                            tbUser.user_sys_id,
                            tbInfo.display_name,
                            (SELECT display_name 
                            FROM m_group 
                            WHERE company_code = tbUser.company_code AND group_id = tbInfo.group_id) AS display_name_group,
                            (SELECT display_name 
                            FROM m_position 
                            WHERE company_code = tbUser.company_code AND position_id = tbInfo.position_id) AS display_name_position,
                            (SELECT TOP(1) base_unit_cost
						    FROM unit_price_history
						    WHERE company_code = tbUser.company_code
								    AND user_sys_id = tbUser.user_sys_id
								    AND (YEAR(apply_start_date) < YEAR(GETDATE()) OR (YEAR(apply_start_date) = YEAR(GETDATE()) AND MONTH(apply_start_date) <= MONTH(GETDATE())))
						    ORDER BY apply_start_date DESC
						    ) AS base_unit_cost,
                            tbInfo.entry_date,
                            tbInfo.mail_address_1,
                            CASE 
                                WHEN tbInfo.password_lock_flg = '1' OR (tbInfo.retirement_date IS NOT NULL AND tbInfo.retirement_date < @CurrentDate) THEN N'無効'
                                ELSE N'有効'
                            END AS is_active,
                            tbInfo.upd_date,
                            (SELECT display_name FROM m_user WHERE company_code = tbUser.company_code AND user_sys_id = tbInfo.upd_id) AS user_update,
                            tbInfo.user_name_mei,
                            tbInfo.user_name_sei,
                            tbInfo.furigana_mei,
                            tbInfo.furigana_sei,
                            tbInfo.group_id,
                            tbInfo.position_id,
                            tbInfo.mail_address_2,
                            tbInfo.del_flg,
                            tbInfo.retirement_date
                        FROM (
                            SELECT
                                ISNULL(tbUserMain.company_code, tbUserProject.company_code) AS company_code,
                                ISNULL(tbUserMain.user_sys_id, tbUserProject.user_sys_id) AS user_sys_id
                            FROM (
                                SELECT
                                    company_code,
                                    user_sys_id
                                FROM
                                    m_user
                                WHERE
                                    company_code = @company_code
                                    AND del_flg = '0'
                                    AND (retirement_date IS NULL OR retirement_date >= @CurrentDate)
                                ) AS tbUserMain FULL JOIN (
                                    SELECT
                                        ISNULL(tbMerge2.company_code, tbBusPic.company_code) AS company_code,
                                        ISNULL(tbMerge2.user_sys_id, tbBusPic.charge_of_sales_id) AS user_sys_id
                                    FROM (
                                        SELECT
                                            ISNULL(tbMerge1.company_code, tbOverheadCost.company_code) AS company_code,
                                            ISNULL(tbMerge1.user_sys_id, tbOverheadCost.charge_person_id) AS user_sys_id
                                        FROM (
                                            SELECT
                                                ISNULL(tbMember.company_code, tbPayment.company_code) AS company_code,
                                                ISNULL(tbMember.user_sys_id, tbPayment.charge_person_id) AS user_sys_id
                                            FROM (
                                                SELECT
                                                    company_code,
                                                    user_sys_id
                                                FROM 
                                                    member_assignment
                                                WHERE
                                                    company_code = @company_code
                                                    AND project_sys_id = @project_sys_id
                                                ) AS tbMember FULL JOIN (
                                                    SELECT DISTINCT
                                                        company_code,
                                                        charge_person_id
                                                    FROM
                                                        sales_payment
                                                    WHERE
                                                        company_code = @company_code
                                                        AND project_sys_id = @project_sys_id
                                                        AND ordering_flg = '2'
                                                ) AS tbPayment
                                                ON tbMember.company_code = tbPayment.company_code
                                                AND tbMember.user_sys_id = tbPayment.charge_person_id
                                            ) AS tbMerge1 FULL JOIN (
                                                SELECT DISTINCT
                                                    company_code,
                                                    charge_person_id
                                                FROM
                                                    overhead_cost
                                                WHERE
                                                    company_code = @company_code
                                                    AND project_sys_id = @project_sys_id
                                            ) AS tbOverheadCost
                                            ON tbMerge1.company_code = tbOverheadCost.company_code
                                            AND tbMerge1.user_sys_id = tbOverheadCost.charge_person_id
                                        ) AS tbMerge2 FULL JOIN (
                                            SELECT company_code, ISNULL(charge_of_sales_id, 0) AS charge_of_sales_id
                                            FROM project_info
                                            WHERE company_code = @company_code
                                            AND project_sys_id = @project_sys_id
                                        ) AS tbBusPic
                                        ON tbMerge2.company_code = tbBusPic.company_code
                                        AND tbMerge2.user_sys_id = tbBusPic.charge_of_sales_id
                                ) AS tbUserProject
                                ON tbUserMain.company_code = tbUserProject.company_code
                                AND tbUserMain.user_sys_id = tbUserProject.user_sys_id
                            ) AS tbUser INNER JOIN m_user AS tbInfo
                            ON tbUser.company_code = tbInfo.company_code
                            AND tbUser.user_sys_id = tbInfo.user_sys_id
                    ) tbResult WHERE tbResult.company_code = @company_code", new { company_code = companyCode, project_sys_id = condition.PROJECT_ID, CurrentDate = Utility.GetCurrentDateTime() });

                if (condition != null)
                {
                    if (!string.IsNullOrEmpty(condition.DISPLAY_NAME))
                        sql.Append(string.Format(" AND (tbResult.display_name LIKE '{0}' ESCAPE '\\' OR tbResult.user_name_sei LIKE '{0}' ESCAPE '\\' OR tbResult.user_name_mei LIKE '{0}' ESCAPE '\\' OR tbResult.furigana_sei LIKE '{0}' ESCAPE '\\' OR tbResult.furigana_mei LIKE '{0}' ESCAPE '\\' )", "%" + replaceWildcardCharacters(condition.DISPLAY_NAME) + "%"));

                    if (condition.GROUP_ID != null)
                        sql.Append(string.Format(" AND tbResult.group_id = {0}", condition.GROUP_ID));

                    if (condition.POSITION_ID != null)
                        sql.Append(string.Format(" AND tbResult.position_id = {0}", condition.POSITION_ID));

                    if (!string.IsNullOrEmpty(condition.MAIL_ADDRESS))
                        sql.Append(string.Format(" AND (tbResult.mail_address_1 LIKE '{0}' ESCAPE '\\' OR tbResult.mail_address_2 LIKE '{0}' ESCAPE '\\')", "%" + replaceWildcardCharacters(condition.MAIL_ADDRESS) + "%"));

                    if (!condition.DELETED_INCLUDE && condition.PROJECT_ID == 0)
                        sql.Append(string.Format(" AND tbResult.del_flg = " + Constant.DeleteFlag.NON_DELETE));

                    if (!condition.RETIREMENT_INCLUDE)
                    {
                        if (condition.PROJECT_ID > 0 && !string.IsNullOrEmpty(condition.FROM_DATE) && !string.IsNullOrEmpty(condition.TO_DATE))
                            sql.Append(string.Format(" AND (tbResult.retirement_date IS NULL OR tbResult.retirement_date >= @CurrentDate OR (tbResult.retirement_date between '" + condition.FROM_DATE + "' AND '" + condition.TO_DATE + "'))"), new { CurrentDate = Utility.GetCurrentDateTime() });
                        else
                            sql.Append(string.Format(" AND (tbResult.retirement_date IS NULL OR tbResult.retirement_date >= @CurrentDate)"), new { CurrentDate = Utility.GetCurrentDateTime() });
                    }
                }
            }
            else
            {
                sql = new Sql("SELECT * FROM ( ");
                sql.Append(BuildSqlUserList(companyCode, condition));
                sql.Append(") AS tbResult ");
                if (sortCol.HasValue && sortCol < 2)
                {
                    sortDir = "desc";
                }

                string firstColumns = "upd_date,upd_date,display_name,";
                string myString = columns.Remove(columns.IndexOf(firstColumns), firstColumns.Length);

                columns = firstColumns + myString;
            }

            var pageInfo = Page<UserPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get a User info
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="userId">user Id</param>
        /// <returns>User info</returns>
        public UserPlus GetUserInfo(string cCode, int userId)
        {
            var sql = new Sql(@"
                SELECT 
                    tb1.*,
                    tb2.display_name [display_name_group],
                    tb3.display_name [display_name_position],
                    (SELECT display_name from m_user WHERE user_sys_id = tb1.upd_id ) [user_update],
                    (SELECT display_name from m_user WHERE user_sys_id = tb1.ins_id ) [user_regist]
                FROM
                    m_user tb1 LEFT JOIN m_group tb2
                    ON tb2.company_code = tb1.company_code
                    AND tb2.group_id = tb1.group_id LEFT JOIN m_position tb3
                    ON tb3.company_code = tb1.company_code
                    AND tb3.position_id = tb1.position_id
                WHERE
                    tb1.company_code = @company_code
                    and tb1.user_sys_id = @user_sys_id"
            , new { company_code = cCode, user_sys_id = userId });

            return this._database.Single<UserPlus>(sql);
        }

        /// <summary>
        /// Get a unit price history info
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="userId">user id</param>
        /// <param name="startDate">apply start date</param>
        /// <returns>user info</returns>
        public IList<UnitPriceHistoryPlus> GetUnitPriceHistoryInfo(string cCode, int userId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                SELECT
                    apply_start_date,
                    base_unit_cost,
                    remarks,
                    ins_date,
                    ins_id
                FROM
                    unit_price_history as ups
                WHERE
                    ups.company_code = '{0}'
                    and ups.user_sys_id = {1}
                    ORDER BY ups.apply_start_date

                    "
            , cCode, userId);

            var sql = new Sql(sb.ToString());
            return this._database.Fetch<UnitPriceHistoryPlus>(sql);
        }

        /// <summary>
        /// Get data_editable_time from company setting to delete progress
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetDataEditTableTime(string companyCode)
        {
            var sql = new Sql(@"
                SELECT data_editable_time 
                FROM m_company_setting
                WHERE company_code = @company_code",
                new
                {
                    company_code = companyCode
                });

            return this._database.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// Get Group list
        /// </summary>
        /// <param name="cCode">cCode</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<Group> GetGroupList(string cCode)
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

            return this.Select<Group>(condition, "display_order");
        }

        /// <summary>
        /// Get list Authority
        /// </summary>
        /// <param name="cCode">cCode</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<AuthorityRole> GetAuthorityRoleList(string cCode)
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

            return this.Select<AuthorityRole>(condition, "display_order");
        }

        /// <summary>
        /// Get Position list
        /// </summary>
        /// <param name="cCode">cCode</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<Position> GetPositionList(string cCode)
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

            return this.Select<Position>(condition, "display_order");
        }

        /// <summary>
        /// Get Branch List
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IEnumerable<BusinessLocation> GetBranchList(string cCode)
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
            return this.Select<BusinessLocation>(condition, "display_order");
        }

        /// <summary>
        /// Get Language ist
        /// </summary>
        /// <returns>IEnumerable</returns>
        public IEnumerable<Language> GetLanguageList()
        {
            var condition = new Dictionary<string, object>
            {

            };
            return this.Select<Language>(null);
        }

        /// <summary>
        /// Update Unit Price History
        /// </summary>
        /// <param name="data"></param>
        /// <param name="editor_id"></param>
        /// <param name="addNewUser"></param>
        public int UpdateUnitPriceHistory(UserPlus data, int editor_id)
        {
            try
            {
                if (data.unit_price_history != null)
                {
                    foreach (var unitPriceHistory in data.unit_price_history)
                    {
                        StringBuilder buildDelSql = new StringBuilder();
                        buildDelSql.AppendFormat(@"DELETE FROM
                        unit_price_history
                        WHERE
                        company_code = '{0}'
                        AND user_sys_id = '{1}'
                        AND ", data.company_code, data.user_sys_id);

                        StringBuilder buildInsSql = new StringBuilder();
                        buildInsSql.AppendFormat(@"INSERT INTO
                            unit_price_history
                            (company_code,
                            user_sys_id,
                            apply_start_date,
                            base_unit_cost,
                            remarks,
                            ins_date,
                            ins_id,
                            upd_id)
                        VALUES ");

                        StringBuilder buildUpdSql = new StringBuilder();
                        buildUpdSql.AppendFormat(@"
                                UPDATE unit_price_history
                                SET base_unit_cost = @base_unit_cost,
                                remarks = @remarks
                                WHERE
                                company_code = @company_code AND
                                user_sys_id = @user_sys_id AND
                                apply_start_date = @apply_start_date
                            ");
                        var checkDel = false;
                        var checkIns = false;
                        var checkUpd = false;
                        if (unitPriceHistory.isDelete.HasValue
                            && unitPriceHistory.isDelete.Value
                            && unitPriceHistory.apply_start_date != null
                            && unitPriceHistory.base_unit_cost != null)
                        {
                            checkDel = true;
                            buildDelSql.AppendFormat(" apply_start_date = '{0}' OR ", unitPriceHistory.apply_start_date);
                        }

                        if (unitPriceHistory.isNew.HasValue
                            && unitPriceHistory.isNew.Value
                            && unitPriceHistory.apply_start_date != null
                            && unitPriceHistory.base_unit_cost != null)
                        {
                            checkIns = true;
                            var ins_date = DateTime.Now;
                            buildInsSql.AppendFormat(@"('{0}', {1}, '{2}', {3}, '{4}', '{5}', {6} ,{7}),", data.company_code, data.user_sys_id, unitPriceHistory.apply_start_date, unitPriceHistory.base_unit_cost, unitPriceHistory.remarks, ins_date, editor_id, editor_id);
                        }

                        if (unitPriceHistory.isUpdate.HasValue
                            && unitPriceHistory.isUpdate.Value
                            && unitPriceHistory.apply_start_date != null
                            && unitPriceHistory.base_unit_cost != null)
                        {
                            checkUpd = true;
                        }

                        if (checkDel)
                        {
                            buildDelSql = buildDelSql.Remove(buildDelSql.Length - 3, 3);
                            Sql sql = new Sql(buildDelSql.ToString());
                            var check = _database.Execute(sql);
                        }
                        if (checkIns)
                        {
                            buildInsSql = buildInsSql.Remove(buildInsSql.Length - 1, 1);
                            Sql sql = new Sql(buildInsSql.ToString());
                            var check = _database.Execute(sql);
                        }

                        if (checkUpd)
                        {
                            Sql sql = new Sql(buildUpdSql.ToString(),
                                new
                                {
                                    company_code = data.company_code,
                                    user_sys_id = data.user_sys_id,
                                    apply_start_date = unitPriceHistory.apply_start_date,
                                    base_unit_cost = unitPriceHistory.base_unit_cost,
                                    remarks = unitPriceHistory.remarks,
                                });
                            var check = _database.Execute(sql);
                        }
                    }
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Edit info user
        /// </summary>
        /// <param name="data">obj user</param>
        /// <returns>Number of record is update</returns>
        public int EditUserData(UserPlus data)
        {
            int res = 0;
            Sql sql;

            GroupHistory groupHistoryData = new GroupHistory();
            groupHistoryData.company_code = data.company_code;
            groupHistoryData.actual_work_year = data.upd_date.Year;
            groupHistoryData.actual_work_month = data.upd_date.Month;
            groupHistoryData.group_id = data.group_id;
            groupHistoryData.location_id = data.location_id;
            groupHistoryData.ins_date = data.ins_date;
            groupHistoryData.ins_id = data.ins_id;
            groupHistoryData.upd_date = data.upd_date;
            groupHistoryData.upd_id = data.upd_id;

            if (data.user_sys_id > 0)
            {
                IDictionary<string, object> columns;
                if (data.password == Constant.DISPLAY_PASSWORD)
                {
                    columns = new Dictionary<string, object>()
                    {
                        { "company_code", data.company_code }
                        , { "user_account", data.user_account }
                        , { "user_name_sei", string.IsNullOrEmpty(data.user_name_sei) ? null : data.user_name_sei }
                        , { "user_name_mei", string.IsNullOrEmpty(data.user_name_mei) ? null : data.user_name_mei }
                        , { "furigana_sei", string.IsNullOrEmpty(data.furigana_sei) ? null : data.furigana_sei }
                        , { "furigana_mei", string.IsNullOrEmpty(data.furigana_mei) ? null : data.furigana_mei }
                        , { "display_name", string.IsNullOrEmpty(data.display_name) ? null : data.display_name }
                        , { "position_id", data.position_id ?? null }
                        , { "group_id", data.group_id ?? null }
                        , { "location_id", data.location_id ?? null }
                        , { "tel_no", string.IsNullOrEmpty(data.tel_no) ? null : data.tel_no }
                        , { "mail_address_1", string.IsNullOrEmpty(data.mail_address_1) ? null : data.mail_address_1 }
                        , { "mail_address_2", string.IsNullOrEmpty(data.mail_address_2) ? null : data.mail_address_2 }
                        , { "entry_date", data.entry_date ?? null }
                        , { "birth_date", data.birth_date ?? null }
                        , { "image_file_path", string.IsNullOrEmpty(data.image_file_path) ? null : data.image_file_path }
                        , { "remarks", string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                        , { "display_order", data.display_order }
                        , { "upd_date", data.upd_date }
                        , { "upd_id", data.upd_id }
                        , { "password_lock_flg", string.IsNullOrEmpty(data.password_lock_flg) ? Constant.UserLockFlag.NON_LOCKED : data.password_lock_flg }
                        , { "temporary_retirement_flg", string.IsNullOrEmpty(data.temporary_retirement_flg) ? Constant.Temporary_Retirement_Flg.NON_RETIREMENT : data.temporary_retirement_flg }
                        , { "role_id", data.role_id }
                        , { "retirement_date", data.retirement_date }
                        , { "del_flg", data.del_flg }
                        , { "employee_no", string.IsNullOrEmpty(data.employee_no) ? null : data.employee_no }
                        , { "actual_work_input_mode", data.actual_work_input_mode }
                    };
                }
                else
                {
                    data.password_last_update = data.upd_date;
                    columns = new Dictionary<string, object>()
                    {
                        { "company_code", data.company_code }
                        , { "user_account", data.user_account }
                        , { "password", data.password }
                        , { "user_name_sei", string.IsNullOrEmpty(data.user_name_sei) ? null : data.user_name_sei }
                        , { "user_name_mei", string.IsNullOrEmpty(data.user_name_mei) ? null : data.user_name_mei }
                        , { "furigana_sei", string.IsNullOrEmpty(data.furigana_sei) ? null : data.furigana_sei }
                        , { "furigana_mei", string.IsNullOrEmpty(data.furigana_mei) ? null : data.furigana_mei }
                        , { "display_name", string.IsNullOrEmpty(data.display_name) ? null : data.display_name }
                        , { "position_id", data.position_id ?? null }
                        , { "group_id", data.group_id ?? null }
                        , { "location_id", data.location_id ?? null }
                        , { "tel_no", string.IsNullOrEmpty( data.tel_no)  ? null : data.tel_no }
                        , { "mail_address_1", string.IsNullOrEmpty(data.mail_address_1) ? null : data.mail_address_1 }
                        , { "mail_address_2", string.IsNullOrEmpty(data.mail_address_2) ? null : data.mail_address_2 }
                        , { "entry_date", data.entry_date ?? null }
                        , { "birth_date", data.birth_date ?? null }
                        , { "image_file_path", string.IsNullOrEmpty(data.image_file_path) ? null : data.image_file_path }
                        , { "remarks", string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                        , { "display_order", data.display_order }
                        , { "upd_date", data.upd_date }
                        , { "upd_id", data.upd_id }
                        , { "password_lock_flg", string.IsNullOrEmpty(data.password_lock_flg) ? Constant.UserLockFlag.NON_LOCKED : data.password_lock_flg }
                        , { "temporary_retirement_flg", string.IsNullOrEmpty(data.temporary_retirement_flg) ? Constant.Temporary_Retirement_Flg.NON_RETIREMENT : data.temporary_retirement_flg }
                        , { "role_id", data.role_id }
                        , { "retirement_date", data.retirement_date }
                        , { "del_flg", data.del_flg }
                        , { "password_last_update", data.password_last_update }
                        , { "employee_no", string.IsNullOrEmpty(data.employee_no) ? null : data.employee_no }
                        , { "actual_work_input_mode", data.actual_work_input_mode }
                    };
                }

                IDictionary<string, object> condition;
                if (data.row_version == null)
                {
                    condition = new Dictionary<string, object>()
                    {
                        { "user_sys_id", data.user_sys_id }
                    };
                }
                else
                {
                    condition = new Dictionary<string, object>()
                    {
                        { "user_sys_id", data.user_sys_id }, {"row_version", data.row_version}
                    };
                }

                if (Update<User>(columns, condition) > 0)
                {
                    groupHistoryData.user_sys_id = data.user_sys_id;
                    if (EditGroupHistory(groupHistoryData) == 1)
                    {
                        res = data.user_sys_id;
                    }
                }
            }
            else
            {
                data.ins_date = data.upd_date;
                data.ins_id = data.upd_id;
                data.password_last_update = data.upd_date;

                sql = new Sql(@"
                    INSERT INTO [dbo].[m_user]
                               (company_code
                               ,user_account
                               ,password
                               ,user_name_sei
                               ,user_name_mei
                               ,furigana_sei
                               ,furigana_mei
                               ,display_name
                               ,position_id
                               ,group_id
                               ,location_id
                               ,tel_no
                               ,mail_address_1
                               ,mail_address_2
                               ,entry_date
                               ,birth_date
                               ,image_file_path
                               ,remarks
                               ,display_order
                               ,ins_date
                               ,ins_id
                               ,upd_date
                               ,upd_id
                               ,del_flg
                               ,password_lock_flg
                               ,temporary_retirement_flg
                               ,role_id
                               ,retirement_date
                               ,password_last_update
                               ,employee_no
                               ,actual_work_input_mode)
                         VALUES
                               (
                                @company_code
                               ,@user_account
                               ,@password
                               ,@user_name_sei
                               ,@user_name_mei
                               ,@furigana_sei
                               ,@furigana_mei
                               ,@display_name
                               ,@position_id
                               ,@group_id
                               ,@location_id
                               ,@tel_no
                               ,@mail_address_1
                               ,@mail_address_2
                               ,@entry_date
                               ,@birth_date
                               ,@image_file_path
                               ,@remarks
                               ,@display_order
                               ,@ins_date
                               ,@ins_id
                               ,@upd_date
                               ,@upd_id
                               ,@del_flg
                               ,@password_lock_flg
                               ,@temporary_retirement_flg
                               ,@role_id
                               ,@retirement_date
                               ,@password_last_update
                               ,@employee_no
                               ,@actual_work_input_mode)"
                    , new { company_code = data.company_code }
                    , new { user_account = data.user_account }
                    , new { password = data.password }
                    , new { user_name_sei = string.IsNullOrEmpty(data.user_name_sei) ? null : data.user_name_sei }
                    , new { user_name_mei = string.IsNullOrEmpty(data.user_name_mei) ? null : data.user_name_mei }
                    , new { furigana_sei = string.IsNullOrEmpty(data.furigana_sei) ? null : data.furigana_sei }
                    , new { furigana_mei = string.IsNullOrEmpty(data.furigana_mei) ? null : data.furigana_mei }
                    , new { display_name = string.IsNullOrEmpty(data.display_name) ? null : data.display_name }
                    , new { position_id = data.position_id ?? null }
                    , new { group_id = data.group_id ?? null }
                    , new { location_id = data.location_id ?? null }
                    , new { tel_no = string.IsNullOrEmpty(data.tel_no) ? null : data.tel_no }
                    , new { mail_address_1 = string.IsNullOrEmpty(data.mail_address_1) ? null : data.mail_address_1 }
                    , new { mail_address_2 = string.IsNullOrEmpty(data.mail_address_2) ? null : data.mail_address_2 }
                    , new { entry_date = data.entry_date ?? null }
                    , new { birth_date = data.birth_date ?? null }
                    , new { image_file_path = string.IsNullOrEmpty(data.image_file_path) ? null : data.image_file_path }
                    , new { remarks = string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                    , new { display_order = data.display_order }
                    , new { ins_date = data.ins_date }
                    , new { ins_id = data.ins_id }
                    , new { upd_date = data.upd_date }
                    , new { upd_id = data.upd_id }
                    , new { del_flg = Constant.DeleteFlag.NON_DELETE }
                    , new { password_lock_flg = string.IsNullOrEmpty(data.password_lock_flg) ? Constant.UserLockFlag.NON_LOCKED : data.password_lock_flg }
                    , new { temporary_retirement_flg = string.IsNullOrEmpty(data.temporary_retirement_flg) ? Constant.Temporary_Retirement_Flg.NON_RETIREMENT : data.temporary_retirement_flg }
                    , new { role_id = data.role_id }
                    , new { retirement_date = data.retirement_date }
                    , new { password_last_update = data.password_last_update }
                    , new { employee_no = string.IsNullOrEmpty(data.employee_no) ? null : data.employee_no }
                    , new { actual_work_input_mode = data.actual_work_input_mode }
                 );

                if (_database.Execute(sql) > 0)
                {
                    var query = "select ident_current('m_user')";
                    var result = _database.ExecuteScalar<int>(query);

                    groupHistoryData.user_sys_id = result;
                    if (EditGroupHistory(groupHistoryData) == 1)
                    {
                        res = result;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Eidt Group history
        /// </summary>
        /// <param name="data">obj Group history</param>
        /// <returns>Number of Group History is update</returns>
        private int EditGroupHistory(GroupHistory data)
        {
            int res = 0;
            Sql sql;

            if (CheckGroupHistory(data) == 1)
            {
                IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                      { "group_id", data.group_id }
                    , { "location_id", data.location_id }
                    , { "remarks", string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                    , { "upd_date", data.upd_date }
                    , { "upd_id", data.upd_id }
                };

                IDictionary<string, object> condition = new Dictionary<string, object>()
                {
                    { "company_code", data.company_code }, {"user_sys_id", data.user_sys_id}, { "actual_work_year", data.actual_work_year }, { "actual_work_month", data.actual_work_month }
                };

                if (Update<GroupHistory>(columns, condition) > 0)
                {
                    res = 1;
                }
            }
            else
            {
                sql = new Sql(@"
                    INSERT INTO [dbo].[enrollment_history]
                                (company_code
                                ,user_sys_id
                                ,actual_work_year
                                ,actual_work_month
                                ,group_id
                                ,location_id
                                ,remarks
                                ,ins_date
                                ,ins_id
                                ,upd_date
                                ,upd_id)
                            VALUES
                                (
                                @company_code
                                ,@user_sys_id
                                ,@actual_work_year
                                ,@actual_work_month
                                ,@group_id
                                ,@location_id
                                ,@remarks
                                ,@ins_date
                                ,@ins_id
                                ,@upd_date
                                ,@upd_id)"
                                        , new { company_code = data.company_code }
                                        , new { user_sys_id = data.user_sys_id }
                                        , new { actual_work_year = data.actual_work_year }
                                        , new { actual_work_month = data.actual_work_month }
                                        , new { group_id = data.group_id }
                                        , new { location_id = data.location_id }
                                        , new { remarks = string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                                        , new { ins_date = Utility.GetCurrentDateTime() }
                                        , new { ins_id = data.ins_id }
                                        , new { upd_date = Utility.GetCurrentDateTime() }
                                        , new { upd_id = data.upd_id }
                                        );

                if (_database.Execute(sql) > 0)
                {
                    res = 1;
                }
            }

            return res;
        }

        /// <summary>
        /// Check esixt of Group History
        /// </summary>
        /// <param name="data">obj Group History</param>
        /// <returns>Number of record is update</returns>
        private int CheckGroupHistory(GroupHistory data)
        {
            Sql sql = Sql.Builder.Append(@"SELECT Count(*) FROM enrollment_history");

            sql.Where("company_code = @company_code AND user_sys_id = @user_sys_id AND actual_work_year = @actual_work_year AND actual_work_month = @actual_work_month",
            new
            {
                @company_code = data.company_code,
                @user_sys_id = data.user_sys_id,
                @actual_work_year = data.actual_work_year,
                @actual_work_month = data.actual_work_month
            });

            return _database.SingleOrDefault<int>(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int PersonalSettingUserData(UserPlus data)
        {
            int res = 0;

            if (data.user_sys_id > 0)
            {
                IDictionary<string, object> columns;
                if (data.password == Constant.DISPLAY_PASSWORD)
                {
                    columns = new Dictionary<string, object>()
                    {
                        { "company_code", data.company_code }
                        , { "user_account", data.user_account }
                        , { "user_name_sei", string.IsNullOrEmpty(data.user_name_sei) ? null : data.user_name_sei }
                        , { "user_name_mei", string.IsNullOrEmpty(data.user_name_mei) ? null : data.user_name_mei }
                        , { "furigana_sei", string.IsNullOrEmpty(data.furigana_sei) ? null : data.furigana_sei }
                        , { "furigana_mei", string.IsNullOrEmpty(data.furigana_mei) ? null : data.furigana_mei }
                        , { "display_name", string.IsNullOrEmpty(data.display_name) ? null : data.display_name }
                        , { "tel_no", string.IsNullOrEmpty(data.tel_no) ? null : data.tel_no}
                        , { "birth_date", data.birth_date ?? null }
                        , { "image_file_path", string.IsNullOrEmpty(data.image_file_path) ? null : data.image_file_path }
                        , { "remarks", string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                        , { "upd_date", data.upd_date }
                        , { "upd_id", data.upd_id }
                        , { "language_id", data.language_id }
                        , { "actual_work_input_mode", data.actual_work_input_mode }
                    };
                }
                else
                {
                    data.password_last_update = data.upd_date;
                    columns = new Dictionary<string, object>()
                    {
                        { "company_code", data.company_code }
                        , { "user_account", data.user_account }
                        , { "password", data.password }
                        , { "user_name_sei", string.IsNullOrEmpty(data.user_name_sei) ? null : data.user_name_sei }
                        , { "user_name_mei", string.IsNullOrEmpty(data.user_name_mei) ? null : data.user_name_mei }
                        , { "furigana_sei", string.IsNullOrEmpty(data.furigana_sei) ? null : data.furigana_sei }
                        , { "furigana_mei", string.IsNullOrEmpty(data.furigana_mei) ? null : data.furigana_mei }
                        , { "display_name", string.IsNullOrEmpty(data.display_name) ? null : data.display_name }
                        , { "tel_no", string.IsNullOrEmpty(data.tel_no) ? null : data.tel_no }
                        , { "birth_date", data.birth_date ?? null }
                        , { "image_file_path", string.IsNullOrEmpty(data.image_file_path) ? null : data.image_file_path }
                        , { "remarks", string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                        , { "upd_date", data.upd_date }
                        , { "upd_id", data.upd_id }
                        , { "language_id", data.language_id }
                        , { "password_last_update", data.password_last_update }
                        , { "actual_work_input_mode", data.actual_work_input_mode }
                    };
                }


                IDictionary<string, object> condition = new Dictionary<string, object>()
                {
                    { "user_sys_id", data.user_sys_id }, {"row_version", data.row_version}
                };

                if (Update<User>(columns, condition) > 0)
                {
                    res = data.user_sys_id;
                }
            }
            return res;
        }

        public int CheckUserEmail(string email1, string email2, int userID, string companyCode)
        {
            Sql sql = Sql.Builder.Append(@"
                 SELECT COUNT(*) FROM (
                     SELECT company_code, user_sys_id, mail_address_1 as mail_address ,del_flg from m_user
                     UNION
                     SELECT company_code, user_sys_id, mail_address_2 as mail_address ,del_flg from m_user
                 ) AS u");

            sql.Where("del_flg = @del_flg", new { del_flg = Constant.DeleteFlag.NON_DELETE });
            sql.Where("u.company_code = @company_code", new { company_code = companyCode });
            sql.Where("u.mail_address <> @mail_address", new { mail_address = "" });
            sql.Where("(u.mail_address = @mail_address_1 or u.mail_address = @mail_address_2)", new { mail_address_1 = email1, mail_address_2 = email2 });
            sql.Where("user_sys_id != @UserID", new { UserID = userID });

            return _database.ExecuteScalar<int>(sql);
        }

        public int CheckUserAccount(string userAcount, string companyCode, int userID)
        {
            Sql sql = Sql.Builder.Append(@"SELECT * FROM m_user");

            sql.Where("user_account = @User_account AND company_code = @Company_code", new { @User_account = userAcount, @Company_code = companyCode });

            if (userID > 0)
                sql.Where("user_sys_id <> @UserID", new { UserID = userID });

            var user = _database.SingleOrDefault<User>(sql);

            return (user != null) ? 1 : 0;
        }

        public User CheckPassword(string userAcount, string companyCode, int userID)
        {
            Sql sql = Sql.Builder.Append(@"SELECT * FROM m_user");

            sql.Where("user_account = @User_account AND company_code = @Company_code AND user_sys_id = @UserID", new { @User_account = userAcount, @Company_code = companyCode, @UserID = userID });

            return _database.SingleOrDefault<User>(sql);
        }

        /// <summary>
        /// Get member assignment list
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <param name="pCode">Project code</param>
        /// <returns>List of member assignment</returns>
        public IList<UserPlus> GetMemberAssignmentList(string cCode, int pCode)
        {
            var sql = new Sql(@"
                SELECT
                    tb5.user_sys_id, 
                    tb5.display_name, 
                    tb5.user_name_mei, 
                    tb5.user_name_sei, 
                    tb5.furigana_mei, 
                    tb5.furigana_sei,
                    tb5.mail_address_1, 
                    tb5.mail_address_2,
                    tb5.group_id, 
                    tb5.position_id, 
                    tb5.base_unit_cost, 
                    tb5.entry_date,
                    tb5.display_name_group,
                    tb5.display_name_position
                FROM
                    member_assignment tb1 LEFT JOIN
                (
                    SELECT
                    tb2.company_code,
                    tb2.user_sys_id, 
                    tb2.display_name, 
                    tb2.user_name_mei, 
                    tb2.user_name_sei, 
                    tb2.furigana_mei, 
                    tb2.furigana_sei,
                    tb2.mail_address_1, 
                    tb2.mail_address_2,
                    tb2.group_id, 
                    tb2.position_id, 
                    tb2.base_unit_cost, 
                    tb2.entry_date,
                    tb3.display_name [display_name_group],
                    tb4.display_name [display_name_position]
                FROM
                    m_user tb2 LEFT JOIN m_group tb3
                    ON tb3.company_code = tb2.company_code
                    AND tb3.group_id = tb2.group_id LEFT JOIN m_position tb4
                    ON tb4.company_code = tb2.company_code
                    AND tb4.position_id = tb2.position_id
                WHERE
                    tb2.company_code = @company_code
                ) tb5
                    ON tb5.company_code = tb1.company_code
                    AND tb5.user_sys_id = tb1.user_sys_id
                WHERE
                    tb1.company_code = @company_code
                    AND tb1.project_sys_id = @project_sys_id
                ", new
            {
                company_code = cCode,
                project_sys_id = pCode
            });

            return this._database.Fetch<UserPlus>(sql);
        }

        /// <summary>
        /// Get list user
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List user</returns>
        public List<UserPlus> GetListUser(Condition condition, string companyCode, string orderBy, string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlUserList(companyCode, condition));
            sql.Append(") AS tbResult ");
            return this._database.Fetch<UserPlus>(sql);
        }

        /// <summary>
        /// Build SQL user list
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Sql BuildSqlUserList(string companyCode, Condition condition)
        {
            var sql = Sql.Builder.Append(@"
                    SELECT
                        tbUser.company_code,
                        tbUser.user_sys_id,
                        tbUser.display_name,
                        tbUser.employee_no,
                        (SELECT display_name 
                        FROM m_group 
                        WHERE company_code = tbUser.company_code AND group_id = tbUser.group_id) AS display_name_group,
                        (SELECT display_name 
                        FROM m_position 
                        WHERE company_code = tbUser.company_code AND position_id = tbUser.position_id) AS display_name_position,
                        (SELECT TOP(1) base_unit_cost
						FROM unit_price_history
						WHERE company_code = tbUser.company_code
								AND user_sys_id = tbUser.user_sys_id
								AND (YEAR(apply_start_date) < YEAR(GETDATE()) OR (YEAR(apply_start_date) = YEAR(GETDATE()) AND MONTH(apply_start_date) <= MONTH(GETDATE())))
						ORDER BY apply_start_date DESC
						) AS base_unit_cost,
                        tbUser.entry_date,
                        tbUser.mail_address_1,
                        CASE 
                        WHEN tbUser.password_lock_flg = '1' OR (tbUser.retirement_date IS NOT NULL AND tbUser.retirement_date < @CurrentDate) THEN N'無効'
                        ELSE N'有効'
                        END AS is_active,
                        tbUser.upd_date,
                        (SELECT display_name FROM m_user WHERE company_code = tbUser.company_code AND user_sys_id = tbUser.upd_id) AS user_update,
                        tbUser.user_name_mei,
                        tbUser.user_name_sei,
                        tbUser.furigana_mei,
                        tbUser.furigana_sei,
                        tbUser.group_id,
                        tbUser.position_id,
                        tbUser.mail_address_2,
                        tbUser.del_flg,
                        tbUser.retirement_date
                    FROM
                        m_user AS tbUser
                    WHERE
                        tbUser.company_code = @company_code", new { company_code = companyCode, CurrentDate = Utility.GetCurrentDateTime() });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.DISPLAY_NAME))
                    sql.Append(string.Format(" AND (tbUser.display_name LIKE '{0}' ESCAPE '\\' OR tbUser.user_name_sei LIKE '{0}' ESCAPE '\\' OR tbUser.user_name_mei LIKE '{0}' ESCAPE '\\' OR tbUser.furigana_sei LIKE '{0}' ESCAPE '\\' OR tbUser.furigana_mei LIKE '{0}' ESCAPE '\\' )", "%" + replaceWildcardCharacters(condition.DISPLAY_NAME) + "%"));

                if (condition.GROUP_ID != null)
                    sql.Append(string.Format(" AND tbUser.group_id = {0}", condition.GROUP_ID));

                if (condition.POSITION_ID != null)
                    sql.Append(string.Format(" AND tbUser.position_id = {0}", condition.POSITION_ID));

                if (!string.IsNullOrEmpty(condition.MAIL_ADDRESS))
                    sql.Append(string.Format(" AND (tbUser.mail_address_1 LIKE '{0}' ESCAPE '\\' OR tbUser.mail_address_2 LIKE '{0}' ESCAPE '\\')", "%" + replaceWildcardCharacters(condition.MAIL_ADDRESS) + "%"));

                if (!condition.DELETED_INCLUDE && condition.PROJECT_ID == 0)
                    sql.Append(string.Format(" AND tbUser.del_flg = " + Constant.DeleteFlag.NON_DELETE));

                if (!condition.RETIREMENT_INCLUDE)
                {
                    if (condition.PROJECT_ID > 0 && !string.IsNullOrEmpty(condition.FROM_DATE) && !string.IsNullOrEmpty(condition.TO_DATE))
                        sql.Append(string.Format(" AND (tbUser.retirement_date IS NULL OR tbUser.retirement_date >= @CurrentDate OR (tbUser.retirement_date between '" + condition.FROM_DATE + "' AND '" + condition.TO_DATE + "'))"), new { CurrentDate = Utility.GetCurrentDateTime() });
                    else
                        sql.Append(string.Format(" AND (tbUser.retirement_date IS NULL OR tbUser.retirement_date >= @CurrentDate)"), new { CurrentDate = Utility.GetCurrentDateTime() });
                }
            }
            return sql;
        }

        public List<UnitPriceHistory> GetListUnitCost(string companyCode, int userId)
        {
            var sql = new Sql(@"
                SELECT apply_start_date
                      ,base_unit_cost
                  FROM unit_price_history
                  WHERE company_code = @company_code AND user_sys_id = @user_sys_id AND del_flg = @del_flg
                ", new
            {
                company_code = companyCode,
                user_sys_id = userId,
                del_flg = Constant.DeleteFlag.NON_DELETE
            });

            return this._database.Fetch<UnitPriceHistory>(sql);
        }
    }
}