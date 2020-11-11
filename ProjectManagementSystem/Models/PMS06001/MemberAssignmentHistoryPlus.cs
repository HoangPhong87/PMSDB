#region License
/// <copyright file="MemberAssignmentHistoryPlus.cs" company="i-Enter Asia">
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
    /// member_assignment_history plus model class
    /// </summary>
    public class MemberAssignmentHistoryPlus : MemberAssignmentHistory
    {
        public string display_name { get; set; }
    }
}