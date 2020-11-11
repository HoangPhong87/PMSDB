using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS10004;

namespace ProjectManagementSystem.ViewModels.PMS10004
{
    public class PMS10004EditViewModel
    {
        public CategoryPlus CATEGORY {get; set;}
        public List<SubCategoryPlus> LIST_SUBCATEGORY { get; set; }
        public bool Delete
        {
            get { return (CATEGORY.del_flg == Constant.DeleteFlag.DELETE); }
            set { CATEGORY.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }
        public PMS10004EditViewModel()
        {
            CATEGORY = new CategoryPlus();
            LIST_SUBCATEGORY = new List<SubCategoryPlus>();
        }
    }
}