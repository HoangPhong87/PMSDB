using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS10002
{
    public class InformationExport
    {
        public int info_no { get; set; }
        public string content { get; set; }
        public string publish_start_date { get; set; }
        public string publish_end_date { get; set; }
        public string upd_date { get; set; }
        public string user_update { get; set; }
    }
}