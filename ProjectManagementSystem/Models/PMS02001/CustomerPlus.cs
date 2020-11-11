using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Models.Entities;
using PetaPoco;
namespace ProjectManagementSystem.Models.PMS02001
{
    [Serializable]
    public class CustomerPlus : Customer
    {
        public int peta_rn { get; set; }

        public string address { get; set; }

        public string user_update { get; set; }

        public string user_regist { get; set; }
    }
}