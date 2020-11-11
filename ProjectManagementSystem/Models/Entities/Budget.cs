#region License
/// <copyright file="Budget.cs" company="i-Enter Asia">
/// Copyright © 2017 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

using ProjectManagementSystem.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models.Entities
{
    [PetaPoco.TableName("m_budget")]
    [PetaPoco.PrimaryKey("company_code", autoIncrement = false)]
    [Serializable]
    public class Budget
    {
        public string company_code { get; set; }
        public string group_id { get; set; }
        public string contract_type_id { get; set; }
        public int target_month { get; set; }
        public int target_year { get; set; }
        public decimal? sales_budget { get; set; }
        public decimal? profit_budget { get; set; }
        [DisplayName("登録日時")]
        public DateTime? ins_date { get; set; }

        public int ins_id { get; set; }

        [DisplayName("更新日時")]
        public DateTime? upd_date { get; set; }

        public int upd_id { get; set; }

        [DisplayName("削除")]
        public string del_flg { get; set; }
    }
}