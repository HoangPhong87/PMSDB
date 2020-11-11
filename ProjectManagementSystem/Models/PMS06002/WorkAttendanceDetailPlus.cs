using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class WorkAttendanceDetailPlus
    {
        public int? actual_work_date { get; set; }
        public decimal? work_start_time { get; set; }
        public decimal? work_end_time { get; set; }
        public decimal? rest_time { get; set; }
        public double? allowed_cost_time { get; set; }
        public int? attendance_type_id { get; set; }
        public string attendance_type_name { get; set; }
        public string regist_type { get; set; }

        public WorkAttendanceDetailPlus()
        {
            work_start_time = null;
            work_end_time = null;
            rest_time = null;
            allowed_cost_time = null;
            attendance_type_id = 0;
        }

        /// <summary>
        /// Return the hour value in two digits(work_start_time)
        /// </summary>
        public string work_start_time_hour
        {
            get
            {
                if (work_start_time.HasValue)
                {
                    var hour = Math.Floor(work_start_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string work_start_time_minute
        {
            get
            {
                if (work_start_time.HasValue)
                {
                    var min = Math.Round(60 * (work_start_time.Value - Math.Floor(work_start_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                    return "";
            }
        }


        /// <summary>
        /// Return the hour value in two digits
        /// </summary>
        public string work_end_time_hour
        {
            get
            {
                if (work_end_time.HasValue)
                {
                    var hour = Math.Floor(work_end_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string work_end_time_minute
        {
            get
            {
                if (work_end_time.HasValue)
                {
                    var min = Math.Round(60 * (work_end_time.Value - Math.Floor(work_end_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                    return "";
            }
        }


        /// <summary>
        /// Return the hour value in two digits
        /// </summary>
        public string rest_time_hour
        {
            get
            {
                if (rest_time.HasValue)
                {
                    var hour = Math.Floor(rest_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string rest_time_minute
        {
            get
            {
                if (rest_time.HasValue)
                {
                    var min = Math.Round(60 * (rest_time.Value - Math.Floor(rest_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Return the hour value in two digits
        /// </summary>
        public string allowed_cost_time_hour
        {
            get
            {
                if (allowed_cost_time.HasValue)
                {
                    var hour = Math.Floor(allowed_cost_time.Value);
                    return string.Format("{0:00}", hour);
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Return the minute value in 2 digits
        /// </summary>
        public string allowed_cost_time_minute
        {
            get
            {
                if (allowed_cost_time.HasValue)
                {
                    var min = Math.Round(60 * (allowed_cost_time.Value - Math.Floor(allowed_cost_time.Value)));
                    return string.Format("{0:00}", min);
                }
                else
                {
                    return "";
                }
            }
        }
    }
}