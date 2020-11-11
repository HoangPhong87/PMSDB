using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("project_info_history")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, history_no", autoIncrement = false)]
    public class ProjectInfoHistory
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public long history_no { get; set; }

        public decimal? estimate_man_days { get; set; }

        public decimal? total_sales { get; set; }

        public decimal? total_payment { get; set; }

        public decimal? tax_rate { get; set; }

        public decimal? gross_profit { get; set; }

        public string delete_status { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        public string insert_date { get; set; }
    }
}