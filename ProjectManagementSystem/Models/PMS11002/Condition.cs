#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author></author>
 /// <createdDate>2017/07/04</createdDate>
#endregion

using System;
using ProjectManagementSystem.Common;
using System.Collections.Generic;
using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS11002
{
    [Serializable]
    public class Condition
    {
        public Condition()
        {
        }
        public string CompanyCode { get; set; }

        public string GroupId { get; set; }

        public string ContractTypeId { get; set; }

        public string BranchId { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public IEnumerable<ContractType> List_Contract { get; set; }
        public IEnumerable<Group> List_Group { get; set; }
        public List<string> List_Month { get; set; }
    }
    public class TimeListBudget
    {
        public int target_year { set; get; }
        public string target_month { set; get; }
    }
}
