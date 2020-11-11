#region License
/// <copyright file="MemberAssignmentDetailHistoryPlus.cs" company="i-Enter Asia">
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
    /// member_assignment_detail_history plus model class
    /// </summary>
    public class MemberAssignmentDetailHistoryPlus : MemberAssignmentDetailHistory
    {
        public decimal? unit_cost { get; set; }
        public decimal? plan_cost { get; set; }
        public string target_time { get; set; }
    }
}