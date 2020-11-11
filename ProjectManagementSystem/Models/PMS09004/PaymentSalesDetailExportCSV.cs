#region License
/// <copyright file="SalesPaymentDetail.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/07/15</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS09004
{
    [Serializable]
    public class PaymentSalesDetailExportCSV
    {
        public string target_time { get; set; }
        public string sales_company { get; set; }
        public string tag_name { get; set; }
        public string end_user_name { get; set; }
        public string project_no { get; set; }
        public string project_name { get; set; }
        public string end_date { get; set; }
        public string acceptance_date { get; set; }
        public string contract_type { get; set; }
        public string total_sales { get; set; }
        public string charge_person { get; set; }
        public string group_name { get; set; }
        public string user_name { get; set; }
        public string sales_amount { get; set; }
        public string unit_cost { get; set; }
        public string plan_man_times { get; set; }
        public string plan_cost { get; set; }
        public string actual_work_time { get; set; }
        public string actual_cost { get; set; }
        public string payment_company { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
        public string sub_category { get; set; }
    }
}
