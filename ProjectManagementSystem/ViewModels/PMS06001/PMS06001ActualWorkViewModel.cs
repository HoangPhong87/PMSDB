#region License
/// <copyright file="PMS06001ActualWorkViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/08/12</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS06001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06001;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Mvc;

    public class PMS06001ActualWorkViewModel
    {
        public string GROUP_NAME { get; set; }

        public string USER_NAME { get; set; }

        public string PROJECT_NAME { get; set; }

        public string DURATION { get; set; }

        public string FROM { get; set; }

        public string TO { get; set; }

        public int PROJECT_ID { get; set; }

        public int USER_ID { get; set; }

        public int TIME_UNIT { get; set; }

        public PMS06001ActualWorkViewModel()
        {
            TIME_UNIT = 2;
        }
    }
}
