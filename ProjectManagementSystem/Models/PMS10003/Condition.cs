using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.PMS10003
{
    [Serializable]
    public class Condition
    {
        /// <summary>
        /// </summary>
        [DisplayName("取引先名")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string DISPLAY_NAME { get; set; }

        /// <summary>
        /// </summary>
        [DisplayName("取引先名カナ")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string CUSTOMER_NAME_KATA { get; set; }

        public bool DELETED_INCLUDE { get; set; }
    }
}