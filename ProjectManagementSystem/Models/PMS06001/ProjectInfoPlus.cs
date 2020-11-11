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
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// project_info plus model class
    /// </summary>
    [Serializable]
    public class ProjectInfoPlus : ProjectInfo
    {
        public string tag_name { get; set; }

        public string contract_type { get; set; }

        public string status { get; set; }

        public string rank { get; set; }

        public string charge_person { get; set; }

        public decimal? base_unit_cost { get; set; }

        [DisplayName("進捗")]
        public decimal? progress { get; set; }

        public DateTime? progress_regist_date { get; set; }

        public int? customer_id { get; set; }

        [DisplayName("発注元")]
        public string customer_name { get; set; }

        [DisplayName("所属")]
        public int? group_id { get; set; }

        public string group_name { get; set; }

        public decimal actual_man_day { get; set; }

        public decimal total_plan_man_days { get; set; }

        public decimal plan_profit { get; set; }

        public decimal actual_profit { get; set; }

        public decimal total_cost { get; set; }

        [DisplayName("更新者")]
        public string upd_user { get; set; }

        [DisplayName("登録者")]
        public string ins_user { get; set; }

        public string pic_order { get; set; }

        public string contract_type_order { get; set; }

        public string group_sales_pic { get; set; }

        public int? group_sales_pic_id { get; set; }

        public string charge_of_sales { get; set; }

        public string sales_type { get; set; }

        public decimal? gross_profit { get; set; }

        public string classComEstimate { get; set; }

        public string classComTotalSales { get; set; }

        public string classComPlanProfit { get; set; }

        public decimal total_plan_cost { get; set; }

        public decimal actual_cost { get; set; }

        public decimal? history_estimate_man_days { get; set; }

        public decimal? history_total_sales { get; set; }

        public decimal? history_gross_profit { get; set; }

        public int data_editable_time { get; set; }

        public int count_plan { get; set; }

        public string temp_start_date { get; set; }

        public string temp_end_date { get; set; }
    }
}