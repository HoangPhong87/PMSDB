#region License
/// <copyright file="ContractTypePlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS03001
{
    /// <summary>
    /// m_contract_type plus model class
    /// </summary>
    [Serializable]
    public class ContractTypePlus : ContractType
    {
        public int peta_rn { get; set; }

        [DisplayName("登録者")]
        public string ins_user { get; set; }

        [DisplayName("更新者")]
        public string upd_user { get; set; }
    }
}