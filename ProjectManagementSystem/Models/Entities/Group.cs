using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_group")]
    [PetaPoco.PrimaryKey("company_code, group_id", autoIncrement = false)]
    [Serializable]
    public class Group
    {
        public string company_code { get; set; }

        public int group_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("所属名")]
        public string group_name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("所属(表示名)")]
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

        [DisplayName("予算対象フラグ")]
        public string budget_setting_flg { get; set; }

        [DisplayName("稼働入力チェック対象")]
        public string check_actual_work_flg { get; set; }
    }
}