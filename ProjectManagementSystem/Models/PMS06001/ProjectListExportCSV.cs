#region License
/// <copyright file="ProjectListExportCSV.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/08/15</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS06001
{
    /// <summary>
    /// project list export to csv file
    /// </summary>
    public class ProjectListExportCSV
    {
        public string project_name { get; set; }

        public string project_no { get; set; }

        public string tag_name { get; set; }

        public string contract_type { get; set; }

        public string rank { get; set; }

        public string start_date { get; set; }

        public string end_date { get; set; }

        public string acceptance_date { get; set; }

        public string charge_person { get; set; }

        public string charge_of_sales_person { get; set; }

        public string status { get; set; }

        public string total_sales { get; set; }

        public string total_payment { get; set; }

        public string estimate_man_days { get; set; }

        public string actual_man_day { get; set; }

        public string progress { get; set; }

        public string plan_profit { get; set; }

        public string actual_profit { get; set; }

        public string customer_name { get; set; }

        public string remarks { get; set; }

        public string upd_date { get; set; }

        public string upd_user { get; set; }
    }
}