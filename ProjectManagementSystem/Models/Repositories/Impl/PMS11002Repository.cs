#region License
/// <copyright file="PMS11002Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS11002;
using System.Collections.Generic;
using System.Text;
using System;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// Group repository class
    /// </summary>
    public class PMS11002Repository : Repository, IPMS11002Repository
    {
        #region Constructor
        /// <summary>
        /// Database
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS11002Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS11002Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Public
        /// <summary>
        /// Get list Contract type to use in Search function 
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IEnumerable<ContractType> GetListContactTypeBySearch(string contractType, string companyCode)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"SELECT * FROM m_contract_type 
                            WHERE company_code = @company_code 
                            AND del_flg = @del_flg AND budget_setting_flg =@budget_setting_flg ");

            if (!string.IsNullOrEmpty(contractType))
            {
                sb.AppendFormat("AND contract_type_id IN({0})", contractType);
            }

            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCode,
                del_flg = Constant.DeleteFlag.NON_DELETE,
                budget_setting_flg = Constant.BudgetSettingFlag.OBJECT
            });

            return this._database.Fetch<ContractType>(query);
        }

        /// <summary>
        /// Get list group to use in Search function
        /// </summary>
        /// <param name="group"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IEnumerable<Group> GetListGroupBySearch(string group, string companyCode)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"SELECT * FROM m_group 
                                WHERE company_code = @company_code 
                                AND del_flg = @del_flg AND budget_setting_flg =@budget_setting_flg ");

            if (!string.IsNullOrEmpty(group))
            {
                sb.AppendFormat(" AND group_id IN({0})", group);
            }

            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCode,
                del_flg = Constant.DeleteFlag.NON_DELETE,
                budget_setting_flg = Constant.BudgetSettingFlag.OBJECT
            });

            return this._database.Fetch<Group>(query);
        }

        /// <summary>
        /// Check whether budget data is existed or not
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public bool CheckCountDataBudget(string companyCode)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"
                    SELECT 
                        COUNT(*)
                    FROM m_budget
                    WHERE company_code = @company_code ");
            var sql = new Sql(query.ToString(), new { company_code = companyCode });
            int result = this._database.FirstOrDefault<int>(sql);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether budget data is valid or not
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public bool CheckDataValid(Budget budget)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"
                    SELECT COUNT(*)
                    FROM
                        m_budget bd
                        LEFT JOIN m_group gr
                               ON bd.group_id = gr.group_id
                        LEFT JOIN m_contract_type ct
                               ON bd.contract_type_id= ct.contract_type_id
                    WHERE 
                        bd.company_code = @company_code
                        AND gr.group_id = @group_id
                        AND ct.contract_type_id =@contract_type
                        AND bd.target_month = @target_month
                        AND bd.target_year = @target_year");

            Sql sql = new Sql(query.ToString(),
                new { company_code = budget.company_code },
                new { group_id = budget.group_id },
                new { contract_type = budget.contract_type_id },
                new { target_month = budget.target_month },
                new { target_year = budget.target_year }
            );

            int result = this._database.FirstOrDefault<int>(sql);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check budget data is null or not
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public bool CheckDataIsNull(Budget budget)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"
                    SELECT COUNT(*)
                    FROM
                        m_budget bd
                    WHERE 
                        company_code = @company_code
                        AND group_id = @group_id
                        AND contract_type_id =@contract_type
                        AND target_month = @target_month
                        AND target_year = @target_year
                        AND sales_budget IS NULL
                        AND profit_budget IS NULL");
            Sql sql = new Sql(query.ToString(),
                new { company_code = budget.company_code },
                new { group_id = budget.group_id },
                new { contract_type = budget.contract_type_id },
                new { target_month = budget.target_month },
                new { target_year = budget.target_year }
            );

            int result = this._database.FirstOrDefault<int>(sql);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Insert budget data into DB
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public int InsertBudgetInfo(Budget budget)
        {
            int result = 0;
            var sqlInsert = new Sql(@"
                    INSERT INTO
                        m_budget
                           (company_code,target_year,target_month,group_id,contract_type_id,
                            sales_budget,profit_budget,display_order,ins_date,ins_id,upd_date,upd_id,del_flg)
                    VALUES(
                        @company_code,@target_year,@target_month,@group_id,@contract_type_id,
                        @sales_budget,@profit_budget,@display_order,@ins_date,@ins_id,@upd_date,@upd_id,@del_flg)",
                        new
                        {
                            company_code = budget.company_code,
                            target_year = budget.target_year,
                            target_month = budget.target_month,
                            group_id = budget.group_id,
                            contract_type_id = budget.contract_type_id,
                            sales_budget = budget.sales_budget,
                            profit_budget = budget.profit_budget,
                            display_order = 0,
                            ins_date = budget.ins_date,
                            ins_id = budget.ins_id,
                            upd_date = budget.upd_date,
                            upd_id = budget.upd_id,
                            del_flg = "0"
                        });
            result = this._database.Execute(sqlInsert);
            return result;
        }

        /// <summary>
        /// Delete budget data from DB
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public int DeleteBudget(Budget budget)
        {
            int result = 0;
            var sqlDel = new Sql(@"
                    delete 
                    from m_budget 
                    where company_code = @company_code and target_month = @target_month  and target_year = @target_year and contract_type_id = @contract_type_id and group_id = @group_id",
                        new
                        {
                            company_code = budget.company_code,
                            target_year = budget.target_year,
                            target_month = budget.target_month,
                            group_id = budget.group_id,
                            contract_type_id = budget.contract_type_id,
                        });
            result = this._database.Execute(sqlDel);
            return result;
        }

        /// <summary>
        /// Process update function for list of budget data
        /// </summary>
        /// <param name="listBudget"></param>
        /// <returns></returns>
        public bool ProcessUpdateBudget(IList<Budget> listBudget)
        {
            var result = false;

            foreach (var budget in listBudget)
            {
                if (CheckDataValid(budget))
                {
                    result = UpdateBudgetInfo(budget) == 0 ? false : true;
                    if (result == true)
                    {
                        if (CheckDataIsNull(budget))
                        {
                            DeleteBudget(budget);
                        }
                    }
                    if (!result) return false;
                }
                else
                {
                    result = InsertBudgetInfo(budget) == 0 ? false : true;
                    if (!result) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update budget data
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public int UpdateBudgetInfo(Budget budget)
        {
            int result = 0;
            var queryUpdate = new Sql();
            StringBuilder query = new StringBuilder();
            query.Append(@"
                    UPDATE 
                        m_budget
                    SET 
                        ins_date = @ins_date,
                        ins_id = @ins_id,
                        upd_date = @upd_date,
                        upd_id = @upd_id,
                        del_flg = @del_flg,");

            if (budget.del_flg == "sales")
            {
                query.Append(@"
                        sales_budget = @sales_budget");
            }

            if (budget.del_flg == "profit")
            {
                query.Append(@"
                        profit_budget = @profit_budget");
            }

            query.Append(@"
                    WHERE 
                       company_code = @company_code AND 
                       target_year = @target_year AND 
                       target_month = @target_month AND 
                       group_id = @group_id AND 
                       contract_type_id = @contract_type_id");
            queryUpdate = new Sql(query.ToString(),
                new
                {
                    company_code = budget.company_code,
                    target_year = budget.target_year,
                    target_month = budget.target_month,
                    group_id = budget.group_id,
                    contract_type_id = budget.contract_type_id,
                    sales_budget = budget.sales_budget,
                    profit_budget = budget.profit_budget,
                    ins_date = budget.ins_date,
                    ins_id = budget.ins_id,
                    upd_date = budget.upd_date,
                    upd_id = budget.upd_id,
                    del_flg = "0"
                });
            result = this._database.Execute(queryUpdate);

            return result;
        }

        /// <summary>
        /// Get List Budget data to use in Search Function
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="group"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public IEnumerable<BudgetPlus> GetListBudgetBySearch(string contractType, string group, string month, string year, string companyCode = "", List<TimeListBudget> timeList = null)
        {
            var sb = new StringBuilder();
            if (timeList != null)
            {
                for (int i = 0; i < timeList.Count; i++)
                {
                    sb.AppendFormat(@"
                    SELECT * FROM m_budget 
                        WHERE company_code = @company_code 
                        AND del_flg = @del_flg ");
                    sb.AppendFormat("AND target_year = {0} ", timeList[i].target_year);
                    sb.AppendFormat("AND target_month = {0} ", timeList[i].target_month);

                    if (!string.IsNullOrEmpty(contractType))
                    {
                        sb.AppendFormat("AND contract_type_id IN({0}) ", contractType);
                    }

                    if (!string.IsNullOrEmpty(group))
                    {
                        sb.AppendFormat("AND group_id IN({0}) ", group);
                    }
                    if (i < timeList.Count - 1)
                    {
                        sb.AppendFormat("  UNION ALL");
                    }

                }
            }
            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCode,
                del_flg = Constant.DeleteFlag.NON_DELETE
            });

            return this._database.Fetch<BudgetPlus>(query);
        }

        /// <summary>
        /// Get list budget data of total year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="companyCode"></param>
        /// <param name="contract_type"></param>
        /// <returns></returns>
        public IList<dynamic> GetListTotalYearBySearch(string year, string companyCode = "", string contract_type = "")
        {
            int getClosingMonth = Convert.ToInt32(GetAccountClosingMonth(companyCode));
            int closingMonthFrom = getClosingMonth + 1;
            int closingMonthTo = getClosingMonth;
            int from_year = Convert.ToInt32(year);
            int to_year = from_year + 1;
            if (getClosingMonth == 12)
            {
                to_year = from_year;
                closingMonthFrom = 1;
                closingMonthTo = 12;
            }

            var sb = new StringBuilder();
            sb.AppendFormat(@"
                SELECT mb.group_id
                       , SUM(sales_budget) AS [total_sales]
                       , SUM(profit_budget) AS [total_profit] 
                FROM m_budget AS mb
					INNER JOIN m_contract_type as mct
					ON mb.contract_type_id = mct.contract_type_id
                    INNER JOIN m_group as mg
                    ON mb.group_id = mg.group_id
                WHERE mb.company_code='" + companyCode + @"'
                      AND mct.budget_setting_flg = '1'
                      AND mg.budget_setting_flg = '1'
                      AND ((target_month >= " + closingMonthFrom + " AND target_year = " + from_year + @")
                          OR(target_month <= " + closingMonthTo + " AND target_year = " + to_year + @" ))");
            if (!string.IsNullOrEmpty(contract_type))
            {
                sb.AppendFormat(" AND mb.contract_type_id in ({0})", contract_type);
            }
            sb.AppendFormat("  group by mb.group_id");
            var query = new Sql(sb.ToString());
            return this._database.Fetch<dynamic>(query);
        }

        /// <summary>
        /// Get account closing month
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetAccountClosingMonth(string companyCode)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    SELECT account_closing_month FROM m_company_setting 
                        WHERE company_code = @company_code ");

            var query = new Sql(sb.ToString(), new
            {
                company_code = companyCode
            });
            return this._database.SingleOrDefault<int>(query);
        }

        #endregion

        #region Private

        #endregion
    }
}
