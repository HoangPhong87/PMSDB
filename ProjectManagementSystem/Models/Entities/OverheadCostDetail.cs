using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("overhead_cost_detail")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, detail_no, target_year, target_month", autoIncrement = false)]
    public class OverheadCostDetail
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public long detail_no { get; set; }

        public int target_year { get; set; }

        public int target_month { get; set; }

        [DisplayName("諸経費一覧の金額")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal? amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }
    }
}