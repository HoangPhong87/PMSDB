#region License
/// <copyright file="PMS04001EditViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2015/01/21</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS04001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.PMS04001;

    public class PMS04001EditViewModel
    {
        public PhasePlus PhaseInfo { get; set; }

        public bool DeleteFlag
        {
            get { return (PhaseInfo.del_flg == Constant.DeleteFlag.DELETE); }
            set { PhaseInfo.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool OLD_DEL_FLAG { get; set; }

        public bool EstimateTargetFlag
        {
            get { return (PhaseInfo.estimate_target_flg == "1"); }
            set { PhaseInfo.estimate_target_flg = (value ? "1" : "0"); }
        }

        public PMS04001EditViewModel()
        {
            PhaseInfo = new PhasePlus();
        }
    }
}
