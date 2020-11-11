using ProjectManagementSystem.ViewModels.PMS11003;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagementSystem.Models.PMS11003
{
    public class DataSalesProfitExport
    {
        public PMS11003ListViewModel data_sale { get; set; }
        public PMS11003ListViewModelPlus data_profit { get; set; }
    }

    public class Ordinate
    {
        public int posX { get; set; }
        public int posY { get; set; }
    }

    public class SalesBudget
    {
        public string contract_type { get; set; }
        public int contract_type_id { get; set; }
        public int group_id { get; set; }
        public string group_name { get; set; }
        public decimal sales_budget { get; set; }
        public int target_month { get; set; }
        public int target_year { get; set; }
    }

}