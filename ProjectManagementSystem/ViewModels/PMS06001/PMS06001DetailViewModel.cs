#region License
/// <copyright file="PMS06001DetailViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/16</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS06001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06001;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Mvc;

    public class PMS06001DetailViewModel
    {
        public ProjectInfoPlus PROJECT_INFO { get; set; }

        public string SALES { get; set; }

        public string PAYMENT { get; set; }

        public string TOTAL_ACTUAL_EFFORT { get; set; }

        public string TOTAL_PLAN_EFFORT { get; set; }

        public string PLAN_PROFIT { get; set; }

        public string ACTUAL_PROFIT { get; set; }

        [DisplayName("予定利益")]
        public string PLAN_PROFIT_RATE { get; set; }

        public string ACTUAL_PROFIT_RATE { get; set; }

        public string TIME_UNIT { get; set; }

        public IList<SelectListItem> TIME_UNIT_LIST { get; set; }

        public int selected_year { get; set; }

        public int selected_month { get; set; }

        //public IList<ProjectAttachFilePlus> FILE_LIST { get; set; }

        public IList<TargetPhasePlus> PHASE_LIST { get; set; }

        public PMS06001DetailViewModel()
        {
            PROJECT_INFO = new ProjectInfoPlus();
            TIME_UNIT_LIST = new List<SelectListItem> {
                new SelectListItem { Text = "時間", Value = "1" },
                new SelectListItem { Text = "人日", Value = "2", Selected = true },
                new SelectListItem { Text = "人月", Value = "3" }
            };
        }
    }
}
