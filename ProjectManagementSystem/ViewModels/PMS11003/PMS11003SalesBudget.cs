#region License
/// <copyright file="PMS11001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS11003
{
    using ProjectManagementSystem.Models.PMS11003;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [Serializable]
    public class PMS11003ListViewModel
    {
        /// <summary>
        /// Time start of period
        /// </summary>
        public string TimeStart { get; set; }

        /// <summary>
        /// Time end of period
        /// </summary>
        public string TimeEnd { get; set; }

        /// <summary>
        /// Search condition of the displayed list
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// List of groups (Affiliation)
        /// </summary>
        public IList<Models.Entities.Group> GroupList { get; set; }

        /// <summary>
        /// List of groups (Affiliation)
        /// </summary>
        public IList<SelectListItem> GroupListToSearch { get; set; }

        /// <summary>
        /// List of ContracType
        /// </summary>
        public IList<Models.Entities.ContractType> ContractTypeList { get; set; }

        /// <summary>
        /// List of ContracType
        /// </summary>
        public IList<SelectListItem> ContractTypeListToSearch { get; set; }

        /// <summary>
        /// List of DataSalesBudget
        /// </summary>
        public IList<SalesData> DataSalesBudget { get; set; }

        /// <summary>
        /// List of Total Group budget
        /// </summary>
        public IList<TotalGroup> TotalGroup { get; set; }

        /// <summary>
        /// List of Total Contract type budget
        /// </summary>
        public IList<TotalContractType> TotalCT { get; set; }

        /// <summary>
        /// List of Total group year
        /// </summary>
        public IList<TotalGrAll> TotalGrYearList { get; set; }

        /// <summary>
        /// List of DataSalesBudget
        /// </summary>
        public IList<TimeList> TimeList { get; set; }

        public string CheckSalesType { get; set; }

        public PMS11003ListViewModel()
        {
            Condition = new Condition();
        }
    }
}
