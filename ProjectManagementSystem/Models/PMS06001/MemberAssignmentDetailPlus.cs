#region License
/// <copyright file="MemberAssignmentDetailPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/22</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// member_assignment_detail plus model class
    /// </summary>
    public class MemberAssignmentDetailPlus : MemberAssignmentDetail
    {
        public decimal? unit_cost { get; set; }
        public decimal? plan_cost { get; set; }

        public string target_time { get; set; }
    }
}