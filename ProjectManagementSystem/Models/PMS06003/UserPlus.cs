#region License
/// <copyright file="UserPlus.cs" company="i-Enter Asia">
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
    public class UserPlus : User
    {
        /// <summary>
        /// Man days
        /// </summary>
        public IList<string> man_days { get; set; }
    }
}