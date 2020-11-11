using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.Entities;
using PetaPoco;
namespace ProjectManagementSystem.Models.PMS10005
{
    [Serializable]
    public class OverHeadCostPlus : M_OverheadCost
    {
        public int peta_rn { get; set; }

        public string user_regist { get; set; }
    }
}