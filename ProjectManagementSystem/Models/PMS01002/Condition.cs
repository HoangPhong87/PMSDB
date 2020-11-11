using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.PMS01002
{
    [Serializable]
    public class Condition
    {
        /// <summary>
        /// </summary>
        [DisplayName("ユーザー名")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string DISPLAY_NAME { get; set; }

        /// <summary>
        /// </summary>
        [DisplayName("メールアドレス")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string MAIL_ADDRESS { get; set; }

        public int? GROUP_ID { get; set; }

        public int? POSITION_ID { get; set; }

        public bool DELETED_INCLUDE { get; set; }

        public bool RETIREMENT_INCLUDE { get; set; }

        public int PROJECT_ID { get; set; }

        public string FROM_DATE { get; set; }

        public string TO_DATE { get; set; }
    }
}