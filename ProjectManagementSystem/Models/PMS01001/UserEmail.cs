using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS01001
{
    public class UserEmail
    {
        public int user_sys_id { get; set; }

        public string company_code { get; set; }

        public string user_account { get; set; }

        public string mail_address_1 { get; set; }

        public string mail_address_2 { get; set; }

        public string password_lock_target { get; set; }
    }
}