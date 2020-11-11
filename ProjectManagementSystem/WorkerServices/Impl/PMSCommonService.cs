#region License
/// <copyright file="PMSCommonService.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/11/05</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.Repositories;
using ProjectManagementSystem.Models.Repositories.Impl;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjectManagementSystem.WorkerServices.Impl
{
    /// <summary>
    /// Common repository class
    /// </summary>
    public class PMSCommonService : IPMSCommonService
    {
        /// <summary>
        /// </summary>
        private IPMSCommonRepository _repository;

        /// <summary>
        /// </summary>
        public PMSCommonService()
            : this(new PMSCommonRepository())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="_repository"></param>
        public PMSCommonService(IPMSCommonRepository _repository)
        {
            this._repository = _repository;
        }

        /// <summary>
        /// Get all status
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of status</returns>
        public IEnumerable<Status> GetStatusList(string companyCode)
        {
            return this._repository.GetStatusList(companyCode);
        }

        /// <summary>
        /// Get all project rank
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>List of project rank</returns>
        public IEnumerable<Rank> GetRankList(string companyCode)
        {
            return this._repository.GetRankList(companyCode);
        }

        /// <summary>
        /// Get tag list
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="customerId">Customer Id</param>
        /// <returns></returns>
        public IList<CustomerTag> GetTagList(string companyCode, int customerId)
        {
            return this._repository.GetTagList(companyCode, customerId);
        }

        /// <summary>
        /// Get select list of status by company code
        /// </summary>
        /// <param name="cCode">Company code</param>
        /// <returns>Select list of status</returns>
        public IList<SelectListItem> GetStatusSelectList(string companyCode)
        {
            return
                this._repository.GetStatusList(companyCode)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.status_id.ToString(),
                            Text = f.status
                        })
                    .ToList();
        }

        /// <summary>
        /// Get select list of contract type by company code
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="isFilter">Filter by budget_setting_flg or not</param>
        /// <returns>Select list of contract type</returns>
        public IList<SelectListItem> GetContractTypeSelectList(string companyCode, bool isFilter = false)
        {
            return
                this._repository.GetContractTypeList(companyCode, isFilter)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.contract_type_id.ToString(),
                            Text = f.contract_type
                        })
                    .ToList();
        }

        /// <summary>
        /// Get select list of project rank
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <returns>Select list of project rank</returns>
        public IList<SelectListItem> GetRankSelectList(string companyCode)
        {
            return
                this._repository.GetRankList(companyCode)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.rank_id.ToString(),
                            Text = f.rank
                        })
                    .ToList();
        }

        /// <summary>
        /// Get select list of user group by company code
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="isFilter">Filter by budget_setting_flg or not</param>
        /// <returns>Select list of user group</returns>
        public IList<SelectListItem> GetUserGroupSelectList(string companyCode, bool isFilter = false)
        {
            return
                this._repository.GetUserGroupList(companyCode,isFilter)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.group_id.ToString(),
                            Text = f.display_name
                        })
                    .ToList();
        }

        /// <summary>
        /// Get Branch Select List
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public IList<SelectListItem> GetBranchSelectList(string cCode)
        {
            return
               this._repository.GetBranchList(cCode)
                   .Select(
                       f =>
                       new SelectListItem
                       {
                           Value = f.location_id.ToString(),
                           Text = f.display_name
                       })
                   .ToList();
        }

        /// <summary>
        /// Get select list of tag link by customer
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="customerID">Customer ID</param>
        /// <returns>Select list of tag link</returns>
        public IList<SelectListItem> GetTagSelectList(string companyCode, int customerID)
        {
            return
                this._repository.GetTagList(companyCode, customerID)
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.tag_id.ToString(),
                            Text = f.tag_name
                        })
                    .ToList();
        }

        /// <summary>
        /// Get select list of prefecture
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetPrefectureList()
        {
            return
                this._repository.GetPrefectureList()
                    .Select(
                        f =>
                        new SelectListItem
                        {
                            Value = f.prefecture_code.ToString(),
                            Text = f.prefecture_name
                        })
                    .ToList();
        }

        /// <summary>
        /// Check limit data by license
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public bool CheckValidUpdateData(string companyCode, string dataType)
        {
            return (this._repository.CheckValidUpdateData(companyCode, dataType, null) == 1);
        }

        /// <summary>
        /// Check limit data by license
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="dataType"></param>
        /// <param name="dataImportQuantity"></param>
        /// <returns></returns>
        public bool CheckValidUpdateData(string companyCode, string dataType, int? dataImportQuantity)
        {
            return (this._repository.CheckValidUpdateData(companyCode, dataType, dataImportQuantity) == 1);
        }

        /// <summary>
        /// get project sys id by project no
        /// </summary>
        /// <param name="projectNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public string getProjectIdByProjectNo(string projectNo, string companyCode)
        {
            return this._repository.getProjectIdByProjectNo(projectNo, companyCode);
        }
    }
}
