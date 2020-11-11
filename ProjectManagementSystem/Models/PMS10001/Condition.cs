using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.PMS10001
{
    [Serializable]
    public class Condition
    {
        public int? CUSTOMER_ID { get; set; }

        public string CUSTOMER_NAME { get; set; }

        /// <summary>
        /// </summary>
        [DisplayName("タグ名")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string TAG_NAME { get; set; }

        public bool DELETED_INCLUDE { get; set; }
    }
}