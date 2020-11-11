#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2014/09/04</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS09002
{
    [Serializable]
    public class Condition
    {
        public string USER_NAME { get; set; }

        public string FROM_DATE { get; set; }

        public string TO_DATE { get; set; }

        public int? GROUP_ID { get; set; }

        public string CONTRACT_TYPE_ID { get; set; }

        public bool DELETE_FLG { get; set; }

        public bool RETIREMENT_INCLUDE { get; set; }

        public int? SORT_COL { get; set; }

        public string SORT_TYPE { get; set; }

        public bool IS_PRIVATE { get; set; }

        public string SELECT_SORT_TYPE { get; set; }
        public string LOCATION_ID { get; set; }
    }
}
