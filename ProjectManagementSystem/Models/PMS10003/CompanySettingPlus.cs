using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS10003
{
    using PetaPoco;
    public class CompanySettingPlus : CompanySetting
    {
        public string user_update { get; set; }
        public string user_regist { get; set; }
    }
}