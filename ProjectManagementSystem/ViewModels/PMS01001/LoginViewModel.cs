using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.ViewModels.PMS01001
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DisplayName("企業コード")] 
        public string CompanyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DisplayName("ユーザーアカウント")] 
        public string UserAccount { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DisplayName("パスワード")] 
        public string Password { get; set; }
    }
}