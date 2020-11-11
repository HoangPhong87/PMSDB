using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS10004
{
    using PetaPoco;
    public class CategoryPlusExport
    {
        public int no { get; set; }
        public string category { get; set; }
        public string sub_category { get; set; }
        public string upd_date { get; set; }
        public string user_update { get; set; }
    }
}