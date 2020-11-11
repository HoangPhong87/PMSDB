using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS10004;

namespace ProjectManagementSystem.ViewModels.PMS10004
{
    public class PMS10004ListViewModel
    {
          /// <summary>
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// Callback function name return execution result
        /// </summary>
        public string CallBack { get; set; }

        public string IS_MULTI_SELECT { get; set; }

        public PMS10004ListViewModel()
        {
            Condition = new Condition();
        }
    }
}