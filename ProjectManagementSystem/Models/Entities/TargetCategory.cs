using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("target_category")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, category_id, sub_category_id", autoIncrement = false)]
    public class TargetCategory
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        [DisplayName("カテゴリ")]
        public int? category_id { get; set; }

        [DisplayName("サブカテゴリ")]
        public int? sub_category_id { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }
    }
}