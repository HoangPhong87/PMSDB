#region License
/// <copyright file="ActualWorkCondition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/08/13</createdDate>
#endregion

using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.PMS06001
{
    [Serializable]
    public class ActualWorkCondition
    {
        public int PROJECT_ID { get; set; }

        public int USER_ID { get; set; }

        public DateTime FROM_DATE { get; set; }

        public DateTime TO_DATE { get; set; }

        public string TIME_UNIT { get; set; }
    }
}
