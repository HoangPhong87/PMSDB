#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/07/15</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS09004
{
    [Serializable]
    public class Condition
    {
        public string COMPANY_CODE { get; set; }

        public string TARGET_TIME { get; set; }

        public string TARGET_TIME_START { get; set; }

        public string TARGET_TIME_END { get; set; }
        public int? GROUP_ID { get; set; }

        public string CONTRACT_TYPE_ID { get; set; }

        public string USER_NAME { get; set; }

        public int? CUSTOMER_ID { get; set; }

        public bool DELETE_FLG { get; set; }

        public bool PLAN_DISPLAY { get; set; }

        public bool PLANNED_MEMBER_INCLUDE { get; set; }

        public string LOCATION_ID { get; set; }

        public bool ESTIMATE_DISPLAY { get; set; }
    }
}
