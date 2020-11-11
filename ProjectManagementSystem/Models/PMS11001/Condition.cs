#region License
 /// <copyright file="Condition.cs" company="i-Enter Asia">
 /// Copyright © 2014 i-Enter Asia. All rights reserved.
 /// </copyright>
 /// <project>Project Management System</project>
 /// <author>HungLQ - Clone</author>
 /// <createdDate>2017/06/28</createdDate>
#endregion

using System;

namespace ProjectManagementSystem.Models.PMS11001
{
    [Serializable]
    public class Condition
    {
        public string BranchName { get; set; }

        public bool DeleteFlag { get; set; }
    }
}
