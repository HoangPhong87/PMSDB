#region License
 /// <copyright file="ProjectCondition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>TrungNT</author>
 /// <createdDate>2014/05/13</createdDate>
#endregion

using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.PMS06003
{
    [Serializable]
    public class ProjectCondition
    {
        /// <summary>
        /// Project Name
        /// </summary>
        public string PROJECT_NAME { get; set; }

        /// <summary>
        /// ID of customer
        /// </summary>
        public int? CUSTOMER_ID { get; set; }

        /// <summary>
        /// ID of group
        /// </summary>
        public int? GROUP_ID { get; set; }

        /// <summary>
        /// ID of tag
        /// </summary>
        public int? TAG_ID { get; set; }

        public string STATUS_ID { get; set; }

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

        /// <summary>
        /// Effort type
        /// </summary>
        public int? EFF_TYPE { get; set; }

        public bool DELETE_FLG { get; set; }

        public bool INACTIVE_FLG { get; set; }

    }
}
