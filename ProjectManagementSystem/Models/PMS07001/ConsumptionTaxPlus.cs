#region License
/// <copyright file="ConsumptionTaxPlus.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/23</createdDate>
#endregion

using ProjectManagementSystem.Models.Entities;
using System;
using System.ComponentModel;

namespace ProjectManagementSystem.Models.PMS07001
{
    /// <summary>
    /// m_consumption_tax plus model class
    /// </summary>
    [Serializable]
    public class ConsumptionTaxPlus : ConsumptionTax
    {
        public int peta_rn { get; set; }

        [DisplayName("登録者")]
        public string ins_user { get; set; }

        public DateTime? old_apply_start_date { get; set; }
    }
}