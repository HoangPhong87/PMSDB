#region License
/// <copyright file="PMS09002ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/09/04</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS09002
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS09002;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class PMS09002ListViewModel
    {
        public Condition Condition { get; set; }

        public IList<SelectListItem> GROUP_LIST { get; set; }

        public IList<SelectListItem> CONTRACT_TYPE_LIST { get; set; }
        
        public IList<SelectListItem> BRANCH_LIST { get; set; }

        public PMS09002ListViewModel()
        {
            Condition = new Condition();
            GROUP_LIST = new List<SelectListItem>();
            CONTRACT_TYPE_LIST = new List<SelectListItem>();
            BRANCH_LIST = new List<SelectListItem>();

            var now = Utility.GetCurrentDateTime();

            Condition.FROM_DATE = now.ToString("yyyy/MM");
            Condition.TO_DATE = now.AddMonths(11).ToString("yyyy/MM");
        }
    }
}
