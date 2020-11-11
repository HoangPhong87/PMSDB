using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS09003;

namespace ProjectManagementSystem.ViewModels.PMS09003
{
    public class PMS09003SalesProjectByCustomerViewModel
    {
        /// <summary>
        /// </summary>
        public ConditionSaleProject Condition { get; set; }

        public string Customer_Name { get; set; }

        public string Location_Name { get; set; }

        public string Tag_Name { get; set; }
        public string Group_Name { get; set; }
        public string Contract_Type_Name { get; set; }
        public PMS09003SalesProjectByCustomerViewModel()
        {
            Condition = new ConditionSaleProject();
            Customer_Name = "";
            Tag_Name = "";
            Group_Name = "";
            Contract_Type_Name = "";
        }
    }
}