using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_business_location")]
    [PetaPoco.PrimaryKey("company_code, location_id", autoIncrement = false)]
    [Serializable]
    public class BusinessLocation
    {
        public string company_code { get; set; }
        public int location_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("拠点名")]
        public string location_name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("拠点(表示名)")]
        public string display_name { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("備考")]
        public string remarks { get; set; }

        public int? display_order { get; set; }

        [DisplayName("登録日時")]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        [DisplayName("更新日時")]
        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

        [DisplayName("削除")]
        public string del_flg { get; set; }
    }
}