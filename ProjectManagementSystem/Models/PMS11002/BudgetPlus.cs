#region License
/// <copyright file="BudgetPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS11002
{
    /// <summary>
    /// 
    /// </summary>
    public class BudgetPlus : Budget
    {
        public int peta_rn { get; set; }

        [DisplayName("登録者")]
        public string ins_user { get; set; }

        [DisplayName("更新者")]
        public string upd_user { get; set; }
    }
}