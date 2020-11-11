#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="WorkDetailPlus.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>23-05-2014</createdDate>
// //<summary>
// // TODO: Update summary.
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion

namespace ProjectManagementSystem.Models.PMS06002
{
    using System;

    public class WorkDetailPlus
    {
        /// <summary>
        /// Auto increment seri number, used to decide to update or insert a record
        /// </summary>
        public long? detail_no { get; set; }
        public int? actual_work_date { get; set; }

        public decimal? actual_work_time { get; set; }

        public WorkDetailPlus()
        {
            detail_no = 0L;
            actual_work_date = 0;
            actual_work_time = 0;
        }

        /// <summary>
        /// Return the hour value in two digits
        /// </summary>
        public string work_hour
        {
            get
            {
                if (actual_work_time.HasValue)
                {
                    var hour =  Math.Floor(actual_work_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string work_minute
        {
            get
            {
                if (actual_work_time.HasValue)
                {
                    var min = Math.Round(60 * (actual_work_time.Value - Math.Floor(actual_work_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                    return "";
            }
        }


    }
}