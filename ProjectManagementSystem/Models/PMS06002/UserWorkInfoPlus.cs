#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="UserWorkInfoPlus.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>27-05-2014</createdDate>
// //<summary>
// // TODO: Update summary.
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion

namespace ProjectManagementSystem.Models.PMS06002
{
    using System;

    /// <summary>
    /// Entity class to represent the common user actual work info in a month
    /// </summary>
    public class UserWorkInfoPlus
    {
        public string group_name { get; set; }

        public string user_name { get; set; }

        public decimal? total_work { get; set; }

        public string employee_no { get; set; }
        /// <summary>
        /// Return the time value in a formatted form
        /// </summary>
        public string total_work_str {
            get
            {
                if (total_work.HasValue)
                {
                    int hour = (int)Math.Floor(total_work.Value);
                    int min = (int)Math.Round((total_work.Value - hour)*60);
                    return string.Format("{0:00}時間{1:00}分", hour, min);
                }
                else
                {
                    return "00:00h";
                }
            }
        }

        public UserWorkInfoPlus()
        {
            group_name = string.Empty;
            user_name = string.Empty;
            total_work = 0;
            employee_no = string.Empty;
        }
    }
}