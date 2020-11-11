#region License
 /// <copyright file="UserCondition.cs" company="i-Enter Asia">
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
    public class UserCondition
    {
        /// <summary>
        /// User Name
        /// </summary>
        public string USER_NAME { get; set; }

        /// <summary>
        /// ID of group
        /// </summary>
        public int? GROUP_ID { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        public string START_DATE { get; set; }

        /// <summary>
        /// End Date
        /// </summary>
        public string END_DATE { get; set; }

        /// <summary>
        /// Effort type
        /// </summary>
        public int? EFF_TYPE { get; set; }

        /// <summary>
        /// Retired include
        /// </summary>
        public bool RETIRED_INCLUDE { get; set; }

        public string STATUS_ID { get; set; }
        public string LOCATION_ID { get; set; }
    }
}
