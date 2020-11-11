#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/01/23</createdDate>
#endregion

using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.PMS07001
{
    [Serializable]
    public class Condition
    {
        [DisplayName("適用開始日")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public DateTime? ApplyStartDate { get; set; }
    }
}
