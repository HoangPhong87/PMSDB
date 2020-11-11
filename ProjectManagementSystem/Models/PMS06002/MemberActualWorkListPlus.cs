using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class MemberActualWorkListPlus
    {
        public int project_sys_id { get; set; }
        public string project_name { get; set; }
        public int phase_id { get; set; }
        public string display_name { get; set; }
        public decimal? actual_work_time { get; set; }
        public string status { get; set; }
        public string sales_type { get; set; }

        public MemberActualWorkListPlus()
        {
            actual_work_time = 0;
        }

        public string work_hour
        {
            get
            {   
                if (actual_work_time.HasValue)
                {
                    var hour = Math.Floor(actual_work_time.Value);
                    var min = Math.Round(60 * (actual_work_time.Value - Math.Floor(actual_work_time.Value)));
                    if (hour == 0 && min == 0)
                    {
                        return string.Format("{0:''}", hour);
                    }
                    else
                    {
                        return string.Format("{0:00}", hour);
                    }
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
                    if (min == 0 && Math.Floor(actual_work_time.Value) == 0)
                    {
                        return string.Format("{0:''}", min);
                    }
                    else
                    {
                        return string.Format("{0:00}", min);
                    }
                }
                else
                    return "";
            }
        }
    }
}