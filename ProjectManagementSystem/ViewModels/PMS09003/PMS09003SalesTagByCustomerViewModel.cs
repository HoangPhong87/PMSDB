using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS09003;

namespace ProjectManagementSystem.ViewModels.PMS09003
{
    public class PMS09003SalesTagByCustomerViewMode
    {
        /// <summary>
        /// </summary>
        public ConditionSaleTag Condition { get; set; }

        public string Customer_Name { get; set; }

        public string Location_Name { get; set; }
        public string Group_Name { get; set; }
        public string Tag_Name { get; set; }
        public string Contract_Type_Name { get; set; }

        public PMS09003SalesTagByCustomerViewMode()
        {
            Condition = new ConditionSaleTag();
            Customer_Name = "";
        }
    }
}