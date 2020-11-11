#region License
/// <copyright file="ProjectInfoPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>TrungNT</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.Models.PMS06003
{
    /// <summary>
    /// project_info plus model class
    /// </summary>
    public class ProjectInfoPlus : ProjectInfo
    {
        /// <summary>
        /// Rank of User
        /// </summary>
        public string rank { get; set; }

        /// <summary>
        /// For TEST
        /// </summary>
        public string MONTH_1 { get; set; }
        public string MONTH_2 { get; set; }
        public string MONTH_3 { get; set; }
        public string MONTH_4 { get; set; }
        public string MONTH_5 { get; set; }
        public string MONTH_6 { get; set; }

    }
}