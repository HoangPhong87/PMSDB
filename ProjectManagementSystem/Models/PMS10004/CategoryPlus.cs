using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.Entities;
using PetaPoco;

namespace ProjectManagementSystem.Models.PMS10004
{
    [Serializable]
    public class CategoryPlus : Category
    {
        public int peta_rn { get; set; }
        public string sub_category { get; set; }
        public string user_update { get; set; }
        public string user_regist { get; set; }

        public string sub_del_flg { get; set; }
    }
}