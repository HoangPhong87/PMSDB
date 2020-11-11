#region License
/// <copyright file="Condition.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>LinhDT</author>
/// <createdDate>2018/12/21</createdDate>
#endregion

using System.Collections.Generic;

namespace ProjectManagementSystem.Models.PMS08001
{
    public class GroupInfor
    {
        public string GROUP_NAME { get; set; }
        public IList<UserInfor> USER_LIST { get; set; }
    }
}