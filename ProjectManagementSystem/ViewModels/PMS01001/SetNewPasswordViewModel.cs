using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.ViewModels.PMS01001
{
    public class SetNewPasswordViewModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        [DisplayName("新しいパスワード")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(32, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [RegularExpression(@"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string NewPassword { get; set; }

        [DisplayName("新しいパスワード（確認用）")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(32, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [RegularExpression(@"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string NewPasswordConfirm { get; set; }

        public string PasswordLockTarget { get; set; }

        public string CompanyCode { get; set; }
    }
}