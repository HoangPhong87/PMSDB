using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("target_phase")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, phase_id", autoIncrement = false)]
    public class TargetPhase
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public int phase_id { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public decimal estimate_man_days { get; set; }
    }
}