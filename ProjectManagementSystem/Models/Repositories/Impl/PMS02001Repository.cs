using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS02001;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    public class PMS02001Repository : Repository, IPMS02001Repository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS02001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS02001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// Search list customer
        /// </summary>
        /// <param name="startItem">startItem</param>
        /// <param name="itemsPerPage">itemsPerPage</param>
        /// <param name="columns">columns</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <returns>List customer</returns>
        public PageInfo<CustomerPlus> Search(int startItem, int itemsPerPage, string columns, int? sortCol, string sortDir, PMS02001.Condition condition, string companyCode)
        {
            Sql sql = new Sql();

            if (condition.PROJECT_ID > 0 && !condition.DELETED_INCLUDE)
            {
                sql = Sql.Builder.Append(@"
                    SELECT * FROM (
                        SELECT
                            ISNULL(tbCustomer.customer_id, tbProjectCustomer.customer_id) AS customer_id, 
                            ISNULL(tbCustomer.customer_name, tbProjectCustomer.customer_name) AS customer_name, 
                            ISNULL(tbCustomer.display_name, tbProjectCustomer.display_name) AS display_name, 
                            ISNULL(tbCustomer.customer_name_kana, tbProjectCustomer.customer_name_kana) AS customer_name_kana, 
                            ISNULL(tbCustomer.url, tbProjectCustomer.url) AS url, 
                            ISNULL(tbCustomer.[address], tbProjectCustomer.[address]) AS [address], 
                            ISNULL(tbCustomer.del_flg, tbProjectCustomer.del_flg) AS del_flg, 
                            ISNULL(tbCustomer.upd_date, tbProjectCustomer.upd_date) AS upd_date, 
                            ISNULL(tbCustomer.[user_update], tbProjectCustomer.[user_update]) AS [user_update]
                        FROM (
                            SELECT 
                                tl1.company_code, 
                                tl1.customer_id, 
                                tl1.customer_name, 
                                tl1.display_name, 
                                tl1.customer_name_kana, 
                                tl1.url, 
                                (ISNULL((SELECT prefecture_name
                                FROM m_prefecture
                                WHERE prefecture_code = tl1.prefecture_code), '')  + ' ' + ISNULL(tl1.city,'') + ' ' + ISNULL(tl1.address_1,'') + ' ' + ISNULL(tl1.address_2,'')) AS [address],
                                tl1.del_flg, 
                                tl1.upd_date, 
                                (SELECT display_name from m_user WHERE user_sys_id = tl1.upd_id ) [user_update]
                            FROM
                                m_customer tl1 
                            WHERE
                                tl1.company_code = @company_code
                                AND del_flg = '0'
                            ) AS tbCustomer FULL JOIN (
                    ", new { company_code = companyCode });

                if (condition.SearchByObject == SearchObject.Customer)
                {
                    sql.Append(@"
                    SELECT DISTINCT
                                        tbSalesPayment.company_code, 
                                        tbSalesPayment.customer_id,
                                        tbCusInfo.customer_name, 
                                        tbCusInfo.display_name,  
                                        tbCusInfo.customer_name_kana, 
                                        tbCusInfo.url, 
                                        (ISNULL((SELECT prefecture_name
                                        FROM m_prefecture
                                        WHERE prefecture_code = tbCusInfo.prefecture_code), '')  + ' ' + ISNULL(tbCusInfo.city,'') + ' ' + ISNULL(tbCusInfo.address_1,'') + ' ' + ISNULL(tbCusInfo.address_2,'')) AS [address],
                                        tbCusInfo.del_flg, 
                                        tbCusInfo.upd_date, 
                                        (SELECT display_name from m_user WHERE user_sys_id = tbCusInfo.upd_id ) [user_update]
                                    FROM sales_payment AS tbSalesPayment INNER JOIN m_customer AS tbCusInfo
                    ON tbCusInfo.company_code = tbSalesPayment.company_code
                    AND tbCusInfo.customer_id = tbSalesPayment.customer_id
                    ", new { company_code = companyCode, project_sys_id = condition.PROJECT_ID });
                }
                else if(condition.SearchByObject == SearchObject.EndUser)
                {
                    sql.Append(@"
                    SELECT DISTINCT
                                        tbSalesPayment.company_code, 
                                        tbSalesPayment.end_user_id as [customer_id],
                                        tbCusInfo.customer_name, 
                                        tbCusInfo.display_name,  
                                        tbCusInfo.customer_name_kana, 
                                        tbCusInfo.url, 
                                        (ISNULL((SELECT prefecture_name
                                        FROM m_prefecture
                                        WHERE prefecture_code = tbCusInfo.prefecture_code), '')  + ' ' + ISNULL(tbCusInfo.city,'') + ' ' + ISNULL(tbCusInfo.address_1,'') + ' ' + ISNULL(tbCusInfo.address_2,'')) AS [address],
                                        tbCusInfo.del_flg, 
                                        tbCusInfo.upd_date, 
                                        (SELECT display_name from m_user WHERE user_sys_id = tbCusInfo.upd_id ) [user_update]
                                    FROM sales_payment AS tbSalesPayment INNER JOIN m_customer AS tbCusInfo
                        ON tbCusInfo.company_code = tbSalesPayment.company_code
                        AND tbCusInfo.customer_id = tbSalesPayment.end_user_id
                    ", new { company_code = companyCode, project_sys_id = condition.PROJECT_ID });
                }

                sql.Append(@"
                                    WHERE tbSalesPayment.company_code = @company_code
                                    AND project_sys_id = @project_sys_id
                                ) AS tbProjectCustomer
                                ON tbProjectCustomer.company_code = tbCustomer.company_code
                                AND tbProjectCustomer.[customer_id] = tbCustomer.customer_id
                                ) AS tbResult WHERE 1 = 1
                ", new { company_code = companyCode, project_sys_id = condition.PROJECT_ID });

                if (condition != null)
                {
                    if (!string.IsNullOrEmpty(condition.DISPLAY_NAME))
                        sql.Append(string.Format(" AND (tbResult.display_name LIKE '{0}' ESCAPE '\\' OR tbResult.customer_name LIKE '{0}' ESCAPE '\\') ", "%" + replaceWildcardCharacters(condition.DISPLAY_NAME) + "%"));

                    if (!string.IsNullOrEmpty(condition.CUSTOMER_NAME_KATA))
                        sql.Append(string.Format(" AND tbResult.customer_name_kana LIKE '{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.CUSTOMER_NAME_KATA) + "%"));

                    if (!condition.DELETED_INCLUDE && condition.PROJECT_ID == 0)
                        sql.Append(string.Format(" AND tbResult.del_flg = " + Constant.DeleteFlag.NON_DELETE));
                }
            }
            else
            {
                sql = new Sql("SELECT * FROM ( ");

                sql.Append(BuildSqlCustomerList(companyCode, condition));
                sql.Append(") AS tbResult ");
            }

            if (sortCol.HasValue && sortCol < 2)
            {
                sortDir = "desc";
            }

            string firstColumns = "upd_date,upd_date,display_name,";
            string myString = columns.Remove(columns.IndexOf(firstColumns), firstColumns.Length);

            columns = firstColumns + myString;

            var pageInfo = Page<CustomerPlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Get customer info with customr id and company code
        /// </summary>
        /// <param name="cCode">company code</param>
        /// <param name="customerId">customer id</param>
        /// <returns>Customer info</returns>
        public CustomerPlus GetCustomerInfo(string cCode, int customerId)
        {
            var sql = new Sql(
                @"
                SELECT 
                    tb1.*,
                    (SELECT display_name from m_user WHERE user_sys_id = tb1.upd_id ) [user_update],
                    (SELECT display_name from m_user WHERE user_sys_id = tb1.ins_id ) [user_regist]
                FROM
                    m_customer tb1
                WHERE
                    tb1.company_code = @company_code
                    and tb1.customer_id = @customer_id",
                new
                {
                    company_code = cCode,
                    customer_id = customerId
                }
            );
            return this._database.SingleOrDefault<CustomerPlus>(sql);
        }

        /// <summary>
        /// Get Prefecture List 
        /// </summary>
        /// <returns>List Prefeture</returns>
        public IEnumerable<Prefecture> GetPrefectureList()
        {
            var condition = new Dictionary<string, object>
            {
                {
                    "del_flg", Constant.DeleteFlag.NON_DELETE
                }
            };

            return this.Select<Prefecture>(condition, "display_order");
        }

        /// <summary>
        /// Edit Customer Info
        /// </summary>
        /// <param name="data">obj Customer</param>
        /// <returns>Number of record is edit</returns>
        public int EditCustomerData(Customer data)
        {
            int res = 0;
            Sql sql;

            if (data.customer_id > 0)
            {
                IDictionary<string, object> columns = new Dictionary<string, object>()
                {
                    { "company_code", data.company_code }
                    , { "customer_name", string.IsNullOrEmpty(data.customer_name) ? null : data.customer_name }
                    , { "customer_name_kana", string.IsNullOrEmpty(data.customer_name_kana) ? null : data.customer_name_kana }
                    , { "customer_name_en", string.IsNullOrEmpty(data.customer_name_en) ? null : data.customer_name_en }
                    , { "display_name", string.IsNullOrEmpty(data.display_name) ? null : data.display_name }
                    , { "zip_code", string.IsNullOrEmpty(data.zip_code) ? null : data.zip_code }
                    , { "prefecture_code", string.IsNullOrEmpty(data.prefecture_code) ? null : data.prefecture_code }
                    , { "city", string.IsNullOrEmpty(data.city) ? null : data.city }
                    , { "address_1", string.IsNullOrEmpty(data.address_1) ? null : data.address_1 }
                    , { "address_2", string.IsNullOrEmpty(data.address_2) ? null : data.address_2 }
                    , { "tel_no", string.IsNullOrEmpty(data.tel_no) ? null : data.tel_no }
                    , { "fax_no", string.IsNullOrEmpty(data.fax_no) ? null : data.fax_no }
                    , { "mail_address", string.IsNullOrEmpty(data.mail_address) ? null : data.mail_address }
                    , { "url", string.IsNullOrEmpty(data.url) ? null : data.url }
                    , { "logo_image_file_path", string.IsNullOrEmpty(data.logo_image_file_path) ? null : data.logo_image_file_path }
                    , { "remarks", string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                    , { "upd_date", data.upd_date }
                    , { "upd_id", data.upd_id }
                    , { "del_flg", data.del_flg }
                };

                IDictionary<string, object> condition;
                if (data.row_version == null)
                {
                    condition = new Dictionary<string, object>()
                    {
                        { "customer_id", data.customer_id }
                    };
                }
                else
                {
                    condition = new Dictionary<string, object>()
                    {
                        { "customer_id", data.customer_id }, {"row_version", data.row_version}
                    };
                }

                if (Update<Customer>(columns, condition) > 0)
                {
                    res = data.customer_id;
                }
            }
            else
            {
                data.ins_date = data.upd_date;
                data.ins_id = data.upd_id;

                sql = new Sql(@"
                INSERT INTO [dbo].[m_customer]
                           (company_code
                           ,customer_name
                           ,customer_name_kana
                           ,customer_name_en
                           ,display_name
                           ,zip_code
                           ,prefecture_code
                           ,city
                           ,address_1
                           ,address_2
                           ,tel_no
                           ,fax_no
                           ,mail_address
                           ,url
                           ,logo_image_file_path
                           ,remarks
                           ,ins_date
                           ,ins_id
                           ,upd_date
                           ,upd_id
                           ,del_flg)
                     VALUES
                           (
                            @company_code
                           ,@customer_name
                           ,@customer_name_kana
                           ,@customer_name_en
                           ,@display_name
                           ,@zip_code
                           ,@prefecture_code
                           ,@city
                           ,@address_1
                           ,@address_2
                           ,@tel_no
                           ,@fax_no
                           ,@mail_address
                           ,@url
                           ,@logo_image_file_path
                           ,@remarks
                           ,@ins_date
                           ,@ins_id
                           ,@upd_date
                           ,@upd_id
                           ,@del_flg)"
                    , new { company_code = data.company_code }
                    , new { customer_name = string.IsNullOrEmpty(data.customer_name) ? null : data.customer_name }
                    , new { customer_name_kana = string.IsNullOrEmpty(data.customer_name_kana) ? null : data.customer_name_kana }
                    , new { customer_name_en = string.IsNullOrEmpty(data.customer_name_en) ? null : data.customer_name_en }
                    , new { display_name = string.IsNullOrEmpty(data.display_name) ? null : data.display_name }
                    , new { zip_code = string.IsNullOrEmpty(data.zip_code) ? null : data.zip_code }
                    , new { prefecture_code = string.IsNullOrEmpty(data.prefecture_code) ? null : data.prefecture_code }
                    , new { city = string.IsNullOrEmpty(data.city) ? null : data.city }
                    , new { address_1 = string.IsNullOrEmpty(data.address_1) ? null : data.address_1 }
                    , new { address_2 = string.IsNullOrEmpty(data.address_2) ? null : data.address_2 }
                    , new { tel_no = string.IsNullOrEmpty(data.tel_no) ? null : data.tel_no }
                    , new { fax_no = string.IsNullOrEmpty(data.fax_no) ? null : data.fax_no }
                    , new { mail_address = string.IsNullOrEmpty(data.mail_address) ? null : data.mail_address }
                    , new { url = string.IsNullOrEmpty(data.url) ? null : data.url }
                    , new { logo_image_file_path = string.IsNullOrEmpty(data.logo_image_file_path) ? null : data.logo_image_file_path }
                    , new { remarks = string.IsNullOrEmpty(data.remarks) ? null : data.remarks }
                    , new { ins_date = data.ins_date }
                    , new { ins_id = data.ins_id }
                    , new { upd_date = data.upd_date }
                    , new { upd_id = data.upd_id }
                    , new { del_flg = Constant.DeleteFlag.NON_DELETE }
                 );

                if (_database.Execute(sql) > 0)
                {
                    var query = "select ident_current('m_customer')";
                    res = _database.ExecuteScalar<int>(query);
                }
            }

            return res;
        }

        /// <summary>
        /// Check mail of Customer
        /// </summary>
        /// <param name="email">email of Customer</param>
        /// <param name="customerId">customer ID</param>
        /// <returns>Number of record in table Customer</returns>
        public int CheckCustomerEmail(string email, int customerId)
        {
            Sql sql = Sql.Builder.Append(@"SELECT * FROM m_customer");
            sql.Where("mail_address = @Email", new { Email = email });

            if (customerId > 0)
                sql.Where("customer_id <> @CustomerID", new { CustomerID = customerId });

            var customer = _database.SingleOrDefault<Customer>(sql);

            return (customer != null) ? 1 : 0;
        }

        /// <summary>
        /// Get list infomation of Customer
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="companyCode">companyCode</param>
        /// <param name="orderBy">orderBy</param>
        /// <param name="orderType">orderType</param>
        /// <returns>List infomation of customer</returns>
        public List<CustomerPlus> GetListCustomer(Condition condition, string companyCode, string orderBy, string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlCustomerList(companyCode, condition));
            sql.Append(") AS tbResult ");

            return this._database.Fetch<CustomerPlus>(sql);
        }

        /// <summary>
        /// Build SQL customer list
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Sql BuildSqlCustomerList(string companyCode, Condition condition)
        {
            var sql = Sql.Builder.Append(@"
                        SELECT 
                            tl1.customer_id, tl1.display_name,  tl1.customer_name_kana, tl1.url, tl1.del_flg, tl1.customer_name,  tl1.upd_date, (SELECT display_name from m_user WHERE user_sys_id = tl1.upd_id ) [user_update],
                            (ISNULL((SELECT prefecture_name
                            FROM m_prefecture
                            WHERE prefecture_code = tl1.prefecture_code), '') + ' ' + ISNULL(tl1.city,'') + ' ' + ISNULL(tl1.address_1,'') + ' ' + ISNULL(tl1.address_2,''))  [address]
                        FROM
                            m_customer tl1 LEFT JOIN m_prefecture tl2
                            ON tl2.prefecture_code = tl1.prefecture_code
                        WHERE
                            tl1.company_code = @company_code
                ", new { company_code = companyCode });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.DISPLAY_NAME))
                    sql.Append(string.Format(" AND (tl1.display_name LIKE '{0}' ESCAPE '\\' OR tl1.customer_name LIKE '{0}' ESCAPE '\\') ", "%" + replaceWildcardCharacters(condition.DISPLAY_NAME) + "%"));

                if (!string.IsNullOrEmpty(condition.CUSTOMER_NAME_KATA))
                    sql.Append(string.Format(" AND tl1.customer_name_kana LIKE '{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.CUSTOMER_NAME_KATA) + "%"));

                if (!condition.DELETED_INCLUDE)
                    sql.Append(string.Format(" AND tl1.del_flg = " + Constant.DeleteFlag.NON_DELETE));
            }

            return sql;
        }
    }
}