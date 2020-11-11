using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS01002
{
    public class UserPlusExport
    {
        public int user_id { get; set; }
        public string display_name { get; set; }
        public string employee_no { get; set; }
        public string display_name_group { get; set; }
        public string display_name_position { get; set; }
        public string base_unit_cost { get; set; }
        public string entry_date { get; set; }
        public string mail_address { get; set; }
        public string is_active { get; set; }
        public string upd_date { get; set; }
        public string user_update { get; set; }
    }
}