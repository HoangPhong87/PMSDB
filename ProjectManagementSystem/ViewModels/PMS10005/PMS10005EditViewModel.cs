using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10005;

namespace ProjectManagementSystem.ViewModels.PMS10005
{
    public class PMS10005EditViewModel
    {
        public OverHeadCostPlus OVERHEAD_COST {get; set;}

        public PMS10005EditViewModel()
        {
            OVERHEAD_COST = new OverHeadCostPlus();
        }
    }
}