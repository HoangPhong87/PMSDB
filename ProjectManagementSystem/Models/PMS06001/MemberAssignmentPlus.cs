#region License
/// <copyright file="MemberAssignmentPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/29</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// member_assignment plus model class
    /// </summary>
    public class MemberAssignmentPlus : MemberAssignment
    {
        public string display_name { get; set; }
    }
}