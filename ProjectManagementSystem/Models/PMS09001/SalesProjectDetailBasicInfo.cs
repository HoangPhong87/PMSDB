using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS09001
{
    public class SalesProjectDetailBasicInfo
    {
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }  
        public string project_name { get; set; }
        public string group_name { get; set; }
    }
}