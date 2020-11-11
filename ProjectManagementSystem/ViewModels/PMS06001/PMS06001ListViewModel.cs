#region License
/// <copyright file="PMS06001ListViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/08</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS06001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS06001;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class PMS06001ListViewModel
    {
        public Condition Condition { get; set; }

        public IList<SelectListItem> CONTRACT_TYPE_LIST { get; set; }

        public IList<SelectListItem> STATUS_LIST { get; set; }
       
        public MultiSelectList GROUP_LIST { get; set; }

        public IList<SelectListItem> TAG_LIST { get; set; }

        public IList<SelectListItem> STATUS_SELECT_LIST { get; set; }

        public string COMPANY_CODE { get; set; }

        public PMS06001ListViewModel()
        {
            Condition = new Condition();
            Condition.COMPLETE_ID = Convert.ToInt32(Constant.CompleteStatusSearchList.OPEN);
        }
    }
}
