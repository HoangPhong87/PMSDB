using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_category")]
    [PetaPoco.PrimaryKey("company_code, category_id", autoIncrement = false)]
    [Serializable]
    public class Category
    {
        public string company_code { get; set; }

        public int category_id { get; set; }

        [DisplayName("カテゴリ")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string category { get; set; }

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