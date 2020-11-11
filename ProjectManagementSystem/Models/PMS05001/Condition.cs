#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HoangPS</author>
 /// <createdDate>2015/01/22</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS05001
{
    [Serializable]
    public class Condition
    {
        public string GroupName { get; set; }

        public bool DeleteFlag { get; set; }
    }
}
