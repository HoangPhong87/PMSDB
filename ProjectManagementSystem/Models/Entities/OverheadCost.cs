using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("overhead_cost")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, detail_no", autoIncrement = false)]
    public class OverheadCost
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public long? detail_no { get; set; }

        public int? overhead_cost_id { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("諸経費一覧（諸経費内容）")]
        public string overhead_cost_detail { get; set; }

        public int? charge_person_id { get; set; }

        [DisplayName("諸経費一覧（合計金額）")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal? total_amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        [DataType(DataType.Date)]
        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }
    }
}