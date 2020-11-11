#region License
/// <copyright file="PMS08001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/20</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS08001
{
    using ProjectManagementSystem.Models.Entities;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Models.PMS08001;

    public class PMS08001ListViewModel
    {
        public IList<Information> INFORMATION_LIST { get; set; }
        public bool PLAN_LIST_VISIBLE { get; set; }
        public bool PROGRESS_LIST_VISIBLE { get; set; }
        public bool PERIOD_LIST_VISIBLE { get; set; }
        public bool SALES_LIST_VISIBLE { get; set; }
        public IList<ProjectInfor> PLAN_LIST { get; set; }
        public IList<ProjectInfor> PROGRESS_LIST { get; set; }
        public IList<ProjectInfor> PERIOD_LIST { get; set; }
        public IList<ProjectInfor> SALES_LIST { get; set; }
        public IList<GroupInfor> GROUP_LIST { get; set; }

        public PMS08001ListViewModel()
        {
            PLAN_LIST_VISIBLE = false;
            PROGRESS_LIST_VISIBLE = false;
            PERIOD_LIST_VISIBLE = false;
            SALES_LIST_VISIBLE = false;
        }
    }
}
