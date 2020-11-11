#region License
/// <copyright file="PMS03001EditViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/26</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS03001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS03001;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class PMS03001EditViewModel
    {
        public ContractTypePlus ContractTypeInfo { get; set; }

        public IList<ContractTypePhasePlus> CONTRACT_TYPE_PHASE_LIST { get; set; }

        public IList<ContractTypeCategoryPlus> CONTRACT_TYPE_CATEGORY_LIST { get; set; }

        public IList<SelectListItem> PHASE_LIST { get; set; }

        public IList<SelectListItem> CATEGORY_LIST { get; set; }

        public bool ChargeOfSalesFlag
        {
            get { return (ContractTypeInfo.charge_of_sales_flg == "1"); }
            set { ContractTypeInfo.charge_of_sales_flg = (value ? "1" : "0"); }
        }

        public bool ExceptionCalculateFlag
        {
            get { return (ContractTypeInfo.exceptional_calculate_flg == "1"); }
            set { ContractTypeInfo.exceptional_calculate_flg = (value ? "1" : "0"); }
        }

        public bool BudgetSettingFlag
        {
            get { return (ContractTypeInfo.budget_setting_flg == "1"); }
            set { ContractTypeInfo.budget_setting_flg = (value ? "1" : "0"); }
        }

        public bool DeleteFlag
        {
            get { return (ContractTypeInfo.del_flg == Constant.DeleteFlag.DELETE); }
            set { ContractTypeInfo.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool CheckPlanFlag
        {
            get { return (ContractTypeInfo.check_plan_flg == "1"); }
            set { ContractTypeInfo.check_plan_flg = (value ? "1" : "0"); }
        }

        public bool CheckProgressFlag
        {
            get { return (ContractTypeInfo.check_progress_flg == "1"); }
            set { ContractTypeInfo.check_progress_flg = (value ? "1" : "0"); }
        }

        public bool CheckPeriodFlag
        {
            get { return (ContractTypeInfo.check_period_flg == "1"); }
            set { ContractTypeInfo.check_period_flg = (value ? "1" : "0"); }
        }

        public bool CheckSalesFlag
        {
            get { return (ContractTypeInfo.check_sales_flg == "1"); }
            set { ContractTypeInfo.check_sales_flg = (value ? "1" : "0"); }
        }

        public bool OLD_DEL_FLAG { get; set; }

        public PMS03001EditViewModel()
        {
            ContractTypeInfo = new ContractTypePlus();
        }
    }
}
