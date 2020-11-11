#region License
/// <copyright file="ProgressHistoryPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/05/11</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// ProgressHistory plus model class
    /// </summary>
    public class ProgressHistoryPlus : ProgressHistory
    {
        public bool? isDelete { get; set; }

        public bool? isNew { get; set; }
    }
}