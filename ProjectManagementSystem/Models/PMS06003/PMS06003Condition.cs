#region License
 /// <copyright file="PMS06003Condition.cs" company="i-Enter Asia">
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
    public class PMS06003Condition
    {
        /// <summary>
        /// </summary>
        public string PROJECT_NAME { get; set; }

        /// <summary>
        /// </summary>
        public int CUSTOMER_ID { get; set; }

        /// <summary>
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Range(typeof(DateTime), "1753/01/01", "9999/12/31", ErrorMessageResourceName = "W009", ErrorMessageResourceType = typeof(Messages))]
        public DateTime? START_DATE { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Range(typeof(DateTime), "1753/01/01", "9999/12/31", ErrorMessageResourceName = "W009", ErrorMessageResourceType = typeof(Messages))]
        public DateTime? END_DATE { get; set; }
    }
}
