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
    public class PMS11003ListViewModelPlus
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
        public IList<Models.Entities.Group> GroupListProfit { get; set; }

        /// <summary>
        /// List of groups (Affiliation)
        /// </summary>
        public IList<SelectListItem> GroupListToSearch { get; set; }

        /// <summary>
        /// List of ContracType
        /// </summary>
        public IList<Models.Entities.ContractType> ContractTypeListProfit { get; set; }

        /// <summary>
        /// List of ContracType
        /// </summary>
        public IList<SelectListItem> ContractTypeListToSearch { get; set; }

        /// <summary>
        /// List of DataProfitBudget
        /// </summary>
        public IList<ProfitData> DataProfitBudget { get; set; }

        /// <summary>
        /// List of Total Group profit
        /// </summary>
        public IList<TotalGroupProfit> TotalGroupProfit { get; set; }

        /// <summary>
        /// List of Total Contract type profit
        /// </summary>
        public IList<TotalContractTypeProfit> TotalCTProfit { get; set; }

        /// <summary>
        /// List of Total month profit
        /// </summary>
        public IList<TotalGrAllProfit> TotalMonthProfit { get; set; }

        /// <summary>
        /// List of Total group year profit
        /// </summary>
        public IList<TotalGrAllProfit> TotalGrYearListProfit { get; set; }

        /// <summary>
        /// List of time
        /// </summary>
        public IList<TimeList> TimeListProfit { get; set; }

        public string CheckSalesType { get; set; }
        public PMS11003ListViewModelPlus()
        {
            Condition = new Condition();
        }
    }
}
