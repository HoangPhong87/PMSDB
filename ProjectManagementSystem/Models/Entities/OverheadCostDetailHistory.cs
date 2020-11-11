using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("overhead_cost_detail")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, detail_no, history_no, target_year, target_month", autoIncrement = false)]
    public class OverheadCostDetailHistory
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public long detail_no { get; set; }

        public long history_no { get; set; }

        public int target_year { get; set; }

        public int target_month { get; set; }

        public decimal? amount { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }
    }
}