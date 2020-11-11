#region License
/// <copyright file="OverheadCostPlus.cs" company="i-Enter Asia">
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
    /// overhead_cost plus model class
    /// </summary>
    public class OverheadCostPlus : OverheadCost
    {
        public string type_name { get; set; }

        public string charge_person_name { get; set; }

        public bool is_change { get; set; }

        public bool is_delete { get; set; }
    }
}