using ProjectManagementSystem.Models.Entities;
using System;

namespace ProjectManagementSystem.Models.PMS06003
{
    using PetaPoco;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// project_info plus model class
    /// </summary>
    public class BaseUnitCost
    {
        public int user_sys_id { get; set; }

        public string company_code { get; set; }

        public string group_name { get; set; }

        public string user_name { get; set; }

        public string Month { get; set; }

        public decimal base_unit_cost { get; set; }
    }
}