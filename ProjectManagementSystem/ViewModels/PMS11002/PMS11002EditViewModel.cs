#region License
/// <copyright file="PMS11002EditViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author></author>
/// <createdDate>2017/07/04</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS11002
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS11002;

    public class PMS11002EditViewModel
    {
        public BudgetPlus BudgetInfo { get; set; }

        public bool DeleteFlag
        {
            get;set;
           // get { return (BudgetInfo.del_flg == Constant.DeleteFlag.DELETE); }
           // set { BudgetInfo.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool OLD_DEL_FLAG { get; set; }

        public PMS11002EditViewModel()
        {
            BudgetInfo = new BudgetPlus();
        }
    }
}
