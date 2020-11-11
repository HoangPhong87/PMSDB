using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS10001
{
    public class CustomerTagExport
    {
        public int tag_id { get; set; }
        public string display_name { get; set; }
        public string tag_name { get; set; }
        public string display_order { get; set; }
        public string upd_date { get; set; }
        public string user_update { get; set; }
    }
}