using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS02001
{
    public class CustomerPlusExport
    {
        public int customer_id { get; set; }
        public string display_name { get; set; }
        public string customer_name_kana { get; set; }
        public string address { get; set; }
        public string url { get; set; }
        public string upd_date { get; set; }
        public string user_update { get; set; }
    }
}