using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_prefecture")]
    [PetaPoco.PrimaryKey("prefecture_code", autoIncrement = false)]
    public class Prefecture
    {
        public string prefecture_code { get; set; }

        public string prefecture_name { get; set; }

        public string prefecture_name_kana { get; set; }

        public int display_order { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }
    }
}