using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Models.PMS01002;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.ViewModels.PMS01002
{
    public class PMS01002PersonalSettingViewModel
    {
        public UserPlus USER_INFO { get; set; }

        [DisplayName("パスワード（確認用）")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(32, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string confirmPassword { get; set; }

        /// <summary>
        /// </summary>
        public IList<SelectListItem> GROUP_LIST { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<SelectListItem> POSITION_LIST { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<SelectListItem> LANGUAGE_LIST { get; set; }

        public string Clear { get; set; }

        public string TypeUpload { get; set; }

        public PMS01002PersonalSettingViewModel()
        {
            USER_INFO = new UserPlus();
        }
    }
}