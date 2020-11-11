#region License
/// <copyright file="SalesDetailByPersonal.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/01/15</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS09002
{
    [Serializable]
    public class SalesDetailByPersonal
    {
        public string project_no { get; set; }

        public string project_name { get; set; }

        public decimal project_sales { get; set; }

        public decimal actual_cost { get; set; }

        public decimal profit { get; set; }

    }
}
