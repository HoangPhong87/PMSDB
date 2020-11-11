using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS02001;

namespace ProjectManagementSystem.ViewModels.PMS02001
{
    public class PMS02001EditViewModel
    {
        public CustomerPlus CUSTOMER_INFO { get; set; }

        public int OLD_CUSTOMER_ID { get; set; }

        public string Clear { get; set; }
        public string TypeUpload { get; set; }
        public IList<SelectListItem> PREFECTURE_LIST { get; set; }

        public PMS02001EditViewModel()
        {
            CUSTOMER_INFO = new CustomerPlus();
        }
        public bool Delete
        {
            get { return (CUSTOMER_INFO.del_flg == Constant.DeleteFlag.DELETE); }
            set { CUSTOMER_INFO.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool OLD_DEL_FLAG { get; set; }
    }
}