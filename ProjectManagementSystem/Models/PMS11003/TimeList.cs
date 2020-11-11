#region License
/// <copyright file="BudgetPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS11003
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TimeList
    {
        public string year { get; set; }

        public string month { get; set; }
    }
}