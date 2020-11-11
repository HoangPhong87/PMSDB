using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_function")]
    [PetaPoco.PrimaryKey("function_id")]
    public class Function
    {
        public int? function_id { get; set; }

        public string function_name { get; set; }

        public string display_name { get; set; }

        public string remarks { get; set; }

        public int display_order { get; set; }

        public DateTime ins_date { get; set; }

        public int? ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int? upd_id { get; set; }

        public string del_flg { get; set; }
    }
}