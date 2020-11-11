using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("sales_payment")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, ordering_flg, customer_id", autoIncrement = false)]
    public class SalesPayment
    {
        public string company_code { get; set; }

        public int? project_sys_id { get; set; }

        public string ordering_flg { get; set; }

        public int? customer_id { get; set; }

        public int? end_user_id { get; set; }

        public int? charge_person_id { get; set; }

        [DisplayName("合計金額")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal? total_amount { get; set; }

        public int? tag_id { get; set; }

        public DateTime? ins_date { get; set; }

        public int? ins_id { get; set; }
    }
}