using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS09003
{
    public class SalesTagByCustomerPlusExport
    {
        public int tag_id { get; set; }
        public string display_name { get; set; }
        public string total_sales { get; set; }
        public string sale_proceeds { get; set; }
        public string gross_profit { get; set; }
        public string gross_profit_rate { get; set; }
    }
}