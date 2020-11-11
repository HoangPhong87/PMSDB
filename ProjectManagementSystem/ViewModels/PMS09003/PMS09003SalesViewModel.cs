using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS09003;
using ProjectManagementSystem.Common;

namespace ProjectManagementSystem.ViewModels.PMS09003
{
    public class PMS09003SalesViewModel
    {
         /// <summary>
        /// </summary>
        public Condition Condition { get; set; }

        public IList<SelectListItem> GROUP_LIST { get; set; }

        public IList<SelectListItem> TAG_LIST { get; set; }

        public IList<SelectListItem> CONTRACT_TYPE_LIST { get; set; }
        public IList<SelectListItem> BRANCH_LIST { get; set; }

        /// <summary>
        public PMS09003SalesViewModel()
        {
            Condition = new Condition();
            var now = Utility.GetCurrentDateTime();

            Condition.START_DATE = now.ToString("yyyy/MM");
            Condition.END_DATE = now.AddMonths(11).ToString("yyyy/MM");
        }
    }
}