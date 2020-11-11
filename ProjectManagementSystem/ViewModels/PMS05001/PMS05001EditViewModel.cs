#region License
/// <copyright file="PMS05001EditViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/22</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS05001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS05001;

    public class PMS05001EditViewModel
    {
        public GroupPlus GroupInfo { get; set; }

        public bool DeleteFlag
        {
            get { return (GroupInfo.del_flg == Constant.DeleteFlag.DELETE); }
            set { GroupInfo.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool BudgetSettingFlag
        {
            get { return (GroupInfo.budget_setting_flg == "1"); }
            set { GroupInfo.budget_setting_flg = (value ? "1" : "0"); }
        }

        public bool OLD_DEL_FLAG { get; set; }

        public bool CheckActualWorkFlag
        {
            get { return (GroupInfo.check_actual_work_flg == "1"); }
            set { GroupInfo.check_actual_work_flg = (value ? "1" : "0"); }
        }

        public PMS05001EditViewModel()
        {
            GroupInfo = new GroupPlus();
        }
    }
}
