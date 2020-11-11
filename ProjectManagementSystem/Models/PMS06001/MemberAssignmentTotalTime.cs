#region License
/// <copyright file="MemberAssignmentTotalTime.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// member_assignment plus model class
    /// </summary>
    public class MemberAssignmentTotalTime
    {
        public int user_sys_id { get; set; }

        public decimal total_plan_man_days { get; set; }

        public decimal total_actual_work_time { get; set; }
    }
}