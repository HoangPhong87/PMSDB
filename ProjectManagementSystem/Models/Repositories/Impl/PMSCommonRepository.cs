#region License
/// <copyright file="PMSCommonRepository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/11/05</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Common repository class
    /// </summary>
    public class PMSCommonRepository : Repository, IPMSCommonRepository
    {
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMSCommonRepository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMSCommonRepository(PMSDatabase database)
        {
            this._database = database;
        }

        /// <summary>
        /// Get all status
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of status</returns>
        public IEnumerable<Status> GetStatusList(string companyCode)
        {
            var condition = new Dictionary<string, object>
                                {
                                    {
                                        "company_code", companyCode
                                    },
                                    {
                                         "del_flg", Constant.DeleteFlag.NON_DELETE
                                    }
                                };

            return this.Select<Status>(condition, "display_order");
        }

        /// <summary>
        /// Get all contract type
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="isFilter">Filter by budget_setting_flg or not</param>
        /// <returns>List of contract type</returns>
        public IEnumerable<ContractType> GetContractTypeList(string companyCode, bool isFilter)
        {
            IEnumerable<ContractType> results =null;
            if (isFilter)
            {
                var newCondition = new Dictionary<string, object>
                                {
                                    {
                                        "company_code", companyCode
                                    },
                                    {
                                        "del_flg", Constant.DeleteFlag.NON_DELETE
                                    },
                                    {
                                        "budget_setting_flg", Constant.BudgetSettingFlag.OBJECT
                                    }
                                };
                results = this.Select<ContractType>(newCondition, "display_order");
            }
            else
            {
                var condition = new Dictionary<string, object>
                                {
                                    {
                                        "company_code", companyCode
                                    },
                                    {
                                        "del_flg", Constant.DeleteFlag.NON_DELETE
                                    }
                                };
                results = this.Select<ContractType>(condition, "display_order");
            }

            return results;
        }

        /// <summary>
        /// Get all project rank
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of project rank</returns>
        public IEnumerable<Rank> GetRankList(string companyCode)
        {
            var condition = new Dictionary<string, object>
                                {
                                    {
                                        "company_code", companyCode
                                    },
                                    {
                                        "del_flg", Constant.DeleteFlag.NON_DELETE
                                    }
                                };

            return this.Select<Rank>(condition, "display_order");
        }

        /// <summary>
        /// Get user group list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="isFilter">Filter by budget_setting_flg or not</param>
        /// <returns>List of user group</returns>
        public IEnumerable<Group> GetUserGroupList(string companyCode, bool isFilter = false)
        {
            IEnumerable<Group> results = null;
            if (isFilter)
            {
                var newCondition = new Dictionary<string, object>
                                {
                                    {
                                        "company_code", companyCode
                                    },
                                    {
                                        "del_flg", Constant.DeleteFlag.NON_DELETE
                                    },
                                    {
                                        "budget_setting_flg", Constant.BudgetSettingFlag.OBJECT
                                    }
                                };
                results = this.Select<Group>(newCondition, "display_order");
            }
            else
            {
                var condition = new Dictionary<string, object>
                                {
                                    {
                                        "company_code", companyCode
                                    },
                                    {
                                        "del_flg", Constant.DeleteFlag.NON_DELETE
                                    }
                                };
                results = this.Select<Group>(condition, "display_order");
            }

            return results;
        }

        /// <summary>
        /// Get branch list
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
        /// Get tag list of customer
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IList<CustomerTag> GetTagList(string companyCode, int customerId)
        {
            var sql = new Sql(@"
                SELECT
                    tag_id, 
                    tag_name
                FROM
                    m_customer_tag
                WHERE
                    company_code = @company_code
                    AND customer_id = @customer_id
                    AND del_flg = @del_flg
                ORDER BY 
                    display_order
                ",
                 new
                 {
                     company_code = companyCode,
                     customer_id = customerId,
                     del_flg = Constant.DeleteFlag.NON_DELETE
                 });

            return this._database.Fetch<CustomerTag>(sql);
        }

        /// <summary>
        /// Get all Prefecture
        /// </summary>
        /// <returns>List of Prefecture</returns>
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
        /// Check limit data by license
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="dataType"></param>
        /// <param name="dataImportQuantity"></param>
        /// <returns></returns>
        public int CheckValidUpdateData(string companyCode, string dataType, int? dataImportQuantity)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"
                SELECT
                    CASE
                        WHEN tbItem.data_quantity = 0 THEN 1
                        WHEN tbItem.data_quantity > (SELECT COUNT(*) ");

            if (dataImportQuantity.HasValue)
            {
                sb.AppendFormat(" + {0} ", dataImportQuantity.Value);
            }

            sb.Append(" FROM ");

            switch (dataType)
            {
                case Constant.LicenseDataType.USER:
                    sb.Append("m_user ");
                    break;
                case Constant.LicenseDataType.CUSTOMER:
                    sb.Append("m_customer ");
                    break;
                case Constant.LicenseDataType.GROUP:
                    sb.Append("m_group ");
                    break;
                case Constant.LicenseDataType.CONTRACT_TYPE:
                    sb.Append("m_contract_type ");
                    break;
                default:
                    sb.Append("m_phase ");
                    break;
            }

            var sql = new Sql(sb.ToString());

            sql.Append(@"
                            WHERE company_code = @company_code AND del_flg = '0') THEN 1
                        ELSE 0
                    END valid
                FROM
                    contract_plan_info tbContract
                    LEFT JOIN plan_item_setting tbItem
                        ON tbContract.plan_id = tbItem.plan_id
                        AND tbContract.version_no = tbItem.version_no
                        AND data_type = @data_type
                WHERE
                    company_code = @company_code",
                 new
                 {
                     company_code = companyCode,
                     data_type = dataType,
                     dataImportQuantity = dataImportQuantity
                 });

            return this._database.SingleOrDefault<int>(sql);
        }

        /// <summary>
        /// get project sys id by project no
        /// </summary>
        /// <param name="projectNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public string getProjectIdByProjectNo(string projectNo, string companyCode)
        {
            var sql = new Sql(@"
                 SELECT
                    project_sys_id
                 FROM
                    project_info
                 WHERE
                    project_no = @project_no
                    AND company_code = @company_code",
                 new
                 {
                     project_no = projectNo,
                     company_code = companyCode,
                 });

            var project_sys_id = this._database.FirstOrDefault<string>(sql);
            return project_sys_id;
        }
    }
}
