using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("member_assignment")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, user_sys_id", autoIncrement = false)]
    public class MemberAssignment
    {
        public string company_code { get; set; }

        public int? project_sys_id { get; set; }

        public int user_sys_id { get; set; }

        [DisplayName("単価")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal? unit_cost { get; set; }

        [DisplayName("予定工数合計")]
        [RegularExpression(@"^[0-9]{1,4}([.][0-9]{1})?$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 9999.9, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal? total_plan_man_days { get; set; }

        [DisplayName("予定原価合計")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public decimal? total_plan_cost { get; set; }

        public DateTime? ins_date { get; set; }

        public int? ins_id { get; set; }
    }
}