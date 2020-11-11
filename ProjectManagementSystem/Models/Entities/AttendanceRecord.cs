using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("attendance_record")]
    [PetaPoco.PrimaryKey("company_code, user_sys_id, actual_work_year, actual_work_month, actual_work_date", autoIncrement = false)]
    public class AttendanceRecord
    {
        public AttendanceRecord()
        {
            work_start_time = null;
            work_end_time = null;
            rest_time = null;
            allowed_cost_time = null;
            attendance_type_id = 0;
        }

        public string company_code { get; set; }

        public int user_sys_id { get; set; }

        public int actual_work_year { get; set; }

        public int actual_work_month { get; set; }

        public int actual_work_date { get; set; }

        public decimal? work_start_time { get; set; }

        public decimal? work_end_time { get; set; }

        /// <summary>
        /// 打刻時間（出社）
        /// </summary>
        public decimal? clock_in_start_time { get; set; }

        /// <summary>
        /// 打刻時間（退社）
        /// </summary>
        public decimal? clock_in_end_time { get; set; }

        public decimal? rest_time { get; set; }

        public double? allowed_cost_time { get; set; }

        public int? attendance_type_id { get; set; }

        public string display_name { get; set; }

        public string remarks { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }
    }
}