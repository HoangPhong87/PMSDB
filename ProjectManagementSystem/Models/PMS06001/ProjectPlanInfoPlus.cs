#region License
/// <copyright file="ProjectInfoPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/12</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// project_info plus model class
    /// </summary>
    public class ProjectPlanInfoPlus : ProjectPlanInfo
    {
        public IList<string> targetList { get; set; }
        public IList<MeasureAndConcern> riskList { get; set; }
        public string customer_name { get; set; }
        public string project_name { get; set; }
        public string person_in_charge { get; set; }
        public string sales_person_in_charge { get; set; }
        public string user_update { get; set; }
        public string user_regist { get; set; }
        public string read_only { get; set; }
    }

    public class MeasureAndConcern
    {
        public string Measure { get; set; }
        public string Concern { get; set; }
    }
}