using ProjectManagementSystem.Models.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Resources;

using System;

namespace ProjectManagementSystem.Models.PMS10001
{
    [Serializable]
    public class CustomerTagPlus : CustomerTag
    {
        public int peta_rn { get; set; }

        [DisplayName("取引先名")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        public string display_name { get; set; }

        [DisplayName("更新者")]
        public string user_update { get; set; }

        [DisplayName("登録者")]
        public string user_regist { get; set; }
    }
}