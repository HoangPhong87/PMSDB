#region License
/// <copyright file="PMS09001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS09001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS09001;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class PMS09001ListViewModel
    {
        public Condition Condition { get; set; }

        public IList<SelectListItem> GROUP_LIST { get; set; }

        public IList<SelectListItem> CONTRACT_TYPE_LIST { get; set; }

        public IList<SelectListItem> BRANCH_LIST { get; set; }

        public PMS09001ListViewModel()
        {
            Condition = new Condition();

            var now = Utility.GetCurrentDateTime();

            Condition.FROM_DATE = now.ToString("yyyy/MM");
            Condition.TO_DATE = now.AddMonths(11).ToString("yyyy/MM");
        }
    }
}
