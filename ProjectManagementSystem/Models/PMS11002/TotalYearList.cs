#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author></author>
 /// <createdDate>2017/08/01</createdDate>
#endregion

using System;
using ProjectManagementSystem.Common;
using System.Collections.Generic;
using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS11002
{
    [Serializable]
    public class TotalYearList
    {
        public TotalYearList()
        {
        }
        public string group_id { get; set; }

        public string profit_budget { get; set; }

        public string sales_budget { get; set; }

    }
}
