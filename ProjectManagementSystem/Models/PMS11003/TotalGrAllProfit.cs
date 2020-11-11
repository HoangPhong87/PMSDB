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
    public class TotalGrAllProfit : Budget
    {
        public decimal tgrProfitBudget { get; set; }

        public decimal tgrCost { get; set; }

        public decimal tgrSaleActual { get; set; }

        public decimal tgrProfitActual { get; set; }

        public string tgrProfitRate { get; set; }

        public string tgrSuccessRate { get; set; }
    }
}