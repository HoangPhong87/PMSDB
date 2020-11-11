using ProjectManagementSystem.Models.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

using System;

namespace ProjectManagementSystem.Models.PMS10002
{
    [Serializable]
    public class InformationPlus : Information
    {
        public int peta_rn { get; set; }
        public string display_name { get; set; }
        public string user_update { get; set; }
        public string user_regist { get; set; }
    }
}