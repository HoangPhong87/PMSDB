#region License
/// <copyright file="PMS11001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS11002
{
    using ProjectManagementSystem.Models.PMS11002;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class PMS11002ListViewModel
    {
        /// <summary>
        /// Search condition of the displayed list
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// Text label to display the current range of months
        /// </summary>
        public string CurrentMonthRange { get; set; }

        /// <summary>
        /// List of groups (Affiliation)
        /// </summary>
        public IList<SelectListItem> GroupList { get; set; }

        /// <summary>
        /// List of ContracType
        /// </summary>
        public IList<SelectListItem> ContractTypeList { get; set; }

        /// <summary>
        /// List of Branch
        /// </summary>
        public IList<SelectListItem> BranchList { get; set; }

        /// <summary>
        /// Month List
        /// </summary>
        public IList<SelectListItem> MonthList { get; set; }

        /// <summary>
        /// BudgetList
        /// </summary>
        public IEnumerable<BudgetPlus> BudgetList { get; set; }

        /// <summary>
        /// Total Year List
        /// </summary>
        public IList<TotalYearList> TotalYearList { get; set; }
        public PMS11002ListViewModel()
        {
            Condition = new Condition();
        }
    }
}
