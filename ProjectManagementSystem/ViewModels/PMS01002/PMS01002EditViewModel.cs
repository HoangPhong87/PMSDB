using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Models.Entities;
using ProjectManagementSystem.Models.PMS01002;

namespace ProjectManagementSystem.ViewModels.PMS01002
{
    public class PMS01002EditViewModel
    {   
        public UserPlus USER_INFO { get; set; }

        public string OLD_USER_ID { get; set; }

        public string Clear { get; set; }

        public string TypeUpload { get; set; }

        /// <summary>
        /// </summary>
        public IList<SelectListItem> GROUP_LIST { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<SelectListItem> POSITION_LIST { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<SelectListItem> AUTHORITYROLE_LIST { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<SelectListItem> BRANCH_LIST { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<UnitPriceHistory> BASE_UNIT_COST_LIST { get; set; }

        public PMS01002EditViewModel()
        {
            USER_INFO = new UserPlus();
        }

        public bool Password_Lock
        {
            get { return (USER_INFO.password_lock_flg == Constant.PasswordLockFlag.LOCK); }
            set { USER_INFO.password_lock_flg = (value ? Constant.PasswordLockFlag.LOCK : Constant.PasswordLockFlag.NON_LOCK); }
        }

        public bool Delete
        {
            get { return (USER_INFO.del_flg == Constant.DeleteFlag.DELETE); }
            set { USER_INFO.del_flg = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public bool Temporary_Retirement
        {
            get { return (USER_INFO.temporary_retirement_flg == Constant.Temporary_Retirement_Flg.RETIREMENT); }
            set { USER_INFO.temporary_retirement_flg = (value ? Constant.Temporary_Retirement_Flg.RETIREMENT : Constant.Temporary_Retirement_Flg.NON_RETIREMENT); }
        }

        public bool OLD_DEL_FLAG { get; set; }

        public int data_editable_time { get; set; }
    }
}