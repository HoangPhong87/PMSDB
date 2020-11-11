#region License
/// <copyright file="SalesDetailByPersonalCondition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/05/21</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS09002
{
    [Serializable]
    public class SalesDetailByPersonalCondition
    {
        public string CompanyCode { get; set; }
        public int UserID { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public string ContractTypeID { get; set; }
    }
}
