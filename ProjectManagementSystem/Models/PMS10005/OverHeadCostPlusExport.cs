using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS10005
{
    using PetaPoco;
    public class OverHeadCostPlusExport
    {
        public int no { get; set; }
        public string overhead_cost_type { get; set; }
        public string remarks { get; set; }
        public string ins_date { get; set; }
        public string user_regist { get; set; }
    }
}