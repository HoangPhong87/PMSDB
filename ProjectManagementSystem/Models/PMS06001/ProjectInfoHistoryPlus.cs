#region License
/// <copyright file="ProjectInfoHistoryPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/11/28</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// project_info_history plus model class
    /// </summary>
    public class ProjectInfoHistoryPlus : ProjectInfoHistory
    {
        public string ins_user { get; set; }

        public string customer_name { get; set; }

        public string end_user_name { get; set; }
    }
}