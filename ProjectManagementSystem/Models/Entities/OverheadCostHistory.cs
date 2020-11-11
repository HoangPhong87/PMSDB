using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("overhead_cost_history")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, detail_no, history_no", autoIncrement = false)]
    public class OverheadCostHistory
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public long detail_no { get; set; }

        public long history_no { get; set; }

        public int overhead_cost_id { get; set; }

        public string overhead_cost_detail { get; set; }

        public int charge_person_id { get; set; }

        public decimal? total_amount { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }
    }
}