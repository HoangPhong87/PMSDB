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

    public class PMS06001EditViewModel
    {
        public ProjectInfoPlus PROJECT_INFO { get; set; }

        public bool delete_flag
        {
            get { return (PROJECT_INFO.del_flg == Constant.DeleteFlag.DELETE); }
            set { PROJECT_INFO.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool definite_assign_date { get; set; }

        public bool old_definite_assign_date { get; set; }

        public IList<SelectListItem> CATEGORY_LIST { get; set; }

        public IList<SelectListItem> RANK_LIST { get; set; }

        public IList<SelectListItem> TAG_LIST { get; set; }

        public IList<SelectListItem> OVERHEAD_COST_TYPE_LIST { get; set; }

        public IList<Status> STATUS_LIST { get; set; }

        public IList<ContractType> CONTRACT_TYPE_LIST { get; set; }

        public IList<Rank> RANK_TABLE { get; set; }

        public IList<ProgressHistoryPlus> PROGRESS_LIST { get; set; }

        public IList<PhasePlus> PHASE_LIST { get; set; }

        public List<TargetCategoryPlus> TARGET_CATEGORY_LIST { get; set; }

        public IList<SalesPaymentPlus> OUTSOURCER_LIST { get; set; }

        public SalesPaymentPlus OUTSOURCER { get; set; }

        public IList<MemberAssignmentDetailPlus> MEMBER_ASSIGNMENT_DETAIL_LIST { get; set; }

        public IList<MemberAssignmentPlus> MEMBER_ASSIGNMENT_LIST { get; set; }

        public IList<SalesPaymentPlus> SUBCONTRACTOR_LIST { get; set; }

        public IList<SalesPaymentDetailPlus> PAYMENT_DETAIL_LIST { get; set; }

        public IList<OverheadCostPlus> OVERHEAD_COST_LIST { get; set; }

        public IList<OverheadCostDetailPlus> OVERHEAD_COST_DETAIL_LIST { get; set; }

        public IList<ProjectAttachFilePlus> FILE_LIST { get; set; }

        public IList<SelectListItem> DEFAULT_CATEGORY_LIST { get; set; }

        public bool IS_CREATE_COPY { get; set; }

        public int COPY_TYPE { get; set; }

        public int PRJ_SYS_ID_TO_COPY { get; set; }

        public bool IS_CHANGE_HISTORY { get; set; }

        public bool IS_UPDATE_ASSIGN_DATE { get; set; }

        public bool SELECT_ALL_PHASES { get; set; }

        public string isNotBack { get; set; }

        public PMS06001EditViewModel()
        {
            PROJECT_INFO = new ProjectInfoPlus();
            OUTSOURCER_LIST = new List<SalesPaymentPlus>();
        }
    }
}
