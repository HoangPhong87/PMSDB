#region License
/// <copyright file="PMS11001EditViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HungLQ</author>
/// <createdDate>2017/06/28</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS11001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS11001;

    public class PMS11001EditViewModel
    {
        public BranchPlus BranchInfo { get; set; }

        public bool DeleteFlag
        {
            get { return (BranchInfo.del_flg == Constant.DeleteFlag.DELETE); }
            set { BranchInfo.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool OLD_DEL_FLAG { get; set; }

        public PMS11001EditViewModel()
        {
            BranchInfo = new BranchPlus();
        }
    }
}
