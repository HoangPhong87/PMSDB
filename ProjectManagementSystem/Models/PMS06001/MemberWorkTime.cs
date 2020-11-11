#region License
/// <copyright file="MemberWorkTime.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/25/09</createdDate>
#endregion

namespace ProjectManagementSystem.Models.PMS06001
{
    using System.Collections.Generic;
    using System.Text;

    public class MemberWorkTime
    {
        public string company_code { get; set; }

        public int user_sys_id { get; set; }

        public string display_name { get; set; }

        public string regist_type { get; set; }

        public decimal total_actual_work { get; set; }

        public decimal attendance_time { get; set; }

        public int actual_work_year { get; set; }

        public int actual_work_month { get; set; }
    }
}