using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;


namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_customer_tag")]
    [PetaPoco.PrimaryKey("company_code, customer_id, tag_id", autoIncrement = false)]
    [Serializable]
    public class CustomerTag
    {
        public string company_code { get; set; }

        public int customer_id { get; set; }

        public int tag_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DisplayName("タグ名")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string tag_name { get; set; }

        [DisplayName("備考")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string remarks { get; set; }

        public int? display_order { get; set; }

        [DisplayName("登録日時")]
        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        [DisplayName("更新日時")]
        [DataType(DataType.Date)]
        public DateTime upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }
    }
}