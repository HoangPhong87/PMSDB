using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("member_actual_work")]
    [PetaPoco.PrimaryKey("company_code, user_sys_id, actual_work_year, actual_work_month", autoIncrement = false)]
    public class MemberActualWork
    {
        public string company_code { get; set; }

        public int user_sys_id { get; set; }

        public int actual_work_year { get; set; }

        public int actual_work_month { get; set; }

        public decimal? total_actual_work { get; set; }

        public string regist_type { get; set; }

        public string fix_flg { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }
    }
}