#region License
/// <copyright file="TargetPhasePlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/28</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// target_phase plus model class
    /// </summary>
    public class TargetPhasePlus : TargetPhase
    {
        public string phase_name { get; set; }

        public decimal total_actual_work { get; set; }

        public string estimate_target_flg { get; set; }
    }
}