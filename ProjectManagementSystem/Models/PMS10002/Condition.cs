using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.PMS10002
{
    [Serializable]
    public class Condition
    {
        /// <summary>
        /// </summary>
        [DisplayName("掲載内容")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string INFORMATION_CONTENT { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM}")]
        [DataType(DataType.Date)]
        public string START_DATE { get; set; }

        /// <summary>
        /// End Date
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM}")]
        [DataType(DataType.Date)]
        public string END_DATE { get; set; }

        public bool DELETED_INCLUDE { get; set; }
    }
}