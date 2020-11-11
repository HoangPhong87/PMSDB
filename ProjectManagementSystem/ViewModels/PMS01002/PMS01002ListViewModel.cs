using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS01002;

namespace ProjectManagementSystem.ViewModels.PMS01002
{
    public class PMS01002ListViewModel
    {
        /// <summary>
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// </summary>
        public IList<SelectListItem> GROUP_LIST { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<SelectListItem> POSITION_LIST { get; set; }

        /// <summary>
        /// Callback function name return execution result
        /// </summary>
        public string CallBack { get; set; }

        public string IS_MULTI_SELECT { get; set; }

        public PMS01002ListViewModel()
        {
            Condition = new Condition();
        }
    }
}