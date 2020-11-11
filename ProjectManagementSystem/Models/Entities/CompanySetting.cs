using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_company_setting")]
    [PetaPoco.PrimaryKey("company_code", autoIncrement = false)]
    public class CompanySetting
    {
        public string company_code { get; set; }

        [DisplayName("規定休日")]
        public string default_holiday_type { get; set; }

        [DisplayName("チェック起点曜日")]
        public string check_point_week { get; set; }

        [DisplayName("規定労働時間/日")]
        [Range(1, 24, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public int default_work_time_days { get; set; }

        [DisplayName("パスワード入力上限")]
        [Range(1, 10, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public int password_input_limit { get; set; }

        [DisplayName("パスワード有効期限（月）")]
        [Range(0, 12, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public int password_effective_month { get; set; }

        [DisplayName("小数計算区分")]
        public string decimal_calculation_type { get; set; }

        [DisplayName("勤務時間単位（分）")]
        [Range(1, 60, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public int working_time_unit_minute { get; set; }

        [DisplayName("決算月")]
        [Range(1, 12, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public int account_closing_month { get; set; }

        [DisplayName("勤務締め日")]
        [Range(1, 31, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E019")]
        public int work_closing_date { get; set; }

        public int data_editable_time { get; set; }

        [DataType(DataType.Date)]
        public DateTime ins_date { get; set; }
        public int ins_id { get; set; }

        [DataType(DataType.Date)]
        public DateTime upd_date { get; set; }
        public int upd_id { get; set; }
    }
}