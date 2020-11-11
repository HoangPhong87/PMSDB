using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("r_contract_type_category")]
    [PetaPoco.PrimaryKey("company_code, contract_type_id, category_id", autoIncrement = false)]
    public class ContractTypeCategory
    {
        public string company_code { get; set; }

        public int contract_type_id { get; set; }

        [DisplayName("カテゴリ")]
        public int? category_id { get; set; }

        [DisplayName("カテゴリ(表示順)")]
        [Range(0, 99, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public int? display_order { get; set; }

        public DateTime? ins_date { get; set; }

        public int? ins_id { get; set; }
    }
}