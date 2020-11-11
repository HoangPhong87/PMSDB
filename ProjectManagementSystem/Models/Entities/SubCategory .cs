using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_sub_category")]
    [PetaPoco.PrimaryKey("company_code, category_id, sub_category_id", autoIncrement = false)]
    public class SubCategory 
    {
        public string company_code { get; set; }

        public int category_id { get; set; }

        public int sub_category_id { get; set; }

        [DisplayName("サブカテゴリ")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string sub_category { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("備考")]
        public string remarks { get; set; }

        public int display_order { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime upd_date { get; set; }

        public int upd_id { get; set; }

        [DisplayName("削除")]
        public string del_flg { get; set; }
    }
}