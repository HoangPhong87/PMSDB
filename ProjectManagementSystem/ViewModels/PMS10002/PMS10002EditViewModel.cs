using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10002;

namespace ProjectManagementSystem.ViewModels.PMS10002
{
    public class PMS10002EditViewModel
    {
        public InformationPlus INFORMATION { get; set; }

        public PMS10002EditViewModel()
        {
            INFORMATION = new InformationPlus();
            INFORMATION.publish_start_date = Utility.GetCurrentDateTime();
            INFORMATION.publish_end_date = Utility.GetCurrentDateTime();
        }
        public bool Delete
        {
            get { return (INFORMATION.del_flg == Constant.DeleteFlag.DELETE); }
            set { INFORMATION.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }
    }
}