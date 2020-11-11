using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("member_assignment_detail")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, user_sys_id, target_year, target_month", autoIncrement = false)]
    public class MemberAssignmentDetail
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public int user_sys_id { get; set; }

        public int target_year { get; set; }

        public int target_month { get; set; }

        [DisplayName("予定工数")]
        [RegularExpression(@"^[0-9]{1,4}([.][0-9]{1})?$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 9999.9, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal? plan_man_days { get; set; }

        [DisplayName("個人売上")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal? individual_sales { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }
    }
}