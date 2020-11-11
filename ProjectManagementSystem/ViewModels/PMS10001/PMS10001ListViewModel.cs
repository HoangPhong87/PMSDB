using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS10001;

namespace ProjectManagementSystem.ViewModels.PMS10001
{
    public class PMS10001ListViewModel
    {
        /// <summary>
        /// </summary>
        public Condition Condition { get; set; }

        public PMS10001ListViewModel()
        {
            Condition = new Condition();
        }
    }
}