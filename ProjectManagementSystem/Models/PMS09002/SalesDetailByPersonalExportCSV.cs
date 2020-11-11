#region License
/// <copyright file="SalesDetailByPersonalExportCSV.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/01/16</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS09002
{
    [Serializable]
    public class SalesDetailByPersonalExportCSV
    {
        public string project_name { get; set; }

        public string sales { get; set; }

        public string cost { get; set; }

        public string profit { get; set; }

    }
}
