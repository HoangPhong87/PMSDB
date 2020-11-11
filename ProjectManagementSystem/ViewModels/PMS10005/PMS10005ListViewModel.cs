using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS10005;

namespace ProjectManagementSystem.ViewModels.PMS10005
{
    public class PMS10005ListViewModel
    {
          /// <summary>
        /// </summary>
        public Condition Condition { get; set; }

        public PMS10005ListViewModel()
        {
            Condition = new Condition();
        }
    }
}