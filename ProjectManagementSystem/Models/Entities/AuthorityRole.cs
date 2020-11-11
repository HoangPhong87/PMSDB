using System;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_authority_role")]
    [PetaPoco.PrimaryKey("company_code, role_id", autoIncrement = false)]
    public class AuthorityRole
    {
        public string company_code { get; set; }

        public int? role_id { get; set; }

        public string role_name { get; set; }

        public string display_name { get; set; }

        public string remarks { get; set; }

        public int display_order { get; set; }

        public DateTime ins_date { get; set; }

        public int ins_id { get; set; }

        public DateTime upd_date { get; set; }

        public int upd_id { get; set; }

        public string del_flg { get; set; }

        public string password_lock_target { get; set; }
    }
}