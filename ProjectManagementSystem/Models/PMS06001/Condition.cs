#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2014/05/08</createdDate>
#endregion

using ProjectManagementSystem.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.PMS06001
{
    [Serializable]
    public class Condition
    {
        public string PROJECT_NAME { get; set; }

        public int TIME_CONDITION_TYPE { get; set; }

        public string FROM_DATE { get; set; }

        public string TO_DATE { get; set; }

        public int? CONTRACT_TYPE_ID { get; set; }

        public int? STATUS_ID { get; set; }

        public int? COMPLETE_ID { get; set; }

        public List<string> GROUP_ID { get; set; }

        public int? CUSTOMER_ID { get; set; }

        public int? TAG_ID { get; set; }

        public string CUSTOMER_NAME { get; set; }

        public bool DELETE_FLG { get; set; }

        public int? CHARGE_PERSON_ID { get; set; }

        public string CHARGE_PERSON { get; set; }

        public int? sortCol { get; set; }

        public string sortDir { get; set; }

    }
}
