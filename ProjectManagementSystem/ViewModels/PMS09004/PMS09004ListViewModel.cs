#region License
/// <copyright file="PMS09004ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/07/15</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS09004
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS09004;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class PMS09004ListViewModel
    {
        public Condition Condition { get; set; }

        public IList<SelectListItem> GROUP_LIST { get; set; }

        public IList<SelectListItem> CONTRACT_TYPE_LIST { get; set; }

        public IList<SelectListItem> BRANCH_LIST { get; set; }

        public PMS09004ListViewModel()
        {
            Condition = new Condition();

            Condition.TARGET_TIME = Utility.GetCurrentDateTime().ToString("yyyy/MM");
            Condition.TARGET_TIME_START = Utility.GetCurrentDateTime().ToString("yyyy/MM");
            Condition.TARGET_TIME_END = Utility.GetCurrentDateTime().ToString("yyyy/MM");
        }
    }
}
