using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS10004
{
    using PetaPoco;
    using ProjectManagementSystem.Common;
    public class SubCategoryPlus : SubCategory
    {
        public bool Delete { get; set; }

        public bool Update { get; set; }

        public string sub_category_old { get; set; }

        public string remarks_old { get; set; }

        public int project_count { get; set; }
    }
}