using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("numbering")]
    [PetaPoco.PrimaryKey("company_code", autoIncrement = false)]
    public class Numbering
    {
        public string company_code { get; set; }

        public string latest_project_no { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime upd_date { get; set; }

        public int upd_id { get; set; }
    }
}