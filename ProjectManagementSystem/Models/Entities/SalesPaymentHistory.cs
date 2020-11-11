using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("sales_payment_history")]
    [PetaPoco.PrimaryKey("company_code, project_sys_id, ordering_flg, customer_id, history_no", autoIncrement = false)]
    public class SalesPaymentHistory
    {
        public string company_code { get; set; }

        public int project_sys_id { get; set; }

        public string ordering_flg { get; set; }

        public int customer_id { get; set; }

        public int? end_user_id { get; set; }

        public long history_no { get; set; }

        public int? charge_person_id { get; set; }

        public decimal? total_amount { get; set; }

        public int? tag_id { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }
    }
}