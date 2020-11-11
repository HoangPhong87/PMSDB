#region Licence
// //--------------------------------------------------------------------------------------------------------------------
// //<copyright file="PMS06002ListViewModel.cs" company="i-Enter Asia">
// // Copyright © 2014 i-Enter Asia. All rights reserved.
// //</copyright>
// //<project>Project Management System</project>
// //<author>Nguyen Minh Hien</author>
// //<email>hiennm@live.com</email>
// //<createdDate>20-05-2014</createdDate>
// //<summary>
// // TODO: Update summary.
// //</summary>
// //--------------------------------------------------------------------------------------------------------------------
#endregion
namespace ProjectManagementSystem.ViewModels.PMS06002
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using ProjectManagementSystem.Models.PMS06002;

    /// <summary>
    /// ViewModel class for the ActualWorkList screen
    /// </summary>
    public class PMS06002ListViewModel
    {
        /// <summary>
        /// Search condition of the displayed list
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// Text label to display the current range of months
        /// </summary>
        public string CurrentMonthRange { get; set; }

        /// <summary>
        /// List of groups
        /// </summary>
        public IList<SelectListItem> GroupList { get; set; }

        /// <summary>
        /// List of groups
        /// </summary>
        public IList<SelectListItem> BranchList { get; set; }

        /// <summary>
        /// List of contract types
        /// </summary>
        public IList<SelectListItem> ContractTypeList { get; set; }

        /// <summary>
        /// List of phases
        /// </summary>
        public IList<PhaseInContractType> PhaseList { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PMS06002ListViewModel()
        {
            Condition = new Condition();
        }

    }
}