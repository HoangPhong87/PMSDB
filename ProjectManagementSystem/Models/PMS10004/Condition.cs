using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.PMS10004
{
    [Serializable]
    public class Condition
    {

        public string COMPANY_CODE { get; set; }
        /// <summary>
        /// </summary>
        [DisplayName("カテゴリ")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string CATEGORY_NAME { get; set; }

        /// <summary>
        /// </summary>
        [DisplayName("サブカテゴリ")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string SUB_CATEGORY_NAME { get; set; }

        public bool DELETED_INCLUDE { get; set; }
    }
}