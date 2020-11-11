#region License
/// <copyright file="PMS03001Repository.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

using PetaPoco;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS03001;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.Repositories.Impl
{
    /// <summary>
    /// ContractType repository class
    /// </summary>
    public class PMS03001Repository : Repository, IPMS03001Repository
    {
        #region Constructor
        private readonly Database _database;

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS03001Repository()
            : this(new PMSDatabase(Constant.CONNECTION_STRING_NAME))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">PetaPoco database class</param>
        public PMS03001Repository(PMSDatabase database)
        {
            this._database = database;
        }

        #endregion

        #region Public
        /// <summary>
        /// Search contractType by condition
        /// </summary>
        /// <param name="startItem">Start item</param>
        /// <param name="itemsPerPage">Item per page</param>
        /// <param name="columns">List of colum name</param>
        /// <param name="sortCol">Sort by colum</param>
        /// <param name="sortDir">Sort type</param>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Search result</returns>
        public PageInfo<ContractTypePlus> Search(int startItem,
            int itemsPerPage,
            string columns,
            int? sortCol,
            string sortDir,
            string companyCode,
            Condition condition)
        {
            var sql = new Sql("SELECT * FROM ( ");

            sql.Append(BuildSqlContractTypeList(companyCode, condition));
            sql.Append(") AS tbResult ");

            if (sortCol == 0)
            {
                sortCol = 6;
                sortDir = "desc";
            }

            var pageInfo = Page<ContractTypePlus>(startItem, itemsPerPage, columns, sortCol, sortDir, sql);

            return pageInfo;
        }

        /// <summary>
        /// Export ContractType List To CSV file
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="orderType">Order type</param>
        /// <returns>ContractType list</returns>
        public IList<ContractTypePlus> ExportContractTypeListToCSV(string companyCode,
            Condition condition,
            string orderBy,
            string orderType)
        {
            var sql = new Sql(string.Format("SELECT ROW_NUMBER() OVER (order by {0} {1}) peta_rn, * FROM ( ", orderBy, orderType));

            sql.Append(BuildSqlContractTypeList(companyCode, condition));
            sql.Append(") AS tbResult ");

            return this._database.Fetch<ContractTypePlus>(sql);
        }

        /// <summary>
        /// Get contractType info by ID
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>ContractType info</returns>
        public ContractTypePlus GetContractTypeInfo(string companyCode, int contractTypeID)
        {
            var sql = new Sql(@"
              SELECT 
                tbContractType.contract_type_id,
                tbContractType.contract_type,
                tbContractType.charge_of_sales_flg,
                tbContractType.exceptional_calculate_flg,
                budget_setting_flg = ISNULL(tbContractType.budget_setting_flg,0),
                tbContractType.check_plan_flg,
                tbContractType.check_progress_flg,
                tbContractType.check_period_flg,
                tbContractType.check_sales_flg,
                tbContractType.display_order,
                tbContractType.remarks,
                tbContractType.ins_date,
                (SELECT display_name FROM m_user WHERE company_code = tbContractType.company_code AND user_sys_id = tbContractType.ins_id) AS ins_user,
                tbContractType.upd_date,
                (SELECT display_name FROM m_user WHERE company_code = tbContractType.company_code AND user_sys_id = tbContractType.upd_id) AS upd_user,
                tbContractType.del_flg
            FROM
                m_contract_type AS tbContractType
            WHERE
                tbContractType.company_code = @company_code
                AND tbContractType.contract_type_id = @contract_type_id ", new { company_code = companyCode, contract_type_id = contractTypeID });

            return this._database.FirstOrDefault<ContractTypePlus>(sql);
        }

        /// <summary>
        /// Get contract type phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>List of contract type phase</returns>
        public IList<ContractTypePhasePlus> GetContractTypePhaseList(string companyCode, int contractTypeID)
        {
            var sql = new Sql(@"
                SELECT
                    tbContractTypePhase.contract_type_id,
                    tbContractTypePhase.phase_id,
                    tbContractTypePhase.display_order,
                    ISNULL(tbTargetPhase.project_count, 0) AS project_count,
                    (SELECT display_name FROM m_phase WHERE company_code = tbContractTypePhase.company_code AND phase_id = tbContractTypePhase.phase_id) AS phase_name
                FROM
                    r_contract_type_phase AS tbContractTypePhase
                    LEFT JOIN (
                        SELECT company_code, phase_id, COUNT(project_sys_id) AS project_count
                        FROM target_phase
                        WHERE company_code = @company_code
                        AND (SELECT contract_type_id
                            FROM project_info
                            WHERE company_code = @company_code
                            AND project_sys_id = target_phase.project_sys_id) = @contract_type_id
                        GROUP BY company_code, phase_id
                    ) AS tbTargetPhase
                    ON tbContractTypePhase.company_code = tbTargetPhase.company_code
                    AND tbContractTypePhase.phase_id = tbTargetPhase.phase_id
                WHERE
                    tbContractTypePhase.company_code = @company_code
                    AND tbContractTypePhase.contract_type_id = @contract_type_id
                ORDER BY 
                    tbContractTypePhase.display_order "
                , new { company_code = companyCode, contract_type_id = contractTypeID });

            return this._database.Fetch<ContractTypePhasePlus>(sql);
        }

        /// <summary>
        /// Get contract type category list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">ContractType ID</param>
        /// <returns>List of contract type category</returns>
        public IList<ContractTypeCategoryPlus> GetContractTypeCategoryList(string companyCode, int contractTypeID)
        {
            var sql = new Sql(@"
                SELECT
                    tbContractTypeCategory.contract_type_id,
                    tbContractTypeCategory.category_id,
                    tbContractTypeCategory.display_order,
                    ISNULL(tbTargetCategory.project_count, 0) AS project_count,
                    (SELECT category FROM m_category WHERE company_code = tbContractTypeCategory.company_code AND category_id = tbContractTypeCategory.category_id) AS category
                FROM
                    r_contract_type_category AS tbContractTypeCategory
                    LEFT JOIN (
                        SELECT company_code, category_id, COUNT(project_sys_id) AS project_count
                        FROM target_category
                        WHERE company_code = @company_code
                        AND (SELECT contract_type_id
                            FROM project_info
                            WHERE company_code = @company_code
                            AND project_sys_id = target_category.project_sys_id) = @contract_type_id
                        GROUP BY company_code, category_id
                    ) AS tbTargetCategory
                    ON tbContractTypeCategory.company_code = tbTargetCategory.company_code
                    AND tbContractTypeCategory.category_id = tbTargetCategory.category_id
                WHERE
                    tbContractTypeCategory.company_code = @company_code
                    AND tbContractTypeCategory.contract_type_id = @contract_type_id
                ORDER BY 
                    tbContractTypeCategory.display_order "
                , new { company_code = companyCode, contract_type_id = contractTypeID });

            return this._database.Fetch<ContractTypeCategoryPlus>(sql);
        }

        /// <summary>
        /// Get phase list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of phase</returns>
        public IList<Phase> GetPhaseList(string companyCode, int contractTypeID)
        {
            var sql = new Sql(@"
                SELECT
                    ISNULL(tbPhase.phase_id, tbTargetContractType.phase_id) AS phase_id,
                    ISNULL(tbPhase.display_name, tbTargetContractType.display_name) AS display_name,
                    ISNULL(tbPhase.display_order, tbTargetContractType.display_order) AS display_order
                FROM (
                    SELECT 
                        phase_id,
                        display_name,
                        display_order
                    FROM
                        m_phase
                    WHERE
                        company_code = @company_code
                        AND del_flg = '0'
                    ) AS tbPhase
                    FULL JOIN (
                        SELECT
                            r_contract_type_phase.phase_id,
                            m_phase.display_name,
                            m_phase.display_order
                        FROM r_contract_type_phase INNER JOIN m_phase
                            ON r_contract_type_phase.company_code = m_phase.company_code
                            AND r_contract_type_phase.phase_id = m_phase.phase_id
                        WHERE r_contract_type_phase.company_code = @company_code
                            AND contract_type_id = @contract_type_id
                    ) AS tbTargetContractType
                    ON tbPhase.phase_id = tbTargetContractType.phase_id
                ORDER BY 
                    display_order"
                , new { company_code = companyCode, contract_type_id = contractTypeID });

            return this._database.Fetch<Phase>(sql);
        }

        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of category</returns>
        public IList<Category> GetCategoryList(string companyCode, int contractTypeID)
        {
            var sql = new Sql(@"
                SELECT
                    ISNULL(tbCategory.category_id, tbTargetContractType.category_id) AS category_id,
                    ISNULL(tbCategory.category, tbTargetContractType.category) AS category,
                    ISNULL(tbCategory.display_order, tbTargetContractType.display_order) AS display_order
                FROM (
                    SELECT 
                        category_id,
                        category,
                        display_order
                    FROM
                        m_category
                    WHERE
                        company_code = @company_code
                        AND del_flg = '0'
                    ) AS tbCategory
                    FULL JOIN (
                        SELECT
                            r_contract_type_category.category_id,
                            m_category.category,
                            m_category.display_order
                        FROM r_contract_type_category INNER JOIN m_category
                            ON r_contract_type_category.company_code = m_category.company_code
                            AND r_contract_type_category.category_id = m_category.category_id
                        WHERE r_contract_type_category.company_code = @company_code
                            AND contract_type_id = @contract_type_id
                    ) AS tbTargetContractType
                    ON tbCategory.category_id = tbTargetContractType.category_id
                ORDER BY 
                    display_order"
                , new { company_code = companyCode, contract_type_id = contractTypeID });

            return this._database.Fetch<Category>(sql);
        }

        /// <summary>
        /// Edit contractType info
        /// </summary>
        /// <param name="data">ContractType info</param>
        /// <param name="contractTypePhaseList">Contract type phase list</param>
        /// <param name="contractTypeCategoryList">Contract type category list</param>
        /// <param name="contractTypeID">ContractType ID output</param>
        /// <returns>Action result</returns>
        public bool EditContractTypeInfo(ContractTypePlus data, IList<ContractTypePhasePlus> contractTypePhaseList, IList<ContractTypeCategoryPlus> contractTypeCategoryList, out int contractTypeID)
        {
            int result = 0;
            contractTypeID = data.contract_type_id;

            if (data.contract_type_id == 0)
            {
                var sqlInsert = new Sql(@"
                    INSERT INTO
                        m_contract_type
                        (company_code,
                        contract_type,
                        charge_of_sales_flg,
                        exceptional_calculate_flg,
                        budget_setting_flg,
                        check_plan_flg,
                        check_progress_flg,
                        check_period_flg,
                        check_sales_flg,
                        remarks,
                        display_order,
                        ins_date,
                        ins_id,
                        upd_date,
                        upd_id,
                        del_flg)
                    VALUES
                        (@company_code, @contract_type, @charge_of_sales_flg, @exceptional_calculate_flg, @budget_setting_flg,
                         @check_plan_flg, @check_progress_flg, @check_period_flg, @check_sales_flg, @remarks,
                         @display_order, @ins_date, @ins_id, @upd_date, @upd_id, @del_flg);
                    SELECT
                        SCOPE_IDENTITY();",
                    new
                    {
                        company_code = data.company_code,
                        contract_type = data.contract_type.Trim(),
                        charge_of_sales_flg = data.charge_of_sales_flg,
                        exceptional_calculate_flg = data.exceptional_calculate_flg,
                        budget_setting_flg = data.budget_setting_flg,
                        check_plan_flg = data.check_plan_flg,
                        check_progress_flg = data.check_progress_flg,
                        check_period_flg = data.check_period_flg,
                        check_sales_flg = data.check_sales_flg,
                        remarks = data.remarks,
                        display_order = data.display_order,
                        ins_date = data.upd_date,
                        ins_id = data.upd_id,
                        upd_date = data.upd_date,
                        upd_id = data.upd_id,
                        del_flg = Constant.DeleteFlag.NON_DELETE
                    });

                contractTypeID = this._database.ExecuteScalar<int>(sqlInsert);

                if (contractTypeID > 0)
                    result = EditContractTypePhase(data.company_code, contractTypeID, data.upd_date.Value, data.upd_id, contractTypePhaseList);

                if (result > 0)
                    result = EditContractTypeCategory(data.company_code, contractTypeID, data.upd_date.Value, data.upd_id, contractTypeCategoryList);
            }
            else
            {
                var sqlUpdate = new Sql(@"
                    UPDATE
                        m_contract_type
                    SET
                        contract_type = @contract_type,
                        charge_of_sales_flg = @charge_of_sales_flg,
                        exceptional_calculate_flg = @exceptional_calculate_flg,
                        budget_setting_flg = @budget_setting_flg,
                        check_plan_flg = @check_plan_flg,
                        check_progress_flg = @check_progress_flg,
                        check_period_flg = @check_period_flg,
                        check_sales_flg = @check_sales_flg,
                        remarks = @remarks,
                        display_order = @display_order,
                        upd_date = @upd_date,
                        upd_id = @upd_id,
                        del_flg = @del_flg
                    WHERE
                        contract_type_id = @contract_type_id
                        AND company_code = @company_code;",
                     new
                     {
                         contract_type = data.contract_type.Trim(),
                         charge_of_sales_flg = data.charge_of_sales_flg,
                         exceptional_calculate_flg = data.exceptional_calculate_flg,
                         budget_setting_flg = data.budget_setting_flg,
                         check_plan_flg = data.check_plan_flg,
                         check_progress_flg = data.check_progress_flg,
                         check_period_flg = data.check_period_flg,
                         check_sales_flg = data.check_sales_flg,
                         remarks = data.remarks,
                         display_order = data.display_order,
                         upd_date = data.upd_date,
                         upd_id = data.upd_id,
                         del_flg = data.del_flg,
                         contract_type_id = data.contract_type_id,
                         company_code = data.company_code
                     });
                result = this._database.Execute(sqlUpdate);

                if (result > 0)
                    result = EditContractTypePhase(data.company_code, data.contract_type_id, data.upd_date.Value, data.upd_id, contractTypePhaseList);

                if (result > 0)
                    result = EditContractTypeCategory(data.company_code, data.contract_type_id, data.upd_date.Value, data.upd_id, contractTypeCategoryList);
            }

            return (result >= 0);
        }

        /// <summary>
        /// Get target phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="phaseID">Phase ID</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of target phase</returns>
        public IList<TargetPhase> GetTargetPhaseList(string companyCode, int phaseID, int contractTypeID)
        {
            var sql = new Sql(@"
                SELECT
                    phase_id
                FROM
                    target_phase
                WHERE
                    company_code = @company_code
                    AND phase_id = @phase_id
                    AND (SELECT contract_type_id
                        FROM project_info
                        WHERE company_code = @company_code
                        AND project_sys_id = target_phase.project_sys_id) = @contract_type_id"
                , new
                {
                    company_code = companyCode,
                    phase_id = phaseID,
                    contract_type_id = contractTypeID
                });

            return this._database.Fetch<TargetPhase>(sql);
        }

        /// <summary>
        /// Get target category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="categoryID">Category ID</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <returns>List of target category>
        public IList<TargetCategory> GetTargetCategoryList(string companyCode, int categoryID, int contractTypeID)
        {
            var sql = new Sql(@"
                SELECT
                    category_id
                FROM
                    target_category
                WHERE
                    company_code = @company_code
                    AND category_id = @category_id
                    AND (SELECT contract_type_id
                        FROM project_info
                        WHERE company_code = @company_code
                        AND project_sys_id = target_category.project_sys_id) = @contract_type_id"
                , new
                {
                    company_code = companyCode,
                    category_id = categoryID,
                    contract_type_id = contractTypeID
                });

            return this._database.Fetch<TargetCategory>(sql);
        }

        #endregion

        #region Private

        /// <summary>
        /// Build sql get contractType list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="condition">Search condition</param>
        /// <returns>Sql get contractType list</returns>
        private Sql BuildSqlContractTypeList(string companyCode, Condition condition)
        {
            var sql = new Sql(@"
              SELECT 
                tbContractType.contract_type_id,
                tbContractType.contract_type,
                tbContractType.charge_of_sales_flg,
                tbContractType.exceptional_calculate_flg,
                budget_setting_flg = ISNULL(tbContractType.budget_setting_flg,0),
                tbContractType.display_order,
                tbContractType.remarks,
                tbContractType.upd_date,
                (SELECT display_name FROM m_user WHERE company_code = tbContractType.company_code AND user_sys_id = tbContractType.upd_id) AS upd_user,
                tbContractType.del_flg
            FROM
                m_contract_type AS tbContractType
            WHERE
                tbContractType.company_code = @company_code ", new { company_code = companyCode });

            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.ContractTypeName))
                    sql.Append(string.Format("AND tbContractType.contract_type LIKE '{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.ContractTypeName) + "%"));

                if (!condition.DeleteFlag)
                    sql.Append("AND tbContractType.del_flg = '0'");
            }

            return sql;
        }

        /// <summary>
        /// Edit contract type phase
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="contractTypePhaseList">Contract type phase list</param>
        private int EditContractTypePhase(string companyCode,
            int contractTypeID,
            DateTime insDate,
            int insUser,
            IList<ContractTypePhasePlus> contractTypePhaseList)
        {
            int result = 0;

            var sqlDelete = new Sql(
                @"
                DELETE FROM
                    r_contract_type_phase
                WHERE
                    company_code = @company_code
                    AND contract_type_id = @contract_type_id;",
                new
                {
                    company_code = companyCode,
                    contract_type_id = contractTypeID
                });

            this._database.Execute(sqlDelete);

            foreach (var item in contractTypePhaseList)
            {
                if (item.phase_id.HasValue)
                {
                    var sqlInsert = new Sql(@"
                        INSERT INTO 
                            r_contract_type_phase
                            (company_code,
                            contract_type_id,
                            phase_id,
                            display_order,
                            ins_date,
                            ins_id)
                        VALUES
                            (@company_code, @contract_type_id, @phase_id, @display_order, @ins_date, @ins_id);",
                        new
                        {
                            company_code = companyCode,
                            contract_type_id = contractTypeID,
                            phase_id = item.phase_id,
                            display_order = item.display_order,
                            ins_date = insDate,
                            ins_id = insUser
                        });

                    result = this._database.Execute(sqlInsert);
                }
            }

            return result;
        }

        /// <summary>
        /// Edit contract type category
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="contractTypeID">Contract type ID</param>
        /// <param name="insDate">Insert date</param>
        /// <param name="insUser">Insert User ID</param>
        /// <param name="contractTypeCategoryList">Contract type category list</param>
        private int EditContractTypeCategory(string companyCode,
            int contractTypeID,
            DateTime insDate,
            int insUser,
            IList<ContractTypeCategoryPlus> contractTypeCategoryList)
        {
            int result = 0;

            var sqlDelete = new Sql(
                @"
                DELETE FROM
                    r_contract_type_category
                WHERE
                    company_code = @company_code
                    AND contract_type_id = @contract_type_id;",
                new
                {
                    company_code = companyCode,
                    contract_type_id = contractTypeID
                });

            result = this._database.Execute(sqlDelete);

            foreach (var item in contractTypeCategoryList)
            {
                if (item.category_id != null)
                {
                    var sqlInsert = new Sql(@"
                        INSERT INTO 
                            r_contract_type_category
                            (company_code,
                            contract_type_id,
                            category_id,
                            display_order,
                            ins_date,
                            ins_id)
                        VALUES
                            (@company_code, @contract_type_id, @category_id, @display_order, @ins_date, @ins_id);",
                        new
                        {
                            company_code = companyCode,
                            contract_type_id = contractTypeID,
                            category_id = item.category_id,
                            display_order = item.display_order,
                            ins_date = insDate,
                            ins_id = insUser
                        });

                    result = this._database.Execute(sqlInsert);
                }
            }

            return result;
        }

        #endregion
    }
}
