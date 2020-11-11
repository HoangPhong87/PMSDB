#region License
/// <copyright file="OverheadCostDetailHistoryPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/02/09</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// overhead_cost_detail plus model class
    /// </summary>
    public class OverheadCostDetailHistoryPlus : OverheadCostDetailHistory
    {
        public string target_time { get; set; }
    }
}