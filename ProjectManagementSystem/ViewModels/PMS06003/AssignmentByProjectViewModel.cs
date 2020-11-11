#region License
/// <copyright file="AssignmentByProjectViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>TrungNT</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS06003;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.ViewModels.PMS06003
{
    /// <summary>
    /// ViewModel for AssignmentByProject page
    /// </summary>
    public class AssignmentByProjectViewModel
    {
        /// <summary>
        /// Search condition
        /// </summary>
        public ProjectCondition Condition { get; set; }

        /// <summary>
        /// Effort list
        /// </summary>
        public IList<SelectListItem> EFFORT_LIST { get; set; }

        /// <summary>
        /// Group list
        /// </summary>
        public IList<SelectListItem> GROUP_LIST { get; set; }

        /// <summary>
        /// Tag list
        /// </summary>
        public IList<SelectListItem> TAG_LIST { get; set; }

        /// <summary>
        /// Status list
        /// </summary>
        public IList<SelectListItem> STATUS_LIST { get; set; }



    }
}