using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_rank")]
    [PetaPoco.PrimaryKey("company_code, rank_id", autoIncrement = false)]
    public class Rank
    {
        public string company_code { get; set; }

        public int rank_id { get; set; }

        public string rank { get; set; }

        public string remarks { get; set; }

        public int display_order { get; set; }

        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }
    }
}