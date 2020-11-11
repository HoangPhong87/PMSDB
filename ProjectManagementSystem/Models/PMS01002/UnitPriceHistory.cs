using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("unit_price_history")]
    [PetaPoco.PrimaryKey("company_code,user_sys_id,apply_start_date", autoIncrement = false)]
    public class UnitPriceHistory
    {
        public string company_code { get; set; }

        public int user_sys_id { get; set; }

        public DateTime apply_start_date { get; set; }

        public decimal? base_unit_cost { get; set; }

        public int display_order { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("備考")]
        public string remarks { get; set; }

        [DisplayName("登録日時")]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        [DisplayName("更新日時")]
        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

        public char del_flag { get; set; }
    }
}