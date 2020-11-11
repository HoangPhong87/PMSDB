using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_status")]
    [PetaPoco.PrimaryKey("status_id, company_code", autoIncrement = false)]
    public class Status
    {
        public string company_code { get; set; }

        public int status_id { get; set; }

        public string status { get; set; }

        public string sales_type { get; set; }

        public string completed_flg { get; set; }

        public string remarks { get; set; }

        public int display_order { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }

        public string operation_target_flg { get; set; }
    }
}