using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("password_reset_management")]
    [PetaPoco.PrimaryKey("reset_id", autoIncrement = true)]
    public class PasswordResetManagement
    {
        public long reset_id { get; set; }

        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
        ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        [DisplayName("メールアドレス")]
        public string mail_address { get; set; }

        public string parameter_value { get; set; }

        public DateTime apply_time { get; set; }

        public string company_code { get; set; }
    }
}