using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_overhead_cost")]
    [PetaPoco.PrimaryKey("company_code, overhead_cost_id", autoIncrement = false)]
    public class OverheadCostType
    {
        public string company_code { get; set; }

        public int overhead_cost_id { get; set; }

        public string overhead_cost_type { get; set; }

        public string remarks { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }
    }
}