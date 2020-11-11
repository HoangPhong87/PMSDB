using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS02001;

namespace ProjectManagementSystem.ViewModels.PMS02001
{
    public class PMS02001ListViewModel
    {
          /// <summary>
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// Callback function name return execution result
        /// </summary>
        public string CallBack { get; set; }

        public string IS_MULTI_SELECT { get; set; }

        public string SearchByObject { get; set; }

        public PMS02001ListViewModel()
        {
            Condition = new Condition();
        }
    }
}