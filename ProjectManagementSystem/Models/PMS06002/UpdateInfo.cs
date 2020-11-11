using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS06002
{
    public class UpdateInfo
    {
        public string regist_type { get; set; }
        public string last_update_date { get; set; }
        public string last_update_person { get; set; }
        public string insert_date { get; set; }
        public string insert_person { get; set; }

        public UpdateInfo()
        {
            last_update_date = " ";
            last_update_person = " ";
            insert_date = " ";
            insert_person = " ";
            regist_type = "仮登録";
        }
    }
}