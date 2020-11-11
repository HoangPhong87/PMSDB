#region License
/// <copyright file="PMS06001EditViewModel.cs" company="i-Enter Asia">
/// Copyright © 2014 i-Enter Asia. All rights reserved.
/// </copyright>
/// <project>Project Management System</project>
/// <author>HoangPS</author>
/// <createdDate>2014/05/08</createdDate>
#endregion

namespace ProjectManagementSystem.ViewModels.PMS06001
{
    using ProjectManagementSystem.Common;
    using ProjectManagementSystem.Models.Entities;
    using ProjectManagementSystem.Models.PMS06001;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class PMS06001EditPlanViewModel
    {
        public ProjectPlanInfoPlus PROJECT_PLAN_INFO { get; set; }

        public bool DELETE_FLAG
        {
            get { return (PROJECT_PLAN_INFO.del_flg == Constant.DeleteFlag.DELETE); }
            set { PROJECT_PLAN_INFO.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool SUPPORT_TEST_PLAN_FLG
        {
            get { return (PROJECT_PLAN_INFO.support_test_plan_flg == Constant.SuportFlag.REQUIRED); }
            set { PROJECT_PLAN_INFO.support_test_plan_flg = (value ? Constant.SuportFlag.REQUIRED : Constant.SuportFlag.NON_REQUIRED); }
        }
        public bool SUPPORT_USER_TEST_FLG
        {
            get { return (PROJECT_PLAN_INFO.support_user_test_flg == Constant.SuportFlag.REQUIRED); }
            set { PROJECT_PLAN_INFO.support_user_test_flg = (value ? Constant.SuportFlag.REQUIRED : Constant.SuportFlag.NON_REQUIRED); }
        }

        public bool SUPPORT_STRESS_TEST_FLG
        {
            get { return (PROJECT_PLAN_INFO.support_stress_test_flg == Constant.SuportFlag.REQUIRED); }
            set { PROJECT_PLAN_INFO.support_stress_test_flg = (value ? Constant.SuportFlag.REQUIRED : Constant.SuportFlag.NON_REQUIRED); }
        }
        public bool SUPPORT_SECURITY_TEST_FLG
        {
            get { return (PROJECT_PLAN_INFO.support_security_test_flg == Constant.SuportFlag.REQUIRED); }
            set { PROJECT_PLAN_INFO.support_security_test_flg = (value ? Constant.SuportFlag.REQUIRED : Constant.SuportFlag.NON_REQUIRED); }
        }

        public string isNotBack { get; set; }

        public PMS06001EditPlanViewModel()
        {
            PROJECT_PLAN_INFO = new ProjectPlanInfoPlus();
        }
    }
}
