using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS10002;

namespace ProjectManagementSystem.ViewModels.PMS10002
{
    public class PMS10002ListViewModel
    {
        /// <summary>
        /// </summary>
        public Condition Condition { get; set; }

        public PMS10002ListViewModel()
        {
            Condition = new Condition();
        }
    }
}