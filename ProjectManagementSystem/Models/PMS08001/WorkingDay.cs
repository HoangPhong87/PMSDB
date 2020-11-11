#region License
/// <copyright file="Condition.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>LinhDT</author>
/// <createdDate>2018/12/21</createdDate>
#endregion

using System.Collections.Generic;

namespace ProjectManagementSystem.Models.PMS08001
{
    public class WorkingDay
    {
        public string company_code { get; set; }
        public int target_year { get; set; }
        public int target_month { get; set; }
        public int target_date { get; set; }
    }
}