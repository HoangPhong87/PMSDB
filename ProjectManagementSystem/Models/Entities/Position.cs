using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_position")]
    [PetaPoco.PrimaryKey("company_code, position_id", autoIncrement = false)]
    public class Position
    {
        public string company_code { get; set; }

        public int position_id { get; set; }

        public string position_name { get; set; }

        public string display_name { get; set; }

        public string remarks { get; set; }

        public int display_order { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }
    }
}