using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10003;

namespace ProjectManagementSystem.ViewModels.PMS10003
{
    public class PMS10003CompanyInfoViewModel
    {
        public CompanyPlus COMPANY_INFO { get; set; }
        public IList<SelectListItem> PREFECTURE_LIST { get; set; }
        public string Clear { get; set; }
        public string TypeUpload { get; set; }
        public PMS10003CompanyInfoViewModel()
        {
            COMPANY_INFO = new CompanyPlus();
        }
    }
}