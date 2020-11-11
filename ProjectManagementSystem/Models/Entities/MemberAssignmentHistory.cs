using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("member_assignment_history")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, user_sys_id, history_no", autoIncrement = false)]
    public class MemberAssignmentHistory
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public int user_sys_id { get; set; }

        public long history_no { get; set; }

        public decimal? unit_cost { get; set; }

        public decimal? total_plan_man_days { get; set; }

        public decimal? total_plan_cost { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }
    }
}