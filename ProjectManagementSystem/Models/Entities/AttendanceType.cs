using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_attendance_type")]
    [PetaPoco.PrimaryKey("company_code, attendance_type_id", autoIncrement = false)]
    public class AttendanceType
    {
        public string company_code { get; set; }

        public int attendance_type_id { get; set; }

        public string attendance_type { get; set; }

        public string display_name { get; set; }

        public int adjustment_time { get; set; }

        public string overtime_flg { get; set; }

        public string remarks { get; set; }

        public int display_order { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }
    }
}