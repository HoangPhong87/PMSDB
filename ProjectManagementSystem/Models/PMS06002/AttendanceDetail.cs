using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class AttendanceDetail
    {
        public AttendanceDetail()
        {
            EmptyData = false;
        }
        public bool EmptyData { get; set; }
        public DateTime working_date { get; set; }
        public string holiday_name { get; set; }
        public decimal work_start_time { get; set; }
        public int work_start_hour
        {
            get
            {
                return Convert.ToInt32(Math.Floor(work_start_time));
            }
        }
        public int work_start_minute
        {
            get
            {
                return Convert.ToInt32(Math.Round(60 * (work_start_time - Math.Floor(work_start_time))));
            }
        }
        public decimal work_end_time { get; set; }
        public int work_end_hour
        {
            get
            {
                return Convert.ToInt32(Math.Floor(work_end_time));
            }
        }
        public int work_end_minute
        {
            get
            {
                return Convert.ToInt32(Math.Round(60 * (work_end_time - Math.Floor(work_end_time))));
            }
        }

        public decimal rest_time { get; set; }
        public int rest_hour
        {
            get
            {
                return Convert.ToInt32(Math.Floor(rest_time));
            }
        }
        public int rest_minute
        {
            get
            {
                return Convert.ToInt32(Math.Round(60 * (rest_time - Math.Floor(rest_time))));
            }
        }
        public decimal allowed_cost_time { get; set; }
        public int allowed_cost_time_hour
        {
            get
            {
                return Convert.ToInt32(Math.Floor(allowed_cost_time));
            }
        }
        public int allowed_cost_time_minute
        {
            get
            {
                return Convert.ToInt32(Math.Round(60 * (allowed_cost_time - Math.Floor(allowed_cost_time))));
            }
        }
        public string remarks { get; set; }

        public string attendance_type_name { get; set; }

        /// <summary>
        /// 出社打刻時間
        /// </summary>
        public decimal clock_in_start_time { get; set; }
        /// <summary>
        /// 出社打刻時間(時)
        /// </summary>
        public int clock_in_start_hour
        {
            get
            {
                return GetHourFromDecimalTime(clock_in_start_time);
            }
        }
        /// <summary>
        /// 出社打刻時間(分)
        /// </summary>
        public int clock_in_start_minute
        {
            get
            {
                return GetMinuteFromDecimalTime(clock_in_start_time);
            }
        }

        /// <summary>
        /// 退社打刻時間
        /// </summary>
        public decimal clock_in_end_time { get; set; }
        /// <summary>
        /// 退社打刻時間(時)
        /// </summary>
        public int clock_in_end_hour
        {
            get
            {
                return GetHourFromDecimalTime(clock_in_end_time);
            }
        }
        /// <summary>
        /// 退社打刻時間(分)
        /// </summary>
        public int clock_in_end_minute
        {
            get
            {
                return GetMinuteFromDecimalTime(clock_in_end_time);
            }
        }

        #region Method

        /// <summary>
        /// decimal型の時刻から「時」を取得します
        /// </summary>
        /// <param name="baseTime">取得元decimal時刻</param>
        /// <returns>時</returns>
        private int GetHourFromDecimalTime(decimal baseTime)
        {
            return Convert.ToInt32(Math.Floor(baseTime));
        }

        /// <summary>
        /// decimal型の時刻から「分」を取得します
        /// </summary>
        /// <param name="baseTime">取得元decimal時刻</param>
        /// <returns>分</returns>
        private int GetMinuteFromDecimalTime(decimal baseTime)
        {
            return Convert.ToInt32(Math.Round(60 * (baseTime - GetHourFromDecimalTime(baseTime))));
        }

        #endregion
    }
}