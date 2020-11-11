using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS09003
{
    [Serializable]
    public class SalesTagByCustomerPlus
    {
        public int tag_id { get; set; }
        public int peta_rn { get; set; }
        public string display_name { get; set; }
        public decimal total_sales { get; set; }
        public decimal sale_proceeds { get; set; }
        public decimal gross_profit { get; set; }
        public decimal gross_profit_rate { get; set; }
    }
}