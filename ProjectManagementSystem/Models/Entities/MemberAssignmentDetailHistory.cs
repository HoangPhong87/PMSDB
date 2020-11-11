using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("member_assignment_detail_history")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, user_sys_id, history_no, target_year, target_month", autoIncrement = false)]
    public class MemberAssignmentDetailHistory
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public int user_sys_id { get; set; }

        public long history_no { get; set; }

        public int target_year { get; set; }

        public int target_month { get; set; }

        public decimal? plan_man_days { get; set; }

        public decimal? individual_sales { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }
    }
}