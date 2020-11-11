#region License
/// <copyright file="OverheadCostDetailPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/12</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// overhead_cost_detail plus model class
    /// </summary>
    public class OverheadCostDetailPlus : OverheadCostDetail
    {
        public string target_time { get; set; }
    }
}