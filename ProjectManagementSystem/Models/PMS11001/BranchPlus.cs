#region License
/// <copyright file="GroupPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ</author>
/// <createdDate>2017/06/28</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS11001
{
    /// <summary>
    /// m_business_location plus model class
    /// </summary>
    [Serializable]
    public class BranchPlus : BusinessLocation
    {
        public int peta_rn { get; set; }

        [DisplayName("登録者")]
        public string ins_user { get; set; }

        [DisplayName("更新者")]
        public string upd_user { get; set; }
    }
}