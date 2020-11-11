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
    public class CompanyCodeViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [DisplayName("企業コード")] 
        public string CompanyCode { get; set; }

        [DisplayName("メールアドレス")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
        ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string Email { get; set; }
    }
}