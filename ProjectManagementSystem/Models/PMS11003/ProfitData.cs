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
    public class ProfitData 
    {
        public int target_year { get; set; }

        public int target_month { get; set; }

        public int group_id { get; set; }

        public int contract_type_id { get; set; }

        public string group_name { get; set; }

        public string contract_type { get; set; }

        public decimal profit_budget { get; set; }

        public decimal cost_price { get; set; }

        public decimal sales_price { get; set; }

        public decimal profit_actual { get; set; }

        public string tgrProfitRate { get; set; }

        public string tgrSuccessRate { get; set; }

        public int group_display_order { get; set; }

        public int contract_type_display_order { get; set; }
    }

    public class ProfitBudget
    {
        public string contract_type { get; set; }

        public int contract_type_id { get; set; }

        public int group_id { get; set; }

        public string group_name { get; set; }

        public decimal profit_budget { get; set; }

        public int target_month { get; set; }

        public int target_year { get; set; }
    }
    public class CostPrice
    {
        public int contract_type_id { get; set; }

        public int group_id { get; set; }

        public decimal cost_price { get; set; }

        public int target_month { get; set; }

        public int target_year { get; set; }
    }

    public class SalesPrice
    {
        public int contract_type_id { get; set; }

        public int group_id { get; set; }

        public decimal total_sales { get; set; }

        public int target_month { get; set; }

        public int target_year { get; set; }

        public int group_display_order { get; set; }

        public int contract_type_display_order { get; set; }
    }
}