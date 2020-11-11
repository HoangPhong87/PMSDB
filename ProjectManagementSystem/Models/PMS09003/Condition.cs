﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Models.PMS09003
{
    [Serializable]
    public class Condition
    {
        public int? CUSTOMER_ID { get; set; }

        public int? TAG_ID { get; set; }

        public string CUSTOMER_NAME { get; set; }

        public string CONTRACT_TYPE_ID { get; set; }

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

        public int? GROUP_ID { get; set; }
        public string LOCATION_ID { get; set; }

        public bool DELETED_INCLUDE { get; set; }

    }
}