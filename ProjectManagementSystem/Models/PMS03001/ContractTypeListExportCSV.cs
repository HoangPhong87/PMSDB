#region License
/// <copyright file="ContractTypeListExportCSV.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

namespace ProjectManagementSystem.Models.PMS03001
{
    /// <summary>
    /// ContractTypeListExportCSV model class
    /// </summary>
    public class ContractTypeListExportCSV
    {
        public string peta_rn { get; set; }

        public string contract_type { get; set; }

        public string charge_of_sales_flg { get; set; }

        public string exceptional_calculate_flg { get; set; }
        public string budget_setting_flg { get; set; }

        public string display_order { get; set; }

        public string remarks { get; set; }

        public string upd_date { get; set; }

        public string upd_user { get; set; }
    }
}