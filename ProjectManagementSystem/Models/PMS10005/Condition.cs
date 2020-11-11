using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.PMS10005
{
    [Serializable]
    public class Condition
    {
        public string COMPANY_CODE { get; set; }

        /// <summary>
        /// </summary>
        [DisplayName("諸経費種別")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E020")]
        public string OVERHEAD_COST_TYPE { get; set; }
    }
}