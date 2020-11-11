#region License
/// <copyright file="Condition.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>LinhDT</author>
/// <createdDate>2018/12/21</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;

namespace ProjectManagementSystem.Models.PMS08001
{
    public class AttendanceRecordPlus:AttendanceRecord
    {
        public string non_operational_flg { get; set; }
    }
}