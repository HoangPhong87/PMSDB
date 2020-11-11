#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2014/09/04</createdDate>
#endregion

using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.PMS09001
{
    [Serializable]
    public class SalesProjectDetailCondition
    {
        public string CompanyCode { get; set; }
        public int GroupId { get; set; }
        public int ProjectId { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
    }
}
