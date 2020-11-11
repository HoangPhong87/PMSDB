using ProjectManagementSystem.Common;
using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("project_info")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id", autoIncrement = false)]
    [Serializable]
    public class ProjectInfo
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        [DisplayName("プロジェクトNo.")]
        public string project_no { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("プロジェクト名")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        public string project_name { get; set; }

        [DisplayName("契約種別")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        public int contract_type_id { get; set; }

        [DisplayName("営業担当者")]
        public int? charge_of_sales_id { get; set; }

        [DisplayName("見積工数")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [RegularExpression(@"^[0-9]{1,4}([.][0-9]{1})?$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 9999.9, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal estimate_man_days { get; set; }

        [DisplayName("ランク")]
        public int? rank_id { get; set; }

        [DisplayName("期間（開始）")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        public DateTime? start_date { get; set; }

        [DisplayName("期間（終了）")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        public DateTime? end_date { get; set; }

        [DisplayName("検収日")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public DateTime? acceptance_date { get; set; }

        [DisplayName("ステータス")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        public int status_id { get; set; }

        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public DateTime? assign_fix_date { get; set; }

        [DisplayName("担当者")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E004")]
        public int charge_person_id { get; set; }

        [DisplayName("売上金額")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal total_sales { get; set; }

        [DisplayName("支払金額")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal? total_payment { get; set; }

        [DisplayName("消費税率")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [Range(0, 999, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public decimal tax_rate { get; set; }

        [DisplayName("状況")]
        [StringLength(2000)]
        public string status_note { get; set; }

        [DisplayName("備考")]
        [StringLength(2000)]
        public string remarks { get; set; }

        [DisplayName("登録日時")]
        [DataType(DataType.Date)]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        [DisplayName("更新日時")]
        [DataType(DataType.Date)]
        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

        [DisplayName("削除")]
        public string del_flg { get; set; }

        [PetaPoco.ResultColumn]
        public byte[] row_version { get; set; }
    }
}