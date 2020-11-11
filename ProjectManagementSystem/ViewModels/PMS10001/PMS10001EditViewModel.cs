using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10001;

namespace ProjectManagementSystem.ViewModels.PMS10001
{
    public class PMS10001EditViewModel
    {   
        public CustomerTagPlus CUSTOMERTAG_INFO { get; set; }

        public PMS10001EditViewModel()
        {
            CUSTOMERTAG_INFO = new CustomerTagPlus();
        }
        public bool Delete
        {
            get { return (CUSTOMERTAG_INFO.del_flg == Constant.DeleteFlag.DELETE); }
            set { CUSTOMERTAG_INFO.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }
    }
}