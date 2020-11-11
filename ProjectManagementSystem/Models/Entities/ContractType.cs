using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_contract_type")]
    [PetaPoco.PrimaryKey("company_code, contract_type_id", autoIncrement = false)]
    [Serializable]
    public class ContractType
    {
        public string company_code { get; set; }

        public int contract_type_id { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("契約種別")]
        public string contract_type { get; set; }

        [DisplayName("営業担当者")]
        public string charge_of_sales_flg { get; set; }

        [DisplayName("予算対象フラグ")]
        public string budget_setting_flg { get; set; }

        [DisplayName("特殊計算フラグ")]
        public string exceptional_calculate_flg { get; set; }

        [DisplayName("未計画")]
        public string check_plan_flg { get; set; }

        [DisplayName("進捗更新未")]
        public string check_progress_flg { get; set; }

        [DisplayName("期日終了")]
        public string check_period_flg { get; set; }

        [DisplayName("未売上入力")]
        public string check_sales_flg { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("備考")]
        public string remarks { get; set; }

        public int display_order { get; set; }

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