using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS09001
{
    [Serializable]
    public class ProjectSalesInfo
    {
        public int project_sys_id { get; set; }
        public string project_no { get; set; }
        public string project_name { get; set; }
        public decimal sales_amount { get; set; }
        public decimal cost { get; set; }
        public decimal profit { get; set; }
    }
}