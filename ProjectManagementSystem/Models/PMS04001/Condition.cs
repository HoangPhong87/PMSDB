#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/01/20</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS04001
{
    [Serializable]
    public class Condition
    {
        public string PhaseName { get; set; }

        public bool DeleteFlag { get; set; }
    }
}
